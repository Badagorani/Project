using DevExpress.XtraEditors;
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
using System.Xml.Serialization;
using System.Xml;
using DevExpress.Utils.Frames;
using DevExpress.Export.Xl;

namespace vision
{
	public partial class Settings_Page : XtraUserControl
	{
		Form1 MainForm;
		public Program_Setting ProgramSetting = new Program_Setting();
		public Camera_Setting CameraSetting = new Camera_Setting();
		public WorkFile_Setting WorkFileSetting = new WorkFile_Setting();
		public Settings_Page(Form1 form)
		{
			InitializeComponent();
			this.MainForm = form;
			XMLLoading();
		}
		public void XMLLoading()
		{
			XMLLoad_Program();
			XMLLoad_Camera();
			XMLLoad_Work();
		}
		public void XMLLoad_Program()
		{
			XMLLoad("ProgramSetting.xml");
			rb_Camera_ProgramSetting.SelectedIndex = ProgramSetting.BasicCameraView;
			lb_ImageSaveFolder_ProgramSetting.Text = ProgramSetting.ImageFilePath;
			lb_ImageSaveFolder_ProgramSetting.OptionsToolTip.ToolTip = ProgramSetting.ImageFilePath;
			cb_ImgFormat_ProgramSetting.Text = ProgramSetting.ImageFileFormat;
			lb_VideoSaveFolder_ProgramSetting.Text = ProgramSetting.VideoFilePath;
			lb_VideoSaveFolder_ProgramSetting.OptionsToolTip.ToolTip = ProgramSetting.VideoFilePath;
		}
		public void XMLLoad_Camera()
		{
			XMLLoad("CameraSetting.xml");
			txt_Cam1IP_CameraSetting.Text = MainForm.RealTimeView.Camera1IP;
			txt_Cam2IP_CameraSetting.Text = MainForm.RealTimeView.Camera2IP;
			txt_Cam3IP_CameraSetting.Text = MainForm.RealTimeView.Camera3IP;
			cb_TextView_CameraSetting.Text = CameraSetting.TextMark;
			if (cb_TextView_CameraSetting.Text.Equals("날짜/시간"))
			{
				lb_TextView_CameraSetting.Text = "  yyyy년 MM월 dd일 HH시 mm분 ss초";
				txt_UserText_CameraSetting.Enabled = false;
			}
			else if (cb_TextView_CameraSetting.Text.Equals("로봇 거리"))
			{
				txt_UserText_CameraSetting.Text = "  로봇 거리";
				txt_UserText_CameraSetting.Enabled = false;
			}
			else
			{
				txt_UserText_CameraSetting.Text = CameraSetting.UserText;
			}
			txt_CameraWidth_CameraSetting.Text = CameraSetting.CamWidth.ToString();
			txt_CameraHeight_CameraSetting.Text = CameraSetting.CamHeight.ToString();
		}
		public void XMLLoad_Work()
		{
			XMLLoad("WorkFileSetting.xml");
			tg_LogSaveOnOff.IsOn = WorkFileSetting.LogSave;
			txt_WorkUser_WorkFileSetting.Text = WorkFileSetting.WorkUser;
			dat_Day_WorkFileSetting.Text = DateTime.Today.ToString("yyyy-MM-dd");
			lb_WorkFolder_WorkFileSetting.Text = WorkFileSetting.WorkFilePath;
			lb_WorkFolder_WorkFileSetting.OptionsToolTip.ToolTip = WorkFileSetting.WorkFilePath;
		}
		public void XMLLoad(string XMLFile)
		{
			string strLocalFolder = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf(@"\")) + @"\xmlFile\";
			try
			{
				DirectoryInfo di = new DirectoryInfo(strLocalFolder);
				if (di.Exists == false)
				{
					di.Create();
				}
				var serializer = new XmlSerializer(typeof(Program_Setting));
				using (var reader = XmlReader.Create(strLocalFolder + XMLFile))
				{
					ProgramSetting = (Program_Setting)serializer.Deserialize(reader);
				}
			}
			catch (Exception ex)
			{
				//Console.WriteLine($"An exception occurred from {MethodBase.GetCurrentMethod().Name}", ex);
				MainForm.ShowMessage("오류", $"XML 문서 로드 중에 예외가 발생하였습니다! : {MethodBase.GetCurrentMethod().Name}\n" + ex, "주의");
			}
		}
		public void XMLSave(object obj, Type type)
		{
			//SetIPAddress();
			string strLocalFolder = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf(@"\"));
			string strXMLFolder;
			string strXMLFile;

			try
			{
				strXMLFolder = @"\xmlFile\";
				strXMLFile = strXMLFolder + @"ProgramSetting.xml";
				DirectoryInfo di = new DirectoryInfo(strLocalFolder + strXMLFolder);
				if (di.Exists == false) di.Create();
				if (obj.GetType() == ProgramSetting.GetType())
					strXMLFile = @"\xmlFile\ProgramSetting.xml";

				using (StreamWriter wr = new StreamWriter(strLocalFolder + strXMLFile))
				{
					var ns = new XmlSerializerNamespaces();
					ns.Add(string.Empty, string.Empty);

					XmlSerializer xs = new XmlSerializer(type);
					xs.Serialize(wr, obj, ns);
				}
				XMLLoad();
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", $"XML 문서 저장 중에 예외가 발생하였습니다! : {MethodBase.GetCurrentMethod().Name}\n" + ex, "주의");
			}
		}
		private void OnlyDigitPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)/* && (e.KeyChar != '.')*/)
			{
				e.Handled = true;
			}

