using System;
using System.Collections.Generic;
using System.ComponentModel;			// [Category]等の指定
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Reflection;

using DevelopLogSystem;

namespace RulerJB
{
	/// <summary>ブックプロジェクト設定データクラスです</summary>
	class BookProjectSetData : IXmlProject
	{
		#region 01_内部メンバー定義 -------

		// --- 設定値（ユーザー設定変更可能）

		/// <summary>計算の使用基準パラメータ(true:紙高 false:ピクセル数)を取得します</summary>
		private bool? _isUseHeightPaper = null;

		/// <summary> 設定された紙高を保持します </summary>
		private double _hightPaperSizeMM;

		/// <summary> 設定された1mm当たりのピクセル数を保持します </summary>
		private double _pixelPerMm;

		/// <summary> ROI （紙・上・左）を保持します </summary>
		private BookROI _roiPTL;

		/// <summary> ROI （紙・下・左）を保持します </summary>
		private BookROI _roiPBL;

		/// <summary> ROI （匡郭・上・左）を保持します </summary>
		private BookROI _roiGTL;
	
		/// <summary> ROI （匡郭・下・左）を保持します </summary>
		private BookROI _roiGBL;
		
		/// <summary> ROI （紙・上・右）を保持します </summary>
		private BookROI _roiPTR;
		
		/// <summary> ROI （紙・下・右）を保持します </summary>
		private BookROI _roiPBR;
		
		/// <summary> ROI （匡郭・上・右）を保持します </summary>
		private BookROI _roiGTR;
		
		/// <summary> ROI （匡郭・下・右）を保持します </summary>
		private BookROI _roiGBR;

		/// <summary>PDFファイルが指定された時のパスを保存します</summary>
		private string _pdfPath;

		/// <summary>IIIFファイルが指定された時のパスを保存します</summary>
		private string _iiifPath;

		/// <summary>デフォルトのXMLキーです</summary>
		const string DefaultKeyword = "SetData";


		#endregion // 内部メンバー定義 -------


		#region 02_全メンバー対象メソッド

		/// <summary>コンストラクタです（同一オブジェクト生成用）</summary>
		/// <param name="isuseheightpaper">計算の使用基準パラメータ(true:紙高 false:ピクセル数)</param>
		/// <param name="heightpaper">紙高</param>
		/// <param name="pixelpermm">1mm当たりのピクセル数</param>
		/// <param name="ptl">ROI （紙・上・左）</param>
		/// <param name="pbl">ROI （紙・下・左）</param>
		/// <param name="gtl">ROI （匡郭・上・左）</param>
		/// <param name="gbl">ROI （匡郭・下・左）</param>
		/// <param name="ptr">ROI （紙・上・右）</param>
		/// <param name="pbr">ROI （紙・下・右）</param>
		/// <param name="gtr">ROI （匡郭・上・右）</param>
		/// <param name="gbr">ROI （匡郭・下・右）</param>
		/// <param name="pdfpath">PDFパス</param>
		private BookProjectSetData(bool? isuseheightpaper, double heightpaper, double pixelpermm, BookROI ptl, BookROI pbl, BookROI gtl, BookROI gbl, BookROI ptr, BookROI pbr, BookROI gtr, BookROI gbr, string pdfpath, string iiifpath)
		{
			// 全メンバーのコピー
			_isUseHeightPaper = isuseheightpaper;
			_hightPaperSizeMM = heightpaper;
			_pixelPerMm = pixelpermm;
			_roiPTL = ptl;
			_roiPBL = pbl;
			_roiGTL = gtl;
			_roiGBL = gbl;
			_roiPTR = ptr;
			_roiPBR = pbr;
			_roiGTR = gtr;
			_roiGBR = gbr;
			_pdfPath = pdfpath;
			_iiifPath = iiifpath;
		}

		/// <summary>
		/// コピーコンストラクタです。
		/// </summary>
		/// <param name="org">オリジナルインスタンス</param>
		public BookProjectSetData(BookProjectSetData org)
		{

		}

		/// <summary>
		/// デフォルトコンストラクタです
		/// </summary>
		public BookProjectSetData()
		{
			SetDefaultValue();
		}


