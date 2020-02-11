using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RulerJB
{
	public partial class ManifestInput : Form
	{
		private string _url;

		public ManifestInput()
		{
			InitializeComponent();
		}

		public string FileURL { get { return _url; } }

		private void buttonOpen_Click(object sender, EventArgs e)
		{
			_url = textBoxURL.Text;
			this.Close();
		}
	}
}
