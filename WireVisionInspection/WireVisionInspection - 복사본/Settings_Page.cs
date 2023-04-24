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
using WireVisionInspection.Properties;
using static System.Net.WebRequestMethods;
using DevExpress.XtraLayout;
using System.Security.Cryptography;
using Microsoft.WindowsAPICodePack.Dialogs;
using DevExpress.XtraWaitForm;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WireVisionInspection
{
	public partial class Settings_Page : XtraUserControl
	{
		Form1 MainForm;
		public LogRecord Log;
		public Program_Setting ProgramSetting = new Program_Setting();
		public Camera_Setting CameraSetting = new Camera_Setting();
		public WorkFile_Setting WorkFileSetting = new WorkFile_Setting();
		public bool UserSet = false;
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
			lb_ImageFolder_ProgramSetting.Text = "  " + ProgramSetting.ImageFilePath;
			//lb_ImageSaveFolder_ProgramSetting.Text = ProgramSetting.ImageFilePath;
			//lb_ImageSaveFolder_ProgramSetting.OptionsToolTip.ToolTip = ProgramSetting.ImageFilePath;
			cb_ImgFormat_ProgramSetting.Text = ProgramSetting.ImageFileFormat;
			lb_VideoFolder_ProgramSetting.Text = "  " + ProgramSetting.VideoFilePath;
			//lb_VideoSaveFolder_ProgramSetting.Text = ProgramSetting.VideoFilePath;
			//lb_VideoSaveFolder_ProgramSetting.OptionsToolTip.ToolTip = ProgramSetting.VideoFilePath;
		}
		public void XMLLoad_Camera()
		{
			XMLLoad("CameraSetting.xml");
			cb_TextView_CameraSetting.Text = CameraSetting.TextMark;
			if(MainForm.RealTimeView != null) MainForm.RealTimeView.cb_TextView.Text = CameraSetting.TextMark;
			if (cb_TextView_CameraSetting.Text.Equals("날짜/시간"))
			{
				lb_TextView_CameraSetting.Text = "  ☑ 년월일시분초";
				txt_UserText_CameraSetting.Enabled = false;
				txt_UserText_CameraSetting.Text = "";
				if (MainForm.RealTimeView != null)
				{
					MainForm.RealTimeView.lb_TextView.Text = "  ☑ 년월일시분초";
					MainForm.RealTimeView.txt_UserText.Enabled = false;
					MainForm.RealTimeView.txt_UserText.Text = "";
				}
			}
			else if (cb_TextView_CameraSetting.Text.Equals("로봇거리"))
			{
				lb_TextView_CameraSetting.Text = "  ☑ 로봇 거리";
				txt_UserText_CameraSetting.Enabled = false;
				txt_UserText_CameraSetting.Text = "";
				if (MainForm.RealTimeView != null)
				{
					MainForm.RealTimeView.lb_TextView.Text = "  ☑ 로봇 거리";
					MainForm.RealTimeView.txt_UserText.Enabled = false;
					MainForm.RealTimeView.txt_UserText.Text = "";
				}
			}
			else
			{
				txt_UserText_CameraSetting.Text = CameraSetting.UserText;
				if (MainForm.RealTimeView != null) MainForm.RealTimeView.txt_UserText.Text = CameraSetting.UserText;
			}
			txt_Cam1pxLength_CameraSetting.Text = CameraSetting.Cam1pxLength.ToString();
			txt_Cam2pxLength_CameraSetting.Text = CameraSetting.Cam2pxLength.ToString();
			txt_Cam3pxLength_CameraSetting.Text = CameraSetting.Cam3pxLength.ToString();
		}
		public void XMLLoad_CameraPlus()
		{
			CameraSetting.Camera1IPAddress = txt_Cam1IP_CameraSetting.Text = MainForm.RealTimeView.Camera1IP;
			CameraSetting.Camera2IPAddress = txt_Cam2IP_CameraSetting.Text = MainForm.RealTimeView.Camera2IP;
			CameraSetting.Camera3IPAddress = txt_Cam3IP_CameraSetting.Text = MainForm.RealTimeView.Camera3IP;
			txt_CameraWidth_CameraSetting.Text = MainForm.RealTimeView.CameraWidth.ToString();
			txt_CameraHeight_CameraSetting.Text = MainForm.RealTimeView.CameraHeight.ToString();
			CameraSetting.CamWidth = MainForm.RealTimeView.CameraWidth;
			CameraSetting.CamHeight = MainForm.RealTimeView.CameraHeight;
			XMLSave(CameraSetting, CameraSetting.GetType(), @"\CameraSetting.xml");
		}
		public void XMLLoad_Work()
		{
			XMLLoad("WorkFileSetting.xml");
			tg_LogSaveOnOff.IsOn = true;
			WorkFileSetting.LogSave = tg_LogSaveOnOff.IsOn;
			//txt_WorkUser_WorkFileSetting.Text = WorkFileSetting.WorkUser = "";
			dat_Day_WorkFileSetting.Text = WorkFileSetting.WorkDay = DateTime.Now.ToString("yyyy-MM-dd dddd");
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
				MainForm.ShowMessage("오류", $"XML 문서 로드 중 예외가 발생하였습니다!\n" + ex, "주의");
			}
		}
		public void LogReady()
		{
			Log = MainForm.Log;
		}
		public void XMLSave(object obj, Type type, string XMLFile)
		{
			if (Log != null) Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
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
				MainForm.ShowMessage("오류", $"XML 문서 저장 중 예외가 발생하였습니다!" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		public void XMLFileCreate(string XMLFolder, string XMLFileName)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
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
						ProgramSetting.ImageFilePath = @"C:\FS-MCS500POE_Image_Save";
						ProgramSetting.ImageFileFormat = "JPG";
						ProgramSetting.VideoFilePath = @"C:\FS-MCS500POE_Video_Save";
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
						CameraSetting.Cam1pxLength = 0.0264;
						CameraSetting.Cam2pxLength = 0.0264;
						CameraSetting.Cam3pxLength = 0.0264;
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
			catch(Exception ex)
			{
				MainForm.ShowMessage("오류", $"XML 문서 생성 중 예외가 발생하였습니다!" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		private void FirstViewCameraChange(object sender, EventArgs e)
		{
			if(Log != null ) Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			//MainForm.LoadingAnimationStart();
			try
			{
				ProgramSetting.BasicCameraView = rb_Camera_ProgramSetting.SelectedIndex;
				XMLSave(ProgramSetting, ProgramSetting.GetType(), @"\ProgramSetting.xml");
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", $"기본 카메라 변경 중 예외가 발생하였습니다!" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//MainForm.LoadingAnimationEnd();
		}
		private void PathChange(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			//MainForm.LoadingAnimationStart();
			try
			{
				string ButtonName = ((SimpleButton)sender).Name.Substring("btn_".Length, ((SimpleButton)sender).Name.LastIndexOf("_") - 4);
				string StartFolderPath;
				using (CommonOpenFileDialog fd = new CommonOpenFileDialog())
				{
					switch (ButtonName)
					{
						case "ImageFolder"	: StartFolderPath = ProgramSetting.ImageFilePath;	break;
						case "VideoFolder"	: StartFolderPath = ProgramSetting.VideoFilePath;	break;
						case "WorkFolder"	: StartFolderPath = WorkFileSetting.WorkFilePath;	break;
						default				: StartFolderPath = Application.ExecutablePath;		break;
					}
					fd.IsFolderPicker = true;
					fd.DefaultDirectory = fd.InitialDirectory = StartFolderPath;
					if (fd.ShowDialog() == CommonFileDialogResult.Ok)
					{
						string NewFolderPath = fd.FileName;
						LabelControl[] labels = new LabelControl[] { lb_ImageFolder_ProgramSetting, lb_VideoFolder_ProgramSetting, lb_WorkFolder_WorkFileSetting };
						foreach(LabelControl label in labels)
						{
							if(ButtonName.Equals(label.Name.Substring("lb_".Length, label.Name.LastIndexOf("_") - 3)))
							{
								label.Text = "  " + NewFolderPath;
								break;
							}
						}
						switch (ButtonName)
						{
							case "ImageFolder"	: ProgramSetting.ImageFilePath = NewFolderPath;	break;
							case "VideoFolder"	: ProgramSetting.VideoFilePath = NewFolderPath;	break;
							case "WorkFolder"	: WorkFileSetting.WorkFilePath = NewFolderPath;	break;
						}
						if (!ButtonName.Equals("WorkFolder"))
						{
							XMLSave(ProgramSetting, ProgramSetting.GetType(), @"\ProgramSetting.xml");
						}
						else
						{
							XMLSave(WorkFileSetting, WorkFileSetting.GetType(), @"\WorkFileSetting.xml");
							//Log.LogFolderSet();
						}
					}
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "폴더를 선택하는 중 예외가 발생하였습니다!\n" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//MainForm.LoadingAnimationEnd();
		}
		private void ImageFormatChange(object sender, EventArgs e)
		{
			if (Log != null) Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			//MainForm.LoadingAnimationStart();
			try
			{
				ProgramSetting.ImageFileFormat = cb_ImgFormat_ProgramSetting.Text;
				XMLSave(ProgramSetting, ProgramSetting.GetType(), @"\ProgramSetting.xml");
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "이미지 포맷을 선택하는 중 예외가 발생하였습니다!\n" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//MainForm.LoadingAnimationEnd();
		}
		private void CameraIPChange(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			//MainForm.LoadingAnimationStart();
			try
			{
				string changeName = ((TextEdit)sender).Name;
				string newIP = ((TextEdit)sender).Text;
				if (!NewIPCheck(newIP))
				{
					MainForm.ShowMessage("아이피 입력", "아이피를 잘못 입력하였습니다!", "경고");
					((TextEdit)sender).Focus();
					return;
				}
				string changeCamera = changeName.Substring("txt_Cam".Length, 1);
				switch (changeCamera)
				{
					case "1":
						if (CameraSetting.Camera1IPAddress.Equals(newIP)) return;
						else
						{
							MainForm.RealTimeView.Camera1IP = newIP;
							CameraSetting.Camera1IPAddress = newIP;
						}
						break;
					case "2":
						if (CameraSetting.Camera2IPAddress.Equals(newIP)) return;
						else
						{
							MainForm.RealTimeView.Camera2IP = newIP;
							CameraSetting.Camera2IPAddress = newIP;
						}
						break;
					case "3":
						if (CameraSetting.Camera3IPAddress.Equals(newIP)) return;
						else
						{
							MainForm.RealTimeView.Camera3IP = newIP;
							CameraSetting.Camera3IPAddress = newIP;
						}
						break;
				}
				XMLSave(CameraSetting, CameraSetting.GetType(), @"\CameraSetting.xml");
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "아이피를 변경하는 중 예외가 발생하였습니다!\n" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//MainForm.LoadingAnimationEnd();
		}
		private bool NewIPCheck(string newIP)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				string[] IPs;
				IPs = newIP.Split('.');
				for (int i = 0; i < IPs.Length; i++)
				{
					int checkIP = int.Parse(IPs[i]);
					if (i == 4) return false;
					else
					{
						if (checkIP < 1 || checkIP > 255) return false;
					}
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "아이피를 체크하는 중 예외가 발생하였습니다!\n" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
				return false;
			}
			return true;
		}
		public void VideoTextChange(object sender, EventArgs e)
		{
			if(Log != null) Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			//MainForm.LoadingAnimationStart();
			try
			{
				if (cb_TextView_CameraSetting.Text.Equals("사용자 선택"))
				{
					if (UserSet) txt_UserText_CameraSetting.Enabled = true;
					lb_TextView_CameraSetting.Text = "  사용자 텍스트 입력";

					if (MainForm.RealTimeView != null)
					{
						MainForm.RealTimeView.txt_UserText.Enabled = true;
						MainForm.RealTimeView.cb_TextView.Text = cb_TextView_CameraSetting.Text;
						MainForm.RealTimeView.lb_TextView.Text = "사용자 텍스트 입력";
					}
				}
				else
				{
					if (cb_TextView_CameraSetting.Text.Equals("날짜/시간")) lb_TextView_CameraSetting.Text = "  ☑ 년월일시분초";
					if (cb_TextView_CameraSetting.Text.Equals("로봇거리")) lb_TextView_CameraSetting.Text = "  ☑ 로봇 거리";
					txt_UserText_CameraSetting.Text = "";
					txt_UserText_CameraSetting.Enabled = false;

					if (MainForm.RealTimeView != null)
					{
						if (cb_TextView_CameraSetting.Text.Equals("날짜/시간")) MainForm.RealTimeView.lb_TextView.Text = "☑ 년월일시분초";
						if (cb_TextView_CameraSetting.Text.Equals("로봇거리")) MainForm.RealTimeView.lb_TextView.Text = "☑ 로봇 거리";
						MainForm.RealTimeView.cb_TextView.Text = cb_TextView_CameraSetting.Text;
						MainForm.RealTimeView.txt_UserText.Text = "";
						MainForm.RealTimeView.txt_UserText.Enabled = false;
					}
				}
				CameraSetting.TextMark = cb_TextView_CameraSetting.Text;
				if (MainForm.RealTimeView != null) CameraSetting.UserText = txt_UserText_CameraSetting.Text;
				else txt_UserText_CameraSetting.Text = CameraSetting.UserText;
				XMLSave(CameraSetting, CameraSetting.GetType(), @"\CameraSetting.xml");
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "영상의 텍스트를 변경하는 중 예외가 발생하였습니다!\n" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//MainForm.LoadingAnimationEnd();
		}
		private void UserTextChange(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			//MainForm.LoadingAnimationStart();
			try
			{
				if (MainForm.RealTimeView != null) CameraSetting.UserText = txt_UserText_CameraSetting.Text;
				else txt_UserText_CameraSetting.Text = CameraSetting.UserText;
				MainForm.RealTimeView.txt_UserText.Text = txt_UserText_CameraSetting.Text;
				XMLSave(CameraSetting, CameraSetting.GetType(), @"\CameraSetting.xml");
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "텍스트를 변경하는 중 예외가 발생하였습니다!\n" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//MainForm.LoadingAnimationEnd();
		}
		private void PixelLengthChange(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			//MainForm.LoadingAnimationStart();
			try
			{
				int CamNo = int.Parse(((TextEdit)sender).Name.Substring("txt_Cam".Length, 1));
				double pixelLength = double.Parse(((TextEdit)sender).Text);
				switch(CamNo)
				{
					case 1: CameraSetting.Cam1pxLength = pixelLength; break;
					case 2: CameraSetting.Cam2pxLength = pixelLength; break;
					case 3: CameraSetting.Cam3pxLength = pixelLength; break;
				}
				((TextEdit)sender).Text = pixelLength.ToString();
				XMLSave(CameraSetting, CameraSetting.GetType(), @"\CameraSetting.xml");
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "픽셀 길이를 변경하는 중 예외가 발생하였습니다!\n" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//MainForm.LoadingAnimationEnd();
		}
		private void LogOnOff(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			//MainForm.LoadingAnimationStart();
			try
			{
				WorkFileSetting.LogSave = tg_LogSaveOnOff.IsOn;
				XMLSave(WorkFileSetting, WorkFileSetting.GetType(), @"\WorkFileSetting.xml");
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "로그 스위치를 변경하는 중 예외가 발생하였습니다!\n" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//MainForm.LoadingAnimationEnd();
		}
		private void WorkUserChange(object sender, EventArgs e)
		{
			if (Log != null) Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			//MainForm.LoadingAnimationStart();
			try
			{
				if (!txt_WorkUser_WorkFileSetting.Text.Equals(""))	txt_WorkUser_WorkFileSetting.Properties.Appearance.BorderColor = Color.Lime;
				else												txt_WorkUser_WorkFileSetting.Properties.Appearance.BorderColor = Color.Red;
				WorkFileSetting.WorkUser = txt_WorkUser_WorkFileSetting.Text;
				//XMLSave(WorkFileSetting, WorkFileSetting.GetType(), @"\WorkFileSetting.xml");
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "작업자를 변경하는 중 예외가 발생하였습니다!\n" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//MainForm.LoadingAnimationEnd();
		}
		private void WorkTargetChange(object sender, EventArgs e)
		{
			if (Log != null) Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			//MainForm.LoadingAnimationStart();
			try
			{
				if (!txt_WorkTarget_WorkFileSetting.Text.Equals(""))	txt_WorkTarget_WorkFileSetting.Properties.Appearance.BorderColor = Color.Lime;
				else													txt_WorkTarget_WorkFileSetting.Properties.Appearance.BorderColor = Color.Red;
				WorkFileSetting.WorkTarget = txt_WorkTarget_WorkFileSetting.Text;
				//XMLSave(WorkFileSetting, WorkFileSetting.GetType(), @"\WorkFileSetting.xml");
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "작업대상을 변경하는 중 예외가 발생하였습니다!\n" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//MainForm.LoadingAnimationEnd();
		}
		private void WorkDayChange(object sender, EventArgs e)
		{
			if (Log != null) Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			//MainForm.LoadingAnimationStart();
			try
			{
				//dat_Day_WorkFileSetting.Properties.Appearance.BorderColor = Color.Lime;
				WorkFileSetting.WorkDay = dat_Day_WorkFileSetting.Text;
				XMLSave(WorkFileSetting, WorkFileSetting.GetType(), @"\WorkFileSetting.xml");
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "작업날짜를 변경하는 중 예외가 발생하였습니다!\n" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//MainForm.LoadingAnimationEnd();
		}
		private void WorkUserReWrite(object sender, EventArgs e)
		{
			if (Log != null) Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			//MainForm.LoadingAnimationStart();
			try
			{
				UserSet = false;
				ControlEnableToggle();
				txt_WorkUser_WorkFileSetting.Text = "";
				txt_WorkTarget_WorkFileSetting.Text = "";
				btn_WorkUserReWrite_WorkFileSetting.Enabled = false;
				btn_WorkUserSet_WorkFileSetting.Enabled = true;
				txt_WorkUser_WorkFileSetting.Enabled = true;
				txt_WorkTarget_WorkFileSetting.Enabled = true;
				//XMLSave(WorkFileSetting, WorkFileSetting.GetType(), @"\WorkFileSetting.xml");
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "작업파일변경을 시작하는 중 예외가 발생하였습니다!\n" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//MainForm.LoadingAnimationEnd();
		}
		private void WorkUserSet(object sender, EventArgs e)
		{
			if (Log != null) Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			//MainForm.LoadingAnimationStart();
			try
			{
				if(txt_WorkUser_WorkFileSetting.Text.Equals("") || txt_WorkTarget_WorkFileSetting.Text.Equals(""))
				{
					MainForm.ShowMessage("알림", "작업자 또는 작업대상을 입력하세요!", "알림");
					return;
				}
				else
				{
					UserSet = true;
					Log.LogFolderChange(txt_WorkUser_WorkFileSetting.Text);
					Log.LogFileChange(txt_WorkTarget_WorkFileSetting.Text);
					lb_WorkFolder_WorkFileSetting.Text = Log.LogFolderPath;
					ControlEnableToggle();
					btn_WorkUserReWrite_WorkFileSetting.Enabled = true;
					btn_WorkUserSet_WorkFileSetting.Enabled = false;
					txt_WorkUser_WorkFileSetting.Enabled = false;
					txt_WorkTarget_WorkFileSetting.Enabled = false;
					XMLSave(WorkFileSetting, WorkFileSetting.GetType(), @"\WorkFileSetting.xml");
					MainForm.ShowMessage("알림", "작업자 설정이 완료되었습니다!", "알림");
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "작업자 설정을 변경하는 중 예외가 발생하였습니다!\n" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//MainForm.LoadingAnimationEnd();
		}
		public void ControlEnableToggle()
		{
			// 기본 카메라
			if (rb_Camera_ProgramSetting.Enabled) rb_Camera_ProgramSetting.Enabled = false;
			else rb_Camera_ProgramSetting.Enabled = true;

			// 이미지 저장 폴더
			if (btn_ImageFolder_ProgramSetting.Enabled) btn_ImageFolder_ProgramSetting.Enabled = false;
			else btn_ImageFolder_ProgramSetting.Enabled = true;

			// 이미지 파일 포맷
			if (cb_ImgFormat_ProgramSetting.Enabled) cb_ImgFormat_ProgramSetting.Enabled = false;
			else cb_ImgFormat_ProgramSetting.Enabled = true;

			// 동영상 저장 폴더
			if (btn_VideoFolder_ProgramSetting.Enabled) btn_VideoFolder_ProgramSetting.Enabled = false;
			else btn_VideoFolder_ProgramSetting.Enabled = true;

			// 카메라1 IP
			if (txt_Cam1IP_CameraSetting.Enabled) txt_Cam1IP_CameraSetting.Enabled = false;
			else txt_Cam1IP_CameraSetting.Enabled = true;

			// 카메라2 IP
			if (txt_Cam2IP_CameraSetting.Enabled) txt_Cam2IP_CameraSetting.Enabled = false;
			else txt_Cam2IP_CameraSetting.Enabled = true;

			// 카메라3 IP
			if (txt_Cam3IP_CameraSetting.Enabled) txt_Cam3IP_CameraSetting.Enabled = false;
			else txt_Cam3IP_CameraSetting.Enabled = true;

			// 영상 텍스트
			if (cb_TextView_CameraSetting.Enabled) cb_TextView_CameraSetting.Enabled = false;
			else cb_TextView_CameraSetting.Enabled = true;

			// 영상 텍스트 사용자 지정
			if (txt_UserText_CameraSetting.Enabled) txt_UserText_CameraSetting.Enabled = false;
			else if (cb_TextView_CameraSetting.Text.Equals("사용자 선택")) txt_UserText_CameraSetting.Enabled = true;

			// 카메라1 1px 길이
			if (txt_Cam1pxLength_CameraSetting.Enabled) txt_Cam1pxLength_CameraSetting.Enabled = false;
			else txt_Cam1pxLength_CameraSetting.Enabled = true;

			// 카메라2 1px 길이
			if (txt_Cam2pxLength_CameraSetting.Enabled) txt_Cam2pxLength_CameraSetting.Enabled = false;
			else txt_Cam2pxLength_CameraSetting.Enabled = true;

			// 카메라3 1px 길이
			if (txt_Cam3pxLength_CameraSetting.Enabled) txt_Cam3pxLength_CameraSetting.Enabled = false;
			else txt_Cam3pxLength_CameraSetting.Enabled = true;

			// 로그 저장 여부
			if (tg_LogSaveOnOff.Enabled) tg_LogSaveOnOff.Enabled = false;
			else tg_LogSaveOnOff.Enabled = true;


		}
		//private void SaveButton(object sender, EventArgs e)
		//{
		//	MainForm.LoadingAnimationStart();
		//	try
		//	{
		//		string savesetting = ((SimpleButton)sender).Name.Substring("btn_Save_".Length);
		//		switch (savesetting)
		//		{
		//			case "ProgramSetting"	:	if (!ProgramSettingSave()) MainForm.ShowMessage("알림", $"프로그램 설정\n변경된 사항이 없거나 변경이 실패하였습니다!", "알림");
		//										else MainForm.ShowMessage("알림", $"프로그램 설정이 변경되었습니다!", "알림");
		//										break;
		//			case "CameraSetting"	:	if (!CameraSettingSave()) MainForm.ShowMessage("알림", $"카메라 설정\n변경된 사항이 없거나 변경이 실패하였습니다!", "알림");
		//										else MainForm.ShowMessage("알림", $"카메라 설정이 변경되었습니다!", "알림");
		//										break;
		//			case "WorkFileSetting"	:	if (!WorkFileSettingSave()) MainForm.ShowMessage("알림", $"작업파일 설정\n변경된 사항이 없거나 변경이 실패하였습니다!", "알림");
		//										else MainForm.ShowMessage("알림", $"작업파일 설정이 변경되었습니다!", "알림");
		//										break;
		//		}
		//	}
		//	catch(Exception ex)
		//	{
		//		MainForm.ShowMessage("오류", $"설정 변경 중 예외가 발생하였습니다!" + ex, "경고");
		//	}
		//	MainForm.LoadingAnimationEnd();
		//}
		//private void ResetButton(object sender, EventArgs e)
		//{
		//	try
		//	{
		//		string savesetting = ((SimpleButton)sender).Name.Substring("btn_Reset_".Length);
		//		switch (savesetting)
		//		{
		//			case "ProgramSetting"	: XMLLoad_Program();						break;
		//			case "CameraSetting"	: XMLLoad_Camera(); XMLLoad_CameraPlus();	break;
		//			case "WorkFileSetting"	: XMLLoad_Work();							break;
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		MainForm.ShowMessage("오류", $"초기화 중 예외가 발생하였습니다!" + ex, "경고");
		//	}
		//}
		//public bool ProgramSettingSave()
		//{
		//	string XMLFileName = "ProgramSetting.xml";
		//	bool IsChanged = false;
		//	try
		//	{
		//		if (ProgramSetting.BasicCameraView != rb_Camera_ProgramSetting.SelectedIndex)
		//		{
		//			ProgramSetting.BasicCameraView = rb_Camera_ProgramSetting.SelectedIndex;
		//			IsChanged = true;
		//		}
		//		if (!ProgramSetting.ImageFilePath.Equals(lb_ImageFolder_ProgramSetting.Text.Trim()))
		//		{
		//			ProgramSetting.ImageFilePath = lb_ImageFolder_ProgramSetting.Text.Trim();
		//			IsChanged = true;
		//		}
		//		if (!ProgramSetting.ImageFileFormat.Equals(cb_ImgFormat_ProgramSetting.Text))
		//		{
		//			ProgramSetting.ImageFileFormat = cb_ImgFormat_ProgramSetting.Text;
		//			IsChanged = true;
		//		}
		//		if (!ProgramSetting.VideoFilePath.Equals(lb_VideoFolder_ProgramSetting.Text.Trim()))
		//		{
		//			ProgramSetting.VideoFilePath = lb_VideoFolder_ProgramSetting.Text.Trim();
		//			IsChanged = true;
		//		}
		//		if (IsChanged) XMLSave(ProgramSetting, ProgramSetting.GetType(), @"\" + XMLFileName);
		//		else return false;
		//	}
		//	catch (Exception ex)
		//	{
		//		MainForm.ShowMessage("오류", "프로그램 설정에 예외가 발생하였습니다!\n" + ex.Message, "경고");
		//		return false;
		//	}
		//	finally
		//	{
		//		XMLLoad_Program();
		//	}
		//	return true;
		//}
		//public bool CameraSettingSave()
		//{
		//	string XMLFileName = "CameraSetting.xml";
		//	try
		//	{
		//		if(!CameraSetting.TextMark.Equals(cb_TextView_CameraSetting.Text)					) CameraSetting.TextMark = cb_TextView_CameraSetting.Text;
		//		if(!CameraSetting.UserText.Equals(txt_UserText_CameraSetting.Text)					) CameraSetting.UserText = txt_UserText_CameraSetting.Text;
		//		if(!CameraSetting.Camera1IPAddress.Equals(txt_Cam1IP_CameraSetting.Text)			) CameraSetting.Camera1IPAddress = txt_Cam1IP_CameraSetting.Text;
		//		if(!CameraSetting.Camera2IPAddress.Equals(txt_Cam2IP_CameraSetting.Text)			) CameraSetting.Camera2IPAddress = txt_Cam2IP_CameraSetting.Text;
		//		if(!CameraSetting.Camera3IPAddress.Equals(txt_Cam3IP_CameraSetting.Text)			) CameraSetting.Camera3IPAddress = txt_Cam3IP_CameraSetting.Text; //MainForm.RealTimeView.Camera3IP;
		//		if(!CameraSetting.CamWidth.ToString().Equals(txt_CameraWidth_CameraSetting.Text)	) CameraSetting.CamWidth = int.Parse(txt_CameraWidth_CameraSetting.Text);
		//		if(!CameraSetting.CamHeight.ToString().Equals(txt_CameraHeight_CameraSetting.Text)	) CameraSetting.CamHeight = int.Parse(txt_CameraHeight_CameraSetting.Text);
		//		XMLSave(CameraSetting, CameraSetting.GetType(), @"\" + XMLFileName);
		//	}
		//	catch (Exception ex)
		//	{
		//		MainForm.ShowMessage("오류", "카메라 설정에 예외가 발생하였습니다!\n" + ex, "주의");
		//		return false;
		//	}
		//	finally
		//	{
		//		XMLLoad_Camera();
		//		XMLLoad_CameraPlus();
		//	}
		//	return true;
		//}
		//public bool WorkFileSettingSave()
		//{
		//	string XMLFileName = "WorkFileSetting.xml";
		//	try
		//	{
		//		if(!WorkFileSetting.LogSave != tg_LogSaveOnOff.IsOn									) WorkFileSetting.LogSave = tg_LogSaveOnOff.IsOn;
		//		if(!WorkFileSetting.WorkUser.Equals(txt_WorkUser_WorkFileSetting.Text)				) WorkFileSetting.WorkUser = txt_WorkUser_WorkFileSetting.Text;
		//		if(!WorkFileSetting.WorkDay.Equals(dat_Day_WorkFileSetting.Text)					) WorkFileSetting.WorkDay = dat_Day_WorkFileSetting.Text;
		//		if(!WorkFileSetting.WorkTarget.Equals(txt_WorkTarget_WorkFileSetting.Text)			) WorkFileSetting.WorkTarget = txt_WorkTarget_WorkFileSetting.Text;
		//		if(!WorkFileSetting.WorkFilePath.Equals(lb_WorkFolder_WorkFileSetting.Text.Trim())	) WorkFileSetting.WorkFilePath = lb_WorkFolder_WorkFileSetting.Text.Trim();
		//		XMLSave(WorkFileSetting, WorkFileSetting.GetType(), @"\" + XMLFileName);
		//	}
		//	catch (Exception ex)
		//	{
		//		MainForm.ShowMessage("오류", "작업파일 설정에 예외가 발생하였습니다!\n" + ex, "주의");
		//		return false;
		//	}
		//	finally
		//	{
		//		XMLLoad_Work();
		//	}
		//	return true;
		//}
		private void OnlyDigitPress(object sender, KeyPressEventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
			{
				e.Handled = true;
			}

			// only allow one decimal point
			if ((e.KeyChar == '.') && ((sender as /*System.Windows.Forms.TextBox*/TextEdit).Text.IndexOf('.') > -1))
			{
				e.Handled = true;
			}
			if(((TextEdit)sender).Text.Length > 15) MainForm.ShowMessage("오류", "세로 크기는 600 ~ 2048 범위입니다!", "주의");
		}
		private void OnKeyPressSingleIP(object sender, KeyPressEventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			TextEdit text = (TextEdit)sender;
			int iPos = 0;               // IP 구역의 현재 위치
			int iDelimitNumber = 0;     // IP 구역의 갯수

			int iLength = text.Text.Length;
			int iIndex = text.Text.LastIndexOf(".");
			int iIndex2 = -1;

			while (true)
			{
				iIndex2 = text.Text.IndexOf(".", iIndex2 + 1);
				if (iIndex2 == -1) break;

				++iDelimitNumber;
			}

			// 숫자키와 백스페이스, '.' 만 입력 가능
			if ((e.KeyChar < 48 || e.KeyChar > 57) &&
			e.KeyChar != 8 && e.KeyChar != '.')
			{
				MainForm.ShowMessage("오류", "숫자만 입력 가능합니다!", "주의");
				e.Handled = true;
				return;
			}

			if (e.KeyChar != 8)
			{
				if (e.KeyChar != '.')
				{
					if (iIndex > 0) iPos = iLength - iIndex;
					else			iPos = iLength + 1;

					if (iPos == 3)
					{
						// 255 이상 체크
						string strTmp = text.Text.Substring(iIndex + 1) + e.KeyChar;
						if (Int32.Parse(strTmp) > 255)
						{
							MainForm.ShowMessage("오류", "IP 숫자는 255를 넘길 수 없습니다!", "주의");
							e.Handled = true;
							return;
						}
						else
						{
							// 3자리가 넘어가면 자동으로 .을 찍어준다
							if (iDelimitNumber < 3)
							{
								text.AppendText(e.KeyChar.ToString());
								text.AppendText(".");
								iDelimitNumber++;
								e.Handled = true;
								return;
							}
						}
					}

					// IP 마지막 4자리째는 무조건 무시
					if (iPos == 4)
					{
						e.Handled = true;
						return;
					}
				}
				else
				{
					// 아이피가 3구역 이상 쓰였으면, 이후 키는 무시한다
					if (iDelimitNumber + 1 > 3)
					{
						MainForm.ShowMessage("오류", "IP 주소가 정확하지 않습니다!", "주의");
						e.Handled = true;
						return;
					}
					else
					{
						// 연속으로 .을 찍었으면 오류
						if (text.Text.EndsWith("."))
						{
							MainForm.ShowMessage("오류", "IP 주소가 정확하지 않습니다!", "주의");
							e.Handled = true;
							return;
						}
						else iDelimitNumber++;
					}
				}
			}
		}
		private void txt_CameraSetting_Leave(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			string TextEditName = ((TextEdit)sender).Name;
			try
			{
				switch (TextEditName)
				{
					case "txt_CameraWidth_CameraSetting": int CameraWidth = int.Parse(txt_CameraWidth_CameraSetting.Text);
						if (800 > CameraWidth && CameraWidth > 2448)
						{
							MainForm.ShowMessage("오류", "가로 크기는 800 ~ 2448 범위입니다!", "주의");
							txt_CameraWidth_CameraSetting.Text = "2448";
						}
						break;
					case "txt_CameraHeight_CameraSetting": int CameraHeight = int.Parse(txt_CameraHeight_CameraSetting.Text);
						if (600 > CameraHeight && CameraHeight > 2048)
						{
							MainForm.ShowMessage("오류", "세로 크기는 600 ~ 2048 범위입니다!", "주의");
							txt_CameraHeight_CameraSetting.Text = "2048";
						}
						break;
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "화면 크기 설정 중 예외가 발생하였습니다!\n" + ex, "주의");
			}
		}
		private void FolderOpen(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				string FolderPath = ((LabelControl)sender).Text.Trim();
				System.Diagnostics.Process.Start(FolderPath);
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "작업폴더를 여는 중 예외가 발생하였습니다!\n" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		public bool ChangeCheck()
		{
			bool ProgramChange = false;
			bool CameraChange = false;
			bool WorkFileChange = false;
			// 프로그램 설정 변경 체크
			if (ProgramSetting.BasicCameraView != rb_Camera_ProgramSetting.SelectedIndex)			ProgramChange = true;
			if (!ProgramSetting.ImageFilePath.Equals(lb_ImageFolder_ProgramSetting.Text.Trim()))	ProgramChange = true;
			if (!ProgramSetting.ImageFileFormat.Equals(cb_ImgFormat_ProgramSetting.Text))			ProgramChange = true;
			if (!ProgramSetting.VideoFilePath.Equals(lb_VideoFolder_ProgramSetting.Text.Trim()))	ProgramChange = true;
			// 카메라 설정 변경 체크
			if (!CameraSetting.TextMark.Equals(cb_TextView_CameraSetting.Text))						CameraChange = true;
			if (!CameraSetting.UserText.Equals(txt_UserText_CameraSetting.Text))					CameraChange = true;
			if (!CameraSetting.Camera1IPAddress.Equals(txt_Cam1IP_CameraSetting.Text))				CameraChange = true;
			if (!CameraSetting.Camera2IPAddress.Equals(txt_Cam2IP_CameraSetting.Text))				CameraChange = true;
			if (!CameraSetting.Camera3IPAddress.Equals(txt_Cam3IP_CameraSetting.Text))				CameraChange = true;
			if (!CameraSetting.CamWidth.ToString().Equals(txt_CameraWidth_CameraSetting.Text))		CameraChange = true;
			if (!CameraSetting.CamHeight.ToString().Equals(txt_CameraHeight_CameraSetting.Text))	CameraChange = true;

			// 작업파일 설정 변경 체크
			if (WorkFileSetting.LogSave != tg_LogSaveOnOff.IsOn)									WorkFileChange = true;
			if (!WorkFileSetting.WorkUser.Equals(txt_WorkUser_WorkFileSetting.Text))				WorkFileChange = true;
			if (!WorkFileSetting.WorkDay.Equals(dat_Day_WorkFileSetting.Text))						WorkFileChange = true;
			if (!WorkFileSetting.WorkTarget.Equals(txt_WorkTarget_WorkFileSetting.Text))			WorkFileChange = true;
			if (!WorkFileSetting.WorkFilePath.Equals(lb_WorkFolder_WorkFileSetting.Text.Trim()))	WorkFileChange = true;

			if (ProgramChange || CameraChange || WorkFileChange)	return false;
			else													return true;
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
		public long CamWidth { get; set; }
		[Category("카메라 설정"), DisplayName("세로 화면 크기"), Description("세로 화면의 크기")]
		public long CamHeight { get; set; }
		[Category("카메라 설정"), DisplayName("카메라1 1px 길이(cm)"), Description("카메라에서 보이는 픽셀의 실제 길이")]
		public double Cam1pxLength { get; set; }
		[Category("카메라 설정"), DisplayName("카메라2 1px 길이(cm)"), Description("카메라에서 보이는 픽셀의 실제 길이")]
		public double Cam2pxLength { get; set; }
		[Category("카메라 설정"), DisplayName("카메라3 1px 길이(cm)"), Description("카메라에서 보이는 픽셀의 실제 길이")]
		public double Cam3pxLength { get; set; }
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
