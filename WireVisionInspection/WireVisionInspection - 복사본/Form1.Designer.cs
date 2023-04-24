namespace WireVisionInspection
{
	partial class Form1
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.navigationFrame1 = new DevExpress.XtraBars.Navigation.NavigationFrame();
			this.ButtonPage = new DevExpress.XtraBars.Navigation.NavigationPage();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.btn_Settings = new DevExpress.XtraEditors.SimpleButton();
			this.btn_RealTimeView = new DevExpress.XtraEditors.SimpleButton();
			this.btn_VideoAnalysis = new DevExpress.XtraEditors.SimpleButton();
			this.btn_VideoCheck = new DevExpress.XtraEditors.SimpleButton();
			this.navigationFrame2 = new DevExpress.XtraBars.Navigation.NavigationFrame();
			this.RealTimeView_Page = new DevExpress.XtraBars.Navigation.NavigationPage();
			this.VideoCheck_Page = new DevExpress.XtraBars.Navigation.NavigationPage();
			this.VideoAnalysis_Page = new DevExpress.XtraBars.Navigation.NavigationPage();
			this.Settings_Page = new DevExpress.XtraBars.Navigation.NavigationPage();
			this.fluentDesignFormContainer1 = new DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormContainer();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.navigationFrame1)).BeginInit();
			this.navigationFrame1.SuspendLayout();
			this.ButtonPage.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.navigationFrame2)).BeginInit();
			this.navigationFrame2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
			this.SuspendLayout();
			// 
			// navigationFrame1
			// 
			this.navigationFrame1.Controls.Add(this.ButtonPage);
			this.navigationFrame1.Dock = System.Windows.Forms.DockStyle.Top;
			this.navigationFrame1.Location = new System.Drawing.Point(0, 0);
			this.navigationFrame1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.navigationFrame1.Name = "navigationFrame1";
			this.navigationFrame1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] {
            this.ButtonPage});
			this.navigationFrame1.SelectedPage = this.ButtonPage;
			this.navigationFrame1.Size = new System.Drawing.Size(1598, 70);
			this.navigationFrame1.TabIndex = 0;
			this.navigationFrame1.Text = "navigationFrame1";
			// 
			// ButtonPage
			// 
			this.ButtonPage.Appearance.BackColor = System.Drawing.Color.White;
			this.ButtonPage.Appearance.Options.UseBackColor = true;
			this.ButtonPage.Caption = "ButtonPage";
			this.ButtonPage.Controls.Add(this.tableLayoutPanel3);
			this.ButtonPage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.ButtonPage.Name = "ButtonPage";
			this.ButtonPage.Size = new System.Drawing.Size(1598, 70);
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.ColumnCount = 4;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel3.Controls.Add(this.btn_Settings, 3, 0);
			this.tableLayoutPanel3.Controls.Add(this.btn_RealTimeView, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this.btn_VideoAnalysis, 2, 0);
			this.tableLayoutPanel3.Controls.Add(this.btn_VideoCheck, 1, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 1;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(1598, 70);
			this.tableLayoutPanel3.TabIndex = 7;
			// 
			// btn_Settings
			// 
			this.btn_Settings.Appearance.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold);
			this.btn_Settings.Appearance.Options.UseFont = true;
			this.btn_Settings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btn_Settings.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
			this.btn_Settings.ImageOptions.SvgImage = global::WireVisionInspection.Properties.Resources.properties;
			this.btn_Settings.Location = new System.Drawing.Point(1200, 4);
			this.btn_Settings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btn_Settings.Name = "btn_Settings";
			this.btn_Settings.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
			this.btn_Settings.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
			this.btn_Settings.Size = new System.Drawing.Size(395, 62);
			this.btn_Settings.TabIndex = 7;
			this.btn_Settings.TabStop = false;
			this.btn_Settings.Text = "설정";
			this.btn_Settings.Click += new System.EventHandler(this.NavigationClick);
			// 
			// btn_RealTimeView
			// 
			this.btn_RealTimeView.Appearance.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold);
			this.btn_RealTimeView.Appearance.Options.UseFont = true;
			this.btn_RealTimeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btn_RealTimeView.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
			this.btn_RealTimeView.ImageOptions.SvgImage = global::WireVisionInspection.Properties.Resources.electronics_video;
			this.btn_RealTimeView.Location = new System.Drawing.Point(3, 4);
			this.btn_RealTimeView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btn_RealTimeView.Name = "btn_RealTimeView";
			this.btn_RealTimeView.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
			this.btn_RealTimeView.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
			this.btn_RealTimeView.Size = new System.Drawing.Size(393, 62);
			this.btn_RealTimeView.TabIndex = 4;
			this.btn_RealTimeView.TabStop = false;
			this.btn_RealTimeView.Text = "실시간 감시";
			this.btn_RealTimeView.Click += new System.EventHandler(this.NavigationClick);
			// 
			// btn_VideoAnalysis
			// 
			this.btn_VideoAnalysis.Appearance.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold);
			this.btn_VideoAnalysis.Appearance.Options.UseFont = true;
			this.btn_VideoAnalysis.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btn_VideoAnalysis.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
			this.btn_VideoAnalysis.ImageOptions.SvgImage = global::WireVisionInspection.Properties.Resources.groupbydate;
			this.btn_VideoAnalysis.Location = new System.Drawing.Point(801, 4);
			this.btn_VideoAnalysis.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btn_VideoAnalysis.Name = "btn_VideoAnalysis";
			this.btn_VideoAnalysis.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
			this.btn_VideoAnalysis.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
			this.btn_VideoAnalysis.Size = new System.Drawing.Size(393, 62);
			this.btn_VideoAnalysis.TabIndex = 6;
			this.btn_VideoAnalysis.TabStop = false;
			this.btn_VideoAnalysis.Text = "영상 분석";
			this.btn_VideoAnalysis.Click += new System.EventHandler(this.NavigationClick);
			// 
			// btn_VideoCheck
			// 
			this.btn_VideoCheck.Appearance.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold);
			this.btn_VideoCheck.Appearance.Options.UseFont = true;
			this.btn_VideoCheck.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btn_VideoCheck.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
			this.btn_VideoCheck.ImageOptions.SvgImage = global::WireVisionInspection.Properties.Resources.resetselectedimages;
			this.btn_VideoCheck.Location = new System.Drawing.Point(402, 4);
			this.btn_VideoCheck.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btn_VideoCheck.Name = "btn_VideoCheck";
			this.btn_VideoCheck.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
			this.btn_VideoCheck.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
			this.btn_VideoCheck.Size = new System.Drawing.Size(393, 62);
			this.btn_VideoCheck.TabIndex = 5;
			this.btn_VideoCheck.TabStop = false;
			this.btn_VideoCheck.Text = "영상 확인";
			this.btn_VideoCheck.Click += new System.EventHandler(this.NavigationClick);
			// 
			// navigationFrame2
			// 
			this.navigationFrame2.Controls.Add(this.RealTimeView_Page);
			this.navigationFrame2.Controls.Add(this.VideoCheck_Page);
			this.navigationFrame2.Controls.Add(this.VideoAnalysis_Page);
			this.navigationFrame2.Controls.Add(this.Settings_Page);
			this.navigationFrame2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.navigationFrame2.Location = new System.Drawing.Point(0, 70);
			this.navigationFrame2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.navigationFrame2.Name = "navigationFrame2";
			this.navigationFrame2.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] {
            this.RealTimeView_Page,
            this.VideoCheck_Page,
            this.VideoAnalysis_Page,
            this.Settings_Page});
			this.navigationFrame2.SelectedPage = this.RealTimeView_Page;
			this.navigationFrame2.Size = new System.Drawing.Size(1598, 798);
			this.navigationFrame2.TabIndex = 1;
			this.navigationFrame2.Text = "navigationFrame2";
			// 
			// RealTimeView_Page
			// 
			this.RealTimeView_Page.Appearance.BackColor = System.Drawing.Color.White;
			this.RealTimeView_Page.Appearance.Options.UseBackColor = true;
			this.RealTimeView_Page.Caption = "RealTimeView_Page";
			this.RealTimeView_Page.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.RealTimeView_Page.Name = "RealTimeView_Page";
			this.RealTimeView_Page.Size = new System.Drawing.Size(1598, 798);
			// 
			// VideoCheck_Page
			// 
			this.VideoCheck_Page.Appearance.BackColor = System.Drawing.Color.White;
			this.VideoCheck_Page.Appearance.Options.UseBackColor = true;
			this.VideoCheck_Page.Caption = "VideoCheck_Page";
			this.VideoCheck_Page.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.VideoCheck_Page.Name = "VideoCheck_Page";
			this.VideoCheck_Page.Size = new System.Drawing.Size(1598, 798);
			// 
			// VideoAnalysis_Page
			// 
			this.VideoAnalysis_Page.Appearance.BackColor = System.Drawing.Color.White;
			this.VideoAnalysis_Page.Appearance.Options.UseBackColor = true;
			this.VideoAnalysis_Page.Caption = "VideoAnalysis_Page";
			this.VideoAnalysis_Page.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.VideoAnalysis_Page.Name = "VideoAnalysis_Page";
			this.VideoAnalysis_Page.Size = new System.Drawing.Size(1598, 798);
			// 
			// Settings_Page
			// 
			this.Settings_Page.Appearance.BackColor = System.Drawing.Color.White;
			this.Settings_Page.Appearance.Options.UseBackColor = true;
			this.Settings_Page.Caption = "Settings_Page";
			this.Settings_Page.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Settings_Page.Name = "Settings_Page";
			this.Settings_Page.Size = new System.Drawing.Size(1598, 798);
			// 
			// fluentDesignFormContainer1
			// 
			this.fluentDesignFormContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fluentDesignFormContainer1.Location = new System.Drawing.Point(0, 0);
			this.fluentDesignFormContainer1.Name = "fluentDesignFormContainer1";
			this.fluentDesignFormContainer1.Size = new System.Drawing.Size(1598, 868);
			this.fluentDesignFormContainer1.TabIndex = 2;
			// 
			// labelControl1
			// 
			this.labelControl1.Appearance.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold);
			this.labelControl1.Appearance.Options.UseFont = true;
			this.labelControl1.Location = new System.Drawing.Point(63, 101);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(87, 28);
			this.labelControl1.TabIndex = 0;
			this.labelControl1.Text = "로그 저장";
			// 
			// emptySpaceItem4
			// 
			this.emptySpaceItem4.AllowHotTrack = false;
			this.emptySpaceItem4.Location = new System.Drawing.Point(0, 46);
			this.emptySpaceItem4.Name = "emptySpaceItem4";
			this.emptySpaceItem4.Size = new System.Drawing.Size(470, 114);
			this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
			// 
			// Form1
			// 
			this.ActiveGlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.Appearance.BackColor = System.Drawing.Color.White;
			this.Appearance.ForeColor = System.Drawing.Color.Black;
			this.Appearance.Options.UseBackColor = true;
			this.Appearance.Options.UseForeColor = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1598, 868);
			this.Controls.Add(this.navigationFrame2);
			this.Controls.Add(this.navigationFrame1);
			this.Controls.Add(this.fluentDesignFormContainer1);
			this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Glow;
			this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("Form1.IconOptions.Icon")));
			this.IconOptions.Image = global::WireVisionInspection.Properties.Resources.Lens;
			this.LookAndFeel.SkinName = "Office 2019 Colorful";
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "와이어 비전 검사";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Shown += new System.EventHandler(this.Form1_Shown);
			this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
			((System.ComponentModel.ISupportInitialize)(this.navigationFrame1)).EndInit();
			this.navigationFrame1.ResumeLayout(false);
			this.ButtonPage.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.navigationFrame2)).EndInit();
			this.navigationFrame2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private DevExpress.XtraBars.Navigation.NavigationFrame navigationFrame1;
		private DevExpress.XtraBars.Navigation.NavigationPage ButtonPage;
		private DevExpress.XtraBars.Navigation.NavigationPage RealTimeView_Page;
		private DevExpress.XtraBars.Navigation.NavigationPage VideoCheck_Page;
		private DevExpress.XtraBars.Navigation.NavigationPage VideoAnalysis_Page;
		private DevExpress.XtraBars.Navigation.NavigationPage Settings_Page;
		private DevExpress.XtraEditors.SimpleButton btn_Settings;
		private DevExpress.XtraEditors.SimpleButton btn_VideoAnalysis;
		private DevExpress.XtraEditors.SimpleButton btn_VideoCheck;
		private DevExpress.XtraEditors.SimpleButton btn_RealTimeView;
		private DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormContainer fluentDesignFormContainer1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
		public DevExpress.XtraBars.Navigation.NavigationFrame navigationFrame2;
	}
}

