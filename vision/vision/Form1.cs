using DevExpress.XtraBars.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Docking;
using System.Reflection;
using DevExpress.Utils.Extensions;
using vision.Properties;
using OMRON_Camera_Control;
using DevExpress.Utils.Helpers;
using System.Security.Cryptography;
using Accord.Video.FFMPEG;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using static DevExpress.Skins.SolidColorHelper;
using Accord.Video;
using DevExpress.LookAndFeel;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using DevExpress.Utils.CodedUISupport;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using DevExpress.XtraLayout;
using DevExpress.Skins;
using System.Configuration;

namespace vision
{
	public partial class Form1 : DevExpress.XtraEditors.XtraForm
	{
		public RealTimeView_Page RealTimeView;
		public VideoCheck_Page VideoCheck;
		public VideoAnalysis_Page VideoAnalysis;
		public Settings_Page Settings;
		public Form1()
		{
			InitializeComponent();
		}
		#region 스플래시 스크린
		private void SplashShow(string message = "")
		{
			FluentSplashScreenOptions op = new FluentSplashScreenOptions();
			if (message.Equals(""))
			{
				op.Title = "와이어 비전 검사 소프트웨어";
				op.Subtitle = "Wire Vision Inspection Software";
				op.RightFooter = "준비";
				op.LeftFooter = "Copyright 2023. Seinens Co.,Ltd.\nAll Rights reserved.";
				op.LoadingIndicatorType = FluentLoadingIndicatorType.Dots;
				op.OpacityColor = Color.Black;
				op.Opacity = 100;
				op.LogoImageOptions.SvgImage = Resources.KIRO_CI;
				SplashScreenManager.ShowFluentSplashScreen(
					op,
					parentForm: this,
					useFadeIn: true,
					useFadeOut: true
				);
			}
			else
			{
				op.RightFooter = message;
				SplashScreenManager.Default.SendCommand(FluentSplashScreenCommand.UpdateOptions, op);
			}
		}
		#endregion
		private void Form1_Load(object sender, EventArgs e)
		{
			try
			{
				SplashShow();
				SplashShow("설정을 불러오는 중...");
				SettingsLoad();
				SplashShow("완료");
				Thread.Sleep(1000);
				SplashScreenManager.CloseForm();
			}
			catch (Exception ex)
			{
				ShowMessage("오류", "카메라 연결에 실패하였습니다!\n프로그램을 종료합니다!\n" + ex.Message, "경고");
				this.Close();
			}
		}
		private void Form1_Shown(object sender, EventArgs e)
		{
			LoadingAnimationStart();
			RealTimeView = new RealTimeView_Page(this);
			RealTimeView.Parent = RealTimeView_Page;
			RealTimeView.BringToFront();
			RealTimeView.Dock = DockStyle.Fill;
			RealTimeView.Show();

			VideoCheck = new VideoCheck_Page(this);
			VideoCheck.Parent = VideoCheck_Page;
			VideoCheck.BringToFront();
			VideoCheck.Dock = DockStyle.Fill;
			VideoCheck.Show();

			VideoAnalysis = new VideoAnalysis_Page(this);
			VideoAnalysis.Parent = VideoAnalysis_Page;
			VideoAnalysis.BringToFront();
			VideoAnalysis.Dock = DockStyle.Fill;
			VideoAnalysis.Show();

			Settings.Parent = Settings_Page;
			Settings.BringToFront();
			Settings.Dock = DockStyle.Fill;
			Settings.Show();

			LoadingAnimationEnd();
		}
		#region 세팅 불러오기
		public void SettingsLoad()
		{
			Settings = new Settings_Page(this);

			// 프로그램 설정
			//rb_Camera_ProgramSetting.SelectedIndex = Settings.ProgramSetting.BasicCameraView;
			//NowSelectedCamNo = Settings.ProgramSetting.BasicCameraView;
			//lb_ImageSaveFolder_ProgramSetting.Text = Settings.ProgramSetting.ImageFilePath;
			//lb_ImageSaveFolder_ProgramSetting.OptionsToolTip.ToolTip = Settings.ProgramSetting.ImageFilePath;
			//cb_ImgFormat_ProgramSetting.Text = Settings.ProgramSetting.ImageFileFormat;
			//lb_VideoSaveFolder_ProgramSetting.Text = Settings.ProgramSetting.VideoFilePath;
			//lb_VideoSaveFolder_ProgramSetting.OptionsToolTip.ToolTip = Settings.ProgramSetting.VideoFilePath;

			//// 카메라 설정
			//txt_Cam1IP_CameraSetting.Text = Camera1IP;
			//txt_Cam2IP_CameraSetting.Text = Camera2IP;
			//txt_Cam3IP_CameraSetting.Text = Camera3IP;
			//cb_TextView_CameraSetting.Text = Settings.CameraSetting.TextMark;
			//if (cb_TextView_CameraSetting.Text.Equals("날짜/시간")) txt_UserText_CameraSetting.Text = Settings.CameraSetting.UserText;
			//else if (cb_TextView_CameraSetting.Text.Equals("로봇 거리")) txt_UserText_CameraSetting.Text = "로봇 거리";
			//else txt_UserText_CameraSetting.Text = Settings.CameraSetting.UserText;
			//txt_CameraWidth_CameraSetting.Text = CameraWidth.ToString();
			//txt_CameraHeight_CameraSetting.Text = CameraHeight.ToString();

			//// 작업파일 설정
			//tg_LogSaveOnOff.IsOn = Settings.WorkFileSetting.LogSave;
			//txt_WorkUser_WorkFileSetting.Text = Settings.WorkFileSetting.WorkUser;
			//dat_Day_WorkFileSetting.Text = DateTime.Today.ToString("yyyy-MM-dd");
			//lb_WorkFolder_WorkFileSetting.Text = "  갸아악";//Settings.WorkFileSetting.WorkFilePath;
			//lb_WorkFolder_WorkFileSetting.OptionsToolTip.ToolTip = Settings.WorkFileSetting.WorkFilePath;
		}
		#endregion
		#region 내비게이션 클릭 이벤트
		private void NavigationClick(object sender, EventArgs e)
		{
			string NavigationName = ((SimpleButton)sender).Name;
			if (RealTimeView.IsRecord)
			{
				ShowMessage("녹화 중...", "녹화 중에는 페이지를 변경할 수 없습니다!!", "경고");
				return;
			}
			switch (NavigationName)
			{
				case "btn_RealTimeView":
					navigationFrame2.SelectedPage = RealTimeView_Page;
					break;
				case "btn_VideoCheck":
					navigationFrame2.SelectedPage = VideoCheck_Page;
					break;
				case "btn_VideoAnalysis":
					navigationFrame2.SelectedPage = VideoAnalysis_Page;
					break;
				case "btn_Settings":
					navigationFrame2.SelectedPage = Settings_Page;
					//if (tg_LogSaveOnOff.IsOn) tg_LogSaveOnOff.IsOn = false;
					//else tg_LogSaveOnOff.IsOn = true;
					break;
			}
		}
		#endregion
		#region 팝업
		/// <summary>
		/// 메세지 박스를 표시한다
		/// </summary>
		/// <param name="Content">내용</param>
		/// <param name="Title">제목</param>
		/// <param name="UseIcon">아이콘 알림❕, 주의❗, 경고❌, 질문❔</param>
		public void ShowMessage(string Title, string Content, string UseIcon = "")
		{
			MessageBoxIcon messageBoxIcon;
			#region 아이콘 선택 (없음이 기본)
			switch (UseIcon)
			{
				case "알림":
					messageBoxIcon = MessageBoxIcon.Information;
					break;
				case "주의":
					messageBoxIcon = MessageBoxIcon.Warning;
					break;
				case "경고":
					messageBoxIcon = MessageBoxIcon.Error;
					break;
				case "질문":
					messageBoxIcon = MessageBoxIcon.Question;
					break;
				default:
					messageBoxIcon = MessageBoxIcon.None;
					break;
			}
			#endregion
			MessageBox.Show(Content, Title, MessageBoxButtons.OK, messageBoxIcon);
		}
		#endregion
		#region 폼 닫기
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (Settings != null)
			{
				//Settings.ProgramSetting.BasicCameraView = rb_Camera_ProgramSetting.SelectedIndex + 1;
				//Settings.ProgramSetting.ImageFileFormat = cb_ImgFormat_ProgramSetting.EditValue.ToString();
				Settings.XMLSave(Settings.ProgramSetting, Settings.ProgramSetting.GetType());
			}
			this.Hide();
			RealTimeView.CameraClose();
			//Viewer_Thread.ViewSetting(NowSelectedCamNo, IsViewing);
			//Camera_Setting.DestroyCamera();
		}
		#endregion
		public IOverlaySplashScreenHandle overlayHandle;
		public void LoadingAnimationStart()
		{
			overlayHandle = SplashScreenManager.ShowOverlayForm(
					navigationFrame2,
					fadeIn: true,
					fadeOut: true,
					backColor: Color.White,
					foreColor: Color.FromArgb(7, 117, 104),
					opacity: 150,
					animationType: WaitAnimationType.Line);
		}
		public void LoadingAnimationEnd()
		{
			overlayHandle.Close();
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
}
