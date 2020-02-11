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
	/// ピクセルサイズ設定フォームです
	/// </summary>
	public partial class FormAdjustPixelSize : Form
	{
		/// <summary>カーソル選択状態を示します</summary>
		private enum Ppm : int
		{
			/// <summary>非選択状態 </summary>
			Non = 0,
			/// <summary>開始点選択状態 </summary>
			Bgn,
			/// <summary>終了点選択状態 </summary>
			End,
			/// <summary>全体選択状態 </summary>
			All
		}


		/// <summary>画像イメージのクローンを保持します</summary>
		private Bitmap _image;

		/// <summary>ピクチャーボックスの拡大・縮小処理クラスインスタンス</summary>
		private ScalablePbox _scalePbox = null;

		/// <summary>1mm当たりのピクセル数を取得します</summary>
		public double PixelPerMm { get; private set; }

		/// <summary>紙高を取得します（単位mm）</summary>
		public double HeightPaper { get; private set; }

		/// <summary>紙高方式か否かを取得します（true:紙高 false:1mm当たりピクセル数)</summary>
		/// <remarks>真の場合にはプロパティ HeightPaper, そうでない場合には PixelPerMmを使用して匡郭高を算出します。</remarks>
		public bool IsUsePaperHeight { get { return rbtnHeightP.Checked; } }

		/// <summary>カーソルを保持します</summary>
		private ShapeLine _ppmCursor;

		/// <summary>カーソル選択状態を保持します</summary>
		private Ppm _ppmMode = Ppm.Non;

		/// <summary> 表示上で、距離２乗値がこの値以内であれば「近い」と判断します </summary>
		private const int JudgeLimit = 20 * 20;             // 半径20ピクセル

		/// <summary>コンストラクタ</summary>
		/// <param name="image">描画イメージです</param>
		/// <param name="isUseHeightPaper">表示デフォルト値－紙高方式か否か</param>
		/// <param name="heightpaper">表示デフォルト値－紙高mm</param>
		/// <param name="pixelpermm">表示デフォルト値－1mm当たりピクセル数</param>
		public FormAdjustPixelSize(Bitmap image, bool isUseHeightPaper, double heightpaper, double pixelpermm)
		{
			InitializeComponent();

			_image = (Bitmap)image.Clone();
			_ppmCursor = new ShapeLine(new Point(_image.Width / 2, _image.Height * 1 / 5), new Point(_image.Width / 2, _image.Height * 4 / 5));

			// 表示デフォルト値
			if (isUseHeightPaper)
			{
				rbtnHeightP.Checked = true;
				rbtnImage.Checked = false;
				rbtnPxPerMm.Checked = false;
			}
			else
			{
				rbtnHeightP.Checked = false;
				rbtnImage.Checked = false;
				rbtnPxPerMm.Checked = true;
			}
			HeightPaper = heightpaper;
			PixelPerMm = pixelpermm;
			tboxHeightP.Text = HeightPaper.ToString("0.000");
			tboxPxPerMm.Text = PixelPerMm.ToString("0.00000");
		}



		/// <summary>ロード時の処理です</summary>
		/// <param title="sender">送信元</param>
		/// <param title="e">イベント情報</param>
		private void FormAdjustPixelSize_Load(object sender, EventArgs e)
		{
			// ピクチャーボックスの拡大縮小対応
			_scalePbox = new ScalablePbox(this, pictboxAdjustPix, (mat, rate) =>
			{
				// 情報表示
				labelDisplayRate.Text = String.Format("{0:###0.0 %}", rate);
				Refresh();
			},
				OnPboxClick,
				OnPboxMove
			);
			//　ScalablePbox規定値・および設定
			_scalePbox.MaxRateMainImage = 10.0f;        // 10倍
			_scalePbox.MinRateMainImage = 0.025f;           // 0.025倍
			_scalePbox.InterpolationMode = CommonValues.ConfigData.ImageInterpolationMode;

			_scalePbox.ChangeMainImage(_image);
			_scalePbox.IsFixed = false;

			// キーを一旦フォームが受け取る
			this.KeyPreview = true;
		}


		/// <summary>ピクチャーボックス(ScalablePbox)のクリック時の処理です</summary>
		/// <param title="x">元データのX座標</param>
		/// <param title="y">元データのY座標</param>
		/// <param title="col">表示している位置の色情報</param>
		/// <param title="down">ボタン・ダウンの場合真</param>
		/// <param title="isLeft">左ボタンの場合真</param>
		private void OnPboxClick(int x, int y, Color col, bool down, bool isLeft)
		{
			// イメージ設定以外が選択されているときはカーソル描画しないため、クリック処理しない
			if (rbtnImage.Checked == false)
			{
				_ppmMode = Ppm.Non;
				return;
			}

			// 左バタンクリックでかつコントロールが押されている場合のみポイントを選択できる
			if (isLeft == true && (Control.ModifierKeys & Keys.Control) != 0)
			{

				if (_ppmMode == Ppm.Non)
				{
					if (IsNearPos(_ppmCursor.PosBgn, x, y))
					{
						_ppmMode = Ppm.Bgn;
					}
					else if (IsNearPos(_ppmCursor.PosEnd, x, y))
					{
						_ppmMode = Ppm.End;
					}
				}
			}
			else
			{
				_ppmMode = Ppm.Non;
			}


			pictboxAdjustPix.Refresh();

		}



		/// <summary>ピクチャーボックス(ScalablePbox)の移動時の処理です</summary>
		/// <param title="x">元データのX座標</param>
		/// <param title="y">元データのY座標</param>
		/// <param title="col">表示している位置の色情報</param>
		/// <param title="down">ボタン・ダウンの場合真</param>
		private void OnPboxMove(int x, int y, Color col, bool down)
		{
			// イメージ設定以外が選択されているときはカーソル描画しないため、移動処理なし
			if (rbtnImage.Checked == false)
			{
				_ppmMode = Ppm.Non;
				return;
			}

			// ボタンが押されていないか、コントロールが押されたままでなければ、移動モードは解除
			if (down == false || (Control.ModifierKeys & Keys.Control) == 0) _ppmMode = Ppm.Non;

			// カーソル移動処理
			if (_ppmMode == Ppm.Bgn)
			{
				_ppmCursor.PosBgn.X = x;
				_ppmCursor.PosBgn.Y = y;

			}
			else if (_ppmMode == Ppm.End)
			{
				_ppmCursor.PosEnd.X = x;
				_ppmCursor.PosEnd.Y = y;
			}

			this.Refresh();
		}


		/// <summary>クローズ時の処理です</summary>
		/// <param title="sender">送信元</param>
		/// <param title="e">イベント情報</param>
		private void FormAdjustPixelSize_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_image != null)
			{
				_image.Dispose();
				_image = null;
			}
		}


		/// <summary>フォームペイント時の処理です</summary>
		/// <param title="sender">送信元</param>
		/// <param title="e">イベント情報</param>
		private void FormAdjustPixelSize_Paint(object sender, PaintEventArgs e)
		{
			// コントロール群の表示調整
			tboxCurLength.Enabled = rbtnImage.Checked;
			tboxPxPerMm.Enabled = rbtnPxPerMm.Checked;
			tboxHeightP.Enabled = rbtnHeightP.Checked;
			tboxDpi.Enabled = rbtnDpi.Checked;

			double cl;
			if (double.TryParse(tboxCurLength.Text, out cl) == false)
			{
				cl = 0.0;
			}

			labelCur.Text = String.Format("{0}   長さ={1} Pix", _ppmCursor.ToStringPad(4), _ppmCursor.GetDistance());

			// イメージ（カーソル）から算出する場合、1mm当たりのピクセル数を常に更新
			if (rbtnImage.Checked)
			{
				// 0.1未満はあり得ないものとして、０に設定する
				if (cl < 0.1) cl = 0.0;
				tboxPxPerMm.Text = String.Format("{0:##0.00000}", (cl >= 0.1) ? (double)(_ppmCursor.GetDistance() / cl) : 0.0);
			}

			if (rbtnDpi.Checked)
			{
				if (cl < 0.1) cl = 0.0;
				tboxPxPerMm.Text = String.Format("{0:##0.00000}", (cl >= 0.1) ? (double.Parse(tboxDpi.Text) / 25.4) : 0.0);
			}

			double ph;
			if (double.TryParse(tboxHeightP.Text, out ph) == false)
			{
				ph = 0.0;
			}
			HeightPaper = ph;
		}


		/// <summary>カーソル設定ラジオボタンが変更されたときの処理です</summary>
		/// <param title="sender">送信元</param>
		/// <param title="e">イベント情報</param>
		private void rbtnImage_CheckedChanged(object sender, EventArgs e)
		{
			Refresh();
		}


		/// <summary>ピクセルサイズ手入力ラジオボタンが変更されたときの処理です</summary>
		/// <param title="sender">送信元</param>
		/// <param title="e">イベント情報</param>
		private void rbtnPxPerMm_CheckedChanged(object sender, EventArgs e)
		{
			Refresh();
		}

		/// <summary>解像度手入力ラジオボタンが変更されたときの処理です</summary>
		/// <param title="sender">送信元</param>
		/// <param title="e">イベント情報</param>
		private void rbtnDpi_CheckedChanged(object sender, EventArgs e)
		{
			Refresh();
		}

		/// <summary>解像度手入力テキストボックスが変更されたときの処理です</summary>
		/// <param title="sender">送信元</param>
		/// <param title="e">イベント情報</param>
		private void tboxDpi_TextChanged(object sender, EventArgs e)
		{
			Refresh();
		}

		/// <summary>カーソルを描画します</summary>
		/// <param name="gra">描画先グラフィック</param>
		/// <param name="isSelect">選択状態</param>
		private void DrawCursor(Graphics gra, bool isSelect = false)
		{
			// イメージ設定以外が選択されているときはカーソル描画しない
			if (rbtnImage.Checked == false) return;

			var penCur = isSelect ? new Pen(Color.Orange, 6.0f) : new Pen(Color.Red, 6.0f);
			penCur.StartCap = penCur.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
			var posB = _scalePbox.TransXyOrgToDisp(_ppmCursor.PosBgn);
			var posE = _scalePbox.TransXyOrgToDisp(_ppmCursor.PosEnd);
			gra.DrawLine(penCur, posB, posE);

			// 補助線
			var pen = new Pen(Color.Magenta, 1.0f);
			var len = _ppmCursor.GetDistance() / 4;     // 高さの 1/4
			var uv = _ppmCursor.GetUnitVector();        // 単位ベクトル
			var uvN = new PointF((float)(uv.Y * len), (float)(-uv.X * len));            // 垂直なベクトル
			var bp0 = _scalePbox.TransXyOrgToDisp(new Point((int)(_ppmCursor.PosBgn.X + uvN.X), (int)(_ppmCursor.PosBgn.Y + uvN.Y)));
			var bp9 = _scalePbox.TransXyOrgToDisp(new Point((int)(_ppmCursor.PosBgn.X - uvN.X), (int)(_ppmCursor.PosBgn.Y - uvN.Y)));
			var be0 = _scalePbox.TransXyOrgToDisp(new Point((int)(_ppmCursor.PosEnd.X + uvN.X), (int)(_ppmCursor.PosEnd.Y + uvN.Y)));
			var be9 = _scalePbox.TransXyOrgToDisp(new Point((int)(_ppmCursor.PosEnd.X - uvN.X), (int)(_ppmCursor.PosEnd.Y - uvN.Y)));
			gra.DrawLine(pen, bp0, bp9);
			gra.DrawLine(pen, be0, be9);
		}


		/// <summary>ピクチャーボックスの描画時の処理です</summary>
		/// <param title="sender">送信元</param>
		/// <param title="e">イベント情報</param>
		private void pictboxAdjustPix_Paint(object sender, PaintEventArgs e)
		{
			DrawCursor(e.Graphics, _ppmMode != Ppm.Non);
		}


		/// <summary>「設定」ボタンがクリックされたときの処理です</summary>
		/// <param title="sender">送信元</param>
		/// <param title="e">イベント情報</param>
		private void btnSet_Click(object sender, EventArgs e)
		{
			double w;
			if (double.TryParse(tboxPxPerMm.Text, out w) == false)
			{
				MessageBox.Show("不適当な値です", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			PixelPerMm = w;
			DialogResult = DialogResult.OK;
			Close();
		}


		/// <summary>「キャンセル」ボタンがクリックされたときの処理です</summary>
		/// <param title="sender">送信元</param>
		/// <param title="e">イベント情報</param>
		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}


		/// <summary>テキストボックスにフォーカスが移動したときの処理です</summary>
		/// <param title="sender">送信元</param>
		/// <param title="e">イベント情報</param>
		private void OnTboxSelectEnter(object sender, EventArgs e)
		{
			((TextBox)sender).SelectAll();
		}


		/// <summary>ピクチャーボックスがダブルクリックされたときの処理です。</summary>
		/// <param title="sender">送信元</param>
		/// <param title="e">イベント情報</param>
		private void pictboxAdjustPix_DoubleClick(object sender, EventArgs e)
		{
			// 画面の大きさにフィット(Fit)させます
			_scalePbox.Zoom();

			Refresh();
		}


		/// <summary>P0とP1が「近い」かどうか判定します</summary>
		/// <param name="p0">ポイント１＜データ座標＞</param>
		/// <param name="x1">ポイント２ x ＜データ座標＞</param>
		/// <param name="y1">ポイント２ y ＜データ座標＞</param>
		/// <returns>ポイント１，２が近い場合、trueを返します</returns>
		private bool IsNearPos(Point p0, int x1, int y1)
		{
			var lmt = JudgeLimit / (_scalePbox.DispRate * _scalePbox.DispRate);
			return ((p0.X - x1) * (p0.X - x1) + (p0.Y - y1) * (p0.Y - y1) <= lmt);
		}

		/// <summary>
		/// キーが押されたときの処理です </summary>
		/// <param name="sender">送信元</param>
		/// <param name="e">イベント情報</param>
		private void FormAdjustPixelSize_KeyDown(object sender, KeyEventArgs e)
		{
			//　コントロールが押されているとき。
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
			{
				switch (e.KeyCode)
				{
					case Keys.Add:
					case Keys.Oemplus:
						_scalePbox.RatePlus(true);
						e.Handled = true;           // 以下のコントロールにキーを渡さない
						break;
					case Keys.Subtract:
					case Keys.OemMinus:
						_scalePbox.RateMinus(true);
						e.Handled = true;           // 以下のコントロールにキーを渡さない
						break;
				}
			}
			else
			{
				// Ctrl + キーによるショートカット
				switch (e.KeyCode)
				{
					case Keys.Add:
					case Keys.Oemplus:
						_scalePbox.RatePlus(false);
						e.Handled = true;           // 以下のコントロールにキーを渡さない
						break;
					case Keys.Subtract:
					case Keys.OemMinus:
						_scalePbox.RateMinus(false);
						e.Handled = true;           // 以下のコントロールにキーを渡さない
						break;
				}
			}
		}

	}
}
