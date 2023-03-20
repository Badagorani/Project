using DevExpress.XtraLayout.Customization.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vision
{
	public class LogRecord
	{
		Form1 MainForm;
		string LogFolder;
		public LogRecord(Form1 form)
		{
			this.MainForm = form;
			LogFolder = MainForm.Settings.WorkFileSetting.WorkFilePath;
			DirectoryInfo di = new DirectoryInfo(LogFolder);
			if (di.Exists == false) di.Create();
			LogWrite("프로그램 시작");
		}
		public void LogWrite(string logcontent)
		{
			if(MainForm.Settings.WorkFileSetting.LogSave)
			{
				try
				{
					string LogFilePath = LogFolder + @"\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
					//DirectoryInfo di = new DirectoryInfo(LogFilePath);
					//if (di.Exists == false) di.Create();
					FileInfo fi = new FileInfo(LogFilePath);
					string writeLog = "[ " + DateTime.Now.ToString("yyyy-MM-dd dddd HH:mm:ss fff") + " ] " + logcontent;
					if (!fi.Exists)
					{
						using (StreamWriter sw = new StreamWriter(LogFilePath))
						{
							sw.WriteLine(writeLog);
							sw.Close();
						}
					}
					else
					{
						using (StreamWriter sw = File.AppendText(LogFilePath))
						{
							sw.WriteLine(writeLog);
							sw.Close();
						}
					}
				}
				catch (Exception ex)
				{
					MainForm.ShowMessage("오류", "로그 작성 중 오류가 발생하였습니다!!\n" + ex.Message, "주의");
				}
			}
		}
	}
}
