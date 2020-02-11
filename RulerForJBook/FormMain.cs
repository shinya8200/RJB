using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;

using SyPoint = System.Drawing.Point;
using CvPoint = OpenCvSharp.Point;
using SySize = System.Drawing.Size;
using CvSize = OpenCvSharp.Size;

using OpenCvSharp;

using DevelopLogSystem;
using System.Net;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;

namespace RulerJB
{
	/// <summary>
	/// メインフォームです </summary>
	public partial class FormBookMain : Form
	{
		/// <summary>開発者ログインスタンスを保持します</summary>
		private DevelopLog _log = DevelopLog.GetInstance();

		/// <summary>ピクチャーボックスの拡大・縮小処理クラスインスタンス</summary>
		private ScalablePbox _scalePbox = null;

		/// <summary>メイン画面用レイヤービットマップ</summary>
		private Bitmap _mainImageBaseLayer = null;

		/// <summary>実行中メッセージフォームを保持します</summary>
		private FormExecAllAutoDetect _formExecAllDetect = null;

		///// <summary>処理中のブックデータを保持します</summary>
		//private BookDataRJB _bookData;

		// UI スレッドへのスケジュール用
		private TaskScheduler _UISyncContext = null;

		// プロジェクトを保持します
		private BookProject _project = null;

		/// <summary>計測点カーソル選択状態を保持します</summary>
		private PpmMes _ppmModeMes = PpmMes.Non;

		/// <summary>選択されているカーソルを保持します</summary>
		private ShapeLineEx _selectLine = null;

		/// <summary>表示しているカーソル群を保持します</summary>
		private ShapeLineEx[] _curLines = null;

		/// <summary>ROIカーソル選択状態を保持します</summary>
		private PpmRoi _ppmModeRoi = PpmRoi.Non;

		/// <summary>選択されているROIカーソルを保持します</summary>
		private BookROI _selectRoi = null;

		/// <summary>選択されているROIの順序（「紙」反時計回り、「匡郭」反時計回りの0～7）を保持します</summary>
		private int _selectRoiIndex;

		/// <summary>選択されているROIカーソルポイント情報を保持します</summary>
		private BookROI.PosInf? _selectRoiPos = null;

		/// <summary>ROI編集時のカーソル移動範囲1（Top,Left)</summary>
		private SyPoint _curAreaRoiTL;

		/// <summary>ROI編集時のカーソル移動範囲2（Bottom,Right)</summary>
		private SyPoint _curAreaRoiBR;


		/// <summary>ROI設定開始時の設定値を保持します。</summary>
		private string _saveSettingXmlForRoi = null;

		/// <summary>クリック時の位置を保持します</summary>
		private SyPoint _onClickPos;

		/// <summary>クリック・移動時の位置を保持します</summary>
		private SyPoint _lastPos;

		/// <summary> 表示上で、距離２乗値がこの値以内であれば「近い」と判断します </summary>
		private const int JudgeLimit = 10 * 10;             // 半径10ピクセル

		/// <summary>実行時にマスクされるコントロール群を登録します</summary>
		private Control[] _maskExecControls = null;

		/// <summary>Roiテーブルを保持します。</summary>
		/// <remarks>ワークなので使用する度にセットして使用します</remarks>
		private BookROI[] _allRois = new BookROI[0];


		/// <summary>PDFファイルの中身を一時的にjpgファイルに保存するフォルダ</summary>
		private string _pdfworkPath;

		/// <summary>PDFファイルの中身を一時的にjpgファイルに保存するフォルダ名を定義します</summary>
		const string PdfWorkFolderName = "_pdfwork";

		/// <summary>IIIFファイルの中身を一時的にtifファイルに保存するフォルダ</summary>
		private string _iiifworkPath;

		/// <summary>IIIFファイルの中身を一時的にtifファイルに保存するフォルダ名を定義します</summary>
		const string IiifWorkFolderName = "_iiifwork";

		#region Enum -----------------------
		/// <summary>計測点カーソル選択状態を示します</summary>
		private enum PpmMes
		{
			/// <summary>非選択状態 </summary>
			Non,
			/// <summary>開始点選択状態 </summary>
			Bgn,
			/// <summary>終了点選択状態 </summary>
			End,
			/// <summary>全体選択状態 </summary>
			All
		}


		/// <summary>ROIカーソル選択状態を示します</summary>
		private enum PpmRoi
		{
			/// <summary>非選択状態 </summary>
			Non,
			/// <summary>点選択状態 </summary>
			Pos,
			/// <summary>全体選択状態 </summary>
			All
		}

		/// <summary>ファイルの種類を示します</summary>
		private enum FileType
		{
			/// <summary>IIIF画像</summary>
			IIIF,
			/// <summary>PDF画像</summary>
			PDF,
			/// <summary>フォルダ指定</summary>
			FOLDER,
			/// <summary>その他画像ファイル</summary>
			IMAGE_FILE
		}

		#endregion // Enum -----------------

		/// <summary>コンストラクタです</summary>
		public FormBookMain()
		{
			InitializeComponent();
			// 設定値を読み込む
			CommonValues.ConfigData.LoadSettingData();
			this.Size = CommonValues.ConfigData.WindowSize;
			this.splitContainerMain.SplitterDistance = CommonValues.ConfigData.SplitterDistance;

			pictureBoxMain.SizeMode = PictureBoxSizeMode.Zoom;


			// アプリケーションフォルダ以下に RJBv01 
			// デフォルトフォルダ（必ず存在する）以下にDevelopLogフォルダが存在する場合、ログ出力
			_log.LogFilePath = Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), CommonValues.AppFolderName, @"DevelopLog");
			_log.Start();       // ログの開始


			// プロジェクト
			_project = new BookProject();

			// UI スレッドへのスケジュール用
			_UISyncContext = TaskScheduler.FromCurrentSynchronizationContext();

			// 実行時にマスクされるコントロール群
			_maskExecControls = new Control[] { btnExec, btnOutCsv, btnExit };

