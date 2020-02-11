namespace RulerJB
{
	partial class ManifestInput
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
			this.labelTitle = new System.Windows.Forms.Label();
			this.buttonOpen = new System.Windows.Forms.Button();
			this.textBoxURL = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// labelTitle
			// 
			this.labelTitle.AutoSize = true;
			this.labelTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.labelTitle.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelTitle.Location = new System.Drawing.Point(12, 9);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(283, 19);
			this.labelTitle.TabIndex = 0;
			this.labelTitle.Text = "manifest URIを入力してください。";
			// 
			// buttonOpen
			// 
			this.buttonOpen.Location = new System.Drawing.Point(700, 71);
			this.buttonOpen.Name = "buttonOpen";
			this.buttonOpen.Size = new System.Drawing.Size(75, 23);
			this.buttonOpen.TabIndex = 1;
			this.buttonOpen.Text = "開く";
			this.buttonOpen.UseVisualStyleBackColor = true;
			this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
			// 
			// textBoxURL
			// 
			this.textBoxURL.Location = new System.Drawing.Point(16, 43);
			this.textBoxURL.Name = "textBoxURL";
			this.textBoxURL.Size = new System.Drawing.Size(759, 19);
			this.textBoxURL.TabIndex = 2;
			this.textBoxURL.Text = "http";
			// 
			// ManifestInput
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(787, 106);
			this.ControlBox = false;
			this.Controls.Add(this.textBoxURL);
			this.Controls.Add(this.buttonOpen);
			this.Controls.Add(this.labelTitle);
			this.Name = "ManifestInput";
			this.Text = "iiif画像を開く";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelTitle;
		private System.Windows.Forms.Button buttonOpen;
		private System.Windows.Forms.TextBox textBoxURL;
	}
}