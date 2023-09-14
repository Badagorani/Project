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
using static DevExpress.Skins.SolidColorHelper;
using System.Data.SQLite;
using System.Security.Cryptography;
using OpenCvSharp.XImgProc;
using DevExpress.Utils.Svg;

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

			tbar[0] = trackBarControl1;
			tbar[1] = trackBarControl2;
			tbar[2] = trackBarControl3;
			tbar[3] = trackBarControl4;
			tbar[4] = trackBarControl5;
			ted[0] = textEdit1;
			ted[1] = textEdit2;
			ted[2] = textEdit3;
			ted[3] = textEdit4;
			ted[4] = textEdit5;
			textEdit1.Text = trackBarControl1.Value.ToString();
			textEdit2.Text = trackBarControl2.Value.ToString();
			textEdit3.Text = trackBarControl3.Value.ToString();
			textEdit4.Text = trackBarControl4.Value.ToString();
			textEdit5.Text = trackBarControl5.Value.ToString();
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
								Testfilter2(image, VideoCamNo[0]);
								bmp = BitmapConverter.ToBitmap(FilterSet(image, VideoCamNo[0], FilterCheck[0]));
								//Testfilter(image, VideoCamNo[0], FilterCheck[0]);
								if (VideoAnalysis_Video1.Image != null)
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
				MainForm.ShowMessage("오류", "영상2 여는 중 예외가 발생하였습니다!\n영상은 'Cam「1 ~ 3」' 폴더 안에 있어야 합니다!\n" + ex.Message, "주의");
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
		public Mat FilterSet(Mat InputMat, int VideoCamNo, int Filter/* = 1*/)
		{
			Stopwatch sw = Stopwatch.StartNew();
			if (InputMat == null) return InputMat;
			Mat OutputMat = InputMat.Clone();

			//OutputMat = NewFilterTest(InputMat, VideoCamNo, Filter);
			//return OutputMat;

			try
			{
				Mat GrayMat = new Mat();
				Mat EdgeMat = new Mat();
				//Mat BlurImg = new Mat();
				Cv2.CvtColor(InputMat, GrayMat, ColorConversionCodes.BGR2GRAY);
				Mat BinaryMat = new Mat();
				//Cv2.Threshold(GrayMat, BinaryMat, 145, 255, ThresholdTypes.Binary);
				Cv2.Threshold(GrayMat, BinaryMat, tbar[0].Value, tbar[1].Value, ThresholdTypes.Binary);
				//Mat imsibinary = new Mat();
				//Cv2.Resize(BinaryMat, imsibinary, new OpenCvSharp.Size(0, 0), 0.3, 0.3);
				//Cv2.ImShow("binary", imsibinary);
				//Cv2.GaussianBlur(InputMat, BlurImg, new OpenCvSharp.Size(3, 3), 1, 0, BorderTypes.Default);
				switch (Filter)
				{
					//case 2: Cv2.Canny(BinaryMat, EdgeMat, 170, 230, 3, true);/*60,200*/	break;
					case 2: Cv2.Canny(BinaryMat, EdgeMat, tbar[2].Value, tbar[3].Value, 3, true);/*60,200*/	break;
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
					Mat kernelEllipse = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(1, 1));
					Cv2.Dilate(EdgeMat, dilate, kernelEllipse, new OpenCvSharp.Point(-1, -1), 1, BorderTypes.Reflect);
					//Mat imsidilate = new Mat();
					//Cv2.Resize(dilate, imsidilate, new OpenCvSharp.Size(0, 0), 0.3, 0.3);
					//Cv2.ImShow("dilate", imsidilate);

					// 윤곽선 찾기
					OutputMat = FindContours(EdgeMat, OutputMat, VideoCamNo);
					//OutputMat = FindContours(EdgeMat, OutputMat, VideoCamNo);
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
			//Cv2.CalibrateCamera()
			return OutputMat;
		}
		private Mat FindContours(Mat dilate, Mat OutputMat, int VideoCamNo)
		{
			OpenCvSharp.Point[][] contours;
			HierarchyIndex[] hierarchy;
			Cv2.FindContours(dilate, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
			//Cv2.DrawContours(OutputMat, contours, -1, Scalar.Red, 2);
			//return OutputMat;

			List<OpenCvSharp.Point> vertex = new List<OpenCvSharp.Point>();
			Console.WriteLine($"\n");
			//if((avrLen / contours.Length) < 1300)
			//{
			//	minLen = 1000;
			//	maxLen = 1850;
			//}
			//else
			//{
			//	minLen = 1800;
			//	maxLen = 3200;
			//}
			int minLen = 0, maxLen = 0;
			double avrLen = 0;
			for (int i = 0; i < contours.Length; i++)
			{
				double length = Cv2.ArcLength(contours[i], false);

				if (i == 0)
				{
					foreach (OpenCvSharp.Point[] p in contours)
					{
						avrLen += Cv2.ArcLength(p, false);
					}
					double AvgLen = avrLen / contours.Length;
					if (i == 0) Console.WriteLine("평균 길이 : " + AvgLen.ToString());
					minLen = (int)AvgLen - 150;
					maxLen = (int)AvgLen + 1000;
				}
				if (/*length > maxLen/*3200 || */length < minLen/*1800*/) continue;
				double imsicontent = tbar[4].Value / (double)10000;
				OpenCvSharp.Point[] pp = Cv2.ApproxPolyDP(contours[i], imsicontent * length, true);
				//OpenCvSharp.Point[] pp = Cv2.ApproxPolyDP(contours[i], 0.05 * length, true);
				//RotatedRect rrect = Cv2.MinAreaRect(pp);
				if (pp.Length == 4)
				{
					Console.WriteLine($"순서 : {i}, 길이 : {length}");
					//Console.WriteLine($"성공 길이 : {length}");
					List<OpenCvSharp.Point> imsipoint = new List<OpenCvSharp.Point>();
					for (int j = 0; j < pp.Length; j++) imsipoint.Add(pp[j]);
					SortPointsClockwise(imsipoint);
					for (int j = 0; j < imsipoint.Count; j++) vertex.Add(imsipoint[j]);
					if(PixelToCentimeter(DistanceToPoint(vertex[vertex.Count - 4], vertex[vertex.Count - 3]), VideoCamNo) > 2)
					{
						Cv2.DrawContours(OutputMat, contours, i, Scalar.Red, -1, LineTypes.AntiAlias, hierarchy);
					}
					else
					{
						int imsiendpoint = vertex.Count - 5;
						for (int j = vertex.Count - 1; j > imsiendpoint; j--) vertex.Remove(vertex[j]);
					}
					//Mat imsiout = new Mat();
					//Cv2.Resize(OutputMat, imsiout, new OpenCvSharp.Size(0, 0), 0.5, 0.5);
					//Cv2.ImShow($"contours : {i} imsiout", imsiout);
				}
			}
			List<OpenCvSharp.Point> RectanglePoint = new List<OpenCvSharp.Point>();
			List<double> RectangleLengths = new List<double>();
			List<double> RectangleLengthsPixel = new List<double>();
			List<OpenCvSharp.Point> writepoint = new List<OpenCvSharp.Point>();
			for (int i = 0; i < vertex.Count; i++)
			{
				double PointLength = 0;
				OpenCvSharp.Point imsiwritepoint = new OpenCvSharp.Point();
				switch (i % 4)
				{
					case 0:
						PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), VideoCamNo);
						if (PointLength < 2)
						{
							i += 3;
							continue;
						}
						RectangleLengthsPixel.Add(DistanceToPoint(vertex[i], vertex[i + 1]));
						imsiwritepoint = LengthWritePoint(vertex[i], vertex[i + 1]);
						imsiwritepoint.X -= 120;
						imsiwritepoint.Y += 50;
						break;
					case 1:
						PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), VideoCamNo);
						RectangleLengthsPixel.Add(DistanceToPoint(vertex[i], vertex[i + 1]));
						imsiwritepoint = LengthWritePoint(vertex[i], vertex[i + 1]);
						imsiwritepoint.X -= 105;
						imsiwritepoint.Y += 50;
						break;
					case 2:
						PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), VideoCamNo);
						RectangleLengthsPixel.Add(DistanceToPoint(vertex[i], vertex[i + 1]));
						imsiwritepoint = LengthWritePoint(vertex[i], vertex[i + 1]);
						imsiwritepoint.X -= 120;
						imsiwritepoint.Y -= 20;
						break;
					case 3:
						PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i - 3]), VideoCamNo);
						RectangleLengthsPixel.Add(DistanceToPoint(vertex[i], vertex[i - 3]));
						imsiwritepoint = LengthWritePoint(vertex[i], vertex[i - 3]);
						imsiwritepoint.X -= 105;
						imsiwritepoint.Y -= 10;
						RectanglePoint.Add(LengthWritePoint(vertex[i], vertex[i - 2]));
						if ((i / 4 + 1) < 10)
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
				writepoint.Add(imsiwritepoint);
				RectangleLengths.Add(PointLength);
				Console.WriteLine("좌표 : " + vertex[i].ToString());
				//Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{PointLength}cm", writepoint, HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
				//Console.WriteLine(i % 4 + 1 + $"면 : {PointLength}cm");
			}
			ErrorCheck(RectanglePoint, RectangleLengths);
			OpenCvSharp.Point MatCenterPoint = new OpenCvSharp.Point(OutputMat.Width / 2, OutputMat.Height / 2);
			double CenterDistance = 0, imsidistance = 1000000, bigimsiset = 0, Criteria = 0;
			int imsiset = 0, criticalset = 0;
			foreach (OpenCvSharp.Point point in RectanglePoint)
			{
				CenterDistance = DistanceToPoint(point, MatCenterPoint);
				// 중심에 가장 가까운 사각형 구함
				if (CenterDistance < imsidistance)
				{
					imsidistance = CenterDistance;
					criticalset = imsiset;
				}
				imsiset++;
			}
			for (int i = 0; i < 4; i++)
			{
				if (RectangleLengthsPixel.Count <= 0) return OutputMat;
				if (RectangleLengthsPixel[criticalset * 4 + i] > bigimsiset) bigimsiset = RectangleLengthsPixel[criticalset * 4 + i];
			}
			Criteria = Math.Round(20 / bigimsiset, 4);
			SubForm.listBox1.Items.Add("--------------구분선--------------");
			SubForm.listBox1.Items.Add("가장 가운데 사각형의 순서 : " + (criticalset + 1));
			SubForm.listBox1.Items.Add("기준 사각형 위 : " + (RectangleLengthsPixel[criticalset * 4 + 0]) + "px"/* + " -> " + RectangleLengths[criticalset * 4 + 0]*/);
			SubForm.listBox1.Items.Add("기준 사각형 오 : " + (RectangleLengthsPixel[criticalset * 4 + 1]) + "px"/* + " -> " + RectangleLengths[criticalset * 4 + 1]*/);
			SubForm.listBox1.Items.Add("기준 사각형 밑 : " + (RectangleLengthsPixel[criticalset * 4 + 2]) + "px"/* + " -> " + RectangleLengths[criticalset * 4 + 2]*/);
			SubForm.listBox1.Items.Add("기준 사각형 왼 : " + (RectangleLengthsPixel[criticalset * 4 + 3]) + "px"/* + " -> " + RectangleLengths[criticalset * 4 + 3]*/);
			SubForm.listBox1.Items.Add("기준 길이 : 20 ÷ " + bigimsiset + " = " + Criteria + "mm");
			SubForm.listBox1.Items.Add("");
			for (int i = 0; i < RectangleLengths.Count; i++)
			{
				switch (i % 4)
				{
					case 0:
						SubForm.listBox1.Items.Add("기준 길이와의 픽셀 차이 : " + (i / 4 + 1) + "번 사각형 " + "위 -> " + (RectangleLengthsPixel[i] + " = " + (RectangleLengthsPixel[i] - bigimsiset)) + "px");
						break;
					case 1:
						SubForm.listBox1.Items.Add("기준 길이와의 픽셀 차이 : " + (i / 4 + 1) + "번 사각형 " + "오 -> " + (RectangleLengthsPixel[i] + " = " + (RectangleLengthsPixel[i] - bigimsiset)) + "px");
						break;
					case 2:
						SubForm.listBox1.Items.Add("기준 길이와의 픽셀 차이 : " + (i / 4 + 1) + "번 사각형 " + "밑 -> " + (RectangleLengthsPixel[i] + " = " + (RectangleLengthsPixel[i] - bigimsiset)) + "px");
						break;
					case 3:
						SubForm.listBox1.Items.Add("기준 길이와의 픽셀 차이 : " + (i / 4 + 1) + "번 사각형 " + "왼 -> " + (RectangleLengthsPixel[i] + " = " + (RectangleLengthsPixel[i] - bigimsiset)) + "px");
						SubForm.listBox1.Items.Add("");
						break;
				}
				double viewmilimeter = Math.Round(Criteria * (RectangleLengthsPixel[i] + ((RectangleLengthsPixel[i] - bigimsiset) * (-1))), 1);
				//if ((RectangleLengthsPixel[i] - bigimsiset) != 0) RectangleLengthsPixel[i] += (RectangleLengthsPixel[i] - bigimsiset) * (-1);
				//Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{Math.Round(Criteria * RectangleLengthsPixel[i], 1)}mm", writepoint[i], HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
				//Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{viewmilimeter}mm", writepoint[i], HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
				//Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{Math.Round((20 / RectangleLengthsPixel[i]) * RectangleLengthsPixel[i], 1)}mm", writepoint[i], HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
				Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{RectangleLengthsPixel[i]}px", writepoint[i], HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
			}
			SubForm.listBox1.SelectedIndex = SubForm.listBox1.Items.Count - 1;
			return OutputMat;
			#region 챗GPT에서 내준 답변
			/*// 체커보드 패턴의 한 변의 실제 길이 (mm)
			double checkerboardSquareSize = 25.0;

			// 체커보드 이미지 경로
			string imagePath = "path/to/checkerboard_image.jpg";

			// 이미지 불러오기
			Mat image = Cv2.ImRead(imagePath, ImreadModes.Color);

			// 그레이스케일 변환
			Mat grayImage = new Mat();
			Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

			// 체커보드 패턴 검출
			bool found = Cv2.FindChessboardCorners(grayImage, new Size(8, 8), out var corners);

			if (found)
			{
				// 코너 좌표 정확도 향상
				Cv2.CornerSubPix(grayImage, corners, new Size(11, 11), new Size(-1, -1),
					new TermCriteria(CriteriaTypes.MaxIter | CriteriaTypes.Eps, 30, 0.1));

				// 체커보드 패턴 그리기 (디버깅용)
				Cv2.DrawChessboardCorners(image, new Size(8, 8), corners, found);

				// 체커보드 패턴의 첫 번째 사각형의 네 꼭지점 좌표 추출
				Point2f topLeft = corners[0];
				Point2f topRight = corners[7];
				Point2f bottomLeft = corners[56];
				Point2f bottomRight = corners[63];

				// 체커보드 패턴의 첫 번째 사각형의 실제 길이 계산
				double pixelDistance = Math.Sqrt(Math.Pow(topRight.X - topLeft.X, 2) + Math.Pow(topRight.Y - topLeft.Y, 2));
				double mmDistance = checkerboardSquareSize * pixelDistance;

				// 결과 출력
				Console.WriteLine("실제 길이: " + mmDistance + "mm");
			}

			// 이미지 출력 (디버깅용)
			using (new Window("Checkerboard", image))
			{
				Cv2.WaitKey();
			}*/
			#endregion
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
		#region 좌표간 거리
		/// <summary>
		/// 좌표간 거리
		/// </summary>
		/// <param name="a">시작 좌표</param>
		/// <param name="b">끝 좌표</param>
		/// <returns></returns>
		public double DistanceToPoint(OpenCvSharp.Point a, OpenCvSharp.Point b)
		{
			return Math.Round((double)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)), 0);
		}
		#endregion
		#region 두 좌표의 중간 구함
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
		#endregion
		#region 픽셀을 센티미터로 변환
		public double PixelToCentimeter(double pixel, int CamNo)
		{
			return Math.Round(pixel * 2.54 / 96, 2);
			//Camera_Setting CamSet = MainForm.Settings.CameraSetting;
			//double[] CamPx = new double[] { CamSet.Cam1pxLength, CamSet.Cam2pxLength, CamSet.Cam3pxLength };
			//return Math.Round(pixel * CamPx[CamNo - 1], 2);
		}
		#endregion
		private void ErrorCheck(List<OpenCvSharp.Point> rectangles, List<double> RectangleLengths)
		{
			//0
			if (rectangles.Count <= 1) return;
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
		private void Testfilter(Mat image, int VideoCamNo, int FilterCheck)
		{
			double checkerboardSquareSize = 20.0;
			// 이미지를 메모리에 로드합니다.
			//Mat image = Cv2.ImRead(imagePath, ImreadModes.Color);

			// 이미지를 그레이스케일로 변환합니다.
			Mat grayImage = new Mat();
			Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);
			//Mat imsigray = new Mat();
			//Cv2.Resize(grayImage, imsigray, new OpenCvSharp.Size(0, 0), 0.3, 0.3);
			//Cv2.ImShow("gray", imsigray);

			// 체커보드 패턴의 모서리를 감지합니다.
			bool found = Cv2.FindChessboardCorners(grayImage, new OpenCvSharp.Size(3, 5), out var corners);

			// 모서리가 성공적으로 감지되지 않으면 종료합니다.
			if (!found)
			{
				Console.WriteLine("체커보드 패턴의 모서리가 성공적으로 감지되지 않았습니다.");
				return;
			}

			// 모서리 좌표의 정확도를 향상시킵니다.
			Cv2.CornerSubPix(grayImage, corners, new OpenCvSharp.Size(11, 11), new OpenCvSharp.Size(-1, -1),
			new TermCriteria(CriteriaTypes.MaxIter | CriteriaTypes.Eps, 30, 0.1));

			// 이미지에 체커보드 패턴을 그립니다.
			Cv2.DrawChessboardCorners(image, new OpenCvSharp.Size(8, 8), corners, found);

			// 체커보드가 아닌 사각형의 모서리를 찾습니다.
			Point2f topLeft = corners[0];
			Point2f topRight = corners[7];
			Point2f bottomLeft = corners[56];
			Point2f bottomRight = corners[63];

			// 체커보드가 아닌 사각형의 모서리 사이의 거리를 계산합니다.
			double pixelDistance = Math.Sqrt(Math.Pow(topRight.X - topLeft.X, 2) + Math.Pow(topRight.Y - topLeft.Y, 2));

			// 체커보드가 아닌 사각형의 길이를 밀리미터로 계산합니다.
			double mmDistance = checkerboardSquareSize * pixelDistance;

			// 체커보드가 아닌 사각형의 길이를 출력합니다.
			Console.WriteLine("체커보드가 아닌 사각형의 길이는 " + mmDistance + "mm입니다.");

			// 이미지를 표시합니다.
			using (new Window("Checkerboard", image))
			{
				Cv2.WaitKey();
			}
		}
		private void Testfilter2(Mat InputMat, int VideoCamNo)
		{
			if (InputMat == null) return;

			// 아웃풋 매트를 프레임으로 초기화
			Mat OutputMat = InputMat.Clone();

			// 흑백 이미지로 변환
			Mat GrayMat = new Mat();
			Cv2.CvtColor(InputMat, GrayMat, ColorConversionCodes.BGR2GRAY);

			// 2진 이미지로 변환
			Mat BinaryMat = new Mat();
			Cv2.Threshold(GrayMat, BinaryMat, 145, 255, ThresholdTypes.Binary);

			// 캐니 엣지를 적용하여 선으로 구분
			Mat EdgeMat = new Mat();
			Cv2.Canny(BinaryMat, EdgeMat, 170, 230, 3, true);

			// 캐니 엣지로 구분한 선들을 잘 보이게 증폭
			Mat dilate = new Mat();
			Mat kernelEllipse = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(8, 8));
			Cv2.Dilate(EdgeMat, dilate, kernelEllipse, new OpenCvSharp.Point(-1, -1), 1, BorderTypes.Reflect);
			//OutputMat = FindContours(BinaryMat, OutputMat, VideoCamNo);

			OpenCvSharp.Point[][] contours;
			HierarchyIndex[] hierarchy;
			Cv2.FindContours(dilate, out contours, out hierarchy, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);
			List<OpenCvSharp.Point> vertex = new List<OpenCvSharp.Point>();

			for (int i = 0; i < contours.Length; i++)
			{
				double length = Cv2.ArcLength(contours[i], true); int minLen, maxLen;
				double avrLen = 0;
				foreach (OpenCvSharp.Point[] p in contours)
				{
					avrLen += Cv2.ArcLength(p, true);
				}
				double AvgLen = avrLen / contours.Length;
				if (i == 0) Console.WriteLine("평균 길이 : " + AvgLen.ToString());
				minLen = (int)AvgLen - 200;
				maxLen = (int)AvgLen + 1000;

				if (/*length > maxLen/*3200 || */length < minLen/*1800*/) continue;
				OpenCvSharp.Point[] pp = Cv2.ApproxPolyDP(contours[i], 0.01 * length, true);
				//RotatedRect rrect = Cv2.MinAreaRect(pp);
				//if (pp.Length == 4)
				//{
				//	Console.WriteLine($"순서 : {i}, 길이 : {length}");
				//	//Console.WriteLine($"성공 길이 : {length}");
				//	List<OpenCvSharp.Point> imsipoint = new List<OpenCvSharp.Point>();
				//	for (int j = 0; j < pp.Length; j++) imsipoint.Add(pp[j]);
				//	SortPointsClockwise(imsipoint);
				//	for (int j = 0; j < imsipoint.Count; j++) vertex.Add(imsipoint[j]);
				//	Cv2.DrawContours(OutputMat, contours, i, Scalar.Red, -1, LineTypes.AntiAlias, hierarchy);
				//}
				if (pp.Length == 4)
				{
					Console.WriteLine($"순서 : {i}, 길이 : {length}");
					//Console.WriteLine($"성공 길이 : {length}");
					List<OpenCvSharp.Point> imsipoint = new List<OpenCvSharp.Point>();
					for (int j = 0; j < pp.Length; j++) imsipoint.Add(pp[j]);
					SortPointsClockwise(imsipoint);
					for (int j = 0; j < imsipoint.Count; j++) vertex.Add(imsipoint[j]);
					if(PixelToCentimeter(DistanceToPoint(vertex[vertex.Count - 4], vertex[vertex.Count - 3]), VideoCamNo) > 2)
					{
						Cv2.DrawContours(OutputMat, contours, i, Scalar.Red, -1, LineTypes.AntiAlias, hierarchy);
					}
					else
					{
						int imsiendpoint = vertex.Count - 5;
						for (int j = vertex.Count - 1; j > imsiendpoint; j--) vertex.Remove(vertex[j]);
					}
					//Mat imsiout = new Mat();
					//Cv2.Resize(OutputMat, imsiout, new OpenCvSharp.Size(0, 0), 0.5, 0.5);
					//Cv2.ImShow($"contours : {i} imsiout", imsiout);
				}
			}
			List<OpenCvSharp.Point> RectanglePoint = new List<OpenCvSharp.Point>();
			List<double> RectangleLengths = new List<double>();
			List<double> RectangleLengthsPixel = new List<double>();
			List<OpenCvSharp.Point> writepoint = new List<OpenCvSharp.Point>();
			for (int i = 0; i < vertex.Count; i++)
			{
				double PointLength = 0;
				OpenCvSharp.Point imsiwritepoint = new OpenCvSharp.Point();
				switch (i % 4)
				{
					case 0:
						PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), VideoCamNo);
						if (PointLength < 2)
						{
							i += 3;
							continue;
						}
						RectangleLengthsPixel.Add(DistanceToPoint(vertex[i], vertex[i + 1]));
						imsiwritepoint = LengthWritePoint(vertex[i], vertex[i + 1]);
						imsiwritepoint.X -= 120;
						imsiwritepoint.Y += 50;
						break;
					case 1:
						PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), VideoCamNo);
						RectangleLengthsPixel.Add(DistanceToPoint(vertex[i], vertex[i + 1]));
						imsiwritepoint = LengthWritePoint(vertex[i], vertex[i + 1]);
						imsiwritepoint.X -= 105;
						imsiwritepoint.Y += 50;
						break;
					case 2:
						PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), VideoCamNo);
						RectangleLengthsPixel.Add(DistanceToPoint(vertex[i], vertex[i + 1]));
						imsiwritepoint = LengthWritePoint(vertex[i], vertex[i + 1]);
						imsiwritepoint.X -= 120;
						imsiwritepoint.Y -= 20;
						break;
					case 3:
						PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i - 3]), VideoCamNo);
						RectangleLengthsPixel.Add(DistanceToPoint(vertex[i], vertex[i - 3]));
						imsiwritepoint = LengthWritePoint(vertex[i], vertex[i - 3]);
						imsiwritepoint.X -= 105;
						imsiwritepoint.Y -= 10;
						RectanglePoint.Add(LengthWritePoint(vertex[i], vertex[i - 2]));
						if ((i / 4 + 1) < 10)
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
				writepoint.Add(imsiwritepoint);
				RectangleLengths.Add(PointLength);
				//Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{PointLength}cm", writepoint, HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
				//Console.WriteLine(i % 4 + 1 + $"면 : {PointLength}cm");
			}
			ErrorCheck(RectanglePoint, RectangleLengths);
			OpenCvSharp.Point MatCenterPoint = new OpenCvSharp.Point(OutputMat.Width / 2, OutputMat.Height / 2);
			double CenterDistance = 0, imsidistance = 1000000, bigimsiset = 0, Criteria = 0;
			int imsiset = 0, criticalset = 0;
			foreach (OpenCvSharp.Point point in RectanglePoint)
			{
				CenterDistance = DistanceToPoint(point, MatCenterPoint);
				// 중심에 가장 가까운 사각형 구함
				if (CenterDistance < imsidistance)
				{
					imsidistance = CenterDistance;
					criticalset = imsiset;
				}
				imsiset++;
			}
			//using (var connection = new SQLiteConnection("database.sqlite"))
			//{
			//	connection.Open();
			//	var cmd = new SQLiteCommand(connection);
			//	// 테이블 생성
			//	cmd.CommandText = @"
			//	CREATE TABLE Pixels
			//	(
			//	  XCoordinate INT,
			//	  YCoordinate INT,
			//	  PixelLength INT
			//	);
			//	";
			//	cmd.ExecuteNonQuery();

			//	// 데이터 삽입
			//	cmd.CommandText = @" INSERT INTO Pixels (XCoordinate, YCoordinate, PixelLength) VALUES (1, 2, 3);
			//	";
			//	cmd.ExecuteNonQuery();

			//	connection.Close();
			//}
			for (int i = 0; i < 4; i++)
			{
				//0
				if (RectangleLengthsPixel.Count <= 0) return;
				if (RectangleLengthsPixel[criticalset * 4 + i] > bigimsiset) bigimsiset = RectangleLengthsPixel[criticalset * 4 + i];
			}
			Criteria = Math.Round(20 / bigimsiset, 4);
			SubForm.listBox1.Items.Add("--------------구분선--------------");
			SubForm.listBox1.Items.Add("가장 가운데 사각형의 순서 : " + (criticalset + 1));
			SubForm.listBox1.Items.Add("기준 사각형 위 : " + (RectangleLengthsPixel[criticalset * 4 + 0]) + "px"/* + " -> " + RectangleLengths[criticalset * 4 + 0]*/);
			SubForm.listBox1.Items.Add("기준 사각형 오 : " + (RectangleLengthsPixel[criticalset * 4 + 1]) + "px"/* + " -> " + RectangleLengths[criticalset * 4 + 1]*/);
			SubForm.listBox1.Items.Add("기준 사각형 밑 : " + (RectangleLengthsPixel[criticalset * 4 + 2]) + "px"/* + " -> " + RectangleLengths[criticalset * 4 + 2]*/);
			SubForm.listBox1.Items.Add("기준 사각형 왼 : " + (RectangleLengthsPixel[criticalset * 4 + 3]) + "px"/* + " -> " + RectangleLengths[criticalset * 4 + 3]*/);
			SubForm.listBox1.Items.Add("기준 길이 : 20 ÷ " + bigimsiset + " = " + Criteria + "mm");
			SubForm.listBox1.Items.Add("");
			for (int i = 0; i < RectangleLengths.Count; i++)
			{
				switch (i % 4)
				{
					case 0:
						SubForm.listBox1.Items.Add("기준 길이와의 픽셀 차이 : " + (i / 4 + 1) + "번 사각형 " + "위 -> " + (RectangleLengthsPixel[i] + " = " + (RectangleLengthsPixel[i] - bigimsiset)) + "px");
						break;
					case 1:
						SubForm.listBox1.Items.Add("기준 길이와의 픽셀 차이 : " + (i / 4 + 1) + "번 사각형 " + "오 -> " + (RectangleLengthsPixel[i] + " = " + (RectangleLengthsPixel[i] - bigimsiset)) + "px");
						break;
					case 2:
						SubForm.listBox1.Items.Add("기준 길이와의 픽셀 차이 : " + (i / 4 + 1) + "번 사각형 " + "밑 -> " + (RectangleLengthsPixel[i] + " = " + (RectangleLengthsPixel[i] - bigimsiset)) + "px");
						break;
					case 3:
						SubForm.listBox1.Items.Add("기준 길이와의 픽셀 차이 : " + (i / 4 + 1) + "번 사각형 " + "왼 -> " + (RectangleLengthsPixel[i] + " = " + (RectangleLengthsPixel[i] - bigimsiset)) + "px");
						SubForm.listBox1.Items.Add("");
						break;
				}
				double viewmilimeter = Math.Round(Criteria * (RectangleLengthsPixel[i] + ((RectangleLengthsPixel[i] - bigimsiset) * (-1))), 1);
				//if ((RectangleLengthsPixel[i] - bigimsiset) != 0) RectangleLengthsPixel[i] += (RectangleLengthsPixel[i] - bigimsiset) * (-1);
				//Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{Math.Round(Criteria * RectangleLengthsPixel[i], 1)}mm", writepoint[i], HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
				//Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{viewmilimeter}mm", writepoint[i], HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
				//Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{Math.Round((20 / RectangleLengthsPixel[i]) * RectangleLengthsPixel[i], 1)}mm", writepoint[i], HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
				Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{RectangleLengthsPixel[i]}mm", writepoint[i], HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
			}

			//Mat imsimat = new Mat();
			//Cv2.Resize(OutputMat, imsimat, new OpenCvSharp.Size(0, 0), 0.5, 0.5);
			//Cv2.ImShow("testbinary", imsimat);
		}
		public TrackBarControl[] tbar = new TrackBarControl[5];
		public TextEdit[] ted = new TextEdit[5];
		private void FilterSettingChange(object sender, EventArgs e)
		{
			int changedint = int.Parse(((TrackBarControl)sender).Name.Substring(((TrackBarControl)sender).Name.Length - 1)) - 1;
			ted[changedint].Text = tbar[changedint].Value.ToString();
		}
		private void TextChanges(object sender, EventArgs e)
		{
			int changedint = int.Parse(((TextEdit)sender).Name.Substring(((TextEdit)sender).Name.Length - 1)) - 1;
			tbar[changedint].Value = int.Parse(ted[changedint].Text);
		}
		public Mat NewFilterTest(Mat InputMat, int VideoCamNo, int Filter/* = 1*/)
		{
			if (InputMat == null) return InputMat;
			Mat OutputMat = InputMat.Clone();
			Mat ChangedMat = new Mat();

			// 가우시안 블러로 노이즈 제거
			//Mat BlurImg = new Mat();
			Cv2.GaussianBlur(InputMat, ChangedMat, new OpenCvSharp.Size(5, 5), 1, 0, BorderTypes.Default);

			// 컬러를 흑백으로 바꿈
			Cv2.CvtColor(InputMat, ChangedMat, ColorConversionCodes.BGR2GRAY);
			//Mat imsigray = new Mat();
			//Cv2.Resize(ChangedMat, imsigray, new OpenCvSharp.Size(0, 0), 0.5, 0.5);
			//Cv2.ImShow("gray", imsigray);

			// 흑백 이미지를 2진 이미지로 변환
			//Cv2.Threshold(ChangedMat, ChangedMat, tbar[0].Value, tbar[1].Value, ThresholdTypes.Binary);
			//ChangedMat.ConvertTo(ChangedMat, MatType.CV_8UC1);
			//Mat BinaryMat = new Mat();
			//Cv2.Resize(ChangedMat, BinaryMat, new OpenCvSharp.Size(0, 0), 0.5, 0.5);
			//Cv2.ImShow("binary", BinaryMat);

			//Cv2.AdaptiveThreshold(ChangedMat, ChangedMat, tbar[0].Value, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 25, 5);
			//Mat AdaptiveBinaryMat = new Mat();
			//Cv2.Resize(ChangedMat, AdaptiveBinaryMat, new OpenCvSharp.Size(0, 0), 0.5, 0.5);
			//Cv2.ImShow("AdaptiveBinaryMat", AdaptiveBinaryMat);

			//55,255,60,100
			// 2진 이미지를 캐니 엣지 필터로 가장자리 검출
			Cv2.Canny(ChangedMat, ChangedMat, 60, 100, 3, true);
			Mat imsicanny = new Mat();
			Cv2.Resize(ChangedMat, imsicanny, new OpenCvSharp.Size(0, 0), 0.5, 0.5);
			Cv2.ImShow("canny111", imsicanny);

			// 검출된 가장자리 이미지 팽창
			Mat dilate = new Mat();
			Mat kernelEllipse = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(4, 4));
			Cv2.Dilate(ChangedMat, ChangedMat, kernelEllipse, new OpenCvSharp.Point(-1, -1), 1, BorderTypes.Reflect);
			//Mat imsidilate = new Mat();
			//Cv2.Resize(ChangedMat, imsidilate, new OpenCvSharp.Size(0, 0), 0.5, 0.5);
			//Cv2.ImShow("dilate", imsidilate);

			// 검출된 가장자리 이미지 침식
			//Mat erode = new Mat();
			//Cv2.Erode(ThreshMat, erode, kernelEllipse, new OpenCvSharp.Point(-1, -1), 1, BorderTypes.Reflect);
			//Mat imsierode = new Mat();
			//Cv2.Resize(erode, imsierode, new OpenCvSharp.Size(0, 0), 0.5, 0.5);
			//Cv2.ImShow("erode", imsierode);

			//Mat kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(1, 1));
			//Mat morph = new Mat();
			//Cv2.MorphologyEx(EdgeMat, morph, MorphTypes.Close, kernel);
			//Mat imsimorph = new Mat();
			//Cv2.Resize(morph, imsimorph, new OpenCvSharp.Size(0, 0), 0.5, 0.5);
			//Cv2.ImShow("morph", imsimorph);

			//FindContours(EdgeMat, OutputMat, 2);
			FindContours2(ChangedMat, OutputMat, 2);
			return OutputMat;
		}

		public List<OpenCvSharp.Point> vertex = new List<OpenCvSharp.Point>();
		public List<OpenCvSharp.Point> moments = new List<OpenCvSharp.Point>();
		public List<OpenCvSharp.Point> RectanglePoint = new List<OpenCvSharp.Point>();
		public List<int> SquareNumber = new List<int>();
		public List<double> RectangleLengths = new List<double>();
		public List<double> RectangleLengthsPixel = new List<double>();
		public List<OpenCvSharp.Point> writepoint = new List<OpenCvSharp.Point>();
		public List<OpenCvSharp.Point> SquareApex = new List<OpenCvSharp.Point>();
		private void FindContours2(Mat InputMat, Mat OutputMat, int CamNo)
		{
			OpenCvSharp.Point[][] contours;
			HierarchyIndex[] hierarchy;
			Cv2.FindContours(InputMat, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
			Console.WriteLine($"\n");
			int minLen = 0, maxLen = 0;
			double avrLen = 0;
			for (int i = 0; i < contours.Length; i++)
			{
				double length = Cv2.ArcLength(contours[i], true);
				if (Cv2.ContourArea(contours[i], true) <= 0) continue;

				if (i == 0)
				{
					foreach (OpenCvSharp.Point[] p in contours)
					{
						avrLen += Cv2.ArcLength(p, true);
					}
					double AvgLen = avrLen / contours.Length;
					if (i == 0) Console.WriteLine("평균 길이 : " + AvgLen.ToString());
					minLen = (int)AvgLen - 150;
					maxLen = (int)AvgLen + 1000;
				}
				if (/*length > maxLen/*3200 || */length < minLen/*1800*/) continue;
				double imsicontent = tbar[4].Value / (double)10000;
				OpenCvSharp.Point[] pp = Cv2.ApproxPolyDP(contours[i], imsicontent * length, true);
				//OpenCvSharp.Point[] pp = Cv2.ApproxPolyDP(contours[i], 0.05 * length, true);
				if (pp.Length == 4)
				{
					Console.WriteLine($"순서 : {i}, 길이 : {length}");
					//Console.WriteLine($"성공 길이 : {length}");
					List<OpenCvSharp.Point> imsipoint = new List<OpenCvSharp.Point>();
					for (int j = 0; j < pp.Length; j++) imsipoint.Add(pp[j]);
					SortPointsClockwise(imsipoint);
					for (int j = 0; j < imsipoint.Count; j++) vertex.Add(imsipoint[j]);
					//Cv2.DrawContours(OutputMat, contours, i, Scalar.Red, -1, LineTypes.AntiAlias, hierarchy);
					//if (PixelToCentimeter(DistanceToPoint(vertex[vertex.Count - 4], vertex[vertex.Count - 3]), CamNo) > 2)
					//{
					//	bool flag = false;
					//	for (int k = vertex.Count - 4; k < vertex.Count; k++)
					//	{
					//		for (int l = k + 1; l < vertex.Count; l++)
					//		{
					//			if (vertex[k] == vertex[l] ||
					//				vertex[k].X == vertex[l].X + 1 ||
					//				vertex[k].X == vertex[l].X - 1 ||
					//				vertex[k].Y == vertex[l].Y + 1 ||
					//				vertex[k].Y == vertex[l].Y - 1)
					//			{
					//				int imsiendpoint = vertex.Count - 5;
					//				for (int j = vertex.Count - 1; j > imsiendpoint; j--) vertex.Remove(vertex[j]);
					//				flag = true;
					//				break;
					//			}
					//		}
					//		if (flag) break;
					//	}
					//	if (!flag) Cv2.Rectangle(OutputMat, Cv2.BoundingRect(contours[i]), Scalar.Red, -1, LineTypes.AntiAlias);
					//	//if (!flag) Cv2.DrawContours(OutputMat, contours, i, Scalar.Red, -1, LineTypes.AntiAlias, hierarchy);
					//}
					//else
					//{
					//	int imsiendpoint = vertex.Count - 5;
					//	for (int j = vertex.Count - 1; j > imsiendpoint; j--) vertex.Remove(vertex[j]);
					//}
					bool flag = false;
					for (int k = vertex.Count - 4; k < vertex.Count; k++)
					{
						for (int l = k + 1; l < vertex.Count; l++)
						{
							if (vertex[k] == vertex[l] /*||
								vertex[k].X == vertex[l].X + 1 ||
								vertex[k].X == vertex[l].X - 1 ||
								vertex[k].Y == vertex[l].Y + 1 ||
								vertex[k].Y == vertex[l].Y - 1*/)
							{
								int imsiendpoint = vertex.Count - 5;
								for (int j = vertex.Count - 1; j > imsiendpoint; j--) vertex.Remove(vertex[j]);
								flag = true;
								break;
							}
						}
						if (flag) break;
					}
					Rect a = Cv2.BoundingRect(contours[i]);
					//if (!flag) Cv2.Rectangle(OutputMat, Cv2.BoundingRect(contours[i]), Scalar.Red, -1, LineTypes.AntiAlias);
					if (!flag)
					{
						Cv2.DrawContours(OutputMat, contours, i, Scalar.Red, -1, LineTypes.AntiAlias, hierarchy);
						Moments mmt = Cv2.Moments(contours[i]);
						double	cx = mmt.M10 / mmt.M00,
								cy = mmt.M01 / mmt.M00;
						moments.Add(new OpenCvSharp.Point(cx, cy));
					}
					//Mat imsiout = new Mat();
					//Cv2.Resize(OutputMat, imsiout, new OpenCvSharp.Size(0, 0), 0.5, 0.5);
					//Cv2.ImShow($"contours : {i} imsiout", imsiout);
				}
			}
			int smallcount = 0;
			for (int i = 0; i < vertex.Count; i++)
			{
				double PointLength = 0;
				OpenCvSharp.Point imsiwritepoint = new OpenCvSharp.Point();
				switch (i % 4)
				{
					case 0:
						PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), CamNo);
						if (PointLength < 2)
						{
							i += 3;
							smallcount++;
							continue;
						}
						vertex[i] = new OpenCvSharp.Point(vertex[i].X - 2, vertex[i].Y - 2);
						RectangleLengthsPixel.Add(DistanceToPoint(vertex[i], vertex[i + 1]));
						imsiwritepoint = LengthWritePoint(vertex[i], vertex[i + 1]);
						imsiwritepoint.X -= 120;
						imsiwritepoint.Y += 50;
						Console.WriteLine(i / 4 + 1 - smallcount + "번 사각형 위왼 좌표 : " + vertex[i].ToString());
						SquareApex.Add(vertex[i]);
						break;
					case 1:
						vertex[i] = new OpenCvSharp.Point(vertex[i].X + 2, vertex[i].Y - 2);
						PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), CamNo);
						RectangleLengthsPixel.Add(DistanceToPoint(vertex[i], vertex[i + 1]));
						imsiwritepoint = LengthWritePoint(vertex[i], vertex[i + 1]);
						imsiwritepoint.X -= 105;
						imsiwritepoint.Y += 50;
						Console.WriteLine(i / 4 + 1 - smallcount + "번 사각형 위오 좌표 : " + vertex[i].ToString());
						SquareApex.Add(vertex[i]);
						break;
					case 2:
						vertex[i] = new OpenCvSharp.Point(vertex[i].X + 2, vertex[i].Y + 2);
						PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i + 1]), CamNo);
						RectangleLengthsPixel.Add(DistanceToPoint(vertex[i], vertex[i + 1]));
						imsiwritepoint = LengthWritePoint(vertex[i], vertex[i + 1]);
						imsiwritepoint.X -= 120;
						imsiwritepoint.Y -= 20;
						Console.WriteLine(i / 4 + 1 - smallcount + "번 사각형 밑오 좌표 : " + vertex[i].ToString());
						SquareApex.Add(vertex[i]);
						break;
					case 3:
						vertex[i] = new OpenCvSharp.Point(vertex[i].X - 2, vertex[i].Y + 2);
						PointLength = PixelToCentimeter(DistanceToPoint(vertex[i], vertex[i - 3]), CamNo);
						RectangleLengthsPixel.Add(DistanceToPoint(vertex[i], vertex[i - 3]));
						imsiwritepoint = LengthWritePoint(vertex[i], vertex[i - 3]);
						imsiwritepoint.X -= 105;
						imsiwritepoint.Y -= 10;
						RectanglePoint.Add(LengthWritePoint(vertex[i], vertex[i - 2]));
						SquareNumber.Add(i / 4 + 1 - smallcount);
						if ((i / 4 + 1) < 10)
						{
							//Cv2.PutText(OutputMat, (i / 4 + 1 - smallcount).ToString(), (LengthWritePoint(vertex[i], vertex[i - 2]) - new OpenCvSharp.Point(50, -50)),
							//HersheyFonts.HersheyScriptSimplex, 5, Scalar.Blue, 5);
							Cv2.PutText(OutputMat, (i / 4 + 1 - smallcount).ToString(), moments[i / 4] - new OpenCvSharp.Point(50, -50),
								HersheyFonts.HersheyScriptSimplex, 5, Scalar.Blue, 5);
						}
						else
						{
							//Cv2.PutText(OutputMat, (i / 4 + 1 - smallcount).ToString(), (LengthWritePoint(vertex[i], vertex[i - 2]) - new OpenCvSharp.Point(100, -50)),
							//HersheyFonts.HersheyScriptSimplex, 5, Scalar.Blue, 5);
							Cv2.PutText(OutputMat, (i / 4 + 1 - smallcount).ToString(), moments[i / 4] - new OpenCvSharp.Point(100, -50),
								HersheyFonts.HersheyScriptSimplex, 5, Scalar.Blue, 5);
						}
						Console.WriteLine(i / 4 + 1 - smallcount + "번 사각형 밑왼 좌표 : " + vertex[i].ToString() + "\n");
						SquareApex.Add(vertex[i]);
						break;
				}
				//if (moments.Count > i) Cv2.PutText(OutputMat, i.ToString(), moments[i] - new OpenCvSharp.Point(50, -50),
				//					   HersheyFonts.HersheyScriptSimplex, 2, Scalar.Orange, 2);
				writepoint.Add(imsiwritepoint);
				RectangleLengths.Add(PointLength);
				//Console.WriteLine("좌표 : " + vertex[i].ToString());
				//Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{PointLength}cm", writepoint, HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
				//Console.WriteLine(i % 4 + 1 + $"면 : {PointLength}cm");
			}
			//ErrorCheck(RectanglePoint, RectangleLengths);
			OpenCvSharp.Point MatCenterPoint = new OpenCvSharp.Point(OutputMat.Width / 2, OutputMat.Height / 2);
			double CenterDistance = 0, imsidistance = 1000000, bigimsiset = 0, Criteria = 0;
			int imsiset = 0, criticalset = 0;
			foreach (OpenCvSharp.Point point in RectanglePoint)
			{
				CenterDistance = DistanceToPoint(point, MatCenterPoint);
				// 중심에 가장 가까운 사각형 구함
				if (CenterDistance < imsidistance)
				{
					imsidistance = CenterDistance;
					criticalset = imsiset;
				}
				imsiset++;
			}
			for (int i = 0; i < 4; i++)
			{
				if (RectangleLengthsPixel.Count <= 0) return;
				if (RectangleLengthsPixel[criticalset * 4 + i] > bigimsiset) bigimsiset = RectangleLengthsPixel[criticalset * 4 + i];
			}
			Criteria = Math.Round(20 / bigimsiset, 4);
			SubForm.listBox1.Items.Add("--------------구분선--------------");
			SubForm.listBox1.Items.Add("가장 가운데 사각형의 순서 : " + (criticalset + 1));
			SubForm.listBox1.Items.Add("기준 사각형 위 : " + (RectangleLengthsPixel[criticalset * 4 + 0]) + "px"/* + " -> " + RectangleLengths[criticalset * 4 + 0]*/);
			SubForm.listBox1.Items.Add("기준 사각형 오 : " + (RectangleLengthsPixel[criticalset * 4 + 1]) + "px"/* + " -> " + RectangleLengths[criticalset * 4 + 1]*/);
			SubForm.listBox1.Items.Add("기준 사각형 밑 : " + (RectangleLengthsPixel[criticalset * 4 + 2]) + "px"/* + " -> " + RectangleLengths[criticalset * 4 + 2]*/);
			SubForm.listBox1.Items.Add("기준 사각형 왼 : " + (RectangleLengthsPixel[criticalset * 4 + 3]) + "px"/* + " -> " + RectangleLengths[criticalset * 4 + 3]*/);
			SubForm.listBox1.Items.Add("기준 길이 : 20 ÷ " + bigimsiset + " = " + Criteria + "mm");
			SubForm.listBox1.Items.Add("");
			for (int i = 0; i < RectangleLengths.Count; i++)
			{
				switch (i % 4)
				{
					case 0:
						SubForm.listBox1.Items.Add("기준 길이와의 픽셀 차이 : " + (i / 4 + 1) + "번 사각형 " + "위 -> " + (RectangleLengthsPixel[i] + " = " + (RectangleLengthsPixel[i] - bigimsiset)) + "px");
						break;
					case 1:
						SubForm.listBox1.Items.Add("기준 길이와의 픽셀 차이 : " + (i / 4 + 1) + "번 사각형 " + "오 -> " + (RectangleLengthsPixel[i] + " = " + (RectangleLengthsPixel[i] - bigimsiset)) + "px");
						break;
					case 2:
						SubForm.listBox1.Items.Add("기준 길이와의 픽셀 차이 : " + (i / 4 + 1) + "번 사각형 " + "밑 -> " + (RectangleLengthsPixel[i] + " = " + (RectangleLengthsPixel[i] - bigimsiset)) + "px");
						break;
					case 3:
						SubForm.listBox1.Items.Add("기준 길이와의 픽셀 차이 : " + (i / 4 + 1) + "번 사각형 " + "왼 -> " + (RectangleLengthsPixel[i] + " = " + (RectangleLengthsPixel[i] - bigimsiset)) + "px");
						SubForm.listBox1.Items.Add("");
						break;
				}
				double viewmilimeter = Math.Round(Criteria * (RectangleLengthsPixel[i] + ((RectangleLengthsPixel[i] - bigimsiset) * (-1))), 1);
				//if ((RectangleLengthsPixel[i] - bigimsiset) != 0) RectangleLengthsPixel[i] += (RectangleLengthsPixel[i] - bigimsiset) * (-1);
				//Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{Math.Round(Criteria * RectangleLengthsPixel[i], 1)}mm", writepoint[i], HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
				//Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{viewmilimeter}mm", writepoint[i], HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
				//Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{Math.Round((20 / RectangleLengthsPixel[i]) * RectangleLengthsPixel[i], 1)}mm", writepoint[i], HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
				Cv2.PutText(OutputMat, /*i % 4 + 1 + */$"{RectangleLengthsPixel[i]}px", writepoint[i], HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 3);
				// 영역에 들어온 픽셀은 길이가 어떻게되는지 알아야함!
			}
			SubForm.listBox1.SelectedIndex = SubForm.listBox1.Items.Count - 1;
			//List<string> strings = new List<string>();
			//List<int> stringcount = new List<int>();
			//for (int i = 0; i < RectangleLengthsPixel.Count; i++)
			//{
			//	if (i == 0)
			//	{
			//		strings.Add(RectangleLengthsPixel[i].ToString());
			//		stringcount.Add(1);
			//	}
			//	else
			//	{
			//		foreach(string s in RectangleLengthsPixel)
			//		{

			//		}
			//	}
			//}
			//vertex.Clear();
			//moments.Clear();
			//RectanglePoint.Clear();
			//RectangleLengths.Clear();
			//SquareNumber.Clear();
			//RectangleLengthsPixel.Clear();
			//writepoint.Clear();
			//SquareApex.Clear();
		}
		public void SaveData()
		{
			//SquareApex
			// 구조체에 담는다면 꼭지점 좌표, 1px당 길이만 가능함
			// 영역의 정보는 담지 못한다
			// DB에 정보를 담아낸다
			string sqliteFile = Environment.CurrentDirectory + "\\" + @"Height100mm.db";
			//SQLiteConnection.CreateFile(sqliteFile);
			string connString = @"Data Source = " + sqliteFile + ";";
			SQLiteConnection conn = null;
			conn = new SQLiteConnection(connString);
			conn.Open();

			//string sql = "CREATE TABLE Coordinates (SquareNo int, CoordinatesX int, CoordinatesY int, RealLength real)";
			//SQLiteCommand command = new SQLiteCommand(sql, conn);
			//int result = command.ExecuteNonQuery();
			//sql = "CREATE INDEX CoordinatesIndex ON Coordinates(CoordinatesX, CoordinatesY)";
			//command = new SQLiteCommand(sql, conn);
			//result = command.ExecuteNonQuery();

			for (int i = 0; i < SquareNumber.Count; i++)
			{
				for (int j = 0; j < SquareApex.Count; j++)
				{
					string strSquareNo = SquareNumber[i].ToString();
					string strCoordinatesX = SquareApex[j].X.ToString();
					string strCoordinatesY = SquareApex[j].Y.ToString();
					string strRealLength = "";
					string sql1 = "INSERT INTO Coordinates (SquareNo, CoordinatesX, CoordinatesY, RealLength) " +
								  "VALUES ('" + strSquareNo + "', '" + strCoordinatesX + "', '" + strCoordinatesY + "', '" + strRealLength + "')";
					SQLiteCommand command1 = new SQLiteCommand(sql1, conn);
					int result1 = command1.ExecuteNonQuery();
				}
			}

			string sql2 = "SELECT RealLength FROM Coordinates WHERE CoordinatesX = 111 AND CoordinatesY = 222";
			SQLiteCommand cmd2 = new SQLiteCommand(sql2, conn);
			SQLiteDataReader rdr = cmd2.ExecuteReader();
			while (rdr.Read())
			{
				MessageBox.Show(rdr["RealLength"].ToString());
			}
			rdr.Close();
			conn.Close();
		}
	}
}