using System;
using System.Collections.Generic;
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
		Fractal fractal;
		public bool debugMode = false;
		
		static int userThreadInterval = 10;
		long lastUserThreadActionTime;
		public event EventHandler UserThreadAction;
		
		DataTree data = new DataTree();
		
		bool aborted = false;
		
		public bool Aborted {
			get {
				return aborted;
			}
		}
		
		public void Abort()
		{
			aborted = true;
		}
		
		private long GetTicks()
		{
			return Environment.TickCount;
		}
		
		BitmapCache bitmapCache = new BitmapCache();
		
		public DataGenerator(Fractal fractal)
		{
			this.fractal = fractal;
		}
		
		void UpdateBitmap(Fragment f)
		{
			if (bitmapCache.IsCached(f)) return;
			
			Bitmap bitmap = f.MakeBitmap(fractal.ColorMap);
			
			// save to cache
			BitmapCacheItem c = bitmapCache.AllocateCache(f);
			Graphics.FromImage(c.Bitmap).DrawImage(bitmap, c.X , c.Y);
			bitmap.Dispose();
		}
		
		long     timeToAbort;
		bool     abortAllowed;
		
		double   initalSize;
		double   terminalSize;
		
		bool     simulation;
		Graphics g;
		
		Matrix extraTransformation;
		
		void RenderFragmentsRecrusivly(Fragment f, double dataX, double dataY, double dataSize, double renderX, double renderY, double renderSize, int depht, bool doVisiblityTest)
		{
			if (GetTicks() > lastUserThreadActionTime + userThreadInterval) {
				lastUserThreadActionTime = GetTicks();
				if (UserThreadAction != null) {
					UserThreadAction(this, EventArgs.Empty);
				}
			}
			if (f == null) return;
			if (aborted) return;
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
			
			f.SetColorIndexes(fractal, dataX, dataY, dataSize / Fragment.FragmentSize);
			
			if (!f.AllSame || (renderSize > 8*terminalSize))
				if (!lastRecrusion)
					if (!f.HasAllChilds)
						f.MakeChilds();
			
			// Anti-aliasing - do all subpixels before
			if (lastVisible && !lastRecrusion) {
				RenderFragmentsRecrusivly (f.ChildLT, dataX              , dataY              , dataSize/2, renderX                , renderY                , renderSize/2, depht + 1, !completlyInside);
				RenderFragmentsRecrusivly (f.ChildRT, dataX + dataSize/2 , dataY              , dataSize/2, renderX + renderSize/2 , renderY                , renderSize/2, depht + 1, !completlyInside);
				RenderFragmentsRecrusivly (f.ChildLB, dataX              , dataY + dataSize/2 , dataSize/2, renderX                , renderY + renderSize/2 , renderSize/2, depht + 1, !completlyInside);
				RenderFragmentsRecrusivly (f.ChildRB, dataX + dataSize/2 , dataY + dataSize/2 , dataSize/2, renderX + renderSize/2 , renderY + renderSize/2 , renderSize/2, depht + 1, !completlyInside);
				
				bitmapCache.ReleaseCache(f); //Force bitmap recreation to apply AA
			}
			
			if (abortAllowed && GetTicks() > timeToAbort) return;
			
			if ((!f.HasAllChilds || lastRecrusion || abortAllowed) && (!skipDrawing || lastVisible) && !invisible)
			{
				UpdateBitmap(f);
				if (simulation == false) {
					BitmapCacheItem c = bitmapCache[f];
					g.DrawImage(c.Bitmap,
								new PointF[] {new PointF( (float)renderX, (float)renderY ),
											  new PointF( (float)renderX + (float)renderSize, (float)renderY ),
											  new PointF( (float)renderX, (float)renderY + (float)renderSize )},
								new RectangleF(c.X, c.Y, Fragment.FragmentSize, Fragment.FragmentSize),
								GraphicsUnit.Pixel);
				}
			}
			if (lastRecrusion) return;
			
			// Normal - do subpixels after
			if (!lastVisible) {
				RenderFragmentsRecrusivly (f.ChildLT, dataX              , dataY              , dataSize/2, renderX                , renderY                , renderSize/2, depht + 1, !completlyInside);
				RenderFragmentsRecrusivly (f.ChildRT, dataX + dataSize/2 , dataY              , dataSize/2, renderX + renderSize/2 , renderY                , renderSize/2, depht + 1, !completlyInside);
				RenderFragmentsRecrusivly (f.ChildLB, dataX              , dataY + dataSize/2 , dataSize/2, renderX                , renderY + renderSize/2 , renderSize/2, depht + 1, !completlyInside);
				RenderFragmentsRecrusivly (f.ChildRB, dataX + dataSize/2 , dataY + dataSize/2 , dataSize/2, renderX + renderSize/2 , renderY + renderSize/2 , renderSize/2, depht + 1, !completlyInside);
			}
		}
		
		static Bitmap tmpImage = new Bitmap(2048, 1024);
		static Graphics tmpG = Graphics.FromImage(tmpImage);
		
		double compulsorySize = 1 / 1d;
		double visibleSize;
		
		long startTime;
		
		public long Render(View v, Graphics destGraphics, int w, int h, double FPS)
		{
			if (aborted) return 0;
			
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
			extraTransformation.Rotate((float)(v.CurrentAngle), MatrixOrder.Append);
			
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
				simulation     = false;
				initalSize     = double.MaxValue;
				terminalSize   = compulsorySize;
				
				RenderFragmentsRecrusivly(data.Root,
									  -data.Size / 2, -data.Size / 2, data.Size,
									  (-v.Xpos + -data.Size / 2) * v.Xzoom, (-v.Ypos + -data.Size / 2) * v.Xzoom, 2 * v.Xzoom * data.Size / 2,
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
				simulation     = true;
				initalSize     = double.MaxValue;
				terminalSize   = compulsorySize;
				
				RenderFragmentsRecrusivly(data.root,
									  -data.size / 2, -data.size / 2, data.size,
									  (-v.Xpos + -data.size / 2) * v.Xzoom, (-v.Ypos + -data.size / 2) * v.Xzoom, 2 * v.Xzoom * data.size / 2,
									  0, true);*/
				
				/// Multiple passes to incerase quality
				/// ----------------------------------
				while (finishedInTime && !aborted)
				{
					abortAllowed   = true;
					simulation     = false;
					initalSize     = terminalSize; // Start where we finished
					terminalSize   = initalSize / 2d; // Do one more level
					
					if (terminalSize < visibleSize/4) break;
					
					RenderFragmentsRecrusivly(data.Root,
										-data.Size / 2, -data.Size / 2, data.Size,
										(-v.Xpos + -data.Size / 2) * v.Xzoom, (-v.Ypos + -data.Size / 2) * v.Xzoom, 2 * v.Xzoom * data.Size / 2,
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
				RenderFragmentsRecrusivly(data.Root,
									  -data.Size / 2, -data.Size / 2, data.Size,
									  (-v.Xpos + -data.Size / 2) * v.Xzoom, (-v.Ypos + -data.Size / 2) * v.Xzoom, 2 * v.Xzoom * data.Size / 2,
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
