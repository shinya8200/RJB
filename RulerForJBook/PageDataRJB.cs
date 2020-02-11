using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Net;

namespace RulerJB
{
	/// <summary>1ページ（1画像）の情報を保持するクラスです</summary>
	class PageDataRJB : IXmlProject
	{

		/// <summary>ページ番号を取得します</summary>
		/// <remarks>ページ番号。0起算でキーとして使用する</remarks>
		public int PageIdx { get; private set; }

		/// <summary>計測情報（左側）を取得します</summary>
		public PageMeasureData SideL { get; private set; }

		/// <summary>計測情報（右側）を取得します</summary>
		public PageMeasureData SideR { get; private set; }

		/// <summary>データの承認状態を取得・設定します</summary>
		public bool IsValidate { get; set; }

		/// <summary>変更があったか否かを取得します</summary>
		public bool IsChanged { get; private set; }

		/// <summary>データパスを取得します</summary>
		public string FilePath { get { return _filePath; } }


		/// <summary>ピクセルのサイズ</summary>
		public double PixToMm { get; private set; }


		/// <summary>デバッグ用イメージを取得します</summary>
		/// <remarks>デバッグ時に使用するイメージです</remarks>
		public Bitmap DebugImage { get; private set; }

		/// <summary>検出ライン（デバッグ用左右） </summary>
		public ShapeLine[] DetectedLines { get; private set; }


		/// <summary>ページ画像のファイルパスを保持します</summary>
		private string _filePath = null;

		/// <summary>AutoDetectの実施有無を取得します</summary>
		public bool IsAutoDetect { get; private set; }


		/// <summary>画像サイズ（幅）のピクセル数を取得します</summary>
		public int ImageWidth { get; private set; }

		/// <summary>画像サイズ（高さ）のピクセル数を取得します</summary>
		public int ImageHeight { get; private set; }

		/// <summary>
		/// 検出アルゴリズムを保持します。
		/// </summary>
		private IDetectProc _detectProc = null;

		/// <summary>コンストラクタです</summary>
		/// <param name="filepath">ファイルパス</param>
		/// <param name="pageNo">ページ番号</param>]
		/// <param name="imgOrg">自動検出を実施する場合イメージを指定します</param>
		/// <remarks>imgがnullの場合、自動解析は実施しません</remarks>
		public PageDataRJB(string filepath, int pageNo, Bitmap img = null)
		{
			SideL = new PageMeasureData();
			SideR = new PageMeasureData();
			InitAll();
			_filePath = filepath;
			IsValidate = false;
			IsAutoDetect = false;
			PageIdx = pageNo;
			ImageWidth = ImageHeight = 0;

			// イメージがある場合にはコンストラクタで自動検出を行います
			if (img != null)
			{
				IsAutoDetect = true;
				AutoDetect(img);
			}
		}

		/// <summary>
		/// ファイルパスを変更します </summary>
		/// <param name="filepath">ファイルパス</param>
		public void SetFilePath(string filepath)
		{
			_filePath = filepath;
		}

		/// <summary>
		/// XMLストリームからのコンストラクタです
		/// </summary>
		/// <param name="reader">XMLストリーム</param>
		/// <param name="keyword">終了キーワード</param>
		public PageDataRJB(XmlReader reader, string keyword)
		{
			SideL = new PageMeasureData();
			SideR = new PageMeasureData();
			InitAll();
			if (FromXmlReader(reader, keyword) == false)
			{
				throw new Exception("PageDataRJBコンストラクタでインスタンス生成に失敗しました.");
			}
		}


		/// <summary>全パラメータを初期化</summary>
		private void InitAll()
		{
			_filePath = null;
			IsValidate = false;
			IsChanged = false;
			PageIdx = 0;
			ImageWidth = ImageHeight = 0;

			InitMeasure();
		}


		/// <summary>測定関連パラメータを初期化</summary>
		private void InitMeasure()
		{
			IsAutoDetect = false;
			IsChanged = false;
			SideL.Init();
			SideR.Init();
			DetectedLines = null;
		}


