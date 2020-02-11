using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;

using OpenCvSharp;
using OpenCvSharp.Extensions;

using DevelopLogSystem;

using SyPoint = System.Drawing.Point;
using CvPoint = OpenCvSharp.Point;
using CvSize = OpenCvSharp.Size;

namespace RulerJB
{
	/// <summary>
	/// ROIを使用した解析クラスです。
	/// </summary>
	class DetectTypeROI:IDetectProc
	{
		private Mat _orgImage;

		private string _forDebugPath = null;
		const string DebugFolder = "RJB_Debug";

		private bool _isBinMode = false;
		
		// 判別に使用するRGBの何れかを指定
		private int _useColor;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public DetectTypeROI()
		{
			// デスクトップにRJB_Debugフォルダがある場合にはデバッグ画像出力
			var wstr =  Path.Combine( System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), DebugFolder );
			_forDebugPath = Directory.Exists( wstr ) ? wstr: null;
			_useColor = 0;
		}

		/// <summary>
		/// 検出を行います。
		/// </summary>
		/// <param name="img">対象画像</param>
		/// <param name="pset">設定値</param>
		/// <param name="sideL">左側ページ情報</param>
		/// <param name="sideR">右側ページ情報</param>
		/// <param name="pars">汎用パラメータ</param>
		/// <returns>成否</returns>
		public bool Exec( Bitmap img, BookProjectSetData pset, PageMeasureData sideL, PageMeasureData sideR, Dictionary<string, object> pars)
		{

			if (pset == null) return false;

			if (pars != null)
			{
				if (pars.ContainsKey("BinMode") == true && pars["BinMode"] is bool) _isBinMode = (bool)pars["BinMode"];
			}

			_orgImage = BitmapConverter.ToMat( img );

			//			Cv2.Smooth(_orgImage, _orgImage, SmoothType.Gaussian, 131, 5);
			//			Cv.Smooth(_orgImage, _orgImage, SmoothType.Gaussian, 1, 5);

			//Cv2.GaussianBlur(_orgImage, _orgImage, new CvSize(131, 131), 5.0);		// 要引数確認。カーネルサイズ大きすぎ？　ハードコード対応

			// 中央値ブラー
			//Cv2.MedianBlur(_orgImage, _orgImage, ksize: 9);

			// バイラテラルフィルタ
			//var blurimage = new Mat(_orgImage.Width, _orgImage.Height, _orgImage.Type(), new Scalar(0));
			//Cv2.BilateralFilter(_orgImage, blurimage, 9, 75, 75);
			//_orgImage.Dispose();
			//_orgImage = blurimage;

			var pT = DetectHolLine(_orgImage, pset.RoiPTL, true, true, true, "RoiPTL");
			var pB = DetectHolLine(_orgImage, pset.RoiPBL, false, true, true, "RoiPBL");
			if (pT != null && pB != null)
			{
				sideL.MeasurePaper = new ShapeLineEx((SyPoint)pT, (SyPoint)pB);
				sideL.HeightPixPaper = (int)sideL.MeasurePaper.GetDistance();
			}
			pT = DetectHolLine(_orgImage, pset.RoiPTR, true, false, true, "RoiPTR");
			pB = DetectHolLine(_orgImage, pset.RoiPBR, false, false, true, "RoiPBR");
			if (pT != null && pB != null)
			{
				sideR.MeasurePaper = new ShapeLineEx((SyPoint)pT, (SyPoint)pB);
				sideR.HeightPixPaper = (int)sideR.MeasurePaper.GetDistance();
			}

			pT = DetectHolLine(_orgImage, pset.RoiGTL, true, true, false, "RoiGTL");
			pB = DetectHolLine(_orgImage, pset.RoiGBL, false, true, false, "RoiGBL");
			if (pT != null && pB != null)
			{
				sideL.MeasureGrid = new ShapeLineEx((SyPoint)pT, (SyPoint)pB, sideL.MeasurePaper);
				sideL.HeightPixGrid = (int)sideL.MeasureGrid.GetDistance();
			}
			pT = DetectHolLine(_orgImage, pset.RoiGTR, true, false, false, "RoiGTR");
			pB = DetectHolLine(_orgImage, pset.RoiGBR, false, false, false, "RoiGBR");
			if (pT != null && pB != null)
			{
				sideR.MeasureGrid = new ShapeLineEx((SyPoint)pT, (SyPoint)pB, sideR.MeasurePaper);
				sideR.HeightPixGrid = (int)sideR.MeasureGrid.GetDistance();
			}
			if (sideL.MeasurePaper != null)
			{
				sideL.MeasurePaper._link = sideL.MeasureGrid;
			}
			if (sideR.MeasurePaper != null)
			{
				sideR.MeasurePaper._link = sideR.MeasureGrid;
			}

			_orgImage.Dispose();

#if false
			// グレーイメージ作成
			var grayImage = new Mat(_orgImage.Width, _orgImage.Height, BitDepth.U8, 1);
			_orgImage.CvtColor( grayImage, ColorConversion.BgrToGray );	 

			var sobleImage = new Mat(_orgImage.Width, _orgImage.Height, BitDepth.U8, 1);
			var cannyImage = new Mat(_orgImage.Width, _orgImage.Height, BitDepth.U8, 1);

			Cv.Sobel(grayImage, sobleImage, 0, 1, ApertureSize.Size1);
			Cv.Canny(grayImage, cannyImage, 50, 200, ApertureSize.Size3);

			grayImage.SaveImage( @"E:\gray.png" );
			sobleImage.SaveImage(@"E:\sobel.png");
			cannyImage.SaveImage(@"E:\canny.png");

			// grayのライン検出(Debug)
			var smg = grayImage.Clone();
			// 画像の二値化【判別分析法(大津の二値化)】
			Cv.Threshold(grayImage, smg, 0, 255, ThresholdType.Binary | ThresholdType.Otsu );
			var linList = GetLines(smg);
			var bgr = new Mat(grayImage.Size, BitDepth.U8, 3);
			grayImage.CvtColor(bgr, ColorConversion.GrayToBgr);
			var wimg = bgr.ToBitmap(PixelFormat.Format24bppRgb);
			var gra = Graphics.FromImage(wimg);
			var pen = new Pen( Color.Red, 3 );
			//foreach (var lin in linList)
			//{
			//    gra.DrawLine(pen, lin[0], lin[1]);
			//}
			//wimg.Save(@"E:\gray_l.png", ImageFormat.Png);
			smg.Dispose();
			gra.Dispose();
			wimg.Dispose();

			// sobleのライン検出(Debug)
			smg = sobleImage.Clone();
//			Cv.EqualizeHist(smg, smg);
			// 画像の二値化【判別分析法(大津の二値化)】
//			Cv.Threshold(sobleImage, smg, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);
			linList = GetLines(smg);
			bgr = new Mat(sobleImage.Size, BitDepth.U8, 3);
			sobleImage.CvtColor(bgr, ColorConversion.GrayToBgr);
			wimg = bgr.ToBitmap(PixelFormat.Format24bppRgb);
			gra = Graphics.FromImage(wimg);
			pen = new Pen(Color.Red, 3);
			foreach (var lin in linList)
			{
				gra.DrawLine(pen, lin[0], lin[1]);
			}
			wimg.Save(@"E:\sobel_l.png", ImageFormat.Png);
			smg.Dispose();
			gra.Dispose();
			wimg.Dispose();


			// Cannyのライン検出(Debug)
			smg = cannyImage.Clone();
			Cv.EqualizeHist(smg, smg);
			// 画像の二値化【判別分析法(大津の二値化)】
//			Cv.Threshold(cannyImage, smg, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);
			linList = GetLines(smg);
			bgr = new Mat(cannyImage.Size, BitDepth.U8, 3);
			cannyImage.CvtColor(bgr, ColorConversion.GrayToBgr);
			wimg = bgr.ToBitmap(PixelFormat.Format24bppRgb);
			gra = Graphics.FromImage(wimg);
			pen = new Pen(Color.Red, 3);
			foreach (var lin in linList)
			{
				gra.DrawLine(pen, lin[0], lin[1]);
			}
			wimg.Save(@"E:\canny_l.png", ImageFormat.Png);
			smg.Dispose();
			gra.Dispose();
			wimg.Dispose();


			grayImage.Dispose();
			sobleImage.Dispose();
			_orgImage.Dispose();
#endif

			return true;
		}


