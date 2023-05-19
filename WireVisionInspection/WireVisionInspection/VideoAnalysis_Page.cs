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
using WireVisionInspection.Properties;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;
using System.Numerics;
using DevExpress.Utils.Drawing.Helpers;
using Microsoft.WindowsAPICodePack.Shell;
using DevExpress.XtraBars.Docking2010.Views.Widget;
using System.Drawing.Imaging;
using System.IO;

namespace WireVisionInspection
{
	public partial class VideoAnalysis_Page : XtraUserControl
	{
		Form1 MainForm;
		Form2 SubForm;
		public LogRecord Log;
		#region Controls
		VideoCapture[] VideoAnalysis_Videos = new VideoCapture[2];
		//double[] fps = new double[2];
		int[] fps = new int[2];
		bool[] IsPlaying = new bool[2];
		bool[] IsPaused = new bool[2];
		int[] FilterCheck = new int[2];
		int[] VideoCamNo = new int[2];
		#endregion
		Thread[] Play = new Thread[2];
		public VideoAnalysis_Page(Form1 form)
		{
			InitializeComponent();
			this.MainForm = form;
			Log = MainForm.Log;
			PanelSettings();
			SubForm = new Form2();
			SubForm.Show();
		}
		private void VideoAnalysis_Page_Load(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
		}
		#region 페널 세팅
		private void PanelSettings()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
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
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "페널을 생성하는 중 예외가 발생하였습니다!" + ex.Message, "경고");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		#endregion
		#region 카메라 화면 닫기 / 다시열기
		private void PanelClosed(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				DockManager manager = ((DockPanel)sender).DockManager;
				SimpleButton sb = new SimpleButton();
				manager.Form.Parent.Controls[0].Controls.Add(sb);
				manager.Form.Parent.Controls[0].Controls.Add(manager.Panels[0]);
				sb.Font = new Font("맑은 고딕", 18F, FontStyle.Bold);
				sb.Text = "화면을 다시 열려면\n더블클릭하세요";
				sb.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
				sb.Dock = DockStyle.Fill;
				sb.DoubleClick += new EventHandler(PanelDoubleClick);
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "페널을 닫는 중 예외가 발생하였습니다!" + ex.Message, "경고");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		private void PanelDoubleClick(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				Control control = ((SimpleButton)sender).Parent;
				DockPanel dp = (DockPanel)control.Controls[1];
				control.Controls.Clear();
				dp.Visibility = DockVisibility.Visible;
				dp.Dock = DockingStyle.Fill;
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "페널을 다시 여는 중 예외가 발생하였습니다!" + ex.Message, "경고");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		#endregion
		#region 영상 분석 페이지 버튼 클릭 이벤트
		private void VideoAnalysis_ButtonActions1(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
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
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		#endregion
		#region 영상 분석 페이지 버튼 클릭 이벤트2
		private void VideoAnalysis_ButtonActions2(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
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
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		#endregion
		private void VideoCheck_FileOpen1()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true })
				{
					ofd.Filter = "동영상 파일 (*.mp4;*.avi;*.mov;*.wmv;*.mkv)|*.mp4;*.avi;*.mov;*.wmv;*.mkv";
					ofd.InitialDirectory = @"C:\FS-MCS500POE_Video_Save";

					MainForm.LoadingAnimationStart();
					if (ofd.ShowDialog() == DialogResult.OK)
					{
						if (IsPlaying[0]) VideoCheck_FileClose1();
						//C:\FS - MCS500POE_Video_Save\Cam3\2023년 02월 16일 09시 21분 23초 녹화.mp4
						string FileName = ofd.FileName;
						VideoCapture VideoAnalysis_VideoCapture = new VideoCapture(FileName);
						VideoAnalysis_Videos[0] = VideoAnalysis_VideoCapture;
						VideoCamNo[0] = int.Parse(FileName.Substring(FileName.LastIndexOf("Cam") + 3, 1));

						if (!VideoAnalysis_VideoCapture.Open(FileName))
						{
							MainForm.ShowMessage("오류", "잘못된 영상 경로 입니다!!", "주의");
							MainForm.LoadingAnimationEnd();
							return;
						}
						VideoAnalysis_VideoCapture.PosFrames = 1;
						Mat image = new Mat();
						VideoAnalysis_VideoCapture.Read(image);
						VideoAnalysis_Video1.Image = BitmapConverter.ToBitmap(FilterSet(image, VideoCamNo[0], FilterCheck[0]));
						VideoAnalysis_VideoCapture.PosFrames = 0;
						tbc_VideoAnalysisTrack1.Properties.Maximum = VideoAnalysis_VideoCapture.FrameCount;
						//fps[0] = VideoAnalysis_VideoCapture.Fps;
						fps[0] = (int)VideoAnalysis_VideoCapture.Fps;
						tbc_VideoAnalysisTrack1.Enabled = true;
						//lbc_VideoCheckFilePath.Text = ofd.InitialDirectory + @"\Cam1/Cam2/Cam3\" + FileName;
						lbc_VideoAnalysisFilePath1.Text = "  " + FileName;
						IsPaused[0] = true;
					}
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "영상1 여는 중 예외가 발생하였습니다!\n영상은 'Cam「1 ~ 3」' 폴더 안에 있어야 합니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			MainForm.LoadingAnimationEnd();
		}
		private void VideoPlay1()
		{
			IsPlaying[0] = true;
			if (VideoAnalysis_Videos[0] != null)
			{
				//int videofps = (int)Math.Ceiling(fps[0]) / 1000;
				int videofps = 1000 / fps[0];
				Stopwatch st = new Stopwatch();
				st.Start();
				using (Mat image = new Mat())
				{
					EventWaitHandle handle = new EventWaitHandle(false, EventResetMode.AutoReset);
					Bitmap bmp;
					while (IsPlaying[0])
					{
						handle.Set();
						long started = st.ElapsedMilliseconds;
						if (IsPaused[0]) continue;
						if (VideoAnalysis_Videos[0].PosFrames == VideoAnalysis_Videos[0].FrameCount) break;
						if (VideoAnalysis_Videos[0].Read(image))
						{
							Invoke((Action)(() =>
							{
								bmp = BitmapConverter.ToBitmap(FilterSet(image, VideoCamNo[0], FilterCheck[0]));
								if(VideoAnalysis_Video1.Image != null)
								{
									//var beforeimage = VideoAnalysis_Video1.Image;
									VideoAnalysis_Video1.Image = null;
									VideoAnalysis_Video1.Image = bmp;
									VideoAnalysis_Video1.Refresh();
									//handle.WaitOne();
									//beforeimage.Dispose();
								}
							}));
						}
						Invoke((Action)(() =>
						{
							tbc_VideoAnalysisTrack1.Value = VideoAnalysis_Videos[0].PosFrames;
						}));
						int elapsed = (int)(st.ElapsedMilliseconds - started);
						int delay = videofps - elapsed;
						//Cv2.WaitKey(videofps1
						if (delay > 0) Cv2.WaitKey(delay);

						Console.WriteLine("Video 01 : " + delay);
						//if (IsPlaying[1] && !IsPaused[1]) Thread.Sleep(30);

						//WaitHandle.
					}
				}
			}
			else Play = null;
		}
		private void VideoCheck_Play1(string ButtonText)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				if (lbc_VideoAnalysisFilePath1.Text.Equals("  파일 위치"))
				{
					MainForm.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
					return;
				}
				if (Play[0] == null)
				{
					#region 스레드 람다식 구현1
					/*IsPlaying[0] = true;
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
											bmp = BitmapConverter.ToBitmap(FilterSet(image, VideoCamNo[0], FilterCheck[0]));
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
									if (IsPlaying[1] && !IsPaused[1]) Thread.Sleep(30);
								}
							}
						}
						else Play = null;
					});*/
					#endregion
					Play[0] = new Thread(new ThreadStart(VideoPlay1));
					//Play[0].IsBackground = true;
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
				MainForm.ShowMessage("오류", "영상1 재생 중 예외가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
				return;
			}
		}
		private void VideoCheck_Stop1()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				if (lbc_VideoAnalysisFilePath1.Text.Equals("  파일 위치"))
				{
					MainForm.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
					return;
				}
				IsPaused[0] = true;
				tbc_VideoAnalysisTrack1.Value = 0;

