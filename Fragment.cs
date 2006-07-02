using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Fractals
{
	public class Fragment
	{
		public const int FragmentSize = 16;
		public const int BitmapSize = Fragment.FragmentSize + 1;
		
		Fragment childLT;
		Fragment childRT;
		Fragment childLB;
		Fragment childRB;
		
		public Fragment ChildLT {
			get {
				return childLT;
			}
			set {
				childLT = value;
			}
		}
		
		public Fragment ChildRT {
			get {
				return childRT;
			}
			set {
				childRT = value;
			}
		}
		
		public Fragment ChildLB {
			get {
				return childLB;
			}
			set {
				childLB = value;
			}
		}
		
		public Fragment ChildRB {
			get {
				return childRB;
			}
			set {
				childRB = value;
			}
		}
		
		
		// NOTE: Fragmet always has half the size of parent
		
		ColorIndex[,] data = new ColorIndex[FragmentSize, FragmentSize];
		bool done;
		int maxColorDifference = 255;
		int depth;
		
		public int MaxColorDifference {
			get {
				return maxColorDifference;
			}
		}
		
		public int Depth {
			get {
				return depth;
			}
		}
		
		public Fragment(int depth)
		{
			this.depth = depth;
		}
		
		public void MakeChilds()
		{
			if (!HasAllChilds) {
				childLT = new Fragment(depth + 1);
				childRT = new Fragment(depth + 1);
				childLB = new Fragment(depth + 1);
				childRB = new Fragment(depth + 1);
			}
		}
		
		public bool HasAllChilds {
			get {
				return childLT != null && childRT != null && childLB != null && childRB != null;
			}
		}
		
		public ColorIndex GetColorIndex(int x, int y)
		{
			return data[x, y];
		}
		
		public void SetColorIndex(int x, int y, ColorIndex val)
		{
			data[x, y] = val;
		}
		
		public void SetColorIndexes(Fractal fractal, double X, double Y, double size)
		{
			if (!done) {
				for(int y = 0; y < Fragment.FragmentSize; y += 1) {
					for(int x = 0; x < Fragment.FragmentSize; x += 1) {
						ColorIndex index = fractal.Equation.GetColorIndex(X + x * size, Y + y * size);
						SetColorIndex(x, y, index);
					}
				}
				maxColorDifference = GetMaxColorDifference(fractal.ColorMap);
				done = true;
			}
		}
		
		public int GetMaxColorDifference(ColorMap colorMap)
		{
			int minR, minG, minB;
			int maxR, maxG, maxB;
			minR = minG = minB = 256;
			maxR = maxG = maxB = 0;
			
			for(int y = 0; y < Fragment.FragmentSize; y += 1) {
				for(int x = 0; x < Fragment.FragmentSize; x += 1) {
					ColorIndex index = GetColorIndex(x, y);
					Color color = colorMap.GetColorFromIndex(index);
					
					minR = Math.Min(minR, color.R);
					minG = Math.Min(minG, color.G);
					minB = Math.Min(minB, color.B);
					
					maxR = Math.Max(maxR, color.R);
					maxG = Math.Max(maxG, color.G);
					maxB = Math.Max(maxB, color.B);
				}
			}
			return Math.Max(maxR - minR, Math.Max(maxG - minG, maxB - minB));
		}
		
		public Color GetColor(ColorMap colorMap, int x, int y)
		{
			return colorMap.GetColorFromIndex(GetColorIndex(x, y));
		}
		
		public Color GetAntiAliasedColor(ColorMap colorMap, int x, int y, int levelsOfAA)
		{
			if (levelsOfAA <= 0) {
				return GetColor(colorMap, x, y);
			} else {
				Fragment srcF;
				int srcX = (x*2)%Fragment.FragmentSize;
				int srcY = (y*2)%Fragment.FragmentSize;
				if (x < Fragment.FragmentSize/2) {
					if (y < Fragment.FragmentSize/2) {
						srcF = ChildLT;
					} else {
						srcF = ChildLB;
					}                
				} else {
					if (y < Fragment.FragmentSize/2) {
						srcF = ChildRT;
					} else {
						srcF = ChildRB;
					}
				}
				if (srcF != null && srcF.done) {
					Color color1 = srcF.GetAntiAliasedColor(colorMap, srcX + 0, srcY + 0, levelsOfAA - 1);
					Color color2 = srcF.GetAntiAliasedColor(colorMap, srcX + 0, srcY + 1, levelsOfAA - 1);
					Color color3 = srcF.GetAntiAliasedColor(colorMap, srcX + 1, srcY + 0, levelsOfAA - 1);
					Color color4 = srcF.GetAntiAliasedColor(colorMap, srcX + 1, srcY + 1, levelsOfAA - 1);
					
					int r = (color1.R + color2.R + color3.R + color4.R) / 4;
					int g = (color1.G + color2.G + color3.G + color4.G) / 4;
					int b = (color1.B + color2.B + color3.B + color4.B) / 4;
					
					return Color.FromArgb(r,g,b);
				} else {
					return GetColor(colorMap, x, y);
				}
			}
		}
		
		public unsafe Bitmap MakeBitmap(ColorMap colorMap)
		{
			Bitmap bitmap = new Bitmap(Fragment.BitmapSize, Fragment.BitmapSize, PixelFormat.Format32bppRgb);
			BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, Fragment.BitmapSize, Fragment.BitmapSize), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			UInt32* ptr = (UInt32*) bmpData.Scan0.ToPointer();
			for(int y = 0; y < Fragment.BitmapSize; y += 1) {
				for(int x = 0; x < Fragment.BitmapSize; x += 1) {
					uint k = 0;
					
					//k = 0x000F00 * (uint)(depth + 4);
					//k = 0x000100 * (uint)maxColorDifference;
//					if (x % 2 == 0 || y % 2 == 0) {
//						k = 0x7F0000;
//					}
//					if (x == 0 && y == 0) {
//						*ptr = 0xFF0000; ptr++; continue;
//					}
					
					if (x == Fragment.FragmentSize) {
						*ptr = *(ptr-1); ptr++;
					} else if (y == Fragment.FragmentSize) {
						*ptr = *(ptr-Fragment.BitmapSize); ptr++;
					} else {
						*ptr = k + (uint)GetAntiAliasedColor(colorMap, x, y, 3).ToArgb(); ptr++;
					}
				}
			}
//			ptr--; *ptr = 0x000000FF; ptr++;
			bitmap.UnlockBits(bmpData);
			return bitmap;
		}
	}
}
