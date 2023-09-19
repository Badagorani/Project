namespace WireExternalInspection
{
	partial class PDFSave
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonPage2 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.pb_Image1 = new System.Windows.Forms.PictureBox();
			this.pb_Image2 = new System.Windows.Forms.PictureBox();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
			this.rtb_AnalysisContent = new System.Windows.Forms.RichTextBox();
			this.btn_Save = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pb_Image1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pb_Image2)).BeginInit();
			this.SuspendLayout();
			// 
			// ribbonControl1
			// 
			this.ribbonControl1.ApplicationCaption = " ";
			this.ribbonControl1.ApplicationDocumentCaption = "PDF 저장";
			this.ribbonControl1.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(30, 35, 30, 35);
			this.ribbonControl1.ExpandCollapseItem.Id = 0;
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
			this.ribbonControl1.Size = new System.Drawing.Size(1019, 37);
			this.ribbonControl1.Toolbar.ShowCustomizeItem = false;
			this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
			this.ribbonControl1.TransparentEditorsMode = DevExpress.Utils.DefaultBoolean.False;
			// 
			// ribbonPage1
			// 
			this.ribbonPage1.Name = "ribbonPage1";
			this.ribbonPage1.Text = "ribbonPage1";
			// 
			// ribbonPage2
			// 
			this.ribbonPage2.Name = "ribbonPage2";
			this.ribbonPage2.Text = "ribbonPage2";
			// 
			// pb_Image1
			// 
			this.pb_Image1.BackColor = System.Drawing.Color.Black;
			this.pb_Image1.Location = new System.Drawing.Point(5, 44);
			this.pb_Image1.Name = "pb_Image1";
			this.pb_Image1.Size = new System.Drawing.Size(501, 357);
			this.pb_Image1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pb_Image1.TabIndex = 9;
			this.pb_Image1.TabStop = false;
			// 
			// pb_Image2
			// 
			this.pb_Image2.BackColor = System.Drawing.Color.Black;
			this.pb_Image2.Location = new System.Drawing.Point(5, 422);
			this.pb_Image2.Name = "pb_Image2";
			this.pb_Image2.Size = new System.Drawing.Size(501, 357);
			this.pb_Image2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pb_Image2.TabIndex = 10;
			this.pb_Image2.TabStop = false;
			// 
			// labelControl1
			// 
			this.labelControl1.Appearance.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.labelControl1.Appearance.ForeColor = System.Drawing.Color.Black;
			this.labelControl1.Appearance.Options.UseFont = true;
			this.labelControl1.Appearance.Options.UseForeColor = true;
			this.labelControl1.Location = new System.Drawing.Point(512, 44);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(141, 37);
			this.labelControl1.TabIndex = 11;
			this.labelControl1.Text = "분석 내용  :";
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
			this.btn_Cancel.Appearance.Font = new System.Drawing.Font("맑은 고딕", 30F, System.Drawing.FontStyle.Bold);
			this.btn_Cancel.Appearance.ForeColor = System.Drawing.Color.Black;
			this.btn_Cancel.Appearance.Options.UseBackColor = true;
			this.btn_Cancel.Appearance.Options.UseFont = true;
			this.btn_Cancel.Appearance.Options.UseForeColor = true;
			this.btn_Cancel.Location = new System.Drawing.Point(512, 786);
			this.btn_Cancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
			this.btn_Cancel.Size = new System.Drawing.Size(501, 72);
			this.btn_Cancel.TabIndex = 86;
			this.btn_Cancel.Text = "취   소";
			this.btn_Cancel.Click += new System.EventHandler(this.PDF_Save_Button_Action);
			// 
			// rtb_AnalysisContent
			// 
			this.rtb_AnalysisContent.Font = new System.Drawing.Font("맑은 고딕", 12F);
			this.rtb_AnalysisContent.ForeColor = System.Drawing.Color.Black;
			this.rtb_AnalysisContent.Location = new System.Drawing.Point(512, 87);
			this.rtb_AnalysisContent.Name = "rtb_AnalysisContent";
			this.rtb_AnalysisContent.Size = new System.Drawing.Size(501, 692);
			this.rtb_AnalysisContent.TabIndex = 87;
			this.rtb_AnalysisContent.Text = "분석 내용을 입력하십시오";
			// 
			// btn_Save
			// 
			this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(115)))), ((int)(((byte)(70)))));
			this.btn_Save.Appearance.Font = new System.Drawing.Font("맑은 고딕", 30F, System.Drawing.FontStyle.Bold);
			this.btn_Save.Appearance.ForeColor = System.Drawing.Color.Black;
			this.btn_Save.Appearance.Options.UseBackColor = true;
			this.btn_Save.Appearance.Options.UseFont = true;
			this.btn_Save.Appearance.Options.UseForeColor = true;
			this.btn_Save.Location = new System.Drawing.Point(5, 786);
			this.btn_Save.LookAndFeel.UseDefaultLookAndFeel = false;
			this.btn_Save.Name = "btn_Save";
			this.btn_Save.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
			this.btn_Save.Size = new System.Drawing.Size(501, 72);
			this.btn_Save.TabIndex = 89;
			this.btn_Save.Text = "저   장";
			this.btn_Save.Click += new System.EventHandler(this.PDF_Save_Button_Action);
			// 
			// PDFSave
			// 
			this.Appearance.BackColor = System.Drawing.Color.White;
			this.Appearance.ForeColor = System.Drawing.Color.Black;
			this.Appearance.Options.UseBackColor = true;
			this.Appearance.Options.UseForeColor = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1019, 863);
			this.Controls.Add(this.btn_Save);
			this.Controls.Add(this.rtb_AnalysisContent);
			this.Controls.Add(this.btn_Cancel);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.pb_Image2);
			this.Controls.Add(this.pb_Image1);
			this.Controls.Add(this.ribbonControl1);
			this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Glow;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.IconOptions.Image = global::WireExternalInspection.Properties.Resources.Lens;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PDFSave";
			this.Ribbon = this.ribbonControl1;
			this.RibbonVisibility = DevExpress.XtraBars.Ribbon.RibbonVisibility.Hidden;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "PDFSave";
			this.Shown += new System.EventHandler(this.PDFSave_Shown);
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pb_Image1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pb_Image2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage2;
		private System.Windows.Forms.PictureBox pb_Image1;
		private System.Windows.Forms.PictureBox pb_Image2;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		public DevExpress.XtraEditors.SimpleButton btn_Cancel;
		private System.Windows.Forms.RichTextBox rtb_AnalysisContent;
		private DevExpress.XtraEditors.SimpleButton btn_Save;
	}
}