using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Tao.FreeGlut;
using Tao.OpenGl;

namespace Fractals
{
	public abstract class Renderer
	{
		public class Texture
		{
			Bitmap bitmap;
			uint textureName;
			
			public Bitmap Bitmap {
				get {
					return bitmap;
				}
				set {
					bitmap = value;
				}
			}
			
			public uint TextureName {
				get {
					return textureName;
				}
				set {
					textureName = value;
				}
			}
		}
		
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
		BitmapCache bitmapCache;
		RenderSet renderSet;
		
		// Movement
		public PointF mousePosition;
		public MouseButtons mouseButtons;
		
		// Real-time rendering
		double convergance = 0.2d;
		double numberOfFragmentsToRender = 16;
		int numberOfFragmentsRendered;
		TimeSpan targetRenderTime = TimeSpan.FromMilliseconds(1d / 20);
		TimeSpan actualRenderTime;
		
		double pixelSize;
		
		public Fractal Fractal {
			get {
				return fractal;
			}
			set {
				fractal = value;
				aborted = false;
			}
		}
		
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
		
		public Renderer(Fractal fractal, Graphics graphics, Rectangle renderRectangle)
		{
			this.fractal = fractal;
			this.graphics = graphics;
			this.renderRectangle = renderRectangle;
			this.bitmapCache = new BitmapCache(this);
		}
		
		public void Abort()
		{
			aborted = true;
		}
		
		public Texture MakeTexture(int width, int height)
		{
			Texture tex = new Texture();
			tex.Bitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
			uint textureName;
			Gl.glGenTextures(1, out textureName);
			tex.TextureName = textureName;
			
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureName);
			uint[,] image = new uint[width, height];
			for(int x = 0; x < width; x++) {
				for(int y = 0; y < height; y++) {
					image[x,y] = 0xFFFF00FF; // Purple
				}
			}
			
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
			
			Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, width, height, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, image);
		
			
			return tex;
		}
		
		public unsafe void UpdateTexture(Texture tex, int dstX, int dstY, uint[,] image)
		{
			Bitmap bitmap = new Bitmap(Fragment.BitmapSize, Fragment.BitmapSize, PixelFormat.Format32bppRgb);
			BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, Fragment.BitmapSize, Fragment.BitmapSize), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			UInt32* ptr = (UInt32*) bmpData.Scan0.ToPointer();
			for(int y = 0; y < Fragment.BitmapSize; y += 1) {
				for(int x = 0; x < Fragment.BitmapSize; x += 1) {
					if (x == Fragment.FragmentSize) {
						*ptr = *(ptr-1); ptr++;
					} else if (y == Fragment.FragmentSize) {
						*ptr = *(ptr-Fragment.BitmapSize); ptr++;
					} else {
						*ptr = image[x,y]; ptr++;
					}
				}
			}
//			ptr--; *ptr = 0x000000FF; ptr++;
			bitmap.UnlockBits(bmpData);
			using(Graphics g = Graphics.FromImage(tex.Bitmap)) {
				g.DrawImage(bitmap, dstX, dstY);
			}
			
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, tex.TextureName);
			Gl.glTexSubImage2D(Gl.GL_TEXTURE_2D, 0, dstX, dstY, Fragment.BitmapSize, Fragment.BitmapSize, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, image);
		}
		
		BitmapCacheItem UpdateBitmap(Fragment f)
		{
			if (bitmapCache.IsCached(f)) {
				return bitmapCache[f];
			} else {
				BitmapCacheItem c = bitmapCache.AllocateCache(f);
				UpdateTexture(c.Texture, c.X, c.Y, f.MakeImage(fractal.ColorMap));
				return c;
			}
		}
		
		void Render(IEnumerable<RenderOperation> operations)
		{
			foreach (RenderOperation operation in operations) {
				Render(operation);
				if (aborted) break;
			}
		}
		
		void Render(RenderOperation operation)
		{
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
			Draw(c.Texture, c.TextureSourceRect, largeEnoughRenderOperation.RenderDestination);
		}
		
		protected virtual void Draw(Texture tex, RectangleF src, RectangleD dest)
		{
			graphics.DrawImage(tex.Bitmap,
			                   new PointF[] {dest.LeftTopCornerF,
			                                 dest.RightTopCornerF,
			                                 dest.LeftBottomCornerF},
			                   src,
			                   GraphicsUnit.Pixel);			
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
			Render(renderSet.MakeRefineOperations(pixelSize));
		}
		
		public void Rotate(int direction)
		{
			fractal.View.Angle += direction * 10;
		}
		
		public void Move()
		{
			if (fractal.View.Rotating) {
				fractal.View.AnimateRotation();
			}
			
			if (mouseButtons != MouseButtons.None) {
				double zoomSpeed = 1;
				if (mouseButtons == MouseButtons.Left)   zoomSpeed = 1+8f/128;
				if (mouseButtons == MouseButtons.Middle) zoomSpeed = 1;
				if (mouseButtons == MouseButtons.Right)  zoomSpeed = 1-8f/128;
				
				PointF logicalPos = new PointF(); // [-1,1] mapping
				logicalPos.X = (mousePosition.X - 0.5f) * 2f  / 8f;
				logicalPos.Y = (mousePosition.Y - 0.5f) * 2f  / 8f;
				
				fractal.View.ZoomIn(logicalPos, zoomSpeed);
			}
		}
	}
}