		/// <summary>画像による自動検出を行います</summary>
		/// <param name="imgOrg">ページ画像。省略またはnullの場合には指定パスから読み込む</param>
		/// <param name="pset">プロジェクト設定値</param>
		/// <param name="prm">汎用パラメータ</param>
		/// <param name="isOldMethod">旧メソッド（アルゴリズム）使用</param>
		/// <returns>成否</returns>
		public bool AutoDetect(Bitmap imgOrg = null, BookProjectSetData pset = null, Dictionary<string, object> prm = null, bool isOldMethod = false)
		{

			InitMeasure();

			// イメージが指定されていない場合には、ファイルパスからイメージを作成する
			var img = imgOrg ?? CreateBitmapFromPath();

			if (img == null) return false;

			// 画像サイズ
			ImageWidth = img.Width;
			ImageHeight = img.Height;


			_detectProc = new DetectTypeROI();

			if (isOldMethod == false) return _detectProc.Exec(img, pset, SideL, SideR, prm);

			// -- 以下、旧アルゴリズム
			int CheckRange = img.Height / 50;   // 上下 CheckRange pixel以内は左側優先
			var height = img.Height;
			var width = img.Width;
			using (var adl = new AutoDetectLines(img))
			{
				if (adl.DetectLinesL.Length >= 1)
				{

					//上下1/3までの範囲で、左側優先でループし、yが±CheckRangeのものを代表のみにする。 判定位置は全てBgnを使用する
					var lstT = new List<ShapeLine>();
					var lstB = new List<ShapeLine>();
					// 上側
					foreach (var w in adl.DetectLinesL.Where(ln => ln.PosBgn.Y < height / 3).OrderBy(ln => ln.PosBgn.X))
					{
						// 範囲内に登録がなければ追加する
						if (lstT.Any(ln => Math.Abs(ln.PosBgn.Y - w.PosBgn.Y) < CheckRange) == false)
						{
							lstT.Add(w);
						}
					}
					// 下側
					foreach (var w in adl.DetectLinesL.Where(ln => ln.PosBgn.Y > height * 2 / 3).OrderBy(ln => ln.PosBgn.X))
					{
						// 範囲内に登録がなければ追加する
						if (lstB.Any(ln => Math.Abs(ln.PosBgn.Y - w.PosBgn.Y) < CheckRange) == false)
						{
							lstB.Add(w);
						}
					}
					var wlstT = lstT.ToArray().OrderBy(w => w.PosBgn.Y).Take(2).ToArray();              // 上から最大2つ
					var wlstB = lstB.ToArray().OrderByDescending(w => w.PosBgn.Y).Take(2).ToArray();    // 下から最大2つ

					var min = Math.Min(wlstT.Length, wlstB.Length);
					var max = Math.Max(wlstT.Length, wlstB.Length);
					// 紙・匡郭ラインあり

					var linePaperT = new ShapeLine(new Point(0, 0), new Point(width - 1, 0));
					var linePaperB = new ShapeLine(new Point(0, height - 1), new Point(width - 1, height - 1));
					var lineGridT = new ShapeLine(new Point(0, 0), new Point(width - 1, 0));
					var lineGridB = new ShapeLine(new Point(0, height - 1), new Point(width - 1, height - 1));

					if (wlstT.Length >= 2)
					{
						linePaperT = wlstT[0];
						lineGridT = wlstT[1];
					}
					else if (wlstT.Length == 1)
					{
						lineGridT = wlstT[0];
					}

					if (wlstB.Length >= 2)
					{
						linePaperB = wlstB[0];
						lineGridB = wlstB[1];
					}
					else if (wlstB.Length == 1)
					{
						lineGridB = wlstB[0];
					}


					// 計測情報セットAction定義
					var setMeasureParam = new Action<PageMeasureData>(s =>
					{
						s.LinePaper = new LinePair(linePaperT, linePaperB);
						s.LineGrid = new LinePair(lineGridT, lineGridB);

						s.HeightPixPaper = (int)s.LinePaper.CalcInterval();
						s.HeightPixGrid = (int)s.LineGrid.CalcInterval();

						// _PEND 仮  bgn寄り 1/3
						var ptop = new Point((s.LinePaper.TopLine.PosBgn.X * 2 + s.LinePaper.TopLine.PosEnd.X) / 3, (s.LinePaper.TopLine.PosBgn.Y * 2 + s.LinePaper.TopLine.PosEnd.Y) / 3); // 中点→左寄り
						var pbtm = new Point(ptop.X, (s.LinePaper.BtmLine.PosBgn.Y + s.LinePaper.BtmLine.PosEnd.Y) / 2);
						s.MeasurePaper = new ShapeLineEx(ptop, pbtm);
						var gtop = new Point((s.LineGrid.TopLine.PosBgn.X * 2 + s.LineGrid.TopLine.PosEnd.X) / 3, (s.LineGrid.TopLine.PosBgn.Y * 2 + s.LineGrid.TopLine.PosEnd.Y) / 3);  // 中点→左寄り
						var gbtm = new Point(gtop.X, (s.LineGrid.BtmLine.PosBgn.Y + s.LineGrid.BtmLine.PosEnd.Y) / 2);
						s.MeasureGrid = new ShapeLineEx(gtop, gbtm);
					});

					setMeasureParam(SideL);
				}

				if (adl.DetectLinesR.Length >= 1)
				{

					//上下1/3までの範囲で、右側優先でループし、yが±CheckRangeのものを代表のみにする。 判定位置は全てEndを使用する
					var lstT = new List<ShapeLine>();
					var lstB = new List<ShapeLine>();
					// 上側
					foreach (var w in adl.DetectLinesR.Where(ln => ln.PosEnd.Y < height / 3).OrderByDescending(ln => ln.PosEnd.X))
					{
						// 範囲内に登録がなければ追加する
						if (lstT.Any(ln => Math.Abs(ln.PosEnd.Y - w.PosEnd.Y) < CheckRange) == false)
						{
							lstT.Add(w);
						}
					}
					// 下側
					foreach (var w in adl.DetectLinesR.Where(ln => ln.PosEnd.Y > height * 2 / 3).OrderByDescending(ln => ln.PosEnd.X))
					{
						// 範囲内に登録がなければ追加する
						if (lstB.Any(ln => Math.Abs(ln.PosEnd.Y - w.PosEnd.Y) < CheckRange) == false)
						{
							lstB.Add(w);
						}
					}
					var wlstT = lstT.ToArray().OrderBy(w => w.PosEnd.Y).Take(2).ToArray();              // 上から最大2つ
					var wlstB = lstB.ToArray().OrderByDescending(w => w.PosEnd.Y).Take(2).ToArray();    // 下から最大2つ

					var min = Math.Min(wlstT.Length, wlstB.Length);
					var max = Math.Max(wlstT.Length, wlstB.Length);
					// 紙・匡郭ラインあり

					var linePaperT = new ShapeLine(new Point(0, 0), new Point(width - 1, 0));
					var linePaperB = new ShapeLine(new Point(0, height - 1), new Point(width - 1, height - 1));
					var lineGridT = new ShapeLine(new Point(0, 0), new Point(width - 1, 0));
					var lineGridB = new ShapeLine(new Point(0, height - 1), new Point(width - 1, height - 1));

					if (wlstT.Length >= 2)
					{
						linePaperT = wlstT[0];
						lineGridT = wlstT[1];
					}
					else if (wlstT.Length == 1)
					{
						lineGridT = wlstT[0];
					}

					if (wlstB.Length >= 2)
					{
						linePaperB = wlstB[0];
						lineGridB = wlstB[1];
					}
					else if (wlstB.Length == 1)
					{
						lineGridB = wlstB[0];
					}


					// 計測情報セットAction定義
					var setMeasureParam = new Action<PageMeasureData>(s =>
					{
						s.LinePaper = new LinePair(linePaperT, linePaperB);
						s.LineGrid = new LinePair(lineGridT, lineGridB);

						s.HeightPixPaper = (int)s.LinePaper.CalcInterval();
						s.HeightPixGrid = (int)s.LineGrid.CalcInterval();

						// _PEND 仮  end寄り2/3の位置
						var ptop = new Point((s.LinePaper.TopLine.PosBgn.X + s.LinePaper.TopLine.PosEnd.X * 2) / 3, (s.LinePaper.TopLine.PosEnd.Y + s.LinePaper.TopLine.PosEnd.Y * 2) / 3); // 中点→右寄り
						var pbtm = new Point(ptop.X, (s.LinePaper.BtmLine.PosEnd.Y + s.LinePaper.BtmLine.PosEnd.Y) / 2);
						s.MeasurePaper = new ShapeLineEx(ptop, pbtm);
						var gtop = new Point((s.LineGrid.TopLine.PosBgn.X + s.LineGrid.TopLine.PosEnd.X * 2) / 3, (s.LineGrid.TopLine.PosEnd.Y + s.LineGrid.TopLine.PosEnd.Y * 2) / 3); // 中点
						var gbtm = new Point(gtop.X, (s.LineGrid.BtmLine.PosEnd.Y + s.LineGrid.BtmLine.PosEnd.Y) / 2);
						s.MeasureGrid = new ShapeLineEx(gtop, gbtm);
					});

					setMeasureParam(SideR);

				}

				//				setMeasureParam(SideR);

				DetectedLines = adl.DetectLinesL.Concat(adl.DetectLinesR).ToArray();

				// AutoDetect実行済みとする
				IsAutoDetect = true;


				//// for debug 
				//if (DebugImage != null) DebugImage.Dispose();
				//DebugImage = ImageLib.CloneBitmapPf(imgOrg, PixelFormat.Format24bppRgb);

				//var g = Graphics.FromImage(DebugImage);
				//var pen = new Pen(Color.Red, 1.0f);
				//foreach (var lin in adl.DetectLines)
				//{
				//    g.DrawLine(pen, lin.PosBgn, lin.PosEnd);
				//}
				//pen = new Pen(Color.Red, 4.0f);
				//if (LinePaper.TopLine.PosBgn.X != 0) g.DrawLine(pen, LinePaper.TopLine.PosBgn, LinePaper.TopLine.PosEnd);
				//if (LinePaper.BtmLine.PosBgn.X != 0) g.DrawLine(pen, LinePaper.BtmLine.PosBgn, LinePaper.BtmLine.PosEnd);
				//pen = new Pen(Color.Yellow, 4.0f);
				//if (LineGrid.TopLine.PosBgn.X != 0) g.DrawLine(pen, LineGrid.TopLine.PosBgn, LineGrid.TopLine.PosEnd);
				//if (LineGrid.BtmLine.PosBgn.X != 0) g.DrawLine(pen, LineGrid.BtmLine.PosBgn, LineGrid.BtmLine.PosEnd);
				////var frm = new FormColorImageS(DebugImage, "for Degug");
				////frm.ShowDialog();
				//g.Dispose();
				//// end debug
			}

			// イメージをクリエイトした場合には、破棄する。
			if (imgOrg == null) img.Dispose();

			return true; // _PEND
		}



