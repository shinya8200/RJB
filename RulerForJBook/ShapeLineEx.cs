using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace RulerJB
{
	class ShapeLineEx : ShapeLine
	{
		// 始点、終点での垂線の任意の点を取るためのｘ座標
		private const int _x0 = 0;
		private const int _x1 = 1000;

		// 測定位置合わせOnの場合に、同時に動かす矢印
		public ShapeLineEx _link;

		// 元の線の座標、直線式計算用
		private Point _saveBegin;
		private Point _saveEnd;

		/// <summary>コンストラクタです</summary>
		/// <param name="beginPoint">始点</param>
		/// <param name="endPoint">終点</param>
		/// <param name="lnk">同時に動かすライン</param>
		public ShapeLineEx(Point beginPoint, Point endPoint, ShapeLineEx lnk = null) : base(beginPoint, endPoint)
		{
			_link = lnk;
			_saveBegin = beginPoint;
			_saveEnd = endPoint;
		}

		/// <summary>コンストラクタです</summary>
		/// <param name="sl">選択したライン</param>
		/// <param name="lnk">同時に動かすライン</param>
		public ShapeLineEx(ShapeLine sl, ShapeLineEx lnk = null) : base(sl.PosBgn, sl.PosEnd)
		{
			_link = lnk;
			_saveBegin = sl.PosBgn;
			_saveEnd = sl.PosEnd;
		}

		/// <summary>
		/// 4点から交点の座標を計算する</summary>
		/// <param name="p00">p1(p3の対角）</param>
		/// <param name="p10">p2</param>
		/// <param name="p01">p3</param>
		/// <param name="p11">p4</param>
		/// <returns>交点座標</returns>
		public Point IntersectionFrom4P(Point p00, Point p01, Point p10, Point p11)
		{
			// 面積S1　= {(P4.X - P2.X) * (P1.Y - P2.Y) - (P4.Y - P2.Y) * (P1.X - P2.X)} / 2
			// 面積S2　= {(P4.X - P2.X) * (P2.Y - P3.Y) - (P4.Y - P2.Y) * (P2.X - P3.X)} / 2 
			// 交点の座標は
			// C1.X　= P1.X + (P3.X - P1.X) * S1 / (S1 + S2)
			// C1.Y　= P1.Y + (P3.Y - P1.Y) * S1 / (S1 + S2)

			double s1 = (double)((p11.X - p10.X) * (p00.Y - p10.Y) - (p11.Y - p10.Y) * (p00.X - p10.X)) / 2.0;
			double s2 = (double)((p11.X - p10.X) * (p10.Y - p01.Y) - (p11.Y - p10.Y) * (p10.X - p01.X)) / 2.0;
			int px = (int)Math.Round(p00.X + (p01.X - p00.X) * s1 / (s1 + s2));
			int py = (int)Math.Round(p00.Y + (p01.Y - p00.Y) * s1 / (s1 + s2));
			return new Point(px, py);
		}

		/// <summary>
		/// _linkの線と始点、終点のエッジの線のそれぞれの線の交点を計算する </summary>
		/// <returns>測定位置合わせのライン</returns>
		public void SetShiftPoint()
		{
			if (_link == null || _link.PosBgn == _link.PosEnd) return;
						
			int y0, y1;
			if (_saveBegin.Y != _saveEnd.Y)
			{
				// 始点でのラインに対する法線ベクトルを求め、x = 0,1000のところの、yの値を算出
				y0 = -(_saveBegin.X - _saveEnd.X) * (_x0 - _saveBegin.X) / (_saveBegin.Y - _saveEnd.Y) + _saveBegin.Y;
				y1 = -(_saveBegin.X - _saveEnd.X) * (_x1 - _saveBegin.X) / (_saveBegin.Y - _saveEnd.Y) + _saveBegin.Y;
			}
			else
			{
				// ラインが水平の場合
				y0 = _saveBegin.Y;
				y1 = _saveBegin.Y;
			}

			Point p00 = new Point(_x0, y0);
			Point p01 = new Point(_x1, y1);
			PosBgn = IntersectionFrom4P(p00, p01, _link.PosBgn, _link.PosEnd);

			if (_saveBegin.Y != _saveEnd.Y)
			{
				// 終点でのラインに対する法線ベクトルを求め、x = 0,1000のところの、yの値を算出
				y0 = -(_saveBegin.X - _saveEnd.X) * (_x0 - _saveEnd.X) / (_saveBegin.Y - _saveEnd.Y) + _saveEnd.Y;
				y1 = -(_saveBegin.X - _saveEnd.X) * (_x1 - _saveEnd.X) / (_saveBegin.Y - _saveEnd.Y) + _saveEnd.Y;
			}
			{
				// ラインが水平の場合
				y0 = _saveEnd.Y;
				y1 = _saveEnd.Y;
			}
			p00 = new Point(_x0, y0);
			p01 = new Point(_x1, y1);
			PosEnd = IntersectionFrom4P(p00, p01, _link.PosBgn, _link.PosEnd);
		}

		/// <summary>
		/// ラインに対して長さを変化させる</summary>
		/// <param name="mouseX">マウスのｘ座標</param>
		/// <param name="mouseY">マウスのｙ座標</param>
		public Point ChangeLength(int mouseX, int mouseY)
		{
			int x = mouseX;
			int y = mouseY;

			if (_saveBegin.Y != _saveEnd.Y)
			{
				// マウスのY座標に対応するライン上のX座標を算出
				x = (mouseY - _saveEnd.Y) * (_saveBegin.X - _saveEnd.X) / (_saveBegin.Y - _saveEnd.Y) + _saveEnd.X;
			}
			if (_saveBegin.X != _saveEnd.X)
			{
				// マウスのX座標に対応するライン上のY座標を算出
				y = (mouseX - _saveEnd.X) * (_saveBegin.Y - _saveEnd.Y) / (_saveBegin.X - _saveEnd.X) + _saveEnd.Y;
			}

			if (_saveBegin.Y != _saveEnd.Y && _saveBegin.X != _saveEnd.X)
			{
				// 算出したx,yでラインに近い方を選択、遠い方はマウスの座標を使用する。
				if (Math.Abs(mouseX - x) > Math.Abs(mouseY - y))
				{
					x = mouseX;
				}
				else
				{
					y = mouseY;
				}
			}

			return new Point(x, y);
		}

		/// <summary>
		/// ラインを長さと垂直方向に移動させます</summary>
		/// <param name="mouseX">マウスのｘ座標</param>
		/// <param name="mouseY">マウスのｙ座標</param>
		public void Translation(int mouseX,int mouseY)
		{
			// 直線式：ax+by-c=0 のa,b,cを算出
			double a = -(_saveBegin.Y - _saveEnd.Y);
			double b = _saveBegin.X - _saveEnd.X;
			double c = -(PosBgn.X - PosEnd.X) * PosEnd.Y + (PosBgn.Y - PosEnd.Y) * PosEnd.X;

			// マウスの位置からラインに垂線を下した位置の座標(x0,y0)を算出
			int x0 = (int)((b * (b * mouseX - a * mouseY) - a * c) / (Math.Pow(a, 2) + Math.Pow(b, 2)));
			int y0 = (int)((a * (-b * mouseX + a * mouseY) - b * c) / (Math.Pow(a, 2) + Math.Pow(b, 2)));

			// 始点をx0からマウスポインタまでの移動量分動かす
			int x1 = PosBgn.X + mouseX - x0;
			// 始点のy座標は、x1の値を使用して算出
			int y1 = -(_saveBegin.X - _saveEnd.X) * ( x1 - _saveBegin.X) / (_saveBegin.Y - _saveEnd.Y) + _saveBegin.Y;

			// 終点をx0からマウスポインタまでの移動量分動かす 
			int x2 = PosEnd.X + mouseX - x0;
			// 終点のy座標は、x2の値を使用して算出
			int y2 = -(_saveBegin.X - _saveEnd.X) * ( x2 - _saveEnd.X) / (_saveBegin.Y - _saveEnd.Y) + _saveEnd.Y;

			PosBgn = new Point(x1, y1);
			PosEnd = new Point(x2, y2);
		}

		/// <summary>
		/// SavePointを更新します。
		/// </summary>
		public void SetSavePoint()
		{
			_saveBegin = PosBgn;
			_saveEnd = PosEnd;
		}

		/// <summary>
		/// PosBgnとPosEndの値を初期化します。
		/// </summary>
		public void ResetPoint()
		{
			PosBgn = new Point(0, 0);
			PosEnd = new Point(0, 0);
		}
	}
}
