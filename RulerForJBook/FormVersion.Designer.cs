namespace RulerJB
{
	partial class FormVersion
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVersion));
			this.buttonExit = new System.Windows.Forms.Button();
			this.labelVersion = new System.Windows.Forms.Label();
			this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
			this.labelProgname = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonExit
			// 
			this.buttonExit.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonExit.Location = new System.Drawing.Point(276, 78);
			this.buttonExit.Name = "buttonExit";
			this.buttonExit.Size = new System.Drawing.Size(86, 34);
			this.buttonExit.TabIndex = 0;
			this.buttonExit.Text = "OK";
			this.buttonExit.UseVisualStyleBackColor = true;
			// 
			// labelVersion
			// 
			this.labelVersion.AutoSize = true;
			this.labelVersion.Font = new System.Drawing.Font("ＭＳ ゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelVersion.Location = new System.Drawing.Point(166, 40);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(103, 15);
			this.labelVersion.TabIndex = 1;
			this.labelVersion.Text = "Ver. x.xx.xx";
			// 
			// pictureBoxIcon
			// 
			this.pictureBoxIcon.Image = global::RulerJB.Properties.Resources.R_2;
			this.pictureBoxIcon.Location = new System.Drawing.Point(23, 27);
			this.pictureBoxIcon.Name = "pictureBoxIcon";
			this.pictureBoxIcon.Size = new System.Drawing.Size(51, 44);
			this.pictureBoxIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxIcon.TabIndex = 2;
			this.pictureBoxIcon.TabStop = false;
			// 
			// labelProgname
			// 
			this.labelProgname.AutoSize = true;
			this.labelProgname.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelProgname.Location = new System.Drawing.Point(108, 40);
			this.labelProgname.Name = "labelProgname";
			this.labelProgname.Size = new System.Drawing.Size(34, 15);
			this.labelProgname.TabIndex = 1;
			this.labelProgname.Text = "RJB";
			// 
			// FormVersion
			// 
			this.AcceptButton = this.buttonExit;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(377, 127);
			this.Controls.Add(this.pictureBoxIcon);
			this.Controls.Add(this.labelProgname);
			this.Controls.Add(this.labelVersion);
			this.Controls.Add(this.buttonExit);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormVersion";
			this.Text = "バージョン";
			this.Load += new System.EventHandler(this.FormVersion_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonExit;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.PictureBox pictureBoxIcon;
		private System.Windows.Forms.Label labelProgname;
	}
}