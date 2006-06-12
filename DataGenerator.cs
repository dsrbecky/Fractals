using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

// TODO: Smooth zoom in
// TODO: TempData extension
// TODO: Anti-alising
// TODO: Save all uder one big bitmap

namespace Fractals
{
	public class DataGenerator
	{
		public bool debugMode = false;
		public bool UseDirectX = true;
		
		[DllImport("kernel32.dll")]
		private static extern int QueryPerformanceFrequency(out long frequency);
		
		[DllImport("kernel32.dll")]
		private static extern int QueryPerformanceCounter(out long tick);
		
		private long GetTicks()
		{
			long ticks;
			long frequency;
			QueryPerformanceFrequency(out frequency);
			if (frequency == 0)
			{
				return DateTime.Now.Ticks / 10000;
			}
			QueryPerformanceCounter(out ticks);
			return (ticks * 1000) / frequency;
		}
		
		public delegate void dlgtGetIndex(double p, double q,out double r,out double g,out double b);
		
		dlgtGetIndex _GetIndex;
		Color[] palette;
		
		// Data identifiers
		TempData data = new TempData();
		
		public bool abortFlag = false;
		
		struct BitmapCacheItem {
			public Bitmap bitmap;
			public int x;
			public int y;
		}
		
		class BitmapCache {
			const int cacheWidth = 15;
			const int cachesCount = 8;
			const int BitmapSize = Fragment.BitmapSize;
			
			const int cacheSizex = cacheWidth * cacheWidth * cachesCount;
			public const int cachePitch = cacheWidth * BitmapSize;
			
			Bitmap[] caches = new Bitmap[cachesCount];
			Fragment[] parentFragment = new Fragment[cacheSizex];
			
			public int nextCacheIndex = 0;
			
			public BitmapCache()
			{
				for (int i = 0; i < cachesCount; i++) {
					caches[i] = new Bitmap(cacheWidth * BitmapSize,
										   cacheWidth * BitmapSize,
										   PixelFormat.Format32bppPArgb);
				}
			}
			
			public void ReleaseCache(Fragment fragment) 
			{
				if (fragment.bitmapID != -1) {
					parentFragment[fragment.bitmapID] = null;
					fragment.bitmapID = -1;
				}
			}
			
			public BitmapCacheItem AllocateNew(Fragment fragment)
			{
				int index = nextCacheIndex;
				if (parentFragment[index] != null) {
					parentFragment[index].bitmapID = -1;
				}
				parentFragment[index] = fragment;
				parentFragment[index].bitmapID = index;
				
				nextCacheIndex = (nextCacheIndex + 1)%cacheSizex;
				//System.Diagnostics.Debug.WriteLine("Bitmap cached: ID=" + (index).ToString());
				
				return this[index];
			}
			
			public BitmapCacheItem this [int bitmapID] {
				get {
					BitmapCacheItem ret;
					
					ret.bitmap = caches[bitmapID / (cacheWidth * cacheWidth)];
					ret.x = (bitmapID % cacheWidth) * BitmapSize; 
					ret.y = ((bitmapID % (cacheWidth * cacheWidth)) / cacheWidth) * BitmapSize;
					
					return ret;
				}
			}
		}
		BitmapCache cache = new BitmapCache();
		
		public DataGenerator(dlgtGetIndex functionGetIndex, Color[] palette)
		{
			_GetIndex = functionGetIndex;
			this.palette = new Color[256];
			for (int i = 0; i < 256; i++) this.palette[i] = Color.Black;
			palette.CopyTo(this.palette, 0);
		}
		
		byte GetIndex(double p, double q)
		{
			double r,g,index;
			_GetIndex (p,q,out r,out g,out index);
			return Math.Min((byte)255,Math.Max((byte)0,(byte)index));
		}
		
		uint GetColor4(double p, double q, double width)
		{
			uint color4;
			color4 = 0;
			
			color4 += GetIndex(p,q);
			p += width;
			color4 *= 0x100;
			color4 += GetIndex(p,q);
			p += width;
			color4 *= 0x100;
			color4 += GetIndex(p,q);
			p += width;
			color4 *= 0x100;
			color4 += GetIndex(p,q);
			
			return color4;
		}
		
