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
            if (m_Camera_system == null) m_Camera_system = new CStSystem();
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
            m_Camera_DataStream.StartAcquisition();
            m_Camera_device.AcquisitionStart();
        }
        public void Stop()
        {
            m_Camera_device.AcquisitionStop();
            m_Camera_DataStream.StopAcquisition();
        }
        public void SetEnableImageCallback(bool value)
        {
            if (m_Camera_DataStream != null)
            {
                m_Camera_DataStream.Dispose();
                m_Camera_DataStream = null;
            }
            m_Camera_DataStream = m_Camera_device.CreateStDataStream(0);
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
        }
    }
}
