using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Accord.Video.FFMPEG;
using OMRON_Camera_Control;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.UI.WinForms.Dialogs.OutputFormats;
using VisioForge.Core.VideoEdit;
using System.Globalization;
using Microsoft.WindowsAPICodePack.Shell;

namespace FS_MCS500POE_CameraViewer
{
	public partial class Form1 : Form
	{
		private const int Camera_Count = 3;
		public delegate void PaintDelegate(PictureBox pb_cam, Bitmap bmp, Mutex mutex);
		Property property;

		private CameraControl[] m_Camera = null;
		private Mutex[] m_mutexImage = null;
		private Bitmap[] m_bitmap = null;
		private Thread[] m_thread = null;
		private bool[] m_isWork = null;
		VideoFileWriter videoFileWriter = null;
		List<AxTimelineAxLib.AxTimelineControl> TimelineList = new List<AxTimelineAxLib.AxTimelineControl>();
		List<Button> TimelineAddButtons = new List<Button>();
		List<Button> TimelinePlayButtons = new List<Button>();
		List<Button> TimelinePauseButtons = new List<Button>();
		List<Button> TimelineStopButtons = new List<Button>();
		List<Button> TimelineDeleteButtons = new List<Button>();
		#region 상태
		short NowSelectedCamNo = 1;
		short PastSelectedCamNo = 0;
		bool IsRecord = false;
		#endregion
		public Form1()
		{
			InitializeComponent();
			m_Camera = new CameraControl[Camera_Count];
			m_mutexImage = new Mutex[Camera_Count];
			m_bitmap = new Bitmap[Camera_Count];
			m_thread = new Thread[Camera_Count];
			m_isWork = new bool[Camera_Count];

			for (int i = 0; i < Camera_Count; i++)
			{
				m_Camera[i] = new CameraControl(this);
				m_mutexImage[i] = new Mutex();
			}
			CreateEngine();
			ListItemAdd();
			AddVideoEvent();
		}
		public string Camera1IP
		{
			get { return m_Camera[0].GevDeviceIPAddress.ToString(); }
			set { m_Camera[0].SetDeviceIPAddress = value; }
		}
		public string Camera2IP
		{
			get { return m_Camera[1].GevDeviceIPAddress.ToString(); }
			set { m_Camera[1].SetDeviceIPAddress = value; }
		}
		public string Camera3IP
		{
			get { return m_Camera[2].GevDeviceIPAddress.ToString(); }
			set { m_Camera[2].SetDeviceIPAddress = value; }
		}
		private void Form1_Shown(object sender, EventArgs e)
		{
			try
			{
				for (int index = 0; index < Camera_Count; index++)
				{
					m_Camera[index].CameraOpen();
					if (!m_Camera[index].IsOpened)
					{
						ShowMessage("오류", "카메라 연결에 실패하였습니다!\n프로그램을 종료합니다!", "경고");
						this.Close();
					}

					long width = 0, height = 0;
					width = m_Camera[index].Width;
					height = m_Camera[index].Height;
					CreateBitmap(index, (int)width, (int)height, PixelFormat.Format24bppRgb);

					string model = "", serial = "";
					model = m_Camera[index].DeviceModelName;
					serial = m_Camera[index].DeviceSerialNumber;
					object[] lParameters = new object[] { this };

					//Callback Registered
					m_Camera[index].SetEnableImageCallback(true);
					m_isWork[index] = true;

					switch (index)
					{
						case 0: m_thread[index] = new Thread(new ParameterizedThreadStart(DisplayThread_Cam1)); break;
						case 1: m_thread[index] = new Thread(new ParameterizedThreadStart(DisplayThread_Cam2)); break;
						case 2: m_thread[index] = new Thread(new ParameterizedThreadStart(DisplayThread_Cam3)); break;
							//case 3: m_thread[index] = new Thread(new ParameterizedThreadStart(DisplayThread_Cam4)); break;
					}
					m_thread[index].Start(lParameters);
					m_Camera[index].Start();
				}
				ListAdd();
				property = new Property(this);
			}
			catch (Exception ex)
			{
				ShowMessage("오류", "카메라 연결에 실패하였습니다!\n프로그램을 종료합니다!\n" + ex.Message, "경고");
				this.Close();
			}
		}
		private void ListAdd()
		{
			//TimelineList.Add(axTimelineControl1);
			//TimelineList.Add(axTimelineControl2);
			//TimelineList.Add(axTimelineControl3);
			//TimelineList.Add(axTimelineControl4);

			TimelineAddButtons.Add(btn_Add_Video1);
			TimelineAddButtons.Add(btn_Add_Video2);
			TimelineAddButtons.Add(btn_Add_Video3);
			TimelineAddButtons.Add(btn_Add_Video4);

			TimelinePlayButtons.Add(btn_Play1);
			TimelinePlayButtons.Add(btn_Play2);
			TimelinePlayButtons.Add(btn_Play3);
			TimelinePlayButtons.Add(btn_Play4);

			TimelinePauseButtons.Add(btn_Pause1);
			TimelinePauseButtons.Add(btn_Pause2);
			TimelinePauseButtons.Add(btn_Pause3);
			TimelinePauseButtons.Add(btn_Pause4);

			TimelineStopButtons.Add(btn_Stop1);
			TimelineStopButtons.Add(btn_Stop2);
			TimelineStopButtons.Add(btn_Stop3);
			TimelineStopButtons.Add(btn_Stop4);

			TimelineDeleteButtons.Add(btn_Delete1);
			TimelineDeleteButtons.Add(btn_Delete2);
			TimelineDeleteButtons.Add(btn_Delete3);
			TimelineDeleteButtons.Add(btn_Delete4);

			foreach (AxTimelineAxLib.AxTimelineControl timectrl in TimelineList)
			{
				for (int i = 0; i < 8; i++)
				{
					if (i == 1) timectrl.SetTrackVisible(i, 1);
					else		timectrl.SetTrackVisible(i, 0);
				}
				//timectrl.SetTrackVisible(0, 0);
				//timectrl.SetTrackVisible(1, 1);
				//timectrl.SetTrackVisible(2, 0);
				//timectrl.SetTrackVisible(3, 0);
				//timectrl.SetTrackVisible(4, 0);
				//timectrl.SetTrackVisible(5, 0);
				//timectrl.SetTrackVisible(6, 0);
				//timectrl.SetTrackVisible(7, 0);
			}
		}
		private void CreateBitmap(int index, int width, int height, System.Drawing.Imaging.PixelFormat format)
		{
			m_bitmap[index] = new Bitmap((int)width, (int)height, PixelFormat.Format24bppRgb);
		}
		#region 카메라 화면을 PictureBox에 보여줌
		#region 카메라 1
		private static void DisplayThread_Cam1(object aParameters)
		{
			object[] lParameters = (object[])aParameters;
			Form1 lThis = (Form1)lParameters[0];

			while (lThis.m_isWork[0])
			{
				try
				{
					Thread.Sleep(100);

					EventWaitHandle handle = lThis.m_Camera[0].HandleGrabDone;
					if (handle.WaitOne(1000) == true)
					{
						lThis.m_mutexImage[0].WaitOne();

						Rectangle rect = new Rectangle(0, 0, lThis.m_bitmap[0].Width, lThis.m_bitmap[0].Height);
						BitmapData bmpData = lThis.m_bitmap[0].LockBits(rect, ImageLockMode.ReadWrite, lThis.m_bitmap[0].PixelFormat);

						IntPtr ptrBmp = bmpData.Scan0;
						int bpp = 24;
						Marshal.Copy(lThis.m_Camera[0].ColorBuffer, 0, ptrBmp, lThis.m_bitmap[0].Width * lThis.m_bitmap[0].Height * bpp / 8);
						lThis.m_bitmap[0].UnlockBits(bmpData);

						lThis.BeginInvoke(new PaintDelegate(lThis.Draw_Cam), new object[3] { lThis.pbx_Cam1, lThis.m_bitmap[0], lThis.m_mutexImage[0] });
						lThis.m_Camera[0].OnResetEventGrabDone();

						lThis.m_mutexImage[0].ReleaseMutex();
					}
				}
				catch (Exception ex)
				{
					//lThis.ShowMessage("오류", "오류임", "경고");
				}
			}
		}
		#endregion
		#region 카메라 2
		private static void DisplayThread_Cam2(object aParameters)
		{
			object[] lParameters = (object[])aParameters;
			Form1 lThis = (Form1)lParameters[0];

			while (lThis.m_isWork[1])
			{
				Thread.Sleep(100);

				EventWaitHandle handle = lThis.m_Camera[1].HandleGrabDone;
				if (handle.WaitOne(1000) == true)
				{
					lThis.m_mutexImage[1].WaitOne();

					Rectangle rect = new Rectangle(0, 0, lThis.m_bitmap[1].Width, lThis.m_bitmap[1].Height);
					BitmapData bmpData = lThis.m_bitmap[1].LockBits(rect, ImageLockMode.ReadWrite, lThis.m_bitmap[1].PixelFormat);

					IntPtr ptrBmp = bmpData.Scan0;
					int bpp = 24;
					Marshal.Copy(lThis.m_Camera[1].ColorBuffer, 0, ptrBmp, lThis.m_bitmap[1].Width * lThis.m_bitmap[1].Height * bpp / 8);
					lThis.m_bitmap[1].UnlockBits(bmpData);

					lThis.BeginInvoke(new PaintDelegate(lThis.Draw_Cam), new object[3] { lThis.pbx_Cam2, lThis.m_bitmap[1], lThis.m_mutexImage[1] });

					lThis.m_Camera[1].OnResetEventGrabDone();

					lThis.m_mutexImage[1].ReleaseMutex();
				}
			}
		}
		#endregion
		#region 카메라 3
		private static void DisplayThread_Cam3(object aParameters)
		{
			object[] lParameters = (object[])aParameters;
			Form1 lThis = (Form1)lParameters[0];

			while (lThis.m_isWork[2])
			{
				Thread.Sleep(100);

				EventWaitHandle handle = lThis.m_Camera[2].HandleGrabDone;
				if (handle.WaitOne(1000) == true)
				{
					lThis.m_mutexImage[2].WaitOne();

					Rectangle rect = new Rectangle(0, 0, lThis.m_bitmap[2].Width, lThis.m_bitmap[2].Height);
					BitmapData bmpData = lThis.m_bitmap[2].LockBits(rect, ImageLockMode.ReadWrite, lThis.m_bitmap[2].PixelFormat);

					IntPtr ptrBmp = bmpData.Scan0;
					int bpp = 24;
					Marshal.Copy(lThis.m_Camera[2].ColorBuffer, 0, ptrBmp, lThis.m_bitmap[2].Width * lThis.m_bitmap[2].Height * bpp / 8);
					lThis.m_bitmap[2].UnlockBits(bmpData);

					lThis.BeginInvoke(new PaintDelegate(lThis.Draw_Cam), new object[3] { lThis.pbx_Cam3, lThis.m_bitmap[2], lThis.m_mutexImage[2] });

					lThis.m_Camera[2].OnResetEventGrabDone();

					lThis.m_mutexImage[2].ReleaseMutex();
				}
			}
		}
		#endregion
		#region 받은 카메라 데이터를 PictureBox Image로 넘김
		void Draw_Cam(PictureBox pb_cam, Bitmap bmp, Mutex mutex)
		{
			try
			{
				mutex.WaitOne();

				if (bmp != null)
				{
					#region Bitmap에 텍스트 입력
					using (Graphics g = Graphics.FromImage(bmp))
					{
						g.SmoothingMode = SmoothingMode.AntiAlias;
						g.InterpolationMode = InterpolationMode.HighQualityBicubic;
						g.PixelOffsetMode = PixelOffsetMode.HighQuality;
						GraphicsPath path = new GraphicsPath(FillMode.Alternate);
						using (FontFamily font = new FontFamily("맑은 고딕"))
						{
							using (StringFormat stringFormat = new StringFormat())
							{
								stringFormat.Alignment = StringAlignment.Center;
								stringFormat.LineAlignment = StringAlignment.Center;
								PointF point = new PointF(850, 100);
								string CamText = "";
								switch (pb_cam.Name)
								{
									case "pbx_Cam1":
										CamText = "CAM1";
										break;
									case "pbx_Cam2":
										CamText = "CAM2";
										break;
									case "pbx_Cam3":
										CamText = "CAM3";
										break;
									default:
										CamText = "";
										break;
								}
								CamText += DateTime.Now.ToString(" yyyy년 MM월 dd일 HH시 mm분 ss초");
								path.AddString(CamText, font, (int)FontStyle.Bold, 80, point, stringFormat);
								//g.DrawString("테스트텍스트", new Font("맑은 고딕", 80, FontStyle.Bold), Brushes.Black, point, StringFormat.GenericTypographic);
								//g.Flush();
							}
						}
						g.FillPath(Brushes.Black, path);
						g.DrawPath(new Pen(Color.White, 3), path);
					}
					#endregion

					Bitmap bmpimg = (Bitmap)bmp.Clone();
					pb_cam.Image = bmpimg;
					if (pb_cam.Name.Substring(("pbx_Cam".Length), 1).Equals(NowSelectedCamNo.ToString()))
					{
						if (NowSelectedCamNo > 0 && NowSelectedCamNo < 4)
						{
							pbx_MainView.Image = bmpimg;
							if (IsRecord) videoFileWriter.WriteVideoFrame(bmpimg);
						}
					}
				}
			}
			catch (System.Exception ex)
			{
				//System.Diagnostics.Trace.WriteLine(exc.Message, "System Exception");
				ShowMessage("카메라", "카메라의 화면을 가져올 수 없습니다!\n" + ex.Message, "경고");
			}
			finally
			{
				mutex.ReleaseMutex();
			}
		}
		#endregion
		#endregion

