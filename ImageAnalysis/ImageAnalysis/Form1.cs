using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Internal.Vectors;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ImageAnalysis
{
	public partial class Form1 : Form
	{
		Mat image;
		OpenCvSharp.Point[][] contours;
		//최적 : 235, 255
		int Threshold1 = 235;
		int Threshold2 = 255;
		//최적 : 170, 230
		int Canny1 = 170;
		int Canny2 = 230;
		int square = 1000;
		decimal delta = 0.06m;
		RetrievalModes retrievalModes = RetrievalModes.Tree;
		ContourApproximationModes contourApproximationModes = ContourApproximationModes.ApproxNone;
		public Form1()
		{
			InitializeComponent();
			//ComboInsert();
		}
		private void ComboInsert()
		{
			comboBox1.Items.Add(RetrievalModes.Tree);
			comboBox1.Items.Add(RetrievalModes.External);
			comboBox1.Items.Add(RetrievalModes.List);
			comboBox1.Items.Add(RetrievalModes.CComp);

			comboBox2.Items.Add(ContourApproximationModes.ApproxNone);
			comboBox2.Items.Add(ContourApproximationModes.ApproxSimple);
			comboBox2.Items.Add(ContourApproximationModes.ApproxTC89L1);
			comboBox2.Items.Add(ContourApproximationModes.ApproxTC89KCOS);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true })
			{
				ofd.Filter = "이미지 파일 (Image File)|*.bmp;*.jpg;*.jpeg;*.png;*.tiff";
				ofd.InitialDirectory = @"C:\FS-MCS500POE_Video_Save";

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					string FileName = ofd.FileName;
					image = new Mat(FileName);
					Mat canny = new Mat();
					Cv2.Canny(image, canny, Canny1, Canny2);

					pictureBox1.Image = BitmapConverter.ToBitmap(canny);
				}
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Mat src = image;
			Mat src2 = new Mat();
			src.CopyTo(src2);
			HierarchyIndex[] hierarchy;
			Mat bin = new Mat();
			Cv2.CvtColor(src, bin, ColorConversionCodes.BGR2GRAY);
			Cv2.Threshold(bin, bin, Threshold1, Threshold2, ThresholdTypes.Binary);

			Mat hierarchy1 = new Mat();
			Cv2.FindContours(bin, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

			for (int i = 0; i < contours.Length; i++)
			{
				Cv2.DrawContours(src2, contours, i, Scalar.Blue, 3, LineTypes.AntiAlias);
				for (int j = 0; j < contours[i].Length; j++)
				{
					Cv2.PutText(src2, i.ToString(), contours[i][j], HersheyFonts.Italic, 0.8, Scalar.Black);
				}
			}

			pictureBox1.Image = BitmapConverter.ToBitmap(src2);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			Mat canny = new Mat();
			Cv2.Canny(image, canny, Canny1, Canny2, 3, true);

			pictureBox1.Image = BitmapConverter.ToBitmap(canny);
		}

		private void button4_Click(object sender, EventArgs e)
		{
			thresholdtest();
		}

		private void button5_Click(object sender, EventArgs e)
		{
			Mat src = image;
			Mat src2 = new Mat();
			src.CopyTo(src2);
			Mat canny = new Mat();
			Cv2.Canny(image, canny, Canny1, Canny2);
			Mat bin = new Mat();
			Cv2.CvtColor(src2, bin, ColorConversionCodes.BGR2GRAY);
			//Cv2.Threshold(canny, canny, Threshold1, Threshold2, ThresholdTypes.Binary);

			HierarchyIndex[] hierarchy;
			Cv2.FindContours(canny, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

			for (int i = 0; i < contours.Length; i++)
			{
				Cv2.DrawContours(canny, contours, i, Scalar.Blue, 3, LineTypes.AntiAlias);
				for (int j = 0; j < contours[i].Length; j++)
				{
					Cv2.PutText(canny, i.ToString(), contours[i][j], HersheyFonts.Italic, 0.8, Scalar.Blue);
				}
			}

			pictureBox1.Image = BitmapConverter.ToBitmap(canny);
		}
		private void thresholdtest()
		{
			Mat canny = new Mat();
			Cv2.Canny(image, canny, Canny1, Canny2);
			Mat src = image;
			Mat src2 = new Mat();
			src.CopyTo(src2);
			HierarchyIndex[] hierarchy;
			Mat bin = new Mat();
			Cv2.CvtColor(src, bin, ColorConversionCodes.BGR2GRAY);
			Cv2.Threshold(bin, bin, Threshold1, Threshold2, ThresholdTypes.Binary);

			Mat hierarchy1 = new Mat();
			Cv2.FindContours(bin, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

			for (int i = 0; i < contours.Length; i++)
			{
				Cv2.DrawContours(src2, contours, i, Scalar.Blue, 3, LineTypes.AntiAlias);
				for (int j = 0; j < contours[i].Length; j++)
				{
					Cv2.PutText(src2, i.ToString(), contours[i][j], HersheyFonts.Italic, 0.8, Scalar.Black);
				}
			}

			pictureBox1.Image = BitmapConverter.ToBitmap(bin);
		}
		private void CannyTest()
		{
			Mat OutMat = new Mat();
			Mat thresh = new Mat();
			Cv2.Threshold(image, thresh, Threshold1, Threshold2, ThresholdTypes.Binary);
			Cv2.Canny(thresh, OutMat, Canny1, Canny2);
			//Cv2.ImShow("thresh", thresh);
			pictureBox1.Image = BitmapConverter.ToBitmap(OutMat);
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if (textBox1.Text.Equals("")) return;
			Threshold1 = int.Parse(textBox1.Text);
			trackBar1.Value = Threshold1;
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{
			if (textBox2.Text.Equals("")) return;
			Threshold2 = int.Parse(textBox2.Text);
			trackBar2.Value = Threshold2;
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			Threshold1 = trackBar1.Value;
			textBox1.Text = Threshold1.ToString();
			thresholdtest();
		}

		private void trackBar2_Scroll(object sender, EventArgs e)
		{
			Threshold2 = trackBar2.Value;
			textBox2.Text = Threshold2.ToString();
			thresholdtest();
		}

		private void button6_Click(object sender, EventArgs e)
		{
			if (trackBar1.Value == trackBar1.Maximum) return;
			trackBar1.Value += 1;
		}

		private void button7_Click(object sender, EventArgs e)
		{
			if (trackBar1.Value == trackBar1.Minimum) return;
			trackBar1.Value -= 1;
		}

		private void button9_Click(object sender, EventArgs e)
		{
			if (trackBar2.Value == trackBar2.Maximum) return;
			trackBar2.Value += 1;
		}

		private void button8_Click(object sender, EventArgs e)
		{
			if (trackBar2.Value == trackBar2.Minimum) return;
			trackBar2.Value -= 1;
		}

		private void trackBar3_Scroll(object sender, EventArgs e)
		{
			Canny1 = trackBar3.Value;
			textBox3.Text = Canny1.ToString();
			CannyTest();
		}

		private void trackBar4_Scroll(object sender, EventArgs e)
		{
			Canny2 = trackBar4.Value;
			textBox4.Text = Canny2.ToString();
			CannyTest();
		}

		private void button10_Click(object sender, EventArgs e)
		{
			if (trackBar3.Value == trackBar3.Maximum) return;
			trackBar3.Value += 1;
		}

		private void button11_Click(object sender, EventArgs e)
		{
			if (trackBar3.Value == trackBar3.Minimum) return;
			trackBar3.Value -= 1;
		}

		private void button12_Click(object sender, EventArgs e)
		{
			if (trackBar4.Value == trackBar4.Maximum) return;
			trackBar4.Value += 1;
		}

		private void button13_Click(object sender, EventArgs e)
		{
			if (trackBar4.Value == trackBar4.Minimum) return;
			trackBar4.Value -= 1;
		}

		private void textBox3_TextChanged(object sender, EventArgs e)
		{
			if (textBox3.Text.Equals("")) return;
			Canny1 = int.Parse(textBox3.Text);
			trackBar3.Value = Canny1;
		}

		private void textBox4_TextChanged(object sender, EventArgs e)
		{
			if (textBox4.Text.Equals("")) return;
			Canny2 = int.Parse(textBox4.Text);
			trackBar4.Value = Canny2;
		}

		private void button14_Click(object sender, EventArgs e)
		{
			Mat grayMat = new Mat();
			Cv2.CvtColor(image, grayMat, ColorConversionCodes.BGR2GRAY);
			Mat canny = new Mat();
			Cv2.Canny(grayMat, canny, Canny1, Canny2);
			Mat threshMat = new Mat();
			Cv2.Threshold(canny, threshMat, Threshold1, Threshold2, ThresholdTypes.Binary);

			// 윤곽선 찾기
			OpenCvSharp.Point[][] contours;
			HierarchyIndex[] hierarchy;
			Cv2.FindContours(canny, out contours, out hierarchy, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);
			// 모든 좌표를 갖는 컨투어 그리기, 초록색

			Mat outmat = image.Clone();
			//Cv2.DrawContours(outmat, contours, -1, Scalar.Green, 4);

			// 컨투어 모든 좌표를 작은 점(원)으로 표시
			int count = 0;
			foreach (var pts in contours)
			{
				double length = Cv2.ArcLength(pts, true);
				OpenCvSharp.Point[] pp = Cv2.ApproxPolyDP(pts, 0.01 * length, true);
				RotatedRect rrect = Cv2.MinAreaRect(pp);
				if(pp.Length == 4)
				{
					Cv2.DrawContours(outmat, contours, count, Scalar.Red, 1, LineTypes.AntiAlias, hierarchy, 100);
				}
				//foreach (var pt in pts)
				//{
				//	//Cv2.Circle(outmat, pt, 1, Scalar.Red, -1);
				//}
				count++;
			}

			//Cv2.ImShow("gray", grayMat);
			Cv2.ImShow("canny", canny);
			Cv2.ImShow("out", outmat);
			pictureBox1.Image = BitmapConverter.ToBitmap(threshMat);
		}

		private void button15_Click(object sender, EventArgs e)
		{
			Mat canny = new Mat();
			Cv2.Canny(image, canny, Canny1, Canny2);

			Mat kernelEllipse = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(3, 3));
			Mat dilate = new Mat();
			Cv2.Dilate(canny, dilate, kernelEllipse, new OpenCvSharp.Point(-1, -1), 1, BorderTypes.Reflect);

			Mat[] contours;
			Mat hierarchy = new Mat();
			Cv2.FindContours(dilate.Clone(), out contours, hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

			Mat coins = image.Clone();
			for (int i = 0; i < contours.Length; i++)
			{
				double area = Cv2.ContourArea(contours[i]);
				if (area > 1000)
				{
					Cv2.DrawContours(coins, contours, i, Scalar.Blue, -1);
					Cv2.ImShow(string.Format($"contour[{i}] / {area}"), coins);
				}
			}
			pictureBox1.Image = BitmapConverter.ToBitmap(coins);
		}

		private void button16_Click(object sender, EventArgs e)
		{
			FillRectImg();
		}
		private void FillRectImg()
		{
			Mat canny = new Mat();
			Cv2.Canny(image, canny, Canny1, Canny2);

			Mat kernelEllipse = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(3, 3));
			Mat dilate = new Mat();
			Cv2.Dilate(canny, dilate, kernelEllipse, new OpenCvSharp.Point(-1, -1), 1, BorderTypes.Reflect);
			Mat imsiDilate = new Mat();
			Cv2.Resize(dilate, imsiDilate, new OpenCvSharp.Size(0, 0), 0.5, 0.5);
			Cv2.ImShow("dd", imsiDilate);

			Mat[] contours;
			Mat hierarchy = new Mat();
			Cv2.FindContours(dilate.Clone(), out contours, hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

			Mat FillRect = image.Clone();
			for (int i = 0; i < contours.Length; i++)
			{
				double area = Cv2.ContourArea(contours[i]);
				if (area > square)
				{
					Cv2.DrawContours(FillRect, contours, i, Scalar.Blue, -1);
				}
			}
			pictureBox1.Image = BitmapConverter.ToBitmap(FillRect);
		}
		private void FillRectImg1()
		{
			Mat canny = new Mat();
			Cv2.Canny(image, canny, Canny1, Canny2);

			Mat kernelEllipse = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(3, 3));
			Mat dilate = new Mat();
			Cv2.Dilate(canny, dilate, kernelEllipse, new OpenCvSharp.Point(-1, -1), 1, BorderTypes.Reflect);

			Mat[] contours;
			Mat hierarchy = new Mat();
			Cv2.FindContours(dilate.Clone(), out contours, hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

			Mat FillRect = canny.Clone();
			for (int i = 0; i < contours.Length; i++)
			{
				double area = Cv2.ContourArea(contours[i]);
				if (area > square)
				{
					Cv2.DrawContours(FillRect, contours, i, Scalar.Blue, -1);
				}
			}
			pictureBox1.Image = BitmapConverter.ToBitmap(FillRect);
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

		private void trackBar5_Scroll(object sender, EventArgs e)
		{
			square = trackBar5.Value;
			textBox5.Text = square.ToString();
			FillRectImg();
		}

		private void button18_Click(object sender, EventArgs e)
		{
			if (trackBar5.Value == trackBar5.Maximum) return;
			trackBar5.Value += 1;
		}

		private void button17_Click(object sender, EventArgs e)
		{
			if (trackBar5.Value == trackBar5.Minimum) return;
			trackBar5.Value -= 1;
		}

		private void textBox5_TextChanged(object sender, EventArgs e)
		{
			if (textBox5.Text.Equals("")) return;
			square = int.Parse(textBox5.Text);
			trackBar5.Value = square;
		}

		private void button19_Click(object sender, EventArgs e)
		{
			Mat canny = new Mat();
			Cv2.Canny(image, canny, Canny1, Canny2);

			Mat kernelEllipse = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(3, 3));
			Mat dilate = new Mat();
			Cv2.Dilate(canny, dilate, kernelEllipse, new OpenCvSharp.Point(-1, -1), 1, BorderTypes.Reflect);

			Mat[] contours;
			Mat hierarchy = new Mat();
			Cv2.FindContours(dilate.Clone(), out contours, hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxTC89KCOS);


			// 윤곽선 찾기
			OpenCvSharp.Point[][] contours1;
			HierarchyIndex[] hierarchy1;
			Cv2.FindContours(canny, out contours1, out hierarchy1, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
			// 윤곽선 그리기 및 텍스트 넣기

			Mat FillRect = image.Clone();
			for (int i = 0; i < contours.Length; i++)
			{
				double area = Cv2.ContourArea(contours[i]);

				Console.WriteLine("Area : " + area);
				if (area > square)
				{
					Cv2.DrawContours(FillRect, contours, i, Scalar.Blue, -1);
					Cv2.ImShow(String.Format("Image(DrawCo) : {0}", i), FillRect);
					//Cv2.BoundingRect(contours[i]);
					Mat AreaRect = contours[i];
					RotatedRect rect = Cv2.MinAreaRect(AreaRect);
					Point2f[] Boxpoint = Cv2.BoxPoints(rect);

					Cv2.PutText(FillRect, string.Format("X:{0}, y={1}", Boxpoint[3].X, Boxpoint[3].Y), Boxpoint[3].ToPoint(), HersheyFonts.Italic, 2, Scalar.Black);
					Cv2.ImShow(String.Format("Image : {0}", i), FillRect);

					Console.WriteLine(string.Format("X:{0}, y={1} -> Image : {2}", Boxpoint[0].X, Boxpoint[0].Y, i));
				}
			}
			//for (int i = 0; i < contours1.Length; i++)
			//{
			//	var cnt = contours1[i];
			//	var approx = Cv2.ApproxPolyDP(cnt, 0.009 * Cv2.ArcLength(cnt, true), true);
			//	//var n = approx.Ravel();
			//	//var n = approx.Reshape(1, approx.Total() * 2);
			//	var n = new Mat(approx.Length, 2, MatType.CV_32F);
			//	for (int j = 0; j < approx.Length; j++)
			//	{
			//		int x = approx[j].X;
			//		int y = approx[j].Y;
			//		n.Set<float>(j, 0, x);
			//		n.Set<float>(j, 1, y);
			//		var stringCoord = $"{x}, {y}";
			//		//if (j == 0)
			//		//{
			//		//	// 가장 위쪽 좌표의 경우
			//		//	Cv2.PutText(FillRect, "dddddddd", new OpenCvSharp.Point(x, y), HersheyFonts.Italic, 0.5, Scalar.White, 1);
			//		//}
			//		//else
			//		//{
			//		//	// 나머지 좌표의 경우
			//		//	Cv2.PutText(FillRect, stringCoord, new OpenCvSharp.Point(x, y), HersheyFonts.Italic, 0.5, Scalar.White, 1);
			//		//}
			//		Cv2.PutText(FillRect, stringCoord, new OpenCvSharp.Point(x, y), HersheyFonts.Italic, 0.5, Scalar.White, 1);
			//	}
			//	//Cv2.DrawContours(FillRect, new[] { approx }, 0, Scalar.Red, 5);
			//}
			pictureBox1.Image = BitmapConverter.ToBitmap(FillRect);
		}

		private void button20_Click(object sender, EventArgs e)
		{
			//Mat GrayScale = new Mat();
			//Mat canny = new Mat();
			//Cv2.CvtColor(image, GrayScale, ColorConversionCodes.BGR2GRAY);
			//Cv2.Canny(GrayScale, canny, Canny1, Canny2);

			// 그레이 스케일로 변환
			Mat OriImg = image.Clone();
			Mat OriImg2 = OriImg.Clone();

			Mat imgray = new Mat();
			Cv2.CvtColor(OriImg, imgray, ColorConversionCodes.BGR2GRAY);

			// 스레시홀드로 바이너리 이미지로 만들어서 검은배경에 흰색전경으로 반전
			Mat imthres = new Mat();
			Cv2.Threshold(imgray, imthres, 127, 255, ThresholdTypes.BinaryInv);

			// 가장 바깥쪽 컨투어에 대해 모든 좌표 반환
			OpenCvSharp.Point[][] contour;
			HierarchyIndex[] hierarchy;
			Cv2.FindContours(imthres, out contour, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);

			// 가장 바깥쪽 컨투어에 대해 꼭지점 좌표만 반환
			OpenCvSharp.Point[][] contour2;
			Cv2.FindContours(imthres, out contour2, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

			// 각각의 컨투어 갯수 출력
			Console.WriteLine($"도형의 갯수: {contour.Length}({contour2.Length})");

			// 모든 좌표를 갖는 컨투어 그리기, 초록색
			Cv2.DrawContours(OriImg, contour, -1, new Scalar(0, 255, 0), 4);

			// 꼭지점 좌표만을 갖는 컨투어 그리기, 초록색
			Cv2.DrawContours(OriImg2, contour2, -1, new Scalar(0, 255, 0), 4);

			// 컨투어 모든 좌표를 작은 파랑색 점(원)으로 표시
			foreach (var pts in contour)
			{
				foreach (var pt in pts)
				{
					Cv2.Circle(OriImg, pt, 1, new Scalar(255, 0, 0), -1);
				}
			}

			// 컨투어 꼭지점 좌표를 작은 파랑색 점(원)으로 표시
			foreach (var pts in contour2)
			{
				foreach (var pt in pts)
				{
					Cv2.Circle(OriImg2, pt, 1, new Scalar(255, 0, 0), -1);
				}
			}

			// 결과 출력
			Cv2.ImShow("CHAIN_APPROX_NONE", OriImg);
			Cv2.ImShow("CHAIN_APPROX_SIMPLE", OriImg2);
			Cv2.WaitKey(0);
			Cv2.DestroyAllWindows();
		}

		private void button21_Click(object sender, EventArgs e)
		{
			Mat src = new Mat();
			Mat src2 = image.Clone();
			Mat yellow = new Mat();
			Mat dst = src2.Clone();
			Cv2.Canny(src2, src, Threshold1, Threshold2, 3, true);

			OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;

            Cv2.InRange(src, new Scalar(0, 127, 127), new Scalar(100, 255, 255), yellow);
            Cv2.FindContours(yellow, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

            List<OpenCvSharp.Point[]> new_contours = new List<OpenCvSharp.Point[]>();
            foreach (OpenCvSharp.Point[] p in contours)
            {
                double length = Cv2.ArcLength(p, true);
				int intlength = int.Parse(Math.Round(length).ToString());
                if ( length > 400)
                {
					Console.WriteLine($"{intlength}");
                    new_contours.Add(p);
                }
            }

            Cv2.DrawContours(dst, new_contours, -1, new Scalar(255, 0, 0), 2, LineTypes.AntiAlias, null, 1);
            Cv2.ImShow("dst", yellow);
			pictureBox1.Image = BitmapConverter.ToBitmap(dst);
		}

		private void button22_Click(object sender, EventArgs e)
		{
			Button22click();
		}
		private void Button22click()
		{
			Mat img_result = image.Clone();
			Mat img_gray = new Mat();
			Cv2.CvtColor(image, img_gray, ColorConversionCodes.BGR2GRAY);

			Mat binary_image = new Mat();
			Cv2.Threshold(img_gray, binary_image, Threshold1, Threshold2, ThresholdTypes.BinaryInv);
			Mat imsibinary = new Mat();
			Cv2.Resize(binary_image, imsibinary, new OpenCvSharp.Size(0, 0), 0.3, 0.3);
			Cv2.ImShow("binary_image", imsibinary);

			Mat canny = new Mat();
			Cv2.Canny(binary_image, canny, Canny1, Canny2, 3, true);
			Mat imsicanny = new Mat();
			Cv2.Resize(canny, imsicanny, new OpenCvSharp.Size(0, 0), 0.3, 0.3);
			Cv2.ImShow("canny_image", imsicanny);

			Mat dilate = new Mat();
			Mat kernelEllipse = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(11, 11));
			Cv2.Dilate(canny, dilate, kernelEllipse, new OpenCvSharp.Point(-1, -1), 1, BorderTypes.Reflect);
			Mat imsidilate = new Mat();
			Cv2.Resize(dilate, imsidilate, new OpenCvSharp.Size(0, 0), 0.3, 0.3);
			Cv2.ImShow("canny_dilate", imsidilate);

			// 윤곽선 찾기
			OpenCvSharp.Point[][] contours;
			HierarchyIndex[] hierarchy;
			Cv2.FindContours(dilate, out contours, out hierarchy, retrievalModes, contourApproximationModes);

			List<OpenCvSharp.Point> vertex = new List<OpenCvSharp.Point>();
			for (int i = 0; i < contours.Length; i++)
			{
				double length = Cv2.ArcLength(contours[i], true);
				OpenCvSharp.Point[] pp = Cv2.ApproxPolyDP(contours[i], (double)delta * length, true);
				RotatedRect rrect = Cv2.MinAreaRect(pp);
				if (pp.Length == 4)
				{
					Console.WriteLine($"순서 : {i}, 넓이 : {length}");
					if (length > 3000 || length < 700) continue;
					Cv2.DrawContours(img_result, contours, i, Scalar.Red, -1, LineTypes.AntiAlias, hierarchy, 100);
					//Cv2.PutText(img_result, string.Format("X:{0}, y={1}", contours[i].X, contours[i].Y), Boxpoint[3].ToPoint(), HersheyFonts.Italic, 2, Scalar.Black);
					Console.WriteLine($"성공 넓이 : {length}");
					Mat imsisuccess = new Mat();
					Cv2.Resize(img_result, imsisuccess, new OpenCvSharp.Size(0, 0), 0.3, 0.3);
					//Cv2.ImShow($"성공 순서 : {i}, 길이 : {length}", imsisuccess);
					for (int j = 0; j < pp.Length; j++) vertex.Add(pp[j]);
				}
			}
			for (int i = 0; i < vertex.Count; i++)
			{
				if(i < vertex.Count - 1)
				{
					float PointLength = DistanceToPoint(vertex[i], vertex[i + 1]);
					Cv2.PutText(img_result, $"{PointLength}", LengthWritePoint(vertex[i], vertex[i + 1]), HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 2);
				}
				else
				{
					float PointLength = DistanceToPoint(vertex[i], vertex[0]);
					Cv2.PutText(img_result, $"{PointLength}", LengthWritePoint(vertex[i], vertex[0]), HersheyFonts.HersheyScriptSimplex, 2, Scalar.Lime, 2);
				}
			}
			pictureBox1.Image = BitmapConverter.ToBitmap(img_result);
		}
		public float DistanceToPoint(OpenCvSharp.Point a, OpenCvSharp.Point b)
		{
			return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
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

		private void trackBar6_Scroll(object sender, EventArgs e)
		{
			delta = Math.Round((decimal)trackBar6.Value / 10000, 3);
			trackBar6.Value = (int)(delta * 10000);
			textBox6.Text = delta.ToString();
			Button22click();
		}

		private void button24_Click(object sender, EventArgs e)
		{
			if (trackBar6.Value == trackBar6.Maximum) return;
			delta += 0.001m;
			trackBar6.Value += 10;
			textBox6.Text = delta.ToString();
			Button22click();
		}

		private void button23_Click(object sender, EventArgs e)
		{
			if (trackBar6.Value == trackBar6.Minimum) return;
			delta -= 0.001m;
			trackBar6.Value -= 10;
			textBox6.Text = delta.ToString();
			Button22click();
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if		(comboBox1.SelectedIndex == 0)	retrievalModes = RetrievalModes.Tree;
			else if (comboBox1.SelectedIndex == 1)	retrievalModes = RetrievalModes.External;
			else if (comboBox1.SelectedIndex == 2)	retrievalModes = RetrievalModes.List;
			else									retrievalModes = RetrievalModes.CComp;
			Button22click();
		}

		private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
		{
			if		(comboBox2.SelectedIndex == 0)	contourApproximationModes = ContourApproximationModes.ApproxNone;
			else if (comboBox2.SelectedIndex == 1)	contourApproximationModes = ContourApproximationModes.ApproxSimple;
			else if (comboBox2.SelectedIndex == 2)	contourApproximationModes = ContourApproximationModes.ApproxTC89L1;
			else									contourApproximationModes = ContourApproximationModes.ApproxTC89KCOS;
			Button22click();
		}

		private void button25_Click(object sender, EventArgs e)
		{
			Mat img_result = image.Clone();
			Mat gray = new Mat();
			Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

			Mat res = new Mat();
			Cv2.PreCornerDetect(gray, res, 3);
			Mat res2 = new Mat();
			Cv2.Threshold(res, res2, 0.1, 0, ThresholdTypes.Tozero);

			OpenCvSharp.Point[] corners = FindLocalMaxima(res2);
			Console.WriteLine("corners.Length=" + corners.Length);

			Mat dst = image.Clone();
			foreach (OpenCvSharp.Point p in corners)
			{
				Cv2.Circle(dst, p, 5, Scalar.Red, 2);
			}

			//Cv2.NamedWindow("Original", WindowFlags.AutoSize);
			//Cv2.NamedWindow("CornerTest", WindowFlags.AutoSize);
			//Cv2.ImShow("Original", image);
			Mat dst2 = new Mat();
			Cv2.Resize(dst, dst2, new OpenCvSharp.Size(0, 0), 0.3, 0.3);
			Cv2.ImShow("CornerTest", dst2);
		}
		public static OpenCvSharp.Point[] FindLocalMaxima(Mat src)
		{
			var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(50, 50));
			var dilate = new Mat();
			Cv2.Dilate(src, dilate, kernel);

			var localMax = new Mat();
			Cv2.Compare(src, dilate, localMax, CmpType.EQ);

			var erode = new Mat();
			Cv2.Erode(src, erode, kernel);

			var localMax2 = new Mat();
			Cv2.Compare(src, erode, localMax2, CmpType.GT);

			localMax &= localMax2;

			var pointsList = new List<OpenCvSharp.Point>();
			for (int y = 0; y < localMax.Rows; y++)
			{
				for (int x = 0; x < localMax.Cols; x++)
				{
					if (localMax.At<byte>(y, x) > 0)
					{
						pointsList.Add(new OpenCvSharp.Point(x, y));
					}
				}
			}

			var points = pointsList.ToArray();
			for (int i = 0; i < points.Length; i++)
			{
				int tmp = points[i].X;
				points[i].X = points[i].Y;
				points[i].Y = tmp;
			}

			return points;
		}
	}
}
