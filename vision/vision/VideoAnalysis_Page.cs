using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vision
{
	public partial class VideoAnalysis_Page : XtraUserControl
	{
		Form1 MainForm;
		public VideoAnalysis_Page(Form1 form)
		{
			InitializeComponent();
			this.MainForm = form;
			PanelSettings();
		}
		#region 페널 세팅
		private void PanelSettings()
		{
			ContainerControl cc9 = new ContainerControl() { Dock = DockStyle.Fill };
			panelControl9.Controls.Add(cc9);
			DockManager dockManager9 = new DockManager(cc9);
			dockManager9.AddPanel(DockingStyle.Fill);
			VideoAnalysis_Video1.Parent = dockManager9.Panels[0];
			dockManager9.Panels[0].Text = "영상1";
			VideoAnalysis_Video1.Dock = DockStyle.Fill;
			dockManager9.Panels[0].ClosedPanel += new DockPanelEventHandler(PanelClosed);

			ContainerControl cc10 = new ContainerControl() { Dock = DockStyle.Fill };
			panelControl10.Controls.Add(cc10);
			DockManager dockManager10 = new DockManager(cc10);
			dockManager10.AddPanel(DockingStyle.Fill);
			VideoAnalysis_Video2.Parent = dockManager10.Panels[0];
			dockManager10.Panels[0].Text = "영상2";
			VideoAnalysis_Video2.Dock = DockStyle.Fill;
			dockManager10.Panels[0].ClosedPanel += new DockPanelEventHandler(PanelClosed);
		}
		#endregion
		#region 카메라 화면 닫기 / 다시열기
		private void PanelClosed(object sender, EventArgs e)
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
		private void PanelDoubleClick(object sender, EventArgs e)
		{
			Control control = ((SimpleButton)sender).Parent;
			DockPanel dp = (DockPanel)control.Controls[1];
			control.Controls.Clear();
			dp.Visibility = DockVisibility.Visible;
			dp.Dock = DockingStyle.Fill;
		}
		#endregion
	}
}
