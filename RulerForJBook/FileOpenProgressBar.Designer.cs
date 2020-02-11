namespace RulerJB
{
	partial class FileOpenProgressBar
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
			this.progressBarFileOpen = new System.Windows.Forms.ProgressBar();
			this.labelwait = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// progressBarFileOpen
			// 
			this.progressBarFileOpen.Location = new System.Drawing.Point(12, 38);
			this.progressBarFileOpen.Name = "progressBarFileOpen";
			this.progressBarFileOpen.Size = new System.Drawing.Size(325, 23);
			this.progressBarFileOpen.TabIndex = 0;
			// 
			// labelwait
			// 
			this.labelwait.AutoSize = true;
			this.labelwait.Location = new System.Drawing.Point(10, 9);
			this.labelwait.Name = "labelwait";
			this.labelwait.Size = new System.Drawing.Size(110, 12);
			this.labelwait.TabIndex = 1;
			this.labelwait.Text = "しばらくお待ちください。";
			// 
			// FileOpenProgressBar
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(349, 73);
			this.Controls.Add(this.labelwait);
			this.Controls.Add(this.progressBarFileOpen);
			this.Name = "FileOpenProgressBar";
			this.Text = "ファイルを開いています。";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBarFileOpen;
		private System.Windows.Forms.Label labelwait;
	}
}