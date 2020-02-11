using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Drawing;

using DevelopLogSystem;

namespace RulerJB
{
	/// <summary>PageDataRJBの計測情報クラスです</summary>
	class PageMeasureData : IXmlProject
	{

		/// <summary>データの承認状態を取得・設定します</summary>
		public bool IsValidate { get; set; }

		/// <summary>変更があったか否かを取得します</summary>
		public bool IsChanged { get; set; }

		/// <summary>紙範囲（外）上下ラインを取得します</summary>
		public LinePair LinePaper { get;  set; }

		/// <summary>匡郭範囲（内）上下ラインを取得します</summary>
		public LinePair LineGrid { get; set; }

		/// <summary>紙測定位置ラインを取得します</summary>
		public ShapeLineEx MeasurePaper { get; set; }

		/// <summary>匡郭測定位置ラインを取得します</summary>
		public ShapeLineEx MeasureGrid { get; set; }

		/// <summary>紙高ピクセル数を取得します</summary>
		public int HeightPixPaper { get; set; }

		/// <summary>匡郭高ピクセル数を取得します</summary>
		public int HeightPixGrid { get; set; }

		/// <summary>デフォルトコンストラクタです </summary>
		public PageMeasureData()
		{
			Init();
		}

		/// <summary>いずれかの測定位置ラインに変更があったときに再計算を行うメソッドです</summary>
		public void ChangeLine()
		{
			if (MeasurePaper == null && MeasureGrid == null) return;

			// 変更ありとする
			IsChanged = true;
			if( MeasurePaper != null ) 	HeightPixPaper= (int)MeasurePaper.GetDistance();
			if( MeasureGrid != null ) HeightPixGrid = (int)MeasureGrid.GetDistance();
		}

		/// <summary>コンストラクタです</summary>
		/// <param name="org">オリジナル</param>
		public PageMeasureData(PageMeasureData org)
		{
			IsValidate		= org.IsValidate; ;
			IsChanged		= org.IsChanged;
			LinePaper		= org.LinePaper;
			LineGrid		= org.LineGrid;
			MeasurePaper	= org.MeasurePaper;
			MeasureGrid		= org.MeasureGrid;
			HeightPixPaper	= org.HeightPixPaper;
			HeightPixGrid	= org.HeightPixGrid;
		}


		/// <summary>
		/// XMLストリームからのコンストラクタです
		/// </summary>
		/// <param name="reader">XMLストリーム</param>
		/// <param name="keyword">終了キーワード</param>
		public PageMeasureData(XmlReader reader, string keyword)
		{
			Init();
			if (FromXmlReader(reader, keyword) == false)
			{
				throw new Exception("PageMeasureDataコンストラクタでインスタンス生成に失敗しました.");
			}
		}


		/// <summary>初期化</summary>
		public void Init()
		{
			IsValidate = false;
			IsChanged = false;
			LinePaper = null;
			LineGrid		= null;
			MeasurePaper	= null;
			MeasureGrid		= null;
			HeightPixPaper	= 0;
			HeightPixGrid	= 0;
		}


		/// <summary>Xmlでの書き込み</summary>
		/// <param name="xmlWriter">書き込み用ストリーム</param>
		/// <returns>成否</returns>
		public bool ToXmlWriter(XmlWriter writer, string keyword)
		{
			bool ret = true;
			try
			{
				writer.WriteStartElement(keyword);

				writer.WriteElementString("IsValidate", IsValidate.ToString());
				writer.WriteElementString("IsChanged", IsChanged.ToString());
				if (LinePaper != null) writer.WriteElementString("LinePaper", LinePaper.ToString());
				if (LineGrid != null) writer.WriteElementString("LineGrid", LineGrid.ToString());
				writer.WriteElementString("HeightPixPaper", HeightPixPaper.ToString());
				writer.WriteElementString("HeightPixGrid", HeightPixGrid.ToString());
				if (MeasurePaper != null) writer.WriteElementString("MeasurePaper", MeasurePaper.ToString());
				if (MeasureGrid != null) writer.WriteElementString("MeasureGrid", MeasureGrid.ToString());

				writer.WriteEndElement();
			}
			catch( Exception ex )
			{
				DevelopLog.LogException(MethodBase.GetCurrentMethod(), "例外発生", ex);
				ret = false;
			}
			return ret;
		}


		/// <summary>紙高比率より算出した匡郭高(mm)を算出します</summary>
		/// <param name="paperMM">紙高mm</param>
		/// <returns>匡郭高(mm)</returns>
		public double HightGridMM(double paperMM)
		{
			return paperMM * HeightPixGrid / (double)HeightPixPaper;
		}

        /// <summary>pixel比より紙高(mm)を算出します</summary>
        /// <param name="pixelPerMM">pixel比</param>
        /// <returns>紙高(mm),匡郭高(mm)</returns>
        public (double paperMM, double gridMM) HightPaperGridMM(double pixelPerMM)
        {
            return ((pixelPerMM == 0 ? 0 : HeightPixPaper / pixelPerMM), (pixelPerMM == 0 ? 0 : HeightPixGrid / pixelPerMM));
        }

        /// <summary>Xmlからの読み込みとインスタンス生成</summary>
        /// <param name="xmlReader">読み込み用ストリーム</param>
        /// <returns>生成インスタンス（失敗時はnull）</returns>
        private bool FromXmlReader(XmlReader reader, string keyword)
		{
			var ret = true;
			MethodBase methodbase = MethodBase.GetCurrentMethod();
			try
			{
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.LocalName.Equals("IsValidate"))
						{
							IsValidate = bool.Parse(reader.ReadString());
						}
						if (reader.LocalName.Equals("IsChanged"))
						{
							IsChanged = bool.Parse(reader.ReadString());
						}
						if (reader.LocalName.Equals("LinePaper"))
						{
							LinePaper = LinePair.Parse(reader.ReadString());
						}
						if (reader.LocalName.Equals("LineGrid"))
						{
							LineGrid = LinePair.Parse(reader.ReadString());
						}
						if (reader.LocalName.Equals("HeightPixPaper"))
						{
							HeightPixPaper = int.Parse(reader.ReadString());
						}
						if (reader.LocalName.Equals("HeightPixGrid"))
						{
							HeightPixGrid = int.Parse(reader.ReadString());
						}
						if (reader.LocalName.Equals("MeasurePaper"))
						{
							MeasurePaper = new ShapeLineEx(ShapeLine.Parse(reader.ReadString()), MeasureGrid);
							if (MeasureGrid != null) MeasureGrid._link = MeasurePaper;
						}
						if (reader.LocalName.Equals("MeasureGrid"))
						{
							MeasureGrid = new ShapeLineEx(ShapeLine.Parse(reader.ReadString()), MeasurePaper);
							if (MeasurePaper != null) MeasurePaper._link = MeasureGrid;
						}
					}
					else if (reader.NodeType == XmlNodeType.EndElement)
					{
						if (reader.LocalName == keyword) break;
					}
				}
			}
			catch (Exception ex)
			{
				var str = String.Format("Msg={0},  Stack={1}", ex.Message, ex.StackTrace);
				ret = false;
			}
			return ret;
		}
	}
}
