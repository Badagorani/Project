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
		//private string[] sDockName = new string[] { "메인 화면", "카메라1", "카메라2", "카메라3", "메인 화면", "카메라1", "카메라2", "카메라3", "영상1", "영상2"};
		//private PanelControl[] pcPanel = new PanelControl[10];
		//private PictureBox[] pbxView = new PictureBox[10];
		public RealTimeView_Page RealTimeView;
		public VideoCheck_Page VideoCheck;
		public VideoAnalysis_Page VideoAnalysis;
		public Settings_Page SettingsPage;
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
				SplashShow("페널 세팅 중...");
				PanelSettings();
				//ListAdd();
				SettingsLoad();
				SplashShow("설정 완료");
				//Thread.Sleep(1000);
				Thread.Sleep(1000);
				SplashScreenManager.CloseForm();
				//WindowsFormsSettings.FormThickBorder = true;
				//WindowsFormsSettings.ThickBorderWidth = 3;
				LoadingAnimationStart();
				RealTimeView = new RealTimeView_Page(this);
				//aa.Size = tableLayoutPanel1.Size;
				//RealTimeView_Page.Visible = false;
				navigationFrame3.Visible = false;
				RealTimeView.Parent = RealTimeView_Page;
				RealTimeView.BringToFront();
				RealTimeView.Dock = DockStyle.Fill;
				RealTimeView.Show();

				VideoCheck = new VideoCheck_Page(this);
				//VideoCheck_Page.Visible = false;
				navigationFrame4.Visible = false;
				VideoCheck.Parent = VideoCheck_Page;
				VideoCheck.BringToFront();
				VideoCheck.Dock = DockStyle.Fill;
				VideoCheck.Show();
			}
			catch (Exception ex)
			{
				ShowMessage("오류", "카메라 연결에 실패하였습니다!\n프로그램을 종료합니다!\n" + ex.Message, "경고");
				this.Close();
			}
		}
		#region 페널 세팅
		private void PanelSettings()
		{
			ContainerControl cc1 = new ContainerControl() { Dock = DockStyle.Fill };
			panelControl1.Controls.Add(cc1);
			DockManager dockManager1 = new DockManager(cc1);
			dockManager1.AddPanel(DockingStyle.Fill);
			//panelControl1.Dock = DockStyle.Fill;
			RealTimeView_MainCam.Parent = dockManager1.Panels[0];
			dockManager1.Panels[0].Text = "메인 화면";
			RealTimeView_MainCam.Dock = DockStyle.Fill;
			cc1.Controls.Add(dockManager1.Panels[0]);
			dockManager1.Panels[0].ClosedPanel += new DockPanelEventHandler(PanelClosed);

			ContainerControl cc2 = new ContainerControl() { Dock = DockStyle.Fill };
			panelControl2.Controls.Add(cc2);
			DockManager dockManager2 = new DockManager(cc2);
			dockManager2.AddPanel(DockingStyle.Fill);
			RealTimeView_Cam1.Parent = dockManager2.Panels[0];
			dockManager2.Panels[0].Text = "카메라1";
			RealTimeView_Cam1.Dock = DockStyle.Fill;
			dockManager2.Panels[0].ClosedPanel += new DockPanelEventHandler(PanelClosed);

			ContainerControl cc3 = new ContainerControl() { Dock = DockStyle.Fill };
			panelControl3.Controls.Add(cc3);
			DockManager dockManager3 = new DockManager(cc3);
			dockManager3.AddPanel(DockingStyle.Fill);
			RealTimeView_Cam2.Parent = dockManager3.Panels[0];
			dockManager3.Panels[0].Text = "카메라2";
			RealTimeView_Cam2.Dock = DockStyle.Fill;
			dockManager3.Panels[0].ClosedPanel += new DockPanelEventHandler(PanelClosed);

			ContainerControl cc4 = new ContainerControl() { Dock = DockStyle.Fill };
			panelControl4.Controls.Add(cc4);
			DockManager dockManager4 = new DockManager(cc4);
			dockManager4.AddPanel(DockingStyle.Fill);
			RealTimeView_Cam3.Parent = dockManager4.Panels[0];
			dockManager4.Panels[0].Text = "카메라3";
			RealTimeView_Cam3.Dock = DockStyle.Fill;
			dockManager4.Panels[0].ClosedPanel += new DockPanelEventHandler(PanelClosed);

			ContainerControl cc5 = new ContainerControl() { Dock = DockStyle.Fill };
			panelControl5.Controls.Add(cc5);
			DockManager dockManager5 = new DockManager(cc5);
			dockManager5.AddPanel(DockingStyle.Fill);
			VideoCheck_MainCam.Parent = dockManager5.Panels[0];
			dockManager5.Panels[0].Text = "메인 화면";
			VideoCheck_MainCam.Dock = DockStyle.Fill;
			dockManager5.Panels[0].ClosedPanel += new DockPanelEventHandler(PanelClosed);

			ContainerControl cc6 = new ContainerControl() { Dock = DockStyle.Fill };
			panelControl6.Controls.Add(cc6);
			DockManager dockManager6 = new DockManager(cc6);
			dockManager6.AddPanel(DockingStyle.Fill);
			VideoCheck_Cam1.Parent = dockManager6.Panels[0];
			dockManager6.Panels[0].Text = "카메라1";
			VideoCheck_Cam1.Dock = DockStyle.Fill;
			dockManager6.Panels[0].ClosedPanel += new DockPanelEventHandler(PanelClosed);

			ContainerControl cc7 = new ContainerControl() { Dock = DockStyle.Fill };
			panelControl7.Controls.Add(cc7);
			DockManager dockManager7 = new DockManager(cc7);
			dockManager7.AddPanel(DockingStyle.Fill);
			VideoCheck_Cam2.Parent = dockManager7.Panels[0];
			dockManager7.Panels[0].Text = "카메라2";
			VideoCheck_Cam2.Dock = DockStyle.Fill;
			dockManager7.Panels[0].ClosedPanel += new DockPanelEventHandler(PanelClosed);

			ContainerControl cc8 = new ContainerControl() { Dock = DockStyle.Fill };
			panelControl8.Controls.Add(cc8);
			DockManager dockManager8 = new DockManager(cc8);
			dockManager8.AddPanel(DockingStyle.Fill);
			VideoCheck_Cam3.Parent = dockManager8.Panels[0];
			dockManager8.Panels[0].Text = "카메라3";
			VideoCheck_Cam3.Dock = DockStyle.Fill;
			dockManager8.Panels[0].ClosedPanel += new DockPanelEventHandler(PanelClosed);

			ContainerControl cc9 = new ContainerControl() { Dock = DockStyle.Fill };
			panelControl9.Controls.Add(cc9);
			DockManager dockManager9 = new DockManager(cc9);
			dockManager9.AddPanel(DockingStyle.Fill);
			VideoAnalysis_Video1.Parent = dockManager9.Panels[0];
			dockManager9.Panels[0].Text = "영상1";
			VideoAnalysis_Video1.Dock = DockStyle.Fill;
			dockManager9.Panels[0].ClosedPanel += new DockPanelEventHandler(PanelClosed);

			ContainerControl cc10 = new ContainerControl() { Dock = DockStyle.Fill };
			panelControl10.Controls.Add(cc10);
			DockManager dockManager10 = new DockManager(cc10);
			dockManager10.AddPanel(DockingStyle.Fill);
			VideoAnalysis_Video2.Parent = dockManager10.Panels[0];
			dockManager10.Panels[0].Text = "영상2";
			VideoAnalysis_Video2.Dock = DockStyle.Fill;
			dockManager10.Panels[0].ClosedPanel += new DockPanelEventHandler(PanelClosed);
		}
		#endregion
		#region
		public void SettingsLoad()
		{
			SettingsPage = new Settings_Page(this);

			// 프로그램 설정
			rb_Camera_ProgramSetting.SelectedIndex = SettingsPage.ProgramSetting.BasicCameraView;
			//NowSelectedCamNo = SettingsPage.ProgramSetting.BasicCameraView;
			lb_ImageSaveFolder_ProgramSetting.Text = SettingsPage.ProgramSetting.ImageFilePath;
			lb_ImageSaveFolder_ProgramSetting.OptionsToolTip.ToolTip = SettingsPage.ProgramSetting.ImageFilePath;
			cb_ImgFormat_ProgramSetting.Text = SettingsPage.ProgramSetting.ImageFileFormat;
			lb_VideoSaveFolder_ProgramSetting.Text = SettingsPage.ProgramSetting.VideoFilePath;
			lb_VideoSaveFolder_ProgramSetting.OptionsToolTip.ToolTip = SettingsPage.ProgramSetting.VideoFilePath;

			// 카메라 설정
			//txt_Cam1IP_CameraSetting.Text = Camera1IP;
			//txt_Cam2IP_CameraSetting.Text = Camera2IP;
			//txt_Cam3IP_CameraSetting.Text = Camera3IP;
			cb_TextView_CameraSetting.Text = SettingsPage.CameraSetting.TextMark;
			if		(cb_TextView_CameraSetting.Text.Equals("날짜/시간"))	txt_UserText_CameraSetting.Text = SettingsPage.CameraSetting.UserText;
			else if (cb_TextView_CameraSetting.Text.Equals("로봇 거리"))	txt_UserText_CameraSetting.Text = "로봇 거리";
			else															txt_UserText_CameraSetting.Text = SettingsPage.CameraSetting.UserText;
			//txt_CameraWidth_CameraSetting.Text = CameraWidth.ToString();
			//txt_CameraHeight_CameraSetting.Text = CameraHeight.ToString();

			// 작업파일 설정
			//tg_LogSaveOnOff.IsOn = SettingsPage.WorkFileSetting.LogSave;
			//txt_WorkUser_WorkFileSetting.Text = SettingsPage.WorkFileSetting.WorkUser;
			dat_Day_WorkFileSetting.Text = DateTime.Today.ToString("yyyy-MM-dd");
			lb_WorkFolder_WorkFileSetting.Text = "  갸아악";//SettingsPage.WorkFileSetting.WorkFilePath;
			lb_WorkFolder_WorkFileSetting.OptionsToolTip.ToolTip = SettingsPage.WorkFileSetting.WorkFilePath;
		}
		#endregion

		#region 카메라 화면 닫기 / 다시열기
		private void PanelClosed(object sender, EventArgs e)
		{
			DockManager manager = ((DockPanel)sender).DockManager;
			SimpleButton sb = new SimpleButton();
			manager.Form.Parent.Controls[0].Controls.Add(sb);
			manager.Form.Parent.Controls[0].Controls.Add(manager.Panels[0]);
			sb.Font = new Font("맑은 고딕", 18F, FontStyle.Bold);
			sb.Text = "화면을 다시 열려면\n더블클릭 하세요";
			sb.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
			sb.Dock = DockStyle.Fill;
			sb.DoubleClick += new EventHandler(PanelDoubleClick);
		}
		private void PanelDoubleClick(object sender, EventArgs e)
		{
			Control control = ((SimpleButton)sender).Parent;
			DockPanel dp = (DockPanel)control.Controls[1];
			control.Controls.Clear();
			dp.Visibility = DockVisibility.Visible;
			dp.Dock = DockingStyle.Fill;
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
			if (!NavigationName.Equals(btn_RealTimeView)) popupControlContainer1.Visible = false;
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

		
		
		
		
		
		private void simpleButton4_Click(object sender, EventArgs e)
		{
			if (RealTimeView_MainCam.Image == null)
			{
				RealTimeView_MainCam.Image = Resources.Lens;
			}
			else if(RealTimeView_Cam1.Image == null)
			{
				RealTimeView_Cam1.Image = Resources.Lens;
			}
			else if (RealTimeView_Cam2.Image == null)
			{
				RealTimeView_Cam2.Image = Resources.Lens;
			}
			else if (RealTimeView_Cam3.Image == null)
			{
				RealTimeView_Cam3.Image = Resources.Lens;
			}
		}
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
			if (SettingsPage != null)
			{
				SettingsPage.ProgramSetting.BasicCameraView = rb_Camera_ProgramSetting.SelectedIndex + 1;
				SettingsPage.ProgramSetting.ImageFileFormat = cb_ImgFormat_ProgramSetting.EditValue.ToString();
				SettingsPage.XMLSave(SettingsPage.ProgramSetting, SettingsPage.ProgramSetting.GetType());
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

		private void Form1_Shown(object sender, EventArgs e)
		{
			LoadingAnimationEnd();
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
