using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using vision;
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
        public int CameraIndex;

        public CameraControl(Form1 form)
        {
            try
            {
                this.form = form;
                m_Cmaera_api = new CStApiAutoInit();
                if (m_Camera_system == null)
                {
                    while (true)
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
            catch(Exception ex)
			{
                form.ShowMessage("오류", "제공된 매개변수가 유효하지 않거나 범위를 벗어났습니다!\n" + ex.Message, "경고");
            }
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
                //form.ShowMessage("오류", "카메라 연결에 실패하였습니다!\n" + ex.Message, "경고");
                m_Camera_device = null;
            }
        }
        private void open()
        {
            try
			{
                m_Camera_lastBuffer = CStApiDotNet.CreateStImageBuffer();
                m_Camera_lastBuffer.CreateBuffer((uint)Width, (uint)Height, (eStPixelFormatNamingConvention)Enum.Parse(typeof(eStPixelFormatNamingConvention), PixelFormat));

                m_Camera_lastColorBuffer = CStApiDotNet.CreateStImageBuffer();
                m_Camera_lastColorBuffer.CreateBuffer((uint)Width, (uint)Height, eStPixelFormatNamingConvention.RGB8);
            }
            catch(Exception ex)
			{
                form.ShowMessage("오류", "카메라 오픈에 실패하였습니다!\n" + ex.Message, "경고");
            }
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
                form.ShowMessage("오류", "시작 실패 : 카메라의 상태를 확인해주세요!\n" + ex.Message, "경고");
            }
        }
        public void Stop()
        {
            try
            {
                if (m_Camera_device != null) m_Camera_device.AcquisitionStop();
                if (m_Camera_DataStream != null) m_Camera_DataStream.StopAcquisition();
            }
            catch (Exception ex)
            {
				//form.ShowMessage("오류", "종료 실패 : 카메라의 상태를 확인해주세요!\n" + ex.Message, "경고");
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
                    //form.ShowMessage("종료", "카메라의 연결이 원활하지 않습니다!\n" + ex.Message, "주의");
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
				if(m_Camera_device != null)
				{
                    try
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
                    catch (Exception ex)
                    {
                        form.ShowMessage("오류", "잘못된 ColorBuffer 참조입니다!\n" + ex, "주의");
                        return m_Camera_lastColorBuffer.GetIStImage().GetByteArray();
                    }
                }
                else return m_Camera_lastColorBuffer.GetIStImage().GetByteArray();
            }
		}
        public void SetIP(string ipvalue)
		{
            if (ipvalue.Equals(GevDeviceIPAddress.ToString())) return;

            try
			{
                CStApiAutoInit now_Cmaera_api = m_Cmaera_api;
                CStSystem now_Camera_system = m_Camera_system;
                CStDevice now_Camera_device = m_Camera_device;
                CStDataStream now_Camera_DataStream = m_Camera_DataStream;
                IStInterface iInterface = m_Camera_system.GetIStInterface(0);
                if (iInterface == null)
                {
                    form.ShowMessage("오류", "인터페이스를 읽어오는데 실패했습니다!\n카메라의 IP를 바꿀 수 없습니다!\n", "주의");
                    return;
                }
                INodeMap nodeMap = iInterface.GetIStPort().GetINodeMap();
                nodeMap.GetNode<IInteger>("DeviceSelector").Value = CameraIndex;
                //INodeMap nodeMap = m_Camera_device.GetLocalIStPort().GetINodeMap();
                // 호스트 측의 IP 주소를 표시합니다
                IInteger nodeGevInterfaceSubnetIPAddress = nodeMap.GetNode<IInteger>("GevInterfaceSubnetIPAddress");

                // 호스트 측의 서브넷 마스크를 표시합니다
                IInteger nodeGevInterfaceSubnetMask = nodeMap.GetNode<IInteger>("GevInterfaceSubnetMask");

                // 카메라의 현재 IP 주소를 표시합니다
                //IInteger nodeGevDeviceIPAddress = nodeMap.GetNode<IInteger>("GevDeviceIPAddress");
                //Console.WriteLine("Device IP Address=" + nodeGevDeviceIPAddress.ToString());

                // 카메라의 현재 서브넷 마스크를 표시합니다
                //IInteger nodeGevDeviceSubnetMask = nodeMap.GetNode<IInteger>("GevDeviceSubnetMask");
                //Console.WriteLine("Device Subnet Mask=" + nodeGevDeviceSubnetMask.ToString());

                IPAddress ipaddr;
                if (!IPAddress.TryParse(ipvalue.Trim(), out ipaddr))
                {
                    form.ShowMessage("오류", "잘못된 IP 주소입니다!\n", "주의");
                    return;
                }

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
                    Stop();
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
                    //Close();
                    //m_Camera_device.GetLocalIStPort().GetINodeMap().GetNode<IInteger>("GevDeviceIPAddress").Value = newDeviceIPAddress;
                    // 카메라의 새 IP 주소를 지정합니다 이 시점에서 카메라 설정은 업데이트되지 않습니다
                    IInteger NowIPAddress = nodeMap.GetNode<IInteger>("GevDeviceForceIPAddressReg");
                    NowIPAddress.Value = newDeviceIPAddress;

                    // 카메라의 새 서브넷 마스크를 지정합니다 이 시점에서 카메라 설정은 업데이트되지 않습니다
                    //IInteger nodeGevDeviceForceSubnetMask = nodeMap.GetNode<IInteger>("GevDeviceForceSubnetMask");
                    //nodeGevDeviceForceSubnetMask.Value = subnetMask;

                    // 카메라 설정을 업데이트합니다
                    ICommand nodeGevDeviceForceIP = nodeMap.GetNode<ICommand>("GevDeviceForceIP");
                    nodeGevDeviceForceIP.Execute();
                }
                else
                {
                    form.ShowMessage("오류", "잘못된 IP입니다!", "주의");
                    return;
                }
                m_Cmaera_api = now_Cmaera_api;
                m_Camera_system = now_Camera_system;
                m_Camera_device = now_Camera_device;
                m_Camera_DataStream = now_Camera_DataStream;
                CameraOpen();
                SetEnableImageCallback(true);
                Start();
                //form.m_thread[form.NowSelectedCamNo - 1].Start();
            }
            catch(Exception ex)
            {
                form.ShowMessage("오류", "IP를 변경하던 중 오류가 발생했습니다!\n" + ex, "주의");
                return;
            }
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
        public IFloat DeviceFrame
        {
            get { return m_Camera_device.GetRemoteIStPort().GetINodeMap().GetNode<IFloat>("AcquisitionFrameRate"); }
        }
        public string SetDeviceIPAddress
		{
            set { SetIP(value); }
		}
        public int SetCameraIndex
		{
			set { CameraIndex = value; }
		}
    }
}
