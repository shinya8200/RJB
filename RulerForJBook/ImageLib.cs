using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace RulerJB
{
	/// <summary>
	/// ライブラリクラスです
	/// </summary>
	public class ImageLib
	{
		/// <summary>色相(Hue)表現は0-HueMax【度】で表すこととします</summary>
		public const int HueMax = 360;
		/// <summary>OpenCVにおけるHSVモードにおける色相は0-180</summary>
		public const int HueMaxCv = 180;


		/// <summary>HSVをRGBに変換します</summary>
		/// <param name="hh">色相(0-360)</param>
		/// <param name="s">彩度(0-255)</param>
		/// <param name="v">明度(0-255)</param>
		static public void RgbFromHsv(int h, int s, int v, out int r, out int g, out int b)
		{
			h %= 360;
			int Hi = (int)Math.Floor((double)h / 60) % 6;

			float f = ((float)h / 60) - Hi;
			float p = ((float)v / 255) * (1 - ((float)s / 255));
			float q = ((float)v / 255) * (1 - f * ((float)s / 255));
			float t = ((float)v / 255) * (1 - (1 - f) * ((float)s / 255));
			p *= 255;
			q *= 255;
			t *= 255;

			r = g = b = 0;
			switch (Hi)
			{
				case 0: r= v;		g= (int)t;		b= (int)p;		break;
				case 1: r=(int)q;	g= v;			b= (int)p;		break;
				case 2: r=(int)p;	g= v;			b= (int)t;		break;
				case 3: r=(int)p;	g= (int)q;		b= v;			break;
				case 4: r=(int)t;	g= (int)p;		b= v;			break;
				case 5: r= v;		g= (int)p;		b= (int)q;		break;
			}
		}


		/// <summary>RGBをHSVに変換します</summary>
		/// <param name="r">R成分(0-255)</param>
		/// <param name="g">G成分(0-255)</param>
		/// <param name="b">B成分(0-255)</param>
		/// <param name="h">H成分(0-359)</param>
		/// <param name="s">S成分(0-255)</param>
		/// <param name="v">V成分(0-255)</param>
		static public void HsvFromRgb(int r, int g, int b, out int h, out int s, out int v)
        {
            float rr = r / 255.0f;
            float gr = g / 255.0f;
            float br = b / 255.0f;

            var list = new float[] { rr, gr, br };
            var max = list.Max();
            var min = list.Min();

            float hh, sr, vr;
            if (max == min)         hh = 0;
            else if (max == rr)     hh = (60 * (gr - br) / (max - min) + 360) % 360;
            else if (max == gr)     hh = 60 * (br - rr) / (max - min) + 120;
			else					hh = 60 * (rr - gr) / (max - min) + 240;

            sr= (max == 0)? 0:(max - min) / max;
            vr = max;

			h = (int)hh;
			s = (int)(sr * 255.0 + 0.5);
			v = (int)(vr * 255.0 + 0.5);
        }


		/// <summary>ピクセルフォーマットを指定したビットマップ生成を行います</summary>
		/// <param name="orgImage">元ビットマップ</param>
		/// <param name="pformat">ピクセルフォーマット</param>
		/// <returns>生成したビットマップ</returns>
		/// <remarks>Bitmapクローン、コンストラクタにはそれぞれ問題があるため作成</remarks>
		static public Bitmap CloneBitmapPf(Bitmap orgImage, PixelFormat pformat)
		{
			try
			{
				Bitmap clone = new Bitmap(orgImage.Width, orgImage.Height, pformat);
				using (Graphics gr = Graphics.FromImage(clone))
				{
					gr.DrawImage(orgImage, new Rectangle(0, 0, clone.Width, clone.Height));
				}
				return clone;
				//return orgImage.Clone(new Rectangle(0, 0, orgImage.Width, orgImage.Height), pformat);
			}
			catch (Exception ex)
			{
				return null;
			}
		}

	}
}