		void UpdateFragment(Fragment f, double X, double Y, double size)
		{
			if (f.done == 0xFFFF) return;
			//if (IsOutOfFrustum((float)X,(float)Y,(float)size,(float)size)) return;
			
			//Console.WriteLine("     - Updaing: X=" + X.ToString() + " Y=" + Y.ToString() + " size=" + size.ToString());
			
			int minR, minG, minB;
			minR = minG = minB = 256;
			int maxR, maxG, maxB;
			maxR = maxG = maxB = 0;
			f.allTheSame = true;
			uint first = GetColor4(X, Y, size);
			
			for(int y = 0;y < Fragment.FragmentSize;y += 1)
			{
				for(int x = 0;x < Fragment.FragmentSize;x += 4)
				{
					uint color4 = GetColor4(X+x*size,
						Y+y*size,
						size);
					f.data[(x + Fragment.FragmentSize*y)/4] = color4;
					
					for (int i = 0; i <= 24; i += 8) {
						minR = Math.Min(minR, palette[(color4 & (0xFFu<<i)) / (0x01u<<i)].R);
						minG = Math.Min(minG, palette[(color4 & (0xFFu<<i)) / (0x01u<<i)].G);
						minB = Math.Min(minB, palette[(color4 & (0xFFu<<i)) / (0x01u<<i)].B);
						
						maxR = Math.Max(maxR, palette[(color4 & (0xFFu<<i)) / (0x01u<<i)].R);
						maxG = Math.Max(maxG, palette[(color4 & (0xFFu<<i)) / (0x01u<<i)].G);
						maxB = Math.Max(maxB, palette[(color4 & (0xFFu<<i)) / (0x01u<<i)].B);
					}
					//if (color4 != first) f.allTheSame = false;
					//if (Math.Abs(color4 - first) > 5) f.allTheSame = false;
				}
			}
			int AAdiff = 5;
			f.allTheSame = ((maxR - minR) < AAdiff) && ((maxG - minG) < AAdiff) && ((maxB - minB) < AAdiff);
			
			f.done = 0xFFFF;
		}
		
		unsafe void UpdateBmp(Fragment f, int depth)
		{
			if (f.bitmapID != -1) return;
			
			Bitmap bitmap = new Bitmap(Fragment.BitmapSize, Fragment.BitmapSize, PixelFormat.Format32bppRgb);
			BitmapData bmpData = bitmap.LockBits(new Rectangle(0,0,Fragment.BitmapSize, Fragment.BitmapSize),ImageLockMode.WriteOnly,PixelFormat.Format32bppArgb);
			UInt32* ptr = (UInt32*) bmpData.Scan0.ToPointer();
			for(int y = 0;y < Fragment.BitmapSize;y += 1)
			{
				for(int x = 0;x < Fragment.BitmapSize;x += 1)
				{
					uint k;
					if (debugMode) {
						k = (uint)(16*depth)*0x100;
						//k += ((x+y)%2 == 0)?0x7Fu:0;
						//k += ((x+y)%4 == 0)?0x7Fu:0;
					} else {
						k = 0;
					}                    
					
					if (x == Fragment.FragmentSize)	{
						*ptr = *(ptr-1); ptr++;
					} else if (y == Fragment.FragmentSize) {
						*ptr = *(ptr-Fragment.BitmapSize); ptr++;
					} else {
						*ptr = k + (uint)GetColorAA(f, x, y, 3).ToArgb(); ptr++;
					}
				}
			}
			bitmap.UnlockBits(bmpData);
			
			// save to cache
			BitmapCacheItem c = cache.AllocateNew(f);
			Graphics.FromImage(c.bitmap).DrawImage(bitmap, c.x, c.y);
			bitmap.Dispose();
			bitmap = null;
		}
		
