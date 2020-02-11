using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Windows.Forms;

using DevelopLogSystem;


namespace RulerJB
{
	/// <summary>ブックプロジェクトクラスです</summary>
	class BookProject:IDisposable,IXmlProject
	{
		/// <summary>ブックプロジェクト用設定ファイルを保持します </summary>
		private BookProjectSetData _setData;

		/// <summary>ブックデータを保持します</summary>
		private BookDataRJB _bookData;

		/// <summary>ブックデータを取得します。 </summary>
		public BookDataRJB BookData
		{
			get { return _bookData; }
		}

		/// <summary>ブックプロジェクト用設定ファイルを取得します</summary>
		public BookProjectSetData SetData
		{
			get { return _setData; }
		}

		/// <summary>デフォルトコンストラクタです</summary>
		public BookProject()
		{
			_setData = new BookProjectSetData();
			_bookData = new BookDataRJB();
		}


		/// <summary>コンストラクタ</summary>
		/// <param name="xmlfile">プロジェクトファイル名</param>
		/// <remarks>プロジェクトファイルの記述はXML形式</remarks>
		public BookProject(string xmlfile ):this()
		{
			using( var reader = new XmlTextReader( xmlfile) )
			{
				FromXmlReader(reader, "BookProject");
			}
		}




		/// <summary>インスタンスを破棄します(IDisposable)</summary>
		public void Dispose()
		{
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
				_setData.ToXmlWriter(writer, "SetData");
				_bookData.ToXmlWriter(writer, "BookData");
				writer.WriteEndElement();
			}
			catch
			{
				ret = false;
			}
			return ret;
		}



