using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace RulerJB
{
	/// <summary>線分を表すクラスです</summary>
	class ShapeLine
	{
		/// <summary>始点</summary>
		public Point PosBgn;

		/// <summary>終点</summary>
		public Point PosEnd;

		/// <summary>パース用Regexを保持します。最初の使用時に１度だけセットされます</summary>
		static Regex _regForParse = null;


		/// <summary>コンストラクタです</summary>
		public ShapeLine()
		{
		}

		/// <summary>コンストラクタです</summary>
		/// <param name="beginPoint">始点</param>
		/// <param name="endPoint">終点</param>
		public ShapeLine(Point beginPoint, Point endPoint)
		{
			PosBgn = beginPoint;
			PosEnd = endPoint;
		}

		/// <summary>線分の長さを取得します</summary>
		/// <returns>線分の長さ</returns>
		public double GetDistance()
		{
			return Math.Sqrt((PosEnd.X - PosBgn.X) * (PosEnd.X - PosBgn.X) + (PosEnd.Y - PosBgn.Y) * (PosEnd.Y - PosBgn.Y));
		}

		/// <summary>単位ベクトルを取得します</summary>
		/// <returns></returns>
		public PointF GetUnitVector()
		{
			var len = GetDistance();
			return new PointF((float)((PosEnd.X - PosBgn.X) / len), (float)((PosEnd.Y - PosBgn.Y) / len));
		}

		/// <summary>座標をフォーマットした文字列を取得します</summary>
		/// <param name="pad">パディングサイズ PadLeft(pad)</param>
		/// <returns>座標文字列</returns>
		public string ToStringPad(int pad)
		{
			return String.Format("({0},{1})-({2},{3})", PosBgn.X.ToString().PadLeft(pad), PosBgn.Y.ToString().PadLeft(pad), PosEnd.X.ToString().PadLeft(pad), PosEnd.Y.ToString().PadLeft(pad));
		}


		/// <summary>指定した点との距離</summary>
		/// <param name="pos">指定店</param>
		/// <returns>距離</returns>
		public double DistanceLineToPoint(Point pos)
		{
			var dx = PosEnd.X - PosBgn.X;
			var dy = PosEnd.Y - PosBgn.Y;
			if (dx == 0 && dy == 0)	return Math.Sqrt((pos.X - PosBgn.X) * (pos.X - PosBgn.X) + (pos.Y - PosBgn.Y) * (pos.Y - PosBgn.Y));
			var r2 = dx * dx + dy * dy;
			var t = (dx * (pos.X - PosBgn.X) + dy * (pos.Y - PosBgn.Y)) / (double)r2;
			if (t < 0)	return Math.Sqrt((pos.X - PosBgn.X) * (pos.X - PosBgn.X) + (pos.Y - PosBgn.Y) * (pos.Y - PosBgn.Y));
			if (t > 1)	return Math.Sqrt((pos.X - PosEnd.X) * (pos.X - PosEnd.X) + (pos.Y - PosEnd.Y) * (pos.Y - PosEnd.Y));
			var cx = (1 - t) * PosBgn.X + t * PosEnd.X;
			var cy = (1 - t) * PosBgn.Y + t * PosEnd.Y;
			return Math.Sqrt((pos.X - cx) * (pos.X - cx) + (pos.Y - cy) * (pos.Y - cy));
		}


		/// <summary>文字列に変換します</summary>
		/// <returns>変換後の文字列</returns>
		public override string ToString()
		{
			return String.Format("({0},{1})-({2},{3})", PosBgn.X, PosBgn.Y, PosEnd.X, PosEnd.Y);
		}



		/// <summary>文字列からパースします</summary>
		/// <param name="str">文字列</param>
		/// <param name="sl">生成したインスタンス</param>
		/// <returns>成否</returns>
		static public bool TryParse(string str, out ShapeLine sl)
		{
			var ret = false;
			sl = new ShapeLine();
			try
			{
				if( _regForParse == null )
				{
					_regForParse = new Regex(@"\(.*?(\d+)\,.*?(\d+)\)\-\(.*?(\d+)\,.*?(\d+)\)", RegexOptions.Compiled);
				}
				var m = _regForParse.Match(str);
				if (m.Success)
				{
					int bX = int.Parse(m.Groups[1].ToString());
					int bY = int.Parse(m.Groups[2].ToString());
					int eX = int.Parse(m.Groups[3].ToString());
					int eY = int.Parse(m.Groups[4].ToString());
					sl = new ShapeLine(new Point(bX, bY), new Point(eX, eY));
					ret = true;
				}
			}
			catch( Exception ex )
			{
				var s = String.Format("str={0}, Msg={1}, Stack={2}", str, ex.Message, ex.StackTrace);
				ret = false;
			}
			return ret;
		}


		/// <summary>文字列からインスタンスを生成します（パース）</summary>
		/// <param name="str">文字列</param>
		/// <returns>生成したインスタンス</returns>
		static public ShapeLine Parse(string str)
		{
			ShapeLine sl;
			if (ShapeLine.TryParse(str, out sl) == true)
			{
				return sl;
			}
			throw new Exception("ShapeLine.Parseに失敗しました");
		}


		/// <summary>角度を取得します。（Degree）</summary>
		/// <returns>角度 (Degree) </returns>
		public double GetAngle()
		{
			int dx = PosEnd.X - PosBgn.X;
			int dy = PosEnd.Y - PosBgn.Y;
			double radian = Math.Atan2(dy, dx);
			return radian * (180 / Math.PI);
		}



		#region			ユーティリティメソッド ----------------------------------------

		/// <summary>２つのポイントを足し算したインスタンスを取得します</summary>
		/// <param name="p0">ポイント０</param>
		/// <param name="p1">ポイント１</param>
		/// <returns>結果ポイント</returns>
		static public Point AddPoint( Point p0, Point p1 )
		{
			return new Point(p0.X + p1.X, p0.Y + p1.Y);
		}

		/// <summary>２つのポイントを引き算したインスタンスを取得します ps - pd</summary>
		/// <param name="ps">ポイントソース</param>
		/// <param name="pd">ポイントディスティネーション</param>
		/// <returns>結果ポイント</returns>
		static public Point SubPoint(Point ps, Point pd)
		{
			return new Point(ps.X - pd.X, ps.Y - pd.Y);
		}

		/// <summary>Point →　PointF に変換します</summary>
		/// <param name="p">変換元ポイント</param>
		/// <returns>変換後PointF</returns>
		static public PointF ConvPointF(Point p)
		{
			return new PointF((float)p.X, (float)p.Y);
		}

		/// <summary>PointF →　Point に変換します</summary>
		/// <param name="p">変換元ポイント</param>
		/// <returns>変換後PointF</returns>
		/// <remarks>float0->intへの変換はは切り捨て</remarks>
		static public Point ConvPoint(PointF p)
		{
			return new Point((int)p.X, (int)p.Y);
		}


		/// <summary>ポイントを定数倍します</summary>
		/// <param name="p">ポイント（ベクトル）</param>
		/// <param name="r">倍率</param>
		/// <returns></returns>
		static public PointF MulPointF(PointF p, float r )
		{
			return new PointF(p.X * r, p.Y * r);
		}


		/// <summary>ポイント（ベクトル）の距離を算出します</summary>
		/// <param name="vct">ポイント（ベクトル）</param>
		/// <returns></returns>
		static public double CalcDistance(PointF vct)
		{
			return Math.Sqrt(vct.X * vct.X + vct.Y * vct.Y);
		}


		/// <summary>単位ベクトルを取得します </summary>
		/// <returns></returns>
		public PointF UnitVector()
		{
			var w = ShapeLine.SubPoint(PosEnd, PosBgn);
			if( w.X == 0 && w.Y== 0 ) return new PointF( 0 , 0 );

			var vct = ShapeLine.ConvPointF(w ) ;
			var dist = ShapeLine.CalcDistance( vct );
			return ShapeLine.MulPointF(vct, (float)(1.0 / dist));

		}
		#endregion //	ユーティリティメソッド ----------------------------------------

	}
}
