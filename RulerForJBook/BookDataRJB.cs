using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Reflection;


namespace RulerJB
{
	/// <summary>管理データクラスです</summary>
	/// <remarks>１つの和本に対応する管理情報で各ページごとの情報を内包します</remarks>
	class BookDataRJB : IXmlProject
	{
		///// <summary>データのパスを取得します</summary>
		//public string FolderPath { get{ return _folderPath; } }

		/// <summary>ページ情報のコレクションを取得します</summary>
		public PageDataRJB[] PageDataList { get { return _pageDataList; } }

		/// <summary>カレントページを取得します</summary>
		/// <remarks>プロキシパターンを利用しキャッシュ機能をサポートします</remarks>
		public Bitmap CurrentImage
		{
			get
			{
				if (_currentImage != null) return _currentImage;
				if (IsValid() == false) return null;
				// イメージデータを確認する。セットされていない場合にはファイルを読み込みセットする。（プロキシパターン）
				if (_currentImage == null)
				{
					SetCurrentImage();
				}
				return _currentImage;
			}
		}


		/// <summary>
		/// カレントインデックスを設定または取得します
		/// </summary>
		public int CurrentIndex
		{
			set
			{
				ClearCurrent();
				// 正常にセットされていない場合にはfalseリターンする
				if (_pageDataList == null || _pageDataList.Length < 1 || value< 0 || _pageDataList.Length <= value)
				{
					throw new Exception("不正なインデックス値がセットされました。　CurrentIndexプロパティ ");
				}
				_currentIndex = value;
			}

			get
			{
				return _currentIndex;
			}
			
		}


		/// <summary>現在のページ情報を取得します</summary>
		public PageDataRJB CurrentPage { get { return _pageDataList[_currentIndex]; } }


		/// <summary>共通部分のパスを取得します</summary>
		public string CommonFolderPath { get; private set; }


		///// <summary>データのパスを保持します（内部）</summary>
		//private string _folderPath;

		/// <summary>ページ情報のコレクションを保持します（内部） </summary>
		private PageDataRJB[] _pageDataList;

		/// <summary>カレントページを保持します（内部）</summary>
		private int _currentIndex = 0;


		/// <summary>カレントページの画像を保持します</summary>
		/// <remarks>画像はキャッシュとしてファイルから読み込んだ後、カレントが変更されたときにDisposeします</remarks>
		private Bitmap _currentImage = null;



		#region コンストラクタ -----------------------------
		/// <summary>コンストラクタです</summary>
		public BookDataRJB()
		{
			Clear();
		}




		/// <summary>ファイル群を指定したコンストラクタです</summary>
		/// <param title="files">ファイルパスリスト</param>
		public BookDataRJB(string[] files)
		{
			SetDataNameFromFiles(files);
		}


		/// <summary>フォルダ指定のコンストラクタです</summary>
		/// <param title="path">データパス</param>
		public BookDataRJB(string path)
		{
			SetDataNameFromFolder(path);
		}

		/// <summary>Xmlストリームによるコンストラクタ</summary>
		/// <param name="reader">Xmlストリーム</param>
		public BookDataRJB(XmlReader reader, string keyword ): this()
		{
			if (FromXmlReader(reader, keyword)== false )
			{
				throw new Exception("BookDataRJBコンストラクタ失敗");
			}
		}

		#endregion // コンストラクタ -----------------------------


		/// <summary>カレントイメージ（キャッシュ）情報をセットする</summary>
		/// <returns>成否</returns>
		private bool SetCurrentImage()
		{
			bool ret = false;
			if (_currentImage != null)
			{
				_currentImage.Dispose();
				_currentImage = null;
			}
			try
			{
				_currentImage = _pageDataList[_currentIndex].CreateBitmapFromPath();
				ret= true;
			}
			catch
			{
				_currentImage = null;
				ret = false;
			}

			return ret;
		}


		/// <summary>
		/// カレント情報をクリアします
		/// </summary>
		private void ClearCurrent()
		{
			_currentIndex = 0;
			if (_currentImage != null) _currentImage.Dispose();
			_currentImage = null;
		}


