using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vision.Properties;

namespace vision
{
	public partial class VideoCheck_Page : XtraUserControl
	{
		Form1 MainForm;
		VideoCapture[] VideoCheck_Videos;
		PictureBox[] VideoCheck_Pictures;
		string[] FilesPath;
		int fps;
		bool IsPlaying = false;
		bool IsPaused = true;
		Thread Play;
		public VideoCheck_Page(Form1 form)
		{
			InitializeComponent();
			this.MainForm = form;
			PanelSettings();
		}
		private void VideoCheck_Page_Load(object sender, EventArgs e)
		{
			MainForm.Log.LogWrite($"{MethodBase.GetCurrentMethod().Name}");
		}
		#region 페널 세팅
		private void PanelSettings()
		{
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
		#region 영상 확인 페이지 버튼 클릭 이벤트
		private void VideoCheck_ButtonActions(object sender, EventArgs e)
		{
			string ButtonText = ((SimpleButton)sender).Text;
			try
			{
				switch (ButtonText)
				{
					case "파일 열기": VideoCheck_FileOpen();		break;
					case "재 생"	:
					case "일시정지"	: VideoCheck_Play(ButtonText);	break;
					case "정 지"	: VideoCheck_Stop();			break;
					case "파일 닫기": VideoCheck_FileClose();		break;
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "잘못된 버튼 이벤트 입니다!!\n" + ex.Message, "주의");
			}
		}
		#endregion
		#region 영상 확인 카메라 화면을 클릭했을 때
		public void VideoCheck_CamActions(object sender, EventArgs e)
		{
			switch (((PictureBox)sender).Name)
			{
				case "VideoCheck_Cam1":
					MainForm.RealTimeView.NowSelectedCamNo = 1;
					break;
				case "VideoCheck_Cam2":
					MainForm.RealTimeView.NowSelectedCamNo = 2;
					break;
				case "VideoCheck_Cam3":
					MainForm.RealTimeView.NowSelectedCamNo = 3;
					break;
				default:
					break;
			}
			if (IsPaused) VideoCheck_MainCam.Image = VideoCheck_Pictures[MainForm.RealTimeView.NowSelectedCamNo - 1].Image;
		}
		#endregion
		private void VideoCheck_FileOpen()
		{
			using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true })
			{
				ofd.Filter = "동영상 파일 (*.mp4;*.avi;*.mov;*.wmv;*.mkv)|*.mp4;*.avi;*.mov;*.wmv;*.mkv";
				ofd.InitialDirectory = @"C:\FS-MCS500POE_Video_Save";

				MainForm.LoadingAnimationStart();
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					if (IsPlaying) VideoCheck_FileClose();
					//C:\FS - MCS500POE_Video_Save\Cam3\2023년 02월 16일 09시 21분 23초 녹화.mp4
					string FileName = ofd.FileName.Substring((ofd.InitialDirectory + @"\Cam1\").Length);
					string FilePath1 = ofd.InitialDirectory + @"\Cam1\" + FileName;
					string FilePath2 = ofd.InitialDirectory + @"\Cam2\" + FileName;
					string FilePath3 = ofd.InitialDirectory + @"\Cam3\" + FileName;
					VideoCapture VideoCheck_Video1 = new VideoCapture(FilePath1);
					VideoCapture VideoCheck_Video2 = new VideoCapture(FilePath2);
					VideoCapture VideoCheck_Video3 = new VideoCapture(FilePath3);
					VideoCheck_Videos = new VideoCapture[] { VideoCheck_Video1, VideoCheck_Video2, VideoCheck_Video3 };
					VideoCheck_Pictures = new PictureBox[] { VideoCheck_Cam1, VideoCheck_Cam2, VideoCheck_Cam3 };
					FilesPath = new string[] { FilePath1, FilePath2, FilePath3 };

					for (int i = 0; i < VideoCheck_Pictures.Length; i++)
					{
						if (!VideoCheck_Videos[i].Open(FilesPath[i]))
						{
							MainForm.ShowMessage("오류", "잘못된 영상 경로 입니다!!", "주의");
							MainForm.LoadingAnimationEnd();
							return;
						}
						VideoCheck_Videos[i].PosFrames = 1;
						Mat imsimat = new Mat();
						VideoCheck_Videos[i].Read(imsimat);
						VideoCheck_Pictures[i].Image = BitmapConverter.ToBitmap(imsimat);
						VideoCheck_Videos[i].PosFrames = 0;
					}
					VideoCheck_MainCam.Image = VideoCheck_Pictures[MainForm.RealTimeView.NowSelectedCamNo - 1].Image;
					tbc_VideoCheckTrack.Properties.Maximum = VideoCheck_Video1.FrameCount;
					fps = (int)VideoCheck_Video1.Fps;
					tbc_VideoCheckTrack.Enabled = true;
					//lbc_VideoCheckFilePath.Text = ofd.InitialDirectory + @"\Cam1/Cam2/Cam3\" + FileName;
					lbc_VideoCheckFilePath.Text = "  " + FileName;
				}
				MainForm.LoadingAnimationEnd();
			}
		}
		private void VideoCheck_Play(string ButtonText)
		{
			try
			{
				if (lbc_VideoCheckFilePath.Text.Equals("  파일 위치"))
				{
					MainForm.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
					return;
				}
				if (Play == null)
				{
					//Play = new Thread[3];
					IsPlaying = true;
					Play = new Thread(() =>
					{
						if (VideoCheck_Videos[0] != null)
						{
							//int videofps = (int)Math.Ceiling(fps) / 1000;
							int videofps = 1000 / fps;
							Stopwatch st = new Stopwatch();
							using (Mat image = new Mat())
							{
								Bitmap bmp;
								while (IsPlaying)
								{
									long started = st.ElapsedMilliseconds;
									if (IsPaused) continue;
									if (VideoCheck_Videos[0].PosFrames == VideoCheck_Videos[0].FrameCount) break;
									if (VideoCheck_Videos[0].Read(image))
									{
										Invoke((Action)(() =>
										{
											bmp = BitmapConverter.ToBitmap(image);
											VideoCheck_Cam1.Image = bmp;
										}));
									}
									if (VideoCheck_Videos[1].Read(image))
									{
										Invoke((Action)(() =>
										{
											bmp = BitmapConverter.ToBitmap(image);
											VideoCheck_Cam2.Image = bmp;
										}));
									}
									if (VideoCheck_Videos[2].Read(image))
									{
										Invoke((Action)(() =>
										{
											bmp = BitmapConverter.ToBitmap(image);
											VideoCheck_Cam3.Image = bmp;
										}));
									}
									//if (NowSelectedCamNo == 1) VideoCheck_MainCam.Image = bmp;
									//VideoCheck_MainCam.Image = VideoCheck_Pictures[NowSelectedCamNo - 1].Image;
									Invoke((Action)(() =>
									{
										VideoCheck_MainCam.Image = VideoCheck_Pictures[MainForm.RealTimeView.NowSelectedCamNo - 1].Image;
										tbc_VideoCheckTrack.Value = VideoCheck_Videos[0].PosFrames;
									}));
									int elapsed = (int)(st.ElapsedMilliseconds - started);
									int delay = videofps - elapsed;
									//Cv2.WaitKey(videofps);
									if (delay > 0) Cv2.WaitKey(delay);
								}
								//VideoCheck_Videos[0].PosFrames = 0;
							}
						}
						else Play = null;
					});
					#region 스레드 하나로 통일 하면서 필요없게됨
					//Play[1] = new Thread(() =>
					//{
					//	if (VideoCheck_Videos[1] != null)
					//	{
					//		int videofps = 1000 / (int)Math.Ceiling(fps);
					//		using (Mat image = new Mat())
					//		{
					//			while (IsPlaying)
					//			{
					//				if (IsPaused) continue;
					//				if (!VideoCheck_Videos[1].Read(image)) break;
					//				Bitmap bmp = BitmapConverter.ToBitmap(image);
					//				VideoCheck_Cam2.Image = bmp;
					//				if (NowSelectedCamNo == 2) VideoCheck_MainCam.Image = bmp;

					//				//Invoke((Action)(() =>
					//				//{
					//				//	VideoCheck_Videos[1].PosFrames = tbc_VideoCheckTrack.Value;
					//				//}));
					//				Cv2.WaitKey(videofps);
					//			}
					//			//VideoCheck_Videos[1].PosFrames = 0;
					//		}
					//	}
					//	else Play[1] = null;
					//});
					//Play[2] = new Thread(() =>
					//{
					//	if (VideoCheck_Videos[2] != null)
					//	{
					//		int videofps = 1000 / (int)Math.Ceiling(fps);
					//		using (Mat image = new Mat())
					//		{
					//			while (IsPlaying)
					//			{
					//				if (IsPaused) continue;
					//				if (!VideoCheck_Videos[2].Read(image)) break;
					//				Bitmap bmp = BitmapConverter.ToBitmap(image);
					//				VideoCheck_Cam3.Image = bmp;
					//				if (NowSelectedCamNo == 3) VideoCheck_MainCam.Image = bmp;

					//				//Invoke((Action)(() =>
					//				//{
					//				//	VideoCheck_Videos[2].PosFrames = tbc_VideoCheckTrack.Value;
					//				//}));
					//				Cv2.WaitKey(videofps);
					//			}
					//			//VideoCheck_Videos[2].PosFrames = 0;
					//		}
					//	}
					//	else Play[0] = null;
					//});
					#endregion
					//Play[1].Start();
					//Play[2].Start();
					Play.Start();
				}
				if (ButtonText.Equals("재 생"))
				{
					IsPaused = false;
					btn_VideoCheckPlayPause.Text = "일시정지";
					btn_VideoCheckPlayPause.ImageOptions.SvgImage = Resources.pause;
				}
				else if ((ButtonText.Equals("일시정지")))
				{
					IsPaused = true;
					btn_VideoCheckPlayPause.Text = "재 생";
					btn_VideoCheckPlayPause.ImageOptions.SvgImage = Resources.next;
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "영상 재생에 문제가 발생하였습니다!!\n" + ex.Message, "주의");
				return;
			}
		}
		private void VideoCheck_Stop()
		{
			if (lbc_VideoCheckFilePath.Text.Equals("  파일 위치"))
			{
				MainForm.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
				return;
			}
			IsPaused = true;
			tbc_VideoCheckTrack.Value = 0;
			for (int i = 0; i < VideoCheck_Pictures.Length; i++)
			{
				VideoCheck_Videos[i].PosFrames = 1;
				Mat imsimat = new Mat();
				VideoCheck_Videos[i].Read(imsimat);
				VideoCheck_Pictures[i].Image = BitmapConverter.ToBitmap(imsimat);
				VideoCheck_Videos[i].PosFrames = 0;
			}
			btn_VideoCheckPlayPause.Text = "재 생";
			btn_VideoCheckPlayPause.ImageOptions.SvgImage = Resources.next;
			VideoCheck_MainCam.Image = VideoCheck_Pictures[MainForm.RealTimeView.NowSelectedCamNo - 1].Image;
		}
		private void VideoCheck_FileClose()
		{
			if (lbc_VideoCheckFilePath.Text.Equals("  파일 위치"))
			{
				MainForm.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
				return;
			}
			IsPaused = true;
			btn_VideoCheckPlayPause.Text = "재 생";
			btn_VideoCheckPlayPause.ImageOptions.SvgImage = Resources.next;
			IsPlaying = false;
			tbc_VideoCheckTrack.Value = 0;
			tbc_VideoCheckTrack.Enabled = false;
			for (int i = 0; i < VideoCheck_Pictures.Length; i++)
			{
				VideoCheck_Pictures[i].Image = null;
				VideoCheck_Videos[i].PosFrames = 0;
				VideoCheck_Videos[i].Release();
			}
			VideoCheck_MainCam.Image = null;
			lbc_VideoCheckFilePath.Text = "  파일 위치";
			Play = null;
		}
		private void tbc_VideoCheckTrack_Scroll(object sender, EventArgs e)
		{
			MainForm.LoadingAnimationStart();
			if (IsPaused)
			{
				for (int i = 0; i < VideoCheck_Videos.Length; i++)
				{
					Mat image = new Mat();
					VideoCheck_Videos[i].PosFrames = tbc_VideoCheckTrack.Value;
					VideoCheck_Videos[i].Read(image);
					Bitmap bmp = BitmapConverter.ToBitmap(image);
					VideoCheck_Pictures[i].Image = bmp;
				}
			}
			else
			{
				IsPaused = true;
				for (int i = 0; i < VideoCheck_Videos.Length; i++)
				{
					VideoCheck_Videos[i].PosFrames = tbc_VideoCheckTrack.Value;
				}
				IsPaused = false;
			}
			VideoCheck_MainCam.Image = VideoCheck_Pictures[MainForm.RealTimeView.NowSelectedCamNo - 1].Image;
			MainForm.LoadingAnimationEnd();
		}
		private void VideoCheck_TrackBar_ValueChanged(object sender, EventArgs e)
		{
			if (tbc_VideoCheckTrack.Value == VideoCheck_Videos[0].FrameCount)
			{
				for (int i = 0; i < VideoCheck_Videos.Length; i++)
				{
					VideoCheck_Videos[i].Open(FilesPath[i]);
				}
				IsPaused = true;
				IsPlaying = false;
				tbc_VideoCheckTrack.Value = 0;
				VideoCheck_Videos[0].PosFrames = 0;
				VideoCheck_Videos[1].PosFrames = 0;
				VideoCheck_Videos[2].PosFrames = 0;
				btn_VideoCheckPlayPause.Text = "재 생";
				btn_VideoCheckPlayPause.ImageOptions.SvgImage = Resources.next;
				Play = null;
			}
		}
		public void PageChange()
		{
			IsPaused = true;
			btn_VideoCheckPlayPause.Text = "재 생";
			btn_VideoCheckPlayPause.ImageOptions.SvgImage = Resources.next;
		}
		public void VideoClose()
		{
			try
			{
				IsPlaying = false;
				if (Play != null)
				{
					//Play.Interrupt();
					Play.Abort();
					Play = null;
				}
			}
			catch(Exception ex)
			{
				//MainForm.ShowMessage("종료", "영상 확인 종료 중에 오류가 발생하였습니다!!\n" + ex.Message, "주의");
			}
		}
	}
}