		/// <summary>設定されているパスからビットマップインスタンスを生成する</summary>
		/// <returns>生成されたBitmapインスタンス。</returns>
		public Bitmap CreateBitmapFromPath()
		{
			Bitmap img;

			if (FilePath.StartsWith("http"))
			{
				int buffSize = 65536; // 一度に読み込むサイズ
				MemoryStream imgStream = new MemoryStream(); 
				
				//----------------------------
				// Webサーバに要求を投げる
				//----------------------------
				WebRequest req = WebRequest.Create(FilePath);
				using (BinaryReader reader = new BinaryReader(req.GetResponse().GetResponseStream()))
				{
					//--------------------------------------------------------
					// Webサーバからの応答データを取得し、imgStreamに保存する
					//--------------------------------------------------------
					while (true)
					{
						byte[] buff = new byte[buffSize];

						// 応答データの取得
						int readBytes = reader.Read(buff, 0, buffSize);
						if (readBytes <= 0)
						{
							// 最後まで取得した->ループを抜ける
							break;
						}

						// バッファに追加
						imgStream.Write(buff, 0, readBytes);
					}

					img = new Bitmap(imgStream);
				}				
			}
			else 
            {
				if (File.Exists(FilePath) == false) return null;
				// ファイルから画像を読み込む。ピクセルフォーマットが指定できないようなのでFormat24bppRgbではない場合には変換する。
				using (var fs = new System.IO.FileStream(FilePath,	System.IO.FileMode.Open, System.IO.FileAccess.Read))
				{
					img = new Bitmap(fs);
				}
				//img = new Bitmap(FilePath);
			}

			if (img != null && img.PixelFormat != PixelFormat.Format24bppRgb)
			{
				var newimg = ImageLib.CloneBitmapPf(img, PixelFormat.Format24bppRgb);
				img.Dispose();          // 古いイメージを破棄
				img = newimg;           // 置き換え
			}
			return img;
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
				writer.WriteElementString("FilePath", FilePath.ToString());
				writer.WriteElementString("PageIdx", PageIdx.ToString());
				SideL.ToXmlWriter(writer, "SideL");
				SideR.ToXmlWriter(writer, "SideR");
				writer.WriteElementString("PixToMm", PixToMm.ToString());
				writer.WriteElementString("ImageWidth", ImageWidth.ToString());
				writer.WriteElementString("ImageHeight", ImageHeight.ToString());

				if (DetectedLines != null)
				{
					foreach (var lst in DetectedLines)
					{
						writer.WriteElementString("DetectedLine", lst.ToString());
					}
				}
				writer.WriteElementString("IsAutoDetect", IsAutoDetect.ToString());

				// その他デバッグイメージ等は書き込まない

				writer.WriteEndElement();
			}
			catch
			{
				ret = false;
			}
			return ret;
		}


