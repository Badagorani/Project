using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
using OMRON_Camera_Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using WireExternalInspection.Properties;
//using System.Windows.Media;

namespace WireExternalInspection
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
	{
		public LogRecordProcessing Log;
		public XmlProcessing Xml;
		public ViewPage viewpage;
		public AnalysisPage analysispage;
		public Bitmap analysisbitmap;
		public SettingsPage settingspage;
		public int NowPageNo = 0;
		public int PastPageNo = 0;

		#region 스플래시 스크린
		private void SplashShow(string message = "")
		{
			FluentSplashScreenOptions op = new FluentSplashScreenOptions();
			if (message.Equals(""))
			{
				op.AppearanceTitle.ForeColor = Color.Black;
				op.AppearanceTitle.Font = new Font("맑은 고딕", 30);
				op.AppearanceSubtitle.ForeColor = Color.Black;
				op.AppearanceSubtitle.Font = new Font("맑은 고딕", 18);
				op.AppearanceRightFooter.ForeColor = Color.Black;
				op.AppearanceRightFooter.Font = new Font("맑은 고딕", 10);
				op.AppearanceLeftFooter.ForeColor = Color.Black;
				op.AppearanceLeftFooter.Font = new Font("맑은 고딕", 10);
				op.Title = "와이어 외선 검사";
				op.Subtitle = "Wire External Inspection";
				op.RightFooter = "준비";
				op.LeftFooter = "Copyrightⓒ 2023. Seinens Co.,Ltd.\nAll Rights reserved.";/*Copyright© 올바른 표기법은?*/
				op.LoadingIndicatorType = FluentLoadingIndicatorType.Dots;
				op.OpacityColor = Color.White;
				op.Opacity = 255;
				op.LogoImageOptions.SvgImage = Resources.KISRI_CI;
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
		#region 내비게이션 프레임 로딩
		public IOverlaySplashScreenHandle overlayHandle;
		public void LoadingAnimationStart()
		{
			overlayHandle = SplashScreenManager.ShowOverlayForm(
					navigationFrame1,
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
		#endregion
		private void SplashAction()
		{
			SplashShow();
			SplashShow("설정을 불러오는 중...");
			Thread.Sleep(2000);
			SplashShow("완료");
			SplashScreenManager.CloseForm();
			Thread.Sleep(1000);
		}
		public MainForm()
		{
			InitializeComponent();
			Log = new LogRecordProcessing(this);
			Xml = new XmlProcessing(this);
			//Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
		}
		private void MainForm_Load(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			SplashAction();
			//Image img = Image.FromFile(@"C:\Users\김태현\Documents\최동규_세인\와이어 외선 사용자 검사 프로그램 프로젝트\찍은 사진\바다사진.jfif");
		}
		private void MainForm_Shown(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			viewpage = new ViewPage(this);
			viewpage.Parent = ViewPage;
			viewpage.BringToFront();
			viewpage.Dock = DockStyle.Fill;
			viewpage.Show();
			analysispage = new AnalysisPage(this);
			analysispage.Parent = AnalysisPage;
			analysispage.BringToFront();
			analysispage.Dock = DockStyle.Fill;
			analysispage.Show();
		}
		public void PageChange(int PageNo)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			switch (PageNo)
			{
				case 1: navigationFrame1.SelectedPage = ViewPage;		break;
				case 2: navigationFrame1.SelectedPage = AnalysisPage;	break;
			}
		}
		private void navigationFrame1_SelectedPageChanged(object sender, DevExpress.XtraBars.Navigation.SelectedPageChangedEventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			if (navigationFrame1.SelectedPage == ViewPage)
			{
				PastPageNo = 2;
				NowPageNo = 1;
			}
			else
			{
				PastPageNo = 1;
				NowPageNo = 2;
				analysispage.ImageChange(analysisbitmap);
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
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			this.Hide();
			if (viewpage != null) viewpage.CameraClose();
			//if (VideoCheck != null) VideoCheck.VideoClose();
			//if (VideoAnalysis != null) VideoAnalysis.VideoClose();
			//Viewer_Thread.ViewSetting(NowSelectedCamNo, IsViewing);
			//Camera_Setting.DestroyCamera();
		}
		#endregion
	}
}