		Color GetColorAA(Fragment f, int x, int y, int levelsOfAA)
		{
			if (levelsOfAA <= 0 || !doAntiAliasing) {
				return GetColor(f,x,y);
			}
			
			Fragment srcF;
			int srcX = (x*2)%Fragment.FragmentSize;
			int srcY = (y*2)%Fragment.FragmentSize;
			if (x < Fragment.FragmentSize/2) {
				if (y < Fragment.FragmentSize/2) {
					srcF = f.childLT;
				} else {
					srcF = f.childLB;
				}                
			} else {
				if (y < Fragment.FragmentSize/2) {
					srcF = f.childRT;
				} else {
					srcF = f.childRB;
				}
			}
			if (srcF != null && srcF.done != 0) {
				Color color1 = GetColorAA(srcF, srcX + 0, srcY + 0, levelsOfAA - 1);
				Color color2 = GetColorAA(srcF, srcX + 0, srcY + 1, levelsOfAA - 1);
				Color color3 = GetColorAA(srcF, srcX + 1, srcY + 0, levelsOfAA - 1);
				Color color4 = GetColorAA(srcF, srcX + 1, srcY + 1, levelsOfAA - 1);
				
				int r = (color1.R + color2.R + color3.R + color4.R) / 4;
				int g = (color1.G + color2.G + color3.G + color4.G) / 4;
				int b = (color1.B + color2.B + color3.B + color4.B) / 4;
				
				return Color.FromArgb(r,g,b);
			} else {
				return GetColor(f,x,y);
			}
		}
		
		Color GetColor(Fragment f, int x, int y)
		{
			uint color4;
			color4 = f.data[(x - x%4 + Fragment.FragmentSize*y)/4];
			
			return palette[(color4 & (0xFF << (8 * (3 - x%4)))) / (0x01 << (8 * (3 - x%4)))];
		}
		
		long     timeToAbort;
		bool     abortAllowed;
		
		double   initalSize;
		double   terminalSize;
		
		bool     simulation;
		Graphics g;
		
		bool     doAntiAliasing;
		
		Matrix extraTransformation;
		
