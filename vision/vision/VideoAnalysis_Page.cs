using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Threading;
using vision.Properties;
using System.Diagnostics;

namespace vision
{
	public partial class VideoAnalysis_Page : XtraUserControl
	{
		Form1 MainForm;
		#region Controls
		VideoCapture[] VideoAnalysis_Videos = new VideoCapture[2];
		//double[] fps = new double[2];
		int[] fps = new int[2];
		bool[] IsPlaying = new bool[2];
		bool[] IsPaused = new bool[2];
		int[] Threshold1 = new int[2];
		int[] Threshold2 = new int[2];
		#endregion
		Thread[] Play = new Thread[2];
		public VideoAnalysis_Page(Form1 form)
		{
			InitializeComponent();
			this.MainForm = form;
			PanelSettings();
		}
		private void VideoAnalysis_Page_Load(object sender, EventArgs e)
		{
			MainForm.Log.LogWrite($"{MethodBase.GetCurrentMethod().Name}");
		}
		#region 페널 세팅
		private void PanelSettings()
		{
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
		#region 영상 분석 페이지 버튼 클릭 이벤트
		private void VideoAnalysis_ButtonActions1(object sender, EventArgs e)
		{
			string ButtonText = ((SimpleButton)sender).Text;
			try
			{
				switch (ButtonText)
				{
					case "파일 열기": VideoCheck_FileOpen1();		break;
					case "재 생"	:
					case "일시정지"	: VideoCheck_Play1(ButtonText);	break;
					case "정 지"	: VideoCheck_Stop1();			break;
					case "파일 닫기": VideoCheck_FileClose1();		break;
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "잘못된 버튼 이벤트 입니다!!\n" + ex.Message, "주의");
			}
		}
		#endregion
		#region 영상 분석 페이지 버튼 클릭 이벤트2
		private void VideoAnalysis_ButtonActions2(object sender, EventArgs e)
		{
			string ButtonText = ((SimpleButton)sender).Text;
			try
			{
				switch (ButtonText)
				{
					case "파일 열기": VideoCheck_FileOpen2(); break;
					case "재 생":
					case "일시정지": VideoCheck_Play2(ButtonText); break;
					case "정 지": VideoCheck_Stop2(); break;
					case "파일 닫기": VideoCheck_FileClose2(); break;
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "잘못된 버튼 이벤트 입니다!!\n" + ex.Message, "주의");
			}
		}
		#endregion
		private void VideoCheck_FileOpen1()
		{
			using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true })
			{
				ofd.Filter = "동영상 파일 (*.mp4;*.avi;*.mov;*.wmv;*.mkv)|*.mp4;*.avi;*.mov;*.wmv;*.mkv";
				ofd.InitialDirectory = @"C:\FS-MCS500POE_Video_Save";

				MainForm.LoadingAnimationStart();
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					//C:\FS - MCS500POE_Video_Save\Cam3\2023년 02월 16일 09시 21분 23초 녹화.mp4
					string FileName = ofd.FileName;
					VideoCapture VideoAnalysis_VideoCapture = new VideoCapture(FileName);
					VideoAnalysis_Videos[0] = VideoAnalysis_VideoCapture;

					if (!VideoAnalysis_VideoCapture.Open(FileName))
					{
						MainForm.ShowMessage("오류", "잘못된 영상 경로 입니다!!", "주의");
						MainForm.LoadingAnimationEnd();
						return;
					}
					VideoAnalysis_VideoCapture.PosFrames = 1;
					Mat imsimat = new Mat();
					VideoAnalysis_VideoCapture.Read(imsimat);
					VideoAnalysis_Video1.Image = BitmapConverter.ToBitmap(imsimat);
					VideoAnalysis_VideoCapture.PosFrames = 0;
					tbc_VideoAnalysisTrack1.Properties.Maximum = VideoAnalysis_VideoCapture.FrameCount;
					//fps[0] = VideoAnalysis_VideoCapture.Fps;
					fps[0] = (int)VideoAnalysis_VideoCapture.Fps;
					tbc_VideoAnalysisTrack1.Enabled = true;
					//lbc_VideoCheckFilePath.Text = ofd.InitialDirectory + @"\Cam1/Cam2/Cam3\" + FileName;
					lbc_VideoAnalysisFilePath1.Text = "  " + FileName;
				}
				MainForm.LoadingAnimationEnd();
			}
		}
		private void VideoCheck_Play1(string ButtonText)
		{
			try
			{
				if (lbc_VideoAnalysisFilePath1.Text.Equals("  파일 위치"))
				{
					MainForm.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
					return;
				}
				if (Play[0] == null)
				{
					IsPlaying[0] = true;
					Play[0] = new Thread(() =>
					{
						if (VideoAnalysis_Videos[0] != null)
						{
							//int videofps = (int)Math.Ceiling(fps[0]) / 1000;
							int videofps = 1000 / fps[0];
							Stopwatch st = new Stopwatch();
							st.Start();
							using (Mat image = new Mat())
							{
								Bitmap bmp;
								while (IsPlaying[0])
								{
									long started = st.ElapsedMilliseconds;
									if (IsPaused[0]) continue;
									if (VideoAnalysis_Videos[0].PosFrames == VideoAnalysis_Videos[0].FrameCount) break;
									if (VideoAnalysis_Videos[0].Read(image))
									{
										Invoke((Action)(() =>
										{
											bmp = BitmapConverter.ToBitmap(image);
											VideoAnalysis_Video1.Image = bmp;
										}));
									}
									Invoke((Action)(() =>
									{
										tbc_VideoAnalysisTrack1.Value = VideoAnalysis_Videos[0].PosFrames;
									}));
									int elapsed = (int)(st.ElapsedMilliseconds - started);
									int delay = videofps - elapsed;
									//Cv2.WaitKey(videofps);
									if (delay > 0) Cv2.WaitKey(delay);
								}
							}
						}
						else Play = null;
					});
					Play[0].Start();
				}
				if (ButtonText.Equals("재 생"))
				{
					IsPaused[0] = false;
					btn_VideoAnalysisPlayPause1.Text = "일시정지";
					btn_VideoAnalysisPlayPause1.ImageOptions.SvgImage = Resources.pause;
				}
				else if ((ButtonText.Equals("일시정지")))
				{
					IsPaused[0] = true;
					btn_VideoAnalysisPlayPause1.Text = "재 생";
					btn_VideoAnalysisPlayPause1.ImageOptions.SvgImage = Resources.next;
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "영상1 재생에 문제가 발생하였습니다!!\n" + ex.Message, "주의");
				return;
			}
		}
		private void VideoCheck_Stop1()
		{
			if (lbc_VideoAnalysisFilePath1.Text.Equals("  파일 위치"))
			{
				MainForm.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
				return;
			}
			IsPaused[0] = true;
			tbc_VideoAnalysisTrack1.Value = 0;

			VideoAnalysis_Videos[0].PosFrames = 1;
			Mat imsimat = new Mat();
			VideoAnalysis_Videos[0].Read(imsimat);
			VideoAnalysis_Video1.Image = BitmapConverter.ToBitmap(imsimat);
			VideoAnalysis_Videos[0].PosFrames = 0;

			btn_VideoAnalysisPlayPause1.Text = "재 생";
			btn_VideoAnalysisPlayPause1.ImageOptions.SvgImage = Resources.next;
		}
		private void VideoCheck_FileClose1()
		{
			if (lbc_VideoAnalysisFilePath1.Text.Equals("  파일 위치"))
			{
				MainForm.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
				return;
			}
			IsPaused[0] = true;
			btn_VideoAnalysisPlayPause1.Text = "재 생";
			btn_VideoAnalysisPlayPause1.ImageOptions.SvgImage = Resources.next;
			IsPlaying[0] = false;
			tbc_VideoAnalysisTrack1.Value = 0;
			tbc_VideoAnalysisTrack1.Enabled = false;

			VideoAnalysis_Video1.Image = null;
			VideoAnalysis_Videos[0].PosFrames = 0;
			VideoAnalysis_Videos[0].Release();

			lbc_VideoAnalysisFilePath1.Text = "  파일 위치";
			Play[0] = null;
		}
		private void tbc_VideoCheckTrack_Scroll1(object sender, EventArgs e)
		{
			MainForm.LoadingAnimationStart();
			if (IsPaused[0])
			{
				Mat image = new Mat();
				VideoAnalysis_Videos[0].PosFrames = tbc_VideoAnalysisTrack1.Value;
				VideoAnalysis_Videos[0].Read(image);
				Bitmap bmp = BitmapConverter.ToBitmap(image);
				VideoAnalysis_Video1.Image = bmp;
			}
			else
			{
				IsPaused[0] = true;
				VideoAnalysis_Videos[0].PosFrames = tbc_VideoAnalysisTrack1.Value;
				IsPaused[0] = false;
			}
			MainForm.LoadingAnimationEnd();
		}
		private void VideoCheck_TrackBar_ValueChanged1(object sender, EventArgs e)
		{
			if (tbc_VideoAnalysisTrack1.Value == VideoAnalysis_Videos[0].FrameCount)
			{
				VideoAnalysis_Videos[0].Open(lbc_VideoAnalysisFilePath1.Text);
				IsPaused[0] = true;
				IsPlaying[0] = false;
				tbc_VideoAnalysisTrack1.Value = 0;
				VideoAnalysis_Videos[0].PosFrames = 0;
				btn_VideoAnalysisPlayPause1.Text = "재 생";
				btn_VideoAnalysisPlayPause1.ImageOptions.SvgImage = Resources.next;
				Play[0] = null;
			}
		}
		private void VideoCheck_FileOpen2()
		{
			using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true })
			{
				ofd.Filter = "동영상 파일 (*.mp4;*.avi;*.mov;*.wmv;*.mkv)|*.mp4;*.avi;*.mov;*.wmv;*.mkv";
				ofd.InitialDirectory = @"C:\FS-MCS500POE_Video_Save";

				MainForm.LoadingAnimationStart();
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					//C:\FS - MCS500POE_Video_Save\Cam3\2023년 02월 16일 09시 21분 23초 녹화.mp4
					string FileName = ofd.FileName;
					VideoCapture VideoAnalysis_VideoCapture = new VideoCapture(FileName);
					VideoAnalysis_Videos[1] = VideoAnalysis_VideoCapture;

					if (!VideoAnalysis_VideoCapture.Open(FileName))
					{
						MainForm.ShowMessage("오류", "잘못된 영상 경로 입니다!!", "주의");
						MainForm.LoadingAnimationEnd();
						return;
					}
					VideoAnalysis_VideoCapture.PosFrames = 1;
					Mat imsimat = new Mat();
					VideoAnalysis_VideoCapture.Read(imsimat);
					VideoAnalysis_Video2.Image = BitmapConverter.ToBitmap(imsimat);
					VideoAnalysis_VideoCapture.PosFrames = 0;

					tbc_VideoAnalysisTrack2.Properties.Maximum = VideoAnalysis_VideoCapture.FrameCount;
					//fps[1] = VideoAnalysis_VideoCapture.Fps;
					fps[1] = (int)VideoAnalysis_VideoCapture.Fps;
					tbc_VideoAnalysisTrack2.Enabled = true;
					//lbc_VideoCheckFilePath.Text = ofd.InitialDirectory + @"\Cam1/Cam2/Cam3\" + FileName;
					lbc_VideoAnalysisFilePath2.Text = "  " + FileName;
				}
				MainForm.LoadingAnimationEnd();
			}
		}
		private void VideoCheck_Play2(string ButtonText)
		{
			try
			{
				if (lbc_VideoAnalysisFilePath2.Text.Equals("  파일 위치"))
				{
					MainForm.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
					return;
				}
				if (Play[1] == null)
				{
					IsPlaying[1] = true;
					Play[1] = new Thread(() =>
					{
						if (VideoAnalysis_Videos[1] != null)
						{
							//int videofps = (int)Math.Ceiling(fps[1]) / 1000;
							int videofps = 1000 / fps[1];
							Stopwatch st = new Stopwatch();
							st.Start();
							using (Mat image = new Mat())
							{
								Bitmap bmp;
								while (IsPlaying[1])
								{
									long started = st.ElapsedMilliseconds;
									if (IsPaused[1]) continue;
									if (VideoAnalysis_Videos[1].PosFrames == VideoAnalysis_Videos[1].FrameCount) break;
									if (VideoAnalysis_Videos[1].Read(image))
									{
										Invoke((Action)(() =>
										{
											Mat InputImage = new Mat();
											Cv2.CvtColor(image, InputImage, ColorConversionCodes.BGR2GRAY);
											Mat EdgeImage = new Mat();
											Cv2.Canny(InputImage, EdgeImage, Threshold1[1], Threshold2[1]);
											bmp = BitmapConverter.ToBitmap(EdgeImage);
											VideoAnalysis_Video2.Image = bmp;
										}));
									}
									Invoke((Action)(() =>
									{
										tbc_VideoAnalysisTrack2.Value = VideoAnalysis_Videos[1].PosFrames;
									}));
									int elapsed = (int)(st.ElapsedMilliseconds - started);
									int delay = videofps - elapsed;
									//Cv2.WaitKey(videofps);
									if (delay > 0) Cv2.WaitKey(delay);
								}
							}
						}
						else Play = null;
					});
					Play[1].Start();
				}
				if (ButtonText.Equals("재 생"))
				{
					IsPaused[1] = false;
					btn_VideoAnalysisPlayPause2.Text = "일시정지";
					btn_VideoAnalysisPlayPause2.ImageOptions.SvgImage = Resources.pause;
				}
				else if ((ButtonText.Equals("일시정지")))
				{
					IsPaused[1] = true;
					btn_VideoAnalysisPlayPause2.Text = "재 생";
					btn_VideoAnalysisPlayPause2.ImageOptions.SvgImage = Resources.next;
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "영상2 재생에 문제가 발생하였습니다!!\n" + ex.Message, "주의");
				return;
			}
		}
		private void VideoCheck_Stop2()
		{
			if (lbc_VideoAnalysisFilePath2.Text.Equals("  파일 위치"))
			{
				MainForm.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
				return;
			}
			IsPaused[1] = true;
			tbc_VideoAnalysisTrack2.Value = 0;

			VideoAnalysis_Videos[1].PosFrames = 1;
			Mat imsimat = new Mat();
			VideoAnalysis_Videos[1].Read(imsimat);
			VideoAnalysis_Video2.Image = BitmapConverter.ToBitmap(imsimat);
			VideoAnalysis_Videos[1].PosFrames = 0;

			btn_VideoAnalysisPlayPause2.Text = "재 생";
			btn_VideoAnalysisPlayPause2.ImageOptions.SvgImage = Resources.next;
		}
		private void VideoCheck_FileClose2()
		{
			if (lbc_VideoAnalysisFilePath2.Text.Equals("  파일 위치"))
			{
				MainForm.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
				return;
			}
			IsPaused[1] = true;
			btn_VideoAnalysisPlayPause2.Text = "재 생";
			btn_VideoAnalysisPlayPause2.ImageOptions.SvgImage = Resources.next;
			IsPlaying[1] = false;
			tbc_VideoAnalysisTrack2.Value = 0;
			tbc_VideoAnalysisTrack2.Enabled = false;

			VideoAnalysis_Video1.Image = null;
			VideoAnalysis_Videos[1].PosFrames = 0;
			VideoAnalysis_Videos[1].Release();

			lbc_VideoAnalysisFilePath2.Text = "  파일 위치";
			Play[1] = null;
		}
		private void tbc_VideoCheckTrack_Scroll2(object sender, EventArgs e)
		{
			MainForm.LoadingAnimationStart();
			if (IsPaused[1])
			{
				Mat image = new Mat();
				VideoAnalysis_Videos[1].PosFrames = tbc_VideoAnalysisTrack2.Value;
				VideoAnalysis_Videos[1].Read(image);
				Bitmap bmp = BitmapConverter.ToBitmap(image);
				VideoAnalysis_Video2.Image = bmp;
			}
			else
			{
				IsPaused[1] = true;
				VideoAnalysis_Videos[1].PosFrames = tbc_VideoAnalysisTrack2.Value;
				IsPaused[1] = false;
			}
			MainForm.LoadingAnimationEnd();
		}
		private void VideoCheck_TrackBar_ValueChanged2(object sender, EventArgs e)
		{
			if (tbc_VideoAnalysisTrack2.Value == VideoAnalysis_Videos[1].FrameCount)
			{
				VideoAnalysis_Videos[1].Open(lbc_VideoAnalysisFilePath2.Text);
				IsPaused[1] = true;
				IsPlaying[1] = false;
				tbc_VideoAnalysisTrack2.Value = 0;
				VideoAnalysis_Videos[1].PosFrames = 0;
				btn_VideoAnalysisPlayPause2.Text = "재 생";
				btn_VideoAnalysisPlayPause2.ImageOptions.SvgImage = Resources.next;
				Play[1] = null;
			}
		}
		public void PageChange()
		{
			for (int i = 0; i < VideoAnalysis_Videos.Length; i++)
			{
				IsPaused[i] = true;
			}
			btn_VideoAnalysisPlayPause1.Text = "재 생";
			btn_VideoAnalysisPlayPause1.ImageOptions.SvgImage = Resources.next;
			btn_VideoAnalysisPlayPause2.Text = "재 생";
			btn_VideoAnalysisPlayPause2.ImageOptions.SvgImage = Resources.next;
		}
		public void VideoClose()
		{
			try
			{
				if (Play != null)
				{
					for (int i = 0; i < VideoAnalysis_Videos.Length; i++)
					{
						IsPlaying[i] = false;
						if (Play[i] != null)
						{
							//Play[i].Interrupt();
							Play[i].Abort();
						}
						Play[i] = null;
					}
					Play = null;
				}
			}
			catch (Exception ex)
			{
				//MainForm.ShowMessage("종료", "영상 분석 종료 중에 오류가 발생하였습니다!!\n" + ex.Message, "주의");
			}
		}
		private void tbc_VideoAnalysisThreshold1_Scroll(object sender, EventArgs e)
		{
			Threshold1[1] = tbc_VideoAnalysisThreshold2_1.Value;
		}
		private void tbc_VideoAnalysisThreshold2_Scroll(object sender, EventArgs e)
		{
			Threshold2[1] = tbc_VideoAnalysisThreshold2_2.Value;
		}
	}
}
