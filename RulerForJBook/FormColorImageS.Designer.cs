namespace RulerJB
{
	partial class FormColorImageS
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
			this.components = new System.ComponentModel.Container();
			this.pboxColImage = new System.Windows.Forms.PictureBox();
			this.contextMenuStripL = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.画像を保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.画像をクリップボードにコピーToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.scalPboxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.labelDisplayRate = new System.Windows.Forms.Label();
			this.labelMouseCursor = new System.Windows.Forms.Label();
			this.labelHsvRange = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pboxColImage)).BeginInit();
			this.contextMenuStripL.SuspendLayout();
			this.SuspendLayout();
			// 
			// pboxColImage
			// 
			this.pboxColImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pboxColImage.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.pboxColImage.ContextMenuStrip = this.contextMenuStripL;
			this.pboxColImage.Location = new System.Drawing.Point(23, 39);
			this.pboxColImage.Margin = new System.Windows.Forms.Padding(2);
			this.pboxColImage.Name = "pboxColImage";
			this.pboxColImage.Size = new System.Drawing.Size(595, 450);
			this.pboxColImage.TabIndex = 0;
			this.pboxColImage.TabStop = false;
			this.pboxColImage.DoubleClick += new System.EventHandler(this.pboxColImage_DoubleClick);
			// 
			// contextMenuStripL
			// 
			this.contextMenuStripL.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.画像を保存ToolStripMenuItem,
            this.画像をクリップボードにコピーToolStripMenuItem,
            this.scalPboxToolStripMenuItem});
			this.contextMenuStripL.Name = "contextMenuStripL";
			this.contextMenuStripL.Size = new System.Drawing.Size(202, 70);
			// 
			// 画像を保存ToolStripMenuItem
			// 
			this.画像を保存ToolStripMenuItem.Name = "画像を保存ToolStripMenuItem";
			this.画像を保存ToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
			this.画像を保存ToolStripMenuItem.Text = "画像をファイルに保存";
			this.画像を保存ToolStripMenuItem.Click += new System.EventHandler(this.画像を保存ToolStripMenuItem_Click);
			// 
			// 画像をクリップボードにコピーToolStripMenuItem
			// 
			this.画像をクリップボードにコピーToolStripMenuItem.Name = "画像をクリップボードにコピーToolStripMenuItem";
			this.画像をクリップボードにコピーToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
			this.画像をクリップボードにコピーToolStripMenuItem.Text = "画像をクリップボードにコピー";
			this.画像をクリップボードにコピーToolStripMenuItem.Click += new System.EventHandler(this.画像をクリップボードにコピーToolStripMenuItem_Click);
			// 
			// scalPboxToolStripMenuItem
			// 
			this.scalPboxToolStripMenuItem.Name = "scalPboxToolStripMenuItem";
			this.scalPboxToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
			this.scalPboxToolStripMenuItem.Text = "画像情報画面で表示";
			this.scalPboxToolStripMenuItem.Click += new System.EventHandler(this.画像情報画面で表示ToolStripMenuItem_Click);
			// 
			// labelDisplayRate
			// 
			this.labelDisplayRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDisplayRate.AutoSize = true;
			this.labelDisplayRate.Location = new System.Drawing.Point(510, 17);
			this.labelDisplayRate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelDisplayRate.Name = "labelDisplayRate";
			this.labelDisplayRate.Size = new System.Drawing.Size(29, 12);
			this.labelDisplayRate.TabIndex = 1;
			this.labelDisplayRate.Text = "----";
			// 
			// labelMouseCursor
			// 
			this.labelMouseCursor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelMouseCursor.AutoSize = true;
			this.labelMouseCursor.Location = new System.Drawing.Point(21, 506);
			this.labelMouseCursor.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelMouseCursor.Name = "labelMouseCursor";
			this.labelMouseCursor.Size = new System.Drawing.Size(53, 12);
			this.labelMouseCursor.TabIndex = 2;
			this.labelMouseCursor.Text = "--------";
			// 
			// labelHsvRange
			// 
			this.labelHsvRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelHsvRange.AutoSize = true;
			this.labelHsvRange.Location = new System.Drawing.Point(328, 506);
			this.labelHsvRange.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelHsvRange.Name = "labelHsvRange";
			this.labelHsvRange.Size = new System.Drawing.Size(53, 12);
			this.labelHsvRange.TabIndex = 2;
			this.labelHsvRange.Text = "--------";
			// 
			// FormColorImageS
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(785, 542);
			this.Controls.Add(this.labelHsvRange);
			this.Controls.Add(this.labelMouseCursor);
			this.Controls.Add(this.labelDisplayRate);
			this.Controls.Add(this.pboxColImage);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "FormColorImageS";
			this.Text = "FormColorImageS";
			this.Load += new System.EventHandler(this.FormColorImageS_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormColorImageS_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.pboxColImage)).EndInit();
			this.contextMenuStripL.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pboxColImage;
		private System.Windows.Forms.Label labelDisplayRate;
		private System.Windows.Forms.Label labelMouseCursor;
		private System.Windows.Forms.Label labelHsvRange;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripL;
		private System.Windows.Forms.ToolStripMenuItem 画像を保存ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 画像をクリップボードにコピーToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem scalPboxToolStripMenuItem;
	}
}