		/// <summary>Xmlストリームによるコンストラクタ</summary>
		/// <param name="reader">Xmlストリーム</param>
		/// <param name="keyword">キーワード</param>
		public BookProjectSetData(XmlReader reader, string keyword= DefaultKeyword):this()
		{
			if (FromXmlReader(reader, keyword)== false )
			{
				throw new Exception("BookProjectSetData　XMLコンストラクタ失敗");
			}
		}


		/// <summary>Xml文字列によるコンストラクタ</summary>
		public BookProjectSetData( string xmlfile ) : this()
		{
			using (var reader = new XmlTextReader(xmlfile))
			{
				FromXmlReader(reader, DefaultKeyword );
			}
		}


		/// <summary> デフォルト値のセット </summary>
		public void SetDefaultValue()
		{
			_isUseHeightPaper = null;
			_hightPaperSizeMM = 250.0;
			_pixelPerMm = 0;
			_roiPTL = new BookROI();
			_roiPBL = new BookROI();
			_roiGTL = new BookROI();
			_roiGBL = new BookROI();
			_roiPTR = new BookROI();
			_roiPBR = new BookROI();
			_roiGTR = new BookROI();
			_roiGBR = new BookROI();
			_pdfPath = null;
			_iiifPath = null;
		}



		/// <summary>イメージを指定して未設定のデフォルトROIの値を調整する</summary>
		/// <param name="imagesize">画像サイズ</param>
		public void AdjustDefaultRoi(Size imagesize)
		{
			if (imagesize == null) return;
			if (_roiPTL.RoiSize.Width == 0) _roiPTL.SetDefaultRoi(imagesize, 1.0 / 8.0, 1.0 / 24.0, 1.0 / 5.0, 1.0 / 4.0, BookROI.PosInf.LeftTop);
			if (_roiPBL.RoiSize.Width == 0) _roiPBL.SetDefaultRoi(imagesize, 1.0 / 8.0, 1.0 / 24.0, 1.0 / 5.0, 1.0 / 4.0, BookROI.PosInf.LeftBtm);
			if (_roiPTR.RoiSize.Width == 0) _roiPTR.SetDefaultRoi(imagesize, 1.0 / 8.0, 1.0 / 24.0, 1.0 / 5.0, 1.0 / 4.0, BookROI.PosInf.RightTop);
			if (_roiPBR.RoiSize.Width == 0) _roiPBR.SetDefaultRoi(imagesize, 1.0 / 8.0, 1.0 / 24.0, 1.0 / 5.0, 1.0 / 4.0, BookROI.PosInf.RightBtm);

			if (_roiGTL.RoiSize.Width == 0) _roiGTL.SetDefaultRoi(imagesize, 1.0 / 6.0, 1.0 / 10.0, 1.0 / 6.0, 1.0 / 5.0, BookROI.PosInf.LeftTop);
			if (_roiGBL.RoiSize.Width == 0) _roiGBL.SetDefaultRoi(imagesize, 1.0 / 6.0, 1.0 / 10.0, 1.0 / 6.0, 1.0 / 5.0, BookROI.PosInf.LeftBtm);
			if (_roiGTR.RoiSize.Width == 0) _roiGTR.SetDefaultRoi(imagesize, 1.0 / 6.0, 1.0 / 10.0, 1.0 / 6.0, 1.0 / 5.0, BookROI.PosInf.RightTop);
			if (_roiGBR.RoiSize.Width == 0) _roiGBR.SetDefaultRoi(imagesize, 1.0 / 6.0, 1.0 / 10.0, 1.0 / 6.0, 1.0 / 5.0, BookROI.PosInf.RightBtm);
		}




