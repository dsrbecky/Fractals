using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Fractals
{
	public class Rendrer
	{
		TempData data;
		int width;
		int height;

		public Rendrer(TempData _data, int _width,int _height)
		{
            data = _data;
			width = _width;
			height = _height;
		}

		unsafe public Bitmap GetMipmap(int depth)
		{
			Bitmap bmp = new Bitmap(32,32,PixelFormat.Format32bppArgb);
			BitmapData bmpData = bmp.LockBits(new Rectangle(0,0,32,32),ImageLockMode.WriteOnly,PixelFormat.Format32bppArgb);
			UInt32* ptr = (UInt32*) bmpData.Scan0.ToPointer();
			for(int y = 0;y < 32;y += 1)
			{
				for(int x = 0;x < 32;x += 4)
				{
					uint color4;
					color4 = data.root.data[(x + Fragment.FragmentSize*y)/4];
					*ptr = 0xFF000000 + (color4 & 0xFF000000) / 0x01000000;ptr++;
					*ptr = 0xFF000000 + (color4 & 0x00FF0000) / 0x00010000;ptr++;
					*ptr = 0xFF000000 + (color4 & 0x0000FF00) / 0x00000100;ptr++;
					*ptr = 0xFF000000 + (color4 & 0x000000FF) / 0x00000001;ptr++;
				}
			}
			bmp.UnlockBits(bmpData);
			return bmp;
		}

		public Bitmap GetBitmap(View v)
		{
			return GetMipmap(0);
		}
	}
}
