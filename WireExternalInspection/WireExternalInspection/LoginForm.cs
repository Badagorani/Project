using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using WireExternalInspection.Properties;

namespace WireExternalInspection
{
	public partial class LoginForm : DevExpress.XtraBars.Ribbon.RibbonForm
	{
		MainForm mainform;
		bool bUserCheck = false;
		bool bPlaceCheck = false;
		public LoginForm()
		{
			InitializeComponent();
			mainform = new MainForm();
		}
		private void LoginForm_Load(object sender, EventArgs e)
		{
			mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
			}
			catch(Exception ex)
			{
				mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		private void LoginForm_Shown(object sender, EventArgs e)
		{
			mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				txt_Inspection_User.Focus();
			}
			catch (Exception ex)
			{
				mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		private void InputCheck(object sender, EventArgs e)
		{
			mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			TextEdit tInput = (TextEdit)sender;
			string sInputName = tInput.Name;
			string sInputText = tInput.Text;
			try
			{
				if (sInputName.Equals("txt_Inspection_User"))
				{
					if (sInputText.Equals(""))
					{
						labelControl1.ImageOptions.SvgImage = Resources.actions_deletecircled;
						txt_Inspection_User.Properties.Appearance.BorderColor = Color.Red;
						bUserCheck = false;
					}
					else
					{
						labelControl1.ImageOptions.SvgImage = Resources.actions_checkcircled;
						txt_Inspection_User.Properties.Appearance.BorderColor = Color.Lime;
						bUserCheck = true;
					}
				}
				else if (sInputName.Equals("txt_Inspection_Place"))
				{
					if (sInputText.Equals(""))
					{
						labelControl2.ImageOptions.SvgImage = Resources.actions_deletecircled;
						txt_Inspection_Place.Properties.Appearance.BorderColor = Color.Red;
						bPlaceCheck = false;
					}
					else
					{
						labelControl2.ImageOptions.SvgImage = Resources.actions_checkcircled;
						txt_Inspection_Place.Properties.Appearance.BorderColor = Color.Lime;
						bPlaceCheck = true;
					}
				}
				if (bUserCheck && bPlaceCheck)
				{
					btn_Start.Appearance.BackColor = Color.FromArgb(33, 115, 70);
					btn_Start.Enabled = true;
				}
				else
				{
					btn_Start.Appearance.BackColor = Color.FromArgb(200, 200, 200);
					btn_Start.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		private void InputEnterEvent(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter && btn_Start.Enabled) Start_Click(sender, e);
		}
		private void WorkSet()
		{
			mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				mainform.Xml.WorkFileSetting.WorkUser = txt_Inspection_User.Text;
				mainform.Xml.WorkFileSetting.WorkTarget = txt_Inspection_Place.Text;
				mainform.Xml.WorkFileSetting.WorkDay = dat_Inspection_Date.Text;
				mainform.Xml.XMLSave(mainform.Xml.WorkFileSetting, mainform.Xml.WorkFileSetting.GetType(), @"\WorkFileSetting.xml");
				mainform.Log.LogContentHeadChange(txt_Inspection_User.Text + " - " + txt_Inspection_Place.Text);
			}
			catch (Exception ex)
			{
				mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
		}
		private void Start_Click(object sender, EventArgs e)
		{
			mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} ");
			try
			{
				for (float i = 0; i < 50; i++)
				{
					this.Opacity -= 0.02;
					Thread.Sleep(1);
				}
				this.Hide();
				WorkSet();
				mainform.NowPageNo = 1;
				mainform.Log.LogWrite($"검사자 : " + txt_Inspection_User.Text + " & 검사장소 : " + txt_Inspection_Place.Text + " ==> 검사 시작");
				mainform.ShowDialog();
			}
			catch (Exception ex)
			{
				mainform.Log.LogWrite($"{this.GetType().Name} -> {MethodBase.GetCurrentMethod().Name} " + ex.Message);
			}
			this.Close();
		}
	}
}