		/// <summary>全メンバー比較を行います　（内部処理用）</summary>
		/// <param name="isuseheightpaper">計算の使用基準パラメータ(true:紙高 false:ピクセル数)</param>
		/// <param name="heightpaper">紙高</param>
		/// <param name="pixelpermm">1mm当たりのピクセル数</param>
		/// <param name="ptl">ROI （紙・上・左）</param>
		/// <param name="pbl">ROI （紙・下・左）</param>
		/// <param name="gtl">ROI （匡郭・上・左）</param>
		/// <param name="gbl">ROI （匡郭・下・左）</param>
		/// <param name="ptr">ROI （紙・上・右）</param>
		/// <param name="pbr">ROI （紙・下・右）</param>
		/// <param name="gtr">ROI （匡郭・上・右）</param>
		/// <param name="gbr">ROI （匡郭・下・右）</param>
		/// <param name="pdfpath">PDF指定時のPDFファイルパス</param>
		/// <returns>比較結果 0:一致</returns>
		public int CompareAllMenber(bool? isuseheightpaper, double heightpaper, double pixelpermm, BookROI ptl, BookROI pbl, BookROI gtl, BookROI gbl, BookROI ptr, BookROI pbr, BookROI gtr, BookROI gbr, string pdfpath, string iiifpath)
		{
			// 全メンバーの比較
			if (_isUseHeightPaper != isuseheightpaper)
			{
				if (_isUseHeightPaper.HasValue == false) return -1;
				if (isuseheightpaper.HasValue == false) return 1;
				return (_isUseHeightPaper == true) ? 1 : -1;
			}
			if (_hightPaperSizeMM != heightpaper)
			{
				return (_hightPaperSizeMM > heightpaper) ? 1 : -1;
			}
			if (_pixelPerMm != pixelpermm)
			{
				return (_pixelPerMm > pixelpermm) ? 1 : -1;
			}

			// BookROI群の比較 最初に見つかった不一致の情報を返す（見つからなければ次の項目へ）
			var dst = new BookROI[] {  ptl,		pbl,		gtl,		gbl,		ptr,		pbr,		gtr,		gbr		};
			var src = new BookROI[] { _roiPTL,	_roiPBL,	_roiGTL,	_roiGBL,	_roiPTR,	_roiPBR,	_roiGTR,	_roiGBR };
			foreach( var cmp in dst.Zip(dst, (s, d) => s.CompareTo(d))  )			// 比較した結果が配列になっている 0, 0, 0, -1, 1,・・
			{
				if( cmp!= 0 ) return cmp;		
			}

			int compVal = 0;
			if (_pdfPath == null && pdfpath == null) compVal = 0;
			else if (_pdfPath != null && pdfpath != null) compVal = String.Compare(_pdfPath, pdfpath);
			else compVal = _pdfPath == null ? -1 : 1;
			if (compVal != 0) return compVal;

			compVal = 0;
			if (_iiifPath == null && iiifpath == null) compVal = 0;
			else if (_iiifPath != null && iiifpath != null) compVal = String.Compare(_iiifPath, iiifpath);
			else compVal = _iiifPath == null ? -1 : 1;
			if (compVal != 0) return compVal;

			return 0;		// 一致
		}


		#endregion // 全メンバー対象メソッド




		/// <summary> デフォルトの設定値ファイル名を取得します </summary>
		/// <returns>デフォルトの設定値ファイル名</returns>
		public string GetDefaultFileName(string datapath)			// 設定値のデフォルトファイル名を返す
		{
			return Path.Combine(Path.GetDirectoryName(datapath), Path.GetFileNameWithoutExtension(datapath) + "Settings.xml");	// 設定値はデータフォルダに関係なく実行形式フォルダ
		}

		/// <summary> デフォルトのCSVファイル名を取得します </summary>
		/// <returns>デフォルトのCSVファイル名</returns>
		public string GetDefaultExportCsv(string datapath)			// 設定値（Export/CSV）のデフォルトファイル名を返す
		{
			return Path.Combine(Path.GetDirectoryName(datapath), Path.GetFileNameWithoutExtension(datapath) + "Settings.csv");	// 設定値はデータフォルダに関係なく実行形式フォルダ
		}


		#region 03_プロパティ ----------

		[Category("１．計測参照値")]
		[Description("計算の使用基準パラメータ(true:紙高 false:ピクセル数)")]
		[DefaultValue(null)]
		[PropertyDisplayName("01.使用基準パラメータ")]
		/// <summary>使用基準パラメータを設定・取得します</summary>
		public bool? IsUseHeightPaper
		{
			get
			{
				return _isUseHeightPaper;
			}
			set
			{
				_isUseHeightPaper = value;
			}
		}

		[Category("１．計測参照値")]
		[Description("実測紙高を指定します＜IsUseHeightPaperがtrueの場合有効＞")]
		[DefaultValue(250.0)]
		[PropertyDisplayName("02.実測紙高値(mm)")]
		/// <summary>実測紙高を指定します＜IsUseHeightPaperがtrueの場合有効＞</summary>
		public double HightPaperSizeMM
		{
			get
			{
				return _hightPaperSizeMM;
			}
			set 
			{
				_hightPaperSizeMM = value;
			}
		}

