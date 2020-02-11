using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace RulerJB
{
	/// <summary>
	/// 2本のペアになるラインを保持するクラスです
	/// </summary>
	class LinePair
	{
		/// <summary>上側の線の位置を保持します</summary>
		private ShapeLine _topLine= new ShapeLine();

		/// <summary>下側の線の位置を保持します</summary>
		private ShapeLine _btmLine = new ShapeLine();

		/// <summary>パース用Regexを保持します。最初の使用時に１度だけセットされます</summary>
		static Regex _regForParse = null;

		#region プロパティ　// -----------------------

		/// <summary>上側の線の位置を取得します</summary>
		public ShapeLine TopLine { get { return _topLine; } }

		/// <summary>下側の線の位置を取得します</summary>
		public ShapeLine BtmLine { get { return _btmLine; } }


		#endregion // プロパティ  -----------------------

		/// <summary>4点指定するコンストラクタです</summary>
		/// <param name="topL"></param>
		/// <param name="topR"></param>
		/// <param name="bottomL"></param>
		/// <param name="bottmR"></param>
		public LinePair(Point topL, Point topR, Point btmL, Point btmR )
		{
			// ラインの始点は全て左側とする
			_topLine.PosBgn = topL;
			_topLine.PosEnd = topR;
			_btmLine.PosBgn = btmL;
			_btmLine.PosEnd = btmR;
		}

		/// <summary>
		/// 上下の線分を指定したコンストラクタです
		/// </summary>
		/// <param name="top"></param>
		/// <param name="btm"></param>
		public LinePair(ShapeLine top, ShapeLine btm)
		{
			_topLine = top;
			_btmLine = btm;
		}

		/// <summary>デフォルトコンストラクタです。</summary>
		public LinePair()
		{
		}


		/// <summary>
		/// ２線間の距離を計算します
		/// </summary>
		/// <returns>２線間の距離</returns>
		public double CalcInterval()
		{
			// _PEND 簡易計算
			// 単純に開始点のｙ軸の差分
			return _btmLine.PosBgn.Y - _topLine.PosBgn.Y;
		}

		/// <summary>文字列に変換します</summary>
		/// <returns>変換後の文字列</returns>
		public override string ToString()
		{
			return String.Format("{0}:{1}", _topLine.ToString(), _btmLine.ToString());
		}



		/// <summary>文字列からパースします</summary>
		/// <param name="str">文字列</param>
		/// <param name="lp">生成したインスタンス</param>
		/// <returns>成否</returns>
		static public bool TryParse(string str, out LinePair lp)
		{
			var ret = false;
			lp = null;
			try
			{
				// 一度だけセットされる
				if (_regForParse == null)
				{
					_regForParse = new Regex(@"(.*?)\:(.*?)$", RegexOptions.Compiled);
				}
				var m = _regForParse.Match(str);
				if (m.Success)
				{
					var s1 = m.Groups[1].ToString();
					var s2 = m.Groups[2].ToString();

					var topL = ShapeLine.Parse(m.Groups[1].ToString());
					var btmL = ShapeLine.Parse(m.Groups[2].ToString());
					lp = new LinePair(topL, btmL);
					ret = true;
				}
			}
			catch
			{
				ret = false;
			}
			return ret;
		}


		/// <summary>文字列からインスタンスを生成します（パース）</summary>
		/// <param name="str">文字列</param>
		/// <returns>生成したインスタンス</returns>
		static public LinePair Parse(string str)
		{
			LinePair lp;
			if (LinePair.TryParse(str, out lp) == true)
			{
				return lp;
			}
			throw new Exception("LinePair.Parseに失敗しました");
		}

	}
}
