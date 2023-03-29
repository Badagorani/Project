namespace ImageAnalysis
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
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.trackBar2 = new System.Windows.Forms.TrackBar();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.button6 = new System.Windows.Forms.Button();
			this.button7 = new System.Windows.Forms.Button();
			this.button8 = new System.Windows.Forms.Button();
			this.button9 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.button13 = new System.Windows.Forms.Button();
			this.button12 = new System.Windows.Forms.Button();
			this.button11 = new System.Windows.Forms.Button();
			this.button10 = new System.Windows.Forms.Button();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.trackBar3 = new System.Windows.Forms.TrackBar();
			this.trackBar4 = new System.Windows.Forms.TrackBar();
			this.button14 = new System.Windows.Forms.Button();
			this.button15 = new System.Windows.Forms.Button();
			this.button16 = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.button17 = new System.Windows.Forms.Button();
			this.button18 = new System.Windows.Forms.Button();
			this.textBox5 = new System.Windows.Forms.TextBox();
			this.trackBar5 = new System.Windows.Forms.TrackBar();
			this.button19 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar5)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.Black;
			this.pictureBox1.Location = new System.Drawing.Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(1212, 600);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button1.Location = new System.Drawing.Point(12, 618);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(1212, 59);
			this.button1.TabIndex = 1;
			this.button1.Text = "이미지 가져오기";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button2.Location = new System.Drawing.Point(312, 681);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(300, 59);
			this.button2.TabIndex = 2;
			this.button2.Text = "윤곽선 숫자";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button3.Location = new System.Drawing.Point(12, 681);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(294, 59);
			this.button3.TabIndex = 3;
			this.button3.Text = "Canny";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button4
			// 
			this.button4.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button4.Location = new System.Drawing.Point(618, 683);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(300, 59);
			this.button4.TabIndex = 4;
			this.button4.Text = "이진화";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button5
			// 
			this.button5.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button5.Location = new System.Drawing.Point(924, 683);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(300, 59);
			this.button5.TabIndex = 5;
			this.button5.Text = "Canny + 윤곽선 숫자";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// trackBar1
			// 
			this.trackBar1.Location = new System.Drawing.Point(1379, 12);
			this.trackBar1.Maximum = 255;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Size = new System.Drawing.Size(579, 45);
			this.trackBar1.TabIndex = 6;
			this.trackBar1.Value = 235;
			this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
			this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_Scroll);
			// 
			// trackBar2
			// 
			this.trackBar2.Location = new System.Drawing.Point(1379, 63);
			this.trackBar2.Maximum = 255;
			this.trackBar2.Name = "trackBar2";
			this.trackBar2.Size = new System.Drawing.Size(579, 45);
			this.trackBar2.TabIndex = 7;
			this.trackBar2.Value = 255;
			this.trackBar2.Scroll += new System.EventHandler(this.trackBar2_Scroll);
			this.trackBar2.ValueChanged += new System.EventHandler(this.trackBar2_Scroll);
			// 
			// textBox1
			// 
			this.textBox1.Font = new System.Drawing.Font("맑은 고딕", 15F);
			this.textBox1.Location = new System.Drawing.Point(2113, 12);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(151, 34);
			this.textBox1.TabIndex = 8;
			this.textBox1.Text = "235";
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnlyDigitPress);
			// 
			// textBox2
			// 
			this.textBox2.Font = new System.Drawing.Font("맑은 고딕", 15F);
			this.textBox2.Location = new System.Drawing.Point(2113, 63);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(151, 34);
			this.textBox2.TabIndex = 9;
			this.textBox2.Text = "255";
			this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
			this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnlyDigitPress);
			// 
			// button6
			// 
			this.button6.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button6.Location = new System.Drawing.Point(1964, 4);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(59, 42);
			this.button6.TabIndex = 10;
			this.button6.Text = "▲";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(this.button6_Click);
			// 
			// button7
			// 
			this.button7.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button7.Location = new System.Drawing.Point(2029, 4);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(59, 42);
			this.button7.TabIndex = 11;
			this.button7.Text = "▼";
			this.button7.UseVisualStyleBackColor = true;
			this.button7.Click += new System.EventHandler(this.button7_Click);
			// 
			// button8
			// 
			this.button8.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button8.Location = new System.Drawing.Point(2029, 55);
			this.button8.Name = "button8";
			this.button8.Size = new System.Drawing.Size(59, 42);
			this.button8.TabIndex = 13;
			this.button8.Text = "▼";
			this.button8.UseVisualStyleBackColor = true;
			this.button8.Click += new System.EventHandler(this.button8_Click);
			// 
			// button9
			// 
			this.button9.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button9.Location = new System.Drawing.Point(1964, 55);
			this.button9.Name = "button9";
			this.button9.Size = new System.Drawing.Size(59, 42);
			this.button9.TabIndex = 12;
			this.button9.Text = "▲";
			this.button9.UseVisualStyleBackColor = true;
			this.button9.Click += new System.EventHandler(this.button9_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("맑은 고딕", 15F);
			this.label1.Location = new System.Drawing.Point(1230, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(143, 28);
			this.label1.TabIndex = 14;
			this.label1.Text = "이진화설정값1";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("맑은 고딕", 15F);
			this.label2.Location = new System.Drawing.Point(1230, 66);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(143, 28);
			this.label2.TabIndex = 15;
			this.label2.Text = "이진화설정값2";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("맑은 고딕", 15F);
			this.label3.Location = new System.Drawing.Point(1230, 168);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(140, 28);
			this.label3.TabIndex = 25;
			this.label3.Text = "Canny설정값2";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("맑은 고딕", 15F);
			this.label4.Location = new System.Drawing.Point(1230, 114);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(140, 28);
			this.label4.TabIndex = 24;
			this.label4.Text = "Canny설정값1";
			// 
			// button13
			// 
			this.button13.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button13.Location = new System.Drawing.Point(2029, 157);
			this.button13.Name = "button13";
			this.button13.Size = new System.Drawing.Size(59, 42);
			this.button13.TabIndex = 23;
			this.button13.Text = "▼";
			this.button13.UseVisualStyleBackColor = true;
			this.button13.Click += new System.EventHandler(this.button13_Click);
			// 
			// button12
			// 
			this.button12.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button12.Location = new System.Drawing.Point(1964, 157);
			this.button12.Name = "button12";
			this.button12.Size = new System.Drawing.Size(59, 42);
			this.button12.TabIndex = 22;
			this.button12.Text = "▲";
			this.button12.UseVisualStyleBackColor = true;
			this.button12.Click += new System.EventHandler(this.button12_Click);
			// 
			// button11
			// 
			this.button11.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button11.Location = new System.Drawing.Point(2029, 106);
			this.button11.Name = "button11";
			this.button11.Size = new System.Drawing.Size(59, 42);
			this.button11.TabIndex = 21;
			this.button11.Text = "▼";
			this.button11.UseVisualStyleBackColor = true;
			this.button11.Click += new System.EventHandler(this.button11_Click);
			// 
			// button10
			// 
			this.button10.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button10.Location = new System.Drawing.Point(1964, 106);
			this.button10.Name = "button10";
			this.button10.Size = new System.Drawing.Size(59, 42);
			this.button10.TabIndex = 20;
			this.button10.Text = "▲";
			this.button10.UseVisualStyleBackColor = true;
			this.button10.Click += new System.EventHandler(this.button10_Click);
			// 
			// textBox4
			// 
			this.textBox4.Font = new System.Drawing.Font("맑은 고딕", 15F);
			this.textBox4.Location = new System.Drawing.Point(2113, 165);
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(151, 34);
			this.textBox4.TabIndex = 19;
			this.textBox4.Text = "250";
			this.textBox4.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
			this.textBox4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnlyDigitPress);
			// 
			// textBox3
			// 
			this.textBox3.Font = new System.Drawing.Font("맑은 고딕", 15F);
			this.textBox3.Location = new System.Drawing.Point(2113, 114);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(151, 34);
			this.textBox3.TabIndex = 18;
			this.textBox3.Text = "300";
			this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
			this.textBox3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnlyDigitPress);
			// 
			// trackBar3
			// 
			this.trackBar3.Location = new System.Drawing.Point(1379, 114);
			this.trackBar3.Maximum = 500;
			this.trackBar3.Name = "trackBar3";
			this.trackBar3.Size = new System.Drawing.Size(579, 45);
			this.trackBar3.TabIndex = 17;
			this.trackBar3.Value = 300;
			this.trackBar3.Scroll += new System.EventHandler(this.trackBar3_Scroll);
			this.trackBar3.ValueChanged += new System.EventHandler(this.trackBar3_Scroll);
			// 
			// trackBar4
			// 
			this.trackBar4.Location = new System.Drawing.Point(1379, 165);
			this.trackBar4.Maximum = 500;
			this.trackBar4.Name = "trackBar4";
			this.trackBar4.Size = new System.Drawing.Size(579, 45);
			this.trackBar4.TabIndex = 16;
			this.trackBar4.Value = 250;
			this.trackBar4.Scroll += new System.EventHandler(this.trackBar4_Scroll);
			this.trackBar4.ValueChanged += new System.EventHandler(this.trackBar4_Scroll);
			// 
			// button14
			// 
			this.button14.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button14.Location = new System.Drawing.Point(1230, 683);
			this.button14.Name = "button14";
			this.button14.Size = new System.Drawing.Size(300, 59);
			this.button14.TabIndex = 26;
			this.button14.Text = "사각형 색칠";
			this.button14.UseVisualStyleBackColor = true;
			this.button14.Click += new System.EventHandler(this.button14_Click);
			// 
			// button15
			// 
			this.button15.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button15.Location = new System.Drawing.Point(1230, 618);
			this.button15.Name = "button15";
			this.button15.Size = new System.Drawing.Size(300, 59);
			this.button15.TabIndex = 27;
			this.button15.Text = "사각형 색칠2";
			this.button15.UseVisualStyleBackColor = true;
			this.button15.Click += new System.EventHandler(this.button15_Click);
			// 
			// button16
			// 
			this.button16.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button16.Location = new System.Drawing.Point(1230, 553);
			this.button16.Name = "button16";
			this.button16.Size = new System.Drawing.Size(300, 59);
			this.button16.TabIndex = 28;
			this.button16.Text = "Canny + 사각형 색칠";
			this.button16.UseVisualStyleBackColor = true;
			this.button16.Click += new System.EventHandler(this.button16_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("맑은 고딕", 15F);
			this.label5.Location = new System.Drawing.Point(1230, 219);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(139, 28);
			this.label5.TabIndex = 33;
			this.label5.Text = "사각형의 넓이";
			// 
			// button17
			// 
			this.button17.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button17.Location = new System.Drawing.Point(2029, 208);
			this.button17.Name = "button17";
			this.button17.Size = new System.Drawing.Size(59, 42);
			this.button17.TabIndex = 32;
			this.button17.Text = "▼";
			this.button17.UseVisualStyleBackColor = true;
			this.button17.Click += new System.EventHandler(this.button17_Click);
			// 
			// button18
			// 
			this.button18.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button18.Location = new System.Drawing.Point(1964, 208);
			this.button18.Name = "button18";
			this.button18.Size = new System.Drawing.Size(59, 42);
			this.button18.TabIndex = 31;
			this.button18.Text = "▲";
			this.button18.UseVisualStyleBackColor = true;
			this.button18.Click += new System.EventHandler(this.button18_Click);
			// 
			// textBox5
			// 
			this.textBox5.Font = new System.Drawing.Font("맑은 고딕", 15F);
			this.textBox5.Location = new System.Drawing.Point(2113, 216);
			this.textBox5.Name = "textBox5";
			this.textBox5.Size = new System.Drawing.Size(151, 34);
			this.textBox5.TabIndex = 30;
			this.textBox5.Text = "250";
			this.textBox5.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
			this.textBox5.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnlyDigitPress);
			// 
			// trackBar5
			// 
			this.trackBar5.Location = new System.Drawing.Point(1379, 216);
			this.trackBar5.Maximum = 3000;
			this.trackBar5.Minimum = 1;
			this.trackBar5.Name = "trackBar5";
			this.trackBar5.Size = new System.Drawing.Size(579, 45);
			this.trackBar5.TabIndex = 29;
			this.trackBar5.Value = 250;
			this.trackBar5.Scroll += new System.EventHandler(this.trackBar5_Scroll);
			this.trackBar5.ValueChanged += new System.EventHandler(this.trackBar5_Scroll);
			// 
			// button19
			// 
			this.button19.Font = new System.Drawing.Font("맑은 고딕", 20F);
			this.button19.Location = new System.Drawing.Point(1230, 488);
			this.button19.Name = "button19";
			this.button19.Size = new System.Drawing.Size(300, 59);
			this.button19.TabIndex = 34;
			this.button19.Text = "좌표 입력";
			this.button19.UseVisualStyleBackColor = true;
			this.button19.Click += new System.EventHandler(this.button19_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(2278, 761);
			this.Controls.Add(this.button19);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.button17);
			this.Controls.Add(this.button18);
			this.Controls.Add(this.textBox5);
			this.Controls.Add(this.trackBar5);
			this.Controls.Add(this.button16);
			this.Controls.Add(this.button15);
			this.Controls.Add(this.button14);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.button13);
			this.Controls.Add(this.button12);
			this.Controls.Add(this.button11);
			this.Controls.Add(this.button10);
			this.Controls.Add(this.textBox4);
			this.Controls.Add(this.textBox3);
			this.Controls.Add(this.trackBar3);
			this.Controls.Add(this.trackBar4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button8);
			this.Controls.Add(this.button9);
			this.Controls.Add(this.button7);
			this.Controls.Add(this.button6);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.trackBar2);
			this.Controls.Add(this.trackBar1);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.pictureBox1);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar5)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.TrackBar trackBar2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.Button button8;
		private System.Windows.Forms.Button button9;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button button13;
		private System.Windows.Forms.Button button12;
		private System.Windows.Forms.Button button11;
		private System.Windows.Forms.Button button10;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.TrackBar trackBar3;
		private System.Windows.Forms.TrackBar trackBar4;
		private System.Windows.Forms.Button button14;
		private System.Windows.Forms.Button button15;
		private System.Windows.Forms.Button button16;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button button17;
		private System.Windows.Forms.Button button18;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.TrackBar trackBar5;
		private System.Windows.Forms.Button button19;
	}
}

