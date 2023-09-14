namespace WireExternalInspection
{
	partial class MainForm
	{
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 디자이너에서 생성한 코드

		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다. 
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
		/// </summary>
		private void InitializeComponent()
		{
			this.ribbonPage2 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.navigationFrame1 = new DevExpress.XtraBars.Navigation.NavigationFrame();
			this.ViewPage = new DevExpress.XtraBars.Navigation.NavigationPage();
			this.AnalysisPage = new DevExpress.XtraBars.Navigation.NavigationPage();
			this.SettingsPage = new DevExpress.XtraBars.Navigation.NavigationPage();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.navigationFrame1)).BeginInit();
			this.navigationFrame1.SuspendLayout();
			this.SuspendLayout();
			// 
			// ribbonPage2
			// 
			this.ribbonPage2.Name = "ribbonPage2";
			this.ribbonPage2.Text = "ribbonPage2";
			// 
			// ribbonPage1
			// 
			this.ribbonPage1.Name = "ribbonPage1";
			this.ribbonPage1.Text = "ribbonPage1";
			// 
			// ribbonControl1
			// 
			this.ribbonControl1.ApplicationCaption = " ";
			this.ribbonControl1.ApplicationDocumentCaption = "와이어 비전 검사 프로그램";
			this.ribbonControl1.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(30, 35, 30, 35);
			this.ribbonControl1.ExpandCollapseItem.Id = 0;
			this.ribbonControl1.Font = new System.Drawing.Font("맑은 고딕", 12F);
			this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem});
			this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
			this.ribbonControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.ribbonControl1.MaxItemId = 1;
			this.ribbonControl1.Name = "ribbonControl1";
			this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
			this.ribbonControl1.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
			this.ribbonControl1.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
			this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
			this.ribbonControl1.ShowMoreCommandsButton = DevExpress.Utils.DefaultBoolean.False;
			this.ribbonControl1.ShowPageHeadersInFormCaption = DevExpress.Utils.DefaultBoolean.False;
			this.ribbonControl1.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
			this.ribbonControl1.ShowQatLocationSelector = false;
			this.ribbonControl1.ShowToolbarCustomizeItem = false;
			this.ribbonControl1.Size = new System.Drawing.Size(1798, 37);
			this.ribbonControl1.Toolbar.ShowCustomizeItem = false;
			this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
			this.ribbonControl1.TransparentEditorsMode = DevExpress.Utils.DefaultBoolean.False;
			// 
			// navigationFrame1
			// 
			this.navigationFrame1.Controls.Add(this.ViewPage);
			this.navigationFrame1.Controls.Add(this.AnalysisPage);
			this.navigationFrame1.Controls.Add(this.SettingsPage);
			this.navigationFrame1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.navigationFrame1.Location = new System.Drawing.Point(0, 37);
			this.navigationFrame1.Name = "navigationFrame1";
			this.navigationFrame1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] {
            this.ViewPage,
            this.AnalysisPage,
            this.SettingsPage});
			this.navigationFrame1.SelectedPage = this.ViewPage;
			this.navigationFrame1.Size = new System.Drawing.Size(1798, 962);
			this.navigationFrame1.TabIndex = 21;
			this.navigationFrame1.Text = "navigationFrame1";
			this.navigationFrame1.SelectedPageChanged += new DevExpress.XtraBars.Navigation.SelectedPageChangedEventHandler(this.navigationFrame1_SelectedPageChanged);
			// 
			// ViewPage
			// 
			this.ViewPage.Name = "ViewPage";
			this.ViewPage.Size = new System.Drawing.Size(1798, 962);
			// 
			// AnalysisPage
			// 
			this.AnalysisPage.Name = "AnalysisPage";
			this.AnalysisPage.Size = new System.Drawing.Size(1798, 962);
			// 
			// SettingsPage
			// 
			this.SettingsPage.Name = "SettingsPage";
			this.SettingsPage.Size = new System.Drawing.Size(1798, 962);
			// 
			// MainForm
			// 
			this.ActiveGlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(115)))), ((int)(((byte)(70)))));
			this.Appearance.BackColor = System.Drawing.Color.White;
			this.Appearance.Options.UseBackColor = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1798, 999);
			this.Controls.Add(this.navigationFrame1);
			this.Controls.Add(this.ribbonControl1);
			this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Glow;
			this.IconOptions.Image = global::WireExternalInspection.Properties.Resources.Lens;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "MainForm";
			this.Ribbon = this.ribbonControl1;
			this.RibbonVisibility = DevExpress.XtraBars.Ribbon.RibbonVisibility.Hidden;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.navigationFrame1)).EndInit();
			this.navigationFrame1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage2;
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
		private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
		private DevExpress.XtraBars.Navigation.NavigationPage SettingsPage;
		public DevExpress.XtraBars.Navigation.NavigationFrame navigationFrame1;
		public DevExpress.XtraBars.Navigation.NavigationPage ViewPage;
		public DevExpress.XtraBars.Navigation.NavigationPage AnalysisPage;
	}
}

