using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OMRON_Camera_Control;
using DevExpress.XtraSplashScreen;
using System.Drawing.Imaging;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using Accord.Video.FFMPEG;
using WireVisionInspection.Properties;
using System.IO;
using System.Reflection;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace WireVisionInspection
{
	public partial class RealTimeView_Page : XtraUserControl
	{
		Form1 MainForm;
		public LogRecord Log;
		public int Camera_Count = 3;
		private CameraControl[] m_Camera = null;
		public Mutex[] m_mutexImage = null;
		public Bitmap[] m_bitmap = null;
		public Thread[] m_thread = null;
		VideoWriter[] VideoFiles = null;
		public delegate void PaintDelegate(PictureBox pb_cam, Bitmap bmp, Mutex mutex);
		#region 상태
		public bool[] m_isWork = null;
		public int NowSelectedCamNo = 1;
		public int PastSelectedCamNo = 0;
		public bool IsRecord = false;
		public object[] lParameters;
		public bool IsAnotherPage;
		#endregion
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
		public RealTimeView_Page(Form1 form)
		{
			InitializeComponent();
			this.MainForm = form;
			Log = MainForm.Log;
			m_Camera = new CameraControl[Camera_Count];
			m_mutexImage = new Mutex[Camera_Count];
			m_bitmap = new Bitmap[Camera_Count];
			m_thread = new Thread[Camera_Count];
			m_isWork = new bool[Camera_Count];

			for (int i = 0; i < Camera_Count; i++)
			{
				m_Camera[i] = new CameraControl(MainForm);
				//if(!m_Camera[i].IsOpened) return;
				m_mutexImage[i] = new Mutex();
			}
			PanelSettings();
			NowSelectedCamNo = MainForm.Settings.ProgramSetting.BasicCameraView + 1;
		}
		private void RealTimeView_Page_Load(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				VideoSetLoad();
				for (int index = 0; index < Camera_Count; index++)
				{
					m_Camera[index].CameraOpen();
					if (!m_Camera[index].IsOpened)
					{
						//MainForm.ShowMessage("오류", "카메라 연결에 실패하였습니다!", "경고");
						//MainForm.Close();
						return;
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
				}
				timer1.Start();
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "카메라 연결에 실패하였습니다!\n프로그램을 종료합니다!\n" + ex.Message, "경고");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		public void VideoSetLoad()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				cb_TextView.Text = MainForm.Settings.CameraSetting.TextMark;
				if (cb_TextView.Text.Equals("날짜/시간"))
				{
					lb_TextView.Text = "  ☑ 년월일시분초";
					txt_UserText.Enabled = false;
				}
				else if (cb_TextView.Text.Equals("로봇거리"))
				{
					lb_TextView.Text = "  ☑ 로봇 거리";
					txt_UserText.Enabled = false;
				}
				else
				{
					lb_TextView.Text = "";
					txt_UserText.Text = MainForm.Settings.CameraSetting.UserText;
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "영상의 텍스트를 변경하는 중 예외가 발생하였습니다!" + ex.Message, "경고");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		//public bool VideoSetSave()
		//{
		//	string XMLFileName = "CameraSetting.xml";
		//	try
		//	{
		//		if (!MainForm.Settings.CameraSetting.TextMark.Equals(cb_TextView.Text)) MainForm.Settings.CameraSetting.TextMark = cb_TextView.Text;
		//		if (!MainForm.Settings.CameraSetting.UserText.Equals(txt_UserText.Text)) MainForm.Settings.CameraSetting.UserText = txt_UserText.Text;
		//		MainForm.Settings.XMLSave(MainForm.Settings.CameraSetting, MainForm.Settings.CameraSetting.GetType(), @"\" + XMLFileName);
		//	}
		//	catch (Exception ex)
		//	{
		//		MainForm.ShowMessage("오류", "카메라 설정에 예외가 발생하였습니다!\n" + ex, "주의");
		//		return false;
		//	}
		//	finally
		//	{
		//		MainForm.Settings.XMLLoad_Camera();
		//	}
		//	return true;
		//}
		private void CreateBitmap(int index, int width, int height, System.Drawing.Imaging.PixelFormat format)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			m_bitmap[index] = new Bitmap((int)width, (int)height, PixelFormat.Format24bppRgb);
		}

		#region 카메라 화면을 PictureBox에 보여줌
		#region 카메라 1
		private static void DisplayThread_Cam1(object aParameters)
		{
			object[] lParameters = (object[])aParameters;
			RealTimeView_Page lThis = (RealTimeView_Page)lParameters[0];

			while (lThis.m_isWork[0])
			{
				try
				{
					if (lThis.MainForm.NowPageNo == 1)
					{
						Thread.Sleep(100);
						if (lThis.MainForm.PastPageNo != 1)
						{
							Thread.Sleep(1500);
							lThis.MainForm.PastPageNo = 1;
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

							lThis.BeginInvoke(new PaintDelegate(lThis.Draw_Cam), new object[3] { lThis.RealTimeView_Cam1, lThis.m_bitmap[0], lThis.m_mutexImage[0] });
							lThis.m_Camera[0].OnResetEventGrabDone();

							lThis.m_mutexImage[0].ReleaseMutex();
						}
						else lThis.m_isWork[0] = false;
					}
				}
				catch (Exception ex)
				{
					lThis.MainForm.ShowMessage("오류", "Cam1 Thread 오류!!\n" + ex.Message, "경고");
				}
			}
		}
		#endregion
		#region 카메라 2
		private static void DisplayThread_Cam2(object aParameters)
		{
			object[] lParameters = (object[])aParameters;
			RealTimeView_Page lThis = (RealTimeView_Page)lParameters[0];

			while (lThis.m_isWork[1])
			{
				try
				{
					if (lThis.MainForm.NowPageNo == 1)
					{
						Thread.Sleep(100);
						if (lThis.MainForm.PastPageNo != 1)
						{
							Thread.Sleep(1500);
							lThis.MainForm.PastPageNo = 1;
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

							lThis.BeginInvoke(new PaintDelegate(lThis.Draw_Cam), new object[3] { lThis.RealTimeView_Cam2, lThis.m_bitmap[1], lThis.m_mutexImage[1] });

							lThis.m_Camera[1].OnResetEventGrabDone();

							lThis.m_mutexImage[1].ReleaseMutex();
						}
						else lThis.m_isWork[1] = false;
					}
				}
				catch (Exception ex)
				{
					lThis.MainForm.ShowMessage("오류", "Cam2 Thread 오류!!\n" + ex.Message, "경고");
				}
			}
		}
		#endregion
		#region 카메라 3
		private static void DisplayThread_Cam3(object aParameters)
		{
			object[] lParameters = (object[])aParameters;
			RealTimeView_Page lThis = (RealTimeView_Page)lParameters[0];

			while (lThis.m_isWork[2])
			{
				try
				{
					if (lThis.MainForm.NowPageNo == 1)
					{
						Thread.Sleep(100);
						if (lThis.MainForm.PastPageNo != 1)
						{
							Thread.Sleep(1500);
							lThis.MainForm.PastPageNo = 1;
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

							lThis.BeginInvoke(new PaintDelegate(lThis.Draw_Cam), new object[3] { lThis.RealTimeView_Cam3, lThis.m_bitmap[2], lThis.m_mutexImage[2] });

							lThis.m_Camera[2].OnResetEventGrabDone();

							lThis.m_mutexImage[2].ReleaseMutex();
						}
						else lThis.m_isWork[2] = false;
					}
				}
				catch (Exception ex)
				{
					lThis.MainForm.ShowMessage("오류", "Cam3 Thread 오류!!\n" + ex.Message, "경고");
				}
			}
		}
		#endregion
		#region 받은 카메라 데이터를 PictureBox Image로 넘김
		void Draw_Cam(PictureBox pb_cam, Bitmap bmp, Mutex mutex)
		{
			try
			{
				if (MainForm.navigationFrame2.SelectedPageIndex != 0) return;

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
								stringFormat.Alignment = StringAlignment.Near;
								stringFormat.LineAlignment = StringAlignment.Near;
								PointF point = new PointF();
								//PointF point = new PointF(850, 100);
								string CamText = "";
								switch (pb_cam.Name)
								{
									case "RealTimeView_Cam1": CamText = "CAM1"; break;
									case "RealTimeView_Cam2": CamText = "CAM2"; break;
									case "RealTimeView_Cam3": CamText = "CAM3"; break;
									default					: CamText = "";		break;
								}
								switch (MainForm.Settings.CameraSetting.TextMark)
								{
									case "날짜/시간"	: CamText += DateTime.Now.ToString(" yyyy년 MM월 dd일 HH시 mm분 ss초");									break;
									case "로봇거리"		: CamText += " 로봇거리" + DateTime.Now.ToString(" HH시 mm분 ss초");									break;
									case "사용자 선택"	: CamText += " " + MainForm.Settings.CameraSetting.UserText + DateTime.Now.ToString(" HH시 mm분 ss초");	break;
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
					#endregion

					Bitmap bmpimg = (Bitmap)bmp.Clone();
					pb_cam.Image = bmpimg;
					int CamNo = int.Parse(pb_cam.Name.Substring(("RealTimeView_Cam".Length), 1));
					if (CamNo == NowSelectedCamNo)
					{
						RealTimeView_MainCam.Image = bmpimg;
						//if (NowSelectedCamNo > 0 && NowSelectedCamNo < 4)
						//{

						//}
					}
					if (IsRecord)
					{
						//VideoFiles[int.Parse(pb_cam.Name.Substring(("RealTimeView_Cam".Length), 1)) - 1].WriteVideoFrame(bmpimg);
						VideoFiles[CamNo - 1].Write(BitmapConverter.ToMat(bmpimg));
					}
				}
				else
				{
					RealTimeView_MainCam.Image = null;
					pb_cam.Image = null;
				}
			}
			catch (System.Exception ex)
			{
				//System.Diagnostics.Trace.WriteLine(exc.Message, "System Exception");
				MainForm.ShowMessage("카메라", "카메라의 화면을 가져올 수 없습니다!\n" + ex.Message, "경고");
			}
			finally
			{
				if (MainForm.navigationFrame2.SelectedPageIndex == 0) mutex.ReleaseMutex();
			}
		}
		#endregion
		#endregion
		#region 페널 세팅
		private void PanelSettings()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
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
				sb.Text = "화면을 다시 열려면\n더블클릭 하세요";
				sb.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
				sb.Dock = DockStyle.Fill;
				sb.DoubleClick += new EventHandler(PanelDoubleClick);
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "페널을 닫는 중 오류가 발생하였습니다!\n" + ex.Message, "주의");
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
				MainForm.ShowMessage("오류", "페널을 여는 중 오류가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		#endregion
		#region 실시간 감시 카메라 화면을 클릭했을 때
		public void RealTimeView_CamActions(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				if (IsRecord)
				{
					MainForm.ShowMessage("녹화 중...", "녹화 중는 카메라를 변경할 수 없습니다!!", "주의");
					return;
				}
				lbc_NowCamInformation.Text = "";
				switch (((PictureBox)sender).Name)
				{
					case "RealTimeView_Cam1":
						NowSelectedCamNo = 1;
						break;
					case "RealTimeView_Cam2":
						NowSelectedCamNo = 2;
						break;
					case "RealTimeView_Cam3":
						NowSelectedCamNo = 3;
						break;
					default:
						break;
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "화면을 전환하던 중 오류가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//Viewer_Thread.ViewSetting(NowSelectedCamNo, IsViewing);
			//stPictureBox_Main.Image = null;
		}
		#endregion
		#region 실시간 감시 페이지 버튼 클릭 이벤트
		private void RealTimeView_ButtonActions(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			string ButtonText = ((SimpleButton)sender).Text;
			if(!ButtonText.Equals("동영상 정보 설정")) popupControlContainer1.Visible = false;
			lbc_NowCamInformation.Text = "";
			if (IsRecord && !ButtonText.Equals("녹화 중지"))
			{
				MainForm.ShowMessage("녹화 중...", "녹화 중는 페이지를 변경할 수 없습니다!!", "주의");
				return;
			}
			try
			{
				switch (ButtonText)
				{
					case "이미지 저장":
						ImageSaveAction();
						break;
					case "동영상 저장":
						btn_VideoSave.Text = "녹화 중지";
						btn_VideoSave.ImageOptions.SvgImage = Resources.stop;
						if (!VideoSaveAction())
						{
							//ShowMessage("화면 오류", "화면 표시 오류!!", "경고");
							IsRecord = false;
							btn_VideoSave.Text = "동영상 저장";
							btn_VideoSave.ImageOptions.SvgImage = Resources.imagetoolssetimagescale;
						}
						break;
					case "녹화 중지":
						IsRecord = false;
						Thread.Sleep(500);
						for (int i = 0; i < VideoFiles.Length; i++)
						{
							VideoFiles[i].Release();
							VideoFiles[i] = null;
						}
						btn_VideoSave.Text = "동영상 저장";
						btn_VideoSave.ImageOptions.SvgImage = Resources.imagetoolssetimagescale;
						MainForm.ShowMessage("저장 중...", FolderPath(2) + "\n저장되었습니다!", "알림");
						break;
					case "현재 카메라 정보":
						Camera_Info();
						break;
					case "동영상 정보 설정":
						if (popupControlContainer1.Visible) popupControlContainer1.Visible = false;
						else popupControlContainer1.Visible = true;
						//validationHint1.Properties.State = Utils.VisualEffects.ValidationHintState.Invalid;
						break;
					case "닫  기":
						popupControlContainer1.Visible = false;
						break;
						//case "저  장":
						//	if (!VideoSetSave()) MainForm.ShowMessage("알림", $"카메라 설정 변경이 실패하였습니다!", "알림");
						//	else MainForm.ShowMessage("알림", $"카메라 설정이 변경되었습니다!", "알림");
						//	popupControlContainer1.Visible = false;
						//	break;
						//case "취  소":
						//	popupControlContainer1.Visible = false;
						//	break;
				}
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "잘못된 버튼 이벤트 입니다!!\n" + ex.Message, "주의");
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
					if (IsRecord) MainForm.ShowMessage("저장 중...", "이미지를 저장할 수 없습니다!!", "주의");
					return;
				}
				PictureBox[] CamImage = new PictureBox[] { RealTimeView_Cam1, RealTimeView_Cam2, RealTimeView_Cam3 };
				ImageFormat Imageformat;
				string ImageFormatText = MainForm.Settings.ProgramSetting.ImageFileFormat;
				switch (ImageFormatText)
				{
					case "JPG": Imageformat = ImageFormat.Jpeg; break;
					case "PNG": Imageformat = ImageFormat.Png; break;
					case "BMP": Imageformat = ImageFormat.Bmp; break;
					default: Imageformat = ImageFormat.Jpeg; break;
				}
				for (int i = 1; i < 4; i++)
				{
					string folderpath = FolderPath(1, i);
					string nowtime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
					string imagepath = folderpath + nowtime + "." + ImageFormatText.ToLower();
					CamImage[i - 1].Image.Save(imagepath, Imageformat);
				}
				MainForm.ShowMessage("저장 중...", FolderPath(1) + "\n저장되었습니다!", "알림");
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("저장 중...", "이미지 저장을 실패했습니다!!\n" + ex.Message, "주의");
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
				for (int i = 1; i < 4; i++)
				{
					string folderpath = FolderPath(2, i);
					string videopath = folderpath + nowtime + " 녹화.mp4";
					//VideoFiles[i - 1].Open(videopath, m_bitmap[NowSelectedCamNo - 1].Width, m_bitmap[NowSelectedCamNo - 1].Height, 7/*Accord.Math.Rational.FromDouble(7.75)*/, VideoCodec.MPEG4, 100000);
					VideoFiles[i - 1].Open(videopath, FourCC.MPG4, 7, new OpenCvSharp.Size(m_bitmap[NowSelectedCamNo - 1].Width, m_bitmap[NowSelectedCamNo - 1].Height), true);
				}
				IsRecord = true;
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("녹화 중...", "동영상 저장을 실패했습니다!!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
				return false;
			}
			return true;
		}
		#endregion
		#region 카메라 정보를 스테이터스 스트립에 표시
		public void Camera_Info()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				string strAnd = " / ";
				string name = "  Camera Name : " + m_Camera[NowSelectedCamNo - 1].DeviceModelName;
				string serial = "Camera SerialNo : " + m_Camera[NowSelectedCamNo - 1].DeviceSerialNumber;
				string version = "Camera Version : " + m_Camera[NowSelectedCamNo - 1].DeviceVersion;
				string ipaddress = "IP Address : " + m_Camera[NowSelectedCamNo - 1].GevDeviceIPAddress.ToString();
				lbc_NowCamInformation.Text = name + strAnd + serial + strAnd + version + strAnd + ipaddress;
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "카메라의 상태를 확인해주세요!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		#endregion
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
					path = MainForm.Settings.ProgramSetting.ImageFilePath;
					switch (CamNo)
					{
						case 1: path += @"\Cam1\"; break;
						case 2: path += @"\Cam2\"; break;
						case 3: path += @"\Cam3\"; break;
					}
				}
				else if (ImageVideo == 2)
				{
					path = MainForm.Settings.ProgramSetting.VideoFilePath;
					switch (CamNo)
					{
						case 1: path += @"\Cam1\"; break;
						case 2: path += @"\Cam2\"; break;
						case 3: path += @"\Cam3\"; break;
					}
				}
				else
				{
					MainForm.ShowMessage("오류", "폴더를 확인할 수 없습니다!!", "주의");
					return "";
				}
				DirectoryInfo di = new DirectoryInfo(path);
				if (di.Exists == false) di.Create();
				return path;
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "폴더 확인에 실패했습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
				return "";
			}
		}
		private void cb_TextView_SelectedValueChanged(object sender, EventArgs e)
		{
			//MainForm.LoadingAnimationStart();
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				if (cb_TextView.Text.Equals("사용자 선택"))
				{
					txt_UserText.Enabled = true;
					lb_TextView.Text = "사용자 텍스트 입력";

					if(MainForm.Settings.UserSet) MainForm.Settings.txt_UserText_CameraSetting.Enabled = true;
					MainForm.Settings.cb_TextView_CameraSetting.Text = cb_TextView.Text;
					MainForm.Settings.lb_TextView_CameraSetting.Text = "  사용자 텍스트 입력";
				}
				else
				{
					if (cb_TextView.Text.Equals("날짜/시간")) lb_TextView.Text = "☑ 년월일시분초";
					if (cb_TextView.Text.Equals("로봇거리")) lb_TextView.Text = "☑ 로봇 거리";
					txt_UserText.Text = "";
					txt_UserText.Enabled = false;

					if (cb_TextView.Text.Equals("날짜/시간")) MainForm.Settings.lb_TextView_CameraSetting.Text = "  ☑ 년월일시분초";
					if (cb_TextView.Text.Equals("로봇거리")) MainForm.Settings.lb_TextView_CameraSetting.Text = "  ☑ 로봇 거리";
					MainForm.Settings.cb_TextView_CameraSetting.Text = cb_TextView.Text;
					MainForm.Settings.txt_UserText_CameraSetting.Text = "";
					MainForm.Settings.txt_UserText_CameraSetting.Enabled = false;
				}
				MainForm.Settings.CameraSetting.TextMark = cb_TextView.Text;
				if (!m_Camera[0].IsOpened) txt_UserText.Text = MainForm.Settings.CameraSetting.UserText;
				else MainForm.Settings.CameraSetting.UserText = txt_UserText.Text;
				MainForm.Settings.XMLSave(MainForm.Settings.CameraSetting, MainForm.Settings.CameraSetting.GetType(), @"\CameraSetting.xml");
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "영상의 텍스트를 변경하는 중 예외가 발생하였습니다!\n" + ex.Message, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//MainForm.LoadingAnimationEnd();
		}
		private void UserTextChange(object sender, EventArgs e)
		{
			//MainForm.LoadingAnimationStart();
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				if (!m_Camera[0].IsOpened) txt_UserText.Text = MainForm.Settings.CameraSetting.UserText;
				else MainForm.Settings.CameraSetting.UserText = txt_UserText.Text;
				MainForm.Settings.txt_UserText_CameraSetting.Text = txt_UserText.Text;
				MainForm.Settings.XMLSave(MainForm.Settings.CameraSetting, MainForm.Settings.CameraSetting.GetType(), @"\CameraSetting.xml");
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "텍스트를 변경하는 중 예외가 발생하였습니다!\n" + ex, "주의");
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			//MainForm.LoadingAnimationEnd();
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			if (!m_isWork[0] && !m_isWork[1] && !m_isWork[2]) MainForm.Close();
		}
		public void CameraClose()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
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
	}
}