		#region 카메라 화면을 클릭했을 때
		public void CamActions(object sender, EventArgs e)
		{
			if (IsRecord) ShowMessage("녹화 중...", "녹화 중에는 카메라를 변경할 수 없습니다!!", "주의");
			if (IsTimeline) ShowMessage("영상 확인 중...", "녹화된 영상을 확인하는 중에는 카메라를 변경할 수 없습니다!!", "주의");
			//Popup.Pop(1, "녹화 중", "녹화 중에는 카메라를 변경할 수 없습니다!");
			else
			{
				CamStatusText.Text = "";
				switch (((PictureBox)sender).Name)
				{
					case "pbx_Cam1":
						NowSelectedCamNo = 1;
						break;
					case "pbx_Cam2":
						NowSelectedCamNo = 2;
						break;
					case "pbx_Cam3":
						NowSelectedCamNo = 3;
						break;
					default:
						break;
				}
				//Viewer_Thread.ViewSetting(NowSelectedCamNo, IsViewing);
				//stPictureBox_Main.Image = null;
			}
		}
		#endregion

		#region 버튼 클릭 이벤트
		public void ButtonActions(object sender, EventArgs e)
		{
			try
			{
				switch (((Button)sender).Text)
				{
					case "이미지 저장":
						ImageSaveAction();
						break;
					case "동영상 저장":
						btn_VideoSave.Text = "녹화 중지";
						if (!VideoSaveAction())
						{
							//ShowMessage("화면 오류", "화면 표시 오류!!", "경고");
							IsRecord = false;
							btn_VideoSave.Text = "동영상 저장";
						}
						break;
					case "녹화 중지":
						IsRecord = false;
						videoFileWriter.Close();
						videoFileWriter = null;
						btn_VideoSave.Text = "동영상 저장";
						break;
					case "동영상 확인":
						VideoCheck();
						break;
					case "카메라 정보":
						Camera_Info();
						break;
					case "설정":
						PropertyShow();
						break;
				}
			}
			catch(Exception ex)
			{
				ShowMessage("오류", "잘못된 버튼 이벤트 입니다!!\n" + ex.Message, "주의");
			}
		}
		#endregion

