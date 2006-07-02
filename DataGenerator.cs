using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Fractals
{
	public class DataGenerator
	{
		// Render target
		Fractal fractal;
		Graphics graphics;
		Rectangle renderRectangle;
		
		// Multi-threading
		TimeSpan userThreadInterval = TimeSpan.FromMilliseconds(10);
		DateTime lastUserThreadActionTime;
		public event EventHandler UserThreadAction;
		bool aborted = false;
		
		// Caching
		DataTree data = new DataTree();
		BitmapCache bitmapCache = new BitmapCache();
		RenderSet renderSet;
		
		// Real-time rendering
		double convergance = 0.2d;
		double numberOfFragmentsToRender = 16;
		int numberOfFragmentsRendered;
		TimeSpan targetRenderTime = TimeSpan.FromMilliseconds(1d / 20);
		TimeSpan actualRenderTime;
		
		double pixelSize;
		
		public bool Aborted {
			get {
				return aborted;
			}
		}
		
		public int NumberOfFragmentsRendered {
			get {
				return numberOfFragmentsRendered;
			}
		}
		
		public TimeSpan TargetRenderTime {
			get {
				return targetRenderTime;
			}
			set {
				targetRenderTime = value;
			}
		}
		
		public TimeSpan ActualRenderTime {
			get {
				return actualRenderTime;
			}
		}
		
		public double FPS {
			get {
				return (double)TimeSpan.TicksPerSecond / actualRenderTime.Ticks;
			}
		}
		
		public DataGenerator(Fractal fractal, Graphics graphics, Rectangle renderRectangle)
		{
			this.fractal = fractal;
			this.graphics = graphics;
			this.renderRectangle = renderRectangle;
		}
		
		public void Abort()
		{
			aborted = true;
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
		
		void Render(IEnumerable<RenderOperation> operations)
		{
			foreach (RenderOperation operation in operations) {
				if (HighPrecisionTimer.Now - lastUserThreadActionTime > userThreadInterval) {
					lastUserThreadActionTime = HighPrecisionTimer.Now;
					if (UserThreadAction != null) {
						UserThreadAction(this, EventArgs.Empty);
					}
				}
				if (aborted) return;
				
				operation.Fragment.SetColorIndexes(fractal, operation.DataSource.X, operation.DataSource.Y, operation.DataSource.Size / Fragment.FragmentSize);
				
				RenderOperation largeEnoughRenderOperation = operation;
				while (largeEnoughRenderOperation.TexelSize < pixelSize && operation.Parent != null) {
					largeEnoughRenderOperation = largeEnoughRenderOperation.Parent;
					bitmapCache.ReleaseCache(largeEnoughRenderOperation.Fragment);
				}
				
				BitmapCacheItem c = UpdateBitmap(largeEnoughRenderOperation.Fragment);
				graphics.DrawImage(c.Bitmap,
				                   new PointF[] {largeEnoughRenderOperation.RenderDestination.LeftTopCornerF,
				                                 largeEnoughRenderOperation.RenderDestination.RightTopCornerF,
				                                 largeEnoughRenderOperation.RenderDestination.LeftBottomCornerF},
				                   new RectangleF(c.X, c.Y, Fragment.FragmentSize, Fragment.FragmentSize),
				                   GraphicsUnit.Pixel);
			}
		}
		
		public void Render()
		{
			DateTime startTime = HighPrecisionTimer.Now;
			
			while(data.Size < fractal.View.BoundingBoxSize * 2) {
				data.ExtendRoot();
			}
			
			Matrix rotation = new Matrix();
			rotation.Rotate((float)fractal.View.CurrentAngle);
			
			graphics.CompositingQuality = CompositingQuality.HighSpeed;
			graphics.InterpolationMode = InterpolationMode.Bilinear;
			graphics.SmoothingMode = SmoothingMode.HighSpeed;
			graphics.ResetTransform();
			graphics.MultiplyTransform(rotation, MatrixOrder.Append);
			// From [-1,1] to [0,w]
			graphics.TranslateTransform(1, 1, MatrixOrder.Append);
			graphics.ScaleTransform(renderRectangle.Width / 2, renderRectangle.Height / 2, MatrixOrder.Append);
			
			RectangleD renderArea = new RectangleD((-fractal.View.Xpos + -data.Size / 2) * fractal.View.Xzoom,
			                                       (-fractal.View.Ypos + -data.Size / 2) * fractal.View.Xzoom,
			                                       fractal.View.Xzoom * data.Size,
			                                       fractal.View.Xzoom * data.Size);
			
			pixelSize = Math.Min(2d / renderRectangle.Width, 2d / renderRectangle.Height);
			
			RenderOperation root = new RenderOperation(null, data.Root, data.Area, renderArea, rotation);
			renderSet = new RenderSet(root, (int)numberOfFragmentsToRender);
			Render(renderSet.RenderOperations);
			
			numberOfFragmentsRendered = renderSet.Count;
			
			actualRenderTime = HighPrecisionTimer.Now - startTime;
			double timeFraction = (double)actualRenderTime.Ticks / (double)targetRenderTime.Ticks;
			double optimalCount = (double)numberOfFragmentsToRender / timeFraction;
			numberOfFragmentsToRender = numberOfFragmentsToRender * (1 - convergance) + optimalCount * convergance;
			numberOfFragmentsToRender = Math.Max(16d, numberOfFragmentsToRender);
		}
		
		public void HighQualityRender()
		{
			Render();
			while(!aborted) {
				Render(renderSet.MakeRefineOperations());
			}
		}
	}
}