		void RenderFragmentsRecrusivly(Fragment f, double dataX, double dataY, double dataSize, double renderX, double renderY, double renderSize, int depht, bool doVisiblityTest)
		{
			if (f == null) return;
			if (abortFlag) return;
			if (abortAllowed && GetTicks() > timeToAbort) return;
			
			bool completlyInside;
			
			if (doVisiblityTest) {
				PointF[] center = new PointF[] {new PointF((float)renderX, (float)renderY)};
				extraTransformation.TransformPoints(center);
				
				// Is completly outside ?
				if (Math.Max(Math.Abs(center[0].X), Math.Abs(center[0].Y))-1.5d * renderSize > 1) return;
				
				// Is completly inside ?
				completlyInside = (Math.Min(Math.Abs(center[0].X), Math.Abs(center[0].Y))+1.5d * renderSize < 1);
			} else {
				completlyInside = true;
			}
			
			completlyInside = false;
			
			//if (!new RectangleF((float)renderX, (float)renderY, (float)renderSize, (float)renderSize).IntersectsWith(new RectangleF(-1f,-1f,2,2))) return;
			
			bool lastRecrusion = (renderSize < terminalSize * 2);
			bool skipDrawing   = (renderSize >= initalSize * 2);
			bool invisible     = (renderSize < visibleSize);
			bool lastVisible   = (renderSize < visibleSize * 2) && !invisible;
			
			UpdateFragment(f, dataX, dataY, dataSize / Fragment.FragmentSize);
			
			if (!f.allTheSame || (renderSize > 8*terminalSize))
				if (!lastRecrusion)
					if (!f.HasAllChilds)
						f.MakeChilds();
			
			// Anti-aliasing - do all subpixels before
			if (lastVisible && !lastRecrusion) {
				RenderFragmentsRecrusivly (f.childLT, dataX              , dataY              , dataSize/2, renderX                , renderY                , renderSize/2, depht + 1, !completlyInside);
				RenderFragmentsRecrusivly (f.childRT, dataX + dataSize/2 , dataY              , dataSize/2, renderX + renderSize/2 , renderY                , renderSize/2, depht + 1, !completlyInside);
				RenderFragmentsRecrusivly (f.childLB, dataX              , dataY + dataSize/2 , dataSize/2, renderX                , renderY + renderSize/2 , renderSize/2, depht + 1, !completlyInside);
				RenderFragmentsRecrusivly (f.childRB, dataX + dataSize/2 , dataY + dataSize/2 , dataSize/2, renderX + renderSize/2 , renderY + renderSize/2 , renderSize/2, depht + 1, !completlyInside);
				
				cache.ReleaseCache(f); //Force bitmap recreation to apply AA
			}
			
			if (abortAllowed && GetTicks() > timeToAbort) return;
			
			if ((!f.HasAllChilds || lastRecrusion || abortAllowed) && (!skipDrawing || lastVisible) && !invisible)
			{
				cache.ReleaseCache(f);
				UpdateBmp(f, depht);
				if (simulation == false) {
					BitmapCacheItem c = cache[f.bitmapID];
					g.DrawImage(c.bitmap,
								new PointF[] {new PointF( (float)renderX, (float)renderY ),
											  new PointF( (float)renderX + (float)renderSize, (float)renderY ),
											  new PointF( (float)renderX, (float)renderY + (float)renderSize )},
								new RectangleF(c.x, c.y, Fragment.FragmentSize, Fragment.FragmentSize),
								GraphicsUnit.Pixel);
				}
			}
			if (lastRecrusion) return;
			
			// Normal - do subpixels after
			if (!lastVisible) {
				RenderFragmentsRecrusivly (f.childLT, dataX              , dataY              , dataSize/2, renderX                , renderY                , renderSize/2, depht + 1, !completlyInside);
				RenderFragmentsRecrusivly (f.childRT, dataX + dataSize/2 , dataY              , dataSize/2, renderX + renderSize/2 , renderY                , renderSize/2, depht + 1, !completlyInside);
				RenderFragmentsRecrusivly (f.childLB, dataX              , dataY + dataSize/2 , dataSize/2, renderX                , renderY + renderSize/2 , renderSize/2, depht + 1, !completlyInside);
				RenderFragmentsRecrusivly (f.childRB, dataX + dataSize/2 , dataY + dataSize/2 , dataSize/2, renderX + renderSize/2 , renderY + renderSize/2 , renderSize/2, depht + 1, !completlyInside);
			}
		}
		
		static Bitmap tmpImage = new Bitmap(2048, 1024);
		static Graphics tmpG = Graphics.FromImage(tmpImage);
		
		double compulsorySize = 1 / 1d;
		double visibleSize;
		
		long startTime;
		
