using System;
using System.Collections.Generic;
using System.ComponentModel;			// [Category]等の指定
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;			// [Category]等の指定
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Reflection;

using DevelopLogSystem;


namespace RulerJB
{
	/// <summary>環境設定データクラスです</summary>
	class ConfigurationData
	{
		#region 01_内部メンバー定義 -------		
		
		// --- 設定値（ユーザー設定変更可能）
		/// <summary> 最後に指定した入力フォルダを保持します </summary>
		private string _dataFolder = null;

		/// <summary>拡大・縮小時の補間方法を保持します</summary>
		private InterpolationMode _imageInterpolationMode;

		// --- 内部保存値（ユーザー設定不可）
		/// <summary> 最後に指定した入力フォルダを保持します </summary>
		private string _inputFolder = null;
		/// <summary> 最後に指定した出力フォルダを保持します </summary>
		private string _outputFolder = null;
		/// <summary>ウィンドウサイズ</summary>
		private Size _windowSize;

		/// <summary>スプリッター位置（SplitterDistance）を保持します</summary>
		private int _splitterDistance;

		#endregion // 内部メンバー定義 -------


		#region 02_全メンバー対象メソッド

		/// <summary>コンストラクタです（同一オブジェクト生成用）</summary>
		/// <param name="datFolder">データフォルダ</param>
		/// <param name="inputFolder">最後に指定した入力フォルダ</param>
		/// <param name="outputFolder">最後に指定した出力フォルダ</param>
		/// <param name="imageInterpolationMode">メイン表示画像の拡大・縮小時の方法</param>
		/// <param name="winsize">ウィンドウサイズ</param>
		/// <param name="splitter">スプリッター位置（SplitterDistance）</param>
		private ConfigurationData(string datFolder, string inputFolder, string outputFolder, InterpolationMode imageInterpolationMode, Size winsize, int splitter )
		{
			// 全メンバーのコピー
			_dataFolder = datFolder;
			_inputFolder = inputFolder;
			_outputFolder = outputFolder;
			_imageInterpolationMode = imageInterpolationMode;
			_windowSize= winsize;
			_splitterDistance = splitter;
		}


		/// <summary>
		/// デフォルトコンストラクタです
		/// </summary>
		public ConfigurationData()
		{
			SetDefaultValue();
		}


		/// <summary> デフォルト値のセット </summary>
		public void SetDefaultValue()
		{
			_dataFolder = CommonValues.DefaultSettingFolder;
			_inputFolder = null;
			_outputFolder = null;
			_imageInterpolationMode = InterpolationMode.NearestNeighbor; // 最近傍法をデフォルトとする
			_windowSize = new Size(1200, 700);
			_splitterDistance = _windowSize.Width * 3 / 5;

		}

		/// <summary>全メンバー比較を行います　（内部処理用）</summary>
		/// <param name="datFolder">データフォルダ</param>
		/// <param name="inputFolder">最後に指定した入力フォルダ</param>
		/// <param name="outputFolder">最後に指定した出力フォルダ</param>
		/// <param name="imageInterpolationMode">メイン表示画像の拡大・縮小時の方法</param>
		/// <param name="winsize">ウィンドウサイズ</param>
		/// <param name="split">スプリッター位置（SplitterDistance）</param>
		/// <remarks>フォルダ名は文字列比較String.Compare()を使用。大文字小文字の違いも有効。null指定も可</remarks>
		public int CompareAllMenber(string datFolder, string inputFolder, string outputFolder, InterpolationMode imageInterpolationMode, Size winsize, int split )
		{
			// 全メンバーの比較
			int w = String.Compare(this._dataFolder, datFolder);
			if (w != 0) return w;

			w = String.Compare(this._inputFolder, inputFolder);
			if (w != 0) return w;

			w = String.Compare(_outputFolder, outputFolder);
			if (w != 0) return w;

			w = (int)_imageInterpolationMode.CompareTo(imageInterpolationMode);
			if (w != 0) return w;

			w = _windowSize.Width - winsize.Width;
			if (w != 0) return w;

			w = _windowSize.Height - winsize.Height;
			if (w != 0) return w;

			w = _splitterDistance - split;
			if (w != 0) return w;

			return 0;		// 一致
		}


