using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;

using OpenCvSharp;
using OpenCvSharp.Extensions;
using CvSize = OpenCvSharp.Size;
using SyPoint = System.Drawing.Point;


namespace RulerJB
{
	/// <summary>
	/// 自動ライン検出クラスです
	/// </summary>
	class AutoDetectLines:IDisposable
	{

		#region --------------------- メンバ変数 ---------------------
		/// <summary>指定されたビットマップの参照を保持します。</summary>
		/// <remarks>内部データ変換後は特に使用しません。</remarks>
		private Bitmap _orgimage;

		/// <summary>オリジナル画像をMatに変換したものを保持します</summary>
		private Mat _orgIplImage;

		/// <summary>グレー変換した画像を保持します</summary>
		private Mat _grayMat;

		/// <summary>２値化画像を保持します</summary>
		private Mat _binImage;

		/// <summary>検出されたライン群を保持します(Left)</summary>
		private ShapeLine[] _detectLinesL;

		/// <summary>検出されたライン群を保持します(Right)</summary>
		private ShapeLine[] _detectLinesR;
	
		/// <summary>
		/// 同期オブジェクト
		/// </summary>
		static private object _syncObject = new object();

		#endregion // --------------------- メンバ変数 ---------------------


		#region --------------------- プロパティ ---------------------

		/// <summary>検出されたライン群を取得します</summary>
		public ShapeLine[] DetectLinesL
		{
			// 無効な場合には0個の要素とする
			get { return (_detectLinesL!= null) ? _detectLinesL: new ShapeLine[0]; }
		}
		public ShapeLine[] DetectLinesR
		{
			// 無効な場合には0個の要素とする
			get { return (_detectLinesR != null) ? _detectLinesR : new ShapeLine[0]; }
		}
	
		#endregion // --------------------- プロパティ ---------------------


		/// <summary>
		/// コンストラクタです
		/// </summary>
		/// <param name="orgimage">対象画像</param>
		public AutoDetectLines(Bitmap orgimage)
		{
			// 引数の保持
			_orgimage = orgimage;
			_orgIplImage = BitmapConverter.ToMat(orgimage);

			// 変数の初期化
			_detectLinesL = new ShapeLine[0];
			_detectLinesR = new ShapeLine[0];

			// オリジナルイメージ変換
			SetConvImages(_orgIplImage);

			// ライン検出
			if(  GetLines(_binImage, out _detectLinesL, out _detectLinesR )== false )
			{
				_detectLinesL = _detectLinesR = null;
			}
		}


		/// <summary>
		/// オリジナルイメージから変換画像群を作成します
		/// </summary>
		/// <param name="orgimg">オリジナルイメージ</param>
		private void SetConvImages( Mat orgimg )
		{
			if (_grayMat != null) _grayMat.Dispose();
			if( _binImage != null ) _binImage.Dispose();
			_grayMat = new Mat( _orgIplImage.Size(), MatType.CV_8UC1, new Scalar(0));
			Cv2.CvtColor(orgimg, _grayMat, ColorConversionCodes.BGR2GRAY );  //グレイスケール画像に変換
			_binImage = new Mat(_grayMat.Size(), MatType.CV_8UC1, new Scalar(0));
			Cv2.Threshold(_grayMat, _binImage, 0, 255, ThresholdTypes.BinaryInv | ThresholdTypes.Otsu);	// 2値化画像（反転）

			//var bmp = _grayMat.ToBitmap();
			//var frm = new FormColorImageS(bmp, "Gray");
			//frm.ShowDialog();

			//bmp = _binImage.ToBitmap();
			//frm = new FormColorImageS(bmp, "Bin");
			//frm.ShowDialog();
		
		}

		/// <summary>
		/// プロファイルを作成します
		/// </summary>
		/// <param name="bimg">画像</param>
		/// <param name="x">x中心位置</param>
		/// <param name="wid">幅（片側）</param>
		/// <returns>各高さごとのプロファイル(平均)</returns>
		unsafe private double[] CreateProfile( Mat bimg, int x, int wid )
		{
			var  prof = new double[bimg.Height];
			var dataPtr = bimg.DataPointer;
			var step = bimg.Step();
			for (int y = 0; y < prof.Length; y++)
			{
				byte* ptr = dataPtr + step * y + (x - wid) * 1;
				for( int i= 0; i< wid*2+ 1; i++ )	prof[y] += *ptr++;
				prof[y] /= (wid * 2 + 1);			// 平均
			}
			return prof;
		}

	
		/// <summary>ラインを取得します </summary>
		/// <param name="bin_image">検出対象画像（2値化)</param>
		/// <param name="left">検出したライン群（左側）</param>
		/// <param name="right">検出したライン群（右側）</param>
		/// <returns>成否</returns>
		private bool GetLines(Mat bin_image, out ShapeLine[]left, out ShapeLine[]right)
		{

			// 補助判定用プロファイル
			// PEND 取りあえず左側のみ （開始点が幅の1/4以内にあること）
			var ret = false;
			try
			{
				var width = bin_image.Width;
				var x = (int)(width * 2 / 8);		// 左半分のさらに半分（8分割）
				var Lim0 = 100.0;							// 余白外判定。いったんこの値未満なるまではピーク判定しない。
				var prof = CreateProfile(bin_image, x, 50).Select((v, i) => new { Y = i, Val = v }).ToArray();
				var yLen = bin_image.Height;
				int skipT = 0;							// == 含む
				for (int i = 0; i < prof.Length / 3 && prof[i].Val >= Lim0; i++) { skipT = prof[i].Y + 1; }
				int skipE =  bin_image.Height;			// == 含まない
				for (int i = 0; i < prof.Length / 3 && prof[prof.Length - i - 1].Val >= Lim0; i++) { skipE = prof[prof.Length - i - 1].Y; }

				var lst= GetLevelShapeLine(bin_image);
				lst = lst.Where(ln => ln.PosBgn.Y >= skipT && ln.PosBgn.Y < skipE && ln.PosEnd.Y >= skipT && ln.PosEnd.Y < skipE).ToArray(); // 余白外を除外
				left = lst.Where(ln => ln.PosBgn.X < width/  4 ).ToArray();																	// 左側 1/4 未満の場所から開始しているもののみ
				right = lst.Where(ln => ln.PosEnd.X > width- width / 4).ToArray();																	// 右側 1/4 未満の場所で終了しているもののみ
				return true;
			}
			catch
			{
			}
//			return GetLevelShapeLine(bin_image);
			left = right = null;
			return false;


		}