		[Category("１．計測参照値")]
		[Description("設定された1mm当たりのピクセル数を指定します＜IsUseHeightPaperがfalseの場合有効＞")]
		[DefaultValue(10.0)]
		[PropertyDisplayName("03.1mm当たりのピクセル数")]
		/// <summary>1mm当たりのピクセル数を指定します＜IsUseHeightPaperがfalseの場合有効＞</summary>
		public double PixelPerMm
		{
			get
			{
				return _pixelPerMm;
			}
			set
			{
				_pixelPerMm = value;
			}
		}


		[Category("２．ROI設定")]
		[Description("ROI （紙・上・左）")]
//		[DefaultValue()]
		[PropertyDisplayName("01. ROI （紙・上・左）")]
		/// <summary> ROI （紙・上・左）を保持します</summary>
		public BookROI RoiPTL
		{
			get	{	return _roiPTL;		}
			set	{	_roiPTL = value;	}
		}


		[Category("２．ROI設定")]
		[Description("ROI （紙・下・左）")]
		//		[DefaultValue()]
		[PropertyDisplayName("02. ROI （紙・下・左）")]
		/// <summary> ROI （紙・下・左）を保持します</summary>
		public BookROI RoiPBL
		{
			get { return _roiPBL; }
			set { _roiPBL = value; }
		}


		[Category("２．ROI設定")]
		[Description("ROI （匡郭・上・左）")]
		//		[DefaultValue()]
		[PropertyDisplayName("03. ROI （匡郭・上・左）")]
		/// <summary>ROI （匡郭・上・左）を保持します</summary>
		public BookROI RoiGTL
		{
			get { return _roiGTL; }
			set { _roiGTL = value; }
		}


		[Category("２．ROI設定")]
		[Description("ROI （匡郭・下・左）")]
		//		[DefaultValue()]
		[PropertyDisplayName("04. ROI （匡郭・下・左）")]
		/// <summary>ROI （匡郭・下・左）を保持します</summary>
		public BookROI RoiGBL
		{
			get { return _roiGBL; }
			set { _roiGBL = value; }
		}



		[Category("２．ROI設定")]
		[Description("ROI （紙・上・右）")]
		//		[DefaultValue()]
		[PropertyDisplayName("05. ROI （紙・上・右）")]
		/// <summary>ROI （紙・上・右）を保持します</summary>
		public BookROI RoiPTR
		{
			get { return _roiPTR; }
			set { _roiPTR = value; }
		}



		[Category("２．ROI設定")]
		[Description("ROI （紙・下・右）")]
		//		[DefaultValue()]
		[PropertyDisplayName("06. ROI （紙・下・右）")]
		/// <summary>ROI （紙・下・右）を保持します</summary>
		public BookROI RoiPBR
		{
			get { return _roiPBR; }
			set { _roiPBR = value; }
		}



		[Category("２．ROI設定")]
		[Description("ROI （匡郭・上・右）")]
		//		[DefaultValue()]
		[PropertyDisplayName("07. ROI （匡郭・上・右）")]
		/// <summary>ROI （匡郭・上・右）を保持します</summary>
		public BookROI RoiGTR
		{
			get { return _roiGTR; }
			set { _roiGTR = value; }
		}



		[Category("２．ROI設定")]
		[Description("ROI （匡郭・下・右）")]
		//		[DefaultValue()]
		[PropertyDisplayName("08. ROI （匡郭・下・右）")]
		/// <summary>ROI （匡郭・下・右）を保持します</summary>
		public BookROI RoiGBR
		{
			get { return _roiGBR; }
			set { _roiGBR = value; }
		}


		[Category("９．内部設定値")]
		[Description("PDF指定時のパス")]
		[DefaultValue("")]
		[ReadOnly(true)]
		[PropertyDisplayName("09. 内部設定（PDFPath）")]
		/// <summary>PDF指定時のパスを保持します</summary>
		public string PdfPath
		{
			get { return _pdfPath; }
			set { _pdfPath = value; }
		}


		[Category("９．内部設定値")]
		[Description("IIIF指定時のパス")]
		[DefaultValue("")]
		[ReadOnly(true)]
		[PropertyDisplayName("09. 内部設定（IIIFPath）")]
		/// <summary>PDF指定時のパスを保持します</summary>
		public string IiifPath
		{
			get { return _iiifPath; }
			set { _iiifPath = value; }
		}

