namespace RulerJB
{
	partial class CreateDirectory
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
			this.label1 = new System.Windows.Forms.Label();
			this.buttonTemp = new System.Windows.Forms.Button();
			this.buttonCreateDir = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(198, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "フォルダを指定して画像を保存しますか？";
			// 
			// buttonTemp
			// 
			this.buttonTemp.Location = new System.Drawing.Point(21, 41);
			this.buttonTemp.Name = "buttonTemp";
			this.buttonTemp.Size = new System.Drawing.Size(187, 42);
			this.buttonTemp.TabIndex = 1;
			this.buttonTemp.Text = "一時保存ファイルで開く";
			this.buttonTemp.UseVisualStyleBackColor = true;
			this.buttonTemp.Click += new System.EventHandler(this.buttonTemp_Click);
			// 
			// buttonCreateDir
			// 
			this.buttonCreateDir.Location = new System.Drawing.Point(221, 41);
			this.buttonCreateDir.Name = "buttonCreateDir";
			this.buttonCreateDir.Size = new System.Drawing.Size(187, 42);
			this.buttonCreateDir.TabIndex = 2;
			this.buttonCreateDir.Text = "名前を付けてファイルを保存する";
			this.buttonCreateDir.UseVisualStyleBackColor = true;
			this.buttonCreateDir.Click += new System.EventHandler(this.buttonCreateDir_Click);
			// 
			// CreateDirectory
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(434, 95);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCreateDir);
			this.Controls.Add(this.buttonTemp);
			this.Controls.Add(this.label1);
			this.Name = "CreateDirectory";
			this.Text = "確認";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonTemp;
		private System.Windows.Forms.Button buttonCreateDir;
	}
}