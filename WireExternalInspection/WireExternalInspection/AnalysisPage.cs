using DevExpress.XtraEditors;
using OpenCvSharp.Extensions;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Reflection.Emit;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace WireExternalInspection
{
	public struct CannyEdgeFigure
	{
		public int cannyLowerThreshold;
		public int cannyUpperThreshold;
	}
	public struct BinaryFigure
	{
		public int binaryThresh;
		public int binaryMaxValue;
	}
	public partial class AnalysisPage : DevExpress.XtraEditors.XtraUserControl
	{
		public MainForm mainform;
		public LogRecordProcessing Log;
		public Bitmap analysisbitmap;
		private Mat analysismat = new Mat();
		public PDFSave pdf;

		public CannyEdgeFigure cannyfigure;
		public BinaryFigure binaryfigure;
		//public int iRealStartPointX = 0, iRealStartPointY = 0, iRealEndPointX = 0, iRealEndPointY = 0;

		public AnalysisPage(MainForm form)
		{
			InitializeComponent();
			mainform = form;
			Log = mainform.Log;

			cannyfigure = new CannyEdgeFigure();
			binaryfigure = new BinaryFigure();

			lb_Inspection_User.Text = mainform.Xml.WorkFileSetting.WorkUser;
			lb_Inspection_Place.Text = mainform.Xml.WorkFileSetting.WorkTarget;
			lb_Inspection_Date.Text = mainform.Xml.WorkFileSetting.WorkDay;
		}
		public void ImageChange(Bitmap ChangedBitmap)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				analysisbitmap = ChangedBitmap;
				pb_OriginalImg.Image = analysisbitmap;
				pb_OriginalImg.Refresh();

				analysismat = GaussianBlur(BitmapConverter.ToMat(analysisbitmap));
				Cv2.CvtColor(analysismat, analysismat, ColorConversionCodes.RGBA2RGB);

				pb_AnalysisImage.Image = BitmapConverter.ToBitmap(analysismat);
			}
			catch (Exception ex)
			{
				mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
				mainform.ShowMessage("오류", $"이미지를 변경하던 중 예외가 발생하였습니다!\n" + ex.Message, "주의");
			}
			pb_AnalysisImage.Refresh();
		}
		private Mat GaussianBlur(Mat mat)
		{
			mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			Mat BlurMat = new Mat();
			try
			{
				Cv2.GaussianBlur(mat, BlurMat, new OpenCvSharp.Size(3, 3), 1, 0, BorderTypes.Default);
				BlurMat.ConvertTo(BlurMat, MatType.CV_8UC1);
			}
			catch (Exception ex)
			{
				mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			return BlurMat;
		}
		private void ViewBack_Click(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			mainform.PageChange(1);
		}
		private void ButtonFilter_Click(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			Mat FilterMat = new Mat();
			string FilterName = ((SimpleButton)sender).Name.Substring("btn_Filter_".Length);
			try
			{
				switch (FilterName)
				{
					case "CannyEdge":
						tb_OptionTrackBar1.Properties.Maximum = 500;
						tb_OptionTrackBar2.Properties.Maximum = 500;

						lb_OptionName1.Text = "하위 임계값";
						if (cannyfigure.cannyLowerThreshold == 0)
							cannyfigure.cannyLowerThreshold = tb_OptionTrackBar1.Value = 170;
						lb_OptionName2.Text = "상위 임계값";
						if (cannyfigure.cannyUpperThreshold == 0)
							cannyfigure.cannyUpperThreshold = tb_OptionTrackBar2.Value = 230;

						Cv2.Canny(analysismat, FilterMat, cannyfigure.cannyLowerThreshold, cannyfigure.cannyUpperThreshold, 3, true);
						pn_OptionPanel.Visible = true;
						break;
					case "Sobel":
						Cv2.Sobel(analysismat, FilterMat, MatType.CV_8U, 1, 0);
						pn_OptionPanel.Visible = false;
						break;
					case "Scharr":
						Cv2.Scharr(analysismat, FilterMat, MatType.CV_8U, 1, 0);
						pn_OptionPanel.Visible = false;
						break;
					case "Laplacian":
						Cv2.Laplacian(analysismat, FilterMat, MatType.CV_8U, 5);
						pn_OptionPanel.Visible = false;
						break;
					case "Threshold":
						tb_OptionTrackBar1.Properties.Maximum = 255;
						tb_OptionTrackBar2.Properties.Maximum = 255;

						lb_OptionName1.Text = "Thresh";
						if (binaryfigure.binaryThresh == 0)
							binaryfigure.binaryThresh = tb_OptionTrackBar1.Value = 145;
						lb_OptionName2.Text = "MaxValue";
						if (binaryfigure.binaryMaxValue == 0)
							binaryfigure.binaryMaxValue = tb_OptionTrackBar2.Value = 255;
						Cv2.CvtColor(analysismat, FilterMat, ColorConversionCodes.BGR2GRAY);
						Cv2.Threshold(FilterMat, FilterMat, binaryfigure.binaryThresh, binaryfigure.binaryMaxValue, ThresholdTypes.Binary);
						pn_OptionPanel.Visible = true;
						break;
					default:
						FilterMat = analysismat;
						pn_OptionPanel.Visible = false;
						break;
				}
			}
			catch (Exception ex)
			{
				mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			pb_AnalysisImage.Image = BitmapConverter.ToBitmap(FilterMat);
		}
		private void OptionTrackBarChanged(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			Mat FiguredMat = new Mat();
			string TrackBarName = ((TrackBarControl)sender).Name;
			try
			{
				switch (TrackBarName)
				{
					case "tb_OptionTrackBar1":
						if (lb_OptionName1.Equals("하위 임계값"))
						{
							cannyfigure.cannyLowerThreshold = tb_OptionTrackBar1.Value;
							Cv2.Canny(analysismat, FiguredMat, cannyfigure.cannyLowerThreshold, cannyfigure.cannyUpperThreshold, 3, true);
						}
						else
						{
							binaryfigure.binaryThresh = tb_OptionTrackBar1.Value;
							Cv2.CvtColor(analysismat, FiguredMat, ColorConversionCodes.BGR2GRAY);
							Cv2.Threshold(FiguredMat, FiguredMat, binaryfigure.binaryThresh, binaryfigure.binaryMaxValue, ThresholdTypes.Binary);
						}
						break;
					case "tb_OptionTrackBar2":
						if (lb_OptionName1.Equals("상위 임계값"))
						{
							cannyfigure.cannyUpperThreshold = tb_OptionTrackBar2.Value;
							Cv2.Canny(analysismat, FiguredMat, cannyfigure.cannyLowerThreshold, cannyfigure.cannyUpperThreshold, 3, true);
						}
						else
						{
							binaryfigure.binaryMaxValue = tb_OptionTrackBar2.Value;
							Cv2.CvtColor(analysismat, FiguredMat, ColorConversionCodes.BGR2GRAY);
							Cv2.Threshold(FiguredMat, FiguredMat, binaryfigure.binaryThresh, binaryfigure.binaryMaxValue, ThresholdTypes.Binary);
						}
						break;
					default: break;
				}
			}
			catch (Exception ex)
			{
				mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			pb_AnalysisImage.Image = BitmapConverter.ToBitmap(FiguredMat);
			pb_AnalysisImage.Refresh();
		}
		private void Report_Export_Click(object sender, EventArgs e)
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				pdf = new PDFSave(mainform, pb_OriginalImg.Image, pb_AnalysisImage.Image);
				pdf.ShowDialog();
			}
			catch(Exception ex)
			{
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
	}
}
