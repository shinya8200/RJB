using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace RulerJB
{
    /// <summary>
    /// １ピクセルを表す構造体
    /// </summary>
    public struct PixelData24
    {
        public byte blue;   // 青
        public byte green;  // 緑
        public byte red;    // 赤
    }
	public struct PixelData32
	{
		public byte blue;   // 青
		public byte green;  // 緑
		public byte red;    // 赤
	}


    /// <summary>
    /// アンセーフなビットマップクラス
    /// </summary>
    public class UsBitMap
    {
        protected Bitmap _bitmapdata;
		/// <summary> 内部データを直接アクセスするときのイメージデータ領域。 BeginAccess()～EndAccess()間有効 </summary>
		protected BitmapData _img;
		/// <summary> 内部データを直接アクセスするときの１ピクセルのバイトサイズ。 BeginAccess()～EndAccess()間有効 </summary>
		protected int _pixelSize;
		/// <summary> 画像幅 </summary>
		protected int _width;
		/// <summary> 画像高さ </summary>
		protected int _height;

        public UsBitMap(string fn)
        {
            SetBitmap(new Bitmap(fn));
        }

		public UsBitMap(Bitmap bm)
		{
			SetBitmap(bm);
		}

        public Bitmap BitmapData
        {
            get { return _bitmapdata; }
        }

		/// <summary> 色マスク（３２ビット専用）</summary>
		[Flags]
		public enum ColorMask32:uint
		{
			RED   = 0x00ff0000,
			GREEN = 0x0000ff00,
			BLUE  = 0x000000ff,
			COLOR = RED | GREEN | BLUE
		}

        // -------------- private ----------------
        
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bdata"></param>
		protected void SetBitmap( Bitmap bdata )
        {
            // _bitmapdata = new Bitmap(bdata);
            _bitmapdata = (Bitmap)bdata.Clone();   // 2013.10.07
            if (_bitmapdata != null)
            {
				_height = _bitmapdata.Height;		// 高さ このプロパティはオーバヘッドが大きい（プロキシ）
				_width = _bitmapdata.Width;			// 幅 　このプロパティはオーバヘッドが大きい（プロキシ）

            }
        }

		/// <summary> テスト用メソッド </summary>
		public void MakeRedFilter()
		{
			BeginAccess();					// アクセス開始
			int xmax = _bitmapdata.Width;	// 幅
			int ymax = _bitmapdata.Height;	// 高さ

			for (int y = 0; y < ymax; y++)
			{
				for (int x = 0; x < xmax; x++)
				{
					//Color col = GetPixel(x, y);
					//SetPixel(x, y, Color.FromArgb(255, col.G, col.B));
					UInt32 col = GetPixel32(x, y);
					SetPixel32(x, y, (UInt32)((col & ~(UInt32)ColorMask32.RED) | (UInt32)ColorMask32.RED));

				}
			}
			EndAccess();					// アクセス終了
		}


		//---------------------------------------------
		// 直接アクセス開始
		//---------------------------------------------
		public void BeginAccess()
		{
			PixelFormat picf = PixelFormat.Format24bppRgb;			// BGR_  (32→24) 
			_img = _bitmapdata.LockBits(new Rectangle(0, 0, _bitmapdata.Width, _bitmapdata.Height), ImageLockMode.ReadWrite, picf );
			_pixelSize = Image.GetPixelFormatSize(picf)/8;			// バイトサイズ
		}

		/// <summary> ピクセル情報の取得 </summary>
		/// <param name="x">X座標</param>
		/// <param name="y">Y座標</param>
		/// <returns>指定した場所の色</returns>
		/// <remarks>PixelFormat.Format24bbpRgbかFormat32bppRgbを想定している。BitmapクラスのGetPixel相当</remarks>
		public Color GetPixel(int x, int y)
		{
			unsafe
			{
				byte* adr = (byte*)_img.Scan0;
				//アドレス計算
				int pos = x * _pixelSize + _img.Stride * y;
				byte b = adr[pos + 0];
				byte g = adr[pos + 1];
				byte r = adr[pos + 2];
				return Color.FromArgb(r, g, b);
			}
		}

		/// <summary> ピクセル情報のセット </summary>
		/// <param name="x">X座標</param>
		/// <param name="y">Y座標</param>
		/// <remarks>PixelFormat.Format24bbpRgbかFormat32bppRgbを想定している。BitmapクラスのSetPixel相当</remarks>
		public void SetPixel(int x, int y, Color col)
		{
			if (_img == null) return;
			unsafe
			{
				byte* adr = (byte*)_img.Scan0;
				//アドレス計算
				int pos = x * _pixelSize + _img.Stride * y;
				adr[pos + 0] = col.B;
				adr[pos + 1] = col.G;
				adr[pos + 2] = col.R;
			}
		}

		/// <summary> ピクセル情報のセット </summary>
		/// <param name="x">X座標</param>
		/// <param name="y">Y座標</param>
		/// <param name="r">赤</param>
		/// <param name="g">緑</param>
		/// <param name="b">青</param>
		/// <remarks>PixelFormat.Format24bbpRgbかFormat32bppRgbを想定している。BitmapクラスのSetPixel相当</remarks>
		public void SetPixel(int x, int y, byte r, byte g, byte b)
		{
			if (_img == null) return;
			unsafe
			{
				byte* adr = (byte*)_img.Scan0;
				//アドレス計算
				int pos = x * _pixelSize + _img.Stride * y;
				adr[pos + 0] = b;
				adr[pos + 1] = g;
				adr[pos + 2] = r;
			}
		}
		/// <summary> Scan0(Bitmap情報領域を返す </summary>
		/// <returns>Bitmap情報領域</returns>
		/// <remarks>BeginAccess()～EndAccess()間に使用します。</remarks>
		unsafe public byte* GetScan0()
		{
			if (_img == null) return (byte*)0;
			return (byte*)_img.Scan0;
		}


		/// <summary> Img(BitmapData) データを返します。 </summary>
		/// <returns>BitmapData を返す</returns>
		/// <remarks>BeginAccess()～EndAccess()間に使用します。</remarks>
		public BitmapData GetImg()
		{
			return _img;
		}


		/// <summary> 画像幅（ピクセル）を示します</summary>
		public int Width
		{
			get { return _width; }
		}


		/// <summary> 画像高さ（ピクセル）を示します</summary>
		public int Height
		{
			get { return _height; }
		}

		/// <summary>
		/// １ピクセルのバイト数を取得します。BeginAccess()～EndAccess()間有効
		/// </summary>
		public int PixelSize
		{
			get { return _pixelSize; }
		}
		/// <summary> ピクセル情報の取得 </summary>
		/// <param name="x">X座標</param>
		/// <param name="y">Y座標</param>
		/// <returns>指定した場所の色</returns>
		/// <remarks>Format32bppRgbを想定している。</remarks>
		public UInt32 GetPixel32(int x, int y)
		{
			unsafe
			{
				UInt32* adr = (UInt32*)_img.Scan0;
				//アドレス計算
				int pos = x  + _width * y;
				return adr[pos];
			}
		}

		/// <summary> ピクセル情報のセット </summary>
		/// <param name="x">X座標</param>
		/// <param name="y">Y座標</param>
		/// <param name="col">色情報32ビットデータ</param>
		/// <remarks>Format32bppRgbを想定している。</remarks>
		public void SetPixel32(int x, int y, UInt32 col)
		{
			if (_img == null) return;
			unsafe
			{
				UInt32* adr = (UInt32*)_img.Scan0;
				int pos = x + _width * y;
				adr[pos] = col;
			}
		}

		//---------------------------------------------
		// 直接アクセス終了
		//---------------------------------------------
		public void EndAccess()
		{
			_bitmapdata.UnlockBits(_img);
			_img = null;
		}

    }
}
