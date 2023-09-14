using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WireExternalInspection
{
	internal static class Program
	{
		/// <summary>
		/// 해당 애플리케이션의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main()
		{
			if (IsExistProcess(Process.GetCurrentProcess().ProcessName))
			{
				MessageBox.Show("프로세스가 이미 실행중입니다!", "프로세스 확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new LoginForm());
			}
		}
		static bool IsExistProcess(string processName)
		{
			int cnt = 0;
			foreach (Process p in Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName))
			{
				cnt++;
				if (cnt > 1) return true;
			}
			return false;
		}
	}
}
