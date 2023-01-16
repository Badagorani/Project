using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace FS_MCS500POE_CameraViewer
{
	public partial class Property : Form
	{
		Form1 form;
		public Property_Setting property_setting = new Property_Setting();
		public Property(Form1 form)
		{
			InitializeComponent();
			this.form = form;
			XmlLoad();
			propertyGrid1.SelectedObject = property_setting;
		}
		private void Property_Close_Click(object sender, EventArgs e)
		{
			this.Hide();
		}
		private void PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
		{
			property_setting = (Property_Setting)propertyGrid1.SelectedObject;
			XmlSave(property_setting, property_setting.GetType());
		}
		public void XmlSave(object obj, Type type)
		{
			//XmlDocument xmlFile = new XmlDocument();
			//XmlNode root = xmlFile.CreateElement("전체");
			//xmlFile.AppendChild(root);

			//XmlNode userinfo = xmlFile.CreateElement("사용자_정보");
			//XmlNode username = xmlFile.CreateElement("사용자_이름");
			//username.InnerText = ps.m_sUser;
			//userinfo.AppendChild(username);
			//XmlNode userpermission = xmlFile.CreateElement("사용자_권한");
			//userpermission.InnerText = ps.m_sPermission;
			//userinfo.AppendChild(userpermission);
			//XmlNode UseStartTime = xmlFile.CreateElement("사용시작시간");
			//UseStartTime.InnerText = ps.m_sStartTime;
			//userinfo.AppendChild(UseStartTime);
			//XmlNode UseEndTime = xmlFile.CreateElement("사용종료시간");
			//UseEndTime.InnerText = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
			//userinfo.AppendChild(UseEndTime);
			//root.AppendChild(userinfo);

			//XmlNode camerainfo = xmlFile.CreateElement("카메라_정보");
			//XmlNode camera1ipaddress1 = xmlFile.CreateElement("카메라1_IP주소");
			////camera1ipaddress1.InnerText = "192.168.11111";
			//camera1ipaddress1.InnerText = form.Camera1IP;
			//camerainfo.AppendChild(camera1ipaddress1);
			//XmlNode camera1ipaddress2 = xmlFile.CreateElement("카메라2_IP주소");
			////camera1ipaddress2.InnerText = "192.168.22222";
			//camera1ipaddress2.InnerText = form.Camera2IP;
			//camerainfo.AppendChild(camera1ipaddress2);
			//XmlNode camera1ipaddress3 = xmlFile.CreateElement("카메라3_IP주소");
			////camera1ipaddress3.InnerText = "192.168.33333";
			//camera1ipaddress3.InnerText = form.Camera3IP;
			//camerainfo.AppendChild(camera1ipaddress3);
			//root.AppendChild(camerainfo);

			//xmlFile.Save(@"../../xml\Settings.xml");

			SetIPAddress();
			string strLocalFolder = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\"));
			string strXMLFile = String.Empty;

			if (obj.GetType() == property_setting.GetType())
				strXMLFile = @"\xmlFile\Settings.xml";

			if (!Directory.Exists(strLocalFolder + @"\xmlFile"))
				Directory.CreateDirectory(strLocalFolder + @"\xmlFile");

			using (StreamWriter wr = new StreamWriter(strLocalFolder + strXMLFile))
			{
				var ns = new XmlSerializerNamespaces();
				ns.Add(string.Empty, string.Empty);

				XmlSerializer xs = new XmlSerializer(type);
				xs.Serialize(wr, obj, ns);
			}
			XmlLoad();
			propertyGrid1.SelectedObject = property_setting;
		}
		public void XmlEndSave(object obj, Type type)
		{
			property_setting.m_sEndTime = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
			property_setting = (Property_Setting)propertyGrid1.SelectedObject;
			XmlSave(obj, type);
		}
		public void XmlLoad()
		{
			string strLocalFolder = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\"));
			string strXMLFile = String.Empty;

			try
			{
				strXMLFile = @"\xmlFile\Settings.xml";
				var serializer = new XmlSerializer(typeof(Property_Setting));
				using (var reader = XmlReader.Create(strLocalFolder + strXMLFile))
				{
					property_setting = (Property_Setting)serializer.Deserialize(reader);
					property_setting.m_sStartTime = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
					property_setting.m_sEndTime = "";
					property_setting.m_sCamera1_DeviceIP = form.Camera1IP;
					property_setting.m_sCamera2_DeviceIP = form.Camera2IP;
					property_setting.m_sCamera3_DeviceIP = form.Camera3IP;
				}
			}
			catch (Exception ex)
			{
				//Console.WriteLine($"An exception occurred from {MethodBase.GetCurrentMethod().Name}", ex);
				form.ShowMessage("오류", $"예외가 발생하였습니다! : {MethodBase.GetCurrentMethod().Name}\n" + ex, "주의");
			}
		}
		public void SetIPAddress()
		{
			form.Camera1IP = property_setting.m_sCamera1_DeviceIP;
			form.Camera2IP = property_setting.m_sCamera2_DeviceIP;
			form.Camera3IP = property_setting.m_sCamera3_DeviceIP;
		}
	}
	public class Property_Setting
	{
		//[Category("카테고리 없으면 기타로 들어감"), DisplayName("왼쪽의 이름"), Description("맨밑에 설명")]
		public string m_sUser;
		[Category("\t사용자 정보"), DisplayName("이름"), Description("사용자의 성함")]
		public string User
		{
			get { return m_sUser; }
			set { m_sUser = value; }
		}

		public string m_sPermission;
		[Category("\t사용자 정보"), DisplayName("권한"), Description("사용자에 따라 달라지는 권한")]
		public string Permission
		{
			get { return m_sPermission; }
			//set { m_sPermission = value; }
		}

		public string m_sStartTime;
		[Category("\t사용자 정보"), DisplayName("사용시작시간"), Description("사용을 시작한 시간")]
		public string StartTime
		{
			get { return m_sStartTime; }
			//set { m_sStartTime = value; }
		}

		public string m_sEndTime;
		[Category("\t사용자 정보"), DisplayName("사용종료시간"), Description("사용을 종료한 시간")]
		public string EndTime
		{
			get { return m_sEndTime; }
			//set { m_sEndTime = value; }
		}
		public string m_sCamera1_DeviceIP;
		[Category("카메라 정보"), DisplayName("카메라1 IP 주소"), Description("카메라에 설정된 IP")]
		public string Camera1_DeviceIP
		{
			get { return m_sCamera1_DeviceIP; }
			set { m_sCamera1_DeviceIP = value; }
		}

		public string m_sCamera2_DeviceIP;
		[Category("카메라 정보"), DisplayName("카메라2 IP 주소"), Description("카메라에 설정된 IP")]
		public string Camera2_DeviceIP
		{
			get { return m_sCamera2_DeviceIP; }
			set { m_sCamera2_DeviceIP = value; }
		}

		public string m_sCamera3_DeviceIP;
		[Category("카메라 정보"), DisplayName("카메라3 IP 주소"), Description("카메라에 설정된 IP")]
		public string Camera3_DeviceIP
		{
			get { return m_sCamera3_DeviceIP; }
			set { m_sCamera3_DeviceIP = value; }
		}
		//[Category("레이아웃"), DisplayName("가로 크기"), Description("센서로 인식하는 가로 크기")]
		//public int _iSensorWidth { get; }
		//[Category("레이아웃"), DisplayName("세로 크기"), Description("센서로 인식하는 세로 크기")]
		//public int _iSensorHeight { get; }
		public string m_MotorDriverComPort;
		[CategoryAttribute("드라이버 설정"),
		DisplayName("드라이버 RS-485 포트")]
		public string MotorDriverComPort
		{
			get { return m_MotorDriverComPort; }
			set { m_MotorDriverComPort = value; }
		}
		
		public string m_SensorDriverComPort;
		[CategoryAttribute("드라이버 설정"),
		DisplayName("센서 및 I/O 드라이버 포트")]
		public string SensorDriverComPort
		{
			get { return m_SensorDriverComPort; }
			set { m_SensorDriverComPort = value; }
		}
		
		public string m_BatteryComPort;
		[CategoryAttribute("드라이버 설정"),
		DisplayName("배터리 상태 확인 포트")]
		public string BatteryComPort
		{
			get { return m_BatteryComPort; }
			set { m_BatteryComPort = value; }
		}
		
		public string m_FrontLidarIP;
		[CategoryAttribute("드라이버 설정"),
		DisplayName("전방 라이다 IP")]
		public string FrontLidarIP
		{
			get { return m_FrontLidarIP; }
			set { m_FrontLidarIP = value; }
		}

		public string m_RearLidarIP;
		[CategoryAttribute("드라이버 설정"),
		DisplayName("후방 라이다 IP")]
		public string RearLidarIP
		{
			get { return m_RearLidarIP; }
			set { m_RearLidarIP = value; }
		}
		
		public string m_TinkerBoardIP;
		[CategoryAttribute("드라이버 설정"),
		DisplayName("Thinker Board2 IP")]
		public string TinkerBoardIP
		{
			get { return m_TinkerBoardIP; }
			set { m_TinkerBoardIP = value; }
		}
		
		public string m_RMS_ServerIP;
		[CategoryAttribute("드라이버 설정"),
		DisplayName("RMS 서버 IP")]
		public string RMS_ServerIP
		{
			get { return m_RMS_ServerIP; }
			set { m_RMS_ServerIP = value; }
		}
		
		public int m_DrivingVelocity;
		[CategoryAttribute("주행 파라메터"),
		DisplayName("기본 주행 속도(rpm)")]
		public int DrivingVelocity
		{
			get { return m_DrivingVelocity; }
			set { m_DrivingVelocity = value; }
		}

		public int m_SteeringVelocity;
		[CategoryAttribute("주행 파라메터"),
		DisplayName("기본 조향 속도(rpm)")]
		public int SteeringVelocity
		{
			get { return m_SteeringVelocity; }
			set { m_SteeringVelocity = value; }
		}
	}
}
