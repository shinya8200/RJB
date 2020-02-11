using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace RulerJB
{
    /// <summary>
    /// �P�s�N�Z����\���\����
    /// </summary>
    public struct PixelData24
    {
        public byte blue;   // ��
        public byte green;  // ��
        public byte red;    // ��
    }
	public struct PixelData32
	{
		public byte blue;   // ��
		public byte green;  // ��
		public byte red;    // ��
	}


    /// <summary>
    /// �A���Z�[�t�ȃr�b�g�}�b�v�N���X
    /// </summary>
    public class UsBitMap
    {
        protected Bitmap _bitmapdata;
		/// <summary> �����f�[�^�𒼐ڃA�N�Z�X����Ƃ��̃C���[�W�f�[�^�̈�B BeginAccess()�`EndAccess()�ԗL�� </summary>
		protected BitmapData _img;
		/// <summary> �����f�[�^�𒼐ڃA�N�Z�X����Ƃ��̂P�s�N�Z���̃o�C�g�T�C�Y�B BeginAccess()�`EndAccess()�ԗL�� </summary>
		protected int _pixelSize;
		/// <summary> �摜�� </summary>
		protected int _width;
		/// <summary> �摜���� </summary>
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

		/// <summary> �F�}�X�N�i�R�Q�r�b�g��p�j</summary>
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
				_height = _bitmapdata.Height;		// ���� ���̃v���p�e�B�̓I�[�o�w�b�h���傫���i�v���L�V�j
				_width = _bitmapdata.Width;			// �� �@���̃v���p�e�B�̓I�[�o�w�b�h���傫���i�v���L�V�j

            }
        }

		/// <summary> �e�X�g�p���\�b�h </summary>
		public void MakeRedFilter()
		{
			BeginAccess();					// �A�N�Z�X�J�n
			int xmax = _bitmapdata.Width;	// ��
			int ymax = _bitmapdata.Height;	// ����

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
			EndAccess();					// �A�N�Z�X�I��
		}


		//---------------------------------------------
		// ���ڃA�N�Z�X�J�n
		//---------------------------------------------
		public void BeginAccess()
		{
			PixelFormat picf = PixelFormat.Format24bppRgb;			// BGR_  (32��24) 
			_img = _bitmapdata.LockBits(new Rectangle(0, 0, _bitmapdata.Width, _bitmapdata.Height), ImageLockMode.ReadWrite, picf );
			_pixelSize = Image.GetPixelFormatSize(picf)/8;			// �o�C�g�T�C�Y
		}

		/// <summary> �s�N�Z�����̎擾 </summary>
		/// <param name="x">X���W</param>
		/// <param name="y">Y���W</param>
		/// <returns>�w�肵���ꏊ�̐F</returns>
		/// <remarks>PixelFormat.Format24bbpRgb��Format32bppRgb��z�肵�Ă���BBitmap�N���X��GetPixel����</remarks>
		public Color GetPixel(int x, int y)
		{
			unsafe
			{
				byte* adr = (byte*)_img.Scan0;
				//�A�h���X�v�Z
				int pos = x * _pixelSize + _img.Stride * y;
				byte b = adr[pos + 0];
				byte g = adr[pos + 1];
				byte r = adr[pos + 2];
				return Color.FromArgb(r, g, b);
			}
		}

		/// <summary> �s�N�Z�����̃Z�b�g </summary>
		/// <param name="x">X���W</param>
		/// <param name="y">Y���W</param>
		/// <remarks>PixelFormat.Format24bbpRgb��Format32bppRgb��z�肵�Ă���BBitmap�N���X��SetPixel����</remarks>
		public void SetPixel(int x, int y, Color col)
		{
			if (_img == null) return;
			unsafe
			{
				byte* adr = (byte*)_img.Scan0;
				//�A�h���X�v�Z
				int pos = x * _pixelSize + _img.Stride * y;
				adr[pos + 0] = col.B;
				adr[pos + 1] = col.G;
				adr[pos + 2] = col.R;
			}
		}

		/// <summary> �s�N�Z�����̃Z�b�g </summary>
		/// <param name="x">X���W</param>
		/// <param name="y">Y���W</param>
		/// <param name="r">��</param>
		/// <param name="g">��</param>
		/// <param name="b">��</param>
		/// <remarks>PixelFormat.Format24bbpRgb��Format32bppRgb��z�肵�Ă���BBitmap�N���X��SetPixel����</remarks>
		public void SetPixel(int x, int y, byte r, byte g, byte b)
		{
			if (_img == null) return;
			unsafe
			{
				byte* adr = (byte*)_img.Scan0;
				//�A�h���X�v�Z
				int pos = x * _pixelSize + _img.Stride * y;
				adr[pos + 0] = b;
				adr[pos + 1] = g;
				adr[pos + 2] = r;
			}
		}
		/// <summary> Scan0(Bitmap���̈��Ԃ� </summary>
		/// <returns>Bitmap���̈�</returns>
		/// <remarks>BeginAccess()�`EndAccess()�ԂɎg�p���܂��B</remarks>
		unsafe public byte* GetScan0()
		{
			if (_img == null) return (byte*)0;
			return (byte*)_img.Scan0;
		}


		/// <summary> Img(BitmapData) �f�[�^��Ԃ��܂��B </summary>
		/// <returns>BitmapData ��Ԃ�</returns>
		/// <remarks>BeginAccess()�`EndAccess()�ԂɎg�p���܂��B</remarks>
		public BitmapData GetImg()
		{
			return _img;
		}


		/// <summary> �摜���i�s�N�Z���j�������܂�</summary>
		public int Width
		{
			get { return _width; }
		}


		/// <summary> �摜�����i�s�N�Z���j�������܂�</summary>
		public int Height
		{
			get { return _height; }
		}

		/// <summary>
		/// �P�s�N�Z���̃o�C�g�����擾���܂��BBeginAccess()�`EndAccess()�ԗL��
		/// </summary>
		public int PixelSize
		{
			get { return _pixelSize; }
		}
		/// <summary> �s�N�Z�����̎擾 </summary>
		/// <param name="x">X���W</param>
		/// <param name="y">Y���W</param>
		/// <returns>�w�肵���ꏊ�̐F</returns>
		/// <remarks>Format32bppRgb��z�肵�Ă���B</remarks>
		public UInt32 GetPixel32(int x, int y)
		{
			unsafe
			{
				UInt32* adr = (UInt32*)_img.Scan0;
				//�A�h���X�v�Z
				int pos = x  + _width * y;
				return adr[pos];
			}
		}

		/// <summary> �s�N�Z�����̃Z�b�g </summary>
		/// <param name="x">X���W</param>
		/// <param name="y">Y���W</param>
		/// <param name="col">�F���32�r�b�g�f�[�^</param>
		/// <remarks>Format32bppRgb��z�肵�Ă���B</remarks>
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
		// ���ڃA�N�Z�X�I��
		//---------------------------------------------
		public void EndAccess()
		{
			_bitmapdata.UnlockBits(_img);
			_img = null;
		}

    }
}
