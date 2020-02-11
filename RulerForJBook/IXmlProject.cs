using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RulerJB
{
	/// <summary>
	/// XML関連のインターフェース
	/// </summary>
	interface IXmlProject
	{
		/// <summary>Xmlでの書き込み</summary>
		/// <param name="writer">書き込み用ストリーム</param>
		/// <param name="keyword">キーワード</param>
		/// <returns>成否</returns>
		bool ToXmlWriter(XmlWriter writer, string keyword);

	}
}