		/// <summary>いずれかの測定位置ラインに変更があったときに再計算を行うメソッドです</summary>
		public void ChangeLine()
		{
			SideL.ChangeLine();
			SideR.ChangeLine();
			if (SideL.IsChanged || SideR.IsChanged) IsChanged = true;
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
				var slList = new List<ShapeLine>();
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						string localName = reader.LocalName;
						switch (localName)
						{
							case "IsValidate":
								IsValidate = bool.Parse(reader.ReadString());
								break;
							case "PageIdx":
								PageIdx = int.Parse(reader.ReadString());
								break;
							case "SideL":
								SideL = new PageMeasureData(reader, "SideL");
								break;
							case "SideR":
								SideR = new PageMeasureData(reader, "SideR");
								break;
							case "IsChanged":
								IsChanged = bool.Parse(reader.ReadString());
								break;
							case "FilePath":
								_filePath = reader.ReadString();
								break;
							case "PixToMm":
								PixToMm = double.Parse(reader.ReadString());
								break;
							case "DetectedLine":
								slList.Add(ShapeLine.Parse(reader.ReadString()));
								break;
							case "IsAutoDetect":
								IsAutoDetect = bool.Parse(reader.ReadString());
								break;
							case "ImageWidth":
								ImageWidth = int.Parse(reader.ReadString());
								break;
							case "ImageHeight":
								ImageHeight = int.Parse(reader.ReadString());
								break;
						}
					}
					else if (reader.NodeType == XmlNodeType.EndElement)
					{
						if (reader.LocalName == keyword) break;
					}
				}
				DetectedLines = slList.ToArray();
			}

			catch (Exception ex)
			{
				var str = String.Format("Msg={0},  Stack={1}", ex.Message, ex.StackTrace);
				ret = false;
			}
			return ret;
		}

