using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

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
		public Form1()
		{
			InitializeComponent();
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
			Cv2.Canny(image, canny, Canny1, Canny2);

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
			Cv2.Canny(image, OutMat, Canny1, Canny2);
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
			Mat canny = new Mat();
			Cv2.Canny(image, canny, Canny1, Canny2);

			HierarchyIndex[] hierarchy;
			Cv2.FindContours(canny, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

			for (int i = 0; i < contours.Length; i++)
			{
				Cv2.DrawContours(canny, contours, i, Scalar.Blue, 3, LineTypes.AntiAlias);
				//for (int j = 0; j < contours[i].Length; j++)
				//{
				//	Cv2.PutText(canny, i.ToString(), contours[i][j], HersheyFonts.Italic, 0.8, Scalar.Black);
				//}


				double peri = Cv2.ArcLength(contours[i], true);

				OpenCvSharp.Point[] pp = Cv2.ApproxPolyDP(contours[i], 0.02 * peri, true);

				//RotatedRect rrect = Cv2.MinAreaRect(pp);
				//double areaRatio = Math.Abs(Cv2.ContourArea(contours[i], false)) / (rrect.Size.Width * rrect.Size.Height);

				if (pp.Length == 4)
				{
					//for (int j = 0; j < pp.Length; j++)
					//{
					//    Cv2.Line(ms2, pp[j], pp[(j + 1) % pp.Length], Scalar.Green, 1, LineTypes.AntiAlias, 0);
					//}
					Cv2.DrawContours(canny, contours, i, Scalar.Red, -1, LineTypes.AntiAlias, hierarchy, 100);
					//Cv2.FillPoly(canny, contours, Scalar.Red, LineTypes.AntiAlias);
					
				}
				else
				{

					Cv2.DrawContours(canny, contours, i, Scalar.Yellow, -1, LineTypes.AntiAlias, hierarchy, 100);
				}
			}

			Cv2.CvtColor(canny, canny, ColorConversionCodes.GRAY2BGR);
			pictureBox1.Image = BitmapConverter.ToBitmap(canny);
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
			Cv2.FindContours(dilate.Clone(), out contours, hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

			
			// 윤곽선 찾기
			OpenCvSharp.Point[][] contours1;
			HierarchyIndex[] hierarchy1;
			Cv2.FindContours(canny, out contours1, out hierarchy1, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
			// 윤곽선 그리기 및 텍스트 넣기
			for (int i = 0; i < contours1.Length; i++)
			{
				var cnt = contours1[i];
				var approx = Cv2.ApproxPolyDP(cnt, 0.009 * Cv2.ArcLength(cnt, true), true);
				//var n = approx.Ravel();
				//var n = approx.Reshape(1, approx.Total() * 2);
				var n = new Mat(approx.Length, 2, MatType.CV_32F);
				for (int j = 0; j < approx.Length; j++)
				{
					int x = approx[j].X;
					int y = approx[j].Y;
					n.Set<float>(j, 0, x);
					n.Set<float>(j, 1, y);
					var stringCoord = $"{x}, {y}";
					if (j == 0)
					{
						// 가장 위쪽 좌표의 경우
						Cv2.PutText(canny, stringCoord, new OpenCvSharp.Point(x, y), HersheyFonts.Italic, 1, Scalar.Black, 1);
					}
					else
					{
						// 나머지 좌표의 경우
						Cv2.PutText(canny, stringCoord, new OpenCvSharp.Point(x, y), HersheyFonts.Italic, 1, Scalar.Black, 1);
					}
				}
				//Cv2.DrawContours(FillRect, new[] { approx }, 0, Scalar.Red, 5);
			}
			Mat FillRect = canny.Clone();
			for (int i = 0; i < contours.Length; i++)
			{
				double area = Cv2.ContourArea(contours[i]);
				if (area > square)
				{
					Cv2.DrawContours(FillRect, contours, i, Scalar.Blue, -1);
					//Cv2.PutText(FillRect, i.ToString(), contours[i], HersheyFonts.Italic, 2, Scalar.Black);
				}
			}
			pictureBox1.Image = BitmapConverter.ToBitmap(FillRect);
		}
	}
}