		/// <summary>
		/// 横向きの長い線を取得します
		/// </summary>
		/// <param name="src_img">元になる画像（対象の色が白、そうでない部分が黒で塗られたモノクロ画像）</param>
		/// <returns>カラーブロック群</returns>
		private ShapeLine[] GetLevelShapeLine(Mat src_img)
		{
			int MinXWidth = src_img.Width/100;	// 採用リミット　X偏差1/100pixel以上
			const double MaxRatioXY = 0.20;		// 採用リミット　Xに対するyの偏差の比
			int EdgeHeight = src_img.Height/80;			// 画像エッジの除外ピクセル数（上下ともこのピクセル分は対象から外す）

			// メモリ領域の確保
			var ret = new List<ShapeLine>();

			// 輪郭の検出(戻り値は取得した輪郭の全個数)
//			int find_contour_num = Cv2.FindContours(src_img, storage, out contours, CvContour.SizeOf, ContourRetrieval.List, ContourChain.ApproxSimple);
			var posvList = src_img.FindContoursAsArray(RetrievalModes.List, ContourApproximationModes.ApproxSimple);

			foreach( var posv in posvList )
			{
				//傾いていない外接四角形領域（フィレ径）
				Rect rect = Cv2.BoundingRect(posv);

				// 横方向に一定の大きさを持つものを登録
				if (rect.Width > MinXWidth )
				{
					//輪郭を構成する頂点座標を取得
					var wposv = posv.Select(cp => new SyPoint(cp.X, cp.Y));
					if( wposv.Any() ){
						wposv = wposv.Concat(wposv.Take(1));			// 最初の点を最後に追加
						var wpv = wposv.ToArray();
						for (int i = 0; i < wpv.Length - 1; i++)
						{
							if (Math.Abs(wpv[i].X - wpv[i + 1].X) >= MinXWidth && Math.Abs((double)(wpv[i].Y - wpv[i + 1].Y) / (double)(wpv[i].X - wpv[i + 1].X)) < MaxRatioXY && wpv[i].Y > EdgeHeight-1 && wpv[i].Y < src_img.Height-EdgeHeight)
							{
								ret.Add( (wpv[i].X < wpv[i+1].X) ?  new ShapeLine( wpv[i], wpv[i+1] ): new ShapeLine( wpv[i+1], wpv[i] ));
							}
						}
					}
				}
			}

			return ret.ToArray();
		}



		///// <summary>
		///// ラインを取得します
		///// </summary>
		///// <param name="bin_image">検出対象画像（2値化)</param>
		///// <returns>検出したライン群</returns>
		//private ShapeLine[] GetLines( Mat bin_image )
		//{
		//    var lst = new List<ShapeLine>();
		//    CvMemStorage storage = new CvMemStorage();
		//    CvSeq lines = Cv.HoughLines2( bin_image, storage, HoughLinesMethod.Standard/*.Probabilistic*/, 1, Math.PI / 180, 50, 100, 10); // Probabilistic:確率的Hough変換
		//    for (int i = 0;i < lines.Total;i++) 
		//    {
		//        CvLineSegmentPoint elem = lines.GetSeqElem<CvLineSegmentPoint>(i).Value;
		//        var p0 = new Point( elem.P1.X, elem.P1.Y );
		//        var p1 = new Point( elem.P2.X, elem.P2.Y );
		//        lst.Add( (p0.X <p1.X) ? new ShapeLine(p0, p1): new ShapeLine(p1, p0) );		// 左側を始点とする
		//    }
		//    lines.Dispose();
		//    storage.Dispose();
		//    return lst.ToArray();
		//}


		/// <summary>インスタンスを破棄します(IDisposable)</summary>
		public void Dispose()
		{
			// 内部で作成したイメージ群の破棄
			if (_orgIplImage != null)	{ _orgIplImage.Dispose();	_orgIplImage= null;	}
			if (_grayMat != null)		{ _grayMat.Dispose();		_grayMat = null;	}
			if (_binImage != null)		{ _binImage.Dispose();		_binImage= null;	}

			// その他の変数の破棄
			_detectLinesL = null;
			_detectLinesR = null;
			_orgimage = null;
		}
	}
}