#if false
		/// <summary>Xmlからの読み込みとインスタンス生成</summary>
		/// <param name="xmlReader">読み込み用ストリーム</param>
		/// <returns>生成インスタンス（失敗時はnull）</returns>
		private bool FromXmlReader(XmlReader reader, string keyword)
		{
			var ret = true;
			MethodBase methodbase = MethodBase.GetCurrentMethod();
			try
			{
				var slList = new List<ShapeLine>();
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.LocalName.Equals("IsValidate"))
						{
							IsValidate = bool.Parse(reader.ReadString());
						}
						if (reader.LocalName.Equals("PageIdx"))
						{
							PageIdx = int.Parse(reader.ReadString());
						}
						if (reader.LocalName.Equals("SideL"))
						{
							SideL = new PageMeasureData(reader, "SideL");
						}
						if (reader.LocalName.Equals("SideR"))
						{
							SideR = new PageMeasureData(reader, "SideR");
						}
						if (reader.LocalName.Equals("IsChanged"))
						{
							IsChanged = bool.Parse(reader.ReadString());
						}
						if (reader.LocalName.Equals("FilePath"))
						{
							_filePath = reader.ReadString();
						}
						if (reader.LocalName.Equals("PixToMm"))
						{
							PixToMm = double.Parse(reader.ReadString());
						}
						if (reader.LocalName.Equals("DetectedLine"))
						{
							slList.Add(ShapeLine.Parse(reader.ReadString()));
						}
						if (reader.LocalName.Equals("IsAutoDetect"))
						{
							IsAutoDetect = bool.Parse(reader.ReadString());
						}
						if (reader.LocalName.Equals("ImageWidth"))
						{
							ImageWidth = int.Parse(reader.ReadString());
						}
						if (reader.LocalName.Equals("ImageHeight"))
						{
							ImageHeight = int.Parse(reader.ReadString());
						}
					}
					else if (reader.NodeType == XmlNodeType.EndElement)
					{
						if (reader.LocalName == keyword) break;
					}
				}
				DetectedLines = slList.ToArray();
			}

			catch (Exception ex)
			{
				var str = String.Format("Msg={0},  Stack={1}", ex.Message, ex.StackTrace);
				ret = false;
			}
			return ret;
		}
