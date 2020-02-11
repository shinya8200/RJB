using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Reflection;
// ver 2014/02/11 Ctrl押しているときと同様、マウスの両ボタンが押されている場合には（画像移動・拡縮を行わない）様、変更


namespace RulerJB
{
	/// <summary>
	/// ピクチャーボックスのマウスによる拡大・縮小サポートクラスです
	/// </summary>
	[Serializable]
	public class ScalablePbox:IDisposable
	{
        // メインピクチャーボックスのマウスによる拡大、縮小
        /// <summary> マウスダウンフラグです </summary>
        protected bool _mainPictureBoxMouseDownFlg = false;

        /// <summary>マウスをクリックした位置を保持します</summary>
         protected PointF _mainPictureBoxOldPoint;

		protected ScrollableControl _parentControl = null;

		/// <summary>ピクチャーボックスを保持します</summary>
		protected PictureBox _pictBox = null;

		/// <summary>オリジナルイメージ情報を保持します</summary>
		protected ScalablePboxMainImage _mainImage =null;

		/// <summary>マウスボタン押し下げ状態を保持します</summary>
		protected bool _trueMouseDownFlag = false;

		/// <summary>マウス操作による拡大・縮小率を取得・設定します　(デフォルトは2回で2倍）</summary>
		public float RateNormal	{	get;	set;	}

		/// <summary>マウス操作によるShit押下時の拡大・縮小率を取得・設定します　(デフォルトは10回で2倍)</summary>
		public float RateShift		{	get;	set;	}


		/// <summary>内部で処理（拡縮）しない場合のマウスホイールイベントの通知イベントです</summary>
		public event MouseEventHandler MouseWheel = null;



		// マウスクリック位置の情報コールバックアクション (x,y,Color,isDown,isLeft)
		protected Action<int,int,Color,bool,bool>_actionOnClick = null;

		// マウス移動時位置の情報コールバックアクション (x,y,Color,isDown)
		protected Action<int,int,Color,bool>_actionOnMove = null;

        /// <summary>表示マトリックスが変更されたときの処理です</summary>
        protected Action<Matrix,float> _onChgMat = null;

		/// <summary>描画時のコールバックアクションを保持します</summary>
		protected Action<Graphics, Size> _onDraw;

		/// <summary>表示領域が変更されたときの処理です</summary>
		/// <remarks>Rectangle(画像領域）, bool（全体が表示されている場合true）</remarks>
		protected Action<Rectangle,bool> _actionChangeRect = null;

		/// <summary>表示領域を保持します</summary>
		protected Rectangle _saveDispRect;


