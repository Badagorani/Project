namespace WireExternalInspection
{
	partial class LoginForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
			this.ribbonPage2 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btn_Start = new DevExpress.XtraEditors.SimpleButton();
			this.dat_Inspection_Date = new DevExpress.XtraEditors.DateEdit();
			this.txt_Inspection_Place = new DevExpress.XtraEditors.TextEdit();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.label4 = new System.Windows.Forms.Label();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.txt_Inspection_User = new DevExpress.XtraEditors.TextEdit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dat_Inspection_Date.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dat_Inspection_Date.Properties.CalendarTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txt_Inspection_Place.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txt_Inspection_User.Properties)).BeginInit();
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
			this.ribbonControl1.ApplicationDocumentCaption = "와이어 비전 검사 프로그램";
			this.ribbonControl1.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(30, 42, 30, 42);
			this.ribbonControl1.ExpandCollapseItem.Id = 0;
			this.ribbonControl1.Font = new System.Drawing.Font("맑은 고딕", 12F);
			this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem});
			this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
			this.ribbonControl1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
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
			this.ribbonControl1.Size = new System.Drawing.Size(698, 37);
			this.ribbonControl1.Toolbar.ShowCustomizeItem = false;
			this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
			this.ribbonControl1.TransparentEditorsMode = DevExpress.Utils.DefaultBoolean.False;
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.Black;
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Default;
			this.pictureBox1.Image = global::WireExternalInspection.Properties.Resources.LogoImage1;
			this.pictureBox1.Location = new System.Drawing.Point(85, 69);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(528, 306);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox1.TabIndex = 10;
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.label1.Location = new System.Drawing.Point(94, 472);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(149, 37);
			this.label1.TabIndex = 12;
			this.label1.Text = "검  사  자 :";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.label2.Location = new System.Drawing.Point(94, 396);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(149, 37);
			this.label2.TabIndex = 14;
			this.label2.Text = "검  사  일 :";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.label3.Location = new System.Drawing.Point(94, 548);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(149, 37);
			this.label3.TabIndex = 16;
			this.label3.Text = "검사 장소 :";
			// 
			// btn_Start
			// 
			this.btn_Start.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
			this.btn_Start.Appearance.Font = new System.Drawing.Font("맑은 고딕", 30F, System.Drawing.FontStyle.Bold);
			this.btn_Start.Appearance.ForeColor = System.Drawing.Color.Black;
			this.btn_Start.Appearance.Options.UseBackColor = true;
			this.btn_Start.Appearance.Options.UseFont = true;
			this.btn_Start.Appearance.Options.UseForeColor = true;
			this.btn_Start.Enabled = false;
			this.btn_Start.Location = new System.Drawing.Point(245, 655);
			this.btn_Start.LookAndFeel.UseDefaultLookAndFeel = false;
			this.btn_Start.Name = "btn_Start";
			this.btn_Start.Size = new System.Drawing.Size(209, 77);
			this.btn_Start.TabIndex = 19;
			this.btn_Start.Text = "시   작";
			this.btn_Start.Click += new System.EventHandler(this.Start_Click);
			// 
			// dat_Inspection_Date
			// 
			this.dat_Inspection_Date.EditValue = new System.DateTime(2023, 8, 22, 15, 10, 19, 525);
			this.dat_Inspection_Date.Location = new System.Drawing.Point(266, 396);
			this.dat_Inspection_Date.MenuManager = this.ribbonControl1;
			this.dat_Inspection_Date.Name = "dat_Inspection_Date";
			this.dat_Inspection_Date.Properties.Appearance.BorderColor = System.Drawing.Color.Lime;
			this.dat_Inspection_Date.Properties.Appearance.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.dat_Inspection_Date.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
			this.dat_Inspection_Date.Properties.Appearance.Options.UseBorderColor = true;
			this.dat_Inspection_Date.Properties.Appearance.Options.UseFont = true;
			this.dat_Inspection_Date.Properties.Appearance.Options.UseForeColor = true;
			this.dat_Inspection_Date.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.dat_Inspection_Date.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.dat_Inspection_Date.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.dat_Inspection_Date.Size = new System.Drawing.Size(289, 44);
			this.dat_Inspection_Date.TabIndex = 22;
			this.dat_Inspection_Date.TabStop = false;
			// 
			// txt_Inspection_Place
			// 
			this.txt_Inspection_Place.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.txt_Inspection_Place.Location = new System.Drawing.Point(266, 548);
			this.txt_Inspection_Place.MenuManager = this.ribbonControl1;
			this.txt_Inspection_Place.Name = "txt_Inspection_Place";
			this.txt_Inspection_Place.Properties.Appearance.BackColor = System.Drawing.Color.White;
			this.txt_Inspection_Place.Properties.Appearance.BorderColor = System.Drawing.Color.Red;
			this.txt_Inspection_Place.Properties.Appearance.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.txt_Inspection_Place.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
			this.txt_Inspection_Place.Properties.Appearance.Options.UseBackColor = true;
			this.txt_Inspection_Place.Properties.Appearance.Options.UseBorderColor = true;
			this.txt_Inspection_Place.Properties.Appearance.Options.UseFont = true;
			this.txt_Inspection_Place.Properties.Appearance.Options.UseForeColor = true;
			this.txt_Inspection_Place.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.txt_Inspection_Place.Size = new System.Drawing.Size(289, 44);
			this.txt_Inspection_Place.TabIndex = 23;
			this.txt_Inspection_Place.EditValueChanged += new System.EventHandler(this.InputCheck);
			this.txt_Inspection_Place.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InputEnterEvent);
			// 
			// labelControl1
			// 
			this.labelControl1.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("labelControl1.ImageOptions.SvgImage")));
			this.labelControl1.ImageOptions.SvgImageSize = new System.Drawing.Size(44, 44);
			this.labelControl1.Location = new System.Drawing.Point(561, 472);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(44, 44);
			this.labelControl1.TabIndex = 26;
			// 
			// labelControl2
			// 
			this.labelControl2.ImageOptions.SvgImage = global::WireExternalInspection.Properties.Resources.actions_deletecircled;
			this.labelControl2.ImageOptions.SvgImageSize = new System.Drawing.Size(44, 44);
			this.labelControl2.Location = new System.Drawing.Point(561, 548);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Size = new System.Drawing.Size(44, 44);
			this.labelControl2.TabIndex = 28;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("맑은 고딕", 15F);
			this.label4.Location = new System.Drawing.Point(152, 610);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(394, 28);
			this.label4.TabIndex = 30;
			this.label4.Text = "☹ 입력한 정보는 변경할 수 없습니다! ☹";
			// 
			// labelControl3
			// 
			this.labelControl3.ImageOptions.SvgImage = global::WireExternalInspection.Properties.Resources.actions_checkcircled;
			this.labelControl3.ImageOptions.SvgImageSize = new System.Drawing.Size(44, 44);
			this.labelControl3.Location = new System.Drawing.Point(561, 396);
			this.labelControl3.Name = "labelControl3";
			this.labelControl3.Size = new System.Drawing.Size(44, 44);
			this.labelControl3.TabIndex = 32;
			// 
			// txt_Inspection_User
			// 
			this.txt_Inspection_User.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.txt_Inspection_User.Location = new System.Drawing.Point(266, 472);
			this.txt_Inspection_User.MenuManager = this.ribbonControl1;
			this.txt_Inspection_User.Name = "txt_Inspection_User";
			this.txt_Inspection_User.Properties.Appearance.BackColor = System.Drawing.Color.White;
			this.txt_Inspection_User.Properties.Appearance.BorderColor = System.Drawing.Color.Red;
			this.txt_Inspection_User.Properties.Appearance.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.txt_Inspection_User.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
			this.txt_Inspection_User.Properties.Appearance.Options.UseBackColor = true;
			this.txt_Inspection_User.Properties.Appearance.Options.UseBorderColor = true;
			this.txt_Inspection_User.Properties.Appearance.Options.UseFont = true;
			this.txt_Inspection_User.Properties.Appearance.Options.UseForeColor = true;
			this.txt_Inspection_User.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.txt_Inspection_User.Size = new System.Drawing.Size(289, 44);
			this.txt_Inspection_User.TabIndex = 21;
			this.txt_Inspection_User.EditValueChanged += new System.EventHandler(this.InputCheck);
			this.txt_Inspection_User.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InputEnterEvent);
			// 
			// LoginForm
			// 
			this.ActiveGlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(115)))), ((int)(((byte)(70)))));
			this.Appearance.BackColor = System.Drawing.Color.White;
			this.Appearance.ForeColor = System.Drawing.Color.Black;
			this.Appearance.Options.UseBackColor = true;
			this.Appearance.Options.UseFont = true;
			this.Appearance.Options.UseForeColor = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(698, 771);
			this.Controls.Add(this.txt_Inspection_User);
			this.Controls.Add(this.labelControl3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelControl2);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.txt_Inspection_Place);
			this.Controls.Add(this.dat_Inspection_Date);
			this.Controls.Add(this.btn_Start);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.ribbonControl1);
			this.Font = new System.Drawing.Font("맑은 고딕", 10F);
			this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Glow;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.IconOptions.Image = global::WireExternalInspection.Properties.Resources.Lens;
			this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginForm";
			this.Ribbon = this.ribbonControl1;
			this.RibbonVisibility = DevExpress.XtraBars.Ribbon.RibbonVisibility.Hidden;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Load += new System.EventHandler(this.LoginForm_Load);
			this.Shown += new System.EventHandler(this.LoginForm_Shown);
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dat_Inspection_Date.Properties.CalendarTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dat_Inspection_Date.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txt_Inspection_Place.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txt_Inspection_User.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage2;
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
		private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private DevExpress.XtraEditors.SimpleButton btn_Start;
		private DevExpress.XtraEditors.DateEdit dat_Inspection_Date;
		private DevExpress.XtraEditors.TextEdit txt_Inspection_Place;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraEditors.LabelControl labelControl2;
		private System.Windows.Forms.Label label4;
		private DevExpress.XtraEditors.LabelControl labelControl3;
		private DevExpress.XtraEditors.TextEdit txt_Inspection_User;
	}
}

