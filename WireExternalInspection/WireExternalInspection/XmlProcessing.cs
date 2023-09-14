using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;

namespace WireExternalInspection
{
	public class XmlProcessing
	{
		MainForm mainform;
		public Program_Setting  ProgramSetting = new Program_Setting();
		public Camera_Setting   CameraSetting = new Camera_Setting();
		public WorkFile_Setting WorkFileSetting = new WorkFile_Setting();
		public XmlProcessing(MainForm form)
		{
			mainform = form;
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
		}
		public void XMLLoad_Camera()
		{
			XMLLoad("CameraSetting.xml");
			//cb_TextView_CameraSetting.Text = CameraSetting.TextMark;
			//if (MainForm.RealTimeView != null) MainForm.RealTimeView.cb_TextView.Text = CameraSetting.TextMark;
			//if (cb_TextView_CameraSetting.Text.Equals("날짜/시간"))
			//{
			//	lb_TextView_CameraSetting.Text = "  ☑ 년월일시분초";
			//	txt_UserText_CameraSetting.Enabled = false;
			//	txt_UserText_CameraSetting.Text = "";
			//	if (MainForm.RealTimeView != null)
			//	{
			//		MainForm.RealTimeView.lb_TextView.Text = "  ☑ 년월일시분초";
			//		MainForm.RealTimeView.txt_UserText.Enabled = false;
			//		MainForm.RealTimeView.txt_UserText.Text = "";
			//	}
			//}
			//else if (cb_TextView_CameraSetting.Text.Equals("로봇거리"))
			//{
			//	lb_TextView_CameraSetting.Text = "  ☑ 로봇 거리";
			//	txt_UserText_CameraSetting.Enabled = false;
			//	txt_UserText_CameraSetting.Text = "";
			//	if (MainForm.RealTimeView != null)
			//	{
			//		MainForm.RealTimeView.lb_TextView.Text = "  ☑ 로봇 거리";
			//		MainForm.RealTimeView.txt_UserText.Enabled = false;
			//		MainForm.RealTimeView.txt_UserText.Text = "";
			//	}
			//}
			//else
			//{
			//	txt_UserText_CameraSetting.Text = CameraSetting.UserText;
			//	if (MainForm.RealTimeView != null) MainForm.RealTimeView.txt_UserText.Text = CameraSetting.UserText;
			//}
			//txt_Cam1pxLength_CameraSetting.Text = CameraSetting.Cam1pxLength.ToString();
			//txt_Cam2pxLength_CameraSetting.Text = CameraSetting.Cam2pxLength.ToString();
			//txt_Cam3pxLength_CameraSetting.Text = CameraSetting.Cam3pxLength.ToString();
		}
		public void XMLLoad_CameraPlus()
		{
			//if (mainform.m_isWork[0]) CameraSetting.Camera1IPAddress = txt_Cam1IP_CameraSetting.Text = MainForm.RealTimeView.Camera1IP;
			//if (mainform.m_isWork[1]) CameraSetting.Camera2IPAddress = txt_Cam2IP_CameraSetting.Text = MainForm.RealTimeView.Camera2IP;
			//if (mainform.m_isWork[2]) CameraSetting.Camera3IPAddress = txt_Cam3IP_CameraSetting.Text = MainForm.RealTimeView.Camera3IP;
			//txt_CameraWidth_CameraSetting.Text = MainForm.RealTimeView.CameraWidth.ToString();
			//txt_CameraHeight_CameraSetting.Text = MainForm.RealTimeView.CameraHeight.ToString();
			//CameraSetting.CamWidth = MainForm.RealTimeView.CameraWidth;
			//CameraSetting.CamHeight = MainForm.RealTimeView.CameraHeight;
			//XMLSave(CameraSetting, CameraSetting.GetType(), @"\CameraSetting.xml");
		}
		public void XMLLoad_Work()
		{
			XMLLoad("WorkFileSetting.xml");
			//tg_LogSaveOnOff.IsOn = true;
			//WorkFileSetting.LogSave = tg_LogSaveOnOff.IsOn;
			//txt_WorkUser_WorkFileSetting.Text = WorkFileSetting.WorkUser = "";
			//dat_Day_WorkFileSetting.Text = WorkFileSetting.WorkDay = DateTime.Now.ToString("yyyy-MM-dd dddd");
			//txt_WorkTarget_WorkFileSetting.Text = WorkFileSetting.WorkTarget = "";
			//lb_WorkFolder_WorkFileSetting.Text = "  " + WorkFileSetting.WorkFilePath;
			XMLSave(WorkFileSetting, WorkFileSetting.GetType(), @"\WorkFileSetting.xml");
		}
		public void XMLLoad(string XMLFileName)
		{
			string strLocalFolder = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf(@"\")) + @"\xmlFile\";
			try
			{
				DirectoryInfo di = new DirectoryInfo(strLocalFolder);
				if (di.Exists == false)
				{
					di.Create();
					XMLFileCreate(strLocalFolder, XMLFileName);
				}
				if (!System.IO.File.Exists(strLocalFolder + XMLFileName)) XMLFileCreate(strLocalFolder, XMLFileName);
				using (var reader = XmlReader.Create(strLocalFolder + XMLFileName))
				{
					switch (XMLFileName)
					{
						case "ProgramSetting.xml":
							var serializer = new XmlSerializer(typeof(Program_Setting));
							ProgramSetting = (Program_Setting)serializer.Deserialize(reader);
							break;
						case "CameraSetting.xml":
							serializer = new XmlSerializer(typeof(Camera_Setting));
							CameraSetting = (Camera_Setting)serializer.Deserialize(reader);
							break;
						case "WorkFileSetting.xml":
							serializer = new XmlSerializer(typeof(WorkFile_Setting));
							WorkFileSetting = (WorkFile_Setting)serializer.Deserialize(reader);
							//using (var reader = XmlReader.Create(strLocalFolder + XMLFileName))
							//{
							//	WorkFileSetting = (WorkFile_Setting)serializer.Deserialize(reader);
							//}
							break;
					}
				}
			}
			catch (Exception ex)
			{
				mainform.ShowMessage("오류", $"XML 문서 로드 중 예외가 발생하였습니다!\n" + ex, "주의");
			}
		}
		public void XMLSave(object obj, Type type, string XMLFile)
		{
			if (mainform.Log != null) mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			//SetIPAddress();
			string strLocalFolder = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf(@"\")) + @"\xmlFile\";
			try
			{
				//using (StreamWriter wr = new StreamWriter(strLocalFolder + XMLFile))
				//{
				//	var ns = new XmlSerializerNamespaces();
				//	ns.Add(string.Empty, string.Empty);

				//	XmlSerializer xs = new XmlSerializer(type);
				//	xs.Serialize(wr, obj, ns);
				//}
				XmlSerializer serializer = new XmlSerializer(type);
				using (TextWriter writer = new StreamWriter(strLocalFolder + XMLFile))
				{
					serializer.Serialize(writer, obj);
				}
			}
			catch (Exception ex)
			{
				mainform.ShowMessage("오류", $"XML 문서 저장 중 예외가 발생하였습니다!" + ex, "주의");
				mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		public void XMLFileCreate(string XMLFolder, string XMLFileName)
		{
			mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				string FilePath = XMLFolder + XMLFileName;
				FileStream fs;
				switch (XMLFileName)
				{
					case "ProgramSetting.xml":
						//fs = System.IO.File.Create(FilePath);
						//fs.Close();
						ProgramSetting.BasicCameraView = 0;
						ProgramSetting.SaveFilePath = @"C:\WireExternalInspectionSaveData";
						ProgramSetting.ImageFilePath = @"C:\WireExternalInspectionSaveData\Image_Save";
						ProgramSetting.ImageFileFormat = "JPG";
						ProgramSetting.VideoFilePath = @"C:\WireExternalInspectionSaveData\Video_Save";
						XMLSave(ProgramSetting, ProgramSetting.GetType(), @"\" + XMLFileName);
						break;
					case "CameraSetting.xml":
						fs = System.IO.File.Create(FilePath);
						fs.Close();
						CameraSetting.TextMark = "날짜/시간";
						CameraSetting.UserText = "";
						CameraSetting.Camera1IPAddress = ""; //inForm.RealTimeView.Camera1IP;
						CameraSetting.Camera2IPAddress = ""; //inForm.RealTimeView.Camera2IP;
						CameraSetting.Camera3IPAddress = ""; //MainForm.RealTimeView.Camera3IP;
						CameraSetting.CamWidth = 0; //MainForm.RealTimeView.CameraWidth;
						CameraSetting.CamHeight = 0; //MainForm.RealTimeView.CameraHeight;
						XMLSave(CameraSetting, CameraSetting.GetType(), @"\" + XMLFileName);
						break;
					case "WorkFileSetting.xml":
						fs = System.IO.File.Create(FilePath);
						fs.Close();
						WorkFileSetting.LogSave = true;
						WorkFileSetting.WorkUser = "";
						WorkFileSetting.WorkDay = "";
						WorkFileSetting.WorkTarget = "";
						WorkFileSetting.WorkFilePath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf(@"\")) + @"\WorkLog";
						XMLSave(WorkFileSetting, WorkFileSetting.GetType(), @"\" + XMLFileName);
						break;
				}
			}
			catch (Exception ex)
			{
				mainform.ShowMessage("오류", $"XML 문서 생성 중 예외가 발생하였습니다!" + ex, "주의");
				mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
	}
	public class Program_Setting
	{
		//[Category("카테고리 없으면 기타로 들어감"), DisplayName("왼쪽의 이름"), Description("맨밑에 설명")]
		[Category("프로그램 설정"), DisplayName("기본 카메라 정보"), Description("선택된 기본 카메라")]
		public int BasicCameraView { get; set; }
		[Category("프로그램 설정"), DisplayName("이미지 저장 폴더"), Description("이미지 폴더의 경로")]
		public string SaveFilePath { get; set; }
		[Category("프로그램 설정"), DisplayName("이미지 저장 폴더"), Description("이미지 폴더의 경로")]
		public string ImageFilePath { get; set; }
		[Category("프로그램 설정"), DisplayName("이미지 포맷 기본값"), Description("이미지 파일의 저장 포맷")]
		public string ImageFileFormat { get; set; }
		[Category("프로그램 설정"), DisplayName("동영상 저장 폴더"), Description("동영상 폴더의 경로")]
		public string VideoFilePath { get; set; }
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
		public long CamWidth { get; set; }
		[Category("카메라 설정"), DisplayName("세로 화면 크기"), Description("세로 화면의 크기")]
		public long CamHeight { get; set; }
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