		/// <summary>コンストラクタです </summary>
		/// <param name="parentform">親フォームまたはパネルを指定します</param>
		/// <param name="pbox">対象となるピクチャーボックスを指定します</param>
		/// <param name="spbox">ピクチャーボックスを指定します</param>
        /// <param name="onChgMat">表示倍率の変更時に実行される処理を指定します。ない場合はnullを指定します</param>
		/// <param name="actPos">マウスクリック位置の情報コールバックアクション (x,y,Color,isDown,isLeft)</param>
		/// <param name="movPos">マウス移動時位置情報コールバックアクション (x,y,Color,isDown)</param>
		/// <param name="chgRct">表示領域が変更されたときにその領域（画像内座標）とそれが全体(Image.Width,Height)であるかをコールバックします</param>
		/// <param name="onDraw">描画時内部画像への書き込みの際、書き込みGraphicsおよび画像サイズをコールバックします</param>
        public ScalablePbox(ScrollableControl parentform, PictureBox pbox, Action<Matrix,float>onChgMat= null, Action<int,int,Color,bool,bool>actPos= null, Action<int,int,Color,bool>movPos=null,Action<Rectangle,bool>chgRct=null, Action<Graphics,Size>onDraw= null)
		{
			RateNormal = 1.414213562f;			// デフォルト値：拡大・縮小率(2回で2倍）
			RateShift = 1.071773463f;			// デフォルト値：Shit押下時の拡大・縮小率　(10回で2倍)

			_mainImage = new ScalablePboxMainImage(pbox);
            _parentControl = parentform;
			_pictBox = pbox;
			_pictBox.SizeChanged += new System.EventHandler(this.MainPictureBox_SizeChanged);
			_pictBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainPictureBox_MouseDown);
			_pictBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainPictureBox_MouseMove);
			_pictBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainPictureBox_MouseUp);

			_pictBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(MainPictureBox_MouseWheel);
            _onChgMat = (onChgMat != null) ? onChgMat : (Matrix Mat, float rate) => { };  // nullの場合は空処理
			_actionChangeRect = (chgRct != null) ? chgRct : (Rectangle rect, bool isall) => { };  // nullの場合は空処理
			_actionOnClick = actPos;
			_actionOnMove = movPos;
			InterpolationMode = InterpolationMode.NearestNeighbor;		// デフォルトを最近傍法とする。
			_saveDispRect = new Rectangle();
			_onDraw = onDraw;
			
			SizeChanged();
		}



		/// <summary>表示している領域を取得します</summary>
		/// <returns>表示領域（オリジナルデータ座標）</returns>
		public Rectangle GetDispRect()
		{
			// 表示上の座標を オリジナルデータ座標系に変換する。
			var pov = new Point[] { new Point(0, 0), new Point(_pictBox.Width - 1, _pictBox.Height - 1) };
			var mat = (Matrix)_mainImage.Mat.Clone();
			mat.Invert();
			mat.TransformPoints(pov);
			mat.Dispose();
			return new Rectangle(pov[0].X, pov[0].Y, (pov[1].X - pov[0].X + 1), (pov[1].Y - pov[0].Y + 1));
		}


		/// <summary>ピクチャーボックス内の余白サイズを取得します</summary>
		/// <param name="topSpace">上部余白</param>
		/// <param name="leftSpace">左側余白</param>
		/// <param name="bottomSpace">下側余白</param>
		/// <param name="rightSpace">右側余白</param>
		/// <remarks>単位は表示ピクセル</remarks>
		public void GetSpeceSize(out int topSpace, out int leftSpace, out int bottomSpace, out int rightSpace)
		{
			var tlPos = TransXyOrgToDisp(new Point(0, 0));														// 元画像原点の表示位置
			var brPos = TransXyOrgToDisp(new Point(_mainImage.Bmap.Width - 1, _mainImage.Bmap.Height - 1));		// 元画像の右下の表示位置
			topSpace = tlPos.Y <= 0 ? 0 : tlPos.Y;
			leftSpace = tlPos.X <= 0 ? 0 : tlPos.X;
			// 右下位置はいったんピクチャーボックスの最終ピクセル位置との差分を出す
			var bottomW = (_pictBox.Height - 1)- brPos.Y;
			var rightW = (_pictBox.Width - 1) - brPos.X;
			bottomSpace = bottomW <= 0 ? 0 : bottomW;
			rightSpace = rightW <= 0 ? 0 : rightW;
		}


	    /// <summary>メインイメージの描画処理です</summary>
        public void DrawMainImage()
        {
            if (_mainImage== null || _mainImage.Bmap == null) return;

            // 補間モードの設定
			try
			{
				_mainImage.Graph.InterpolationMode = InterpolationMode;
				// アフィン変換行列の設定 
				if (_mainImage.Mat != null)
				{
					_mainImage.Graph.Transform = _mainImage.Mat;
				}
				_mainImage.Graph.Clear(_pictBox.BackColor);             // ピクチャボックスのクリア 
				_mainImage.Graph.DrawImage(_mainImage.Bmap, 0, 0, _mainImage.Bmap.Width, _mainImage.Bmap.Height);      // 描画 
				if( _onDraw != null ) _onDraw( _mainImage.Graph, _mainImage.Bmap.Size ) ;
				_pictBox.Refresh();                                     // 再描画 
			}
			catch (Exception ex)
			{
			}
        }


		/// <summary>現在のビットマップのクローンを返します</summary>
		/// <returns>現在のビットマップのクローン</returns>
		public Bitmap CloneBitmap()
		{
			return ( _mainImage!= null && _mainImage.Bmap!= null ) ? (Bitmap)_mainImage.Bmap.Clone(): null;
		}


		/// <summary>描画領域の通知を行います</summary>
		protected void NoticeDispRect()
		{
			if (_mainImage == null || _mainImage.Bmap == null) return;
			// 描画領域の通知
			var rect = GetDispRect();
			var isAll = (_mainImage.Bmap.Width <= rect.Width) && (_mainImage.Bmap.Height <= rect.Height);
			if (_saveDispRect != rect && _actionChangeRect != null )
			{
				_actionChangeRect(rect, isAll);
				_saveDispRect = rect;
			}
		}


		/// <summary>指定したポイントx,yが描画イメージ上にあるか調べる </summary>
		/// <param name="x">調べるポイントX座標</param>
		/// <param name="y">調べるポイントY座標</param>
		/// <returns>指定したポイントx,yが描画イメージ上にあるときtrue</returns>
		public bool IsPointOnImage(int x, int y)
		{
			if (_mainImage == null) return false;
			return _mainImage.IsPointOnImage( x, y );
		}


        /// <summary>マウスホイールイベント</summary>
        /// <param name="sender">イベント発信元</param>
        /// <param name="e">イベント引数</param>
        protected void MainPictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
			if (_mainImage == null || _mainImage.Bmap == null) return;

