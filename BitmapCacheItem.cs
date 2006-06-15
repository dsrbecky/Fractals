using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Fractals
{
	public class BitmapCacheItem 
	{
		Fragment owner;
		Bitmap bitmap;
		int x, y;
		
		public Fragment Owner {
			get {
				return owner;
			}
			set {
				owner = value;
			}
		}
		
		public Bitmap Bitmap {
			get {
				return bitmap;
			}
		}
		
		public int X {
			get {
				return x;
			}
		}
		
		public int Y {
			get {
				return y;
			}
		}
		
		public BitmapCacheItem(Bitmap bitmap, int x, int y)
		{
			this.bitmap = bitmap;
			this.x = x;
			this.y = y;
		}
	}
}