			// only allow one decimal point
			//if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
			//{
			//	e.Handled = true;
			//}
		}
	}
	public class Program_Setting
	{
		//[Category("카테고리 없으면 기타로 들어감"), DisplayName("왼쪽의 이름"), Description("맨밑에 설명")]
		[Category("프로그램 설정"), DisplayName("기본 카메라 정보"), Description("선택된 기본 카메라")]
		public int	BasicCameraView	{ get; set; }
		[Category("프로그램 설정"), DisplayName("이미지 저장 폴더"), Description("이미지 폴더의 경로")]
		public string	ImageFilePath	{ get; set; }
		[Category("프로그램 설정"), DisplayName("이미지 포맷 기본값"), Description("이미지 파일의 저장 포맷")]
		public string	ImageFileFormat	{ get; set; }
		[Category("프로그램 설정"), DisplayName("동영상 저장 폴더"), Description("동영상 폴더의 경로")]
		public string	VideoFilePath	{ get; set; }
	}
	public class Camera_Setting
	{
		//[Category("카테고리 없으면 기타로 들어감"), DisplayName("왼쪽의 이름"), Description("맨밑에 설명")]
		[Category("카메라 설정"), DisplayName("카메라1 IP"), Description("카메라1의 IP 주소")]
		public string Camera1IPAddress { get; set; }
		[Category("카메라 설정"), DisplayName("카메라2 IP"), Description("카메라1의 IP 주소")]
		public string Camera2IPAddress { get; set; }
		[Category("카메라 설정"), DisplayName("카메라3 IP"), Description("카메라1의 IP 주소")]
		public string Camera3IPAddress { get; set; }
		[Category("카메라 설정"), DisplayName("영상 텍스트 기본값"), Description("영상에 표시할 텍스트")]
		public string TextMark { get; set; }
		[Category("카메라 설정"), DisplayName("사용자 텍스트"), Description("영상에 표시할 사용자 텍스트")]
		public string UserText { get; set; }
		[Category("카메라 설정"), DisplayName("가로 화면 크기"), Description("가로 화면의 크기")]
		public string CamWidth { get; set; }
		[Category("카메라 설정"), DisplayName("세로 화면 크기"), Description("세로 화면의 크기")]
		public string CamHeight { get; set; }
	}
	public class WorkFile_Setting
	{
		//[Category("카테고리 없으면 기타로 들어감"), DisplayName("왼쪽의 이름"), Description("맨밑에 설명")]
		[Category("작업파일 설정"), DisplayName("로그 저장 여부"), Description("로그의 저장 여부 설정")]
		public bool LogSave { get; set; }
		[Category("카메라 설정"), DisplayName("작업자"), Description("작업자의 성함")]
		public string WorkUser { get; set; }
		[Category("카메라 설정"), DisplayName("작업일자"), Description("작업하는 날짜")]
		public string WorkDay { get; set; }
		[Category("카메라 설정"), DisplayName("작업대상"), Description("작업의 대상")]
		public string WorkTarget { get; set; }
		[Category("카메라 설정"), DisplayName("작업폴더"), Description("로그 저장 폴더")]
		public string WorkFilePath { get; set; }
	}
}