//			float rate = (Control.ModifierKeys & Keys.Shift )== Keys.Shift ? RateShift: RateNormal;
//			bool onf = _mainImage.IsPointOnImage(e.X, e.Y);
//			if (!onf) return;               // 画像内にマウスポインタがないときには、拡大・縮小しない
//			onf = _mainImage.ChangeScaleRate(e.X, e.Y, e.Delta > 0 ? rate : 1.0f / rate);  // onfは上で取得したものと同じものだが、こちらのリターン値を使用する。
//            // 画像の描画 
//            ChangedMainImageMatrix(!onf);   // 画像内にマウスポインタがある場合には位置調整を行わない
//            DrawMainImage();
//			// 描画領域の通知
//			NoticeDispRect();
			// Fixモード時およびコントロールキーが押されている場合、右ボタンが押されている（両押し）は拡縮を行わない
			var mf = (_mainImage.IsFixed == false || _mainImage.IsPowerFlag) && (Control.ModifierKeys & Keys.Control) == 0 && (e.Button & MouseButtons.Right) == 0;
			if (mf)
			{
				float rate = (Control.ModifierKeys & Keys.Shift) == Keys.Shift ? RateShift : RateNormal;
				bool onf = _mainImage.IsPointOnImage(e.X, e.Y);
				if (!onf) return;               // 画像内にマウスポインタがないときには、拡大・縮小しない
				onf = _mainImage.ChangeScaleRate(e.X, e.Y, e.Delta > 0 ? rate : 1.0f / rate);  // onfは上で取得したものと同じものだが、こちらのリターン値を使用する。
				// 画像の描画 
				ChangedMainImageMatrix(!onf);   // 画像内にマウスポインタがある場合には位置調整を行わない
				DrawMainImage();
				// 描画領域の通知
				NoticeDispRect();
			}
			else
			{
				// 拡縮を行わなかった場合で、かつイベントハンドラが追加されている場合には呼び出しを行う。
				if (MouseWheel != null)
				{
					MouseWheel(sender, e);
				}
			}
        }

		/// <summary>
		/// 拡大します</summary>
		/// <param name="isSmallChange">微調整</param>
		/// <remarks>マウスホイール同様</remarks>
		public void RatePlus(bool isSmallChange = false)
		{
			float rate = isSmallChange ? RateShift : RateNormal;

			_mainImage.ChangeScaleRate(_pictBox.Width / 2, _pictBox.Height / 2, rate);  // 中心

			// 画像の描画 
			ChangedMainImageMatrix(false);   // 位置調整を行わない
			DrawMainImage();
			// 描画領域の通知
			NoticeDispRect();
		}


		/// <summary>
		/// 縮小します</summary>
		/// <param name="isSmallChange">微調整</param>
		/// <remarks>マウスホイール同様</remarks>
		public void RateMinus(bool isSmallChange = false)
		{
			float rate = isSmallChange ? RateShift : RateNormal;
			_mainImage.ChangeScaleRate(_pictBox.Width / 2, _pictBox.Height / 2, 1 / rate);  // 中心

			// 画像の描画 
			ChangedMainImageMatrix(false);   // 位置調整を行わない
			DrawMainImage();
			// 描画領域の通知
			NoticeDispRect();
		}

		/// <summary>メインイメージのマトリックスに倍率をセットします。</summary>
		/// <param name="rate">拡大率</param>
		/// <remarks>Matrix.Scale()では現在の値に対し、倍率をかけてしまうため。また、Elements[]はgetのみのプロパティなので全体をセットする</remarks>
		public void SetMainMatrixRate(float rate)
        {
			var w = new Matrix(rate, _mainImage.Mat.Elements[1], _mainImage.Mat.Elements[2], rate, _mainImage.Mat.Elements[4], _mainImage.Mat.Elements[5]);
			_mainImage.Mat = w;
			// 描画領域の通知
			NoticeDispRect();
		}



		/// <summary>メインイメージのマトリックスに原点位置をセットします。</summary>
		/// <param name="x">元画像の原点のX位置</param>
		/// <param name="y">元画像の原点のY位置</param>
		/// <remarks>Matrix.Translate()では現在の値に対し、倍率をかけてしまうため。また、Elements[]はgetのみのプロパティなので全体をセットする</remarks>
		protected void SetMainMatrixTranslate(float x, float y)
		{
			var w = new Matrix(_mainImage.Mat.Elements[0], _mainImage.Mat.Elements[1], _mainImage.Mat.Elements[2], _mainImage.Mat.Elements[3], x, y);
			_mainImage.Mat = w;
			// 描画領域の通知
			NoticeDispRect();
		}




        /// <summary>描画方法(_mainMatrix)を変更した時の処理です</summary>
        /// <param name="isAdjust">不都合な位置に表示されていた場合の調整を実施する</param>
        protected void ChangedMainImageMatrix(bool isAdjust= true)
        {
            // 調整を行う場合
            if (isAdjust)
            {
                // 表示位置の調整を行う。
				_mainImage.AdjustImagePosition(_pictBox);
            }
            // Elements[0]: 回転がかかっていないかつ縦横同じ比率で拡大・縮小されているものとする
            _onChgMat(_mainImage.Mat, _mainImage.Mat.Elements[0]);
			// 描画領域の通知
			NoticeDispRect();
		}



		/// <summary>メインピクチャーボックスサイズの変更時処理です</summary>
		/// <param name="sender">イベント発信元</param>
		/// <param name="e">イベント引数</param>
		protected void MainPictureBox_SizeChanged(object sender, EventArgs e)
		{
			SizeChanged();
		}



		/// <summary>イメージサイズの拡大・縮小　（±rate)</summary>
		/// <param name="val">（±rate)</param>
		/// <returns>変更後の倍率</returns>
		public float ImageSizeDif( float val )
		{
			_mainImage.ChangeScaleDif(0, 0, val);		// 原点を中心に+ 5%
			ChangedMainImageMatrix();
			DrawMainImage();
			return _mainImage.Mat.Elements[0];
		}



		/// <summary>オリジナルサイズ(100%)表示にします</summary>
		/// <returns>変更後の倍率</returns>
		public float OriginalSize()
		{
			_mainImage.Mat.Reset();
 			if( _mainImage == null ) return 1.0f;
           // 画像の描画 
			ChangedMainImageMatrix();
			DrawMainImage();
			return _mainImage.Mat.Elements[0];
		}



        /// <summary>
        /// Zoom時の拡大率を計算する
        /// </summary>
        /// <param name="spbox">ピクチャーボックス</param>
        /// <param name="bmp">ビットマップ</param>
       /// <returns>Zoom時の拡大率</returns>
        public static float CalZoomScale(PictureBox pbox, Bitmap bmp )
        {
            return Math.Min((float)pbox.Width / (float)bmp.Width, (float)pbox.Height / (float)bmp.Height);		// 小さい方を使用する
        }



		/// <summary>画面フィット表示(Zoom)にします。</summary>
		/// <returns>変更後の倍率</returns>
		public float Zoom()
		{
            if (_mainImage == null || _mainImage.Bmap== null ) return 0.0f;
			_mainImage.Mat.Reset();
            float rate = CalZoomScale(_pictBox, _mainImage.Bmap );
            _mainImage.Mat.Scale(rate, rate);
            // 画像の描画 
			ChangedMainImageMatrix();
			DrawMainImage();
			return _mainImage.Mat.Elements[0];
		}


		/// <summary>元画像位置(x, y)を表示上座標(xx,yy)に移動する</summary>
		/// <param name="x">データ座標x</param>
		/// <param name="y">データ座標y</param>
		/// <param name="xx">指定表示位置xx</param>
		/// <param name="yy">指定表示位置yy</param>
		/// <param name="rate">倍率（指定なし＝負の値は現在の拡大率）</param>
		public void MoveDispPos(int x, int y, int xx, int yy, float rate= -1.0f)
		{
			_mainImage.MoveDispPos(x, y, xx, yy, rate < 0 ? _mainImage.DispRate: rate);
			// 画像の描画 
			ChangedMainImageMatrix();
			DrawMainImage();
		}


		/// <summary>表示座標系からオリジナルデータ座標系系に変換します</summary>
		/// <param name="dpo">表示座標系ポイント</param>
		/// <returns>オリジナルデータ座標系ポイント</returns>
		public Point TransXyDispToOrg(Point dpo)
		{
			// 表示上の座標を オリジナルデータ座標系に変換する。
			var pov = new Point[] { new Point(dpo.X, dpo.Y) };
			var mat = (Matrix)_mainImage.Mat.Clone();
			mat.Invert();
			mat.TransformPoints(pov);
			return pov[0];
		}

		/// <summary>表示座標系からオリジナルデータ座標系系に変換します</summary>
		/// <param name="dpos">表示座標系ポイント群</param>
		/// <returns>オリジナルデータ座標系ポイント群</returns>
		public Point[] TransXyDispToOrgPoints(Point[] dpos)
		{
			// 表示上の座標を オリジナルデータ座標系に変換する。
			var pov = (Point[])dpos.Clone();
			var mat = (Matrix)_mainImage.Mat.Clone();
			mat.Invert();
			mat.TransformPoints(pov);
			return pov;
		}


		/// <summary>オリジナルデータ座標系から表示座標系に変換します</summary>
		/// <param name="opo">オリジナルデータ座標系ポイント</param>
		/// <returns>表示座標系ポイント</returns>
		public Point TransXyOrgToDisp(Point opo)
		{
			// オリジナルデータ座標系の座標を 表示上の座標系に変換する。
			var pov = new Point[] { new Point(opo.X, opo.Y) };
			_mainImage.Mat.TransformPoints(pov);
			return pov[0];
		}

		/// <summary>オリジナルデータ座標系から表示座標系に変換します</summary>
		/// <param name="opos">オリジナルデータ座標系ポイント群</param>
		/// <returns>表示座標系ポイント群</returns>
		public Point[] TransXyOrgToDispPoints(Point[] opos)
		{
			// オリジナルデータ座標系の座標を 表示上の座標系に変換する。
			var pov = (Point[])opos.Clone();
			_mainImage.Mat.TransformPoints(pov);
			return pov;
		}


        /// <summary>マウスボタンがクリック(DOWN)したときの処理です</summary>
        /// <param name="sender">イベント発信元</param>
        /// <param name="e">イベント引数</param>
        protected void MainPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
			_trueMouseDownFlag = true;
			// フォーカスの設定 
            //（クリックしただけではMouseWheelイベントが有効にならない） 
            this._pictBox.Focus();
            // マウスをクリックした位置の記録 
            _mainPictureBoxOldPoint.X = e.X;
            _mainPictureBoxOldPoint.Y = e.Y;
            // クリック位置→画像位置への変換
            var pt = new Point[] { new Point(e.X, e.Y) };
            var ma = Mat.Clone();
            ma.Invert();
            ma.TransformPoints(pt);
            // クリックした位置が、画像内であれば画像情報をステータスバーに表示する。そうでなければリターンし、ダウンフラグはセットしない。
            if (_mainImage != null && _mainImage.Bmap != null && pt[0].X >= 0 && pt[0].X < _mainImage.Bmap.Width && pt[0].Y >= 0 && pt[0].Y < _mainImage.Bmap.Height)
            {
                Color c = _mainImage.Bmap.GetPixel(pt[0].X, pt[0].Y);
				
				if( _actionOnClick != null ) _actionOnClick( pt[0].X, pt[0].Y, c, true, (e.Button & MouseButtons.Left)!= 0 );
            }
            else
            {
                return;
            }

             // Fixモード時および右ボタンがクリックされているときはスケール処理は何もしない 
			if ((IsFixed && !IsPowerFlag) || e.Button == System.Windows.Forms.MouseButtons.Right) { return; }
			// コントロールキーが押されている場合も無視する。
			if( (Control.ModifierKeys & Keys.Control)!= 0 ){	return;	}

           // マウスダウンフラグ 
            _mainPictureBoxMouseDownFlg = true;
            _pictBox.Cursor = System.Windows.Forms.Cursors.Hand;
        }



        /// <summary>マウスが移動したときの処理です</summary>
        /// <param name="sender">イベント発信元</param>
        /// <param name="e">イベント引数</param>
		protected void MainPictureBox_MouseMove( object sender, MouseEventArgs e )
		{
			if( _mainImage.Bmap == null )
				return;
			// Fixモード時およびコントロールキーが押されている場合、右ボタンが押されている（両押し）は移動処理しない
			var mf = (_mainImage.IsFixed == false || _mainImage.IsPowerFlag ) &&  (Control.ModifierKeys & Keys.Control)== 0 && (e.Button &MouseButtons.Right)== 0;
			if( mf )
			{
				float eXX = e.X;
				float eYY = e.Y;
				// マウスをクリックしながら移動中のとき 
				if( _mainPictureBoxMouseDownFlg == true )
				{
					Point[] px = { new Point( 0, 0 ), new Point( _mainImage.Bmap.Width, _mainImage.Bmap.Height ) };
					_mainImage.Mat.TransformPoints( px );
					// もし、画像の右端がPictureBoxの半分より左側にあったらこれ以上左には移動しない。
					if( px[1].X < _pictBox.Width / 2 && e.X < _mainPictureBoxOldPoint.X )
						eXX = _mainPictureBoxOldPoint.X;
					// もし、画像の左端がPictureBoxの半分より右側にあったらこれ以上右には移動しない。
					if( px[0].X > _pictBox.Width / 2 && e.X > _mainPictureBoxOldPoint.X )
						eXX = _mainPictureBoxOldPoint.X;
					// もし、画像の上端がPictureBoxの半分より下側にあったらこれ以上下には移動しない。
					if( px[0].Y > _pictBox.Height / 2 && e.Y > _mainPictureBoxOldPoint.Y )
						eYY = _mainPictureBoxOldPoint.Y;
					// もし、画像の下端がPictureBoxの半分より上側にあったらこれ以上上には移動しない。
					if( px[1].Y < _pictBox.Height / 2 && e.Y < _mainPictureBoxOldPoint.Y )
						eYY = _mainPictureBoxOldPoint.Y;

					// 画像の移動 
					_mainImage.Mat.Translate( eXX - _mainPictureBoxOldPoint.X, eYY - _mainPictureBoxOldPoint.Y, System.Drawing.Drawing2D.MatrixOrder.Append );

					// 画像の描画 
					DrawMainImage();

					// ポインタ位置の保持 
					_mainPictureBoxOldPoint.X = e.X;
					_mainPictureBoxOldPoint.Y = e.Y;

					// 描画領域の通知（表示領域が変わると想定して）
					NoticeDispRect();
				}
			}
			// コールバック関数が指定されていた場合
			if( _actionOnMove!= null )
			{
				// 左ボタンダウン時以外処理する。
				if( mf== false || _mainPictureBoxMouseDownFlg== false )
				{
					// カーソル位置→画像位置への変換
					var pt = new Point[] { new Point( e.X, e.Y ) };
					var ma = Mat.Clone();
					ma.Invert();
					ma.TransformPoints( pt );
					// 移動した位置が、画像内であれば画像情報をセットする。そうでなければ座標に-1を入れてリターンする。
					if( _mainImage != null && _mainImage.Bmap != null && pt[0].X >= 0 && pt[0].X < _mainImage.Bmap.Width && pt[0].Y >= 0 && pt[0].Y < _mainImage.Bmap.Height )
					{
						Color c = _mainImage.Bmap.GetPixel( pt[0].X, pt[0].Y );
						_actionOnMove(pt[0].X, pt[0].Y, c, 	_trueMouseDownFlag );
					}
					else _actionOnMove(-1, -1, Color.Black, _trueMouseDownFlag );
				}
			}
		}




        /// <summary>マウスボタンをアップしたときの処理です</summary>
		/// <param name="sender">イベント発信元</param>
		/// <param name="e">イベント引数</param>
		/// <remarks>コントロールキーが押されていてもUPは処理する必要がある</remarks>
		protected void MainPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            // マウスダウンフラグ 
			_trueMouseDownFlag = false;
			_mainPictureBoxMouseDownFlg = false;
			_pictBox.Cursor = System.Windows.Forms.Cursors.Default;
			if( _actionOnClick != null )
			{
				_actionOnClick( 0, 0, Color.Black, false, (e.Button & MouseButtons.Left)!= 0 );	// falseの場合、他の情報はセットしない
			}
        }



		/// <summary>メインピクチャーボックスのイメージ変更</summary>
		/// <param name="img"></param>
		public void ChangeMainImage(Bitmap img)
		{
//            DevelopLog.LogINF(MethodBase.GetCurrentMethod(), String.Format("メインのイメージが切り替え。 サイズ({0},{1}), 解像度({2:##0.0},{3:##0.0})", img.Width, img.Height, img.HorizontalResolution, img.VerticalResolution));
            _mainImage.Bmap = img;
			DrawMainImage();
			ChangedMainImageMatrix();
			// 描画領域の通知
			_saveDispRect = new Rectangle();
			NoticeDispRect();
		}



		/// <summary>
		/// ピクチャーボックスサイズ変更時の処理です
		/// </summary>
		public void SizeChanged()
		{
			_mainImage.PboxSizeChanged(_pictBox);
			// 画像の描画 
			DrawMainImage();
			// 描画領域の通知
			NoticeDispRect();
		}



		/// <summary>フィックスモード（固定倍率）の真偽を設定または取得します</summary>
		public bool IsFixed
		{
			get{ return _mainImage.IsFixed;	}
			set{_mainImage.IsFixed = value;	}
		}

		/// <summary>フィックスモードを無視する強制フラグの真偽を設定または取得します</summary>
		public bool IsPowerFlag
		{
			get { return _mainImage.IsPowerFlag; }
			set { _mainImage.IsPowerFlag = value; }
		}



		/// <summary>メインイメージが有効・無効を取得します</summary>
		public bool IsMainImageValid
		{
			get { return _mainImage!= null && _mainImage.Bmap != null; }
		}



		/// <summary>現在の変換マトリックスを取得します</summary>
		public Matrix Mat
		{
			get { return _mainImage.Mat; }
		}


		/// <summary>表示倍率を指定します</summary>
		/// <param name="rate">表示倍率</param>
		public void SetRate(float rate)
		{
			SetMainMatrixRate(rate);
			ChangedMainImageMatrix();
			DrawMainImage();
			// 描画領域の通知
			NoticeDispRect();
		}


		/// <summary>
		/// 中心の座標と表示倍率を指定します。
		/// </summary>
		/// <param name="center">中心座標【クライアント座標】</param>
		/// <param name="rate">倍率</param>
		public void SetRatePos(Point center, float rate)
		{
			bool onf = _mainImage.ChangeScaleRate(center.X, center.Y,rate, true);
			if (!onf) return;               // 画像内にマウスポインタがないときには、拡大・縮小しない
			// 画像の描画 
			ChangedMainImageMatrix(!onf);   // 画像内にマウスポインタがある場合には位置調整を行わない
			DrawMainImage();
			// 描画領域の通知
			NoticeDispRect();
		}


		/// <summary>センター位置を指定します</summary>
		/// <param name="po">中心に表示するデータ座標</param>
		public void SetCenterPos(Point po)
		{
			SetCenterPos(po.X, po.Y);
		}


		/// <summary>センター位置を指定します</summary>
		/// <param name="x">中心に表示するデータ座標 x</param>
		/// <param name="y">中心に表示するデータ座標 y</param>
		public void SetCenterPos(int x, int y)
		{
			// 今の拡大率で中心がデータ(x,y)となるよう表示する。
			_mainImage.MoveCenterPos(x, y);
			DrawMainImage();
			// 描画領域の通知
			NoticeDispRect();
		}



		/// <summary>最大拡大率を設定・取得します</summary>
		public float MaxRateMainImage
		{
			get{	return _mainImage.MaxRate;		}
			set{	_mainImage.MaxRate = value;	}
		}


		/// <summary>最小縮小率を設定・取得します</summary>
		public float MinRateMainImage
		{
			get{	return _mainImage.MinRate;		}
			set{	_mainImage.MinRate = value;	}
		}


		/// <summary>メイン画面拡大・縮小のアルゴリズムを設定・取得します</summary>
		public InterpolationMode InterpolationMode	{	get;	set;	}


		/// <summary>オブジェクトを破棄します</summary>
		public void Dispose()
		{
		}

		
		/// <summary>内包しているベースピクチャーボックスを取得します</summary>
		public PictureBox BasePbox
		{
			get { return _pictBox; }
		}


		/// <summary>表示倍率を取得します</summary>
		public float DispRate
		{
			get
			{
				return _mainImage.DispRate;
			}
		}

	}	


	//---------------- internal class -----------------
	/// <summary>メインフォームで使用するメインイメージクラスです</summary>
	public class ScalablePboxMainImage:IDisposable
	{
        protected Bitmap _bmap = null;
        protected Graphics _graph = null;
        protected Matrix _mat = new Matrix();
        protected PictureBox _pbox = null;
		protected double _maxRateMainImage = 10;		// 10倍
		protected double _minRateMainImage = 0.1;		// 0.1倍

		// ---- プロパティ
		/// <summary>表示されるビットマップデータを設定または取得します</summary>
		public Bitmap Bmap
		{
			get { return _bmap; }
            set
            {
                if (_bmap != null)
                {
                    _bmap.Dispose();
                }
                _bmap = (Bitmap)value.Clone();
                if (IsFixed == false || IsPowerFlag == true)	// 画面サイズFixモードでなければ、Matrixを初期化する
                {
                    ResetMatrix();
                }

            }
		}

        /// <summary>
        /// デフォルトマトリックスを生成する
        /// </summary>
        public void ResetMatrix()
        {
            if (_mat != null) _mat.Dispose();
            _mat = new Matrix();
            if ( true /* _PEND CommonValue.Settings.IsDefaultZoom */)
            {
                float r = ScalablePbox.CalZoomScale(_pbox, _bmap);
                AdjustScaleRate(r);
            }
            // 表示位置の調整を行う。
            AdjustImagePosition(_pbox);
        }

 		/// <summary>描画のためのGraphicsオブジェクトを設定または取得します</summary>
       public Graphics Graph
		{
			get{ return _graph; }
			set{ _graph = value; }
		}
 		/// <summary>表示用マトリックスを設定または取得します</summary>
       public Matrix Mat
		{
			get{ return _mat; }
			set{ _mat = value; }
		}
 		/// <summary>固定倍率フラグを設定または取得します</summary>
		public bool IsFixed{ get; set; }

		/// <summary>フィックスモードを無視する強制フラグの真偽を設定または取得します</summary>
		public bool IsPowerFlag { get; set; }

		/// <summary>最大拡大率を設定・取得します</summary>
		public float MaxRate	{	get;	set;	}


		/// <summary>最小縮小率を設定・取得します</summary>
		public float MinRate	{	get;	set;	}


		/// <summary>表示倍率を取得します</summary>
		public float DispRate
		{
			get { return (_mat != null) ? _mat.Elements[0] : 0.0f; }
		}


		/// <summary>デフォルトコンストラクタです。</summary>
        /// <param name="spbox">ピクチャーボックス</param>
		public ScalablePboxMainImage(PictureBox pbox)
		{
            _pbox = pbox;
		}


		/// <summary>拡大・縮小率を範囲内に入るよう調整し、マトリックスにセットします</summary>
		/// <param name="resr">予想最終倍率</param>
		protected void AdjustScaleRate(float resr)
		{
			// 最大値を超えていたら、最大値になるようratedifをセットします
			if (resr > MaxRate)
			{
				resr = MaxRate;
			}
			// 最小値未満であれば、最小値になるようratedifをセットします
			if (resr < MinRate)
			{
				resr = MinRate;
			}
			float rate = resr / _mat.Elements[0];
			_mat.Scale(rate, rate, System.Drawing.Drawing2D.MatrixOrder.Append);
		}

        /// <summary>指定したポイントx,yが描画イメージ上にあるか調べる </summary>
        /// <param name="x">調べるポイントX座標</param>
        /// <param name="y">調べるポイントY座標</param>
        /// <returns>指定したポイントx,yが描画イメージ上にあるときtrue</returns>
        public bool IsPointOnImage(int x, int y)
        {
            Point[] px = { new Point(0, 0), new Point(_bmap.Width, _bmap.Height) };
            _mat.TransformPoints(px);
            return (x >= px[0].X && x <= px[1].X && y >= px[0].Y && y <= px[1].Y);
        }


		/// <summary>表示倍率を指定された分UpまたはDownします</summary>
		/// <param name="x">中心位置 x</param>
		/// <param name="y">中心位置 y</param>
		/// <param name="ratedif">(拡大率の）増減</param>
        /// <returns>中心位置が画像上にあった場合には真を返す</returns>
        /// <remarks>現在の表示倍率を5%UP （拡大）ならば, ratedif=0.05とします</remarks>
		public bool ChangeScaleDif(int x, int y, float ratedif)
		{
			if (IsFixed && !IsPowerFlag) return false;
            bool onf = IsPointOnImage(x, y);
			// 中心位置→原点へ移動 
			_mat.Translate(-x, -y, System.Drawing.Drawing2D.MatrixOrder.Append);

			AdjustScaleRate(_mat.Elements[0] + ratedif);

			// 原点→ポインタの位置へ移動(元の位置へ戻す) 
			_mat.Translate(x, y, System.Drawing.Drawing2D.MatrixOrder.Append);

            return onf;

		}


		/// <summary>現在の表示を指定倍率拡大します</summary>
		/// <param name="x">中心位置 x</param>
		/// <param name="y">中心位置 y</param>
		/// <param name="rateR">拡大</param>
		/// <param name="powerFlag">強制フラグ(IsFixedでも実行する）</param>
        /// <returns>中心位置が画像上にあった場合には真を返す</returns>
		/// <remarks>現在の表示倍率2倍（拡大）ならば, rateR=2.0f,半分にするときは rateR= 0.5とします</remarks>
		public bool ChangeScaleRate(int x, int y, float rateR, bool powerFlag= false )
		{
			if (Bmap== null) return false;
 			//			float rate1 = 1.41421356f;			// 1回の拡大・縮小率
			if ((IsFixed && !IsPowerFlag) && powerFlag == false) return false;
            bool onf = IsPointOnImage(x, y);
            // 中心位置→原点へ移動 
			_mat.Translate(-x, -y, System.Drawing.Drawing2D.MatrixOrder.Append);

			AdjustScaleRate(_mat.Elements[0] * rateR);

			// 原点→ポインタの位置へ移動(元の位置へ戻す) 
			_mat.Translate(x, y, System.Drawing.Drawing2D.MatrixOrder.Append);
            return onf;
		}


		/// <summary>元画像位置(x, y)を中央に移動する</summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void MoveCenterPos(int x, int y)
		{
			var rate = _mat.Elements[0];		// 縦横同比率が前提
			MoveDispPos(x, y, _pbox.Width / 2, _pbox.Height / 2, rate);
		}


		/// <summary>元画像位置(x, y)を表示上座標(xx,yy)に移動する</summary>
		/// <param name="x">データ座標x</param>
		/// <param name="y">データ座標y</param>
		/// <param name="xx">指定表示位置xx</param>
		/// <param name="yy">指定表示位置yy</param>
		/// <remarks>倍率は現在の倍率</remarks>
		public void MoveDispPos(int x, int y, int xx, int yy, float rate )
		{
			_mat.Reset();
			_mat.Translate(-x, -y, MatrixOrder.Append);
			_mat.Scale(rate, rate, MatrixOrder.Append);			// 縦横同比率が前提
			_mat.Translate(xx, yy, MatrixOrder.Append );
		}


        /// <summary>表示位置を調整する</summary>
        public void AdjustImagePosition(PictureBox Pbox)
        {
 			if (Bmap== null) return;
			if (IsFixed && !IsPowerFlag) return;            // Fixモードの場合には調整を行わない
           float rate = _mat.Elements[0];  // 倍率
            if ( false /*CommonValue.Settings.IsDefaultCentering*/)
            {
                // X方向
                int difX = Pbox.Width - (int)((float)Bmap.Width * rate);
                if (difX < 0) difX = 0;
                // Y方向
                int difY = Pbox.Height - (int)((float)Bmap.Height * rate);
                if (difY < 0) difY = 0;
                // 平行移動
                SetMainMatrixTranslate(difX / 2, difY / 2);
            }
            else
            {
                // センタリングが指定されていない場合には左上に寄せる
                // 表示幅がPictureBoxより小さい場合には、左端に寄せる。
                if (_bmap.Width * _mat.Elements[0] < Pbox.Width)
                {
                    SetMainMatrixTranslate(0, _mat.Elements[5]);        // X->0, Yそのまま
                }
                // 表示高さがPictureBoxより小さい場合には、上端に寄せる。
                if (_bmap.Height * _mat.Elements[0] < Pbox.Height)
                {
                    SetMainMatrixTranslate(_mat.Elements[4], 0);        // Xそのまま, Y->0
                }
            }
        }


		/// <summary>メインイメージのマトリックスに原点位置をセットします。</summary>
		/// <param name="x">元画像の原点のX位置</param>
		/// <param name="y">元画像の原点のY位置</param>
		/// <remarks>Matrix.Translate()では現在の値に対し、倍率をかけてしまうため。また、Elements[]はgetのみのプロパティなので全体をセットする</remarks>
		protected void SetMainMatrixTranslate(float x, float y)
		{
			var w = new Matrix(_mat.Elements[0], _mat.Elements[1], _mat.Elements[2], _mat.Elements[3], x, y);
			_mat.Dispose();
			_mat = w;
		}

		/// <summary>メインピクチャーボックスサイズの変更時処理です</summary>
		public void PboxSizeChanged(PictureBox pbox)
		{
			try
			{
				// PictureBoxと同じ大きさのBitmapクラスを作成する。 
				var newbmp = new Bitmap( pbox.Width, pbox.Height, PixelFormat.Format24bppRgb );
				// 空のBitmapをPictureBoxのImageに指定する。 
				pbox.Image = newbmp;
			}
			catch( Exception ex )
			{
//				DevelopLog.LogException( MethodBase.GetCurrentMethod(), "サイズ変更時に新ビットマップ作成に失敗", ex );
				return;
			}
			if (_graph != null)
			{
				_mat = _graph.Transform;
				_graph.Dispose();
				_graph = null;
			}

			// Graphicsオブジェクトの作成(FromImageを使う) 
			_graph = Graphics.FromImage(pbox.Image);

			// アフィン変換行列の設定 
			if (_mat != null)
			{
				_graph.Transform = _mat;
			}
		}

		/// <summary>
		/// インスタンスを破棄します
		/// </summary>
		public void Dispose()
		{
		}

	}


}