		#region 이미지 저장 구현
		public void ImageSaveAction()
		{
			try
			{
				if (NowSelectedCamNo == 0)
				{
					if (IsRecord) ShowMessage("저장 중...", "이미지를 저장할 수 없습니다!!", "주의");
					return;
				}
				string folderpath = @"C:\FS-MCS500POE_Image_Save\";
				DirectoryInfo di = new DirectoryInfo(folderpath);
				if (di.Exists == false) di.Create();
				string nowtime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
				string imagepath = folderpath + nowtime + ".jpg";
				pbx_MainView.Image.Save(imagepath, ImageFormat.Jpeg);
				ShowMessage("저장 중...", "저장되었습니다!\n" + imagepath, "알림");
			}
			catch (Exception ex)
			{
				ShowMessage("저장 중...", "이미지 저장을 실패했습니다!!\n" + ex.Message, "주의");
			}
		}
		#endregion

		#region 동영상 저장 구현
		public bool VideoSaveAction()
		{
			try
			{
				if (IsTimeline)
				{
					ShowMessage("영상 확인 중...", "녹화된 영상을 확인하는 중에는 녹화를 할 수 없습니다!!", "주의");
					return false;
				}
				videoFileWriter = new VideoFileWriter();
				string folderpath = @"C:\FS-MCS500POE_Video_Save\";
				DirectoryInfo di = new DirectoryInfo(folderpath);
				if (di.Exists == false) di.Create();
				string nowtime = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
				string videopath = folderpath + nowtime + " 녹화.mp4";
				videoFileWriter.Open(videopath, m_bitmap[NowSelectedCamNo - 1].Width, m_bitmap[NowSelectedCamNo - 1].Height, Accord.Math.Rational.FromDouble(7.75), VideoCodec.MPEG4, 100000);
				IsRecord = true;
			}
			catch (Exception ex)
			{
				ShowMessage("녹화 중...", "녹화에 문제가 생겼습니다!!\n" + ex.Message, "주의");
				return false;
			}
			return true;
		}
		#endregion

