namespace RulerJB
{
	partial class FormAdjustPixelSize
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdjustPixelSize));
			this.btnSet = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.tboxPxPerMm = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.tboxDpi = new System.Windows.Forms.TextBox();
			this.rbtnDpi = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.rbtnImage = new System.Windows.Forms.RadioButton();
			this.tboxCurLength = new System.Windows.Forms.TextBox();
			this.rbtnHeightP = new System.Windows.Forms.RadioButton();
			this.tboxHeightP = new System.Windows.Forms.TextBox();
			this.rbtnPxPerMm = new System.Windows.Forms.RadioButton();
			this.pictboxAdjustPix = new System.Windows.Forms.PictureBox();
			this.labelCur = new System.Windows.Forms.Label();
			this.labelDisplayRate = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictboxAdjustPix)).BeginInit();
			this.SuspendLayout();
			// 
			// btnSet
			// 
			this.btnSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSet.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnSet.Location = new System.Drawing.Point(768, 537);
			this.btnSet.Name = "btnSet";
			this.btnSet.Size = new System.Drawing.Size(75, 33);
			this.btnSet.TabIndex = 1;
			this.btnSet.Text = "設定";
			this.btnSet.UseVisualStyleBackColor = true;
			this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(859, 537);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 33);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "キャンセル";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// tboxPxPerMm
			// 
			this.tboxPxPerMm.Location = new System.Drawing.Point(83, 208);
			this.tboxPxPerMm.Name = "tboxPxPerMm";
			this.tboxPxPerMm.Size = new System.Drawing.Size(59, 19);
			this.tboxPxPerMm.TabIndex = 3;
			this.tboxPxPerMm.Text = "10.000";
			this.tboxPxPerMm.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.tboxPxPerMm.Enter += new System.EventHandler(this.OnTboxSelectEnter);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.tboxDpi);
			this.groupBox1.Controls.Add(this.rbtnDpi);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.rbtnImage);
			this.groupBox1.Controls.Add(this.tboxCurLength);
			this.groupBox1.Controls.Add(this.rbtnHeightP);
			this.groupBox1.Controls.Add(this.tboxHeightP);
			this.groupBox1.Controls.Add(this.rbtnPxPerMm);
			this.groupBox1.Controls.Add(this.tboxPxPerMm);
			this.groupBox1.Location = new System.Drawing.Point(768, 41);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(187, 353);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(148, 289);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(20, 12);
			this.label4.TabIndex = 7;
			this.label4.Text = "dpi";
			// 
			// tboxDpi
			// 
			this.tboxDpi.Location = new System.Drawing.Point(83, 286);
			this.tboxDpi.Name = "tboxDpi";
			this.tboxDpi.Size = new System.Drawing.Size(59, 19);
			this.tboxDpi.TabIndex = 6;
			this.tboxDpi.Text = "600";
			this.tboxDpi.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.tboxDpi.TextChanged += new System.EventHandler(this.tboxDpi_TextChanged);
			// 
			// rbtnDpi
			// 
			this.rbtnDpi.AutoSize = true;
			this.rbtnDpi.Location = new System.Drawing.Point(19, 264);
			this.rbtnDpi.Name = "rbtnDpi";
			this.rbtnDpi.Size = new System.Drawing.Size(82, 16);
			this.rbtnDpi.TabIndex = 4;
			this.rbtnDpi.Text = "解像度(dpi)";
			this.rbtnDpi.UseVisualStyleBackColor = true;
			this.rbtnDpi.CheckedChanged += new System.EventHandler(this.rbtnDpi_CheckedChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(148, 55);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(23, 12);
			this.label3.TabIndex = 5;
			this.label3.Text = "mm";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(148, 211);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(18, 12);
			this.label2.TabIndex = 5;
			this.label2.Text = "Px";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(148, 133);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(23, 12);
			this.label1.TabIndex = 5;
			this.label1.Text = "mm";
			// 
			// rbtnImage
			// 
			this.rbtnImage.AutoSize = true;
			this.rbtnImage.Location = new System.Drawing.Point(19, 108);
			this.rbtnImage.Name = "rbtnImage";
			this.rbtnImage.Size = new System.Drawing.Size(105, 16);
			this.rbtnImage.TabIndex = 4;
			this.rbtnImage.Text = "画像カーソル長さ";
			this.rbtnImage.UseVisualStyleBackColor = true;
			this.rbtnImage.CheckedChanged += new System.EventHandler(this.rbtnImage_CheckedChanged);
			// 
			// tboxCurLength
			// 
			this.tboxCurLength.Location = new System.Drawing.Point(83, 130);
			this.tboxCurLength.Name = "tboxCurLength";
			this.tboxCurLength.Size = new System.Drawing.Size(59, 19);
			this.tboxCurLength.TabIndex = 3;
			this.tboxCurLength.Text = "100.0";
			this.tboxCurLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.tboxCurLength.Enter += new System.EventHandler(this.OnTboxSelectEnter);
			// 
			// rbtnHeightP
			// 
			this.rbtnHeightP.AutoSize = true;
			this.rbtnHeightP.Checked = true;
			this.rbtnHeightP.Location = new System.Drawing.Point(19, 30);
			this.rbtnHeightP.Name = "rbtnHeightP";
			this.rbtnHeightP.Size = new System.Drawing.Size(97, 16);
			this.rbtnHeightP.TabIndex = 4;
			this.rbtnHeightP.TabStop = true;
			this.rbtnHeightP.Text = "紙高入力(mm)";
			this.rbtnHeightP.UseVisualStyleBackColor = true;
			this.rbtnHeightP.CheckedChanged += new System.EventHandler(this.rbtnPxPerMm_CheckedChanged);
			// 
			// tboxHeightP
			// 
			this.tboxHeightP.Location = new System.Drawing.Point(83, 52);
			this.tboxHeightP.Name = "tboxHeightP";
			this.tboxHeightP.Size = new System.Drawing.Size(59, 19);
			this.tboxHeightP.TabIndex = 3;
			this.tboxHeightP.Text = "20.000";
			this.tboxHeightP.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.tboxHeightP.Enter += new System.EventHandler(this.OnTboxSelectEnter);
			// 
			// rbtnPxPerMm
			// 
			this.rbtnPxPerMm.AutoSize = true;
			this.rbtnPxPerMm.Location = new System.Drawing.Point(19, 186);
			this.rbtnPxPerMm.Name = "rbtnPxPerMm";
			this.rbtnPxPerMm.Size = new System.Drawing.Size(123, 16);
			this.rbtnPxPerMm.TabIndex = 4;
			this.rbtnPxPerMm.Text = "1mm当たりのPixel数";
			this.rbtnPxPerMm.UseVisualStyleBackColor = true;
			this.rbtnPxPerMm.CheckedChanged += new System.EventHandler(this.rbtnPxPerMm_CheckedChanged);
			// 
			// pictboxAdjustPix
			// 
			this.pictboxAdjustPix.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictboxAdjustPix.BackColor = System.Drawing.Color.Black;
			this.pictboxAdjustPix.Location = new System.Drawing.Point(52, 41);
			this.pictboxAdjustPix.Name = "pictboxAdjustPix";
			this.pictboxAdjustPix.Size = new System.Drawing.Size(702, 472);
			this.pictboxAdjustPix.TabIndex = 0;
			this.pictboxAdjustPix.TabStop = false;
			this.pictboxAdjustPix.Paint += new System.Windows.Forms.PaintEventHandler(this.pictboxAdjustPix_Paint);
			this.pictboxAdjustPix.DoubleClick += new System.EventHandler(this.pictboxAdjustPix_DoubleClick);
			// 
			// labelCur
			// 
			this.labelCur.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCur.AutoSize = true;
			this.labelCur.Location = new System.Drawing.Point(443, 537);
			this.labelCur.Name = "labelCur";
			this.labelCur.Size = new System.Drawing.Size(35, 12);
			this.labelCur.TabIndex = 7;
			this.labelCur.Text = "label3";
			// 
			// labelDisplayRate
			// 
			this.labelDisplayRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDisplayRate.AutoSize = true;
			this.labelDisplayRate.Location = new System.Drawing.Point(713, 23);
			this.labelDisplayRate.Name = "labelDisplayRate";
			this.labelDisplayRate.Size = new System.Drawing.Size(41, 12);
			this.labelDisplayRate.TabIndex = 9;
			this.labelDisplayRate.Text = "---.- %";
			// 
			// FormAdjustPixelSize
			// 
			this.AcceptButton = this.btnSet;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(967, 587);
			this.Controls.Add(this.labelDisplayRate);
			this.Controls.Add(this.labelCur);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSet);
			this.Controls.Add(this.pictboxAdjustPix);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAdjustPixelSize";
			this.Text = "FormAdjustPixelSize";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAdjustPixelSize_FormClosed);
			this.Load += new System.EventHandler(this.FormAdjustPixelSize_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormAdjustPixelSize_Paint);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormAdjustPixelSize_KeyDown);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictboxAdjustPix)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictboxAdjustPix;
		private System.Windows.Forms.Button btnSet;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox tboxPxPerMm;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton rbtnImage;
		private System.Windows.Forms.TextBox tboxCurLength;
		private System.Windows.Forms.RadioButton rbtnPxPerMm;
		private System.Windows.Forms.Label labelCur;
		private System.Windows.Forms.Label labelDisplayRate;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.RadioButton rbtnHeightP;
		private System.Windows.Forms.TextBox tboxHeightP;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tboxDpi;
		private System.Windows.Forms.RadioButton rbtnDpi;
	}
}