namespace RulerJB
{
	partial class FormExecAllAutoDetect
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExecAllAutoDetect));
			this.labelMessage = new System.Windows.Forms.Label();
			this.labelCount = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelMessage
			// 
			this.labelMessage.AutoSize = true;
			this.labelMessage.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelMessage.Location = new System.Drawing.Point(53, 32);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(43, 15);
			this.labelMessage.TabIndex = 0;
			this.labelMessage.Text = "label1";
			// 
			// labelCount
			// 
			this.labelCount.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelCount.Location = new System.Drawing.Point(12, 98);
			this.labelCount.Name = "labelCount";
			this.labelCount.Size = new System.Drawing.Size(438, 23);
			this.labelCount.TabIndex = 1;
			this.labelCount.Text = "label1";
			this.labelCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// FormExecAllAutoDetect
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(455, 197);
			this.ControlBox = false;
			this.Controls.Add(this.labelCount);
			this.Controls.Add(this.labelMessage);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormExecAllAutoDetect";
			this.Text = "自動測定ダイアログ";
			this.TopMost = true;
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormExecAllAutoDetect_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelMessage;
		private System.Windows.Forms.Label labelCount;
	}
}