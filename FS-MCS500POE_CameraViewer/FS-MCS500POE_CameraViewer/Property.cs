using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace FS_MCS500POE_CameraViewer
{
	public partial class Property : Form
	{
		Form1 form;
		private Property_Setting ps;
		public Property(Form1 form)
		{
			InitializeComponent();
			this.form = form;
			ps = new Property_Setting(form);
			propertyGrid1.SelectedObject = ps;
		}
		private void Property_Close_Click(object sender, EventArgs e)
		{
			this.Hide();
		}
		private void PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
		{
			XmlSave();
		}
		public void XmlSave()
		{
			XmlDocument xmlFile = new XmlDocument();
			XmlNode root = xmlFile.CreateElement("전체");
			xmlFile.AppendChild(root);

			XmlNode userinfo = xmlFile.CreateElement("사용자_정보");
			XmlNode username = xmlFile.CreateElement("사용자_이름");
			username.InnerText = ps.m_sUser;
			userinfo.AppendChild(username);
			XmlNode userpermission = xmlFile.CreateElement("사용자_권한");
			userpermission.InnerText = ps.m_sPermission;
			userinfo.AppendChild(userpermission);
			XmlNode UseStartTime = xmlFile.CreateElement("사용시작시간");
			UseStartTime.InnerText = ps.m_sStartTime;
			userinfo.AppendChild(UseStartTime);
			XmlNode UseEndTime = xmlFile.CreateElement("사용종료시간");
			UseEndTime.InnerText = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
			userinfo.AppendChild(UseEndTime);
			root.AppendChild(userinfo);

			XmlNode camerainfo = xmlFile.CreateElement("카메라_정보");
			XmlNode camera1ipaddress1 = xmlFile.CreateElement("카메라1_IP주소");
			//camera1ipaddress1.InnerText = "192.168.11111";
			camera1ipaddress1.InnerText = form.Camera1IP;
			camerainfo.AppendChild(camera1ipaddress1);
			XmlNode camera1ipaddress2 = xmlFile.CreateElement("카메라2_IP주소");
			//camera1ipaddress2.InnerText = "192.168.22222";
			camera1ipaddress2.InnerText = form.Camera2IP;
			camerainfo.AppendChild(camera1ipaddress2);
			XmlNode camera1ipaddress3 = xmlFile.CreateElement("카메라3_IP주소");
			//camera1ipaddress3.InnerText = "192.168.33333";
			camera1ipaddress3.InnerText = form.Camera3IP;
			camerainfo.AppendChild(camera1ipaddress3);
			root.AppendChild(camerainfo);

			xmlFile.Save(@"../../xml\Settings.xml");
			ps.XmlLoad();
		}
	}
	public class Property_Setting
	{
		Form1 form;
		public Property_Setting(Form1 form)
		{
			this.form = form;
			// 여기서 xml 문서를 읽어와야 한다!!
			XmlLoad();
		}
		public void XmlLoad()
		{
			XmlDocument xmlFile = new XmlDocument();
			xmlFile.Load(@"../../xml\Settings.xml");

			XmlNodeList nodes = xmlFile.SelectNodes("/전체/사용자_정보");
			foreach(XmlNode node in nodes)
			{
				m_sUser = node.SelectSingleNode("사용자_이름").InnerText;
				if (m_sUser.Equals("홍길동")) m_sPermission = "관리자";
				else m_sPermission = "일반 유저";
				m_sStartTime = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
			}
			nodes = xmlFile.SelectNodes("/전체/카메라_정보");
			foreach (XmlNode node in nodes)
			{
				m_sCamera1_DeviceIP = form.Camera1IP;
				m_sCamera2_DeviceIP = form.Camera2IP;
				m_sCamera3_DeviceIP = form.Camera3IP;
			}
		}
		//[Category("카테고리 없으면 기타로 들어감"), DisplayName("왼쪽의 이름"), Description("맨밑에 설명")]
		public string m_sUser;
		[Category("\t사용자 정보"), DisplayName("이름"), Description("사용자의 성함")]
		public string User
		{
			get { return m_sUser; }
			//set { m_sUser = value; }
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
	}
}
