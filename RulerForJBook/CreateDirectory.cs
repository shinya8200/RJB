﻿using System;
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
	public partial class CreateDirectory : Form
	{
		public bool _createDir; 
		public CreateDirectory()
		{
			InitializeComponent();
		}

		private void buttonTemp_Click(object sender, EventArgs e)
		{
			_createDir = false;
			Close();
		}

		private void buttonCreateDir_Click(object sender, EventArgs e)
		{
			_createDir = true;
			Close();
		}
	}
}
