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

namespace vision
{
	public partial class Property : Form
	{
		Form1 form;
		public Property_Setting property_setting = new Property_Setting();
		public ProgramSetting ps = new ProgramSetting();
		public Property(Form1 form)
		{
			InitializeComponent();
			this.form = form;
			XmlLoad();
			propertyGrid1.SelectedObject = ps;
		}
		public void XmlSave(object obj, Type type)
		{
			//SetIPAddress();
			string strLocalFolder = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\"));
			string strXMLFolder;
			string strXMLFile;

			try
			{
				strXMLFolder = @"\xmlFile\";
				strXMLFile = strXMLFolder + @"Settings.xml";
				DirectoryInfo di = new DirectoryInfo(strLocalFolder + strXMLFolder);
				if (di.Exists == false) di.Create();
				if (obj.GetType() == ps.GetType())
					strXMLFile = @"\xmlFile\Settings.xml";

				using (StreamWriter wr = new StreamWriter(strLocalFolder + strXMLFile))
				{
					var ns = new XmlSerializerNamespaces();
					ns.Add(string.Empty, string.Empty);

					XmlSerializer xs = new XmlSerializer(type);
					xs.Serialize(wr, obj, ns);
				}
				XmlLoad();
				propertyGrid1.SelectedObject = ps;
			}
			catch(Exception ex)
			{
				form.ShowMessage("오류", $"XML 문서 저장 중에 예외가 발생하였습니다! : {MethodBase.GetCurrentMethod().Name}\n" + ex, "주의");
			}
		}
		public void XmlEndSave(object obj, Type type)
		{
			property_setting = (Property_Setting)propertyGrid1.SelectedObject;
			XmlSave(obj, type);
		}
		public void XmlLoad()
		{
			string strLocalFolder = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\"));
			string strXMLFolder;
			string strXMLFile;

			try
			{
				strXMLFolder = @"\xmlFile\";
				strXMLFile = strXMLFolder + @"Settings.xml";
				DirectoryInfo di = new DirectoryInfo(strLocalFolder + strXMLFolder);
				if (di.Exists == false) di.Create();
				var serializer = new XmlSerializer(typeof(ProgramSetting));
				using (var reader = XmlReader.Create(strLocalFolder + strXMLFile))
				{
					ps = (ProgramSetting)serializer.Deserialize(reader);
					//property_setting.m_sCamera1_DeviceIP = form.Camera1IP;
					//property_setting.m_sCamera2_DeviceIP = form.Camera2IP;
					//property_setting.m_sCamera3_DeviceIP = form.Camera3IP;
				}
			}
			catch (Exception ex)
			{
				//Console.WriteLine($"An exception occurred from {MethodBase.GetCurrentMethod().Name}", ex);
				form.ShowMessage("오류", $"XML 문서 로드 중에 예외가 발생하였습니다! : {MethodBase.GetCurrentMethod().Name}\n" + ex, "주의");
			}
		}
		public void SetIPAddress()
		{
			//form.Camera1IP = property_setting.m_sCamera1_DeviceIP;
			//form.Camera2IP = property_setting.m_sCamera2_DeviceIP;
			//form.Camera3IP = property_setting.m_sCamera3_DeviceIP;
		}
	}
	public class ProgramSetting
	{
		//[Category("카테고리 없으면 기타로 들어감"), DisplayName("왼쪽의 이름"), Description("맨밑에 설명")]
		[Category("프로그램 설정"), DisplayName("기본 카메라 정보"), Description("선택된 기본 카메라")]
		public string BasicCameraView
		{
			get;
			set;
		}
		[Category("프로그램 설정"), DisplayName("이미지 저장 폴더"), Description("이미지 폴더의 경로")]
		public string ImageFilePath
		{
			get;
			set;
		}
		[Category("프로그램 설정"), DisplayName("이미지 포맷 기본값"), Description("이미지 파일의 저장 포맷")]
		public string ImageFileFormat
		{
			get;
			set;
		}
		[Category("프로그램 설정"), DisplayName("동영상 저장 폴더"), Description("동영상 폴더의 경로")]
		public string VideoFilePath
		{
			get;
			set;
		}
	}
	public class Property_Setting
	{
		//[Category("카테고리 없으면 기타로 들어감"), DisplayName("왼쪽의 이름"), Description("맨밑에 설명")]
		
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
	}
}