		/// <summary>
		/// デバッグ用画像出力（ライン）
		/// </summary>
		/// <param name="img">画像(U8,1)</param>
		/// <param name="lines">ライン群</param>
		/// <param name="preStr">ファイル名につける文字列</param>
		private void DebugLineImage(Mat img, List<SyPoint[]> lines, string preStr)
		{
			if (_forDebugPath == null) return;
			try
			{
				img.SaveImage(Path.Combine(_forDebugPath, preStr + @"_Bin.png"));
				var bgr = img.CvtColor( ColorConversionCodes.GRAY2BGR);
				var wimg = bgr.ToBitmap(PixelFormat.Format24bppRgb);
				var gra = Graphics.FromImage(wimg);
				var pens = new Pen[] { new Pen(Color.Red, 3), new Pen(Color.Yellow, 3), new Pen(Color.Green, 3), new Pen(Color.DarkBlue) };
				for (int i = 0; i < lines.Count; i++)
				{
					var lin = lines[i];
					var pen = pens[i % pens.Length];
					pen.DashStyle = ((i / pens.Length) % 2 == 0) ? System.Drawing.Drawing2D.DashStyle.Solid : System.Drawing.Drawing2D.DashStyle.Dot;
					gra.DrawLine(pen, lin[0], lin[1]);
				}
				wimg.Save(Path.Combine(_forDebugPath, preStr + @"_Line.png"), ImageFormat.Png);
				gra.Dispose();
				wimg.Dispose();
			}
			catch
			{
			}
		}

