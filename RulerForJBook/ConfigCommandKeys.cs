using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace RulerJB
{
	/// <summary>
	/// キーにコマンドを割り当てるためのデータのクラスです
	/// </summary>
	public class ConfigCommandKeys:IXmlProject
	{
		/// <summary>実行処理を示す番号です</summary>
		public enum CommandNo:int
		{
			/// <summary>次のデータに表示を移動します</summary>
			ViewNextData= 1,
			/// <summary>前のデータに表示を移動します</summary>
			ViewBackData,

		}

		/// <summary>キーパターンと実行内容の辞書を保持します。</summary>
		private Dictionary<ConfigCommandKeyData, CommandNo> _commandKeys;

		/// <summary>デフォルトのXMLキーワードを定義します</summary>
		const string DefaultKeyString = "ConfigCommandKeys";

		/// <summary>
		/// コンストラクタです
		/// </summary>
		public ConfigCommandKeys()
		{
			_commandKeys = new Dictionary<ConfigCommandKeyData, CommandNo>();
		}


		/// <summary>
		/// コンストラクタです
		/// </summary>
		/// <param name="filename">ファイル名</param>
		public ConfigCommandKeys(string filename)
		{
			_commandKeys = new Dictionary<ConfigCommandKeyData, CommandNo>();
			_commandKeys.Add( new ConfigCommandKeyData( Keys.N, true, true, false ), CommandNo.ViewNextData );		// Ctrl+Shift+N
			_commandKeys.Add( new ConfigCommandKeyData( Keys.B, true, true, false ), CommandNo.ViewBackData );		// Ctrl+Shift+B
		}


		/// <summary>
		/// 情報をXMLファイルより読み出します
		/// </summary>
		/// <param name="filename">ファイル名</param>
		/// <returns>成否</returns>
		public bool Load(string filename)
		{
			return false;
		}

		/// <summary>
		/// 情報をXMLファイルに保存します
		/// </summary>
		/// <param name="filename">ファイル名</param>
		/// <returns>成否</returns>
		public bool Save(string filename)
		{
			return false;
		}


		/// <summary>Xmlでの書き込み</summary>
		/// <param name="writer">書き込み用ストリーム</param>
		/// <param name="keyword">キーワード</param>
		/// <returns>成否</returns>
		public bool ToXmlWriter(XmlWriter writer, string keyword)
		{
			bool ret = true;
			try
			{
			//    writer.WriteStartElement(keyword);
			//    foreach (var lst in _commandKeys)
			//    {
			//        if (lst.ToXmlWriter(writer, "CommandKeys") == false) ret = false;
			//    }

			//    if (CommonFolderPath != null)
			//    {
			//        writer.WriteElementString("CommonFolderPath", CommonFolderPath.ToString());
			//    }


			//    // 他はCurrent・・・なので書き込まない


			//    writer.WriteEndElement();
			}
			catch
			{
				ret = false;
			}
			return ret;
		}



		/// <summary>Xmlからの読み込みでデータをセットします</summary>
		/// <param name="xmlReader">読み込み用ストリーム</param>
		/// <returns>生成インスタンス（失敗時はnull）</returns>
		private bool FromXmlReader(XmlReader reader, string keyword)
		{
			var ret = true;
			//MethodBase methodbase = MethodBase.GetCurrentMethod();
			//try
			//{
			//    var pdList = new List<PageDataRJB>();
			//    while (reader.Read())
			//    {
			//        if (reader.NodeType == XmlNodeType.Element)
			//        {
			//            if (reader.LocalName.Equals("CommonFolderPath"))
			//            {
			//                CommonFolderPath = reader.ReadString();
			//            }
			//            if (reader.LocalName.Equals("PageDataList"))
			//            {
			//                pdList.Add(new PageDataRJB(reader, "PageDataList"));
			//            }
			//        }
			//        else if (reader.NodeType == XmlNodeType.EndElement)
			//        {
			//            if (reader.LocalName == keyword) break;
			//        }
			//    }
			//    _pageDataList = pdList.ToArray();
			//}

			//catch
			//{
			//    ret = false;
			//}
			return ret;
		}

	}


	// ----- 内部クラス -----
	/// <summary>
	/// コマンドの対応を示すキーの組み合わせクラスです
	/// </summary>
	public class ConfigCommandKeyData : IXmlProject, IComparable
	{
		/// <summary>割り当てキーを取得します</summary>
		public Keys KeyData { get; private set; }

		/// <summary>割り当てキー・コントロールの状態を取得します</summary>
		public bool IsCtrl { get; private set; }

		/// <summary>割り当てキー・シフトの状態を取得します</summary>
		public bool IsShift { get; private set; }

		/// <summary>割り当てキー・ALTの状態を取得します</summary>
		public bool IsAlt { get; private set; }

		

		/// <summary>コンストラクタです</summary>
		public ConfigCommandKeyData( Keys keydata, bool isctrl, bool isshift, bool isalt)
		{
			KeyData = keydata;
			IsCtrl = isctrl;
			IsShift = isshift;
			IsAlt = isalt;
		}


		/// <summary>Xmlでの書き込み</summary>
		/// <param name="writer">書き込み用ストリーム</param>
		/// <param name="keyword">キーワード</param>
		/// <returns>成否</returns>
		public bool ToXmlWriter(System.Xml.XmlWriter writer, string keyword)
		{
			throw new NotImplementedException();
		}


		/// <summary>データを比較します</summary>
		/// <param name="obj">比較対象</param>
		/// <returns>比較結果</returns>
		public int CompareTo(object obj)
		{
			var o = (ConfigCommandKeyData)obj;
			var ret = KeyData.CompareTo(o.KeyData);
			if (ret != 0) return ret;
			ret = IsCtrl.CompareTo(o.IsCtrl);
			if (ret != 0) return ret;
			ret = IsShift.CompareTo(o.IsShift);
			if (ret != 0) return ret;
			ret = IsAlt.CompareTo(o.IsAlt);
			return ret;
		}
	}
}