		public long Render(View v, Graphics destGraphics, int w, int h, double FPS)
		{
			visibleSize = (((double)Fragment.FragmentSize*2)/(double)Math.Min(w,h));
			System.Diagnostics.Debug.WriteLine("visibleSize = " + visibleSize.ToString());
			
			g = destGraphics;
			//g = tmpG;
			startTime = GetTicks();
			if (FPS == 0) {
				timeToAbort = long.MaxValue;
			} else {
				timeToAbort = startTime + (int)(1000d / FPS);
			}
			
			g.ResetTransform();
			
			// TESTING: Wide field of view
			if (debugMode) {
				g.ScaleTransform(0.5f, 0.5f, MatrixOrder.Append);
			}
			
			// Extra transformation
			extraTransformation = new Matrix();
			extraTransformation.Rotate((float)(v.Angle), MatrixOrder.Append);
			
			g.MultiplyTransform(extraTransformation, MatrixOrder.Append);
			
			// From [-1,1] to [0,w]
			g.TranslateTransform(1, 1, MatrixOrder.Append);
			g.ScaleTransform(w/2, h/2, MatrixOrder.Append);           
			
			// TESTING: White background
			if (debugMode) {
				g.Clear(Color.White);
			}
			
			//g.CompositingMode = CompositingMode.SourceCopy;
			g.CompositingQuality = CompositingQuality.HighSpeed;
			g.InterpolationMode = InterpolationMode.Bilinear;
			g.SmoothingMode = SmoothingMode.HighSpeed;
			
			for (int i = 0; i < 1; i++)
			{
				simulation = false;
				
				bool finishedInTime;
				
				/// Primary draw - must finish
				/// --------------------------
				abortAllowed   = false;
				doAntiAliasing = false;
				simulation     = false;
				initalSize     = double.MaxValue;
				terminalSize   = compulsorySize;
				
				RenderFragmentsRecrusivly(data.root,
									  data.minX, data.minY, data.size,
									  (-v.Xpos + data.minX) * v.Xzoom, (-v.Ypos + data.minY) * v.Xzoom, 2 * v.Xzoom * data.size / 2,
									  0, true);
				
				finishedInTime = GetTicks() < timeToAbort;
				
				if (!finishedInTime)
				{
					// We haven't drawn it in time
					// -> be less strict next time
					compulsorySize *= 2;
				}
				
				/*
				/// Anti-alias primary draw
				/// --------------------------
				abortAllowed   = true;
				doAntiAliasing = true;
				simulation     = true;
				initalSize     = double.MaxValue;
				terminalSize   = compulsorySize;
				
				RenderFragmentsRecrusivly(data.root,
									  data.minX, data.minY, data.size,
									  (-v.Xpos + data.minX) * v.Xzoom, (-v.Ypos + data.minY) * v.Xzoom, 2 * v.Xzoom * data.size / 2,
									  0, true);*/
				
				/// Multiple passes to incerase quality
				/// ----------------------------------
				while (finishedInTime && !abortFlag)
				{
					abortAllowed   = true;
					doAntiAliasing = true;
					simulation     = false;
					initalSize     = terminalSize; // Start where we finished
					terminalSize   = initalSize / 2d; // Do one more level
					
					if (terminalSize < visibleSize/4) break;
					
					RenderFragmentsRecrusivly(data.root,
										data.minX, data.minY, data.size,
										(-v.Xpos + data.minX) * v.Xzoom, (-v.Ypos + data.minY) * v.Xzoom, 2 * v.Xzoom * data.size / 2,
										0, true);
					
					finishedInTime = GetTicks() < timeToAbort;
					
					if (finishedInTime && FPS != 0 && !(terminalSize < visibleSize))
					{
						// We have drawn more in time
						// -> try to do the same amount next time
						compulsorySize = terminalSize;
					}
				}
			}
			for (int k = 0; k < 0; k++)
			{
				simulation = true;
				RenderFragmentsRecrusivly(data.root,
									  data.minX, data.minY, data.size,
									  (-v.Xpos + data.minX) * v.Xzoom, (-v.Ypos + data.minY) * v.Xzoom, 2 * v.Xzoom * data.size / 2,
									  0, true);
			}
			
			// TESTING: Red bounding box
			//g.FillPolygon(new SolidBrush(Color.FromArgb(64, 255, 0, 0)), new PointF[4] { new PointF(-1, -1), new PointF(-1, 1), new PointF(1, 1), new PointF(1, -1) });
			//g.DrawLines(new Pen(Color.Red, 0), new PointF[] { new PointF(-1, -1), new PointF(-1, 1), new PointF(-1, 1), new PointF(1, 1), new PointF(1, 1), new PointF(1, -1), new PointF(1, -1), new PointF(-1, -1) });
			//g.FillPolygon(new SolidBrush(Color.FromArgb(64, 255, 0, 0)), frustum);
			//g.DrawLines(new Pen(Color.Red, 0), new PointF[] { frustum[0], frustum[1], frustum[1], frustum[2], frustum[2], frustum[3], frustum[3], frustum[0] });
			
			g.ResetTransform();
			//for (int j = 0; j < 1; j++) gr.DrawImage(tmpImage, 0, 0);
			
			long miliseconds = GetTicks() - startTime;
			//System.Diagnostics.Debug.WriteLine("Time to draw: " + miliseconds.ToString() +
			//                                   " quality " + (1d/compulsorySize).ToString());
			
			//destGraphics.DrawImage(tmpImage,0,0);
			
			return miliseconds;
		}
	}
}
