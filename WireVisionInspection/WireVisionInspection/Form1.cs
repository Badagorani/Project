﻿using DevExpress.XtraBars.Navigation;
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
using WireVisionInspection.Properties;
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
using DevExpress.XtraWaitForm;

namespace WireVisionInspection
{
	public partial class Form1 : DevExpress.XtraEditors.XtraForm
	{
		public RealTimeView_Page RealTimeView;
		public VideoCheck_Page VideoCheck;
		public VideoAnalysis_Page VideoAnalysis;
		public Settings_Page Settings;
		public LogRecord Log;
		public int NowPageNo = 0;
		public int PastPageNo = 0;
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
				op.LeftFooter = "Copyright 2023. Seinens Co.,Ltd.\nAll Rights reserved.";/*Copyright© 올바른 표기법은?*/
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
				Settings = new Settings_Page(this);
				Log = new LogRecord(this);
				Settings.LogReady();
				Settings.ControlEnableToggle();
				NowPageNo = 4;
				PastPageNo = 4;
			}
			catch (Exception ex)
			{
				ShowMessage("오류", "카메라 연결에 실패하였습니다!\n프로그램을 종료합니다!\n" + ex.Message, "경고");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
				this.Close();
			}
			SplashShow("완료");
			Thread.Sleep(1000);
			SplashScreenManager.CloseForm();
			Thread.Sleep(1000);
		}
		private void Form1_Shown(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			LoadingAnimationStart();
			try
			{
				RealTimeView = new RealTimeView_Page(this);
				RealTimeView.Parent = RealTimeView_Page;
				RealTimeView.BringToFront();
				RealTimeView.Dock = DockStyle.Fill;
				RealTimeView.Show();
				if (RealTimeView.m_isWork != null && RealTimeView.m_isWork[0] && RealTimeView.m_isWork[1] && RealTimeView.m_isWork[2])
				{
					Settings.XMLLoad_CameraPlus();
				}
				else if (RealTimeView.m_isWork[0] || RealTimeView.m_isWork[1] || RealTimeView.m_isWork[2])
				{
					int ConnectedCamera = 3;
					if (RealTimeView.m_isWork[0]) ConnectedCamera--;
					if (RealTimeView.m_isWork[1]) ConnectedCamera--;
					if (RealTimeView.m_isWork[2]) ConnectedCamera--;
					ShowMessage("오류", ConnectedCamera + "대의 카메라 연결에 실패하였습니다!\n", "경고");
					Settings.XMLLoad_CameraPlus();
				}
				else
				{
					//ShowMessage("오류", "3대의 카메라 연결에 실패하였습니다!\n프로그램을 종료합니다!\n", "경고");
					ShowMessage("오류", "3대의 카메라 연결에 실패하였습니다!\n카메라 연결 후 다시 실행해 주세요!\n", "경고");
					Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
					this.Close();
					return;
				}

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

				navigationFrame2.SelectedPage = Settings_Page;
				Settings.txt_WorkUser_WorkFileSetting.Focus();
			}
			catch(Exception ex)
			{
				ShowMessage("오류", "3대의 카메라 연결에 실패하였습니다!\n" + ex.Message, "경고");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			LoadingAnimationEnd();
		}
		#region 내비게이션 클릭 이벤트
		private void NavigationClick(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				RealTimeView.popupControlContainer1.Visible = false;
				string NavigationName = ((SimpleButton)sender).Name;
				PastPageNo = NowPageNo;
				if (!Settings.UserSet)
				{
					ShowMessage("작업", "작업자 설정이 필요합니다!", "주의");
					return;
				}
				//if(NowPageNo == 4 && !Settings.ChangeCheck())
				//{
				//	ShowMessage("설정", "설정을 저장하지 않아 페이지를 변경할 수 없습니다!!", "경고");
				//	return;
				//}
				if (RealTimeView.IsRecord)
				{
					ShowMessage("녹화 중...", "녹화 중는 페이지를 변경할 수 없습니다!!", "경고");
					return;
				}
				if		(NowPageNo == 2) VideoCheck.PageChange();
				else if (NowPageNo == 3) VideoAnalysis.PageChange();

				switch (NavigationName)
				{
					case "btn_RealTimeView":
						NowPageNo = 1;
						navigationFrame2.SelectedPage = RealTimeView_Page;
						break;
					case "btn_VideoCheck":
						NowPageNo = 2;
						navigationFrame2.SelectedPage = VideoCheck_Page;
						break;
					case "btn_VideoAnalysis":
						NowPageNo = 3;
						navigationFrame2.SelectedPage = VideoAnalysis_Page;
						break;
					case "btn_Settings":
						NowPageNo = 4;
						navigationFrame2.SelectedPage = Settings_Page;
						//if (tg_LogSaveOnOff.IsOn) tg_LogSaveOnOff.IsOn = false;
						//else tg_LogSaveOnOff.IsOn = true;
						break;
				}
			}
			catch (Exception ex)
			{
				ShowMessage("오류", "페이지 전환 중 예외가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
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
			//Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
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
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			if (Settings != null)
			{
				//Settings.ProgramSetting.BasicCameraView = rb_Camera_ProgramSetting.SelectedIndex + 1;
				//Settings.ProgramSetting.ImageFileFormat = cb_ImgFormat_ProgramSetting.EditValue.ToString();
				//Settings.XMLSave(Settings.ProgramSetting, Settings.ProgramSetting.GetType(), "ProgramSetting.xml");
			}
			this.Hide();
			if (RealTimeView != null) RealTimeView.CameraClose();
			if (VideoCheck != null) VideoCheck.VideoClose();
			if (VideoAnalysis!= null) VideoAnalysis.VideoClose();
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

		private void Form1_SizeChanged(object sender, EventArgs e)
		{
			// 위치 보정
			System.Drawing.Point point = new System.Drawing.Point();
			point.X = RealTimeView.btn_VideoSetting.Location.X;
			point.Y = RealTimeView.Height - RealTimeView.btn_VideoSetting.Size.Height - RealTimeView.popupControlContainer1.Height;
			RealTimeView.popupControlContainer1.Location = point;

			// 크기 보정
			System.Drawing.Size size = new System.Drawing.Size();
			size.Width = RealTimeView.btn_VideoSetting.Size.Width;
			size.Height = RealTimeView.popupControlContainer1.Height;
			RealTimeView.popupControlContainer1.Size = size;
		}
	}
}
