using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RulerJB
{
	/// <summary>
	/// 自動測定中のメッセージボックスクラスです
	/// </summary>
	public partial class FormExecAllAutoDetect : Form
	{
		private int _execNo = 0;
		private int _maxCount = 0;


		/// <summary>コンストラクタです</summary>
		public FormExecAllAutoDetect()
		{
			InitializeComponent();

			Clear();
		}

		/// <summary>情報をクリアします</summary>
		public void Clear()
		{
			_execNo = 0;
			_maxCount = 0;
		}

		/// <summary>表示するカウント値を指定します</summary>
		/// <remarks>番号とマックス値を指定する方法です</remarks>
		/// <param name="no"></param>
		/// <param name="max"></param>
		public void SetCount( int no, int max )
		{
			_execNo = no;
			_maxCount = max;
		}


		/// <summary>マックス値を設定します</summary>
		/// <param name="max">マックス値</param>
		public void SetMaxCount(int max )
		{
			_maxCount = max;
			Invalidate();
		}


		///// <summary>カウントアップし、終了チェックを行います</summary>
		///// <returns>終了条件を満たしているときtrue、そうでない場合false</returns>
		//public bool CountUp()
		//{
		//    _execNo++;
		//    Invalidate();
		//    return (_execNo >= _maxCount);
		//}

		/// <summary>描画時の処理です</summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormExecAllAutoDetect_Paint(object sender, PaintEventArgs e)
		{
			if (_maxCount == 0)
			{
				labelMessage.Text = "準備中";
				labelCount.Text = "----- / -----";
			}
			else
			{
				labelMessage.Text = (_execNo < _maxCount) ? "解析／計算中・・・" : "完了";
				labelCount.Text = String.Format("{0,5} / {1,5}", _execNo, _maxCount);
			}
		}
	}
}
