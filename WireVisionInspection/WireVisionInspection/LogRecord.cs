using DevExpress.Schedule;
using DevExpress.XtraLayout.Customization.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireVisionInspection
{
	public class LogRecord
	{
		Form1 MainForm;
		//string LogFolder;
		//string NowYearFolder;
		//string NowMonthFolder;
		public string LogFolderPath = @"WorkLog\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM");
		string LogFilePath = "";
		string LogContentHead = "default";
		public LogRecord(Form1 form)
		{
			this.MainForm = form;
			LogFolderSet();
			LogWrite("프로그램 시작");
		}
		public void LogFolderSet()
		{
			LogFilePath = LogFolderPath + @"\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
			MainForm.Settings.lb_WorkFolder_WorkFileSetting.Text = LogFolderPath;
			//LogFolder = MainForm.Settings.WorkFileSetting.WorkFilePath;
			//NowYearFolder = LogFolder + @"\" + DateTime.Now.ToString("yyyy");
			//NowMonthFolder = NowYearFolder + @"\" + DateTime.Now.ToString("MM");

			//DirectoryInfo di = new DirectoryInfo(LogFolder);
			//if (di.Exists == false) di.Create();
			//di = new DirectoryInfo(NowYearFolder);
			//if (di.Exists == false) di.Create();
			//di = new DirectoryInfo(NowMonthFolder);
			//if (di.Exists == false) di.Create();
		}
		public void LogWrite(string logmessage)
		{
			if (MainForm.Settings.WorkFileSetting.LogSave)
			{
				try
				{
					//MonthCheck();
					FileInfo fi = new FileInfo(LogFilePath);
					string writeLog = "[ " + LogContentHead + DateTime.Now.ToString(" yyyy-MM-dd dddd HH:mm:ss fff") + " ] " + logmessage;
					if (!fi.Exists)
					{
						DirectoryInfo di = new DirectoryInfo(LogFolderPath);
						if (di.Exists == false) di.Create();
						using (StreamWriter sw = new StreamWriter(LogFilePath))
						{
							sw.WriteLine("[ " + DateTime.Now.ToString(" yyyy-MM-dd dddd HH:mm:ss fff") + " ] " + "파일 생성 성공");
							sw.Close();
						}
					}
					using (StreamWriter sw = File.AppendText(LogFilePath))
					{
						sw.WriteLine(writeLog);
						sw.Close();
					}
				}
				catch (Exception ex)
				{
					MainForm.ShowMessage("오류", "로그 작성 중 오류가 발생하였습니다!!\n" + ex.Message, "주의");
				}
			}
		}
		public void LogFolderChange(string NewFolderPath)
		{
			try
			{
				LogFolderPath = @"WorkLog\" + NewFolderPath;
				DirectoryInfo di = new DirectoryInfo(LogFolderPath);
				if (di.Exists == false) di.Create();
			}
			catch(Exception ex)
			{
				MainForm.ShowMessage("오류", "로그 폴더 변경 중 오류가 발생하였습니다!!\n" + ex.Message, "주의");
			}
		}
		public void LogFileChange(string NewFilePath)
		{
			try
			{
				LogFilePath = LogFolderPath + @"\" + NewFilePath + " " + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
				LogContentHeadChange(NewFilePath);
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "로그 파일 생성 중 오류가 발생하였습니다!!\n" + ex.Message, "주의");
			}
		}
		public void LogContentHeadChange(string NewContent)
		{
			try
			{
				LogContentHead = NewContent;
			}
			catch (Exception ex)
			{
				MainForm.ShowMessage("오류", "로그 헤더 변경 중 오류가 발생하였습니다!!\n" + ex.Message, "주의");
			}
		}
		private void MonthCheck()
		{
			//if (NowMonthFolder.Substring((NowYearFolder + @"\").Length).Equals(DateTime.Now.ToString("MM"))) return;
			//else
			//{
			//	NowMonthFolder = NowYearFolder + @"\" + DateTime.Now.ToString("MM");
			//	DirectoryInfo di = new DirectoryInfo(NowMonthFolder);
			//	if (di.Exists == false) di.Create();
			//}
		}
	}
}
