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
using vision.Properties;
using static System.Net.WebRequestMethods;
using DevExpress.XtraLayout;
using System.Security.Cryptography;
using Microsoft.WindowsAPICodePack.Dialogs;

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
			if (cb_TextView_CameraSetting.Text.Equals("날짜/시간"))
			{
				lb_TextView_CameraSetting.Text = "  ☑ 년월일시분초";
				txt_UserText_CameraSetting.Enabled = false;
			}
			else if (cb_TextView_CameraSetting.Text.Equals("로봇거리"))
			{
				lb_TextView_CameraSetting.Text = "  ☑ 로봇 거리";
				txt_UserText_CameraSetting.Enabled = false;
			}
			else
			{
				txt_UserText_CameraSetting.Text = CameraSetting.UserText;
			}
		}
		public void XMLLoad_CameraPlus()
		{
			txt_Cam1IP_CameraSetting.Text = MainForm.RealTimeView.Camera1IP;
			txt_Cam2IP_CameraSetting.Text = MainForm.RealTimeView.Camera2IP;
			txt_Cam3IP_CameraSetting.Text = MainForm.RealTimeView.Camera3IP;
			txt_CameraWidth_CameraSetting.Text = MainForm.RealTimeView.CameraWidth.ToString();
			txt_CameraHeight_CameraSetting.Text = MainForm.RealTimeView.CameraHeight.ToString();
		}
		public void XMLLoad_Work()
		{
			XMLLoad("WorkFileSetting.xml");
			tg_LogSaveOnOff.IsOn = WorkFileSetting.LogSave;
			txt_WorkUser_WorkFileSetting.Text = WorkFileSetting.WorkUser;
			dat_Day_WorkFileSetting.Text = DateTime.Today.ToString("yyyy-MM-dd");
			lb_WorkFolder_WorkFileSetting.Text = "  " + WorkFileSetting.WorkFilePath;
			//SizeF textSize = Graphics.FromImage(new Bitmap(1, 1)).MeasureString(lb_WorkFolder_WorkFileSetting.Text, lb_WorkFolder_WorkFileSetting.AppearanceItemCaption.Font);
			//if (textSize.Width > lb_WorkFolder_WorkFileSetting.MaxSize.Width)
			//{
			//	int index = 0;
			//	while (Graphics.FromImage(new Bitmap(1, 1)).MeasureString(lb_WorkFolder_WorkFileSetting.Text.Substring(0, index) + "...", lb_WorkFolder_WorkFileSetting.AppearanceItemCaption.Font).Width <= lb_WorkFolder_WorkFileSetting.MaxSize.Width)
			//	{
			//		index++;
			//	}
			//	lb_WorkFolder_WorkFileSetting.Text = lb_WorkFolder_WorkFileSetting.Text.Substring(0, index - 1) + "...";
			//}
			//lb_WorkFolder_WorkFileSetting.OptionsToolTip.ToolTip = WorkFileSetting.WorkFilePath;
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
				//Console.WriteLine($"An exception occurred from {MethodBase.GetCurrentMethod().Name}", ex);
				MainForm.ShowMessage("오류", $"XML 문서 로드 중에 예외가 발생하였습니다! : {MethodBase.GetCurrentMethod().Name}\n" + ex, "주의");
			}
		}
		public void XMLSave(object obj, Type type, string XMLFile)
		{
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
				MainForm.ShowMessage("오류", $"XML 문서 저장 중에 예외가 발생하였습니다! : {MethodBase.GetCurrentMethod().Name}\n" + ex, "주의");
			}
		}
		public void XMLFileCreate(string XMLFolder, string XMLFileName)
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
					XMLSave(CameraSetting, CameraSetting.GetType(), @"\" + XMLFileName);
					break;
				case "WorkFileSetting.xml":
					fs = System.IO.File.Create(FilePath);
					fs.Close();
					WorkFileSetting.LogSave = true;
					WorkFileSetting.WorkUser = "";
					WorkFileSetting.WorkDay = "";
					WorkFileSetting.WorkTarget = "";
					WorkFileSetting.WorkFilePath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf(@"\")) + @"WorkLog";
					XMLSave(WorkFileSetting, WorkFileSetting.GetType(), @"\" + XMLFileName);
					break;
			}
		}
		private void PathChange(object sender, EventArgs e)
		{
			try
			{
				using (CommonOpenFileDialog fd = new CommonOpenFileDialog())
				{
					string ButtonName = ((SimpleButton)sender).Name.Substring("btn_".Length, ((SimpleButton)sender).Name.LastIndexOf("_") - 4);
					string StartFolderPath;
					switch (ButtonName)
					{
						case "ImageFolder"	: StartFolderPath = ProgramSetting.ImageFilePath;	break;
						case "VideoFolder"	: StartFolderPath = ProgramSetting.VideoFilePath;	break;
						case "WorkFolder"	: StartFolderPath = WorkFileSetting.WorkFilePath;	break;
						default				: StartFolderPath = Application.ExecutablePath;		break;
					}
					fd.IsFolderPicker = true;
					fd.InitialDirectory = StartFolderPath;
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
					}
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "폴더를 선택하는 중에 예외가 발생하였습니다!\n" + ex, "주의");
			}
		}
		private void SaveButton(object sender, EventArgs e)
		{
			try
			{
				string savesetting = ((SimpleButton)sender).Name.Substring("btn_Save_".Length);
				switch (savesetting)
				{
					case "ProgramSetting"	:	ProgramSettingSave();
												MainForm.ShowMessage("오류", $"프로그램 설정이 변경되었습니다!\n", "알림");
												break;
					case "CameraSetting"	:	CameraSettingSave();
												MainForm.ShowMessage("오류", $"카메라 설정이 변경되었습니다!\n", "알림");
												break;
					case "WorkFileSetting"	:	WorkFileSettingSave();
												MainForm.ShowMessage("오류", $"작업파일 설정이 변경되었습니다!\n", "알림");
												break;
				}
			}
			catch(Exception ex)
			{
				MainForm.ShowMessage("오류", $"XML 문서 저장 중에 예외가 발생하였습니다! : {MethodBase.GetCurrentMethod().Name}\n" + ex, "경고");
			}
		}

		private void ResetButton(object sender, EventArgs e)
		{
			try
			{
				string savesetting = ((SimpleButton)sender).Name.Substring("btn_Reset_".Length);
				switch (savesetting)
				{
					case "ProgramSetting"	: XMLLoad_Program();						break;
					case "CameraSetting"	: XMLLoad_Camera(); XMLLoad_CameraPlus();	break;
					case "WorkFileSetting"	: XMLLoad_Work();							break;
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", $"XML 문서 저장 중에 예외가 발생하였습니다! : {MethodBase.GetCurrentMethod().Name}\n" + ex, "경고");
			}
		}
		public bool ProgramSettingSave()
		{
			string XMLFileName = "ProgramSetting.xml";
			try
			{
				if (ProgramSetting.BasicCameraView != rb_Camera_ProgramSetting.SelectedIndex) ProgramSetting.BasicCameraView = rb_Camera_ProgramSetting.SelectedIndex;
				if (!ProgramSetting.ImageFilePath.Equals(lb_ImageFolder_ProgramSetting.Text)) ProgramSetting.ImageFilePath = lb_ImageFolder_ProgramSetting.Text.Trim();
				if (!ProgramSetting.ImageFileFormat.Equals(cb_ImgFormat_ProgramSetting.Text)) ProgramSetting.ImageFileFormat = cb_ImgFormat_ProgramSetting.Text;
				if (!ProgramSetting.VideoFilePath.Equals(lb_VideoFolder_ProgramSetting.Text)) ProgramSetting.VideoFilePath = lb_VideoFolder_ProgramSetting.Text.Trim();
				XMLSave(ProgramSetting, ProgramSetting.GetType(), @"\" + XMLFileName);
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "폴더를 선택하는 중에 예외가 발생하였습니다!\n" + ex, "경고");
				return false;
			}
			finally
			{
				XMLLoad_Program();
			}
			return true;
		}
		public bool CameraSettingSave()
		{
			string XMLFileName = "CameraSetting.xml";
			try
			{
				if(!CameraSetting.TextMark.Equals(cb_TextView_CameraSetting.Text)					) CameraSetting.TextMark = cb_TextView_CameraSetting.Text;
				if(!CameraSetting.UserText.Equals(txt_UserText_CameraSetting.Text)					) CameraSetting.UserText = txt_UserText_CameraSetting.Text;
				if(!CameraSetting.Camera1IPAddress.Equals(txt_Cam1IP_CameraSetting.Text)			) CameraSetting.Camera1IPAddress = txt_Cam1IP_CameraSetting.Text;
				if(!CameraSetting.Camera2IPAddress.Equals(txt_Cam2IP_CameraSetting.Text)			) CameraSetting.Camera2IPAddress = txt_Cam2IP_CameraSetting.Text;
				if(!CameraSetting.Camera3IPAddress.Equals(txt_Cam3IP_CameraSetting.Text)			) CameraSetting.Camera3IPAddress = txt_Cam3IP_CameraSetting.Text; //MainForm.RealTimeView.Camera3IP;
				if(!CameraSetting.CamWidth.ToString().Equals(txt_CameraWidth_CameraSetting.Text)	) CameraSetting.CamWidth = int.Parse(txt_CameraWidth_CameraSetting.Text);
				if(!CameraSetting.CamHeight.ToString().Equals(txt_CameraHeight_CameraSetting.Text)	) CameraSetting.CamHeight = int.Parse(txt_CameraHeight_CameraSetting.Text);
				XMLSave(CameraSetting, CameraSetting.GetType(), @"\" + XMLFileName);
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "폴더를 선택하는 중에 예외가 발생하였습니다!\n" + ex, "주의");
				return false;
			}
			finally
			{
				XMLLoad_Camera();
				XMLLoad_CameraPlus();
			}
			return true;
		}
		public bool WorkFileSettingSave()
		{
			string XMLFileName = "WorkFileSetting.xml";
			try
			{
				if(!WorkFileSetting.LogSave != tg_LogSaveOnOff.IsOn							) WorkFileSetting.LogSave = tg_LogSaveOnOff.IsOn;
				if(!WorkFileSetting.WorkUser.Equals(txt_WorkUser_WorkFileSetting.Text)		) WorkFileSetting.WorkUser = txt_WorkUser_WorkFileSetting.Text;
				if(!WorkFileSetting.WorkDay.Equals(dat_Day_WorkFileSetting.Text)			) WorkFileSetting.WorkDay = dat_Day_WorkFileSetting.Text;
				if(!WorkFileSetting.WorkTarget.Equals(txt_WorkTarget_WorkFileSetting.Text)	) WorkFileSetting.WorkTarget = txt_WorkTarget_WorkFileSetting.Text;
				if(!WorkFileSetting.WorkFilePath.Equals(lb_WorkFolder_WorkFileSetting.Text)	) WorkFileSetting.WorkFilePath = lb_WorkFolder_WorkFileSetting.Text.Trim();
				XMLSave(WorkFileSetting, WorkFileSetting.GetType(), @"\" + XMLFileName);
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "폴더를 선택하는 중에 예외가 발생하였습니다!\n" + ex, "주의");
				return false;
			}
			finally
			{
				XMLLoad_Work();
			}
			return true;
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
		private void cb_TextView_CameraSetting_SelectedValueChanged(object sender, EventArgs e)
		{
			if (cb_TextView_CameraSetting.Text.Equals("사용자 선택"))
			{
				txt_UserText_CameraSetting.Enabled = true;
				lb_TextView_CameraSetting.Text = "";
			}
			else
			{
				if (cb_TextView_CameraSetting.Text.Equals("날짜/시간")) lb_TextView_CameraSetting.Text = "  ☑ 년월일시분초";
				if (cb_TextView_CameraSetting.Text.Equals("로봇거리")) lb_TextView_CameraSetting.Text = "  ☑ 로봇 거리";
				txt_UserText_CameraSetting.Enabled = false;
			}
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
