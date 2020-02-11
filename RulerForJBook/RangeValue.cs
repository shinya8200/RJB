using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RulerJB
{
	/// <summary>値の範囲を扱うクラスです</summary>
	/// <remarks>Tは比較可能なオブジェクト</remarks>
	public class RangeValue<T> where T : IComparable
	{
		/// <summary>上限または最大値を保持します</summary>
		protected T _upperLimit;
		/// <summary>下限または最小値を保持します</summary>
		protected T _lowerLimit;

		/// <summary>上限値を取得します</summary>
		public T Max { get { return _upperLimit; } }

		/// <summary>下限値を取得します</summary>
		public T Min { get { return _lowerLimit; } }

		/// <summary>設定されている範囲が有効であるか取得します</summary>
		public bool IsValid { get { return _upperLimit.CompareTo(_lowerLimit) >= 0; } }


		/// <summary>コンストラクタです</summary>
		public RangeValue(T initMin, T initMax )
		{
			_lowerLimit = initMin;
			_upperLimit = initMax;
		}

		/// <summary>コンストラクタです</summary>
		public RangeValue(RangeValue<T>org):this(org._lowerLimit, org._upperLimit )
		{
		}

		/// <summary>
		/// 指定した値が上限または下限を超えていた場合、その値を更新します
		/// </summary>
		/// <param name="val">指定値</param>
		public void SetLimit(T val)
		{
			if (val.CompareTo(_lowerLimit) < 0) _lowerLimit = val;
			if (val.CompareTo(_upperLimit) > 0) _upperLimit = val;
		}

		/// <summary>
		/// 指定した値が上限と下限内であるか調べます
		/// </summary>
		/// <param name="val">指定値</param>
		/// <param name="isEqu">リミット値を含む場合真を指定します</param>
		public bool CheckLimit(T val, bool isEqu )
		{
			// リミット同値を含む場合、リミットと同値の場合にはエラーにしない
			if (isEqu)
			{
				if(  val.CompareTo(_lowerLimit) < 0 || val.CompareTo(_upperLimit)> 0 )  return false;
			}
			else
			{
				if(  val.CompareTo(_lowerLimit) <= 0 || val.CompareTo(_upperLimit)>= 0 )  return false;
			}
			return true;
		}

		/// <summary>
		/// 文字列に変換します
		/// </summary>
		/// <returns>オブジェクト文字列</returns>
		public override string ToString()
		{
			return String.Format("({0}- {1})",Min, Max);
		} 

	}
}