		#region 동영상 확인 폴더 열기
		public void VideoCheck()
		{
			string folderpath = @"C:\FS-MCS500POE_Video_Save";
			DirectoryInfo di = new DirectoryInfo(folderpath);
			if (di.Exists == false) di.Create();
			System.Diagnostics.ProcessStartInfo ps = new System.Diagnostics.ProcessStartInfo
			{
				FileName = folderpath,
				UseShellExecute = true
			};
			System.Diagnostics.Process.Start(ps);
			//System.Diagnostics.Process.Start(folderpath);
		}
		#endregion

		#region 카메라 정보를 스테이터스 스트립에 표시
		public void Camera_Info()
		{
			try
			{
				string strAnd = " / ";
				string name = "Camera Name : " + m_Camera[NowSelectedCamNo - 1].DeviceModelName;
				string serial = "Camera SerialNo : " + m_Camera[NowSelectedCamNo - 1].DeviceSerialNumber;
				string version = "Camera Version : " + m_Camera[NowSelectedCamNo - 1].DeviceVersion;
				string ipaddress = "IP Address : " + m_Camera[NowSelectedCamNo - 1].GevDeviceIPAddress.ToString();
				CamStatusText.Text = name + strAnd + serial + strAnd + version + strAnd + ipaddress;
			}
			catch (Exception ex)
			{
				ShowMessage("오류", "카메라의 상태를 확인해주세요!\n" + ex.Message, "주의");
				return;
			}
		}
		#endregion

