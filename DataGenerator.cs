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
		
		double visibleSize;
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
			
			// Visiblity test
			PointF[] center = new PointF[] {renderDest.LeftTopCornerF};
			extraTransformation.TransformPoints(center);
			if (Math.Max(Math.Abs(center[0].X), Math.Abs(center[0].Y))-1.5d * renderDest.Size > 1) return;
			
			bool invisible     = (renderDest.Size < visibleSize);
			bool lastVisible   = (renderDest.Size < visibleSize * 2) && !invisible;
			
			f.SetColorIndexes(fractal, dataSrc.X, dataSrc.Y, dataSrc.Size / Fragment.FragmentSize);
			if (f.Depth < 10) {
				f.MakeChilds();
			}
			
			BitmapCacheItem c = UpdateBitmap(f);
			g.DrawImage(c.Bitmap,
						new PointF[] {renderDest.LeftTopCornerF,
									  renderDest.RightTopCornerF,
									  renderDest.LeftBottomCornerF},
						new RectangleF(c.X, c.Y, Fragment.FragmentSize, Fragment.FragmentSize),
						GraphicsUnit.Pixel);
			
			RenderFragmentsRecrusivly(f.ChildLT, dataSrc.LeftTopQuater    , renderDest.LeftTopQuater);
			RenderFragmentsRecrusivly(f.ChildRT, dataSrc.RightTopQuater   , renderDest.RightTopQuater);
			RenderFragmentsRecrusivly(f.ChildLB, dataSrc.LeftBottomQuater , renderDest.LeftBottomQuater);
			RenderFragmentsRecrusivly(f.ChildRB, dataSrc.RightBottomQuater, renderDest.RightBottomQuater);
		}
		
		public long Render(View v, Graphics destGraphics, int w, int h, double FPS)
		{
			long startTime = GetTicks();
			
			visibleSize = (((double)Fragment.FragmentSize*2)/(double)Math.Min(w,h));
			
			extraTransformation = new Matrix();
			extraTransformation.Rotate((float)(v.CurrentAngle), MatrixOrder.Append);
			
			g = destGraphics;
			g.CompositingQuality = CompositingQuality.HighSpeed;
			g.InterpolationMode = InterpolationMode.Bilinear;
			g.SmoothingMode = SmoothingMode.HighSpeed;
			g.ResetTransform();
			g.MultiplyTransform(extraTransformation, MatrixOrder.Append);
			// From [-1,1] to [0,w]
			g.TranslateTransform(1, 1, MatrixOrder.Append);
			g.ScaleTransform(w/2, h/2, MatrixOrder.Append);
			
			RectangleD renderArea = new RectangleD((-v.Xpos + -data.Size / 2) * v.Xzoom,
			                                       (-v.Ypos + -data.Size / 2) * v.Xzoom,
			                                       2 * v.Xzoom * data.Size / 2,
			                                       2 * v.Xzoom * data.Size / 2);
			
			RenderFragmentsRecrusivly(data.Root, data.Area, renderArea);
			
			long endTime = GetTicks();
			return endTime - startTime;
		}
	}
}
