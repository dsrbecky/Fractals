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
		Renderer.Texture texture;
		int x, y;
		
		public Fragment Owner {
			get {
				return owner;
			}
			set {
				owner = value;
			}
		}
		
		public Renderer.Texture Texture {
			get {
				return texture;
			}
		}
		
		public RectangleF TextureSourceRect {
			get {
				return new RectangleF((float)(x + 0.5f),
				                      (float)(y + 0.5f),
				                      (float)Fragment.FragmentSize,
				                      (float)Fragment.FragmentSize);
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
		
		public BitmapCacheItem(Renderer.Texture texture, int x, int y)
		{
			this.texture = texture;
			this.x = x;
			this.y = y;
		}
	}
}