#endif

		/// <summary>カーソルをデフォルト位置にします </summary>
		/// <param name="ifCondition">条件によりセットする場合、真とする</param>
		/// <param name="set">設定値</param>
		/// <remarks>ifConditionがセットされている場合には、nullのものだけ設定される</remarks>
		public void SetDefaultCursor(bool ifCondition, BookProjectSetData set)
		{
			const double MinLen = 10.0;
			if (ifCondition == false || SideL.MeasurePaper == null || SideL.MeasurePaper.GetDistance() < MinLen)
			{
				SideL.MeasurePaper = new ShapeLineEx(set.RoiPTL.GetCenterPosition(), set.RoiPBL.GetCenterPosition());
			}
			if (ifCondition == false || SideL.MeasureGrid == null || SideL.MeasureGrid.GetDistance() < MinLen)
			{
				SideL.MeasureGrid = new ShapeLineEx(set.RoiGTL.GetCenterPosition(), set.RoiGBL.GetCenterPosition(), SideL.MeasurePaper);
			}
			if (ifCondition == false || SideR.MeasurePaper == null || SideR.MeasurePaper.GetDistance() < MinLen)
			{
				SideR.MeasurePaper = new ShapeLineEx(set.RoiPTR.GetCenterPosition(), set.RoiPBR.GetCenterPosition());
			}
			if (ifCondition == false || SideR.MeasureGrid == null || SideR.MeasureGrid.GetDistance() < MinLen)
			{
				SideR.MeasureGrid = new ShapeLineEx(set.RoiGTR.GetCenterPosition(), set.RoiGBR.GetCenterPosition(), SideR.MeasurePaper);
			}
			if (SideL.MeasurePaper != null) SideL.MeasurePaper._link = SideL.MeasureGrid;
			if (SideR.MeasurePaper != null) SideR.MeasurePaper._link = SideR.MeasureGrid;
		}
	}
}


