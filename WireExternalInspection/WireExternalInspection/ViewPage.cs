using DevExpress.XtraEditors;
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
using OMRON_Camera_Control;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.IO;
using static DevExpress.Utils.Menu.DXMenuItemPainter;
using DevExpress.XtraBars;
using WireExternalInspection.Properties;
using OpenCvSharp.Flann;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using Sentech.GenApiDotNET;
using System.Diagnostics;
using OpenCvSharp.Dnn;
using DevExpress.Utils.Extensions;
using DevExpress.Drawing;
using DevExpress.XtraWaitForm;
//using System.Windows.Media;

namespace WireExternalInspection
{
	public partial class ViewPage : DevExpress.XtraEditors.XtraUserControl
	{
		MainForm mainform;
		LogRecordProcessing Log;
		public int Camera_Count = 3;
		private CameraControl[] m_Camera = null;
		public Mutex[] m_mutexImage = null;
		public Bitmap[] m_bitmap = null;
		public Thread[] m_thread = null;
		VideoWriter[] VideoFiles = null;
		PictureBox[] ViewCams;
		public delegate void PaintDelegate(PictureBox pb_cam, Bitmap bmp, Mutex mutex);
		#region 상태
		public bool[] m_isWork = null;
		public int NowSelectedCamNo = 1;
		public int PastSelectedCamNo = 0;
		public bool IsRecord = false;
		public bool IsSelecting = false;
		public object[] lParameters;
		#endregion
		public ViewPage(MainForm form)
		{
			InitializeComponent();
			mainform = form;
			Log = mainform.Log;

			if (Camera_Count > 0)
			{
				m_Camera = new CameraControl[Camera_Count];
				m_mutexImage = new Mutex[Camera_Count];
				m_bitmap = new Bitmap[Camera_Count];
				m_thread = new Thread[Camera_Count];
				m_isWork = new bool[Camera_Count];
			}

			for (int i = 0; i < Camera_Count; i++)
			{
				m_Camera[i] = new CameraControl(mainform);
				//if(!m_Camera[i].IsOpened) return;
				m_mutexImage[i] = new Mutex();
			}
		}
		#region 카메라 속성
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
		public long CameraWidth
		{
			get { return m_Camera[0].Width; }
			set
			{
				for (int i = 0; i < m_Camera.Length; i++)
				{
					m_Camera[i].Width = value;
				}
			}
		}
		public long CameraHeight
		{
			get { return m_Camera[0].Height; }
			set
			{
				for (int i = 0; i < m_Camera.Length; i++)
				{
					m_Camera[i].Height = value;
				}
			}
		}
		#endregion
		private void ViewPage_Load(object sender, EventArgs e)
		{
			try
			{
				mainform.LoadingAnimationStart();
				//VideoSetLoad();
				for (int index = 0; index < Camera_Count; index++)
				{
					m_Camera[index].CameraOpen();
					if (!m_Camera[index].IsOpened)
					{
						//mainform.ShowMessage("오류", "카메라 연결에 실패하였습니다!", "경고");
						//mainform.Close();
						continue;
					}

					long width = 0, height = 0;
					width = m_Camera[index].Width;
					height = m_Camera[index].Height;
					CreateBitmap(index, (int)width, (int)height, PixelFormat.Format24bppRgb);

					string model = "", serial = "";
					model = m_Camera[index].DeviceModelName;
					serial = m_Camera[index].DeviceSerialNumber;
					m_Camera[index].SetCameraIndex = index;
					lParameters = new object[] { this };

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
					ViewCams = new PictureBox[] { pb_ViewCam1, pb_ViewCam2, pb_ViewCam3 };
				}
			}
			catch (Exception ex)
			{
				mainform.ShowMessage("오류", "카메라 연결에 실패하였습니다!\n프로그램을 종료합니다!\n" + ex.Message, "경고");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			timer1.Start();
			mainform.LoadingAnimationEnd();
		}
		#region 카메라 화면을 PictureBox에 보여줌
		#region 카메라 1
		private static void DisplayThread_Cam1(object aParameters)
		{
			object[] lParameters = (object[])aParameters;
			ViewPage lThis = (ViewPage)lParameters[0];

			while (lThis.m_isWork[0])
			{
				try
				{
					if (lThis.IsVideoOpen || lThis.IsSelecting) continue;
					if (lThis.mainform.NowPageNo == 1 /*|| lThis.NowSelectedCamNo == 1*/)
					{
						Thread.Sleep(100);
						if (lThis.mainform.PastPageNo != 1)
						{
							Thread.Sleep(1500);
							lThis.mainform.PastPageNo = 1;
						}

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

							lThis.BeginInvoke(new PaintDelegate(lThis.Draw_Cam), new object[3] { lThis.pb_ViewCam1, lThis.m_bitmap[0], lThis.m_mutexImage[0] });
							lThis.m_Camera[0].OnResetEventGrabDone();

							lThis.m_mutexImage[0].ReleaseMutex();
						}
						//else lThis.m_isWork[0] = false;
						else
						{
							bool flag = false;
							for (int i = 0; i < 10; i++)
							{
								if (handle.WaitOne(1000))
								{
									flag = true;
									break;
								}
							}
							if (!flag) lThis.m_isWork[0] = false;
						}
					}
				}
				catch (Exception ex)
				{
					lThis.mainform.ShowMessage("오류", "Cam1 Thread 오류!!\n" + ex.Message, "경고");
				}
			}
		}
		#endregion
		#region 카메라 2
		private static void DisplayThread_Cam2(object aParameters)
		{
			object[] lParameters = (object[])aParameters;
			ViewPage lThis = (ViewPage)lParameters[0];

			while (lThis.m_isWork[1])
			{
				try
				{
					if (lThis.IsVideoOpen || lThis.IsSelecting) continue;
					if (lThis.mainform.NowPageNo == 1 /*|| lThis.NowSelectedCamNo == 2*/)
					{
						Thread.Sleep(100);
						if (lThis.mainform.PastPageNo != 1)
						{
							Thread.Sleep(1500);
							lThis.mainform.PastPageNo = 1;
						}

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

							lThis.BeginInvoke(new PaintDelegate(lThis.Draw_Cam), new object[3] { lThis.pb_ViewCam2, lThis.m_bitmap[1], lThis.m_mutexImage[1] });

							lThis.m_Camera[1].OnResetEventGrabDone();

							lThis.m_mutexImage[1].ReleaseMutex();
						}
						//else lThis.m_isWork[1] = false;
						else
						{
							bool flag = false;
							for (int i = 0; i < 10; i++)
							{
								if (handle.WaitOne(1000))
								{
									flag = true;
									break;
								}
							}
							if (!flag) lThis.m_isWork[1] = false;
						}
					}
				}
				catch (Exception ex)
				{
					lThis.mainform.ShowMessage("오류", "Cam2 Thread 오류!!\n" + ex.Message, "경고");
				}
			}
		}
		#endregion
		#region 카메라 3
		private static void DisplayThread_Cam3(object aParameters)
		{
			object[] lParameters = (object[])aParameters;
			ViewPage lThis = (ViewPage)lParameters[0];

			while (lThis.m_isWork[2])
			{
				try
				{
					if (lThis.IsVideoOpen || lThis.IsSelecting) continue;
					if (lThis.mainform.NowPageNo == 1 /*|| lThis.NowSelectedCamNo == 3*/)
					{
						Thread.Sleep(100);
						if (lThis.mainform.PastPageNo != 1)
						{
							Thread.Sleep(1500);
							lThis.mainform.PastPageNo = 1;
						}

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

							lThis.BeginInvoke(new PaintDelegate(lThis.Draw_Cam), new object[3] { lThis.pb_ViewCam3, lThis.m_bitmap[2], lThis.m_mutexImage[2] });

							lThis.m_Camera[2].OnResetEventGrabDone();

							lThis.m_mutexImage[2].ReleaseMutex();
						}
						//else lThis.m_isWork[2] = false;
						else
						{
							bool flag = false;
							for (int i = 0; i < 10; i++)
							{
								if (handle.WaitOne(1000))
								{
									flag = true;
									break;
								}
							}
							if (!flag) lThis.m_isWork[2] = false;
						}
					}
				}
				catch (Exception ex)
				{
					lThis.mainform.ShowMessage("오류", "Cam3 Thread 오류!!\n" + ex.Message, "경고");
				}
			}
		}
		#endregion
		#region 받은 카메라 데이터를 PictureBox Image로 넘김
		void Draw_Cam(PictureBox pb_cam, Bitmap bmp, Mutex mutex)
		{
			try
			{
				//if (mainform.navigationFrame1.SelectedPageIndex != 0) return;
				mutex.WaitOne();
				if (bmp != null)
				{
					#region Bitmap에 텍스트 입력
					if(mainform.NowPageNo == 1)
					{
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
									stringFormat.Alignment = StringAlignment.Near;
									stringFormat.LineAlignment = StringAlignment.Near;
									PointF point = new PointF();
									//PointF point = new PointF(850, 100);
									string CamText = "";
									switch (pb_cam.Name)
									{
										case "pb_ViewCam1": CamText = "CAM1"; break;
										case "pb_ViewCam2": CamText = "CAM2"; break;
										case "pb_ViewCam3": CamText = "CAM3"; break;
										default:			CamText = "";	  break;
									}
									switch (mainform.Xml.CameraSetting.TextMark)
									{
										case "날짜/시간": CamText += DateTime.Now.ToString(" yyyy년 MM월 dd일 HH시 mm분 ss초"); break;
										case "로봇거리": CamText += " 로봇거리" + DateTime.Now.ToString(" HH시 mm분 ss초"); break;
										case "사용자 선택": CamText += " " + mainform.Xml.CameraSetting.UserText + DateTime.Now.ToString(" HH시 mm분 ss초"); break;
									}
									//float x = g.MeasureString(CamText, new Font(font, 80, FontStyle.Bold)).Width;
									//float y = g.MeasureString(CamText, new Font(font, 80, FontStyle.Bold)).Height;
									//CamText += DateTime.Now.ToString(" yyyy년 MM월 dd일 HH시 mm분 ss초");
									path.AddString(CamText, font, (int)FontStyle.Bold, 80, point, stringFormat);
									//g.DrawString("테스트텍스트", new Font("맑은 고딕", 80, FontStyle.Bold), Brushes.Black, point, StringFormat.GenericTypographic);
									//g.Flush();
								}
							}
							g.FillPath(Brushes.Black, path);
							g.DrawPath(new Pen(Color.White, 3), path);
						}
					}
					#endregion

					Bitmap bmpimg = (Bitmap)bmp.Clone();
					if (mainform.NowPageNo == 1)
					{
					}
					// 영상이 넘어가야 한다면 위의 if안에 넣어야 한다!
					pb_cam.Image = bmpimg;
					pb_cam.Refresh();

					int CamNo = int.Parse(pb_cam.Name.Substring(("pb_ViewCam".Length), 1));
					if (CamNo == NowSelectedCamNo)
					{
						if (mainform.NowPageNo == 1)
						{
						}
						// 영상이 넘어가야 한다면 위의 if안에 넣어야 한다!그리고 아래의 else 주석 해제가 필요
						pb_SelectedView.Image = bmpimg;
						pb_SelectedView.Refresh();
						//else
						//{
						//	//mainform.analysispage.pb_OriginalImg.Image = bmpimg;
						//	Rectangle rectangle = new Rectangle(iRealStartPointX, iRealStartPointY, iRealEndPointX - iRealStartPointX, iRealEndPointY - iRealStartPointY);
						//	Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height);
						//	using (Graphics graphics = Graphics.FromImage(bitmap))
						//	{
						//		graphics.DrawImage(bmpimg, 0, 0, rectangle, GraphicsUnit.Pixel);
						//		mainform.analysispage.pb_OriginalImg.Image = bitmap;
						//		mainform.analysispage.pb_OriginalImg.Refresh();
						//	}
						//}
					}
					if (IsRecord)
					{
						//VideoFiles[int.Parse(pb_cam.Name.Substring(("RealTimeView_Cam".Length), 1)) - 1].WriteVideoFrame(bmpimg);
						//VideoFiles[CamNo - 1].Write(BitmapConverter.ToMat(bmpimg));
						VideoFiles[CamNo - 1].Write(BitmapConverter.ToMat(bmpimg));
					}
				}
				else
				{
					pb_SelectedView.Image = null;
					pb_cam.Image = null;
				}
			}
			catch (System.Exception ex)
			{
				//System.Diagnostics.Trace.WriteLine(exc.Message, "System Exception");
				mainform.ShowMessage("카메라", "카메라의 화면을 가져올 수 없습니다!\n" + ex.Message, "경고");
			}
			finally
			{
				mutex.ReleaseMutex();
				//if (mainform.navigationFrame1.SelectedPageIndex == 0) mutex.ReleaseMutex();
			}
		}
		#endregion
		#endregion
		private void CreateBitmap(int index, int width, int height, System.Drawing.Imaging.PixelFormat format)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			m_bitmap[index] = new Bitmap((int)width, (int)height, PixelFormat.Format24bppRgb);
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			if (m_isWork != null)
			{
				if (!m_isWork[0] && !m_isWork[1] && !m_isWork[2])
				{
					timer1.Stop();
					mainform.ShowMessage("오류", "모든 카메라가 동작하지 않습니다!\n프로그램을 종료합니다!", "경고");
					mainform.Close();
				}
			}
		}

		#region 뷰 페이지 버튼 클릭 이벤트
		private void ViewPage_ButtonActions(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			string ButtonText = ((SimpleButton)sender).Text;
			string ButtonName = ((SimpleButton)sender).Name;
			if (IsRecord && !ButtonName.Equals("btn_VideoSave"))
			{
				mainform.ShowMessage("녹화 중...", "녹화 중는 페이지를 변경할 수 없습니다!!", "주의");
				return;
			}
			try
			{
				switch (ButtonName)
				{
					case "btn_ImageSave": ImageSaveAction(); break;
					case "btn_VideoSave":
					//btn_VideoSave.ImageOptions.SvgImage = Resources.stop;
					if (!IsRecord)
					{
						if (!VideoSaveAction()) IsRecord = false;
					}
					else
					{
						IsRecord = false;
						Thread.Sleep(500);
						for (int i = 0; i < VideoFiles.Length; i++)
						{
							VideoFiles[i].Release();
							VideoFiles[i] = null;
						}
						mainform.ShowMessage("저장 중...", FolderPath(2) + "\n저장되었습니다!", "알림");
					}
					break;
					case "btn_SaveFolderOpen":
					try
					{
						string FolderPath = mainform.Xml.ProgramSetting.SaveFilePath;
						System.Diagnostics.Process.Start(FolderPath);
					}
					catch (Exception ex)
					{
						mainform.ShowMessage("오류", "작업폴더를 여는 중 예외가 발생하였습니다!\n" + ex, "주의");
						Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
					}
					break;
					case "btn_VideoOpen":  VideoOpen();  break;
					case "btn_VideoPlay":  VideoPlay();	 break;
					case "btn_VideoStop":  VideoStop();  break;
					case "btn_VideoClose": VideoClose(); break;
				}
			}
			catch (Exception ex)
			{
				mainform.ShowMessage("오류", "잘못된 버튼 이벤트 입니다!!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		#endregion
		#region 카메라 화면을 클릭했을 때
		public void ViewCamActions(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				switch (((PictureBox)sender).Name)
				{
					case "pb_ViewCam1": NowSelectedCamNo = 1; break;
					case "pb_ViewCam2": NowSelectedCamNo = 2; break;
					case "pb_ViewCam3": NowSelectedCamNo = 3; break;
					default:								  break;
				}
				pb_SelectedView.Image = null;
				if (IsPaused) pb_SelectedView.Image = ViewCams[NowSelectedCamNo - 1].Image;
			}
			catch (Exception ex)
			{
				mainform.ShowMessage("오류", "메인 카메라 화면을 전환 중 예외가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		#endregion
		#region 이미지 저장 구현
		public void ImageSaveAction()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				if (NowSelectedCamNo == 0)
				{
					if (IsRecord) mainform.ShowMessage("저장 중...", "이미지를 저장할 수 없습니다!!", "주의");
					return;
				}
				PictureBox[] CamImage = new PictureBox[] { pb_ViewCam1, pb_ViewCam2, pb_ViewCam3 };
				ImageFormat Imageformat;
				string ImageFormatText = mainform.Xml.ProgramSetting.ImageFileFormat;
				switch (ImageFormatText)
				{
					case "JPG": Imageformat = ImageFormat.Jpeg; break;
					case "PNG": Imageformat = ImageFormat.Png; break;
					case "BMP": Imageformat = ImageFormat.Bmp; break;
					default: Imageformat = ImageFormat.Jpeg; break;
				}
				for (int i = 1; i < 4; i++)
				{
					if (CamImage[i - 1].Image == null) continue;
					string folderpath = FolderPath(1, i);
					string nowtime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
					string imagepath = folderpath + nowtime + "." + ImageFormatText.ToLower();
					CamImage[i - 1].Image.Save(imagepath, Imageformat);
				}
				mainform.ShowMessage("저장 중...", FolderPath(1) + "\n저장되었습니다!", "알림");
			}
			catch (Exception ex)
			{
				mainform.ShowMessage("저장 중...", "이미지 저장을 실패했습니다!!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		#endregion
		#region 동영상 저장 구현
		public bool VideoSaveAction()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				VideoWriter VideoFile1 = new VideoWriter();
				VideoWriter VideoFile2 = new VideoWriter();
				VideoWriter VideoFile3 = new VideoWriter();
				VideoFiles = new VideoWriter[] { VideoFile1, VideoFile2, VideoFile3 };
				string nowtime = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
				for (int i = 0; i < Camera_Count; i++)
				{
					if (!m_Camera[i].IsOpened) continue;
					string folderpath = FolderPath(2, i + 1);
					string videopath = folderpath + nowtime + " 녹화.mp4";
					//VideoFiles[i - 1].Open(videopath, m_bitmap[NowSelectedCamNo - 1].Width, m_bitmap[NowSelectedCamNo - 1].Height, 7/*Accord.Math.Rational.FromDouble(7.75)*/, VideoCodec.MPEG4, 100000);
					VideoFiles[i].Open(videopath, FourCC.MPG4, 7, new OpenCvSharp.Size(m_bitmap[NowSelectedCamNo - 1].Width, m_bitmap[NowSelectedCamNo - 1].Height), true);
				}
				IsRecord = true;
			}
			catch (Exception ex)
			{
				mainform.ShowMessage("녹화 중...", "동영상 저장을 실패했습니다!!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
				return false;
			}
			return true;
		}
		#endregion
		#region 저장할 폴더 반환
		/// <summary>
		/// 저장할 폴더를 반환한다
		/// </summary>
		/// <param name="ImageVideo">1: 이미지 폴더, 2 : 동영상 폴더</param>
		/// <param name="CamNo">1 : 1번 카메라, 2 : 2번 카메라, 3 : 3번 카메라</param>
		/// <returns></returns>
		public string FolderPath(int ImageVideo, int CamNo = 0)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				string path;
				if (ImageVideo == 1)
				{
					path = mainform.Xml.ProgramSetting.ImageFilePath;
					switch (CamNo)
					{
						case 1: path += @"\Cam1\"; break;
						case 2: path += @"\Cam2\"; break;
						case 3: path += @"\Cam3\"; break;
					}
				}
				else if (ImageVideo == 2)
				{
					path = mainform.Xml.ProgramSetting.VideoFilePath;
					switch (CamNo)
					{
						case 1: path += @"\Cam1\"; break;
						case 2: path += @"\Cam2\"; break;
						case 3: path += @"\Cam3\"; break;
					}
				}
				else
				{
					mainform.ShowMessage("오류", "폴더를 확인할 수 없습니다!!", "주의");
					return "";
				}
				DirectoryInfo di = new DirectoryInfo(path);
				if (di.Exists == false) di.Create();
				return path;
			}
			catch (Exception ex)
			{
				mainform.ShowMessage("오류", "폴더 확인에 실패했습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
				return "";
			}
		}
		#endregion
		VideoCapture[] Videos;
		Thread Play;
		VideoCapture FirstExistenceVideo;
		string[] FilesPath;
		int fps;
		bool IsVideoOpen = false;
		bool IsPlaying = false;		// 영상이 재생 중인지 체크
		bool IsPaused = true;       // 영상이 일시정지 중인지 체크
		private void VideoOpen()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true })
				{
					ofd.Filter = "동영상 파일 (*.mp4;*.avi;*.mov;*.wmv;*.mkv)|*.mp4;*.avi;*.mov;*.wmv;*.mkv";
					ofd.InitialDirectory = mainform.Xml.ProgramSetting.VideoFilePath;
					IsVideoOpen = true;
					mainform.LoadingAnimationStart();
					if (ofd.ShowDialog() == DialogResult.OK)
					{
						IsPlaying = false;
						IsPaused = true;
						Play = null;
						btn_VideoPlay.ImageOptions.SvgImage = Resources.next;
						//if (IsPlaying) VideoClose();
						//IsVideoOpen = true;
						//C:\WireExternalInspectionSaveData\Video_Save\Cam3\2023년 12월 31일 23시 59분 59초 녹화.mp4
						string FileName = ofd.FileName.Substring((ofd.InitialDirectory + @"\Cam1\").Length);
						string FilePath1 = ofd.InitialDirectory + @"\Cam1\" + FileName;
						string FilePath2 = ofd.InitialDirectory + @"\Cam2\" + FileName;
						string FilePath3 = ofd.InitialDirectory + @"\Cam3\" + FileName;
						VideoCapture Video1 = new VideoCapture(FilePath1);
						VideoCapture Video2 = new VideoCapture(FilePath2);
						VideoCapture Video3 = new VideoCapture(FilePath3);
						Videos = new VideoCapture[] { Video1, Video2, Video3 };
						FilesPath = new string[] { FilePath1, FilePath2, FilePath3 };

						for (int i = 0; i < Videos.Length; i++)
						{
							if (!Videos[i].Open(FilesPath[i]))
							{
								mainform.ShowMessage("오류", "영상 " + FilesPath[i] + " 을 찾을 수 없습니다!", "주의");
								Videos[i] = null;
								continue;
							}
						}
						ThumbnailCreate();
						pb_SelectedView.Image = ViewCams[NowSelectedCamNo - 1].Image;
						for (int i = 0; i < Videos.Length; i++)
						{
							if (Videos[i] != null)
							{
								FirstExistenceVideo = Videos[i];
								tb_VideoTrack.Properties.Maximum = FirstExistenceVideo.FrameCount;
								fps = (int)FirstExistenceVideo.Fps;
								break;
							}
						}
						tb_VideoTrack.Enabled = true;
						btn_VideoPlay.Enabled = true;
						btn_VideoStop.Enabled = true;
						btn_VideoClose.Enabled = true;
						btn_VideoPlay.ImageOptions.SvgImage = Resources.next;
					}
					else IsVideoOpen = false;
				}
			}
			catch (Exception ex)
			{
				mainform.ShowMessage("오류", "영상을 불러오는 중 예외가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
				IsVideoOpen = false;
			}
			mainform.LoadingAnimationEnd();
		}
		private void ThumbnailCreate()
		{
			for (int i = 0; i < Videos.Length; i++)
			{
				if(Videos[i] != null)
				{
					Videos[i].PosFrames = 1;
					Mat thumbnail = new Mat();
					Videos[i].Read(thumbnail);
					ViewCams[i].Image = null;
					ViewCams[i].Image = BitmapConverter.ToBitmap(thumbnail);
					Videos[i].PosFrames = 0;
				}
			}
		}
		private void VideoPlayProcessing(Mat image, PictureBox viewcam)
		{
			Bitmap bmp;
			bmp = BitmapConverter.ToBitmap(image);
			viewcam.Image = bmp;
			viewcam.Refresh();
		}
		private void VideoPlay()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				if (!IsVideoOpen)
				{
					mainform.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
					return;
				}
				if (iRectangleArea > 0)
				{
					rect = new Rectangle(0, 0, 0, 0);
					iRectangleArea = 0;
				}
				if (Play == null)
				{
					//Play = new Thread[3];
					IsPlaying = true;
					Play = new Thread(() =>
					{
						if (Videos[0] != null || Videos[1] != null || Videos[2] != null)
						{
							//int videofps = (int)Math.Ceiling(fps) / 1000;
							int videofps = 1000 / fps;
							Stopwatch st = new Stopwatch();
							using (Mat image = new Mat())
							{
								while (IsPlaying)
								{
									long started = st.ElapsedMilliseconds;
									if (IsPaused) continue;
									if (FirstExistenceVideo.PosFrames >= FirstExistenceVideo.FrameCount - 1) break;
									if (Videos[0] != null && Videos[0].Read(image))
									{
										Invoke((Action)(() => VideoPlayProcessing(image, pb_ViewCam1)));
									}
									if (Videos[1] != null && Videos[1].Read(image))
									{
										Invoke((Action)(() => VideoPlayProcessing(image, pb_ViewCam2)));
									}
									if (Videos[2] != null && Videos[2].Read(image))
									{
										Invoke((Action)(() => VideoPlayProcessing(image, pb_ViewCam3)));
									}
									#region 람다식 원형
									//if (VideoCheck_Videos[0].Read(image))
									//{
									//	Invoke((Action)(() =>
									//	{
									//		bmp = BitmapConverter.ToBitmap(image);
									//		VideoCheck_Cam1.Image = bmp;
									//		VideoCheck_Cam1.Refresh();
									//	}));
									//}
									#endregion
									//if (NowSelectedCamNo == 1) VideoCheck_MainCam.Image = bmp;
									//VideoCheck_MainCam.Image = VideoCheck_Pictures[NowSelectedCamNo - 1].Image;
									Invoke((Action)(() =>
									{
										pb_SelectedView.Image = ViewCams[NowSelectedCamNo - 1].Image;
										pb_SelectedView.Refresh();
										tb_VideoTrack.Value = FirstExistenceVideo.PosFrames;
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
				if (IsPaused)
				{
					IsPaused = false;
					btn_VideoPlay.ImageOptions.SvgImage = Resources.pause;
				}
				else
				{
					IsPaused = true;
					btn_VideoPlay.ImageOptions.SvgImage = Resources.next;
				}
			}
			catch (Exception ex)
			{
				mainform.ShowMessage("오류", "영상 재생 중 예외가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
				return;
			}
		}
		private void VideoStop()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				if (!IsVideoOpen)
				{
					mainform.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
					return;
				}
				IsPaused = true;
				tb_VideoTrack.Value = 0;
				ThumbnailCreate();
				btn_VideoPlay.ImageOptions.SvgImage = Resources.next;
				pb_SelectedView.Image = ViewCams[NowSelectedCamNo - 1].Image;
			}
			catch (Exception ex)
			{
				mainform.ShowMessage("오류", "영상 정지 중 예외가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		private void VideoClose()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				if (!IsVideoOpen)
				{
					mainform.ShowMessage("재생", "영상을 불러오지 않았습니다!!", "주의");
					return;
				}
				IsPaused = true;
				btn_VideoPlay.ImageOptions.SvgImage = Resources.next;
				IsPlaying = false;
				tb_VideoTrack.Value = 0;
				tb_VideoTrack.Enabled = false;
				btn_VideoPlay.Enabled = false;
				btn_VideoStop.Enabled = false;
				btn_VideoClose.Enabled = false;
				for (int i = 0; i < ViewCams.Length; i++)
				{
					ViewCams[i].Image = null;
					if (Videos[i] != null)
					{
						Videos[i].PosFrames = 0;
						Videos[i].Release();
						Videos[i] = null;
					}
				}
				pb_SelectedView.Image = null;
				Play = null;
				IsVideoOpen = false;
			}
			catch (Exception ex)
			{
				mainform.ShowMessage("오류", "영상 닫기 중 예외가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		private void Track_Scroll(object sender, EventArgs e)
		{
			//MainForm.LoadingAnimationStart();
			if (IsPaused)
			{
				for (int i = 0; i < Videos.Length; i++)
				{
					Mat image = new Mat();
					Videos[i].PosFrames = tb_VideoTrack.Value;
					//Videos[i].Read(image);
					//Bitmap bmp = BitmapConverter.ToBitmap(image);
					//ViewCams[i].Image = bmp;
				}
			}
			else
			{
				IsPaused = true;
				for (int i = 0; i < Videos.Length; i++)
				{
					Videos[i].PosFrames = tb_VideoTrack.Value;
				}
				IsPaused = false;
			}
			pb_SelectedView.Image = ViewCams[NowSelectedCamNo - 1].Image;
			//MainForm.LoadingAnimationEnd();
		}
		private void TrackBar_ValueChanged(object sender, EventArgs e)
		{
			if (tb_VideoTrack.Value >= Videos[0].FrameCount - 1) VideoStop();
			if (tb_VideoTrack.Value >= Videos[0].FrameCount - 1)
			{
				//for (int i = 0; i < Videos.Length; i++)
				//{
				//	Videos[i].Open(FilesPath[i]);
				//}
				//IsPaused = true;
				//IsPlaying = false;
				//tb_VideoTrack.Value = 0;
				//tb_VideoTrack.Refresh();
				//Videos[0].PosFrames = 0;
				//Videos[1].PosFrames = 0;
				//Videos[2].PosFrames = 0;
				//btn_VideoPlay.ImageOptions.SvgImage = Resources.next;
				//Play = null;
			}
		}

		private bool drawing = false;
		private Rectangle rect;
		int istartX = 0, istartY = 0, iendX = 0, iendY = 0;
		int iRealStartPointX = 0, iRealStartPointY = 0, iRealEndPointX = 0, iRealEndPointY = 0;
		int imouseDownX, imouseDownY;
		int iRectangleArea;
		ToolTip tooltip = new ToolTip();// 나중에 삭제!!
		private void SelectedView_Click(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			MouseEventArgs mouseEvent = (MouseEventArgs)e;
			if (mouseEvent.Button == MouseButtons.Right)
			{
				if (iRectangleArea > 0)
				{
					ContextMenu contextmenu = new ContextMenu();
					MenuItem item1 = new MenuItem();
					item1.Text = "분석";
					item1.Click += (senders, es) => 
					{
						// 마우스 드레그로 만든 영역의 이미지
						RealPointDetecter(pb_SelectedView, out iRealStartPointX, out iRealStartPointY, istartX, istartY);
						RealPointDetecter(pb_SelectedView, out iRealEndPointX, out iRealEndPointY, iendX, iendY);
						Rectangle rectangle = new Rectangle(iRealStartPointX, iRealStartPointY, iRealEndPointX - iRealStartPointX, iRealEndPointY - iRealStartPointY);
						Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height);
						using (Graphics graphics = Graphics.FromImage(bitmap))
						{
							graphics.DrawImage(pb_SelectedView.Image, 0, 0, rectangle, GraphicsUnit.Pixel);
						}
						mainform.analysisbitmap = bitmap;
						mainform.PageChange(2);
						//mainform.analysispage.iRealStartPointX = iRealStartPointX;
						//mainform.analysispage.iRealStartPointY = iRealStartPointY;
						//mainform.analysispage.iRealEndPointX   = iRealEndPointX;
						//mainform.analysispage.iRealEndPointY   = iRealEndPointY;
					};
					contextmenu.MenuItems.Add(item1);
					contextmenu.Show(pb_SelectedView, new System.Drawing.Point(mouseEvent.X, mouseEvent.Y));
				}
			}
			iRectangleArea = (iendX - istartX) * (iendY - istartY);
		}
		private void RealPointDetecter(PictureBox picturebox, out int targetX, out int targetY, int sourceX, int sourceY)
		{
			int pictureboxWidth		= picturebox.Width;
			int pictureboxHeight	= picturebox.Height;
			int imageWidth			= picturebox.Image.Width;
			int imageHeight			= picturebox.Image.Height;

			float pictureBoxAspectRatio = pictureboxWidth / (float)pictureboxHeight;
			float imageAspectRatio = imageWidth / (float)imageHeight;

			if(pictureBoxAspectRatio > imageAspectRatio)
			{
				targetY = (int)(imageHeight * sourceY / (float)pictureboxHeight);
				float scaledWidth = imageWidth * pictureboxHeight / imageHeight;
				float deltaX = (pictureboxWidth - scaledWidth) / 2;
				targetX = (int)((sourceX - deltaX) * imageHeight / (float)pictureboxHeight);
			}
			else
			{
				targetX = (int)(imageWidth * sourceX / (float)pictureboxWidth);
				float scaledHeight = imageHeight * pictureboxWidth / imageWidth;
				float deltaY = (pictureboxHeight - scaledHeight) / 2;
				targetY = (int)((sourceY - deltaY) * imageWidth / pictureboxWidth);
			}
		}
		private void SelectedView_MouseDown(object sender, MouseEventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			if(e.Button == MouseButtons.Left)
			{
				IsSelecting = true;
				if (IsPlaying && !IsPaused) VideoPlay();
				imouseDownX = istartX = iendX = e.X;
				imouseDownY = istartY = iendY = e.Y;
				rect = new Rectangle(imouseDownX, imouseDownY, 0, 0);
				pb_SelectedView.Refresh();
			}
		}

		private void SelectedView_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				tooltip.SetToolTip(pb_SelectedView, "시작좌표X : " + istartX + "\n시작좌표Y : " + istartY
				+ "\n끝 좌 표 X : " + iendX + "\n끝 좌 표 Y : " + iendY + "\n너비 : " + iRectangleArea);
				if (iRectangleArea > 0)
				{
					//pb_SelectedView.AddControl(film);
				}
				else
				{
					IsSelecting = false;
					if (IsPlaying && !IsPaused) VideoPlay();
				}
				pb_SelectedView.Refresh();
			}
		}

		private void SelectedView_MouseMove(object sender, MouseEventArgs e)
		{
			//Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			if (e.Button == MouseButtons.Left)
			{
				if (imouseDownX < e.X)
				{
					istartX = imouseDownX;
					iendX = e.X;
				}
				else
				{
					istartX = e.X;
					iendX = imouseDownX;
				}
				if (imouseDownY < e.Y)
				{
					istartY = imouseDownY;
					iendY = e.Y;
				}
				else
				{
					istartY = e.Y;
					iendY = imouseDownY;
				}
				// 시작점 또는 끝점이 0보다 작은가?
				istartX = istartX < 0 ? 0 : istartX;
				istartY = istartY < 0 ? 0 : istartY;
				iendX = iendX < 0 ? 0 : iendX;
				iendY = iendY < 0 ? 0 : iendY;

				// 시작점 또는 끝점이 픽쳐박스의 최대값보다 큰가?
				istartX = istartX > pb_SelectedView.Width ? pb_SelectedView.Width : istartX;
				istartY = istartY > pb_SelectedView.Height ? pb_SelectedView.Height : istartY;
				iendX = iendX > pb_SelectedView.Width ? pb_SelectedView.Width : iendX;
				iendY = iendY > pb_SelectedView.Height ? pb_SelectedView.Height : iendY;

				rect = new Rectangle(istartX, istartY, iendX - istartX, iendY - istartY);
				iRectangleArea = (iendX - istartX) * (iendY - istartY);
				pb_SelectedView.Refresh();
			}
		}
		private void SelectedView_MouseLeave(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			MouseEventArgs mouseEvent = (MouseEventArgs)e;
			if (mouseEvent.Button != MouseButtons.Left) istartX = istartY = iendX = iendY = iRectangleArea = 0;
		}
		private void SelectedView_Paint(object sender, PaintEventArgs e)
		{
			using (Pen pen = new Pen(Color.Red, 3))
			{
				e.Graphics.DrawRectangle(pen, rect);
				if (iRectangleArea > 0)
				{
					using (Brush brush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
					{
						e.Graphics.FillRectangle(brush, 0, 0, pb_SelectedView.Width, istartY);
						e.Graphics.FillRectangle(brush, 0, iendY, pb_SelectedView.Width, (pb_SelectedView.Height - iendY));
						e.Graphics.FillRectangle(brush, 0, istartY, istartX, (iendY - istartY));
						e.Graphics.FillRectangle(brush, iendX, istartY, (pb_SelectedView.Width - iendX), (iendY - istartY));
						//e.Graphics.FillRectangle(brush, new Rectangle(0, 0, pb_SelectedView.Width, pb_SelectedView.Height));
					}
				}
			}
		}
		public void CameraClose()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				IsPlaying = false;
				if (Play != null)
				{
					//Play.Interrupt();
					Play.Abort();
					Play = null;
				}
				for (int index = 0; index < Camera_Count; index++)
				{
					if (m_thread[index] != null)
					{
						m_isWork[index] = false;
						m_thread[index].Join();
						m_thread[index] = null;
					}
					if (m_Camera[index] != null)
					{
						m_Camera[index].Stop();
						m_Camera[index].Close();
					}
				}
			}
			catch (Exception ex)
			{
				//MainForm.ShowMessage("종료", "영상 확인 종료 중 오류가 발생하였습니다!!\n" + ex.Message, "주의");
				mainform.ShowMessage("종료", "종료 중 예외가 발생하였습니다!!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
	}
}
