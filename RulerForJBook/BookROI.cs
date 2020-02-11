using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Text.RegularExpressions;
using System.Xml;
using System.Reflection;



namespace RulerJB
{
	/// <summary>
	/// ROI構造体クラスです
	/// </summary>
	/// <remarks>ToXmlをサポートしない代わりに ToString()のオーバーライドおよびTryParse(),Parse()をサポート</remarks>
	class BookROI: IComparable
	{
		/// <summary>デフォルト設定のためのROI位置指定列挙体です</summary>
		public enum PosInf : int
		{
			/// <summary>左上</summary>
			LeftTop = 0,
			/// <summary>左下</summary>
			LeftBtm,
			/// <summary>右上</summary>
			RightTop,
			/// <summary>右下</summary>
			RightBtm
		};

		/// <summary>ROI情報を保持します</summary>
		private Rectangle _rect;

		/// <summary>保持している矩形領域を取得します</summary>
		public Rectangle Rect { get { return _rect; } }

		/// <summary>基点位置（左上）の位置を保持します</summary>
		public Point BasePoint { 
			get { return _rect.Location; }
			set { _rect = new Rectangle(value, _rect.Size); }
		}

		/// <summary>ROIのサイズを保持します</summary>
		public Size RoiSize
		{
			get { return _rect.Size; }
			set { _rect = new Rectangle(_rect.Location, value); }
		}

		/// <summary>パース用Regexを保持します。最初の使用時に１度だけセットされます</summary>
		static Regex _regForParse = null;


		/// <summary>
		/// デフォルトコンストラクタです
		/// </summary>
		public BookROI():this( new Point(), new Size() )
		{
		}
	

		/// <summary>コンストラクタです</summary>
		/// <param name="bp">基点位置（左上）の位置</param>
		/// <param name="siz">ROIのサイズ</param>
		public BookROI(Point bp, Size siz)
		{
			_rect = new Rectangle(bp, siz);
		}


		/// <summary>コンストラクタです</summary>
		/// <param name="bp">矩形情報</param>
		public BookROI( Rectangle rect )
		{
			_rect = rect;
		}

		/// <summary>
		/// コピーコンストラクタです
		/// </summary>
		/// <param name="original">オリジナル</param>
		public BookROI(BookROI original): this( original.BasePoint, original.RoiSize )
		{
		}


		/// <summary>文字列に変換します</summary>
		/// <returns>変換後の文字列</returns>
		public override string ToString()
		{
			return String.Format("Point({0},{1}) Size({2},{3})", BasePoint.X, BasePoint.Y, RoiSize.Width, RoiSize.Height);
		}



		/// <summary>文字列からパースします</summary>
		/// <param name="str">文字列</param>
		/// <param name="roi">生成したインスタンス</param>
		/// <returns>成否</returns>
		static public bool TryParse(string str, out BookROI roi)
		{
			var ret = false;
			roi = new BookROI();
			try
			{
				if( _regForParse == null )
				{
					_regForParse = new Regex(@"Point\(.*?(\d+)\,.*?(\d+)\) Size\(.*?(\d+)\,.*?(\d+)\)", RegexOptions.Compiled);
				}
				var m = _regForParse.Match(str);
				if (m.Success)
				{
					int bX = int.Parse(m.Groups[1].ToString());
					int bY = int.Parse(m.Groups[2].ToString());
					int ww = int.Parse(m.Groups[3].ToString());
					int hh = int.Parse(m.Groups[4].ToString());
					roi = new BookROI(new Point(bX, bY), new Size(ww, hh));
					ret = true;
				}
			}
			catch (Exception ex)
			{
				var s = String.Format("str={0}, Msg={1}, Stack={2}", str, ex.Message, ex.StackTrace);
				ret = false;
			}
			return ret;
		}


		/// <summary>文字列からインスタンスを生成します（パース）</summary>
		/// <param name="str">文字列</param>
		/// <returns>生成したインスタンス</returns>
		static public BookROI Parse(string str)
		{
			BookROI w;
			if (BookROI.TryParse(str, out w) == true)
			{
				return w;
			}
			throw new Exception("BookROI.Parseに失敗しました");
		}


		/// <summary>
		/// 比較用メソッドです(ICompareable)
		/// </summary>
		/// <param name="obj">比較対象</param>
		/// <returns>大小関係値 0:一致</returns>
		public int CompareTo(object obj)
		{
			BookROI dst = (BookROI)obj;
			int w;
			w = this.BasePoint.X - dst.BasePoint.X;
			if (w != 0) return w;
			w = this.BasePoint.Y - dst.BasePoint.Y;
			if (w != 0) return w;
			w = this.RoiSize.Width - dst.RoiSize.Width;
			if (w != 0) return w;
			w = this.RoiSize.Height - dst.RoiSize.Height;
			if (w != 0) return w;
			return 0;
		}

	
		/// <summary>画像サイズからデフォルト位置をセットするメソッドです</summary>
		/// <param name="imgSize">イメージサイズ</param>
		/// <param name="rX">ベース位置比率 x</param>
		/// <param name="rY">ベース位置比率 y</param>
		/// <param name="rWid">ROI大きさ比率 Width</param>
		/// <param name="rHei">ROI大きさ比率 Height</param>
		/// <param name="posinf"></param>
		public void SetDefaultRoi(Size imgSize, double rX, double rY, double rWid, double rHei, PosInf posinf)
		{
			int w = (int)(rWid * imgSize.Width);
			int h = (int)(rHei * imgSize.Height);
			int ox = (int)(rX * imgSize.Width);
			int oy = (int)(rY * imgSize.Height);
			RoiSize = new Size((int)w, (int)h);
			switch( posinf )
			{
				case PosInf.LeftTop:
					BasePoint= new Point(ox, oy );
					break;
				case PosInf.LeftBtm:
					BasePoint = new Point(ox, imgSize.Height - 1 - oy - h );
					break;
				case PosInf.RightTop:
					BasePoint = new Point(imgSize.Width - 1 - ox - w, oy );
					break;
				case PosInf.RightBtm:
					BasePoint = new Point(imgSize.Width - 1 - ox - w, imgSize.Height - 1 - oy - h);
					break;
			}
		}


		/// <summary>中心点を取得します</summary>
		/// <returns>中心点</returns>
		public Point GetCenterPosition()
		{
			var x = BasePoint.X + RoiSize.Width / 2;
			var y = BasePoint.Y + RoiSize.Height / 2;
			return new Point(x, y);
		}

        /// <summary>
        /// ポイント位置をベース(TopLeft)から反時計回りに返します。
        /// </summary>
        /// <returns>ポイント群</returns>
        public Point[] GetPoints()
        {
            return new Point[]{ 
                new Point( BasePoint.X, BasePoint.Y ), 
                new Point( BasePoint.X, BasePoint.Y+ RoiSize.Height-1 ), 
                new Point( BasePoint.X+ RoiSize.Width-1, BasePoint.Y+ RoiSize.Height-1 ) ,  
                new Point( BasePoint.X+ RoiSize.Width-1, BasePoint.Y ) 
            };
        }

        /// <summary>
        /// GetPointsのポイント情報 (ZIPでを用いて使用）
        /// </summary>
        /// <returns></returns>
        public PosInf[] GetPosInf()
        {
            return new PosInf[]{
                PosInf.LeftTop, PosInf.LeftBtm, PosInf.RightBtm, PosInf.RightTop
            };
        }
	}

}
