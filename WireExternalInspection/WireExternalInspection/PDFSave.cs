using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraWaitForm;
using System.IO;
using System.Reflection;
using DevExpress.XtraEditors;
using System.Drawing.Imaging;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports;

namespace WireExternalInspection
{
    public partial class PDFSave : DevExpress.XtraBars.Ribbon.RibbonForm
    {
		public MainForm mainform;
		public LogRecordProcessing Log;
		public PDFSave(MainForm form, System.Drawing.Image img1, System.Drawing.Image img2)
		{
			InitializeComponent();
			mainform = form;
			Log = mainform.Log;
			pb_Image1.Image = img1;
			pb_Image2.Image = img2;
		}
		private void PDFSave_Shown(object sender, EventArgs e)
		{
			rtb_AnalysisContent.Focus();
			rtb_AnalysisContent.SelectAll();
		}
		private void Report_Export()
		{
			Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				#region iTextSharp 사용 PDF 저장
				string pdfSaveFolderPath = mainform.Xml.ProgramSetting.SaveFilePath + @"\Report\";
				DirectoryInfo di = new DirectoryInfo(pdfSaveFolderPath);
				string nowtime = DateTime.Now.ToString("yyyyMMddHHmmss");
				if (di.Exists == false) di.Create();
				FileStream fs = new FileStream(pdfSaveFolderPath + "외선분석결과_" + nowtime + ".pdf",
					FileMode.Create, FileAccess.Write, FileShare.None);
				iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50);
				PdfWriter writer = PdfWriter.GetInstance(document, fs);
				document.Open();

				System.Drawing.Image SourceImage1 = pb_Image1.Image;
				Size resize1 = new Size(500, 340);
				Bitmap resizeImage1 = new Bitmap(SourceImage1, resize1);
				iTextSharp.text.Image iTextImage1 = iTextSharp.text.Image.GetInstance(resizeImage1, BaseColor.BLACK);
				document.Add(iTextImage1);
				document.Add(new Paragraph("\n\n"));

				System.Drawing.Image SourceImage12 = pb_Image2.Image;
				Size resize2 = new Size(500, 340);
				Bitmap resizeImage2 = new Bitmap(SourceImage12, resize2);
				iTextSharp.text.Image iTextImage2 = iTextSharp.text.Image.GetInstance(resizeImage2, BaseColor.BLACK);
				document.Add(iTextImage2);
				document.Add(new Paragraph("\n\n"));

				//BaseFont objBaseFont = BaseFont.CreateFont("font/MALGUN.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
				//iTextSharp.text.Font objFont = new iTextSharp.text.Font(objBaseFont, 12);
				document.Add(new Paragraph(rtb_AnalysisContent.Text, SetCustomFont()));
				document.Close();
				mainform.ShowMessage("저장 중...", pdfSaveFolderPath + "\n저장되었습니다!", "알림");
				#endregion
				#region XtraReport 사용 PDF 저장
				//XtraReport1 report = new XtraReport1();
				//PdfExportOptions pdfOptions = report.ExportOptions.Pdf;

				//// Specify the pages to export.
				//pdfOptions.PageRange = "1, 3-5";

				//// Specify the quality of exported images.
				//pdfOptions.ConvertImagesToJpeg = false;
				//pdfOptions.ImageQuality = PdfJpegImageQuality.Medium;

				//// Specify the PDF/A-compatibility.
				//pdfOptions.PdfACompatibility = PdfACompatibility.PdfA3b;

				//// The following options are not compatible with PDF/A.
				//// The use of these options will result in PDF validation errors.
				////pdfOptions.NeverEmbeddedFonts = "Tahoma;Courier New";
				////pdfOptions.ShowPrintDialogOnOpen = true;

				//// (Optional) You can specify the security and signature options. 
				////pdfOptions.PasswordSecurityOptions
				////pdfOptions.SignatureOptions

				//// (Optional) You can add metadata and attachments,
				//// to produce a ZUGFeRD-compatible PDF.
				////pdfOptions.AdditionalMetadata
				////pdfOptions.Attachments

				//// Specify the document options.
				//pdfOptions.DocumentOptions.Application = "Test Application";
				//pdfOptions.DocumentOptions.Author = "DX Documentation Team";
				//pdfOptions.DocumentOptions.Keywords = "DevExpress, Reporting, PDF";
				//pdfOptions.DocumentOptions.Producer = Environment.UserName.ToString();
				//pdfOptions.DocumentOptions.Subject = "Document Subject";
				//pdfOptions.DocumentOptions.Title = "Document Title";

				//// Check the validity of PDF export options 
				//// and return a list of any detected inconsistencies.
				//IList<string> result = pdfOptions.Validate();
				//if (result.Count > 0)
				//	Console.WriteLine(String.Join(Environment.NewLine, result));
				//else
				//{
				//	report.CreateDocument();
				//	if (ExportOptionsTool.EditExportOptions(pdfOptions, report.PrintingSystem)
				//		== DialogResult.OK)
				//	{
				//		report.ExportToPdf("result.pdf", pdfOptions);
				//		// ...
				//	}
				//}
				#endregion
			}
			catch (Exception ex)
			{
				Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			}
		}
		private iTextSharp.text.Font SetCustomFont()
		{
			iTextSharp.text.Font returnFont;
			string BatangFont = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\batang.ttc";
			string MalgunGothicFont = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\malgun.ttf";
			FontFactory.Register(BatangFont);
			FontFactory.Register(MalgunGothicFont);
			//returnFont = FontFactory.GetFont("바탕체", BaseFont.IDENTITY_H, 10);
			returnFont = FontFactory.GetFont("맑은 고딕", BaseFont.IDENTITY_H, 12);
			return returnFont;
		}
		private void PDF_Save_Button_Action(object sender, EventArgs e)
		{
			string ButtonName = ((SimpleButton)sender).Name.Substring("btn_".Length);
			switch (ButtonName)
			{
				case "Save"		: Report_Export();	break;
				case "Cancel"	:					break;
			}
			this.Close();
		}
	}
}
