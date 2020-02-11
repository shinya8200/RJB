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
	public partial class FileOpenProgressBar : Form
	{
		public int value { private get;  set; }

		public FileOpenProgressBar()
		{
			InitializeComponent();
		}

		public void UpdateBar()
		{
			progressBarFileOpen.Value = value;
			progressBarFileOpen.Refresh();
		}
	}
}