			//PDFファイル読み込み時のフォルダpath
			_pdfworkPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), PdfWorkFolderName);
			try { if (Directory.Exists(_pdfworkPath)) Directory.Delete(_pdfworkPath, true); }
			catch { }

			//IIIFファイル読み込み時のフォルダpath
			_iiifworkPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), IiifWorkFolderName);
			try { if (Directory.Exists(_iiifworkPath)) Directory.Delete(_iiifworkPath, true); }
			catch { }
		}


		/// <summary>
		/// 画像ファイルまたは画像のあるフォルダから新規ブックを作成します
		/// </summary>
		/// <param name="files">ファイル群。フォルダの場合は</param>
		private void SetNewBook(string[] files)
		{
			if (_formExecAllDetect != null)
			{
				MessageBox.Show("データ実行中です。　無視します。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (files != null && files.Length > 0)
			{
				_project = new BookProject();
				_project.BookData.Clear();
				var createDirectory = new CreateDirectory();
				FileType fileType;
				//PDFまたはIIIFの場合
				if (files[0].Contains(@"@context") && files[0].Contains(@"@id") && files[0].Contains(@"@type"))
					fileType = FileType.IIIF;
				else if (Path.GetExtension(files[0]).ToUpper() == ".PDF")
					fileType = FileType.PDF;
				else if (files.Length == 1 && Directory.Exists(files[0]) == true)
					fileType = FileType.FOLDER;
				else
					fileType = FileType.IMAGE_FILE;

				if (fileType == FileType.IIIF || fileType == FileType.PDF)
				{
					createDirectory.ShowDialog(this);
					if (createDirectory._createDir)
					{
						try
						{
							var sfd = new SaveFileDialog();                                 // ファイルダイアログ

							//はじめに表示されるフォルダを指定する
							if (CommonValues.ConfigData.OutputFolder != null)
							{
								sfd.InitialDirectory = CommonValues.ConfigData.OutputFolder;
							}

							sfd.Filter = String.Format("画像ファイル(*.jpg;*.bmp;*.png;*.tif;*.jpeg;*.tiff)|*.jpg;*.bmp;*.png;*.tif;*.jpeg;*.tiff");    //[ファイルの種類]に表示される選択肢を指定する
							sfd.FilterIndex = 0;																										//[ファイルの種類]ではじめに「プロジェクトファイル」が選択されているようにする
							sfd.Title = "保存するファイル名を指定してください";																			//タイトルを設定する
																								
							//はじめのファイル名を指定する
							if (fileType == FileType.IIIF )
							{
								_project.SetData.IiifPath = files[0];
								sfd.FileName = "元画像のファイル名で保存されます";
							}
							else
							{
								sfd.FileName = Path.GetFileNameWithoutExtension(files[0]);
							}

							sfd.InitialDirectory = CommonValues.ConfigData.OutputFolder;	//はじめに表示されるフォルダを指定する
							sfd.RestoreDirectory = true;                                    //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする

							var ret = sfd.ShowDialog();
							if (ret == System.Windows.Forms.DialogResult.OK)
							{
								if (fileType == FileType.IIIF)
								{
									IiiftoJpg(_project.ManifestToURL(), sfd.FileName);
									_project.SetData.IiifPath = null;
								}
								else
								{
									PdftoJpg(files[0], sfd.FileName);
									_project.SetData.PdfPath = null;
								}
								_project.BookData.SetDataNameFromFolder(Path.GetDirectoryName(sfd.FileName));

								// 出力フォルダを設定値に保存
								CommonValues.ConfigData.OutputFolder = Path.GetDirectoryName(sfd.FileName);
							}
						}
						catch (Exception ex)
						{
							MessageBox.Show("画像ファイルの保存で例外を発生しました。", "例外発生", MessageBoxButtons.OK, MessageBoxIcon.Error);
							DevelopLog.LogException(MethodBase.GetCurrentMethod(), "画像ファイル保存で例外を発生しました。", ex);
						}
					}
					else
					{
						//IIIFファイルの場合
						if (fileType == FileType.IIIF)
						{
							_project.SetData.IiifPath = files[0];
							// URLからJPGファイル作成
							IiiftoJpg(_project.ManifestToURL(), _iiifworkPath);
							_project.BookData.SetDataNameFromFolder(_iiifworkPath);
						}
						//PDFファイルの場合
						else if (fileType == FileType.PDF)
						{
							_project.SetData.PdfPath = files[0];
							PdftoJpg(files[0],_pdfworkPath);
							_project.BookData.SetDataNameFromFolder(_pdfworkPath);
						}
					}
				}
				//フォルダの場合
				else if (fileType == FileType.FOLDER) 
				{
					_project.BookData.SetDataNameFromFolder(files[0]);      // フォルダ
				}
				//画像ファイルの場合
				else
				{
					_project.BookData.SetDataNameFromFiles(files);
				}
			}
			// 対象となるデータ数のチェック
			if (_project.BookData.IsValid() == false)
			{
				MessageBox.Show(String.Format("指定されたデータが不正です"), "データなし", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else
			{
				if (_project.BookData.PageDataList.Any())
				{
					// 最初のデータでデフォルトのROI位置を設定する
					_project.BookData.CurrentIndex = 0;
					//_project.SetData.AdjustDefaultRoi(_project.BookData.CurrentImage.Size);
					_project.SetData.ResetRoi(_project.BookData.CurrentImage.Size);
					cboxRoiInfo.Checked = true;

					////　実行ダイアログ
					//_formExecAllDetect = new FormExecAllAutoDetect();
					//_formExecAllDetect.SetMaxCount(_project.BookData.PageDataList.Length);
					//_formExecAllDetect.Show();
					//var t = Task.Factory.StartNew(() =>
					//    {
					//        // 別スレッドで実行
					//        _project.BookData.AllAutoDetect(OnCountAutoDetect); // _PEND_
					//        Thread.Sleep(10);
					//    }
					//);
					DrawDataGridViewList();
					DispNewData();
					Refresh();
				}
			}
			_allRois = new BookROI[] { _project.SetData.RoiPTL, _project.SetData.RoiPBL, _project.SetData.RoiPBR, _project.SetData.RoiPTR, _project.SetData.RoiGTL, _project.SetData.RoiGBL, _project.SetData.RoiGBR, _project.SetData.RoiGTR };
		}

		/// <summary>PDFファイルから画像を抽出します</summary>
		/// <returns>JPEG画像のバイト列</returns>
		public static IEnumerable<byte[]> ExtractJpegs(string pdfPath)
		{
			using (var doc = PdfReader.Open(pdfPath, PdfDocumentOpenMode.ReadOnly))
			{
				return doc.Pages
					.Cast<PdfPage>()
					.Select(page => page.Elements.GetDictionary("/Resources"))
					.Where(res => res != null)
					.Select(res => res.Elements.GetDictionary("/XObject"))
					.Where(xobj => xobj != null)
					.SelectMany(xobj => xobj.Elements.Values)
					.OfType<PdfReference>()
					.Select(r => r.Value)
					.OfType<PdfDictionary>()
					.Where(xobj => xobj != null && xobj.Elements.GetString("/Subtype") == "/Image")
					.Where(image => image.Elements.GetName("/Filter") == "/DCTDecode")
					.Select(image => image.Stream.Value);
			}
		}

		/// <summary>PDFファイルからjpgファイルを生成します</summary>
		/// <param name="path">PDFファイルのパス</param>
		public void PdftoJpg(string path, string savePath)
		{
			IEnumerable<byte[]> datalist = ExtractJpegs(path);
			if (datalist == null || !datalist.Any())
			{
				MessageBox.Show(String.Format("画像データが見つかりませんでした。"), "データなし", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (savePath == _pdfworkPath)
			{
				try { if (Directory.Exists(_pdfworkPath)) Directory.Delete(_pdfworkPath, true); }
				catch { }
				Directory.CreateDirectory(_pdfworkPath);
			}

			FileOpenProgressBar fileOpenProgressBar = new FileOpenProgressBar();
			fileOpenProgressBar.Show();
			fileOpenProgressBar.Update();
			foreach (var (data, index) in datalist.Select((data, index) => (data, index)))
			{
				fileOpenProgressBar.value = 100 * (index + 1) / datalist.Count();
				fileOpenProgressBar.UpdateBar();
				MemoryStream ms = new MemoryStream(data);
				Bitmap img = new Bitmap(ms);
				if (savePath == _pdfworkPath)
					img.Save(_pdfworkPath + @"\\" + String.Format("{0:D5}", (index + 1)) + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
				else
				{
					var ext = Path.GetExtension(savePath).ToLower();
					ImageFormat fmt;
					if (ext == ".jpg" || ext == ".jpeg")
						fmt = System.Drawing.Imaging.ImageFormat.Jpeg;
					else if (ext == ".tif" || ext == ".tiff")
						fmt = System.Drawing.Imaging.ImageFormat.Tiff;
					else if (ext == ".bmp")
						fmt = System.Drawing.Imaging.ImageFormat.Bmp;
					else if (ext == ".png")
						fmt = System.Drawing.Imaging.ImageFormat.Png;
					else
					{
						ext = ".jpg";
						fmt = System.Drawing.Imaging.ImageFormat.Jpeg;
					}
					img.Save(Path.GetDirectoryName(savePath) + "\\" + Path.GetFileNameWithoutExtension(savePath) + String.Format("_{0:D5}", (index + 1)) + ext, fmt);
				}
				img.Dispose();
				ms.Close();
			}
			fileOpenProgressBar.Close();
		}

		/// <summary>IIIFファイルからjpgファイルを生成します</summary>
		/// <param name="url">IIIFファイルのURL</param>
		public void IiiftoJpg(string[] urllist, string savePath)
		{
			if (urllist == null || !urllist.Any())
			{
				MessageBox.Show(String.Format("画像データが見つかりませんでした。"), "データなし", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (savePath == _iiifworkPath) 
			{
				try { if (Directory.Exists(_iiifworkPath)) Directory.Delete(_iiifworkPath, true); }
				catch { }
				Directory.CreateDirectory(_iiifworkPath);
			}

			FileOpenProgressBar fileOpenProgressBar = new FileOpenProgressBar();
			fileOpenProgressBar.Show();
			fileOpenProgressBar.Update();
			int buffSize = 65536; // 一度に読み込むサイズ
			foreach (var (url, index) in urllist.Select((url, index) => (url, index)))
			{
				MemoryStream imgStream = new MemoryStream();
				fileOpenProgressBar.value = 100 * (index + 1) / urllist.Count();
				fileOpenProgressBar.UpdateBar();

				//----------------------------
				// Webサーバに要求を投げる
				//----------------------------
				try
				{
					WebRequest req = WebRequest.Create(url);
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
						string filename = url;
						for (int i = 0; i < 4; i++)
						{
							filename = Path.GetDirectoryName(filename);
						}
						filename = Path.GetFileNameWithoutExtension(filename);

						Bitmap img = new Bitmap(imgStream);

						if (savePath == _iiifworkPath)
						{
							img.Save(_iiifworkPath + @"\\" + filename + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
						}
						else
						{
							var ext = Path.GetExtension(savePath).ToLower();
							ImageFormat fmt;
							if (ext == ".jpg" || ext == ".jpeg")
								fmt = System.Drawing.Imaging.ImageFormat.Jpeg;
							else if (ext == ".tif" || ext == ".tiff")
								fmt = System.Drawing.Imaging.ImageFormat.Tiff;
							else if (ext == ".bmp")
								fmt = System.Drawing.Imaging.ImageFormat.Bmp;
							else if (ext == ".png")
								fmt = System.Drawing.Imaging.ImageFormat.Png;
							else
							{
								ext = ".jpg";
								fmt = System.Drawing.Imaging.ImageFormat.Jpeg;
							}
							img.Save(Path.GetDirectoryName(savePath) + "\\" + filename + ext, fmt);
						}

						img.Dispose();
					}
				}
				catch
				{
					MessageBox.Show(String.Format("ネットワークで異常が発生しました。"), "例外発生", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
				imgStream.Dispose();
			}
			fileOpenProgressBar.Close();
		}

		/// <summary>AllAutoDetect()からのカウントアップイベント処理です</summary>
		/// <param name="n">カウント</param>
		/// <param name="m">全データ数</param>
		private void OnCountAutoDetect(int n, int m)
		{
			if (_formExecAllDetect == null) return;

			// カウントアップおよび終了処理をUIスレッドで実行させる
			Task reportProgressTask = Task.Factory.StartNew(() =>
			{
				// 表示用ダイアログにカウント値を表示
				_formExecAllDetect.SetCount(n, m);
				_formExecAllDetect.Refresh();
				if (n >= m)
				{
					// 終了
					_formExecAllDetect.Close();
					_formExecAllDetect.Dispose();
					_formExecAllDetect = null;
					DrawDataGridViewList(false);
					DispNewData();
					Refresh();
				}
			},
			CancellationToken.None,
			TaskCreationOptions.None,
			_UISyncContext);
		}


		#region ドラッグアンドドロップ処理 --------------------------------------------

		/// <summary>
		/// ドラッグ＆ドロップの処理
		/// </summary>
		/// <param title="sender">送信元</param>
		/// <param title="e">イベント情報</param>
		/// <remarks>最終入力フォルダには保存していない。（変更したくない場合ドラッグアンドドロップすれば良い）</remarks>
		private void FormMain_DragDrop(object sender, DragEventArgs e)
		{
			// データロード
			try
			{
				//ドロップされたデータがFileDrop型か調べる
				if (e.Data.GetDataPresent(DataFormats.FileDrop))
				{
					string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
					SetNewBook(files);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("データ生成で例外を発生しました。入力データのフォーマットを確認してください." + ex.Message + "  " + ex.StackTrace, "例外発生", MessageBoxButtons.OK, MessageBoxIcon.Error);
				DevelopLog.LogException(MethodBase.GetCurrentMethod(), "入力データ生成で例外を発生しました。入力データのフォーマットを確認してください", ex);
			}

		}


		/// <summary>
		/// マウスDragEnter処理
		/// </summary>
		/// <param title="sender">送信元</param>
		/// <param title="e">イベント情報</param>
		private void FormMain_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.None;
			////　実行中は無視する
			//if (_processManager.ProcessIndex >= 0) return;

			//ドラッグされているデータがstring型に変換できるか調べ、
			//そうであればドロップ効果をMoveにする
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				if (((string[])e.Data.GetData(DataFormats.FileDrop)).Length >= 1)
				{
					e.Effect = DragDropEffects.Move;
				}
			}
		}


		#endregion // ドラッグアンドドロップ処理 --------------------------------------------

		/// <summary>
		/// 現在のデータグリッド行を再描画します</summary>
		private void RedrawCurrentRowDgv()
		{
			int rowIndex = dataGridViewMain.CurrentCell.RowIndex;
			var pd = FindCurrentPageNoFromDgv(rowIndex);
			if (pd == null) return;
			var objs = CreateDataGridObjects(pd);
			for (int i = 0; i < objs.Length; i++)
			{
				dataGridViewMain.CurrentRow.Cells[i].Value = objs[i];
			}
			this.Invalidate();
		}

		/// <summary>ページインデックスのデータグリッド行を再描画します</summary>
		/// <param name="idx">ページインデックス</param>
		private void RedrawRowDgv(int idx)
		{
			int ww;
			var rowidt = dataGridViewMain.Rows.Cast<DataGridViewRow>().Select((r, i) => new { Row = r, Index = i }).Where(w => int.TryParse(w.Row.Cells["PageIdx"].Value.ToString(), out ww)).FirstOrDefault(w => int.Parse(w.Row.Cells[0].Value.ToString()) == idx + 1);
			if (rowidt == null) return;
			var rowIndex = rowidt.Index;
			var pd = _project.BookData.PageDataList[idx];
			var objs = CreateDataGridObjects(pd);
			for (int i = 0; i < objs.Length; i++)
			{
				dataGridViewMain.Rows[rowIndex].Cells[i].Value = objs[i];
			}
			this.Invalidate();
		}


		/// <summary>ページデータからデータグリッドビュー用オブジェクト群を生成します</summary>
		/// <param name="pdata">ページデータ</param>
		/// <returns>オブジェクト群</returns>
		private object[] CreateDataGridObjects(PageDataRJB pdata)
		{
			string highPL = pdata.SideL.HeightPixPaper.ToString();
			string highGL = pdata.SideL.HeightPixGrid.ToString();
			string highPR = pdata.SideR.HeightPixPaper.ToString();
			string highGR = pdata.SideR.HeightPixGrid.ToString();
			string fold1, fname;
			if (pdata.FilePath.StartsWith(_pdfworkPath))
			{
				fold1 = Path.GetFileName(Path.GetDirectoryName(_project.SetData.PdfPath));
				fname = Path.GetFileName(_project.SetData.PdfPath) + " " + Path.GetFileNameWithoutExtension(pdata.FilePath);
			}
			else if (pdata.FilePath.StartsWith(_iiifworkPath))
			{
				string[] pathlist = _project.ManifestToURL();
				for (int i = 0; i < 4; i++)
				{
					pathlist[0] = Path.GetDirectoryName(pathlist[0]);
				}
				fold1 = pathlist[0];                            // URL
				fname = Path.GetFileName(pdata.FilePath);       // TIFFファイル名
			}
			else
			{
				fold1 = Path.GetFileName(Path.GetDirectoryName(pdata.FilePath));
				fname = Path.GetFileName(pdata.FilePath);
			}
			if (rbtnMmFromPerMm.Checked && _project.SetData.IsUseHeightPaper.HasValue && _project.SetData.IsUseHeightPaper == false)        // mm（1mm当たりピクセル数より算出）で表示
			{
				highPL = String.Format("{0:##0.000}", pdata.SideL.HeightPixPaper / (double)_project.SetData.PixelPerMm);
				highGL = String.Format("{0:##0.000}", pdata.SideL.HeightPixGrid / (double)_project.SetData.PixelPerMm);
				highPR = String.Format("{0:##0.000}", pdata.SideR.HeightPixPaper / (double)_project.SetData.PixelPerMm);
				highGR = String.Format("{0:##0.000}", pdata.SideR.HeightPixGrid / (double)_project.SetData.PixelPerMm);
			}
			if (rbtnMmFromPaperH.Checked && _project.SetData.IsUseHeightPaper.HasValue && _project.SetData.IsUseHeightPaper == true)        // mm（紙高より算出）で表示
			{
				highPL = String.Format("{0:##0.000}", _project.SetData.HightPaperSizeMM);                                                   // 設定値となる
				highGL = String.Format("{0:##0.000}", _project.SetData.HightPaperSizeMM * pdata.SideL.HeightPixGrid / (double)pdata.SideL.HeightPixPaper);
				highPR = String.Format("{0:##0.000}", _project.SetData.HightPaperSizeMM);                                                   // 設定値となる
				highGR = String.Format("{0:##0.000}", _project.SetData.HightPaperSizeMM * pdata.SideR.HeightPixGrid / (double)pdata.SideR.HeightPixPaper);
			}
			return new object[] { (pdata.PageIdx + 1).ToString().PadLeft(4), pdata.IsValidate, fold1, fname, highPL.PadLeft(10), highGL.PadLeft(10), highPR.PadLeft(10), highGR.PadLeft(10), pdata.IsChanged };
		}


		/// <summary>
		/// データリスト変更時のデータグリッドビューの描画</summary>
		/// <param name="isRestruct">再作成</param>
		/// <remarks>表示内容が変わったときには変更が必要</remarks>
		private void DrawDataGridViewList(bool isRestruct = true)
		{
			if (isRestruct)
			{
				// データグリッドデータをクリアし、DataRJBの内容に合わせデータを再セットする
				dataGridViewMain.Rows.Clear();
				var w = _project.BookData.PageDataList.ToArray();
				foreach (var ww in _project.BookData.PageDataList)
				{
					this.dataGridViewMain.Rows.Add(CreateDataGridObjects(ww));
				}
			}
			else
			{
				// 再描画のみ
				int max = dataGridViewMain.Rows.Count;
				for (int rowIndex = 0; rowIndex < max; rowIndex++)
				{
					int pageindex;
					if (int.TryParse(dataGridViewMain.Rows[rowIndex].Cells["PageIdx"].Value.ToString(), out pageindex) == false) break; // Error.
					pageindex--;            // 表示は+1されているため。
					var pd = _project.BookData.PageDataList[pageindex];
					var objs = CreateDataGridObjects(pd);
					for (int i = 0; i < objs.Length; i++)
					{
						dataGridViewMain.Rows[rowIndex].Cells[i].Value = objs[i];
					}
				}
			}
			this.dataGridViewMain.Refresh();
		}



		/// <summary>
		/// ロード時の処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void FormBookMain_Load(object sender, EventArgs e)
		{
			// ピクチャーボックスの拡大縮小対応
			_scalePbox = new ScalablePbox(this, pictureBoxMain, (mat, rate) =>
			{
				// 情報表示
				labelDisplayRate.Text = String.Format("{0:###0.0 %}", rate);
				Refresh();
			},
				OnPboxClick,
				OnPboxMove,
				null,
				pictureBoxMain_Paint
			);
			//　ScalablePbox規定値・および設定
			_scalePbox.MaxRateMainImage = 10.0f;        // 10倍
			_scalePbox.MinRateMainImage = 0.025f;           // 0.025倍
			_scalePbox.InterpolationMode = CommonValues.ConfigData.ImageInterpolationMode;
			_scalePbox.MouseWheel += new MouseEventHandler(ScalPbox_MouseWheel);            // オプションイベント

			// その他の値
			_formExecAllDetect = null;      // 非実行中

			// キーを一旦フォームが受け取る
			this.KeyPreview = true;
		}



		/// <summary>
		/// クリック点から編集対象ROIおよびポイントがあれば取得する。
		/// </summary>
		/// <param name="x">クリック位置 x</param>
		/// <param name="y">クリック位置 y</param>
		/// <param name="roi">対象ROI</param>
		/// <param name="po">対象ポイント（ポイントの場合のみ）</param>
		/// <param name="posinfo">ポイントの位置情報（ポイントの場合のみ有効）</param>
		/// <param name="roiIndex">ROIのインデックス（「紙」用反時計回り、「匡郭」反時計回りの 0～7）</param>
		/// <returns></returns>
		private bool GetNearRoiPos(int x, int y, out BookROI roi, out SyPoint? po, out BookROI.PosInf posinfo, out int roiIndex)
		{
			bool ret = false;
			posinfo = BookROI.PosInf.LeftTop; // default.
			roi = null;
			po = null;
			roiIndex = 0;

			//　SyPoint, 親ROI, 位置のテーブルを作成する。tbl　順序は「紙」からTopLeft反時計回り、「匡郭」～。
			_allRois = new BookROI[] { _project.SetData.RoiPTL, _project.SetData.RoiPBL, _project.SetData.RoiPBR, _project.SetData.RoiPTR, _project.SetData.RoiGTL, _project.SetData.RoiGBL, _project.SetData.RoiGBR, _project.SetData.RoiGTR };
			var tbl = _allRois.Select((ro, i) => new { Roi = ro, No = i }).SelectMany(rot => rot.Roi.GetPoints().Zip(rot.Roi.GetPosInf(), (pos, inf) => new { Pos = pos, Roi = rot.Roi, Inf = inf, Idx = rot.No })).ToArray();

			if (tbl.Any() == false) return ret;

			// x, yに一番近いテーブル要素（ポイント）を返す
			var nearPos = tbl.OrderBy(t => (t.Pos.X - x) * (t.Pos.X - x) + (t.Pos.Y - y) * (t.Pos.Y - y)).First();

			// それが「近い」ところにあれば、そのポイントを返す
			if (IsNearPos(nearPos.Pos, x, y) == true)
			{
				roi = nearPos.Roi;
				roiIndex = nearPos.Idx;
				po = nearPos.Pos;
				posinfo = nearPos.Inf;
				return true;
			}

			//　ポイントから遠くてもボックスの中ならばROI情報を返す。(SyPointはnull)。複数のROI中ならば面積の小さいもの
			var attachedRoi = _allRois.Select((ro, i) => new { Roi = ro, No = i }).Where(rot => rot.Roi.Rect.Contains(new SyPoint(x, y)));

			// どのROIにも含まれない場合、falseを返す
			if (attachedRoi.Any() == false)
			{
				return false;
			}

			var selRoi = attachedRoi.OrderBy(rt => rt.Roi.RoiSize.Width * rt.Roi.RoiSize.Width + rt.Roi.RoiSize.Height * rt.Roi.RoiSize.Height).First();     // 一番面積の小さい
			roi = selRoi.Roi;
			roiIndex = selRoi.No;
			po = null;
			return true;
		}


		/// <summary>
		/// ROI編集時のカーソルエリア設定 (_curAreaRoiTL, _curAreaRoiBR)</summary>
		/// <remarks>  _selectRoi, _secectRoiIndex, _selectRoiPos  ==> _curAreaRoiTL, _curAreaRoiBR</remarks>
		private void SetCurAreaRoi()
		{
			if (_selectRoi == null) return;
			int LLimitX = 0;
			int LLimitY = 0;
			int HLimitX = 0;
			int HLimitY = 0;
			int wid = _project.BookData.CurrentImage.Width;
			int hei = _project.BookData.CurrentImage.Height;
			switch (_selectRoiIndex % 4)
			{
				case 0: LLimitX = 0; LLimitY = 0; HLimitX = wid / 2 - 1; HLimitY = hei / 2 - 1; break; // 左上
				case 1: LLimitX = 0; LLimitY = hei / 2; HLimitX = wid / 2 - 1; HLimitY = hei - 1; break; // 左下
				case 2: LLimitX = wid / 2; LLimitY = hei / 2; HLimitX = wid - 1; HLimitY = hei - 1; break; // 右下
				case 3: LLimitX = wid / 2; LLimitY = 0; HLimitX = wid - 1; HLimitY = hei / 2 - 1; break; // 右上
			}

			// Allの場合と各点の場合で分けて処理
			if (_ppmModeRoi == PpmRoi.All)      // All
			{
				// ALLの場合には最悪のケースで外れないこととする。
				LLimitX += _selectRoi.RoiSize.Width / 2 + 1;
				LLimitY += _selectRoi.RoiSize.Height / 2 + 1;
				HLimitX -= _selectRoi.RoiSize.Width / 2 + 1;
				HLimitY -= _selectRoi.RoiSize.Height / 2 + 1;
			}
			else if (_ppmModeRoi == PpmRoi.Pos)
			{
				//switch (_selectRoiPos)
				//{
				//    case BookROI.PosInf.LeftTop: HLimitX = _selectRoi.BasePoint.X+ _selectRoi.RoiSize.Width-1- ; HLimitY -= _selectRoi.RoiSize.Height; break;
				//    case BookROI.PosInf.LeftBtm: HLimitX -= _selectRoi.RoiSize.Width; LLimitY += _selectRoi.RoiSize.Height; break;
				//    case BookROI.PosInf.RightTop: LLimitX += _selectRoi.RoiSize.Width; HLimitY -= _selectRoi.RoiSize.Height; break;
				//    case BookROI.PosInf.RightBtm: LLimitX += _selectRoi.RoiSize.Width; LLimitY += _selectRoi.RoiSize.Height; break;
				//}
			}
			_curAreaRoiTL = new SyPoint(LLimitX, LLimitY);
			_curAreaRoiBR = new SyPoint(HLimitX, HLimitY);
		}


		/// <summary>メインピクチャーボックス(ScalablePbox)のクリック時の処理です</summary>
		/// <param title="x">元データのX座標</param>
		/// <param title="y">元データのY座標</param>
		/// <param title="col">表示している位置の色情報</param>
		/// <param title="down">ボタン・ダウンの場合真</param>
		/// <param title="isLeft">左ボタンの場合真</param>
		/// <remarks>計測点の編集の場合と、ROI情報の編集の両方対応する</remarks>
		private void OnPboxClick(int x, int y, Color col, bool down, bool isLeft)
		{
			bool isEditMes;
			bool isEditRoi;
			// 前処理および編集処理の可否
			if (PreClickAndMove(x, y, out isEditMes, out isEditRoi) == false) return;

			try
			{
				// 左バタンクリックでかつコントロールが押されている場合のみポイントを選択できる
				if (isLeft == true && (Control.ModifierKeys & Keys.Control) != 0)
				{
					// 測定点編集モードならば
					if (isEditMes)
					{
						if (_ppmModeMes == PpmMes.Non)
						{
							SetSavePoint();
							_selectLine = null;
							foreach (var sl in _curLines)
							{
								_onClickPos = new SyPoint(x, y);
								if (IsNearPos(sl.PosBgn, x, y))
								{
									_ppmModeMes = PpmMes.Bgn;
									_selectLine = sl;
									_onClickPos = sl.PosBgn;
									break;
								}
								else if (IsNearPos(sl.PosEnd, x, y))
								{
									_ppmModeMes = PpmMes.End;
									_selectLine = sl;
									_onClickPos = sl.PosEnd;
									break;
								}
							}
							if (_selectLine == null)
							{
								foreach (var sl in _curLines)
								{
									if (sl.DistanceLineToPoint(_onClickPos) < Math.Sqrt(JudgeLimit))
									{
										// そうでなく、直線に近い
										_ppmModeMes = PpmMes.All;
										_selectLine = sl;
										break;
									}
								}
							}
						}
					}
					// Roi編集モードならば
					if (isEditRoi)
					{
						if (_ppmModeRoi == PpmRoi.Non)
						{
							BookROI wRoi;
							SyPoint? wPos;
							BookROI.PosInf wInf;
							int idx;
							var ret = GetNearRoiPos(x, y, out wRoi, out wPos, out wInf, out idx);
							if (ret == true)
							{
								_selectRoi = wRoi;
								_selectRoiIndex = idx;
								_selectRoiPos = wInf;
								_ppmModeRoi = wPos == null ? PpmRoi.All : PpmRoi.Pos;

								// 点指定ならば、実際の点の位置にカーソルを合わせる。ALLはROI中央に移動。
								SyPoint newPos = wPos ?? new SyPoint(_selectRoi.BasePoint.X + (_selectRoi.RoiSize.Width - 1) / 2, _selectRoi.BasePoint.Y + (_selectRoi.RoiSize.Height - 1) / 2);
								Cursor.Position = _scalePbox.BasePbox.PointToScreen(_scalePbox.TransXyOrgToDisp(newPos));       // 座標変換（データ座標→Pboxクライアント→スクリーン）してカーソル移動
								x = newPos.X;
								y = newPos.Y;
								_onClickPos = new SyPoint(x, y);
							}
							if (_selectRoi != null)
							{
								SetCurAreaRoi();                // ROI指定時のカーソル範囲制限
							}
						}
					}
				}
				else
				{
					_ppmModeMes = PpmMes.Non;
					_selectLine = null;
					_ppmModeRoi = PpmRoi.Non;
					_selectRoi = null;
					_selectRoiPos = null;
				}
			}
			finally
			{
				// 位置を保存
				_lastPos.X = x;
				_lastPos.Y = y;
			}

			// マウス・カーソル情報の表示
			SetInfoLabel();

			this.Update();
			//pictureBoxMain.Refresh();
		}


		/// <summary>
		/// クリック時および移動時前処理・共通部分</summary>
		/// <param name="x">マウス座標 x</param>
		/// <param name="y">マウス座標 y</param>
		/// <param name="iseditM">計測点編集</param>
		/// <param name="iseditR">ROI編集</param>
		/// <returns>実施可否(falseなら以下の処理を行わない）</returns>
		private bool PreClickAndMove(int x, int y, out bool iseditM, out bool iseditR)
		{
			// 計測点（編集）がチェックされていない
			iseditM = cboxMeasPoint.Checked == true && cboxMeasPoint.Enabled == true;
			if (iseditM == false)
			{
				// 計測点編集情報クリア
				_ppmModeMes = PpmMes.Non;
				_selectLine = null;
			}
			// ROI（編集）がチェックされていない
			iseditR = cboxRoiInfo.Checked == true && cboxRoiInfo.Enabled == true;
			if (iseditR == false)
			{
				// ROI編集情報クリア
				_ppmModeRoi = PpmRoi.Non;
				_selectRoi = null;
				_selectRoiPos = null;
			}

			// 計測点（編集）ROI（編集）ともにチェックされていないときはカーソル描画しないため、移動処理しない
			return (iseditM == true || iseditR == true);
		}


		/// <summary>計測点変更モードにおけるメインピクチャーボックス(ScalablePbox)のマウス移動時処理です</summary>
		/// <param title="x">元データのX座標</param>
		/// <param title="y">元データのY座標</param>
		/// <param title="col">表示している位置の色情報</param>
		/// <param title="down">ボタン・ダウンの場合真</param>
		private void OnPboxMoveMes(int x, int y, Color col, bool down)
		{
			// ボタンが押されていないか、コントロールが押されたままでなければ、移動モードは解除
			if (down == false || (Control.ModifierKeys & Keys.Control) == 0)
			{
				_ppmModeMes = PpmMes.Non;
				_selectLine = null;
			}

			// 画面外の場合処理しない
			if (x < 0 || y < 0) return;

			// カーソル移動処理
			if (_ppmModeMes == PpmMes.Bgn)
			{
				if ((Control.ModifierKeys & Keys.Shift) == 0)
				{
					_selectLine.PosBgn = _selectLine.ChangeLength(x, y);
				}
				else
				{
					_selectLine.PosBgn.X = x;
					_selectLine.PosBgn.Y = y;
					if (rOI垂直モードToolStripMenuItem.Checked)
					{
						_selectLine.PosEnd.X = x;
					}
				}
				if (_selectLine._link != null)
				{
					if (cboxMeasurmentMatch.Checked)
					{
						_selectLine._link.SetShiftPoint();
					}
				}
				_project.BookData.CurrentPage.ChangeLine();
				RedrawCurrentRowDgv();
			}
			else if (_ppmModeMes == PpmMes.End)
			{
				if ((Control.ModifierKeys & Keys.Shift) == 0)
				{
					_selectLine.PosEnd = _selectLine.ChangeLength(x, y);
				}
				else
				{
					_selectLine.PosEnd.X = x;
					_selectLine.PosEnd.Y = y;
					if (rOI垂直モードToolStripMenuItem.Checked)
					{
						_selectLine.PosBgn.X = x;
					}
				}
				if (_selectLine._link != null)
				{
					if (cboxMeasurmentMatch.Checked)
					{
						_selectLine._link.SetShiftPoint();
					}
				}
				_project.BookData.CurrentPage.ChangeLine();
				RedrawCurrentRowDgv();
			}
			else if (_ppmModeMes == PpmMes.All)
			{
				if ((Control.ModifierKeys & Keys.Shift) == 0)
				{
					_selectLine.Translation(x, y);
					if (cboxMeasurmentMatch.Checked && _selectLine._link != null)
					{
						_selectLine._link.Translation(x, y);
					}
				}
				else
				{
					_selectLine.PosBgn.X = _selectLine.PosBgn.X + x - _onClickPos.X;
					_selectLine.PosBgn.Y = _selectLine.PosBgn.Y + y - _onClickPos.Y;
					_selectLine.PosEnd.X = _selectLine.PosEnd.X + x - _onClickPos.X;
					_selectLine.PosEnd.Y = _selectLine.PosEnd.Y + y - _onClickPos.Y;
					if (cboxMeasurmentMatch.Checked && _selectLine._link != null)
					{
						_selectLine._link.PosBgn.X = _selectLine._link.PosBgn.X + x - _onClickPos.X;
						_selectLine._link.PosBgn.Y = _selectLine._link.PosBgn.Y + y - _onClickPos.Y;
						_selectLine._link.PosEnd.X = _selectLine._link.PosEnd.X + x - _onClickPos.X;
						_selectLine._link.PosEnd.Y = _selectLine._link.PosEnd.Y + y - _onClickPos.Y;
					}
				}
				_onClickPos.X = x;
				_onClickPos.Y = y;
				_project.BookData.CurrentPage.ChangeLine();
				RedrawCurrentRowDgv();
			}
			_scalePbox.DrawMainImage();
		}


		/// <summary>ROI変更モードにおけるメインピクチャーボックス(ScalablePbox)のマウス移動時処理です</summary>
		/// <param title="x">元データのX座標</param>
		/// <param title="y">元データのY座標</param>
		/// <param title="col">表示している位置の色情報</param>
		/// <param title="down">ボタン・ダウンの場合真</param>
		private void OnPboxMoveRoi(int x, int y, Color col, bool down)
		{
			// ROI未指定、ボタンが押されていない、コントロールが押されたままでなければ、移動モードは解除
			if (_selectRoi == null || down == false || (Control.ModifierKeys & Keys.Control) == 0)
			{
				_ppmModeRoi = PpmRoi.Non;
				_selectRoi = null;
				_selectRoiPos = null;
				return;
			}

			// 画面外の場合処理しない
			if (x < 0 || y < 0) return;

			// 移動範囲内に収める
			if (x < _curAreaRoiTL.X) x = _curAreaRoiTL.X;
			if (y < _curAreaRoiTL.Y) y = _curAreaRoiTL.Y;
			if (x > _curAreaRoiBR.X) x = _curAreaRoiBR.X;
			if (y > _curAreaRoiBR.Y) y = _curAreaRoiBR.Y;

			//上下方向固定モード
			if (radioButtonROIVertical.Checked) x = _onClickPos.X;

			//左右方向固定モード
			if (radioButtonROIHorizon.Checked) y = _onClickPos.Y;

			// 垂直モード時の調整処理アクション
			var actionVmode = new Action(() =>
				{
					BookROI adjRoi = null;      // 垂直モードの場合に調整される対象のROI
					_allRois = new BookROI[] { _project.SetData.RoiPTL, _project.SetData.RoiPBL, _project.SetData.RoiPBR, _project.SetData.RoiPTR, _project.SetData.RoiGTL, _project.SetData.RoiGBL, _project.SetData.RoiGBR, _project.SetData.RoiGTR };
					var idx = Array.IndexOf(_allRois, _selectRoi);

					// 測定位置合わせ時
					if (cboxMeasurmentMatch.Checked && rOI垂直モードToolStripMenuItem.Checked)
					{
						if (idx >= 0)
						{
							int[] lindex = { 0, 1, 4, 5 };
							int[] rindex = { 2, 3, 6, 7 };
							int[] target = lindex.Contains(idx) ? lindex : rindex;
							foreach (int i in target)
							{
								adjRoi = _allRois[i];
								if (adjRoi != null)
								{
									adjRoi.BasePoint = new SyPoint(_selectRoi.BasePoint.X, adjRoi.BasePoint.Y);     // Xは_selectRoiに合わせる
									adjRoi.RoiSize = new SySize(_selectRoi.RoiSize.Width, adjRoi.RoiSize.Height);       // Widthは_selectRoiに合わせる。
								}
							}
						}
					}
					else if (rOI垂直モードToolStripMenuItem.Checked)
					{
						if (idx >= 0) adjRoi = _allRois[idx ^ 0x01];        // 上下反対側を指す。 0→1, 1→0, ・・
						if (adjRoi != null)
						{
							adjRoi.BasePoint = new SyPoint(_selectRoi.BasePoint.X, adjRoi.BasePoint.Y);     // Xは_selectRoiに合わせる
							adjRoi.RoiSize = new SySize(_selectRoi.RoiSize.Width, adjRoi.RoiSize.Height);       // Widthは_selectRoiに合わせる。
						}
					}
					if (rOI高さ連動モードToolStripMenuItem.Checked)
					{
						var b = idx / 4 * 4;        // 0 or 4
						var ro = _allRois[b + (7 - idx % 4) % 4];
						ro.BasePoint = new SyPoint(ro.BasePoint.X, _selectRoi.BasePoint.Y);
						ro.RoiSize = new SySize(ro.RoiSize.Width, _selectRoi.RoiSize.Height);
					}
				});

			// カーソル移動処理
			if (_ppmModeRoi == PpmRoi.All)      // 全部を移動
			{
				_selectRoi.BasePoint = new SyPoint(x - _selectRoi.RoiSize.Width / 2, y - _selectRoi.RoiSize.Height / 2);
				actionVmode();
				_scalePbox.DrawMainImage();

			}
			else if (_ppmModeRoi == PpmRoi.Pos)
			{

				if (_selectRoiPos != null)
				{
					int hi, wi, xx, yy;
					hi = wi = xx = yy = 0;

					switch (_selectRoiPos)
					{
						case BookROI.PosInf.LeftTop:
							wi = _selectRoi.RoiSize.Width + _selectRoi.BasePoint.X - x;
							hi = _selectRoi.RoiSize.Height + _selectRoi.BasePoint.Y - y;
							xx = x;
							yy = y;
							break;
						case BookROI.PosInf.LeftBtm:
							wi = _selectRoi.RoiSize.Width + (_selectRoi.BasePoint.X - x);
							hi = y - _selectRoi.BasePoint.Y + 1;
							xx = x;
							yy = _selectRoi.BasePoint.Y;
							break;
						case BookROI.PosInf.RightBtm:
							wi = x - _selectRoi.BasePoint.X + 1;
							hi = y - _selectRoi.BasePoint.Y + 1;
							xx = _selectRoi.BasePoint.X;
							yy = _selectRoi.BasePoint.Y;
							break;
						case BookROI.PosInf.RightTop:
							wi = x - _selectRoi.BasePoint.X + 1;
							hi = _selectRoi.RoiSize.Height + (_selectRoi.BasePoint.Y - y);
							xx = _selectRoi.BasePoint.X;
							yy = y;
							break;
					}

					// 移動後、「小さすぎたり」「位置が不適当」な場合には、元の値を採用（_selectRoiに対して、何もしない）。
					var limit = (int)Math.Ceiling(Math.Sqrt((double)JudgeLimit));       // 2乗値なので戻しておく。安全のため切り上げ。
					if (wi >= limit && hi >= limit)
					{
						_selectRoi.BasePoint = new SyPoint(xx, yy);
						_selectRoi.RoiSize = new SySize(wi, hi);
					}
					actionVmode();
					_scalePbox.DrawMainImage();

				}
			}
		}


		/// <summary>メインピクチャーボックス(ScalablePbox)のマウス移動時の処理です</summary>
		/// <param title="x">元データのX座標</param>
		/// <param title="y">元データのY座標</param>
		/// <param title="col">表示している位置の色情報</param>
		/// <param title="down">ボタン・ダウンの場合真</param>
		private void OnPboxMove(int x, int y, Color col, bool down)
		{
			bool isEditMes;
			bool isEditRoi;

			if (_isOpenPboxContextMenu == false && _baseSelectedRoi != null)
			{
				_baseSelectedRoi = null;
				_scalePbox.DrawMainImage();
			}

			// 前処理および編集処理の可否
			if (PreClickAndMove(x, y, out isEditMes, out isEditRoi) == false) return;
			try
			{
				if (isEditMes)
				{
					OnPboxMoveMes(x, y, col, down);
				}
				if (isEditRoi)
				{
					OnPboxMoveRoi(x, y, col, down);
				}
			}
			finally
			{
				// 位置を保存
				_lastPos.X = x;
				_lastPos.Y = y;
			}

			labelCurInfo.Invalidate();

			// マウス・カーソル情報の表示
			SetInfoLabel();

			this.Update();
		}



		/// <summary>カーソル情報（測定点）のラベル表示を行います</summary>
		private void SetInfoLabel()
		{
			// ラベル更新
			labelPointInfo.Text = String.Format("({0,5} , {1,5} )", _lastPos.X, _lastPos.Y);

			if (_selectLine == null) return;
			var dist = _selectLine.GetDistance();
			var angl = ((int)dist == 0) ? "----------" : _selectLine.GetAngle().ToString("0.000");
			labelCurInfo.Text = String.Format("{0},   長さ= {1,8} Px,  角度= {2,8}", _selectLine.ToString(), dist.ToString("0.00"), angl);
		}



		/// <summary>P0とP1が「近い」かどうか判定します</summary>
		/// <param name="p0">ポイント１＜データ座標＞</param>
		/// <param name="x1">ポイント２ x ＜データ座標＞</param>
		/// <param name="y1">ポイント２ y ＜データ座標＞</param>
		/// <returns>ポイント１，２が近い場合、trueを返します</returns>
		private bool IsNearPos(SyPoint p0, int x1, int y1)
		{
			var lmt = JudgeLimit / (_scalePbox.DispRate * _scalePbox.DispRate);
			return ((p0.X - x1) * (p0.X - x1) + (p0.Y - y1) * (p0.Y - y1) <= lmt);
		}





		/// <summary>メインピクチャーボックス上のマウスホイールイベント処理です（拡縮を行わない場合のみ発生）</summary>
		/// <param name="sender">イベント発信元</param>
		/// <param name="e">イベント引数</param>
		/// <remarks>DataGridView表示順で移動するように、先にDataGridView情報を操作する</remarks>
		protected void ScalPbox_MouseWheel(object sender, MouseEventArgs e)
		{
			// データ未登録の場合無視する（リターン）
			if (_project.BookData == null) return;

			var index = dataGridViewMain.CurrentCell.RowIndex;

			//　スクロール方向前方で前にデータがある場合
			if (e.Delta > 0 && index > 0)
			{
				index--;

			}
			else if (e.Delta < 0 && index < dataGridViewMain.Rows.Count - 1)
			{
				index++;
			}

			// カレントデータと異なっている場合、表示変更
			if (dataGridViewMain.CurrentCell.RowIndex != index)
			{
				// カーソルを移動、後は移動時のイベント処理に任せる
				dataGridViewMain.CurrentCell = dataGridViewMain[0, index];
			}
		}



		/// <summary> フォームを閉じた時の処理です </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			DevelopLog.LogDebug(System.Reflection.MethodBase.GetCurrentMethod(), "フォームを閉じるタイミングです。");
			try
			{
				CommonValues.ConfigData.WindowSize = this.Size;     // 終了時のメインフォームのサイズを保持
				if (_scalePbox != null)
				{
					_scalePbox.MouseWheel -= new MouseEventHandler(ScalPbox_MouseWheel);
					_scalePbox.Dispose();
					_scalePbox = null;
				}


				// 保存処理など
				//				CommonValues.ConfigData.SaveSettingData();
				// インスタンス破棄
				if (_log != null)
				{
					_log.Stop();
					_log.Dispose();
					_log = null;
				}
			}
			catch (Exception ex)
			{
				DevelopLog.LogException(MethodBase.GetCurrentMethod(), "フォームを閉じるタイミングで例外が発生しました。", ex);
			}
		}



		/// <summary>
		/// フォームを閉じようとするときの処理です
		/// </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void FormBookMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			// 終了確認を行う。　確認毎に設定値とプロジェクトの自動保存を実施しておく。

			// 設定値デフォルト保存　ウィンドウサイズを設定値にセットし、設定値を保存する。
			CommonValues.ConfigData.WindowSize = this.Size;
			CommonValues.ConfigData.SplitterDistance = this.splitContainerMain.SplitterDistance;
			CommonValues.ConfigData.SaveSettingData();

			// プロジェクトのデフォルト保存　データがあればプロジェクトを保存し、終了確認を行う
			if (_project != null && _project.BookData != null && _project.BookData.PageDataList != null && _project.BookData.PageDataList.Length > 0)
			{
				// プロジェクトのデフォルト保存
				var defname = Path.Combine(CommonValues.DefaultSettingFolder, "__AutoSaveProjectData" + CommonValues.ExtBookProject);
				_project.SaveXmlFile(defname);

				// 終了確認　キャンセルが指定された場合には終了しない。　その場合でも上記のデフォルト保存は行う。
				var ret = MessageBox.Show("終了します。よろしいですか？", "終了確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
				e.Cancel = (ret == DialogResult.Cancel);
				try
				{
					if (!e.Cancel && Directory.Exists(_pdfworkPath)) Directory.Delete(_pdfworkPath, true);
					if (!e.Cancel && Directory.Exists(_iiifworkPath)) Directory.Delete(_iiifworkPath, true);
				}
				catch { }
			}
		}


		/// <summary>
		/// 新しいデータの表示
		/// </summary>
		private bool DispNewData()
		{
			if (_project.BookData == null || _project.BookData.IsValid() == false) return false;    // データなし
			this.Text = "【 " + _project.BookData.CommonFolderPath + " 】";
			if (_mainImageBaseLayer != null)
			{
				_mainImageBaseLayer.Dispose();
				_mainImageBaseLayer = null;
			}
			var img = _project.BookData.CurrentImage;
			if (img == null) return false;
			_mainImageBaseLayer = ImageLib.CloneBitmapPf(_project.BookData.CurrentImage, PixelFormat.Format24bppRgb);

			// 補助線の描画　(_PEND)


			// 新しい画像を表示する。コントロールキーが押されていたら、FIXモードで表示する。
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
			{
				_scalePbox.IsFixed = true;
			}
			_scalePbox.ChangeMainImage(_mainImageBaseLayer);
			_scalePbox.IsFixed = false;

			labelImageInf.Text = $"Width {img.Width} px × Height {img.Height} px";
			//
			Refresh();
			return true;
		}

		/// <summary>カレントデータ（ワーク）のクリアを行います</summary>
		private void CleanWorkData()
		{
			// 現状はなし
		}


		/// <summary>データグリッド行が選択された時の処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void dataGridViewMain_RowEnter(object sender, DataGridViewCellEventArgs e)
		{
			// カレントインデックスの変更
			if (_project.BookData.CurrentIndex != e.RowIndex)
			{
				CleanWorkData();
				var noStr = dataGridViewMain.Rows[e.RowIndex].Cells["PageIdx"].Value.ToString();
				int no;
				if (int.TryParse(noStr, out no) == false)
				{
					var erstr = $"インデックス変換に失敗しました RowIndex={e.RowIndex}, {noStr}";
					DevelopLog.LogERR(MethodBase.GetCurrentMethod(), erstr);
					MessageBox.Show(erstr);     // for Debug _PEND.
					throw new Exception(erstr);
				}
				_project.BookData.CurrentIndex = no - 1;
			}

			DispNewData();
		}


		/// <summary>
		/// セルがクリックされたときの処理です
		/// </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		/// <remarks>ALTが押されていた場合、実行処理も行います。 ver 010.040以降　CTRLは画面Fixも兼ねているため</remarks>
		private void dataGridViewMain_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= dataGridViewMain.Rows.Count) return;


			//  Alt+クリックで実行。 CtrlとAltが押されている場合には実行されない。
			if ((Control.ModifierKeys & (Keys.Control | Keys.Alt)) == Keys.Alt)
			{
				ExecuteCurrentData();
			}
			DispNewData();
		}



		/// <summary>ピクチャーボックスがダブルクリックされたときの処理です。</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void pictureBoxMain_DoubleClick(object sender, EventArgs e)
		{
			// 画面の大きさにフィット(Fit)させます
			_scalePbox.Zoom();

			Refresh();
		}



		/// <summary>フォーム描画の処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void FormBookMain_Paint(object sender, PaintEventArgs e)
		{

			// ラジオボタンの状態を設定します
			// データなしまたは算出パラメータが未設定の場合には規定値とする
			if (_project.BookData == null || _project.SetData.IsUseHeightPaper == null)
			{
				rbtnPx.Checked = true;
				rbtnMmFromPerMm.Checked = false;
				rbtnMmFromPerMm.Enabled = false;
				rbtnMmFromPaperH.Checked = false;
				rbtnMmFromPaperH.Enabled = false;
			}
			else
			{
				rbtnMmFromPaperH.Enabled = _project.SetData.IsUseHeightPaper.Value;
				rbtnMmFromPerMm.Enabled = !_project.SetData.IsUseHeightPaper.Value;
			}

			//labelCurInfo.Visible = IsExistCurrentPage() && _selectLine != null;
			//labelCurInfo.Visible = IsExistCurrentPage();

			// マウス・カーソル情報の表示
			SetInfoLabel();
		}



		/// <summary>ライン情報用ペン（紙）</summary>
		private Pen __penPaper = new Pen(Color.Red, 4.0f);

		/// <summary>ライン情報用ペン（グリッド）</summary>
		private Pen __penGrid = new Pen(Color.Yellow, 4.0f);

		/// <summary>ライン情報用ペン（全検出）</summary>
		private Pen __penDline = new Pen(Color.Orange, 2.0f);

		/// <summary>
		/// ピクチャーボックスの描画時の処理です　(ScalablePboxに指定）
		/// </summary>
		/// <param name="gra">グラフィックス</param>
		/// <param name="imgsize">画像サイズ</param>
		private void pictureBoxMain_Paint(Graphics gra, SySize imgsize)
		{
			if (_scalePbox == null || _scalePbox.IsMainImageValid == false) return;
			if (_project.BookData.IsValid() == false || _project.BookData.PageDataList == null || _project.BookData.PageDataList.Length == 0) return;
			if (pictureBoxMain.Image == null) return;

			// 検出ライン情報の描画
			var pd = _project.BookData.PageDataList[_project.BookData.CurrentIndex];
			if (pd == null) return;
			if (cboxDetectLineAll.Checked)
			{
				if (pd.DetectedLines != null)
				{
					foreach (var dl in pd.DetectedLines)
					{
						gra.DrawLine(__penDline, dl.PosBgn, dl.PosEnd);
					}
				}
			}


			// 全検出ライン情報の描画
			if (cboxDetectSel.Checked)
			{
				if (pd.SideL.LinePaper != null)
				{
					gra.DrawLine(__penPaper, pd.SideL.LinePaper.TopLine.PosBgn, pd.SideL.LinePaper.TopLine.PosEnd);
					gra.DrawLine(__penPaper, pd.SideL.LinePaper.BtmLine.PosBgn, pd.SideL.LinePaper.BtmLine.PosEnd);
				}
				if (pd.SideL.LineGrid != null)
				{
					gra.DrawLine(__penGrid, pd.SideL.LineGrid.TopLine.PosBgn, pd.SideL.LineGrid.TopLine.PosEnd);
					gra.DrawLine(__penGrid, pd.SideL.LineGrid.BtmLine.PosBgn, pd.SideL.LineGrid.BtmLine.PosEnd);
				}
				if (pd.SideR.LinePaper != null)
				{
					gra.DrawLine(__penPaper, pd.SideR.LinePaper.TopLine.PosBgn, pd.SideR.LinePaper.TopLine.PosEnd);
					gra.DrawLine(__penPaper, pd.SideR.LinePaper.BtmLine.PosBgn, pd.SideR.LinePaper.BtmLine.PosEnd);
				}
				if (pd.SideR.LineGrid != null)
				{
					gra.DrawLine(__penGrid, pd.SideR.LineGrid.TopLine.PosBgn, pd.SideR.LineGrid.TopLine.PosEnd);
					gra.DrawLine(__penGrid, pd.SideR.LineGrid.BtmLine.PosBgn, pd.SideR.LineGrid.BtmLine.PosEnd);
				}
			}

			// カーソルの描画と登録
			var lst = new List<ShapeLineEx>();
			var sl = pd.SideL.MeasurePaper;
			if (sl != null)
			{
				lst.Add(sl);
				DrawCursor(sl, gra, Color.Red, sl == _selectLine);
			}

			sl = pd.SideL.MeasureGrid;
			if (sl != null)
			{
				lst.Add(sl);
				DrawCursor(sl, gra, Color.Green, sl == _selectLine);
			}


			sl = pd.SideR.MeasurePaper;
			if (sl != null)
			{
				lst.Add(sl);
				DrawCursor(sl, gra, Color.Red, sl == _selectLine);
			}

			sl = pd.SideR.MeasureGrid;
			if (sl != null)
			{
				lst.Add(sl);
				DrawCursor(sl, gra, Color.Green, sl == _selectLine);
			}
			_curLines = lst.ToArray();

			// ROI情報
			if (cboxRoiInfo.Checked)
			{
				var penP = new Pen(Color.DarkRed, 5.0f);
				var roisP = new BookROI[] { _project.SetData.RoiPTL, _project.SetData.RoiPBL, _project.SetData.RoiPTR, _project.SetData.RoiPBR };
				var rectP = roisP.Select(r => new Rectangle(r.BasePoint, r.RoiSize)).ToArray();
				gra.DrawRectangles(penP, rectP);

				var penG = new Pen(Color.DarkGreen, 5.0f);
				var roisG = new BookROI[] { _project.SetData.RoiGTL, _project.SetData.RoiGBL, _project.SetData.RoiGTR, _project.SetData.RoiGBR };
				var rectG = roisG.Select(r => new Rectangle(r.BasePoint, r.RoiSize)).ToArray();
				gra.DrawRectangles(penG, rectG);

				if (_baseSelectedRoi != null)
				{
					gra.FillRectangle(new HatchBrush(HatchStyle.ForwardDiagonal, Color.White), _baseSelectedRoi.Rect);
				}
			}


			// その他のコントロール
			labelPageCount.Text = String.Format("画像数　{0}", _project.BookData == null ? "---" : _project.BookData.PageDataList.Length.ToString());

			// 表示
			labelPixelForMm.Visible = _project.SetData.IsUseHeightPaper.HasValue;
			if (_project.SetData.IsUseHeightPaper.HasValue)
			{
				// 紙高による計算または１mm当たりのピクセル数により表示を変える
				labelPixelForMm.Text = _project.SetData.IsUseHeightPaper.Value == true ?
					String.Format("指定紙高: {0} mm", (_project.BookData == null) ? "------" : (_project.SetData.HightPaperSizeMM).ToString("##0.000")) :
					String.Format("1mm当たりPixel: {0}", (_project.BookData == null) ? "------" : (_project.SetData.PixelPerMm).ToString("##0.000"));
			}
		}



		/// <summary>バージョンメニューが選択されたときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void バージョンToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var frm = new FormVersion();
			frm.ShowDialog();
		}


		/// <summary>コンテキストメニュー「データ設定」が選択された時の処理です </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void データ設定ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_mainImageBaseLayer == null)
			{
				MessageBox.Show("画像がありません", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			var frm = new FormAdjustPixelSize(_mainImageBaseLayer, _project.SetData.IsUseHeightPaper ?? true, _project.SetData.HightPaperSizeMM, _project.SetData.PixelPerMm);
			var ret = frm.ShowDialog();
			if (ret == DialogResult.OK)
			{
				_project.SetData.IsUseHeightPaper = frm.IsUsePaperHeight;
				_project.SetData.HightPaperSizeMM = frm.HeightPaper;
				_project.SetData.PixelPerMm = frm.PixelPerMm;
				Refresh();
			}

		}

		/// <summary>表示方式のラジオボタンのチェックが変わったときの処理です。</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void OnCheckedChanbeRbtn(object sender, EventArgs e)
		{
			var rb = sender as RadioButton;
			if (rb == null) return;
			if (rb.Checked == false) return;
			DrawDataGridViewList(false);
		}


		/// <summary>終了ボタンが押されたときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void btnExit_Click(object sender, EventArgs e)
		{
			Close();
		}


		/// <summary>
		/// 指定したデータグリッドビューrowIndexからページ情報を検索する
		/// </summary>
		/// <param name="rowIndex">RowIndex</param>
		/// <returns>検索結果（ない場合にはnull）</returns>
		private PageDataRJB FindCurrentPageNoFromDgv(int rowIndex)
		{
			int pno;
			if (int.TryParse(dataGridViewMain[0, rowIndex].Value.ToString(), out pno) == false) return null;
			var wk = _project.BookData.PageDataList.Where(pd => pd.PageIdx == pno - 1);         // ページ番号の一致するページ情報（必ず１つあるはず）
			if (wk.Any() == false) return null;
			return wk.First();
		}


		/// <summary>グリッドビューのセルの値が変更されたときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void dataGridViewMain_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0) return;
			//列のインデックスを確認する
			if (e.ColumnIndex == 1)              // X==1 Valid欄
			{
				bool cbx;
				if (bool.TryParse(dataGridViewMain[e.ColumnIndex, e.RowIndex].Value.ToString(), out cbx) == false) return;
				var pagedata = FindCurrentPageNoFromDgv(e.RowIndex);
				if (pagedata == null) return;
				pagedata.IsValidate = cbx;      // 更新する
			}
		}


		/// <summary>グリッドビューのセルがダーティになった場合の処理です。</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void dataGridViewMain_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			// チェックボックスセルはクリックしたときには変更が認識されないため、ここで強制的にコミットする
			if (dataGridViewMain.CurrentCellAddress.X == 1 && dataGridViewMain.IsCurrentCellDirty)  // X==1 Valid欄
			{
				//コミットする
				dataGridViewMain.CommitEdit(DataGridViewDataErrorContexts.Commit);
			}


		}

		/// <summary>
		/// 検出ライン関連チェックボックスに変更があったときの処理です
		/// </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void CheckedChange_DispDebug(object sender, EventArgs e)
		{
			_scalePbox.DrawMainImage();

			// ROI編集時はいくつかのコントロールをDisableにする。
			foreach (Control c in new Control[] { btnExec, cboxMeasPoint, btnOutCsv, btnManualAdjust })
			{
				c.Enabled = (cboxRoiInfo.Checked == false);
			}

			Refresh();
		}


		/// <summary>
		/// ROI情報チェックボックスに変更があったときの処理です。
		/// </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void CheckedChange_RoiInfo(object sender, EventArgs e)
		{
			var cb = sender as CheckBox;
			if (cb.Checked == false)
			{
				var xml = _project.SetData.ToXmlString();       // 設定値をXML文字列化する

				// ROIが変更されていたら
				if (String.Compare(xml, _saveSettingXmlForRoi) != 0)
				{
					var w = MessageBox.Show("ROI情報を記録しますか？　（いいえを選択すると破棄されます）", "保存確認", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
					if (w == System.Windows.Forms.DialogResult.No)
					{
						_project.SetData.FromXmlString(_saveSettingXmlForRoi); // 元に戻す
					}
					else if (w == DialogResult.Cancel)
					{
						cb.Checked = true;
						return;
					}
				}
				_saveSettingXmlForRoi = null;
			}
			else
			{
				// チェックがつけられた（＝開始時）
				_saveSettingXmlForRoi = _project.SetData.ToXmlString();

			}

			// 表示変更チェックボックス変更時　共通メソッド
			CheckedChange_DispDebug(sender, e);
		}


		/// <summary>
		/// 「画像を別のウィンドウで表示」が選択されたときの処理です
		/// </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void 画像を別のウィンドウで表示ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_mainImageBaseLayer == null)
			{
				MessageBox.Show("画像がありません", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			var frm = new FormColorImageS(_mainImageBaseLayer, _project.BookData.CurrentPage.FilePath);
			frm.Show();
		}


		/// <summary>
		/// 「CSVファイルに出力」が選択されたときの処理です
		/// </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void cSVファイルに出力ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (dataGridViewMain.Rows.Count < 1) return;
			RJB_Utility.SaveGridToCsv(dataGridViewMain);
		}

		/// <summary>
		/// 「CSV出力」ボタンがクリックされたときの処理です
		/// </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void btnOutCsv_Click(object sender, EventArgs e)
		{
			if (dataGridViewMain.Rows.Count < 1) return;
			_project.SaveCsvFile(rbtnType0.Checked ? 0 : 1);        // Type0(A)が選択されているときは 0(１画像2行）
		}


		/// <summary>
		/// 「クリップボードにコピー」が選択されたときの処理です
		/// </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void クリップボードにコピーToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (dataGridViewMain.Rows.Count < 1) return;
			RJB_Utility.DataGridDataToClipBoard(dataGridViewMain);
		}


		/// <summary>「終了」が選択されたときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}


		/// <summary>「画像データの読み込み」が選択されたときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void 画像データの読み込みToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				// ファイルダイアログ
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.Multiselect = true;     // 複数ファイル選択可能
											//はじめに表示されるフォルダを指定する
				ofd.InitialDirectory = CommonValues.ConfigData.InputFolder;
				//[ファイルの種類]に表示される選択肢を指定する
				ofd.Filter = String.Format("画像ファイル|{0}|すべてのファイル(*.*)|*.*", BookDataRJB.GetStringLoadFormat()); ;
				//[ファイルの種類]ではじめに「すべてのファイル」が選択されているようにする
				ofd.FilterIndex = 1;
				//タイトルを設定する
				ofd.Title = "開くファイルを選択してください";
				//ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
				ofd.RestoreDirectory = true;

				var ret = ofd.ShowDialog();
				if (ret == System.Windows.Forms.DialogResult.OK)
				{
					SetNewBook(ofd.FileNames);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("データ生成で例外を発生しました。入力データのフォーマットを確認してください", "例外発生", MessageBoxButtons.OK, MessageBoxIcon.Error);
				DevelopLog.LogException(MethodBase.GetCurrentMethod(), "入力データ生成で例外を発生しました。入力データのフォーマットを確認してください", ex);
			}
		}


		/// <summary>「新規画像フォルダ」が選択されたときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void 新規画像フォルダToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				// フォルダ指定ダイアログ
				FolderBrowserDialog fbd = new FolderBrowserDialog();
				fbd.ShowNewFolderButton = false;            // 新しいフォルダ生成のボタンは表示しない
				fbd.SelectedPath = CommonValues.ConfigData.InputFolder;
				var ret = fbd.ShowDialog();
				if (ret == System.Windows.Forms.DialogResult.OK)
				{
					SetNewBook(new string[] { fbd.SelectedPath });
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("データ生成で例外を発生しました。入力データのフォーマットを確認してください", "例外発生", MessageBoxButtons.OK, MessageBoxIcon.Error);
				DevelopLog.LogException(MethodBase.GetCurrentMethod(), "入力データ生成で例外を発生しました。入力データのフォーマットを確認してください", ex);
			}
		}


		/// <summary>「プロジェクトファイル保存」が選択されたときの処理です </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void プロジェクトファイル保存ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{

				var sfd = new SaveFileDialog();                                 // ファイルダイアログ

				//はじめに表示されるフォルダを指定する
				if (CommonValues.ConfigData.OutputFolder != null)
				{
					sfd.InitialDirectory = CommonValues.ConfigData.OutputFolder;
				}

				sfd.Filter = String.Format("プロジェクトファイル|*{0}|すべてのファイル(*.*)|*.*", CommonValues.ExtBookProject);       //[ファイルの種類]に表示される選択肢を指定する
				sfd.FilterIndex = 0;                                                        //[ファイルの種類]ではじめに「プロジェクトファイル」が選択されているようにする
				sfd.Title = "保存するファイル名を指定してください";                           //タイトルを設定する
																			//はじめのファイル名を指定する
				sfd.FileName = Path.GetFileName(_project.BookData.CommonFolderPath) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");        // 共通のフォルダ名（通常は親フォルダ）
																																			//はじめに表示されるフォルダを指定する
				sfd.InitialDirectory = CommonValues.ConfigData.OutputFolder;
				sfd.RestoreDirectory = true;                                                //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする

				var ret = sfd.ShowDialog();
				if (ret == System.Windows.Forms.DialogResult.OK)
				{
					_project.SaveXmlFile(sfd.FileName);

					// 出力フォルダを設定値に保存
					CommonValues.ConfigData.OutputFolder = Path.GetDirectoryName(sfd.FileName);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("プロジェクトファイル保存で例外を発生しました。", "例外発生", MessageBoxButtons.OK, MessageBoxIcon.Error);
				DevelopLog.LogException(MethodBase.GetCurrentMethod(), "プロジェクトファイル保存で例外を発生しました。", ex);
			}
		}



		/// <summary>「プロジェクトファイルの読み込み」が選択されたときの処理です </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void プロジェクトファイルの読み込みToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				var ofd = new OpenFileDialog();                                         // ファイルダイアログ

				//はじめに表示されるフォルダを指定する。XMLは出力先のフォルダをデフォルトとする。。
				if (CommonValues.ConfigData.OutputFolder != null)
				{
					ofd.InitialDirectory = CommonValues.ConfigData.OutputFolder;
				}
				ofd.Multiselect = false;                                                // マルチセレクトは不可
				ofd.Filter = String.Format("プロジェクトファイル|*{0}|すべてのファイル(*.*)|*.*", CommonValues.ExtBookProject);       //[ファイルの種類]に表示される選択肢を指定する
				ofd.FilterIndex = 0;                                                    //[ファイルの種類]ではじめに「プロジェクトファイル」が選択されているようにする
				ofd.Title = "読み込むファイル名を指定してください";                       //タイトルを設定する
				ofd.RestoreDirectory = true;                                            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする

				var ret = ofd.ShowDialog();
				if (ret == System.Windows.Forms.DialogResult.OK)
				{
					if (_project != null) _project.Dispose();
					_project = new BookProject(ofd.FileName);

					// PDFファイルの展開
					if (_project.SetData.PdfPath != null && _project.SetData.PdfPath.Any()) PdftoJpg(_project.SetData.PdfPath,_pdfworkPath);

					// IIIFファイルの展開
					if (_project.SetData.IiifPath != null && _project.SetData.IiifPath.Any()) IiiftoJpg(_project.ManifestToURL(), _iiifworkPath);

					// 入出力フォルダを設定値に保存
					CommonValues.ConfigData.OutputFolder = Path.GetDirectoryName(ofd.FileName);

					// Projectの再描画
					if (_project.BookData.PageDataList.Any())
					{
						_project.BookData.CurrentIndex = 0;
					}
					while (!DispNewData())
					{
						//画像ファイルが見つからない場合
						MessageBox.Show("画像ファイルが読み込めませんでした。\n画像ファイルのフォルダを再指定してください。", "エラー発生", MessageBoxButtons.OK, MessageBoxIcon.Error);
						// フォルダ指定ダイアログ
						FolderBrowserDialog fbd = new FolderBrowserDialog();
						fbd.ShowNewFolderButton = false;            // 新しいフォルダ生成のボタンは表示しない
						fbd.SelectedPath = CommonValues.ConfigData.InputFolder;
						ret = fbd.ShowDialog();
						if (ret == System.Windows.Forms.DialogResult.OK)
						{
							foreach (var pd in _project.BookData.PageDataList)
							{
								pd.SetFilePath(fbd.SelectedPath + "\\" + Path.GetFileName(pd.FilePath)); // ファイルパス更新
							}
						}
						else break;
					}
					DrawDataGridViewList();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("プロジェクトファイルの読み込み", "例外発生", MessageBoxButtons.OK, MessageBoxIcon.Error);
				DevelopLog.LogException(MethodBase.GetCurrentMethod(), "プロジェクトファイルの読み込み", ex);
			}
		}


		/// <summary>カーソルを描画します</summary>
		/// <param name="curs">対象カーソル</param>
		/// <param name="gra">描画先グラフィック</param>
		/// <param name="defColor">カーソルデフォルト色</param>
		/// <param name="isSelect">選択状態</param>
		private void DrawCursor(ShapeLine curs, Graphics gra, Color defColor, bool isSelect = false)
		{
			// カーソル描画がfalseの場合、またはカーソル無効の場合にはリターンする
			if (cboxMeasPoint.Checked == false) return;

			var wid = 6.0f / (float)_scalePbox.DispRate;
			using (var penCur = isSelect ? new Pen(Color.Orange, wid) : new Pen(defColor, wid))
			{
				penCur.StartCap = penCur.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
				gra.DrawLine(penCur, curs.PosBgn, curs.PosEnd);
			}

			// 補助線
			using (var pen = new Pen(Color.Magenta, 1.0f / _scalePbox.DispRate))
			{
				var len = curs.GetDistance() / 4;       // 高さの 1/4
				var uv = curs.GetUnitVector();      // 単位ベクトル
				var uvN = new PointF((float)(uv.Y * len), (float)(-uv.X * len));            // 垂直なベクトル
				var bp0 = new SyPoint((int)(curs.PosBgn.X + uvN.X), (int)(curs.PosBgn.Y + uvN.Y));
				var bp9 = new SyPoint((int)(curs.PosBgn.X - uvN.X), (int)(curs.PosBgn.Y - uvN.Y));
				var be0 = new SyPoint((int)(curs.PosEnd.X + uvN.X), (int)(curs.PosEnd.Y + uvN.Y));
				var be9 = new SyPoint((int)(curs.PosEnd.X - uvN.X), (int)(curs.PosEnd.Y - uvN.Y));
				gra.DrawLine(pen, bp0, bp9);
				gra.DrawLine(pen, be0, be9);
			}
		}


		/// <summary>
		/// カレントページ存在確認チェックを行います
		/// </summary>
		/// <returns>カレントページ存在</returns>
		private bool IsExistCurrentPage()
		{
			if (_project == null || _project.BookData == null || _project.BookData.PageDataList == null || _project.BookData.PageDataList.Length < 1) return false;
			return true;
		}


		/// <summary>カレントのページデータを取得します。ない場合には null となります。 </summary>
		/// <returns>カレントページ PageDataRJBインスタンスまたはnull</returns>
		private PageDataRJB CurrentPageOrDefault()
		{
			return (IsExistCurrentPage()) ? _project.BookData.CurrentPage : null;
		}



		/// <summary>マニュアル編集ボタンが押されたときの処理です </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void btnManualAdjust_Click(object sender, EventArgs e)
		{
			var cp = CurrentPageOrDefault();
			if (cp == null) return;
			var ifcf = (Control.ModifierKeys & Keys.Control) == 0;

			// 強制実行となる場合には、実行確認を取る
			if (ifcf == false)
			{
				if (MessageBox.Show("測定点が強制的にリセットされます。　よろしいですか？", "実行確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;
			}

			cp.SetDefaultCursor(ifcf, _project.SetData);
			cp.ChangeLine();
			DispNewData();
			RedrawCurrentRowDgv();
			Refresh();
		}



		/// <summary>
		/// 「自動計測」ボタンがクリックされたときの処理です
		/// </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void btnExec_Click(object sender, EventArgs e)
		{
			if (_project == null || _project.BookData == null || _project.BookData.PageDataList == null || _project.BookData.PageDataList.Length == 0)
			{
				return;
			}
			if (_formExecAllDetect != null) return;


			var isIfCond = (Control.ModifierKeys & Keys.Control) == 0;

			// 実行確認
			var msg = String.Format("全データの自動計測を実行します。よろしいですか？（{0}）", isIfCond ? "手動で変更した値は対象に含みません " : "全データ");
			if (MessageBox.Show(msg, "実行確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;

			// コントロール群をマスク
			foreach (var c in _maskExecControls) c.Enabled = false;

			//　実行ダイアログ
			_formExecAllDetect = new FormExecAllAutoDetect();
			_formExecAllDetect.SetMaxCount(_project.BookData.PageDataList.Length);
			_formExecAllDetect.Show();
			var t = Task.Factory.StartNew(() =>
			{
				// 別スレッドで実行
				var prm = new Dictionary<string, object>();
				prm["BinMode"] = cboxBinMethod.Checked;
				_project.BookData.AllAutoDetect(isIfCond, OnCountAutoDetect, _project.SetData, prm, cboxOldMethod.Checked);
				if (cboxMeasurmentMatch.Checked) MeasurmentMatch();
				Thread.Sleep(10);
			})
			.ContinueWith(o =>
			{
				// コントロール群をマスク解除
				foreach (var c in _maskExecControls) c.Enabled = true;
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}


		/// <summary>
		/// 全体表示がクリックされたときの処理です
		/// </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void btnAutoSize_Click(object sender, EventArgs e)
		{
			_scalePbox.Zoom();
		}


		/// <summary>
		/// ROIリセットボタンがクリックされたときの処理です。
		/// </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void btnResetRoi_Click(object sender, EventArgs e)
		{
			if (_project.SetData == null || _project.BookData == null || _project.BookData.CurrentImage == null) return;
			var ret = MessageBox.Show("ROI情報をリセットします。よろしいですか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			if (ret == System.Windows.Forms.DialogResult.OK)
			{
				_project.SetData.ResetRoi(_project.BookData.CurrentImage.Size);
				_scalePbox.DrawMainImage();
				Refresh();
			}
		}


		/// <summary>「ROI設定読み込み」が選択されたときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void rOI設定読み込みToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				var ofd = new OpenFileDialog();                                         // ファイルダイアログ

				//はじめに表示されるフォルダを指定する。XMLは出力先のフォルダをデフォルトとする。。
				if (CommonValues.ConfigData.OutputFolder != null)
				{
					ofd.InitialDirectory = CommonValues.ConfigData.OutputFolder;
				}
				ofd.Multiselect = false;                                                // マルチセレクトは不可
				ofd.Filter = String.Format("設定ファイル(XML)|*.xml|すべてのファイル(*.*)|*.*", CommonValues.ExtBookProject);     //[ファイルの種類]に表示される選択肢を指定する
				ofd.FilterIndex = 0;                                                    //[ファイルの種類]ではじめに「設定ファイル(XML)」が選択されているようにする
				ofd.Title = "保存するファイル名を指定してください";                       //タイトルを設定する
				ofd.RestoreDirectory = true;                                            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする

				var ret = ofd.ShowDialog();
				if (ret == System.Windows.Forms.DialogResult.OK)
				{
					if (_project != null) _project.Dispose();
					_project.SetData.LoadXmlFile(ofd.FileName);
					// 入出力フォルダを設定値に保存
					CommonValues.ConfigData.OutputFolder = Path.GetDirectoryName(ofd.FileName);
					DispNewData();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("設定ファイル(XML)読み込み", "例外発生", MessageBoxButtons.OK, MessageBoxIcon.Error);
				DevelopLog.LogException(MethodBase.GetCurrentMethod(), "設定ファイル(XML)の読み込み", ex);
			}

		}



		/// <summary>「ROI設定保存」が選択されたときの処理です </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void rOI設定保存ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				var sfd = new SaveFileDialog();                                 // ファイルダイアログ

				//はじめに表示されるフォルダを指定する
				if (CommonValues.ConfigData.OutputFolder != null)
				{
					sfd.InitialDirectory = CommonValues.ConfigData.OutputFolder;
				}

				sfd.Filter = "設定ファイル(XML)|*.XML|すべてのファイル(*.*)|*.*";     //[ファイルの種類]に表示される選択肢を指定する
				sfd.FilterIndex = 0;                                                        //[ファイルの種類]ではじめに「プロジェクトファイル」が選択されているようにする
				sfd.Title = "保存するファイル名を指定してください";                           //タイトルを設定する
																			//はじめのファイル名を指定する
				sfd.FileName = Path.GetFileName(_project.BookData.CommonFolderPath) + "_Settings_" + DateTime.Now.ToString("yyyyMMddHHmmss");       // 共通のフォルダ名（通常は親フォルダ）
																																					//はじめに表示されるフォルダを指定する
				sfd.InitialDirectory = CommonValues.ConfigData.OutputFolder;
				sfd.RestoreDirectory = true;                                                //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする

				var ret = sfd.ShowDialog();
				if (ret == System.Windows.Forms.DialogResult.OK)
				{
					_project.SetData.SaveXmlFile(sfd.FileName);

					// 出力フォルダを設定値に保存
					CommonValues.ConfigData.OutputFolder = Path.GetDirectoryName(sfd.FileName);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("設定ファイル(XML)保存で例外を発生しました。", "例外発生", MessageBoxButtons.OK, MessageBoxIcon.Error);
				DevelopLog.LogException(MethodBase.GetCurrentMethod(), "設定ファイル(XML)保存で例外を発生しました。", ex);
			}
		}


		/// <summary>
		/// 「テスト計測カレント実行」が選択されているときの処理です。選択されているデータのみを計測します。
		/// </summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void テスト計測カレント実行ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExecuteCurrentData();
		}


		/// <summary>
		/// カレントデータを実行します
		/// </summary>
		private void ExecuteCurrentData()
		{
			if (_project == null || _project.BookData == null || _project.BookData.CurrentImage == null || _project.BookData.CurrentPage == null)
			{
				MessageBox.Show("データがありません。実行をキャンセルします", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			var prm = new Dictionary<string, object>();
			prm["BinMode"] = cboxBinMethod.Checked;
			_project.BookData.CurrentPage.AutoDetect(_project.BookData.CurrentImage, _project.SetData, prm, cboxOldMethod.Checked);
			RedrawRowDgv(_project.BookData.CurrentIndex);
		}


		/// <summary>
		/// 指定データを実行します
		/// </summary>
		/// <param name="pageindex">ページインデックス</param>
		private void ExecutePageData(int pageindex)
		{
			if (_project == null || _project.BookData == null || _project.BookData.CurrentImage == null || pageindex < 0 || pageindex >= _project.BookData.PageDataList.Length)
			{
				MessageBox.Show("データがありません。実行をキャンセルします", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (pageindex == _project.BookData.CurrentIndex) ExecuteCurrentData();      // こちらの方が、画像読み出しの点で有利なので。
			else
			{
				var prm = new Dictionary<string, object>();
				prm["BinMode"] = cboxBinMethod.Checked;
				_project.BookData.PageDataList[pageindex].AutoDetect(null, _project.SetData, prm, cboxOldMethod.Checked);
				RedrawRowDgv(pageindex);
			}
		}

		/// <summary>「ROIサイズ合わせ」を選択したときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void rOIサイズ合わせToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_baseSelectedRoi == null) return;
			if (MessageBox.Show("ROIサイズ合わせを実行します。よろしいですか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
			{
				var rois = _allRois.Skip(_baseSelectRoiIndex < 4 ? 0 : 4).Take(4);
				var siz = _baseSelectedRoi.RoiSize;
				foreach (var ro in rois)
				{
					ro.RoiSize = siz;
				}
			}
			_scalePbox.DrawMainImage();

		}

		/// <summary>「ROI高さ位置合わせ」を選択したときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void rOI高さ位置合わせToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_baseSelectedRoi == null) return;
			if (MessageBox.Show("高さ位置合わせを実行します。よろしいですか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
			{
				var b = _baseSelectRoiIndex / 4 * 4;        // 0 or 4
				var ro = _allRois[b + (7 - _baseSelectRoiIndex % 4) % 4];
				ro.BasePoint = new SyPoint(ro.BasePoint.X, _baseSelectedRoi.BasePoint.Y);
				ro.RoiSize = new SySize(ro.RoiSize.Width, _baseSelectedRoi.RoiSize.Height);

			}
			_scalePbox.DrawMainImage();

		}

		/// <summary>「ROI左右位置合わせ」を選択したときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void rOI左右位置合わせToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_baseSelectedRoi == null) return;
			if (MessageBox.Show("左右位置合わせを実行します。よろしいですか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
			{
				var idx = _baseSelectRoiIndex ^ 0x01;       // 0<->1, 2<=>3, 4<->5, 6<->7
				var ro = _allRois[idx];
				ro.BasePoint = new SyPoint(_baseSelectedRoi.BasePoint.X, ro.BasePoint.Y);
				ro.RoiSize = new SySize(_baseSelectedRoi.RoiSize.Width, ro.RoiSize.Height);
			}
			_scalePbox.DrawMainImage();

		}

		/// <summary>コンテキストメニューでROI調整を選択した時点の基準となるROIを保持します</summary>
		private BookROI _baseSelectedRoi;
		/// <summary>コンテキストメニューでROI調整を選択した時点の基準となるROIのインデックスを保持します</summary>
		private int _baseSelectRoiIndex;

		/// <summary>コンテキストメニュー表示中</summary>
		private bool _isOpenPboxContextMenu = false;


		/// <summary>ピクチャーボックスでのコンテキストメニューが開かれるときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		/// <remarks>選択できないメニューを選択不可にします</remarks>
		private void contextMenuPbox_Opening(object sender, CancelEventArgs e)
		{
			bool isInRoi = false;
			_isOpenPboxContextMenu = true;
			// ROI調整モードでかつカーソルがROIの中にある。
			_allRois = new BookROI[] { _project.SetData.RoiPTL, _project.SetData.RoiPBL, _project.SetData.RoiPBR, _project.SetData.RoiPTR, _project.SetData.RoiGTL, _project.SetData.RoiGBL, _project.SetData.RoiGBR, _project.SetData.RoiGTR };
			_baseSelectRoiIndex = -1;
			_baseSelectedRoi = null;
			if (_lastPos != null)
			{
				var w = _allRois.Select((ro, i) => new { Ro = ro, Index = i }).Where(ro => ro.Ro.Rect.Contains(_lastPos)).OrderBy(ro => ro.Ro.RoiSize.Width * ro.Ro.RoiSize.Height).FirstOrDefault();
				if (w != null)
				{
					_baseSelectRoiIndex = w.Index;
					_baseSelectedRoi = w.Ro;
				}
			}
			isInRoi = (cboxRoiInfo.Checked && _lastPos != null && _baseSelectedRoi != null);
			this.rOI調整ToolStripMenuItem.Visible = isInRoi;
			bool isInImage = (_project != null && _project.BookData != null && _project.BookData.PageDataList != null && _project.BookData.PageDataList.Length != 0);
			this.検出ライン表示ToolStripMenuItem.Visible = isInImage;
			if (isInImage)
			{
				var pd = _project.BookData.PageDataList[_project.BookData.CurrentIndex];
				if (pd != null)
				{
					this.紙左側ToolStripMenuItem.Checked = (pd.SideL.MeasurePaper != null && pd.SideL.HeightPixPaper != 0);
					this.紙右側ToolStripMenuItem.Checked = (pd.SideR.MeasurePaper != null && pd.SideR.HeightPixPaper != 0);
					this.匡郭左側ToolStripMenuItem.Checked = (pd.SideL.MeasureGrid != null && pd.SideL.HeightPixGrid != 0);
					this.匡郭右側ToolStripMenuItem.Checked = (pd.SideR.MeasureGrid != null && pd.SideR.HeightPixGrid != 0);
				}
			}
			_scalePbox.DrawMainImage();
		}


		/// <summary>ピクチャーボックスでのコンテキストメニューが閉じられるときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		/// <remarks>選択できないメニューを選択不可にします</remarks>
		private void contextMenuPbox_Closed(object sender, ToolStripDropDownClosedEventArgs e)
		{
			_isOpenPboxContextMenu = false;
		}


		/// <summary>リッド上のコンテキストメニューで「カーソル選択データの実行」が選択されたときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void カーソル選択データの実行ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_project == null || _project.BookData == null || _project.BookData.PageDataList == null || _project.BookData.PageDataList.Length == 0)
			{
				return;
			}
			if (_formExecAllDetect != null) return;

			if (dataGridViewMain.Rows.Count == 0) return;
			// カーソル選択データのコレクション
			// LINQ中の wnoについてはもっと良い方法があると思うので、後ほどリファクタリングする。Indexは表示されている値から１を引いたもの
			var selRowsNo = dataGridViewMain.SelectedRows.Cast<DataGridViewRow>().Select(row => row.Cells["PageIdx"].Value.ToString()).Where(no => { int wno; return int.TryParse(no, out wno); }).Select(no => int.Parse(no) - 1);
			var selList = selRowsNo.Where(no => (no >= 0) && no < _project.BookData.PageDataList.Length).Select(no => _project.BookData.PageDataList[no]).ToArray();

			// 実行確認
			var msg = $"選択データ{selList.Length}件の自動計測を実行します。よろしいですか？";
			if (MessageBox.Show(msg, "実行確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;

			// コントロール群をマスク
			foreach (var c in _maskExecControls) c.Enabled = false;

			//　実行ダイアログ
			_formExecAllDetect = new FormExecAllAutoDetect();
			_formExecAllDetect.SetMaxCount(selList.Length);
			_formExecAllDetect.Show();
			var t = Task.Factory.StartNew(() =>
			{
				// 別スレッドで実行
				var prm = new Dictionary<string, object>();
				prm["BinMode"] = cboxBinMethod.Checked;
				_project.BookData.SelectedPagesAutoDetect(selList, false, OnCountAutoDetect, _project.SetData, prm, cboxOldMethod.Checked);
				Thread.Sleep(10);
			})
			.ContinueWith(o =>
			{
				// コントロール群をマスク解除
				foreach (var c in _maskExecControls) c.Enabled = true;
			}, TaskScheduler.FromCurrentSynchronizationContext());

		}

		/// <summary>
		/// キーが押されたときの処理です </summary>
		/// <param name="sender">送信元</param>
		/// <param name="e">イベント情報</param>
		private void FormBookMain_KeyDown(object sender, KeyEventArgs e)
		{
			//　コントロールが押されているとき。
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
			{
				switch (e.KeyCode)
				{
					//Ctrl + ShiftのOn-Off時に座標のSavePointを更新
					case Keys.Shift:
						SetSavePoint();
						e.Handled = true;           // 以下のコントロールにキーを渡さない
						break;
					//Ctrl + +キー：微小拡大
					case Keys.Add:
					case Keys.Oemplus:
						_scalePbox.RatePlus(true);
						e.Handled = true;           // 以下のコントロールにキーを渡さない
						break;
					//Ctrl + -キー：微小縮小
					case Keys.Subtract:
					case Keys.OemMinus:
						_scalePbox.RateMinus(true);
						e.Handled = true;           // 以下のコントロールにキーを渡さない
						break;
				}
			}
			else
			{
				switch (e.KeyCode)
				{
					//+キー：拡大
					case Keys.Add:
					case Keys.Oemplus:
						_scalePbox.RatePlus(false);
						e.Handled = true;           // 以下のコントロールにキーを渡さない
						break;
					//-キー：縮小
					case Keys.Subtract:
					case Keys.OemMinus:
						_scalePbox.RateMinus(false);
						e.Handled = true;           // 以下のコントロールにキーを渡さない
						break;
				}
			}
		}


		/// <summary>
		///  測定位置合わせチェックボックスに変更があったときの処理です。 </summary>
		/// <param name="sender">送信元</param>
		/// <param name="e">イベント情報</param>
		private void CboxMeasurmentMatch_CheckedChanged(object sender, EventArgs e)
		{
			var cb = sender as CheckBox;
			if (cb.Checked == true)
			{
				// 検出ライン情報の描画
				MeasurmentMatch();

				if (rOI垂直モードToolStripMenuItem.Checked)
				{
					_project.SetData.RoiPTL.BasePoint = new SyPoint(_project.SetData.RoiGTL.BasePoint.X, _project.SetData.RoiPTL.BasePoint.Y);
					_project.SetData.RoiPTL.RoiSize = new SySize(_project.SetData.RoiGTL.RoiSize.Width, _project.SetData.RoiPTL.RoiSize.Height);
					_project.SetData.RoiPBL.BasePoint = new SyPoint(_project.SetData.RoiGBL.BasePoint.X, _project.SetData.RoiPBL.BasePoint.Y);
					_project.SetData.RoiPBL.RoiSize = new SySize(_project.SetData.RoiGBL.RoiSize.Width, _project.SetData.RoiPBL.RoiSize.Height);
					_project.SetData.RoiPTR.BasePoint = new SyPoint(_project.SetData.RoiGTR.BasePoint.X, _project.SetData.RoiPTR.BasePoint.Y);
					_project.SetData.RoiPTR.RoiSize = new SySize(_project.SetData.RoiGTR.RoiSize.Width, _project.SetData.RoiPTR.RoiSize.Height);
					_project.SetData.RoiPBR.BasePoint = new SyPoint(_project.SetData.RoiGBR.BasePoint.X, _project.SetData.RoiPBR.BasePoint.Y);
					_project.SetData.RoiPBR.RoiSize = new SySize(_project.SetData.RoiGBR.RoiSize.Width, _project.SetData.RoiPBR.RoiSize.Height);
				}
			}
			// 表示変更チェックボックス変更時　共通メソッド
			CheckedChange_DispDebug(sender, e);
		}

		/// <summary>
		/// 2つの測定ラインを重ね合わせます。
		/// </summary>
		private void MeasurmentMatch()
		{
			// 検出ライン情報の描画
			foreach (var pd in _project.BookData.PageDataList)
			{
				if (pd == null) break;
				if (pd.SideL.MeasurePaper != null && pd.SideL.MeasureGrid != null)
				{
					pd.SideL.MeasurePaper.SetShiftPoint();
				}
				if (pd.SideR.MeasurePaper != null && pd.SideR.MeasureGrid != null)
				{
					pd.SideR.MeasurePaper.SetShiftPoint();
				}
			}
		}

		/// <summary>
		/// マウスの左クリックを上げたときの動作です。測定位置合わせOffからOnに切り替え時に合わせる座標をセット
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pictureBoxMain_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				SetSavePoint();
			}
		}

		/// <summary>
		/// 現在の測定座標を直線の計算式の座標に保存します。/// </summary>
		private void SetSavePoint()
		{
			if (_project == null || _project.BookData == null || _project.BookData.PageDataList == null || _project.BookData.PageDataList.Length == 0)
			{
				return;
			}
			foreach (var sl in _curLines)
			{
				sl.SetSavePoint();
			}
		}


		//_allRois = new BookROI[] { _project.SetData.RoiPTL, _project.SetData.RoiPBL, _project.SetData.RoiPBR, _project.SetData.RoiPTR,
		//_project.SetData.RoiGTL, _project.SetData.RoiGBL, _project.SetData.RoiGBR, _project.SetData.RoiGTR	};


		/// <summary>「ROI高さ位置合わせ一括左側優先」を選択したときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void rOI高さ位置合わせ一括左側優先ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("左側ROIの高さに、右側ROIを一括で位置合わせします。\nよろしいですか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
			{
				int[] lindex = new int[4] { 0, 1, 4, 5 };
				foreach (int i in lindex)
				{
					var b = i / 4 * 4;        // 0 or 4
					var ro = _allRois[b + (7 - i % 4) % 4];
					ro.BasePoint = new SyPoint(ro.BasePoint.X, _allRois[i].BasePoint.Y);
					ro.RoiSize = new SySize(ro.RoiSize.Width, _allRois[i].RoiSize.Height);
				}
			}
			_scalePbox.DrawMainImage();
		}

		/// <summary>「ROI高さ位置合わせ一括右側優先」を選択したときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void rOI高さ位置合わせ一括右側優先ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("右側ROIの高さに、左側ROIを一括で位置合わせします。\nよろしいですか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
			{
				int[] index = new int[4] { 2, 3, 6, 7 };
				foreach (int i in index)
				{
					var b = i / 4 * 4;        // 0 or 4
					var ro = _allRois[b + (7 - i % 4) % 4];
					ro.BasePoint = new SyPoint(ro.BasePoint.X, _allRois[i].BasePoint.Y);
					ro.RoiSize = new SySize(ro.RoiSize.Width, _allRois[i].RoiSize.Height);
				}
			}
			_scalePbox.DrawMainImage();
		}

		/// <summary>キーを離したときの処理です</summary>
		/// <param name="sender">送信元情報</param>
		/// <param name="e">イベント情報</param>
		private void FormBookMain_KeyUp(object sender, KeyEventArgs e)
		{
			//　Ctrlを押した状態で、Shiftキーを離した場合の処理
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control && e.KeyCode == Keys.ShiftKey)
			{
				//Ctrl + ShiftのOn-Off時に座標のSavePointを更新
				SetSavePoint();
			}
		}

		/// <summary>新規画像URLを選択したときの動作です。</summary>
		/// <param name="sender">送信元</param>
		/// <param name="e">イベント情報</param>
		private void 新規画像URLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ManifestInput manifestInput = new ManifestInput();
			manifestInput.ShowDialog(this);

			string manifesturl = manifestInput.FileURL;
			string manifest = "";
			if (File.Exists(manifesturl))
			{
				StreamReader sr = new StreamReader(manifesturl, Encoding.GetEncoding("Shift_JIS"));
				manifest = sr.ReadToEnd();
				sr.Close();
			}
			else
			{
				WebClient client = new WebClient();
				try
				{
					manifest = client.DownloadString(manifesturl);
				}
				catch (Exception ex)
				{
					MessageBox.Show("manifestが開けません、URIが正しいか確認してください。", "エラー発生", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
			manifestInput.Dispose();

			SetNewBook(new string[] { manifest });
		}

		/// <summary>紙（左側）をクリックしたときの動作です</summary>
		/// <param name="sender">送信元</param>
		/// <param name="e">イベント情報</param>
		private void 紙左側ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var pd = _project.BookData.PageDataList[_project.BookData.CurrentIndex];
			if (pd != null)
			{
				if (this.紙左側ToolStripMenuItem.Checked)
				{
					pd.SideL.MeasurePaper.ResetPoint();
					pd.ChangeLine();
					pd.SideL.MeasurePaper = null;
				}
				else
				{
					pd.SideL.MeasurePaper = new ShapeLineEx(new SyPoint(_project.SetData.RoiPTL.BasePoint.X + _project.SetData.RoiPTL.RoiSize.Width / 2,
																		_project.SetData.RoiPTL.BasePoint.Y + _project.SetData.RoiPTL.RoiSize.Height / 2),
															new SyPoint(_project.SetData.RoiPBL.BasePoint.X + _project.SetData.RoiPBL.RoiSize.Width / 2,
																		_project.SetData.RoiPBL.BasePoint.Y + _project.SetData.RoiPBL.RoiSize.Height / 2));

					if (pd.SideL.MeasureGrid != null)
					{
						pd.SideL.MeasurePaper._link = pd.SideL.MeasureGrid;
						pd.SideL.MeasureGrid._link = pd.SideL.MeasurePaper;
					}
					pd.ChangeLine();
				}
				DispNewData();
				RedrawCurrentRowDgv();
				Refresh();
			}
		}

		/// <summary>紙（右側）をクリックしたときの動作です</summary>
		/// <param name="sender">送信元</param>
		/// <param name="e">イベント情報</param>
		private void 紙右側ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var pd = _project.BookData.PageDataList[_project.BookData.CurrentIndex];
			if (pd != null)
			{
				if (this.紙右側ToolStripMenuItem.Checked)
				{
					pd.SideR.MeasurePaper.ResetPoint();
					pd.ChangeLine();
					pd.SideR.MeasurePaper = null;
				}
				else
				{
					pd.SideR.MeasurePaper = new ShapeLineEx(new SyPoint(_project.SetData.RoiPTR.BasePoint.X + _project.SetData.RoiPTR.RoiSize.Width / 2,
																		_project.SetData.RoiPTR.BasePoint.Y + _project.SetData.RoiPTR.RoiSize.Height / 2),
															new SyPoint(_project.SetData.RoiPBR.BasePoint.X + _project.SetData.RoiPBR.RoiSize.Width / 2,
																		_project.SetData.RoiPBR.BasePoint.Y + _project.SetData.RoiPBR.RoiSize.Height / 2));
					if (pd.SideR.MeasureGrid != null)
					{
						pd.SideR.MeasurePaper._link = pd.SideR.MeasureGrid;
						pd.SideR.MeasureGrid._link = pd.SideR.MeasurePaper;
					}
					pd.ChangeLine();
				}
				DispNewData();
				RedrawCurrentRowDgv();
				Refresh();
			}
		}

		/// <summary>匡郭（左側）をクリックしたときの動作です</summary>
		/// <param name="sender">送信元</param>
		/// <param name="e">イベント情報</param>
		private void 匡郭左側ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var pd = _project.BookData.PageDataList[_project.BookData.CurrentIndex];
			if (pd != null)
			{
				if (this.匡郭左側ToolStripMenuItem.Checked)
				{
					pd.SideL.MeasureGrid.ResetPoint();
					pd.ChangeLine();
					pd.SideL.MeasureGrid = null;
				}
				else
				{
					pd.SideL.MeasureGrid = new ShapeLineEx(new SyPoint(_project.SetData.RoiGTL.BasePoint.X + _project.SetData.RoiGTL.RoiSize.Width / 2,
															           _project.SetData.RoiGTL.BasePoint.Y + _project.SetData.RoiGTL.RoiSize.Height / 2),
														   new SyPoint(_project.SetData.RoiGBL.BasePoint.X + _project.SetData.RoiGBL.RoiSize.Width / 2,
																	   _project.SetData.RoiGBL.BasePoint.Y + _project.SetData.RoiGBL.RoiSize.Height / 2));
					if (pd.SideL.MeasurePaper != null)
					{
						pd.SideL.MeasureGrid._link = pd.SideL.MeasurePaper;
						pd.SideL.MeasurePaper._link = pd.SideL.MeasureGrid;
					}
					pd.ChangeLine();
				}
				DispNewData();
				RedrawCurrentRowDgv();
				Refresh();
			}
		}

		/// <summary>匡郭（右側）をクリックしたときの動作です</summary>
		/// <param name="sender">送信元</param>
		/// <param name="e">イベント情報</param>
		private void 匡郭右側ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var pd = _project.BookData.PageDataList[_project.BookData.CurrentIndex];
			if (pd != null)
			{
				if (this.匡郭右側ToolStripMenuItem.Checked)
				{
					pd.SideR.MeasureGrid.ResetPoint();
					pd.ChangeLine();
					pd.SideR.MeasureGrid = null;
				}
				else
				{
					pd.SideR.MeasureGrid = new ShapeLineEx(new SyPoint(_project.SetData.RoiGTR.BasePoint.X + _project.SetData.RoiGTR.RoiSize.Width / 2,
																	   _project.SetData.RoiGTR.BasePoint.Y + _project.SetData.RoiGTR.RoiSize.Height / 2),
														   new SyPoint(_project.SetData.RoiGBR.BasePoint.X + _project.SetData.RoiGBR.RoiSize.Width / 2,
																	   _project.SetData.RoiGBR.BasePoint.Y + _project.SetData.RoiGBR.RoiSize.Height / 2));
					if (pd.SideR.MeasurePaper != null)
					{
						pd.SideR.MeasureGrid._link = pd.SideR.MeasurePaper;
						pd.SideR.MeasurePaper._link = pd.SideR.MeasureGrid;
					}
					pd.ChangeLine();
				}
				DispNewData();
				RedrawCurrentRowDgv();
				Refresh();
			}
		}
	}
}