		// -- 内部設定値（ユーザ設定しない・Exportしない）--

#if false  // 参考
		/// <summary>最後に指定した入力用パスを設定・取得します</summary>
		[Category("内部設定")]
		[Description("入力用フォルダの設定です。通常は最終入力フォルダ")]
		[PropertyDisplayName("01. 入力フォルダ")]
		[ReadOnly(true)]
		public string InputFolder
		{
			set { _inputFolder = value; }
			get { return (_inputFolder == null || _inputFolder.Length == 0) ? CommonValues.ConfigData.DataFolder : _inputFolder; }
		}

		/// <summary>最後に指定した出力用パスを設定・取得します</summary>
		/// <summary>最後に指定した入力用パスを設定・取得します</summary>
		[Category("内部設定")]
		[Description("出力用フォルダの設定です。通常は最終出力フォルダ")]
		[PropertyDisplayName("02. 出力フォルダ")]
		[ReadOnly(true)]
		public string OutputFolder
		{
			set { _outputFolder = value; }
			get { return (_outputFolder == null || _outputFolder.Length == 0) ? CommonValues.ConfigData.DataFolder : _outputFolder; }
		}

		/// <summary> 起動時のウィンドウサイズを取得または設定します </summary>
		/// <remarks>非ユーザー設定項目</remarks>
		[Category("内部設定")]
		[Description("ウィンドウのサイズ")]
		[PropertyDisplayName("03. ウィンドウサイズ")]
		[ReadOnly(true)]
		public Size WindowSize
		{
			set { _windowSize = value; }
			get { return _windowSize; }
		}

#endif // 参考 end

		#endregion // 03_プロパティ ----------