				VideoAnalysis_Videos[0].PosFrames = 1;
				Mat image = new Mat();
				VideoAnalysis_Videos[0].Read(image);
				VideoAnalysis_Video1.Image = BitmapConverter.ToBitmap(FilterSet(image, VideoCamNo[0], FilterCheck[0]));
				VideoAnalysis_Videos[0].PosFrames = 0;

				btn_VideoAnalysisPlayPause1.Text = "재 생";
				btn_VideoAnalysisPlayPause1.ImageOptions.SvgImage = Resources.next;
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "영상1 정지 중 예외가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		private void VideoCheck_FileClose1()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
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
				VideoAnalysis_Videos[0] = null;

				lbc_VideoAnalysisFilePath1.Text = "  파일 위치";
				Play[0] = null;
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "영상1 닫기 중 예외가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		private void tbc_VideoCheckTrack_Scroll1(object sender, EventArgs e)
		{
			if (IsPaused[0])
			{
				Mat image = new Mat();
				VideoAnalysis_Videos[0].PosFrames = tbc_VideoAnalysisTrack1.Value;
				VideoAnalysis_Videos[0].Read(image);
				Bitmap bmp = BitmapConverter.ToBitmap(FilterSet(image, VideoCamNo[0], FilterCheck[0]));
				VideoAnalysis_Video1.Image = bmp;
			}
			else
			{
				IsPaused[0] = true;
				VideoAnalysis_Videos[0].PosFrames = tbc_VideoAnalysisTrack1.Value;
				IsPaused[0] = false;
			}
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
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true })
				{
					ofd.Filter = "동영상 파일 (*.mp4;*.avi;*.mov;*.wmv;*.mkv)|*.mp4;*.avi;*.mov;*.wmv;*.mkv";
					ofd.InitialDirectory = @"C:\FS-MCS500POE_Video_Save";

					MainForm.LoadingAnimationStart();
					if (ofd.ShowDialog() == DialogResult.OK)
					{
						if (IsPlaying[1]) VideoCheck_FileClose2();
						//C:\FS - MCS500POE_Video_Save\Cam3\2023년 02월 16일 09시 21분 23초 녹화.mp4
						string FileName = ofd.FileName;
						VideoCapture VideoAnalysis_VideoCapture = new VideoCapture(FileName);
						VideoAnalysis_Videos[1] = VideoAnalysis_VideoCapture;
						VideoCamNo[1] = int.Parse(FileName.Substring(FileName.LastIndexOf("Cam") + 3, 1));

						if (!VideoAnalysis_VideoCapture.Open(FileName))
						{
							MainForm.ShowMessage("오류", "잘못된 영상 경로 입니다!!", "주의");
							MainForm.LoadingAnimationEnd();
							return;
						}
						VideoAnalysis_VideoCapture.PosFrames = 1;
						Mat image = new Mat();
						VideoAnalysis_VideoCapture.Read(image);
						VideoAnalysis_Video2.Image = BitmapConverter.ToBitmap(FilterSet(image, VideoCamNo[1], FilterCheck[1]));
						VideoAnalysis_VideoCapture.PosFrames = 0;

						tbc_VideoAnalysisTrack2.Properties.Maximum = VideoAnalysis_VideoCapture.FrameCount;
						//fps[1] = VideoAnalysis_VideoCapture.Fps;
						fps[1] = (int)VideoAnalysis_VideoCapture.Fps;
						tbc_VideoAnalysisTrack2.Enabled = true;
						//lbc_VideoCheckFilePath.Text = ofd.InitialDirectory + @"\Cam1/Cam2/Cam3\" + FileName;
						lbc_VideoAnalysisFilePath2.Text = "  " + FileName;
						IsPaused[1] = true;
					}
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "영상2 여는 중 예외가 발생하였습니다!" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			MainForm.LoadingAnimationEnd();
		}
		private void VideoPlay2()
		{
			IsPlaying[1] = true;
			if (VideoAnalysis_Videos[1] != null)
			{
				//int videofps = (int)Math.Ceiling(fps[1]) / 1000;
				int videofps = 1000 / fps[1];
				Stopwatch st = new Stopwatch();
				st.Start();
				using (Mat image = new Mat())
				{
					EventWaitHandle handle = new EventWaitHandle(false, EventResetMode.AutoReset);
					Bitmap bmp;
					while (IsPlaying[1])
					{
						handle.Set();
						long started = st.ElapsedMilliseconds;
						if (IsPaused[1]) continue;
						if (VideoAnalysis_Videos[1].PosFrames == VideoAnalysis_Videos[1].FrameCount) break;
						if (VideoAnalysis_Videos[1].Read(image))
						{
							Invoke((Action)(() =>
							{
								bmp = BitmapConverter.ToBitmap(FilterSet(image, VideoCamNo[1], FilterCheck[1]));
								if (VideoAnalysis_Video2.Image != null)
								{
									//var beforeimage = VideoAnalysis_Video2.Image;
									VideoAnalysis_Video2.Image = null;
									VideoAnalysis_Video2.Image = bmp;
									VideoAnalysis_Video2.Refresh();
									//handle.WaitOne();
									//beforeimage.Dispose();
								}
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
						Console.WriteLine("Video 02 : " + delay);

						Thread.Sleep(1);
					}
				}
			}
			else Play = null;
		}
		private void VideoCheck_Play2(string ButtonText)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				if (lbc_VideoAnalysisFilePath2.Text.Equals("  파일 위치"))
				{
					MainForm.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
					return;
				}
				if (Play[1] == null)
				{
					#region 스레드 람다식 구현2
					/*IsPlaying[1] = true;
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
											bmp = BitmapConverter.ToBitmap(FilterSet(image, VideoCamNo[1], FilterCheck[1]));
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
					});*/
					#endregion
					Play[1] = new Thread(new ThreadStart(VideoPlay2));
					//Play[1].IsBackground = true;
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
				MainForm.ShowMessage("오류", "영상2 재생 중 예외가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
				return;
			}
		}

		private void VideoCheck_Stop2()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				if (lbc_VideoAnalysisFilePath2.Text.Equals("  파일 위치"))
				{
					MainForm.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
					return;
				}
				IsPaused[1] = true;
				tbc_VideoAnalysisTrack2.Value = 0;

				VideoAnalysis_Videos[1].PosFrames = 1;
				Mat image = new Mat();
				VideoAnalysis_Videos[1].Read(image);
				VideoAnalysis_Video2.Image = BitmapConverter.ToBitmap(FilterSet(image, VideoCamNo[1], FilterCheck[1]));
				VideoAnalysis_Videos[1].PosFrames = 0;

				btn_VideoAnalysisPlayPause2.Text = "재 생";
				btn_VideoAnalysisPlayPause2.ImageOptions.SvgImage = Resources.next;
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "영상2 정지 중 예외가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		private void VideoCheck_FileClose2()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
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

				VideoAnalysis_Video2.Image = null;
				VideoAnalysis_Videos[1].PosFrames = 0;
				VideoAnalysis_Videos[1].Release();
				VideoAnalysis_Videos[1] = null;

				lbc_VideoAnalysisFilePath2.Text = "  파일 위치";
				Play[1] = null;
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "영상2 닫기 중 예외가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		private void tbc_VideoCheckTrack_Scroll2(object sender, EventArgs e)
		{
			if (IsPaused[1])
			{
				Mat image = new Mat();
				VideoAnalysis_Videos[1].PosFrames = tbc_VideoAnalysisTrack2.Value;
				VideoAnalysis_Videos[1].Read(image);
				Bitmap bmp = BitmapConverter.ToBitmap(FilterSet(image, VideoCamNo[1], FilterCheck[1]));
				VideoAnalysis_Video2.Image = bmp;
			}
			else
			{
				IsPaused[1] = true;
				VideoAnalysis_Videos[1].PosFrames = tbc_VideoAnalysisTrack2.Value;
				IsPaused[1] = false;
			}
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
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
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
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "영상 분석 페이지 전환 중 예외가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		public void VideoClose()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
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
				MainForm.ShowMessage("종료", "영상 분석 종료 중 예외가 발생하였습니다!!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		private Mat FilterSet(Mat InputMat, int VideoCamNo, int Filter/* = 1*/)
		{
			Stopwatch sw = Stopwatch.StartNew();
			if (InputMat == null) return InputMat;

			Mat OutputMat = InputMat.Clone();
			try
			{
				Mat GrayMat = new Mat();
				Mat EdgeMat = new Mat();
				//Mat BlurImg = new Mat();
				Cv2.CvtColor(InputMat, GrayMat, ColorConversionCodes.BGR2GRAY);
				Mat BinaryMat = new Mat();
				Cv2.Threshold(GrayMat, BinaryMat, 145, 255, ThresholdTypes.Binary);
				//Mat imsibinary = new Mat();
				//Cv2.Resize(BinaryMat, imsibinary, new OpenCvSharp.Size(0, 0), 0.3, 0.3);
				//Cv2.ImShow("binary", imsibinary);
				//Cv2.GaussianBlur(InputMat, BlurImg, new OpenCvSharp.Size(3, 3), 1, 0, BorderTypes.Default);
				switch (Filter)
				{
					case 2: Cv2.Canny(BinaryMat, EdgeMat, 170, 230, 3, true);/*60,200*/	break;
					case 3: Cv2.Sobel(BinaryMat, EdgeMat, MatType.CV_8U, 1, 0); break;
					case 4: Cv2.Scharr(BinaryMat, EdgeMat, MatType.CV_8U, 1, 0); break;
					case 5: Cv2.Laplacian(BinaryMat, EdgeMat, MatType.CV_8U, 5); break;
					//case 2	: Cv2.Canny(GrayMat, EdgeMat, Threshold1[0], Threshold2[0], 3, true);/*60,200*/	break;
					//case 3	: Cv2.Sobel(GrayMat, EdgeMat, MatType.CV_8U, 1, 1, ksize: 3, scale: 1, delta: 0, BorderTypes.Default);	break;
					//case 4	: Cv2.Scharr(GrayMat, EdgeMat, MatType.CV_8U, 1, 0, scale: 1, delta: 0, BorderTypes.Default);				break;
					//case 5	: Cv2.Laplacian(GrayMat, EdgeMat, MatType.CV_8U, ksize: 3, scale: 1, delta: 0, BorderTypes.Default);		break;
					default: OutputMat = InputMat; return OutputMat;
				}
				//if (Filter > 2) EdgeMat.ConvertTo(EdgeMat, MatType.CV_8UC1);
				#region 도형찾기실패
				/*// 도형 찾기
				OpenCvSharp.Point[][] contours;
				HierarchyIndex[] hierarchy;
				Cv2.FindContours(FilterMat, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

				//// 도형 내부 파란색으로 채우기
				//Mat square = new Mat();
				//Cv2.CvtColor(FilterMat, OutputMat, ColorConversionCodes.GRAY2BGR);
				//foreach (var contour in contours)
				//{
				//	Cv2.DrawContours(OutputMat, new[] { contour }, -1, new Scalar(255, 0, 0), -1);
				//	//Cv2.FloodFill(OutputMat, new[] { contour }, )
				//}

				// 도형 내부 채우기
				OutputMat = FilterMat.Clone();
				OpenCvSharp.Size MatResize = new OpenCvSharp.Size(FilterMat.Size().Width + 2, FilterMat.Size().Height + 2);
				FilterMat = FilterMat.Resize(MatResize);
				Mat mask = new Mat(FilterMat.Size(), MatType.CV_8UC1, new Scalar(0));
				foreach (var contour in contours)
				{
					Cv2.DrawContours(mask, new[] { contour }, -1, new Scalar(255, 0, 0), -1);
				}

				//OutputMat = FilterMat.Clone();
				//OutputMat.Resize(OutputMat.Size().Width + 2, OutputMat.Size().Height + 2);
				Cv2.FloodFill(OutputMat, mask, new OpenCvSharp.Point(0, 0), new Scalar(255, 0, 0));*/
				#endregion
				#region 인터넷 C++ 소스 문서 보고 만듦(안됨) https://poorman.tistory.com/184
				/*Mat kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(11, 11));
				Mat morph = new Mat();
				Cv2.MorphologyEx(EdgeMat, morph, MorphTypes.Close, kernel);

				//Vector<Vector<OpenCvSharp.Point>> contours;
				//Vector<Vec4i> hierarchy;
				OpenCvSharp.Point[][] contours;
				HierarchyIndex[] hierarchy;
				Cv2.FindContours(morph, out contours, out hierarchy, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple, new OpenCvSharp.Point(0, 0));

				Mat contours_img = new Mat();
				Cv2.CvtColor(EdgeMat, contours_img, ColorConversionCodes.GRAY2BGR);
				for (int i = 0; i < contours.Length; i++)
				{
					RotatedRect rect = Cv2.MinAreaRect(contours[i]);
					//double areaRatio = Math.Abs(Cv2.ContourArea(contours[i])) / (rect.Size.Width * rect.Size.Height);
					Cv2.DrawContours(contours_img, contours, i, new Scalar(255, 0, 0), 2);
				}

				//OpenCvSharp.Point[] poly;
				//Vector<OpenCvSharp.Point> poly;
				List<OpenCvSharp.Point> poly = new List<OpenCvSharp.Point>();
				Mat poly_img = new Mat();
				Mat rectangle_img = new Mat();
				Cv2.CvtColor(EdgeMat, poly_img, ColorConversionCodes.GRAY2BGR);
				Cv2.CvtColor(EdgeMat, rectangle_img, ColorConversionCodes.GRAY2BGR);
				for (int i = 0; i < contours.Length; i++)
				{
					Cv2.ApproxPolyDP(contours[i], poly, 1, true);
					for (int j = 0; j < poly.Count; j++)
					{
						Cv2.Line(poly_img, poly[j], poly[(j + 1) % poly.Count], new Scalar(0, 255, 0), 2);
						if(poly.Count == 4) Cv2.Line(rectangle_img, poly[j], poly[(j + 1) % poly.Count], new Scalar(0, 0, 255), 2);
					}
				}*/
				#endregion
				#region 인터넷 소스 문서 보고 만듦(안됨) https://bigenergy.tistory.com/entry/C-OpenCvSharp%EC%9D%84-%EC%9D%B4%EC%9A%A9%ED%95%9C-%EC%82%AC%EA%B0%81%ED%98%95-%EA%B2%80%EC%B6%9C-%EB%B0%A9%EB%B2%95
				/*OpenCvSharp.Point testpoint = new OpenCvSharp.Point();
				OpenCvSharp.Point[][] contours;
				HierarchyIndex[] hierarchy;
				InputArray ia;
				Cv2.FindContours(EdgeMat, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
				OpenCvSharp.Size cvsize;
				cvsize.Width = EdgeMat.Width;
				cvsize.Height = EdgeMat.Height;
				for (int i = 0; i < contours.Length; i++)
				{
					double peri = Cv2.ArcLength(contours[i], true);

					OpenCvSharp.Point[] pp = Cv2.ApproxPolyDP(contours[i], 0.02 * peri, true);

					RotatedRect rrect = Cv2.MinAreaRect(pp);
					double areaRatio = Math.Abs(Cv2.ContourArea(contours[i], false)) / (rrect.Size.Width * rrect.Size.Height);

					if (pp.Length == 4) Cv2.DrawContours(EdgeMat, contours, i, new Scalar(0, 255, 0), 1, LineTypes.AntiAlias, hierarchy, 100);
					else Cv2.DrawContours(EdgeMat, contours, i, new Scalar(0, 0, 255), 1, LineTypes.AntiAlias, hierarchy, 100);

				}*/
				#endregion
				#region ???
				//Mat src = InputMat;
				//Mat src2 = new Mat();
				//src.CopyTo(OutputMat);
				//OpenCvSharp.Point[][] contours;
				//HierarchyIndex[] hierarchy;
				//Mat bin = new Mat();
				//Cv2.CvtColor(src, bin, ColorConversionCodes.BGR2GRAY);
				//Cv2.Threshold(bin, bin, 127, 255, ThresholdTypes.Binary);

				//Mat hierarchy1 = new Mat();
				//Cv2.FindContours(bin, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

				//for (int i = 0; i < contours.Length; i++)
				//{
				//	Cv2.DrawContours(OutputMat, contours, i, Scalar.Blue, 3, LineTypes.AntiAlias);
				//	for(int j = 0; j < contours[i].Length; j++)
				//	{
				//		Cv2.PutText(OutputMat, i.ToString(), contours[i][j], HersheyFonts.Italic, 2, Scalar.Black);
				//	}
				//}

				//Cv2.FindContours(EdgeMat, out OpenCvSharp.Point[][] contour2, out HierarchyIndex[] hierarchy2,
				//	RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

				//for (int i = 0; i < contour2.Length; i++)
				//{
				//	Cv2.DrawContours(OutputMat, contour2, i, new Scalar(0, 255, 255), 3, LineTypes.AntiAlias);
				//}
				#endregion
				#region 사각형 색칠 성공 https://stackoverflow.com/questions/72067943/how-do-i-fill-specific-parts-in-the-image-with-canny-edge-detection-open-cv
				/*if (Filter == 2)
				{
					Mat kernelEllipse = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(3, 3));
					Mat dilate = new Mat();
					Cv2.Dilate(EdgeMat, dilate, kernelEllipse, new OpenCvSharp.Point(-1, -1), 1, BorderTypes.Reflect);

					Mat[] contours;
					Mat hierarchy = new Mat();
					Cv2.FindContours(dilate.Clone(), out contours, hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

					OutputMat = EdgeMat.Clone();
					for (int i = 0; i < contours.Length; i++)
					{
						double area = Cv2.ContourArea(contours[i]);
						if (area > 2000)
						{
							Cv2.DrawContours(OutputMat, contours, i, Scalar.Blue, -1);
						}
					}
				}
				else OutputMat = EdgeMat;*/
				#endregion
				#region 사각형 색칠, 좌표 표시, 길이 표시
				if (Filter == 2)
				{
					//Mat imsicanny = new Mat();
					//Cv2.Resize(EdgeMat, imsicanny, new OpenCvSharp.Size(0, 0), 0.3, 0.3);
					//Cv2.ImShow("canny", imsicanny);
					Mat dilate = new Mat();
					Mat kernelEllipse = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(13, 13));
					Cv2.Dilate(EdgeMat, dilate, kernelEllipse, new OpenCvSharp.Point(-1, -1), 1, BorderTypes.Reflect);
					//Mat imsidilate = new Mat();
					//Cv2.Resize(dilate, imsidilate, new OpenCvSharp.Size(0, 0), 0.5, 0.5);
					//Cv2.ImShow("dilate", imsidilate);

					// 윤곽선 찾기
					OpenCvSharp.Point[][] contours;
					HierarchyIndex[] hierarchy;
					Cv2.FindContours(dilate, out contours, out hierarchy, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);

					List<OpenCvSharp.Point> vertex = new List<OpenCvSharp.Point>();
					Console.WriteLine($"\n");
					int minLen, maxLen;
					double avrLen = 0;
					foreach(OpenCvSharp.Point[] p in contours)
					{
						avrLen += Cv2.ArcLength(p, true);
					}
					Console.WriteLine("평균 길이 : " + (avrLen / contours.Length).ToString());
					if((avrLen / contours.Length) < 1300)
					{
						minLen = 1000;
						maxLen = 1850;
					}
					else
					{
						minLen = 1800;
						maxLen = 3200;
					}
					for (int i = 0; i < contours.Length; i++)
					{
						double length = Cv2.ArcLength(contours[i], true);
						if (length > maxLen/*3200*/ || length < minLen/*1800*/) continue;
						OpenCvSharp.Point[] pp = Cv2.ApproxPolyDP(contours[i], 0.02 * length, true);
						//RotatedRect rrect = Cv2.MinAreaRect(pp);
						if (pp.Length == 4)
						{
							Console.WriteLine($"순서 : {i}, 길이 : {length}");
							Cv2.DrawContours(OutputMat, contours, i, Scalar.Red, -1, LineTypes.AntiAlias, hierarchy);
							//Console.WriteLine($"성공 길이 : {length}");
							List<OpenCvSharp.Point> imsipoint = new List<OpenCvSharp.Point>();
							for (int j = 0; j < pp.Length; j++) imsipoint.Add(pp[j]);
							SortPointsClockwise(imsipoint);
							for (int j = 0; j < imsipoint.Count; j++) vertex.Add(imsipoint[j]);
							//Mat imsiout = new Mat();
							//Cv2.Resize(OutputMat, imsiout, new OpenCvSharp.Size(0, 0), 0.5, 0.5);
							//Cv2.ImShow($"contours : {i} imsiout", imsiout);
						}
					}
					//List<OpenCvSharp.Point> imsivertex = new List<OpenCvSharp.Point>();
					//for (int i = 3; i < vertex.Count; i += 4)
					//{
					//	imsivertex.Add(LengthWritePoint(vertex[i], vertex[i - 2]));
					//}
					//SortPointsClockwise(imsivertex);
					//List<OpenCvSharp.Point> sortedvertex = new List<OpenCvSharp.Point>();
					//for (int i = 0; i < imsivertex.Count; i ++)
					//{
					//	for (int j = 3; j < vertex.Count; j += 4)
					//	{
					//		if (imsivertex[i] == LengthWritePoint(vertex[j], vertex[j - 2]))
					//		{
					//			sortedvertex.Add(vertex[j - 3]);
					//			sortedvertex.Add(vertex[j - 2]);
					//			sortedvertex.Add(vertex[j - 1]);
					//			sortedvertex.Add(vertex[j - 0]);
					//		}
					//	}
					//}
					List<OpenCvSharp.Point> RectanglePoint = new List<OpenCvSharp.Point>();
					List<double> RectangleLengths = new List<double>();
					for (int i = 0; i < vertex.Count; i++)
					{
						#region 과거 코드
						//if (i % 4 != 3)
						//{
						//	double PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), VideoCamNo);
						//	Cv2.PutText(OutputMat, (i % 4 + 1).ToString() + $"면 : {PointLength}cm", LengthWritePoint(vertex[i], vertex[i + 1]), HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 2);
						//	Console.WriteLine(i + $"면 : {PointLength}cm");
						//}
						//else
						//{
						//	double PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i - 3]), VideoCamNo);
						//	Cv2.PutText(OutputMat, (i % 4 + 1).ToString() + $"면 : {PointLength}cm", LengthWritePoint(vertex[i], vertex[i - 3]), HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 2);
						//	Console.WriteLine(i + $"면 : {PointLength}cm");
						//}
						#endregion
						//if (i % 4 == 0)
						//{
						//	double PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), VideoCamNo);
						//	if (PointLength < 2)
						//	{
						//		i += 3;
						//		continue;
						//	}
						//	OpenCvSharp.Point writepoint = LengthWritePoint(vertex[i], vertex[i + 1]);
						//	writepoint.Y += 30;
						//	Cv2.PutText(OutputMat, i % 4 + 1 + $"side : {PointLength}cm", writepoint, HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 2);
						//	Console.WriteLine(i % 4 + 1 + $"면 : {PointLength}cm");
						//}
						//else if (i % 4 == 1)
						//{
						//	double PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), VideoCamNo);
						//	OpenCvSharp.Point writepoint = LengthWritePoint(vertex[i], vertex[i + 1]);
						//	writepoint.X -= 15;
						//	writepoint.Y += 40;
						//	Cv2.PutText(OutputMat, i % 4 + 1 + $"side : {PointLength}cm", writepoint, HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 2);
						//	Console.WriteLine(i % 4 + 1 + $"면 : {PointLength}cm");
						//}
						//else if (i % 4 == 2)
						//{
						//	double PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), VideoCamNo);
						//	OpenCvSharp.Point writepoint = LengthWritePoint(vertex[i], vertex[i + 1]);
						//	writepoint.Y -= 30;
						//	Cv2.PutText(OutputMat, i % 4 + 1 + $"side : {PointLength}cm", writepoint, HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 2);
						//	Console.WriteLine(i % 4 + 1 + $"면 : {PointLength}cm");
						//}
						//else if (i % 4 == 3)
						//{
						//	double PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i - 3]), VideoCamNo);
						//	OpenCvSharp.Point writepoint = LengthWritePoint(vertex[i], vertex[i - 3]);
						//	writepoint.X += 15;
						//	writepoint.Y -= 40;
						//	Cv2.PutText(OutputMat, i % 4 + 1 + $"side : {PointLength}cm", writepoint, HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 2);
						//	Console.WriteLine(i % 4 + 1 + $"면 : {PointLength}cm");
						//	Cv2.PutText(OutputMat, (i / 4 + 1).ToString(), LengthWritePoint(vertex[i], vertex[i - 2]), HersheyFonts.HersheyScriptSimplex, 5, Scalar.Blue, 5);
						//	Console.WriteLine($"오차 : cm");
						//}
						double PointLength = 0;
						OpenCvSharp.Point writepoint = new OpenCvSharp.Point();
						switch (i % 4)
						{
							case 0:
								PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), VideoCamNo);
								if (PointLength < 2)
								{
									i += 3;
									continue;
								}
								writepoint = LengthWritePoint(vertex[i], vertex[i + 1]);
								writepoint.X -= 120;
								writepoint.Y += 50;
								break;
							case 1:
								PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), VideoCamNo);
								writepoint = LengthWritePoint(vertex[i], vertex[i + 1]);
								writepoint.X -= 105;
								writepoint.Y += 50;
								break;
							case 2:
								PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), VideoCamNo);
								writepoint = LengthWritePoint(vertex[i], vertex[i + 1]);
								writepoint.X -= 120;
								writepoint.Y -= 20;
								break;
							case 3:
								PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i - 3]), VideoCamNo);
								writepoint = LengthWritePoint(vertex[i], vertex[i - 3]);
								writepoint.X -= 105;
								writepoint.Y -= 10;
								RectanglePoint.Add(LengthWritePoint(vertex[i], vertex[i - 2]));
								if((i / 4 + 1) < 10)
								{
									Cv2.PutText(OutputMat, (i / 4 + 1).ToString(), (LengthWritePoint(vertex[i], vertex[i - 2]) - new OpenCvSharp.Point(50, -50)), 
									HersheyFonts.HersheyScriptSimplex, 5, Scalar.Blue, 5);
								}
								else
								{
									Cv2.PutText(OutputMat, (i / 4 + 1).ToString(), (LengthWritePoint(vertex[i], vertex[i - 2]) - new OpenCvSharp.Point(100, -50)),
									HersheyFonts.HersheyScriptSimplex, 5, Scalar.Blue, 5);
								}
								break;
						}
						RectangleLengths.Add(PointLength);
						Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{PointLength}cm", writepoint, HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
						
						//Console.WriteLine(i % 4 + 1 + $"면 : {PointLength}cm");
					}
					ErrorCheck(vertex, RectanglePoint, RectangleLengths);
				}
				else OutputMat = EdgeMat;
				#endregion
			}
			catch(Exception ex)
			{
				//MainForm.ShowMessage("종료", "영상 필터 적용 중 예외가 발생하였습니다!!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
				OutputMat = InputMat;
			}
			string CallFunction = new StackFrame(1, true).GetMethod().Name;
			//Console.WriteLine(VidoeDisplayNo + "번째 영상 메서드 실행 시간 : " + sw.ElapsedMilliseconds.ToString() + "ms");
			if (CallFunction.Contains(">"))
			{
				string VidoeDisplayNo = CallFunction.Substring(CallFunction.LastIndexOf(">") - 1, 1);
				SubForm.listBox1.Items.Add("적용된 필터 : " + Filter + " / " + VidoeDisplayNo + "번째 영상 메서드 실행 시간 : " + sw.ElapsedMilliseconds.ToString() + "ms\n");
				SubForm.listBox1.SelectedIndex = SubForm.listBox1.Items.Count - 1;
			}
			return OutputMat;
		}
		#region 사각형의 변을 시계방향으로 정렬 - 참조 : https://www.crocus.co.kr/1634
		public static void SortPointsClockwise(List<OpenCvSharp.Point> points)
		{
			float averageX = 0;
			float averageY = 0;

			foreach (OpenCvSharp.Point point in points)
			{
				averageX += point.X;
				averageY += point.Y;
			}

			float finalAverageX = averageX / points.Count;
			float finalAverageY = averageY / points.Count;

			Comparison<OpenCvSharp.Point> comparison = (lhs, rhs) =>
			{
				double lhsAngle = Math.Atan2(lhs.Y - finalAverageY, lhs.X - finalAverageX);
				double rhsAngle = Math.Atan2(rhs.Y - finalAverageY, rhs.X - finalAverageX);

				// Depending on the coordinate system, you might need to reverse these two conditions
				if (lhsAngle < rhsAngle) return -1;
				if (lhsAngle > rhsAngle) return 1;

				return 0;
			};

			points.Sort(comparison);
		}
		#endregion
		public double DistanceToPoint(OpenCvSharp.Point a, OpenCvSharp.Point b)
		{
			return Math.Round((double)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)), 0);
		}
		public OpenCvSharp.Point LengthWritePoint(OpenCvSharp.Point a, OpenCvSharp.Point b)
		{
			int longX = a.X > b.X ? a.X : b.X;
			int shortX = a.X < b.X ? a.X : b.X;
			int longY = a.Y > b.Y ? a.Y : b.Y;
			int shortY = a.Y < b.Y ? a.Y : b.Y;
			int PointX = shortX + ((longX - shortX) / 2);
			int PointY = shortY + ((longY - shortY) / 2);
			return new OpenCvSharp.Point(PointX, PointY);
		}
		public double PixelToCentimeter(double pixel, int CamNo)
		{
			//return Math.Round(pixel * 2.54 / 96, 2);
			Camera_Setting CamSet = MainForm.Settings.CameraSetting;
			double[] CamPx = new double[] { CamSet.Cam1pxLength, CamSet.Cam2pxLength, CamSet.Cam3pxLength };
			return Math.Round(pixel * CamPx[CamNo - 1], 2);
		}
		private void ErrorCheck(List<OpenCvSharp.Point> points, List<OpenCvSharp.Point> rectangles, List<double> RectangleLengths)
		{
			if (rectangles[1].Y < rectangles[0].Y - 100 && rectangles[1].Y < rectangles[0].Y + 100)
			{
				// 1이 가장 밑
				for (int i = 1; i < rectangles.Count; i++)
				{
					Console.WriteLine("\n1번 사각형 기준 " + (i + 1) + "번 사각형");
					Console.WriteLine("오차 1면 : " + Math.Round((RectangleLengths[0] - RectangleLengths[i * 4 + 0]), 2) + "cm");
					Console.WriteLine("오차 2면 : " + Math.Round((RectangleLengths[1] - RectangleLengths[i * 4 + 1]), 2) + "cm");
					Console.WriteLine("오차 3면 : " + Math.Round((RectangleLengths[2] - RectangleLengths[i * 4 + 2]), 2) + "cm");
					Console.WriteLine("오차 4면 : " + Math.Round((RectangleLengths[3] - RectangleLengths[i * 4 + 3]), 2) + "cm");
				}
			}
			else
			{
				// 1, 2가 가장 밑
				//for (int i = 2; i < rectangles.Count; i+=2)
				//{
				//	Console.WriteLine("1번 사각형 기준" + (i + 1) + "번 사각형");
				//	Console.WriteLine("오차 1면 : " + Math.Round((RectangleLengths[0] - RectangleLengths[i * 4 + 0]), 2) + "cm");
				//	Console.WriteLine("오차 2면 : " + Math.Round((RectangleLengths[1] - RectangleLengths[i * 4 + 1]), 2) + "cm");
				//	Console.WriteLine("오차 3면 : " + Math.Round((RectangleLengths[2] - RectangleLengths[i * 4 + 2]), 2) + "cm");
				//	Console.WriteLine("오차 4면 : " + Math.Round((RectangleLengths[3] - RectangleLengths[i * 4 + 3]), 2) + "cm");
				//}
				//for (int i = 3; i < rectangles.Count; i += 2)
				//{
				//	Console.WriteLine("2번 사각형 기준" + (i + 1) + "번 사각형");
				//	Console.WriteLine("오차 1면 : " + Math.Round((RectangleLengths[4] - RectangleLengths[i * 4 + 0]), 2) + "cm");
				//	Console.WriteLine("오차 2면 : " + Math.Round((RectangleLengths[5] - RectangleLengths[i * 4 + 1]), 2) + "cm");
				//	Console.WriteLine("오차 3면 : " + Math.Round((RectangleLengths[6] - RectangleLengths[i * 4 + 2]), 2) + "cm");
				//	Console.WriteLine("오차 4면 : " + Math.Round((RectangleLengths[7] - RectangleLengths[i * 4 + 3]), 2) + "cm");
				//}
				for (int i = 2; i < rectangles.Count; i++)
				{
					if (rectangles[i].X > rectangles[0].X - 150 && rectangles[i].X < rectangles[0].X + 150)
					{
						Console.WriteLine("\n1번 사각형 기준 " + (i + 1) + "번 사각형");
						Console.WriteLine("오차 1면 : " + Math.Round((RectangleLengths[0] - RectangleLengths[i * 4 + 0]), 2) + "cm");
						Console.WriteLine("오차 2면 : " + Math.Round((RectangleLengths[1] - RectangleLengths[i * 4 + 1]), 2) + "cm");
						Console.WriteLine("오차 3면 : " + Math.Round((RectangleLengths[2] - RectangleLengths[i * 4 + 2]), 2) + "cm");
						Console.WriteLine("오차 4면 : " + Math.Round((RectangleLengths[3] - RectangleLengths[i * 4 + 3]), 2) + "cm");
					}
					else
					{
						Console.WriteLine("\n2번 사각형 기준 " + (i + 1) + "번 사각형");
						Console.WriteLine("오차 1면 : " + Math.Round((RectangleLengths[4] - RectangleLengths[i * 4 + 0]), 2) + "cm");
						Console.WriteLine("오차 2면 : " + Math.Round((RectangleLengths[5] - RectangleLengths[i * 4 + 1]), 2) + "cm");
						Console.WriteLine("오차 3면 : " + Math.Round((RectangleLengths[6] - RectangleLengths[i * 4 + 2]), 2) + "cm");
						Console.WriteLine("오차 4면 : " + Math.Round((RectangleLengths[7] - RectangleLengths[i * 4 + 3]), 2) + "cm");
					}
				}
			}
		}
		private void FilterClick(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				MainForm.LoadingAnimationStart();
				CheckEdit ClickedEdit = ((CheckEdit)sender);
				int VideoNo = int.Parse(ClickedEdit.Name.Substring(ClickedEdit.Name.Length - 3, 1)) - 1;
				int CheckNo = int.Parse(ClickedEdit.Name.Substring(ClickedEdit.Name.Length - 1, 1));
				if (ClickedEdit.Checked)
				{
					ClickedEdit.Checked = false;
					MainForm.LoadingAnimationEnd();
					return;
				}
				else
				{
					CheckEdit[] CheckEdits;
					switch (VideoNo)
					{
						case 0: CheckEdits = new CheckEdit[] { CheckEdit1_1, CheckEdit1_2, CheckEdit1_3, CheckEdit1_4, CheckEdit1_5 }; break;
						case 1: CheckEdits = new CheckEdit[] { CheckEdit2_1, CheckEdit2_2, CheckEdit2_3, CheckEdit2_4, CheckEdit2_5 }; break;
						default: MainForm.ShowMessage("오류", "필터 변경에 문제가 발생하였습니다!!", "주의"); return;
					}
					foreach (CheckEdit edit in CheckEdits)
					{
						edit.Checked = false;
						//if(!edit.Name.Equals(ClickedEdit.Name)) edit.Checked = false;
					}
					//CheckEdits[VideoNo].Checked = true;
					FilterCheck[VideoNo] = CheckNo;
					if (VideoAnalysis_Videos[VideoNo] != null && IsPaused[VideoNo])
					{
						PictureBox[] pbx = new PictureBox[] { VideoAnalysis_Video1, VideoAnalysis_Video2 };
						if (VideoAnalysis_Videos[VideoNo].PosFrames >= 2) VideoAnalysis_Videos[VideoNo].PosFrames -= 1;
						else VideoAnalysis_Videos[VideoNo].PosFrames = 1;
						Mat image = new Mat();
						VideoAnalysis_Videos[VideoNo].Read(image);
						pbx[VideoNo].Image = BitmapConverter.ToBitmap(FilterSet(image, VideoCamNo[VideoNo], FilterCheck[VideoNo]));
						if (VideoAnalysis_Videos[VideoNo].PosFrames <= 2) VideoAnalysis_Videos[VideoNo].PosFrames -= 1;
					}
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("종료", "영상 필터 변경 중 예외가 발생하였습니다!!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			MainForm.LoadingAnimationEnd();
		}
	}
}