using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using FS_MCS500POE_CameraViewer;
using Sentech.GenApiDotNET;
using Sentech.StApiDotNET;
using System.Net;

namespace OMRON_Camera_Control
{
	class CameraControl
	{
        Form1 form;
        private CStApiAutoInit m_Cmaera_api = null;
        private static CStSystem m_Camera_system = null;
        private CStDevice m_Camera_device = null;
        private CStDataStream m_Camera_DataStream = null;

        private IStImageBuffer m_Camera_lastBuffer = null;
        private IStImageBuffer m_Camera_lastColorBuffer = null;
        public IStImageBuffer m_Camera_RotateBuffer = null;
        public IStImageBuffer m_Camera_ReverseBuffer = null;
        private EventWaitHandle m_Camera_grabDone = null;

        public CameraControl(Form1 form)
        {
            this.form = form;
            m_Cmaera_api = new CStApiAutoInit();
            if (m_Camera_system == null)
            {
                while(true)
				{
                    m_Camera_system = null;
                    m_Camera_system = new CStSystem(eStSystemVendor.Default, eStInterfaceType.GigEVision);
                    if (m_Camera_system.GetIStInterface(0).GetIStPort().GetINodeMap().GetNode<IInteger>("GevDeviceForceIPAddress").AccessMode != eAccessMode.RW)
                        continue;
                    else break;
				}
            }
            m_Camera_grabDone = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        ~CameraControl()
        {
            Close();

            if (m_Camera_system != null)
            {
                m_Camera_system.Dispose();
                m_Camera_system = null;
            }

            if (m_Cmaera_api != null)
            {
                m_Cmaera_api.Dispose();
                m_Cmaera_api = null;
            }
        }
        public void CameraOpen()
		{
            try
			{
                m_Camera_device = m_Camera_system.CreateFirstStDevice();
                if (m_Camera_device == null) return;
                open();
            }
            catch(Exception ex)
            {
                form.ShowMessage("오류", "카메라 연결에 실패하였습니다!\n" + ex.Message, "경고");
                m_Camera_device = null;
            }
        }
        private void open()
        {
            //if (m_Camera_device == null) throw new Exception("The device is not created.");

            m_Camera_lastBuffer = CStApiDotNet.CreateStImageBuffer();
            m_Camera_lastBuffer.CreateBuffer((uint)Width, (uint)Height, (eStPixelFormatNamingConvention)Enum.Parse(typeof(eStPixelFormatNamingConvention), PixelFormat));

            m_Camera_lastColorBuffer = CStApiDotNet.CreateStImageBuffer();
            m_Camera_lastColorBuffer.CreateBuffer((uint)Width, (uint)Height, eStPixelFormatNamingConvention.RGB8);

            //m_Camera_RotateBuffer = CStApiDotNet.CreateStImageBuffer();
            //m_Camera_RotateBuffer.CreateBuffer((uint)Height, (uint)Width, (eStPixelFormatNamingConvention)Enum.Parse(typeof(eStPixelFormatNamingConvention), PixelFormat));

            //m_Camera_ReverseBuffer = CStApiDotNet.CreateStImageBuffer();
            //m_Camera_ReverseBuffer.CreateBuffer((uint)Width, (uint)Height, (eStPixelFormatNamingConvention)Enum.Parse(typeof(eStPixelFormatNamingConvention), PixelFormat));

        }
        public void Close()
        {
            try
            {
                if (m_Camera_lastBuffer != null) m_Camera_lastBuffer = null;
                if (m_Camera_lastColorBuffer != null) m_Camera_lastColorBuffer = null;
                if (m_Camera_RotateBuffer != null) m_Camera_RotateBuffer = null;
                if (m_Camera_ReverseBuffer != null) m_Camera_ReverseBuffer = null;

                if (m_Camera_DataStream != null)
                {
                    m_Camera_DataStream.Dispose();
                    m_Camera_DataStream = null;
                }

                if (m_Camera_device != null)
                {
                    m_Camera_device.Dispose();
                    m_Camera_device = null;
                }
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(exc.StackTrace);
                //Trace.WriteLine(exc.Message);
                form.ShowMessage("종료", "카메라의 상태를 확인해주세요!\n" + ex.Message, "경고");
            }
        }
        public void Start()
        {
            try
            {
                m_Camera_DataStream.StartAcquisition();
                m_Camera_device.AcquisitionStart();
            }
            catch(Exception ex)
			{
                form.ShowMessage("종료", "시작 실패 : 카메라의 상태를 확인해주세요!\n" + ex.Message, "경고");
            }
        }
        public void Stop()
        {
            try
            {
                m_Camera_device.AcquisitionStop();
                m_Camera_DataStream.StopAcquisition();
            }
            catch (Exception ex)
            {
                form.ShowMessage("종료", "종료 실패 : 카메라의 상태를 확인해주세요!\n" + ex.Message, "경고");
            }
        }
        public void SetEnableImageCallback(bool value)
        {
            if (m_Camera_DataStream != null)
            {
                m_Camera_DataStream.Dispose();
                m_Camera_DataStream = null;
            }
            m_Camera_DataStream = m_Camera_device.CreateStDataStream(0);
            if (m_Camera_DataStream == null)
            {
                //Console.WriteLine("Camera가 연결 되어 있지 않습니다.");
                form.ShowMessage("오류", "데이터 스트림을 만드는데 실패했습니다!", "주의");
                return;
			}
            if (value == true)
            {
                m_Camera_DataStream.RegisterCallbackMethod(OnCallback);
            }
        }
        private void OnCallback(IStCallbackParamBase paramBase, object[] param)
        {
            // Check callback type. Only NewBuffer event is handled in here.
            if (paramBase.CallbackType == eStCallbackType.TL_DataStreamNewBuffer)
            {
                // In case of receiving a NewBuffer events:
                // Convert received callback parameter into IStCallbackParamGenTLEventNewBuffer for acquiring additional information.
                IStCallbackParamGenTLEventNewBuffer callbackParam = paramBase as IStCallbackParamGenTLEventNewBuffer;

                try
                {
                    // Get the IStDataStream interface object from the received callback parameter.
                    IStDataStream dataStream = callbackParam.GetIStDataStream();

                    // Retrieve the buffer of image data for that callback indicated there is a buffer received.
                    using (CStStreamBuffer streamBuffer = dataStream.RetrieveBuffer(0))
                    {
                        // Check if the acquired data contains image data.
                        if (streamBuffer.GetIStStreamBufferInfo().IsImagePresent)
                        {
                            // If yes, we create a IStImage object for further image handling.
                            m_Camera_lastBuffer.CopyImage(streamBuffer.GetIStImage());
                            m_Camera_grabDone.Set();
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Trace.WriteLine(exc.StackTrace);
                    //Trace.WriteLine(exc.Message);
                    form.ShowMessage("종료", "카메라의 연결이 원활하지 않습니다!\n" + ex.Message, "주의");
                }
            }
            else
            {
                string strError = "";
                switch(paramBase.CallbackType)
                {
                    case eStCallbackType.TL_SystemError: strError = "TL_SystemError";
                        break;
                    case eStCallbackType.TL_InterfaceError: strError = "TL_InterfaceError";
                        break;
                    case eStCallbackType.TL_DeviceError: strError = "TL_DeviceError";
                        break;
                    case eStCallbackType.TL_DataStreamError: strError = "TL_DataStreamError";
                        break;
                    case eStCallbackType.TL_StreamBufferError: strError = "TL_StreamBufferError";
                        break;
                    case eStCallbackType.IP_VideoFilerOpen: strError = "IP_VideoFilerOpen";
                        break;
                    case eStCallbackType.IP_VideoFilerClose: strError = "IP_VideoFilerClose";
                        break;
                    case eStCallbackType.IP_VideoFilerError: strError = "IP_VideoFilerError";
                        break;
                    case eStCallbackType.GUI_DisplayImageWndDrawing: strError = "GUI_DisplayImageWndDrawing";
                        break;
                    case eStCallbackType.GUI_WndCreate: strError = "GUI_WndCreate";
                        break;
                    case eStCallbackType.GUI_WndClose: strError = "GUI_WndClose";
                        break;
                    case eStCallbackType.GUI_WndDestroy: strError = "GUI_WndDestroy";
                        break;
                    case eStCallbackType.GUI_WndError: strError = "GUI_WndError";
                        break;
                    case eStCallbackType.Count: strError = "Count";
                        break;
                }
                form.ShowMessage("오류", "잘못된 CallBack 수신입니다!\n" + strError, "주의");
            }
        }
        public byte[] ColorBuffer
        {
            get
            {
                using (CStImageBuffer imageBuffer = CStApiDotNet.CreateStImageBuffer())
                using (CStPixelFormatConverter pixelFormatConverter = new CStPixelFormatConverter())
                {
                    imageBuffer.CreateBuffer((uint)Width, (uint)Height, eStPixelFormatNamingConvention.BGR8);
                    pixelFormatConverter.DestinationPixelFormat = eStPixelFormatNamingConvention.BGR8;
                    pixelFormatConverter.BayerInterpolationMethod = eStBayerInterpolationMethod.NearestNeighbor2;
                    pixelFormatConverter.Convert(m_Camera_lastBuffer.GetIStImage(), imageBuffer);

                    m_Camera_lastColorBuffer.CopyImage(imageBuffer.GetIStImage());
                    return m_Camera_lastColorBuffer.GetIStImage().GetByteArray();
                }
            }
        }
        public void SetIP(string ipvalue)
		{
            CStApiAutoInit now_Cmaera_api = m_Cmaera_api;
            CStSystem now_Camera_system = m_Camera_system;
            CStDevice now_Camera_device = m_Camera_device;
            CStDataStream now_Camera_DataStream = m_Camera_DataStream;
            IStInterface iInterface = null;

            for (uint i = 0; i < m_Camera_system.InterfaceCount; i++)
            {
                if (0 < m_Camera_system.GetIStInterface(i).DeviceCount)
                {
                    iInterface = m_Camera_system.GetIStInterface(i);
                    break;
                }
            }
            if (iInterface == null)
            {
                throw new RuntimeException("There is no device.");
            }
            INodeMap nodeMap = iInterface.GetIStPort().GetINodeMap();

            // 호스트 측의 IP 주소를 표시합니다
            IInteger nodeGevInterfaceSubnetIPAddress = nodeMap.GetNode<IInteger>("GevInterfaceSubnetIPAddress");
            //Console.WriteLine("Interface IP Address=" + nodeGevInterfaceSubnetIPAddress.ToString());

            // 호스트 측의 서브넷 마스크를 표시합니다
            IInteger nodeGevInterfaceSubnetMask = nodeMap.GetNode<IInteger>("GevInterfaceSubnetMask");
            //Console.WriteLine("Interface Subnet Mask=" + nodeGevInterfaceSubnetMask.ToString());

            // 첫 번째 카메라를 선택합니다
            //const uint deviceSelectorValue = 0;
            //IInteger nodeDeviceSelector = nodeMap.GetNode<IInteger>("DeviceSelector");
            //nodeDeviceSelector.Value = deviceSelectorValue;

            // 카메라의 현재 IP 주소를 표시합니다
            IInteger nodeGevDeviceIPAddress = nodeMap.GetNode<IInteger>("GevDeviceIPAddress");
            //Console.WriteLine("Device IP Address=" + nodeGevDeviceIPAddress.ToString());

            // 카메라의 현재 서브넷 마스크를 표시합니다
            IInteger nodeGevDeviceSubnetMask = nodeMap.GetNode<IInteger>("GevDeviceSubnetMask");
            //Console.WriteLine("Device Subnet Mask=" + nodeGevDeviceSubnetMask.ToString());

            IPAddress ipaddr;
            IPAddress.TryParse(ipvalue.Trim(), out ipaddr);

            // 새 IP 주소 문자열을 32비트 숫자로 변환합니다
            byte[] bytes = ipaddr.GetAddressBytes();
            uint newDeviceIPAddress = (uint)(IPAddress.NetworkToHostOrder(BitConverter.ToUInt32(bytes, 0)) >> 32);

            // 호스트 측의 서브넷 마스크를 가져옵니다
            uint subnetMask = (uint)nodeGevInterfaceSubnetMask.Value;

            // 호스트 측의 IP 주소를 가져옵니다
            uint interfaceIPAddress = (uint)nodeGevInterfaceSubnetIPAddress.Value;

            // 호스트와 카메라의 서브넷 주소가 일치하고 카메라의 호스트와 IP 주소가 서로 다른지 확인합니다
            if (((interfaceIPAddress & subnetMask) == (newDeviceIPAddress & subnetMask)) && (interfaceIPAddress != newDeviceIPAddress))
            {
                // 카메라의 새 IP 주소를 지정합니다 이 시점에서 카메라 설정은 업데이트되지 않습니다
                IInteger nodeGevDeviceForceIPAddress = nodeMap.GetNode<IInteger>("GevDeviceForceIPAddress");
                nodeGevDeviceForceIPAddress.Value = newDeviceIPAddress;

                // 카메라의 새 서브넷 마스크를 지정합니다 이 시점에서 카메라 설정은 업데이트되지 않습니다
                IInteger nodeGevDeviceForceSubnetMask = nodeMap.GetNode<IInteger>("GevDeviceForceSubnetMask");
                nodeGevDeviceForceSubnetMask.Value = subnetMask;

                // 카메라 설정을 업데이트합니다
                Stop();
                Close();
                ICommand nodeGevDeviceForceIP = nodeMap.GetNode<ICommand>("GevDeviceForceIP");
                nodeGevDeviceForceIP.Execute();
            }
            else
            {
                form.ShowMessage("오류", "잘못된 IP입니다!", "주의");
            }
            m_Cmaera_api = now_Cmaera_api;
            m_Camera_system = now_Camera_system;
            m_Camera_device = now_Camera_device;
            m_Camera_DataStream = now_Camera_DataStream;
            CameraOpen();
            Start();
        }
        public EventWaitHandle HandleGrabDone
        {
            get { return m_Camera_grabDone; }
        }
        public void OnResetEventGrabDone()
        {
            m_Camera_grabDone.Reset();
        }
        public bool IsOpened
        {
            get { return m_Camera_device != null; }
        }
        public long Width
        {
            get { return m_Camera_device.GetRemoteIStPort().GetINodeMap().GetNode<IInteger>("Width").Value; }
            set { m_Camera_device.GetRemoteIStPort().GetINodeMap().GetNode<IInteger>("Width").Value = value; }
        }
        public long Height
        {
            get { return m_Camera_device.GetRemoteIStPort().GetINodeMap().GetNode<IInteger>("Height").Value; }
            set { m_Camera_device.GetRemoteIStPort().GetINodeMap().GetNode<IInteger>("Height").Value = value; }
        }
        public string PixelFormat
        {
            get { return m_Camera_device.GetRemoteIStPort().GetINodeMap().GetNode<IEnum>("PixelFormat").StringValue; }
            set { m_Camera_device.GetRemoteIStPort().GetINodeMap().GetNode<IEnum>("PixelFormat").StringValue = value; }
        }
        public string DeviceModelName
        {
            get { return m_Camera_device.GetRemoteIStPort().GetINodeMap().GetNode<IString>("DeviceModelName").Value; }
        }
        public string DeviceSerialNumber
        {
            get { return m_Camera_device.GetLocalIStPort().GetINodeMap().GetNode<IString>("DeviceSerialNumber").Value; }
        }
        public string DeviceVersion
        {
            get { return m_Camera_device.GetLocalIStPort().GetINodeMap().GetNode<IString>("DeviceVersion").Value; }
        }
        public IInteger GevDeviceIPAddress
        {
            get { return m_Camera_device.GetLocalIStPort().GetINodeMap().GetNode<IInteger>("GevDeviceIPAddress"); }
            set {  }
        }
        public string SetDeviceIPAddress
		{
            set { SetIP(value); }
		}
    }
}