		public void ResetRoi(Size imageSize)
		{
			RoiPTL.RoiSize = RoiPBL.RoiSize = RoiPBR.RoiSize = RoiPTR.RoiSize = new Size(imageSize.Height / 16, imageSize.Height / 16);
			RoiGTL.RoiSize = RoiGBL.RoiSize = RoiGBR.RoiSize = RoiGTR.RoiSize = new Size(imageSize.Height / 16, imageSize.Height / 16);
			RoiPTL.BasePoint = new Point( imageSize.Width * 2 / 16, imageSize.Height * 1 / 32);
			RoiPBL.BasePoint = new Point( imageSize.Width * 2 / 16, imageSize.Height * 29/ 32 );
			RoiPBR.BasePoint = new Point( imageSize.Width * 13 / 16, imageSize.Height * 29 / 32);
			RoiPTR.BasePoint = new Point( imageSize.Width * 13 / 16, imageSize.Height * 1 / 32);
			RoiGTL.BasePoint = new Point( imageSize.Width * 2 / 16, imageSize.Height *  3 / 32);
			RoiGBL.BasePoint = new Point( imageSize.Width * 2 / 16, imageSize.Height * 27 / 32);
			RoiGBR.BasePoint = new Point( imageSize.Width * 13 / 16, imageSize.Height * 27 / 32);
			RoiGTR.BasePoint = new Point( imageSize.Width * 13 / 16, imageSize.Height *  3 / 32);
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
				if( IsUseHeightPaper.HasValue ) writer.WriteElementString("IsUseHeightPaper", IsUseHeightPaper.ToString());
				writer.WriteElementString("HightPaperSizeMM", HightPaperSizeMM.ToString());
				writer.WriteElementString("PixelPerMm", PixelPerMm.ToString());
				writer.WriteElementString("RoiPTL",  RoiPTL.ToString());
				writer.WriteElementString("RoiPBL",  RoiPBL.ToString());
				writer.WriteElementString("RoiGTL",  RoiGTL.ToString());
				writer.WriteElementString("RoiGBL",  RoiGBL.ToString());
				writer.WriteElementString("RoiPTR",  RoiPTR.ToString());
				writer.WriteElementString("RoiPBR",  RoiPBR.ToString());
				writer.WriteElementString("RoiGTR",  RoiGTR.ToString());
				writer.WriteElementString("RoiGBR", RoiGBR.ToString());
				if (_pdfPath != null) writer.WriteElementString("PdfPath", _pdfPath);
				if (_iiifPath != null) writer.WriteElementString("IiifPath", _iiifPath);

				writer.WriteEndElement();
			}
			catch
			{
				ret = false;
			}
			return ret;
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
						if (reader.LocalName.Equals("IsUseHeightPaper"))
						{
							IsUseHeightPaper = bool.Parse( reader.ReadString() );
						}
						if (reader.LocalName.Equals("HightPaperSizeMM"))
						{
							HightPaperSizeMM = double.Parse( reader.ReadString() );
						}
						if (reader.LocalName.Equals("PixelPerMm"))
						{
							PixelPerMm = double.Parse( reader.ReadString() );
						}
						if (reader.LocalName.Equals("RoiPTL"))
						{
							 RoiPTL = BookROI.Parse( reader.ReadString() );
						}
						if (reader.LocalName.Equals("RoiPBL"))
						{
							 RoiPBL = BookROI.Parse( reader.ReadString() );
						}
						if (reader.LocalName.Equals("RoiGTL"))
						{
							 RoiGTL = BookROI.Parse( reader.ReadString() );
						}
						if (reader.LocalName.Equals("RoiGBL"))
						{
							 RoiGBL = BookROI.Parse( reader.ReadString() );
						}
						if (reader.LocalName.Equals("RoiPTR"))
						{
							 RoiPTR = BookROI.Parse( reader.ReadString() );
						}
						if (reader.LocalName.Equals("RoiPBR"))
						{
							 RoiPBR = BookROI.Parse( reader.ReadString() );
						}
						if (reader.LocalName.Equals("RoiGTR"))
						{
							 RoiGTR = BookROI.Parse( reader.ReadString() );
						}
						if (reader.LocalName.Equals("RoiGBR"))
						{
							 RoiGBR = BookROI.Parse( reader.ReadString() );
						}
						if( (reader.LocalName.Equals("PdfPath")))
						{
							_pdfPath = reader.ReadString();
						}
						if ((reader.LocalName.Equals("IiifPath")))
						{
							_iiifPath = reader.ReadString();
						}
					}
					else if (reader.NodeType == XmlNodeType.EndElement)
					{
						if (reader.LocalName == keyword) break;
					}
				}
			}

			catch
			{
				ret = false;
			}
			return ret;
		}

        /// <summary>
        /// XML文字列に変換します。
        /// </summary>
        /// <returns> XML文字列</returns>
        public string ToXmlString()
		{
			string ret = null;
			var stringWriter = new StringWriter();
			var xmlWriter = new XmlTextWriter(stringWriter);
			try
			{
				if (ToXmlWriter(xmlWriter, DefaultKeyword ) == true)
				{
					ret = stringWriter.ToString();
				}
			}
			catch (Exception ex)
			{
				DevelopLog.LogException(MethodBase.GetCurrentMethod(), "ToXmlString()で例外", ex);
				ret = null;
			}
			finally
			{
				xmlWriter.Close();
			}
			return ret;
		}


		/// <summary>
		/// XMLから設定を復元する
		/// </summary>
		/// <param name="xml">XML 文字列</param>
		/// <returns>成否</returns>
		public bool FromXmlString(string xml)
		{
			var stringReader= new StringReader(xml);
			var xmlReader = new XmlTextReader(stringReader);
			return FromXmlReader(xmlReader, DefaultKeyword);
		}



		/// <summary>XMLファイルに保存します </summary>
		/// <param name="filename">保存ファイル名</param>
		/// <returns>成否</returns>
		public bool SaveXmlFile(string filename)
		{
			bool ret = false;
			XmlTextWriter writer = null;
			try
			{
				writer = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
				writer.Formatting = Formatting.Indented;
				writer.WriteStartDocument(true);
				ToXmlWriter(writer, DefaultKeyword);

				writer.WriteEndDocument();
				writer.Flush();
				ret = true;
			}
			catch
			{
				ret = false;
			}
			finally
			{
				if (writer != null)
				{
					writer.Close();
					writer = null;
				}
			}
			return ret;
		}


		/// <summary>XMLファイルより読み込みます </summary>
		/// <param name="filename">ファイル名</param>
		/// <returns>成否</returns>
		public bool LoadXmlFile(string filename)
		{
			bool ret = false;
			var reader = new XmlTextReader(filename);
			ret= FromXmlReader( reader, DefaultKeyword );
			reader.Close();
			return ret;
		}
	}

}