		/// <summary>インスタンスが有効であるかチェックします</summary>
		/// <returns>有効：true 無効：false</returns>
		public bool IsValid()
		{
			// 正常にセットされていない場合にはfalseリターンする
			if ( _pageDataList == null || _pageDataList.Length < 1 || _pageDataList.Length <= _currentIndex ) return false;

			// イメージデータを確認する。セットされていない場合にはファイルを読み込みセットする。
			if( _currentImage == null )
			{
				if( SetCurrentImage()== false ) return false;
			}
			return true;
		}


		/// <summary>ブックの全ページに対し自動検出を行います</summary>
		/// <param name="isIfCondFlag">条件による実行フラグ（真の場合、手動変更・バリデートしたデータは実行されません)</param>
		/// <param name="onCount">カウント時のアクション（無指定の場合、null）引数は処理番号、全データ数</param>
		/// <param name="setd">設定値データ</param>
		/// <param name="prm">汎用パラメータ</param>
		/// <param name="isOldMethod">旧メソッド（アルゴリズム）使用</param>
		public void AllAutoDetect(bool isIfCondFlag, Action<int, int> onCount = null, BookProjectSetData setd= null, Dictionary<string,object> prm= null, bool isOldMethod= false )
		{
			SelectedPagesAutoDetect(_pageDataList, isIfCondFlag, onCount, setd, prm, isOldMethod );		// 全ページ
		}


		/// <summary>ブック中の指定されたページ群に対し自動検出を行います</summary>
		/// <param name="selList">選択されているページ群</param>
		/// <param name="isIfCondFlag">条件による実行フラグ（真の場合、手動変更・バリデートしたデータは実行されません)</param>
		/// <param name="onCount">カウント時のアクション（無指定の場合、null）引数は処理番号、全データ数</param>
		/// <param name="setd">設定値データ</param>
		/// <param name="prm">汎用パラメータ</param>
		/// <param name="isOldMethod">旧メソッド（アルゴリズム）使用</param>
		public void SelectedPagesAutoDetect( PageDataRJB[] selList, bool isIfCondFlag, Action<int, int> onCount = null, BookProjectSetData setd = null, Dictionary<string, object> prm = null, bool isOldMethod = false)
		{

			int counter = 0;
			var wlst = selList.Select((n, i) => new { Index = i, Data = n });	// indexとデータをペアにしておく
			if (onCount != null) onCount(counter, selList.Length);
			//			Parallel.ForEach(selList, d =>
			foreach (var d in selList)
			{
				if (isIfCondFlag == false || (d.IsChanged == false && d.IsValidate == false))
				{
					d.AutoDetect(null, setd, prm, isOldMethod);
				}
				if (onCount != null) onCount(Interlocked.Increment(ref counter), selList.Length);
				System.GC.Collect(0);
				System.GC.Collect(1);
				System.GC.Collect(2);
			}
			//			});
			// メモリ不足を防ぐため、ガベージコレクションを強制的に実行
			System.GC.Collect();
		}


		/// <summary>変数間の調整を行います</summary>
		private void AdjustRelation()
		{
			// 共通フォルダプロパティのセット
			SetCommonFolderPath();
		}