		/// <summary>入力した配列の中央値を返します</summary>
		/// <param name="mat">配列データ</param>
		/// <returns>中央値</returns>
		public double GetMedian(Mat mat)
		{
			byte[] data = new byte[mat.Width * mat.Height];
			int count = mat.Width * mat.Height;
			mat.GetArray(0, 0, data);

			Array.Sort(data);

			double median;
			int pos = count / 2;

			if (count % 2 == 0)
			{
				median = (data[pos] + data[pos + 1]) / 2;
			}
			else
			{
				median = data[pos];
			}
			return median;
		}

		/// <summary>
		/// 平行線の検出を行います
		/// </summary>
		/// <param name="imageOrg">オリジナルイメージ</param>
		/// <param name="roi">対象ROI</param>
		/// <param name="isTop">上側</param>
		/// <param name="isLeft">左側</param>
		/// <param name="isPaper">紙高用</param>
		/// <param name="degStr">デバッグ時文字列</param>
		/// <returns>ポイント（エラー null）</returns>
		public SyPoint? DetectHolLine(Mat imageOrg, BookROI roi, bool isTop, bool isLeft, bool isPaper, string debStr )
		{
			SyPoint? ret = null;
			// ROIを設定
			var image= new Mat( imageOrg, new Rect(roi.BasePoint.X, roi.BasePoint.Y, roi.RoiSize.Width, roi.RoiSize.Height));
			try
			{
				// ROIのディープコピー作成
				var wimage = new Mat(roi.RoiSize.Width, roi.RoiSize.Height, image.Type(), new Scalar(0));
				image.CopyTo(wimage);

				Mat[] channels;
				const int judgLine = 3; // ROIの上３行、下３行を検査して判定する
				Cv2.Split(wimage, out channels);
				if(isTop && isLeft && isPaper)
				{
					double diff = 0;
					for (int i = 0; i < 3; i++) 
					{
						var upper = new Mat(channels[i], new Rect(0, 0, roi.RoiSize.Width, judgLine));
						var bottom = new Mat(channels[i], new Rect(0, roi.RoiSize.Height - judgLine, roi.RoiSize.Width, 3));
						double tempDiff = Math.Abs(GetMedian(upper) - GetMedian(bottom));
						if (diff < tempDiff)
						{
							diff = tempDiff;
							_useColor = i;
						}
					}
				}

				var grayImage = new Mat(roi.RoiSize.Width, roi.RoiSize.Height, MatType.CV_8UC1, new Scalar(0));

				// バイラテラルフィルタ
				Cv2.BilateralFilter(channels[_useColor], grayImage, 9, 75, 75);

				// グレーイメージ作成
				//var grayImage = wimage.CvtColor( ColorConversionCodes.BGR2GRAY );

				if (_isBinMode)
				{
					// 画像の二値化【判別分析法(大津の二値化)】
					Cv2.Threshold(grayImage, grayImage, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
				}


				var sobleImage = new Mat(wimage.Width, wimage.Height, MatType.CV_8UC1, new Scalar(0));
				var cannyImage = new Mat(wimage.Width, wimage.Height, MatType.CV_8UC1, new Scalar(0));

				Cv2.Sobel(grayImage, sobleImage, MatType.CV_8UC1, 0, 1, 1);
				Cv2.Canny(grayImage, cannyImage, 300, 500, 5);

				if (debStr != null && _forDebugPath != null)
				{
					grayImage.SaveImage(Path.Combine(_forDebugPath, debStr + "_GrayImage.png"));
					sobleImage.SaveImage(Path.Combine(_forDebugPath, debStr + "_SobleImage.png"));
					cannyImage.SaveImage(Path.Combine(_forDebugPath, debStr + "_CannyImage.png"));
				}


				// sobleのライン検出(Debug)
				var smg = sobleImage.Clone();
				// 画像の二値化【判別分析法(大津の二値化)】
				Cv2.Threshold(sobleImage, smg, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
				var linListSobel = GetLines(smg);
				if (debStr != null) DebugLineImage(sobleImage, linListSobel, debStr + "_Soble");
				smg.Dispose();


				// Cannyのライン検出(Debug)
				smg = cannyImage.Clone();
				// 画像の二値化【判別分析法(大津の二値化)】
				Cv2.Threshold(cannyImage, smg, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
				var linListCanny = GetLines(cannyImage);
				if (debStr != null) DebugLineImage(cannyImage, linListSobel, debStr + "_Canny");
				smg.Dispose();


				// 傾きが 1/5未満のすべての線を集める。
				var wlin = true ? linListSobel.Concat(linListCanny) : linListCanny;
				var linAll = wlin.Where(ln => ln[0].X != ln[1].X && Math.Abs(ln[0].Y - ln[1].Y) * 5 < Math.Abs(ln[0].X - ln[1].X));
				// 座標変換する。
				if( isLeft== false ) linAll = linAll.Select(ln=>new SyPoint[]{ new SyPoint(roi.RoiSize.Width-1- ln[0].X, ln[0].Y), new SyPoint(roi.RoiSize.Width-1-ln[1].X, ln[1].Y) });
				if (isTop == false) linAll = linAll.Select(ln => new SyPoint[] { new SyPoint(ln[0].X, roi.RoiSize.Height - 1 - ln[0].Y), new SyPoint(ln[1].X, roi.RoiSize.Height - 1 -ln[1].Y) });

				var voteTable = new double[roi.RoiSize.Height];		// 高さ分
				// Ｙ位置と重み（長さ／（外挿比率：外挿距離／長さ））
				var post = linAll.Select(ln => new { YPos = (int)((ln[1].X * ln[0].Y - ln[0].X * ln[1].Y) / (ln[1].X - ln[0].X) + 0.5), Rate = (ln[0].X - ln[1].X+1) * (ln[0].X - ln[1].X+1) / (double)(((ln[0].X < ln[1].X) ? ln[0].X : ln[1].X) + 1),Lines=ln });

				// for Debug.
				if (debStr != null && _forDebugPath!= null )
				{
					using (StreamWriter sw = new StreamWriter(Path.Combine(_forDebugPath, debStr+@"_vote.csv"), false, Encoding.GetEncoding("Shift-JIS")))
					{
						sw.WriteLine("Y軸, 重み, x0, y0, x1, y1");
						foreach (var r in post)
						{
							var line = String.Join(",", new object[] { r.YPos, r.Rate, r.Lines[0].X, r.Lines[0].Y, r.Lines[1].X, r.Lines[1].Y });
							sw.WriteLine(line);
						}
						sw.Close();
					}
				}

				// ｙ方向範囲に収まっているもので投票
				foreach( var wl in post.Where(ww=>ww.YPos>= 0 && ww.YPos < voteTable.Length ) )
				{
					voteTable[wl.YPos] += wl.Rate;
				}
				// 移動平均（どうせ正規化するので足し合わせのみ）voteTable[]書きつぶし
				var wid = 4; // 前後に±4
				var wtbl = Enumerable.Repeat<double>(voteTable.First(), wid).Concat(voteTable).Concat(Enumerable.Repeat<double>(voteTable.Last(), wid)).ToArray();
				voteTable[0] = wtbl.Take(wid * 2 + 1).Sum();
				for (int i = 1; i < voteTable.Length; i++)
				{
					voteTable[i] = voteTable[i - 1] - wtbl[i - 1] + wtbl[i + wid * 2];
				}

				// 正規化
				var peakVal = voteTable.Max();
				if (peakVal > 0)
				{
					voteTable = voteTable.Select(va => va * 100.0 / peakVal).ToArray();
				}
				// for Debug.
				if (debStr != null && _forDebugPath != null)
				{
					using (StreamWriter sw = new StreamWriter(Path.Combine(_forDebugPath, debStr + @"_voteStd.csv"), false, Encoding.GetEncoding("Shift-JIS")))
					{
						sw.WriteLine("Y軸offs, 値");
						foreach (var r in voteTable.Select((v, i) => new { YPos = i, Val = v }))
						{
							sw.WriteLine(String.Join(",", new object[] { r.YPos, r.Val }));
						}
						sw.Close();
					}
				}

				// 最初のピークをエッジと判定。初めて50を超えた点とする
				var lmtv= 50.0;
				var pk = voteTable.Select((v, i) => new { Val = v, Idx = i }).FirstOrDefault(w => w.Val > lmtv);
				if (pk == null) return null;


				// 調整
				// エッジライン作成 (最終調整用。横５ドットの平均輝度）中央部５ドットを使用する。
				var edgeLine = new byte[roi.RoiSize.Height];
				unsafe
				{
					var wx = roi.RoiSize.Width/2 - 5/2;
					var wy = isTop ? 0 : roi.RoiSize.Height - 1;
					var yo = isTop ? 1 : -1;
					var dataPtr = grayImage.DataPointer;
					var step = grayImage.Step();
					for (int i = 0; i < edgeLine.Length; i++)
					{
						int sum = 0;
						for (int j = 0; j < 5; j++)
						{
							sum += dataPtr[step * (wy + yo * i) + wx + j];
						}
						edgeLine[i] = (byte)(sum / 5);
					}
				}
				// 調整は紙の場合と匡郭で異なる
				var newIdx = pk.Idx;
				if (isPaper)
				{
					for (int i = 0; i < 10 && (edgeLine[newIdx] > 128 && newIdx > 0); i++) newIdx--;
					for (int i = 0; i < 10 & newIdx < roi.RoiSize.Height - 1 && edgeLine[newIdx] < 128; i++) newIdx++;
					if (newIdx <= 0 || edgeLine[newIdx] < 128 || edgeLine[newIdx - 1] >= 128) newIdx = pk.Idx;  //　エッジ発見できなかったので、元に戻す。
				}
				else
				{
					for (int i = 0; i < 10 && (edgeLine[newIdx] < 128 && newIdx > 0); i++) newIdx--;
					for (int i = 0; i < 10 & newIdx < roi.RoiSize.Height - 1 && edgeLine[newIdx] >= 128; i++) newIdx++;
					if (newIdx <= 0 || edgeLine[newIdx] >= 128 || edgeLine[newIdx - 1] < 128) newIdx = pk.Idx;  //　エッジ発見できなかったので、元に戻す。
				}

				// ２値化モードではこれですべて決定してしまう。
				if (_isBinMode)
				{
					for ( newIdx = 1; newIdx < edgeLine.Length && edgeLine[newIdx - 1] < 128 == edgeLine[newIdx] < 128; newIdx++) {}
					if (newIdx == edgeLine.Length) newIdx = pk.Idx;
				}

				var vx = roi.BasePoint.X + roi.RoiSize.Width/2;				// Ｘ中央を使用
				var vy = isTop ? (roi.BasePoint.Y + newIdx):(roi.BasePoint.Y+roi.RoiSize.Height- newIdx -1);


				ret = new SyPoint(vx, vy);

				grayImage.Dispose();
				sobleImage.Dispose();
				wimage.Dispose();
			}
			catch (Exception ex)
			{
				DevelopLog.LogException(System.Reflection.MethodBase.GetCurrentMethod(), "例外発生", ex);
				ret = null;
			}
			finally
			{
				image.Dispose();
			}
			return ret;
		}



		/// <summary>
		/// ラインを取得します
		/// </summary>
		/// <param name="src_image"></param>
		/// <returns></returns>
		private List<SyPoint[]> GetLines(Mat src_image)
		{
			var lst = new List<SyPoint[]>();
			var lines = Cv2.HoughLinesP(src_image, 1, Math.PI / 360, 50, 20, 10); //.Ho.HoughLines2(src_image, storage, HoughLinesMethod.Probabilistic, 1, Math.PI / 360, 50, 20, 10); // Probabilistic:確率的Hough変換
			foreach (var elem in lines)
			{
				lst.Add(new SyPoint[] { new SyPoint(elem.P1.X, elem.P1.Y), new SyPoint(elem.P2.X, elem.P2.Y) });
			}
			return lst;
		}

		/// <summary>
		/// インスタンスを破棄します
		/// </summary>
		public void Dispose()
		{
		}
	}
}
