using DevExpress.Schedule;
using DevExpress.XtraWaitForm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WireExternalInspection
{
	public class LogRecordProcessing
	{
		MainForm mainform;
		public string LogFolderPath = @"WorkLog\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM");
		string LogFilePath = "";
		string LogContentHead = "NoName";
		public LogRecordProcessing(MainForm form)
		{
			LogFolderSet();
			LogWrite("프로그램 시작");
			mainform = form;
		}
		public void LogFolderSet()
		{
			LogFilePath = LogFolderPath + @"\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
		}
		/// <summary>
		/// 로그 쓰기
		/// </summary>
		/// <param name="logmessage">로그 내용</param>
		public void LogWrite(string logmessage)
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
				//mainform.ShowMessage("오류", "로그 작성 중 오류가 발생하였습니다!!\n" + ex.Message, "주의");
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
			catch (Exception ex)
			{
				//mainform.ShowMessage("오류", "로그 폴더 변경 중 오류가 발생하였습니다!!\n" + ex.Message, "주의");
			}
		}
		public void LogFileChange(string NewFilePath)
		{
			try
			{
				//LogFilePath = LogFolderPath + @"\" + NewFilePath + " " + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
				LogContentHeadChange(NewFilePath);
			}
			catch (Exception ex)
			{
				//mainform.ShowMessage("오류", "로그 파일 생성 중 오류가 발생하였습니다!!\n" + ex.Message, "주의");
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
				//mainform.ShowMessage("오류", "로그 헤더 변경 중 오류가 발생하였습니다!!\n" + ex.Message, "주의");
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
		public string CameraErrorCode(string ErrorMessage)
		{
			LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			string sOutMessage = "";
			try
			{
				string sCheckMessage = ErrorMessage.Substring(0, 6).Trim();
				switch (sCheckMessage)
				{
					case "-1001" : sOutMessage = "에러명 : ERR_ERROR / 에러 번호 : '-1001'\n지정되지 않은 런타임 오류입니다!";																																																																	break;
					case "-1002" : sOutMessage = "에러명 : ERR_NOT_INITIALIZED / 에러 번호 : '-1002'\n모듈 또는 리소스가 초기화되지 않았습니다!";																																																													break;
					case "-1003" : sOutMessage = "에러명 : ERR_NOT_IMPLEMENTED / 에러 번호 : '-1003'\n요청된 작업이 구현되지 않았습니다!";																																																														break;
					case "-1004" : sOutMessage = "에러명 : ERR_RESOURCE_IN_USE / 에러 번호 : '-1004'\n요청된 리소스가 이미 사용 중입니다!";																																																														break;
					case "-1005" : sOutMessage = "에러명 : ERR_ACCESS_DENIED / 에러 번호 : '-1005'\n요청된 작업은 허용되지 않습니다!";																																																															break;
					case "-1006" : sOutMessage = "에러명 : ERR_INVALID_HANDLE / 에러 번호 : '-1006'\n지정된 핸들은 잘못된 핸들 또는 NULL 포인터에 대한 함수 호출과 같은 작업을 지원하지 않습니다!";																																																break;
					case "-1007" : sOutMessage = "에러명 : ERR_INVALID_ID / 에러 번호 : '-1007'\nID를 리소스에 연결할 수 없습니다! 예를 들어 지정된 ID의 디바이스를 현재 사용할 수 없습니다!";																																																	break;
					case "-1008" : sOutMessage = "에러명 : ERR_NO_DATA / 에러 번호 : '-1008'\n이 함수에는 작업할 데이터가 없습니다!";																																																																break;
					case "-1009" : sOutMessage = "에러명 : ERR_INVALID_PARAMETER / 에러 번호 : '-1009'\n지정된 매개 변수 중 하나가 유효하지 않거나 범위를 벗어났습니다!";																																																							break;
					case "-1010" : sOutMessage = "에러명 : ERR_IO / 에러 번호 : '-1010'\n통신 오류(예: 원격 장치에 대한 읽기 또는 쓰기 작업 실패)가 발생했습니다!";																																																								break;
					case "-1011" : sOutMessage = "에러명 : ERR_TIMEOUT / 에러 번호 : '-1011'\n작업 시간이 만료되어 완료할 수 없습니다!";																																																															break;
					case "-1012" : sOutMessage = "에러명 : ERR_ABORT / 에러 번호 : '-1012'\n작업이 완료되기 전에 중단되었습니다!";																																																																break;
					case "-1013" : sOutMessage = "에러명 : ERR_INVALID_BUFFER / 에러 번호 : '-1013'\n사용자가 현재 활성 수집 모드에서 수집을 시작하기에 충분한 버퍼를 제공하지 않았습니다!";																																																		break;
					case "-1014" : sOutMessage = "에러명 : ERR_NOT_AVAILABLE / 에러 번호 : '-1014'\n현재 상태에서는 특정 시간에 리소스 또는 정보를 사용할 수 없습니다!";																																																							break;
					case "-1015" : sOutMessage = "에러명 : ERR_INVALID_ADDRESS / 에러 번호 : '-1015'\n지정된 주소가 범위를 벗어났거나 내부적인 이유로 유효하지 않습니다!";																																																						break;
					case "-1016" : sOutMessage = "에러명 : ERR_BUFFER_TOO_SMALL / 에러 번호 : '-1016'\n제공된 버퍼가 너무 작아서 예상되는 양의 데이터를 수신할 수 없습니다!\n이는 버퍼가 예상 페이로드 크기보다 작을 경우 데이터 스트림 모듈의 획득 버퍼에 영향을 줄 수 있고, 정보 또는 ID를 검색하기 위해 GenTL Producer 인터페이스의 다른 기능으로 전달되는 버퍼도 영향을 줄 수 있습니다!";		break;
					case "-1017" : sOutMessage = "에러명 : ERR_INVALID_INDEX / 에러 번호 : '-1017'\n내부 개체를 참조하는 제공된 인덱스가 경계를 벗어났습니다!";																																																									break;
					case "-1018" : sOutMessage = "에러명 : ERR_PARSING_CHUNK_DATA / 에러 번호 : '-1018'\n청크 데이터를 포함하는 버퍼를 구문 분석하는 동안 오류가 발생했습니다!";																																																					break;
					case "-1019" : sOutMessage = "에러명 : ERR_INVALID_VALUE / 에러 번호 : '-1019'\n레지스터 쓰기 기능이 잘못된 값을 쓰려고 했습니다!";																																																											break;
					case "-1020" : sOutMessage = "에러명 : ERR_RESOURCE_EXHAUSTED / 에러 번호 : '-1020'\n요청된 리소스가 모두 사용 중입니다!";																																																													break;
					case "-1021" : sOutMessage = "에러명 : ERR_OUT_OF_MEMORY / 에러 번호 : '-1021'\n시스템 및/또는 시스템의 다른 하드웨어(프레임 그래버)에 메모리가 부족합니다!";																																																					break;
					case "-1022" : sOutMessage = "에러명 : ERR_BUSY / 에러 번호 : '-1022'\n담당 모듈/디바이스가 요청된 작업과 동시에 수행할 수 없는 작업을 수행 중이므로 필요한 작업을 실행할 수 없습니다";																																														break;
					case "-10000": sOutMessage = "에러명 : ERR_CUSTOM_ID / 에러 번호 : '-10001'\n일반 런타임 오류!";																																																																				break;
					default		 : sOutMessage = "원인을 알 수 없는 에러!";																																																																														break;
				}
			}
			catch(Exception ex)
			{
				mainform.ShowMessage("오류", "에러를 확인할 수 없습니다!\n", "경고");
				LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
				sOutMessage = "";
			}
			return sOutMessage;
		}
	}
}