		/// <summary>共通のフォルダパスをプロパティ CommonFolderPath にセットします</summary>
		private void SetCommonFolderPath()
		{
			// フォルダ間のセパレータを準備
			var SepStrs = new string[]{ @"\" };

			// 全ページ（ファイル）から異なるフォルダのみを列挙する。フォルダはセパレータで分割する。
			var fg = _pageDataList.Select(pd => Path.GetDirectoryName(pd.FilePath)).Distinct().Select(na => na.Split(SepStrs,StringSplitOptions.None)).ToArray();
			var ln = fg.Min(fp => fp.Length);		// 初期値は一番浅いフォルダ数から
			for (int i = 0; i < fg.Length-1 && ln> 0; i++)
			{
				int j = 0;
				for (; j < ln && fg[i][j]== fg[i+1][j]; j++){}
				ln = j;											// 一致している階層数を更新する。
			}

			// セパレータで先頭からln個のフォルダを連結して当該プロパティにセットする
			CommonFolderPath= String.Join(SepStrs[0], fg[0], 0, ln );
		}

		/// <summary>処理対象ファイルオープンフォーマットを取得します</summary>
		static public string GetStringLoadFormat()
		{
			return @"*.bmp;*.jpg;*.png;*.tif;*.jpeg;*.tiff;*.pdf";
		}

		// 対応イメージファイルの拡張子を登録します（拡張子がJPG,BMP,GIF,PNG,TIFのみ）。
		private string[] extList = new string[] { ".JPG", ".BMP", ".GIF", ".PNG", ".TIF", ".JPEG", ".TIFF" };



		/// <summary>指定されたファイル名またはフォルダからデータ名リストを取得します</summary>
		/// <param name="files">ファイル名またはフォルダ名</param>
		/// <returns>ファイル名群</returns>
		private string[] GetImageFilePathRecursive(string []files)
		{
			var plist = new List<string>();
			foreach (var fn in files)
			{
				// フォルダ名であれば、再帰的にそのフォルダ以下のファイルパスを取得する。
				if (Directory.Exists(fn))
				{
					plist.AddRange(GetImageFilePathRecursive(Directory.GetFiles(fn)));
					plist.AddRange(GetImageFilePathRecursive(Directory.GetDirectories(fn)));
				}
				else
				{
					if( extList.Contains(Path.GetExtension(fn).ToUpper() ) )
					{
						plist.Add( fn );
					}
				}
			}
			return plist.ToArray();
		}



		/// <summary>ファイル群を指定してデータ名リストをセットします</summary>
		/// <param title="files">ファイルパスリスト</param>
		private void InputDataFiles(string[] files)
		{
			Clear();

			_pageDataList = GetImageFilePathRecursive(files).OrderBy(path => path).Select((fn, i) => new PageDataRJB(fn, i)).ToArray();
			AdjustRelation();
		}



		/// <summary>ファイル群を指定してデータ名リストをセットします</summary>
		/// <param title="files">ファイルパスリスト</param>
		public void SetDataNameFromFiles(string[] files)
		{
			InputDataFiles(files);
			_currentIndex= 0;
			AdjustRelation();
		}



		/// <summary>指定フォルダ直下のファイルをデータ登録します</summary>
		/// <param title="path">データパス</param>
		public void SetDataNameFromFolder(string path)
		{
			Clear();
			//			_dataList.Clear();
			if (Directory.Exists(path) == true)
			{
				SetDataNameFromFiles(new string[]{path});
				_currentIndex = 0;
			}

			AdjustRelation();
		}


		/// <summary>データのクリア</summary>
		public void Clear()
		{
			ClearCurrent();
			_pageDataList = new PageDataRJB[0];
			_currentIndex = 0;
			CommonFolderPath = null;
		}



		/// <summary>Xmlでの書き込み</summary>
		/// <param name="xmlWriter">書き込み用ストリーム</param>
		/// <returns>成否</returns>
		public bool ToXmlWriter(XmlWriter writer, string keyword)
		{
			bool ret= true;
			try
			{
				writer.WriteStartElement(keyword);
				foreach( var lst in _pageDataList )
				{
					if( lst.ToXmlWriter( writer, "PageDataList" )== false ) ret= false;
				}

				if( CommonFolderPath != null ) {
					writer.WriteElementString("CommonFolderPath", CommonFolderPath.ToString());
				}


				// 他はCurrent・・・なので書き込まない


				writer.WriteEndElement();
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
			MethodBase methodbase = MethodBase.GetCurrentMethod();
			try
			{
				var pdList = new List<PageDataRJB>();
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.LocalName.Equals("CommonFolderPath"))
						{
							CommonFolderPath = reader.ReadString();	
						}
						if (reader.LocalName.Equals("PageDataList"))
						{
							pdList.Add(new PageDataRJB(reader, "PageDataList"));
						}
					}
					else if( reader.NodeType == XmlNodeType.EndElement )
					{
						if( reader.LocalName == keyword ) break;
					}
				}
				_pageDataList = pdList.ToArray();
			}

			catch
			{
				ret = false;
			}
			return ret;
		}
	}
}
