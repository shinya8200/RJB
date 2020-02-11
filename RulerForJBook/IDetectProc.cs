using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace RulerJB
{
	/// <summary>
	/// 検出処理用のインターフェースです
	/// </summary>
	interface IDetectProc:IDisposable
	{
		/// <summary>
		/// 検出を行います。
		/// </summary>
		/// <param name="img">対象画像</param>
		/// <param name="pset">設定値</param>
		/// <param name="sideL">左側ページ情報</param>
		/// <param name="sideR">右側ページ情報</param>
		/// <param name="pars">汎用パラメータ</param>
		/// <returns>成否</returns>
		bool Exec(Bitmap img, BookProjectSetData pset, PageMeasureData sideL, PageMeasureData sideR, Dictionary<string, object> pars);
	}
}
