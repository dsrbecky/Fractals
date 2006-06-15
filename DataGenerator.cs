using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

// TODO: TempData extension

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
		
		BitmapCacheItem UpdateBitmap(Fragment f)
		{
			if (bitmapCache.IsCached(f)) {
				return bitmapCache[f];
			} else {
				BitmapCacheItem c = bitmapCache.AllocateCache(f);
				using(Bitmap bitmap = f.MakeBitmap(fractal.ColorMap)) {
					using(Graphics g = Graphics.FromImage(c.Bitmap)) {
						g.DrawImage(bitmap, c.X , c.Y);
					}
				}
				return c;
			}
		}
		
		
		
		long     timeToAbort;
		bool     abortAllowed;
		
		double   initalSize;
		double   terminalSize;
		
		double compulsorySize = 1 / 1d;
		double visibleSize;
		
		long startTime;
		
		Graphics g;
		
		Matrix extraTransformation;
		
		void RenderFragmentsRecrusivly(Fragment f, RectangleD dataSrc, RectangleD renderDest)
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
			
			// Visiblity test
			PointF[] center = new PointF[] {renderDest.LeftTopCornerF};
			extraTransformation.TransformPoints(center);
			if (Math.Max(Math.Abs(center[0].X), Math.Abs(center[0].Y))-1.5d * renderDest.Size > 1) return;
			
			bool lastRecrusion = (renderDest.Size < terminalSize * 2);
			bool skipDrawing   = (renderDest.Size >= initalSize * 2);
			bool invisible     = (renderDest.Size < visibleSize);
			bool lastVisible   = (renderDest.Size < visibleSize * 2) && !invisible;
			
			f.SetColorIndexes(fractal, dataSrc.X, dataSrc.Y, dataSrc.Size / Fragment.FragmentSize);
			
			if ((!f.AllSame || (renderDest.Size > 8 * terminalSize)) && !lastRecrusion && !f.HasAllChilds) {
				f.MakeChilds();
			}
			
			// Anti-aliasing - do all subpixels before
			if (lastVisible && !lastRecrusion) {
				RenderFragmentsRecrusivly (f.ChildLT, dataSrc.LeftTopQuater    , renderDest.LeftTopQuater);
				RenderFragmentsRecrusivly (f.ChildRT, dataSrc.RightTopQuater   , renderDest.RightTopQuater);
				RenderFragmentsRecrusivly (f.ChildLB, dataSrc.LeftBottomQuater , renderDest.LeftBottomQuater);
				RenderFragmentsRecrusivly (f.ChildRB, dataSrc.RightBottomQuater, renderDest.RightBottomQuater);
				
				bitmapCache.ReleaseCache(f); //Force bitmap recreation to apply AA
			}
			
			if (abortAllowed && GetTicks() > timeToAbort) return;
			
			if ((!f.HasAllChilds || lastRecrusion || abortAllowed) && (!skipDrawing || lastVisible) && !invisible) {
				BitmapCacheItem c = UpdateBitmap(f);
				g.DrawImage(c.Bitmap,
							new PointF[] {renderDest.LeftTopCornerF,
										  renderDest.RightTopCornerF,
										  renderDest.LeftBottomCornerF},
							new RectangleF(c.X, c.Y, Fragment.FragmentSize, Fragment.FragmentSize),
							GraphicsUnit.Pixel);
			}
			
			// Normal - do subpixels after
			if (!lastRecrusion && !lastVisible) {
				RenderFragmentsRecrusivly (f.ChildLT, dataSrc.LeftTopQuater    , renderDest.LeftTopQuater);
				RenderFragmentsRecrusivly (f.ChildRT, dataSrc.RightTopQuater   , renderDest.RightTopQuater);
				RenderFragmentsRecrusivly (f.ChildLB, dataSrc.LeftBottomQuater , renderDest.LeftBottomQuater);
				RenderFragmentsRecrusivly (f.ChildRB, dataSrc.RightBottomQuater, renderDest.RightBottomQuater);
			}
		}
		
		public long Render(View v, Graphics destGraphics, int w, int h, double FPS)
		{
			if (aborted) return 0;
			
			visibleSize = (((double)Fragment.FragmentSize*2)/(double)Math.Min(w,h));
			System.Diagnostics.Debug.WriteLine("visibleSize = " + visibleSize.ToString());
			
			g = destGraphics;
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
			
			RectangleD renderArea = new RectangleD((-v.Xpos + -data.Size / 2) * v.Xzoom,
			                                       (-v.Ypos + -data.Size / 2) * v.Xzoom,
			                                       2 * v.Xzoom * data.Size / 2,
			                                       2 * v.Xzoom * data.Size / 2);
			
			// TESTING: White background
			if (debugMode) {
				g.Clear(Color.White);
			}
			
			g.CompositingQuality = CompositingQuality.HighSpeed;
			g.InterpolationMode = InterpolationMode.Bilinear;
			g.SmoothingMode = SmoothingMode.HighSpeed;
			
			bool finishedInTime;
			
			/// Primary draw - must finish
			/// --------------------------
			abortAllowed   = false;
			initalSize     = double.MaxValue;
			terminalSize   = compulsorySize;
			
			RenderFragmentsRecrusivly(data.Root, data.Area, renderArea);
			
			finishedInTime = GetTicks() < timeToAbort;
			
			if (!finishedInTime) {
				// We haven't drawn it in time
				// -> be less strict next time
				compulsorySize *= 2;
			}
			
			/// Multiple passes to incerase quality
			/// ----------------------------------
			while (finishedInTime && !aborted) {
				abortAllowed   = true;
				initalSize     = terminalSize; // Start where we finished
				terminalSize   = initalSize / 2d; // Do one more level
				
				if (terminalSize < visibleSize/4) break;
				
				RenderFragmentsRecrusivly(data.Root, data.Area, renderArea);
				
				finishedInTime = GetTicks() < timeToAbort;
				
				if (finishedInTime && FPS != 0 && !(terminalSize < visibleSize)) {
					// We have drawn more in time
					// -> try to do the same amount next time
					compulsorySize = terminalSize;
				}
			}
			
			// TESTING: Red bounding box
			//g.FillPolygon(new SolidBrush(Color.FromArgb(64, 255, 0, 0)), new PointF[4] { new PointF(-1, -1), new PointF(-1, 1), new PointF(1, 1), new PointF(1, -1) });
			//g.DrawLines(new Pen(Color.Red, 0), new PointF[] { new PointF(-1, -1), new PointF(-1, 1), new PointF(-1, 1), new PointF(1, 1), new PointF(1, 1), new PointF(1, -1), new PointF(1, -1), new PointF(-1, -1) });
			//g.FillPolygon(new SolidBrush(Color.FromArgb(64, 255, 0, 0)), frustum);
			//g.DrawLines(new Pen(Color.Red, 0), new PointF[] { frustum[0], frustum[1], frustum[1], frustum[2], frustum[2], frustum[3], frustum[3], frustum[0] });
			
			g.ResetTransform();
			
			long miliseconds = GetTicks() - startTime;
			
			return miliseconds;
		}
	}
}