		/// <summary>Xmlからの読み込みとデータセット</summary>
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
						string localName = reader.LocalName;
						switch (localName)
						{
							case "SetData":
								_setData = new BookProjectSetData(reader, "SetData");
								break;
							case "BookData":
								_bookData = new BookDataRJB(reader, "BookData");
								break;
						}
					}
					else if( reader.NodeType == XmlNodeType.EndElement )
					{
						if( reader.LocalName == keyword ) break;
					}
				}
			}

			catch( Exception ex )
			{
				ret = false;
			}
			return ret;
		}


		/// <summary>XMLファイルに保存します </summary>
		/// <param name="filename">保存ファイル名</param>
		/// <returns>成否</returns>
		public bool SaveXmlFile( string filename )
		{
			bool ret = false;
			XmlTextWriter writer = null;
			try
			{
				writer = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
				writer.Formatting = Formatting.Indented;
				writer.WriteStartDocument(true);
				ToXmlWriter(writer, "BookProject");

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


		/// <summary>
		/// CSVファイル出力を行います。
		/// </summary>
		/// <returns>成否</returns>
		public bool SaveCsvFile(int format)
		{
			string fname;

			Func<string, bool> outCsvFunc = (format == 0) ? ((Func<string, bool>)ExportCsvFileDataType0) : ((Func<string, bool>)ExportCsvFileDataType1);
			var sfd = new SaveFileDialog();
			sfd.Filter = "CSVファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*";
			//[ファイルの種類]ではじめに
			//「すべてのファイル」が選択されているようにする
			sfd.FilterIndex = 1;
			//はじめのファイル名を指定する
			sfd.FileName = Path.GetFileName(BookData.CommonFolderPath)+"_"+ DateTime.Now.ToString("yyyyMMddHHmmss");		// 共通のフォルダ名（通常は親フォルダ）
			//はじめに表示されるフォルダを指定する
			sfd.InitialDirectory = CommonValues.ConfigData.OutputFolder;
			//タイトルを設定する
			sfd.Title = "CSV保存先のファイルを選択してください";
			if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				fname = sfd.FileName;
				CommonValues.ConfigData.OutputFolder = Path.GetDirectoryName(fname);
				if (outCsvFunc(fname) == false)
				{
					MessageBox.Show(String.Format("ファイル（{0}）書き込みが失敗しました", Path.GetFileName(fname)), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
			}
			return true;

		}

		/// <summary> CSVファイルにExportします </summary>
		/// <param title="filepath">ファイル名</param>
		/// <returns>成否を返します</returns>
		private bool ExportCsvFileDataType0(string filename)
		{
			var header = new string[]{ "No.", "Valid", "変更有無", "フォルダ名", "ファイル名", "紙高左px", "匡郭高左px","紙高右px", "匡郭高右px","紙高左mm", "紙高右mm", "匡郭高左mm", "匡郭高右mm", "画像幅px","画像高さpx" };

			var isUsePaper = _setData.IsUseHeightPaper ?? false;		// 設定されていて、かつtrue

			bool ret = false;
			try
			{
				using (StreamWriter sw = new StreamWriter(filename, false, Encoding.GetEncoding("Shift-JIS")))
				{
					sw.WriteLine(String.Join(",", header) );
					foreach (var pd in _bookData.PageDataList )
					{
						var sl = new List<string>();
						sl.Add((pd.PageIdx+1).ToString());		// No.
						sl.Add(pd.IsValidate.ToString());		// Valid
						sl.Add(pd.IsChanged? "有": "無");     // Change
						if (_setData.PdfPath != null && _setData.PdfPath.Any())
						{
							sl.Add(Path.GetDirectoryName(_setData.PdfPath));													//元PDFのファイルパス 
							sl.Add(Path.GetFileName(_setData.PdfPath) + " " + Path.GetFileNameWithoutExtension(pd.FilePath));	//元PDFのファイル名
						}
						else if (_setData.IiifPath != null && _setData.IiifPath.Any())
						{
							string[] path = ManifestToURL();
							for (int i = 0; i < 5; i++)
							{
								path[0] = Path.GetDirectoryName(path[0]);
							}
							sl.Add(path[0]);							// URL
							sl.Add(Path.GetFileName(pd.FilePath));		// TIFFファイル名
						}
						else
						{
							sl.Add(Path.GetDirectoryName(pd.FilePath));		// ファイルパス
							sl.Add(Path.GetFileName(pd.FilePath));			// ファイル名
						}
						sl.Add(pd.SideL.HeightPixPaper.ToString());		// 紙高左px
						sl.Add(pd.SideL.HeightPixGrid.ToString());		// 匡郭高左px
						sl.Add(pd.SideR.HeightPixPaper.ToString());		// 紙高右px
						sl.Add(pd.SideR.HeightPixGrid.ToString());		// 匡郭高右px
						sl.Add((isUsePaper)?_setData.HightPaperSizeMM.ToString() : pd.SideL.HightPaperGridMM(_setData.PixelPerMm).paperMM.ToString());      // 紙高左mm
						sl.Add((isUsePaper)?_setData.HightPaperSizeMM.ToString() : pd.SideR.HightPaperGridMM(_setData.PixelPerMm).paperMM.ToString());      // 紙高右mm
                        sl.Add((isUsePaper)?pd.SideL.HightGridMM(_setData.HightPaperSizeMM).ToString() : pd.SideL.HightPaperGridMM(_setData.PixelPerMm).gridMM.ToString());         // 匡郭高左mm
                        sl.Add((isUsePaper)?pd.SideR.HightGridMM(_setData.HightPaperSizeMM).ToString() : pd.SideR.HightPaperGridMM(_setData.PixelPerMm).gridMM.ToString());         // 匡郭高右mm
                        sl.Add(pd.ImageWidth.ToString());		// 画像幅px
						sl.Add(pd.ImageHeight.ToString());		// 画像高さpx

						sw.WriteLine(String.Join(",", sl.ToArray()));
					}
					sw.Close();
					ret = true;
				}
			}
			catch( Exception ex  )
			{
				DevelopLog.LogException(MethodBase.GetCurrentMethod(), "CSVファイルの書き込みに失敗しました", ex );
				ret = false;
			}
			return ret;
		}


		/// <summary> CSVファイルにExportします </summary>
		/// <param title="filepath">ファイル名</param>
		/// <returns>成否を返します</returns>
		/// <remarks>１画像の左右を別の行として出力します。</remarks>
		private bool ExportCsvFileDataType1(string filename)
		{
			var header = new string[] { "No.", "Valid", "変更有無", "フォルダ名", "ファイル名", "左右", "紙高px", "匡郭高px", "紙高mm", "匡郭高mm", "画像幅px", "画像高さpx" };

			var isUsePaper = _setData.IsUseHeightPaper ?? false;		// 設定されていて、かつtrue
	
			bool ret = false;
			try
			{
				using (StreamWriter sw = new StreamWriter(filename, false, Encoding.GetEncoding("Shift-JIS")))
				{
					sw.WriteLine(String.Join(",", header));
					foreach (var pd in _bookData.PageDataList)
					{

						// 右側
						var sl = new List<string>();
						sl.Add((pd.PageIdx*2 + 1).ToString());	// No.
						sl.Add(pd.IsValidate.ToString());		// Valid
						sl.Add(pd.IsChanged ? "有" : "無");       // Change
						if (_setData.PdfPath != null && _setData.PdfPath.Any())
						{
							sl.Add(Path.GetDirectoryName(_setData.PdfPath));                                                    //元PDFのファイルパス 
							sl.Add(Path.GetFileName(_setData.PdfPath) + " " + Path.GetFileNameWithoutExtension(pd.FilePath));   //元PDFのファイル名
						}
						else if (_setData.IiifPath != null && _setData.IiifPath.Any())
						{
							string[] path = ManifestToURL();
							for (int i = 0; i < 5; i++)
							{
								path[0] = Path.GetDirectoryName(path[0]);
							}
							sl.Add(path[0]);                            // URL
							sl.Add(Path.GetFileName(pd.FilePath));      // TIFFファイル名
						}
						else
						{
							sl.Add(Path.GetDirectoryName(pd.FilePath));     // ファイルパス
							sl.Add(Path.GetFileName(pd.FilePath));          // ファイル名
						}
						sl.Add("右");									// 左右
						sl.Add(pd.SideR.HeightPixPaper.ToString());		// 紙高右px
						sl.Add(pd.SideR.HeightPixGrid.ToString());      // 匡郭高右px
                        sl.Add((isUsePaper) ? _setData.HightPaperSizeMM.ToString() : pd.SideR.HightPaperGridMM(_setData.PixelPerMm).paperMM.ToString());                            // 紙高mm
                        sl.Add((isUsePaper) ? pd.SideR.HightGridMM(_setData.HightPaperSizeMM).ToString() : pd.SideR.HightPaperGridMM(_setData.PixelPerMm).gridMM.ToString());       // 匡郭高右mm
                        sl.Add(pd.ImageWidth.ToString());       // 画像幅px
                        sl.Add(pd.ImageHeight.ToString());      // 画像高さpx

                        sw.WriteLine(String.Join(",", sl.ToArray()));

                        // 左側
                        sl = new List<string>();
                        sl.Add((pd.PageIdx * 2 + 2).ToString());        // No. (２倍になる）
                        sl.Add(pd.IsValidate.ToString());       // Valid
                        sl.Add(pd.IsChanged ? "有" : "無");       // Change
                        sl.Add(Path.GetDirectoryName(pd.FilePath));     // ファイルパス
                        sl.Add(Path.GetFileName(pd.FilePath));          // ファイル名
                        sl.Add("左");                                    // 左右
                        sl.Add(pd.SideL.HeightPixPaper.ToString());     // 紙高px
                        sl.Add(pd.SideL.HeightPixGrid.ToString());      // 匡郭高px
                        sl.Add((isUsePaper) ? _setData.HightPaperSizeMM.ToString() : pd.SideL.HightPaperGridMM(_setData.PixelPerMm).paperMM.ToString());                            // 紙高mm
                        sl.Add((isUsePaper) ? pd.SideL.HightGridMM(_setData.HightPaperSizeMM).ToString() : pd.SideL.HightPaperGridMM(_setData.PixelPerMm).gridMM.ToString());       // 匡郭高左mm
                        sl.Add(pd.ImageWidth.ToString());		// 画像幅px
						sl.Add(pd.ImageHeight.ToString());		// 画像高さpx

						sw.WriteLine(String.Join(",", sl.ToArray()));
					}
					sw.Close();
					ret = true;
				}
			}
			catch (Exception ex)
			{
				DevelopLog.LogException(MethodBase.GetCurrentMethod(), "CSVファイルの書き込みに失敗しました", ex);
				ret = false;
			}
			return ret;
		}

		/// <summary>
		/// manifestから画像のURLを抽出します。
		/// </summary>
		/// <param name="manifest">manifest文</param>
		/// <returns>URLリスト</returns>
		public string[] ManifestToURL()
		{
			string manifest = _setData.IiifPath;
			manifest = System.Text.RegularExpressions.Regex.Replace(manifest, @"[ \n\t]", "");		//半角スペース、\n、\tを削除
			manifest = manifest.Replace(@"\", "");													//\を削除
			var arr = manifest.Split(',');
			var list = new List<string>();
			bool rscFlag = false;
			int scope = 0;
			foreach (string line in arr)
			{
				if (line.StartsWith("\"resource\""))
				{
					rscFlag = true;
				}
				if (rscFlag == true)
				{
					scope += line.Where(c => c == '{').Count();
					if (line.Contains("@id") && scope == 1)
					{
						int urlstart = line.IndexOf("\"@id\":\"") + ("\"@id\":\"").Length;
						int urlend = line.Length - 1;
						list.Add(line.Substring(urlstart, urlend - urlstart));
					}
					scope -= line.Where(c => c == '}').Count();
					if (scope <= 0)
					{
						scope = 0;
						rscFlag = false;
					}
				}
			}
			var url = list.ToArray();

			if (string.IsNullOrEmpty(url[0]))
			{
				MessageBox.Show("iiifのmanifestのURI、もしくはmanifestファイルを指定してください。", "エラー発生", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}
			return url;
		}
	}
}