		#region 설정 정보창 오픈
		private void PropertyLocationSettings()
		{
			if(property != null)
			{
				property.Height = this.Height;
				property.StartPosition = FormStartPosition.Manual;
				Point ParentPoint = this.Location;
				property.Location = new Point(ParentPoint.X + this.Width, ParentPoint.Y);
			}
		}
		private void PropertyShow()
		{
			PropertyLocationSettings();
			property.Show();
			property.BringToFront();
		}
		#endregion

		#region 팝업
		/// <summary>
		/// 메세지 박스를 표시한다
		/// </summary>
		/// <param name="Content">내용</param>
		/// <param name="Title">제목</param>
		/// <param name="UseIcon">아이콘 알림, 주의, 경고, 질문</param>
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
			if(property != null)
			{
				property.Hide();
				property.XmlEndSave(property.property_setting, property.property_setting.GetType());
			}
			this.Hide();
			//Viewer_Thread.ViewSetting(NowSelectedCamNo, IsViewing);
			//Camera_Setting.DestroyCamera();
			for (int index = 0; index < Camera_Count; index++)
			{
				if (m_thread[index] != null)
				{
					m_isWork[index] = false;
					m_thread[index].Join();
					m_thread[index] = null;
				}
				m_Camera[index].Stop();
				m_Camera[index].Close();
			}
		}
		#endregion

		#region 영상 재생 관련
		private VideoEditCore VideoEdit1;
		private VideoEditCore VideoEdit2;
		private VideoEditCore VideoEdit3;
		private VideoEditCore VideoEdit4;
		List<VideoEditCore> videos = new List<VideoEditCore>();
		List<Button> AddButtons = new List<Button>();
		List<Button> PlayButtons = new List<Button>();
		List<Button> PauseButtons = new List<Button>();
		List<Button> StopButtons = new List<Button>();
		List<Button> DeleteButtons = new List<Button>();
		List<PictureBox> Thumbnails = new List<PictureBox>();
		List<TrackBar> TrackBars = new List<TrackBar>();
		List<Label> Labels = new List<Label>();

		private MP4SettingsDialog mp4SettingsDialog;
		bool applyingPictureBoxImage = false;
		string[] Files = new string[4];
		bool IsTimeline = false;
		int TimelineNo = 0;
		#endregion