		#endregion // 全メンバー対象メソッド


		#region 03_プロパティ ----------

		/// <summary> デフォルトの設定値ファイル名を取得します </summary>
		/// <returns></returns>
		public string GetDefaultFileName()			// 設定値のデフォルトファイル名を返す
		{
			string appPath = Application.ExecutablePath;				// ファイル名も含めた実行ファイルパス
			return Path.Combine( Path.GetDirectoryName(appPath), Path.GetFileNameWithoutExtension(appPath) + ".xml");	// 設定値はデータフォルダに関係なく実行形式フォルダ
		}

		/// <summary> デフォルトの設定値ファイル名を取得します </summary>
		/// <returns></returns>
		public string GetDefaultExportCsv()			// 設定値（Export/CSV）のデフォルトファイル名を返す
		{
			string appPath = Application.ExecutablePath;				// ファイル名も含めた実行ファイルパス
			return Path.Combine(CommonValues.DefaultSettingFolder, Path.GetFileNameWithoutExtension(appPath) + ".csv");	// 設定値はデータフォルダに関係なく実行形式フォルダ
		}


		[Category("１．全般")]
		[Description("データフォルダの位置を指定します（空白デフォルトフォルダ）")]
		[DefaultValue("")]
		[Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[PropertyDisplayName("01.設定値フォルダ")]
		/// <summary>データフォルダの位置を指定します</summary>
		public string DataFolder
		{
			get
			{
				return (_dataFolder == null || _dataFolder.Length == 0) ? CommonValues.DefaultSettingFolder : _dataFolder;
			}
			set { _dataFolder = value; }
		}



		[Category("２．画像設定")]
		[Description("表示画像の拡大・縮小時の方法")]
		[DefaultValue(InterpolationMode.NearestNeighbor)]
		[PropertyDisplayName("描画（補間）方式")]
		/// <summary> メイン表示画像の拡大・縮小時の方法を取得または設定します </summary>
		public InterpolationMode ImageInterpolationMode
		{
			set { _imageInterpolationMode = value; }
			get { return _imageInterpolationMode; }
		}


	
		// -- 内部設定値（ユーザ設定しない・Exportしない）--

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

		/// <summary> スプリッター位置（SplitterDistance）を取得または設定します </summary>
		/// <remarks>非ユーザー設定項目</remarks>
		[Category("内部設定")]
		[Description("スプリッター位置")]
		[PropertyDisplayName("03. スプリッター位置")]
		[ReadOnly(true)]
		public int SplitterDistance
		{
			set { _splitterDistance = value; }
			get { return _splitterDistance; }
		}
		#endregion // 03_プロパティ ----------


		#region 04_Save・Load -------------
		/// <summary> 設定ファイルをロードします </summary>
		/// <param title="fileName">ファイル名（パス）</param>
		/// <returns>成否を返します。（成功：true  失敗:false）</returns>
		public bool LoadSettingData(string fileName)			// 設定値の読み込み
		{
			bool ret = true;
			MethodBase methodbase = MethodBase.GetCurrentMethod();
			XmlTextReader reader = null;

			if (File.Exists(fileName) == false)
			{
				DevelopLog.LogINF(methodbase, String.Format("ファイルが存在しなかったので、デフォルト値をセットしています。ファイル名:{0}", fileName));
				SetDefaultValue();
				return ret;
			}
			try
			{
				reader = new XmlTextReader(fileName);
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.LocalName.Equals("DataFolder"))
						{
							DataFolder = reader.ReadString();			// _dataFolder
						}
						if (reader.LocalName.Equals("ImageInterpolationMode"))
						{
							var n = reader.ReadString();
							var w = (InterpolationMode)Enum.Parse(typeof(InterpolationMode), n);
							_imageInterpolationMode = w;
						}
						if (reader.LocalName.Equals("InputFolder"))
						{
							InputFolder = reader.ReadString();
						}
						if (reader.LocalName.Equals("OutputFolder"))
						{
							OutputFolder = reader.ReadString();
						}
						if (reader.LocalName.Equals("WindowSize"))
						{
							var n = reader.ReadString();
							Regex r = new Regex(@".*?(\d+)\,.*?(\d+)");
							var m = r.Match(n);
							if (m.Success)
							{
								string w = m.Groups[1].ToString();
								string h = m.Groups[2].ToString();
								WindowSize = new Size(int.Parse(m.Groups[1].ToString()), int.Parse(m.Groups[2].ToString())); ;
							}
						}
						if (reader.LocalName.Equals("SplitterDistance"))
						{
							_splitterDistance = int.Parse(reader.ReadString());
						}

					}
				}
			}
			catch
			{
				ret = false;
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
					reader = null;
				}
			}
			return ret;
		}


		/// <summary> 設定値をロードします。(デフォルトファイル名） </summary>
		/// <returns>成否を返します。（成功：true  失敗:false）</returns>
		public bool LoadSettingData()						// 引数無し設定値ロード
		{
			return LoadSettingData(GetDefaultFileName());			// デフォルトの設定値ファイルをロード
		}


		/// <summary> 設定値を保存します </summary>
		/// <param title="fileName">設定値ファイル名</param>
		/// <returns>成否を返します。（成功：true  失敗:false）</returns>
		public bool SaveSettingData(string fileName)		// 設定値の書き込み
		{
			bool ret = true;
			XmlTextWriter writer = null;
			try
			{
				writer = new XmlTextWriter(fileName, System.Text.Encoding.UTF8);
				writer.Formatting = Formatting.Indented;
				writer.WriteStartDocument(true);
				writer.WriteStartElement("ConfigurationData");
				writer.WriteElementString("DataFolder", DataFolder);
				writer.WriteElementString("ImageInterpolationMode", _imageInterpolationMode.ToString());
				writer.WriteElementString("InputFolder", InputFolder.ToString());
				writer.WriteElementString("OutputFolder", OutputFolder.ToString());
				writer.WriteElementString("WindowSize", WindowSize.Width.ToString() + "," + WindowSize.Height.ToString());
				writer.WriteElementString("SplitterDistance", SplitterDistance.ToString());
				writer.WriteEndElement();
				writer.WriteEndDocument();
				writer.Flush();
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



		/// <summary>デフォルトファイル名に設定値を保存します </summary>
		/// <returns>成否を返します。（成功：true  失敗:false）</returns>
		public bool SaveSettingData()						// 引数無し設定値セーブ
		{
			return SaveSettingData(GetDefaultFileName());			// デフォルトの設定値ファイルにセーブ
		}



		/// <summary> CSVファイルにExportします </summary>
		/// <param title="filename">ファイル名</param>
		/// <returns>成否を返します</returns>
		public bool ExportCsvFile(string filename)
		{
			bool ret = false;
			DevelopLog.LogOPR(System.Reflection.MethodBase.GetCurrentMethod(), String.Format("設定値をCSVにExportします。→{0}", filename));
			try
			{
				using (StreamWriter sw = new StreamWriter(filename, false, Encoding.GetEncoding("Shift-JIS")))
				{
					sw.WriteLine(String.Format("{0},{1}", "DataFolder", DataFolder.ToString()));
					sw.WriteLine(String.Format("{0},{1}", "ImageInterpolationMode", ImageInterpolationMode.ToString()));
					sw.WriteLine(String.Format("{0},{1}", "InputFolder", InputFolder.ToString()));
					sw.WriteLine(String.Format("{0},{1}", "OutputFolder", OutputFolder.ToString()));
					sw.WriteLine(String.Format("{0},{1}", "WindowSize", WindowSize.ToString()));
					sw.WriteLine(String.Format("{0},{1}", "SplitterDistance", SplitterDistance.ToString()));
					sw.Close();
					ret = true;				// 成功
				}
			}
			catch (Exception ex)
			{
				DevelopLog.LogException(System.Reflection.MethodBase.GetCurrentMethod(), "保存に失敗しました", ex);
				ret = false;
			}
			return ret;
		}
		#endregion // 04_Save・Load -------------

	}
 

}
