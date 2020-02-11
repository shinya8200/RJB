using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace RulerJB
{
	/// <summary>カラー情報表示用フォームクラスです</summary>
	public partial class FormColorImageS : Form
	{
		/// <summary>ScalablePboxのインスタンスを保持します</summary>
		private ScalablePbox _scalePbox;
		private Bitmap _sourceImage;
		private RangeValue<int> _rangeSat = new RangeValue<int>(int.MaxValue, int.MinValue);
		private RangeValue<int> _rangeVal = new RangeValue<int>(int.MaxValue, int.MinValue);
		private int _rangeHueMin = int.MaxValue;
		private int _rangeHueMax = int.MinValue;

		/// <summary>コンストラクタです</summary>
		public FormColorImageS( Bitmap bmp, string title )
		{
			InitializeComponent();
			this.Text = title;
			_sourceImage = bmp;
		}

		/// <summary>フォームロード時の処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void FormColorImageS_Load(object sender, EventArgs e)
		{
			_scalePbox = new ScalablePbox(this, pboxColImage, (mat, rate) =>
			{
				labelDisplayRate.Text = String.Format("{0:###0.0 %}", rate);
				Refresh();
			},
				null,
				OnMouseMove
			);
			//　ScalablePbox規定値・および設定
			_scalePbox.MaxRateMainImage = 100.0f;
			_scalePbox.MinRateMainImage = 0.25f;
			_scalePbox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;	// 固定
			_scalePbox.ChangeMainImage(_sourceImage);
			_scalePbox.SizeChanged();

			// キーを一旦フォームが受け取る
			this.KeyPreview = true;
		}


		/// <summary>マウスカーソル移動時の処理です</summary>
		/// <param name="x">X位置</param>
		/// <param name="y">Y位置</param>
		/// <param name="col">色情報</param>
		/// <param name="isDown">マウスボタン状態（ダウン時真）未使用</param>
		private void OnMouseMove(int x, int y, Color col, bool isDown )
		{
			int h, s, v;
			ImageLib.HsvFromRgb(col.R, col.G, col.B, out h, out s, out v);
			labelMouseCursor.Text= String.Format( "( {0,4},{1,4} )  RGB[{2,3},{3,3},{4,3}],  HSV[{5,3},{6,3},{7,3}]",x, y, col.R, col.G, col.B, h, s, v );

			// SHIFT押し下げ状態ではその範囲を記録し、表示する
			if( (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				// Hueの処理
				if( _rangeHueMax < 0 ){
					_rangeHueMin = _rangeHueMax = h;
				}
				else{

					if( _rangeHueMax >= _rangeHueMin ) {
						// 範囲外
						if( h< _rangeHueMin || h> _rangeHueMax )
						{
							if( (_rangeHueMin+360 - h)% 360 <= (h+360-_rangeHueMax)%360 )
							{
								_rangeHueMin = h;
							}
							else{
								_rangeHueMax = h;
							}
						}
					}
					else{
						_rangeHueMax+= 360;
						if( h< _rangeHueMin ) h+= 360;
						if( h< _rangeHueMin || h> _rangeHueMax )
						{
							if( (_rangeHueMin+360*2 - h)% 360 <= (h+360*2-_rangeHueMax)%360 )
							{
								_rangeHueMin = h;
							}
							else{
								_rangeHueMax = h;
							}
						}

					}
					_rangeHueMin %= 360;
					_rangeHueMax %= 360;

				}
				_rangeSat.SetLimit(s);
				_rangeVal.SetLimit(v);
			}
			else{
				// Shiftが押されていなければクリアする
				_rangeHueMin = int.MaxValue;
				_rangeHueMax = int.MinValue;
				_rangeSat = new RangeValue<int>(int.MaxValue, int.MinValue);
				_rangeVal = new RangeValue<int>(int.MaxValue, int.MinValue);
			}
			if (_rangeHueMax>= 0 && _rangeSat.IsValid && _rangeVal.IsValid)
			{
				labelHsvRange.Text = String.Format("HSV範囲：H({0,3} - {1,3})   S({2,3} - {3,3})   V({4,3} - {5,3}) ", _rangeHueMin%360, _rangeHueMax%360, _rangeSat.Min, _rangeSat.Max, _rangeVal.Min, _rangeVal.Max);
			}
		}

		/// <summary>
		/// ピクチャーボックスをダブルクリックしたときの処理です。（画像をフィットします）
		/// </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void pboxColImage_DoubleClick(object sender, EventArgs e)
		{
			_scalePbox.Zoom();

		}

		/// <summary>
		/// 「画像を保存が選択されたときの処理です」
		/// </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void 画像を保存ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (pboxColImage.Image == null) return;
			var sfd = new SaveFileDialog();

			//はじめのファイル名を指定する
			sfd.FileName = "Image";
			//はじめに表示されるフォルダを指定する
			sfd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			//[ファイルの種類]に表示される選択肢を指定する
			sfd.Filter = "PNGファイル(*.png)|*.png|JPGファイル(*.jpg)|*.jpg|ビットマップファイル(*.bmp)|*.bmp|TIFFファイル(*.tif)|*.tif|すべてのファイル(*.*)|*.*";
			//[ファイルの種類]ではじめに「PNGファイル」が選択されているようにする
			sfd.FilterIndex = 1;
			//タイトルを設定する
			sfd.Title = "保存ファイルを選択してください";
			//ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
			sfd.RestoreDirectory = true;
			//ダイアログを表示する
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				//OKボタンがクリックされたとき
				// 拡張子に強制的にあわせる場合には、ここにコードを入れること（現在はインデックスにより決定される）
				var it = new ImageFormat[] { ImageFormat.Png, ImageFormat.Jpeg, ImageFormat.Bmp, ImageFormat.Tiff };
				ImageFormat fmt = (sfd.FilterIndex > 0 && sfd.FilterIndex < it.Length) ? it[sfd.FilterIndex - 1] : ImageFormat.Png;
				pboxColImage.Image.Save(sfd.FileName, fmt);
			}
		}


		/// <summary>
		/// 「画像をクリップボードにコピー」が選択されたときの処理です
		/// </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void 画像をクリップボードにコピーToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (pboxColImage.Image == null) return;
			Clipboard.SetDataObject(pboxColImage.Image);
		}



		/// <summary>「画画像情報画面で表示」が選択されたときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void 画像情報画面で表示ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var frm = new FormColorImageS((Bitmap)pboxColImage.Image, this.Text + "(Copy)");
			frm.Show();
		}

		/// <summary>
		/// キーが押されたときの処理です </summary>
		/// <param name="sender">送信元</param>
		/// <param name="e">イベント情報</param>
		private void FormColorImageS_KeyDown(object sender, KeyEventArgs e)
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