		private void CreateEngine()
		{
			VideoEdit1 = new VideoEditCore();
			VideoEdit2 = new VideoEditCore();
			VideoEdit3 = new VideoEditCore();
			VideoEdit4 = new VideoEditCore();
		}
		private void ListItemAdd()
		{
			videos.Add(VideoEdit1);
			videos.Add(VideoEdit2);
			videos.Add(VideoEdit3);
			videos.Add(VideoEdit4);

			AddButtons.Add(btn_Add_Video1);
			AddButtons.Add(btn_Add_Video2);
			AddButtons.Add(btn_Add_Video3);
			AddButtons.Add(btn_Add_Video4);

			PlayButtons.Add(btn_Play1);
			PlayButtons.Add(btn_Play2);
			PlayButtons.Add(btn_Play3);
			PlayButtons.Add(btn_Play4);

			PauseButtons.Add(btn_Pause1);
			PauseButtons.Add(btn_Pause2);
			PauseButtons.Add(btn_Pause3);
			PauseButtons.Add(btn_Pause4);

			StopButtons.Add(btn_Stop1);
			StopButtons.Add(btn_Stop2);
			StopButtons.Add(btn_Stop3);
			StopButtons.Add(btn_Stop4);

			DeleteButtons.Add(btn_Delete1);
			DeleteButtons.Add(btn_Delete2);
			DeleteButtons.Add(btn_Delete3);
			DeleteButtons.Add(btn_Delete4);

			Thumbnails.Add(Video_Thumbnail1);
			Thumbnails.Add(Video_Thumbnail2);
			Thumbnails.Add(Video_Thumbnail3);
			Thumbnails.Add(Video_Thumbnail4);

			TrackBars.Add(trackBar1);
			TrackBars.Add(trackBar2);
			TrackBars.Add(trackBar3);
			TrackBars.Add(trackBar4);

			Labels.Add(label1);
			Labels.Add(label2);
			Labels.Add(label3);
			Labels.Add(label4);
		}
		private void AddVideoEvent()
		{
			videos[0].OnStart += VideoEdit_OnStart1;
			videos[1].OnStart += VideoEdit_OnStart2;
			videos[2].OnStart += VideoEdit_OnStart3;
			videos[3].OnStart += VideoEdit_OnStart4;

			videos[0].OnProgress += VideoEdit_OnProgress1;
			videos[1].OnProgress += VideoEdit_OnProgress2;
			videos[2].OnProgress += VideoEdit_OnProgress3;
			videos[3].OnProgress += VideoEdit_OnProgress4;

			for (int i = 0; i < videos.Count; i++)
			{
				//videos[i].OnStart += VideoEdit_OnStart;
				//videos[i].OnProgress += VideoEdit_OnProgress;
				videos[i].OnStop += VideoEdit_OnStop;
				videos[i].OnVideoFrameBitmap += VideoEdit_OnVideoFrameBitmap;
			}
		}
		private void AddVideoProgressEvent()
		{
			switch (TimelineNo)
			{
				case 0: videos[TimelineNo].OnProgress += VideoEdit_OnProgress1;
					break;
				case 1: videos[TimelineNo].OnProgress += VideoEdit_OnProgress2;
					break;
				case 2: videos[TimelineNo].OnProgress += VideoEdit_OnProgress3;
					break;
				case 3: videos[TimelineNo].OnProgress += VideoEdit_OnProgress4;
					break;
			}
		}
		private void DeleteVideoProgressEvent()
		{
			switch (TimelineNo)
			{
				case 0: videos[TimelineNo].OnProgress -= VideoEdit_OnProgress1;
					break;
				case 1: videos[TimelineNo].OnProgress -= VideoEdit_OnProgress2;
					break;
				case 2: videos[TimelineNo].OnProgress -= VideoEdit_OnProgress3;
					break;
				case 3: videos[TimelineNo].OnProgress -= VideoEdit_OnProgress4;
					break;
			}
		}
		public void VideoEdit_OnStart1(object sender, EventArgs e)
		{
			Invoke((Action)(() =>
			{
				TrackBars[0].Maximum = (int)videos[0].Duration().TotalSeconds;
			}));
		}
		public void VideoEdit_OnStart2(object sender, EventArgs e)
		{
			Invoke((Action)(() =>
			{
				TrackBars[1].Maximum = (int)videos[1].Duration().TotalSeconds;
			}));
		}
		public void VideoEdit_OnStart3(object sender, EventArgs e)
		{
			Invoke((Action)(() =>
			{
				TrackBars[2].Maximum = (int)videos[2].Duration().TotalSeconds;
			}));
		}
		public void VideoEdit_OnStart4(object sender, EventArgs e)
		{
			Invoke((Action)(() =>
			{
				TrackBars[3].Maximum = (int)videos[3].Duration().TotalSeconds;
			}));
		}
		public void VideoEdit_OnStop(object sender, StopEventArgs e)
		{
			if (!e.Successful)
			{
				//MessageBox.Show("Completed successfully", "성공!", MessageBoxButtons.OK);
				//MessageBox.Show(e.Position.ToString(), "문제!", MessageBoxButtons.OK);
				ShowMessage("오류", "영상을 멈추는데 문제가 있습니다!", "주의");
				return;
			}
			Stop_Video_Timeline();
		}
		public void VideoEdit_OnProgress1(object sender, ProgressEventArgs e)
		{
			Invoke((Action)(() =>
			{
				double ValuePercent = e.Progress;
				double ValueCalc = (ValuePercent / 100) * TrackBars[0].Maximum;
				TrackBars[0].Value = int.Parse(((int)ValueCalc).ToString());
				Trace.WriteLine(e.Progress, videos[0].State().ToString());
			}));
		}
		public void VideoEdit_OnProgress2(object sender, ProgressEventArgs e)
		{
			Invoke((Action)(() =>
			{
				double ValuePercent = e.Progress;
				double ValueCalc = (ValuePercent / 100) * TrackBars[1].Maximum;
				TrackBars[1].Value = int.Parse(((int)ValueCalc).ToString());
				Trace.WriteLine(e.Progress, videos[1].State().ToString());
			}));
		}
		public void VideoEdit_OnProgress3(object sender, ProgressEventArgs e)
		{
			Invoke((Action)(() =>
			{
				double ValuePercent = e.Progress;
				double ValueCalc = (ValuePercent / 100) * TrackBars[2].Maximum;
				TrackBars[2].Value = int.Parse(((int)ValueCalc).ToString());
				Trace.WriteLine(e.Progress, videos[2].State().ToString());
			}));
		}
		public void VideoEdit_OnProgress4(object sender, ProgressEventArgs e)
		{
			Invoke((Action)(() =>
			{
				double ValuePercent = e.Progress;
				double ValueCalc = (ValuePercent / 100) * TrackBars[3].Maximum;
				TrackBars[3].Value = int.Parse(((int)ValueCalc).ToString());
				Trace.WriteLine(e.Progress, videos[3].State().ToString());
			}));
		}
		public void VideoEdit_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
		{
			if (applyingPictureBoxImage)
			{
				return;
			}

			applyingPictureBoxImage = true;

			var image = pbx_MainView.Image;
			pbx_MainView.Image = new Bitmap(e.Frame);

			image?.Dispose();

			applyingPictureBoxImage = false;
		}
		private void TimelineButtonActions(object sender, EventArgs e)
		{
			TimelineNo = int.Parse(((Button)sender).Name.Substring(((Button)sender).Name.Length - 1, 1)) - 1;
			switch (((Button)sender).Text)
			{

				case "+": Add_Video_Timeline();
					break;
				case "▶": Play_Video_Timeline();
					break;
				case "∥": Pause_Video_Timeline();
					break;
				case "■": Stop_Video_Timeline();
					break;
				case "X": Delete_Video_Timeline();
					break;
			}
		}
		private void Add_Video_Timeline()
		{
			using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true, Filter = "All Files (*.*)|*.*|WMV|*.wmv|WAV|*.wav|MP4|*.mp4|MKV|*.mkv|AVI|*.avi" })
			{
				TrackBars[TimelineNo].Enabled = true;
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					if (videos[TimelineNo].State() == PlaybackState.Play)
					{
						Delete_Video_Timeline();
						VideoSource videoFile = new VideoSource(ofd.FileName, null, null, VideoEditStretchMode.Letterbox, 0, 100 / 100.0);
						videos[TimelineNo].Input_AddVideoFileAsync(videoFile, TimeSpan.Zero, 0, 0, 0);
					}
					else
					{
						VideoSource videoFile = new VideoSource(ofd.FileName, null, null, VideoEditStretchMode.Letterbox, 0, 100 / 100.0);
						videos[TimelineNo].Input_AddVideoFileAsync(videoFile, TimeSpan.Zero, 0, 0, 0);
						Files[TimelineNo] = ofd.FileName;
						Output_FormatCreate();
					}
					ShellFile shellfile = ShellFile.FromFilePath(ofd.FileName);
					Thumbnails[TimelineNo].Image = shellfile.Thumbnail.Bitmap;
					Labels[TimelineNo].Text = "\"" + ofd.FileName + "\"";
					PlayButtons[TimelineNo].Enabled = true;
					DeleteButtons[TimelineNo].Enabled = true;
				}
			}
		}
		private void Output_FormatCreate()
		{
			var mp4Output = new MP4Output();
			SetMP4Output(ref mp4Output);
			videos[TimelineNo].Output_Format = mp4Output;
		}
		private void SetMP4Output(ref MP4Output mp4Output)
		{
			if (this.mp4SettingsDialog == null) this.mp4SettingsDialog = new MP4SettingsDialog();
			this.mp4SettingsDialog.SaveSettings(ref mp4Output);
		}
		private void Play_Video_Timeline()
		{
			PastSelectedCamNo = NowSelectedCamNo;
			NowSelectedCamNo = 4;
			if (!IsTimeline) pbx_MainView.Image = null;
			IsTimeline = true;
			foreach (VideoEditCore core in videos) core.PauseAsync();
			videos[TimelineNo].Mode = VideoEditMode.Preview;
			videos[TimelineNo].Video_FrameRate = new VideoFrameRate(Convert.ToDouble(0, CultureInfo.InvariantCulture));
			if (videos[TimelineNo].State() == PlaybackState.Play)	videos[TimelineNo].ResumeAsync();
			else													videos[TimelineNo].StartAsync();
			PauseButtons[TimelineNo].Enabled = true;
			StopButtons[TimelineNo].Enabled = true;
		}
		private void Pause_Video_Timeline()
		{
			videos[TimelineNo].PauseAsync();
			PauseButtons[TimelineNo].Enabled = false;
		}
		private void Stop_Video_Timeline()
		{
			Trace.WriteLine(videos[TimelineNo].State().ToString());
			videos[TimelineNo].StopAsync();
			videos[TimelineNo].Dispose();
			videos[TimelineNo] = new VideoEditCore();
			TrackBars[TimelineNo].Value = 0;
			pbx_MainView.Image = null;
			Output_FormatCreate();
			AddVideoEvent();
			Trace.WriteLine(videos[TimelineNo].State().ToString());
			IsTimeline = false;
			NowSelectedCamNo = PastSelectedCamNo;
			VideoSource videoFile = new VideoSource(Files[TimelineNo], null, null, VideoEditStretchMode.Letterbox, 0, 100 / 100.0);
			videos[TimelineNo].Input_AddVideoFileAsync(videoFile, TimeSpan.Zero, 0, 0, 0);
			PlayButtons[TimelineNo].Enabled = true;
			PauseButtons[TimelineNo].Enabled = false;
			StopButtons[TimelineNo].Enabled = false;
		}
		private void Delete_Video_Timeline()
		{
			Stop_Video_Timeline();
			PlayButtons[TimelineNo].Enabled = false;
			DeleteButtons[TimelineNo].Enabled = false;
			Labels[TimelineNo].Text = "";
			Thumbnails[TimelineNo].Image = null;
			TrackBars[TimelineNo].Value = 0;
			TrackBars[TimelineNo].Enabled = false;
		}
		private void TrackBar_Scroll(object sender, EventArgs e)
		{
			DeleteVideoProgressEvent();
			videos[TimelineNo].PauseAsync();
			toolTip1.SetToolTip(TrackBars[TimelineNo], TimeSpan.FromSeconds(TrackBars[TimelineNo].Value).ToString());
		}
		private void TrackBar_MouseUp(object sender, MouseEventArgs e)
		{
			AddVideoProgressEvent();
			videos[TimelineNo].Position_Set(TimeSpan.FromSeconds(TrackBars[TimelineNo].Value));
			videos[TimelineNo].ResumeAsync();
			toolTip1.RemoveAll();
		}
		private void Form1_LocationAndSize_Changed(object sender, EventArgs e)
		{
			PropertyLocationSettings();
		}
	}
}
