using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Fractals
{
	public class Rendrer
	{
		public static TempData data;
		public static Graphics g;
		public static PointF[] frustum = new PointF[4];
		public static Matrix transform = new Matrix();
		float minSize;

		public Rendrer(TempData _data)
		{
            //data = _data;
		}

		static unsafe void UpdateBmp(Fragment f, int depth)
		{
			if (f.bitmap != null) return;

			f.bitmap = new Bitmap(Fragment.FragmentSize+1, Fragment.FragmentSize+1, PixelFormat.Format32bppArgb);
			BitmapData bmpData = f.bitmap.LockBits(new Rectangle(0,0,Fragment.FragmentSize+1, Fragment.FragmentSize+1),ImageLockMode.WriteOnly,PixelFormat.Format32bppArgb);
			UInt32* ptr = (UInt32*) bmpData.Scan0.ToPointer();
			for(int y = 0;y <= Fragment.FragmentSize;y += 1)
			{
				for(int x = 0;x <= Fragment.FragmentSize;x += 4)
				{
					if (x == Fragment.FragmentSize)
					{
						*ptr = *(ptr-1);
						ptr++;
						continue;
					}
					if (y == Fragment.FragmentSize)
					{
						*ptr = *(ptr-Fragment.FragmentSize-1);ptr++;
						*ptr = *(ptr-Fragment.FragmentSize-1);ptr++;
						*ptr = *(ptr-Fragment.FragmentSize-1);ptr++;
						*ptr = *(ptr-Fragment.FragmentSize-1);ptr++;
						continue;
					}

					uint color4;
					color4 = f.data[(x + Fragment.FragmentSize*y)/4];
					//uint k = (uint)Math.Log(1/(f.srcWidth*Fragment.FragmentSize/4d),2)*16*256;
					uint k = (uint)(16*depth)*0x100;
					*ptr = 0xFF000000 + k + (color4 & 0xFF000000) / 0x01000000;ptr++;
					*ptr = 0xFF000000 + k + (color4 & 0x00FF0000) / 0x00010000;ptr++;
					*ptr = 0xFF000000 + k + (color4 & 0x0000FF00) / 0x00000100;ptr++;
					*ptr = 0xFF000000 + k + (color4 & 0x000000FF) / 0x00000001;ptr++;
				}
			}
			f.bitmap.UnlockBits(bmpData);
		}

		/*static void UpdateFragmentsRecrusivly (Fragment f, int levels)
		{
			if (f == null) return;
			UpdateFragment(f,0);
			if (levels != 0)
			{
				for(int i = 0; i < 4; i++)
					UpdateFragmentsRecrusivly (f.childs[i], levels - 1);
			}
		}*/

		void RenderFragmentsRecrusivly (Fragment f,float x, float y, float w, float h,int depht)
		{
			if (f == null) return;
			if (IsOutOfFrustum(x,y,w,h)) return;

			//if (f.bitmap == null) return;
			bool tooSmall = (w < minSize) || (h < minSize);

			/*if (!f.allTheSame)
				for(int i = 0; i < 4; i++)
					if (f.childs[i] == null)
						f.MakeChild(i);*/
			/*if (!tooSmall)
				if (!f.allTheSame)
					f.MakeChilds();*/
			
			if (f.childs[0] == null || f.childs[1] == null || f.childs[2] == null || f.childs[3] == null || tooSmall)
			{
				//g.FillRectangle(new SolidBrush (Color.Red),x,y,w,h);
				//g.DrawImage(f.bitmap ,x ,y ,w, h);
                
				Algorihtm.UpdateFragment(f,x,y,h/Fragment.FragmentSize);

				UpdateBmp(f,depht);
				g.DrawImage(f.bitmap, new RectangleF(x,y,w,h), 
					        new RectangleF(0,0,Fragment.FragmentSize,Fragment.FragmentSize),
					        GraphicsUnit.Pixel);
				//g.DrawImage(f.bitmap,
			}
			if (tooSmall) return;
			RenderFragmentsRecrusivly (f.childs[0], x      ,      y ,     w/2,     h/2, depht + 1);
			RenderFragmentsRecrusivly (f.childs[1], x + w/2,      y , w - w/2,     h/2, depht + 1);
			RenderFragmentsRecrusivly (f.childs[2], x      , y + h/2,     w/2, h - h/2, depht + 1);
			RenderFragmentsRecrusivly (f.childs[3], x + w/2, y + h/2, w - w/2, h - h/2, depht + 1);
		}

		unsafe public void Render(View v, Graphics destGraphic, int w, int h)
		{
			if (data==null) data = new TempData();

			//UpdateFragmentsRecrusivly(data.root,-1);
			g = destGraphic;
			//g.RotateTransform(45);
			g.ResetTransform();
			//g.ScaleTransform(1f/w,1f/h);
			g.Transform = v.matrix;
			transform = v.matrix;
			//g.RotateTransform(1);

			SetFrustum(v);

			g.ScaleTransform(w/2, h/2, MatrixOrder.Append);
			g.TranslateTransform(w/2, h/2, MatrixOrder.Append);
			g.InterpolationMode = InterpolationMode.Bilinear;
			//g.InterpolationMode = InterpolationMode.NearestNeighbor;

			//g.Clear(Color.White);
			//g.DrawImage(data.root.bitmap ,0 ,0 ,w, h);
			minSize = (float)(16/v.Xzoom/w);

			g.TranslateTransform(-w/2, -h/2, MatrixOrder.Append);
			//g.ScaleTransform(0.5f,0.5f, MatrixOrder.Append);
			g.TranslateTransform(w/2, h/2, MatrixOrder.Append);

			//lock(data.syncRoot)
				RenderFragmentsRecrusivly(data.root, -2, -2, 4, 4,0);

			
			g.TranslateTransform(-w/2, -h/2, MatrixOrder.Append);
			//g.ScaleTransform(0.9f,0.9f, MatrixOrder.Append);
			g.TranslateTransform(w/2, h/2, MatrixOrder.Append);
			//g.FillPolygon(new SolidBrush(Color.FromArgb(64,255,0,0)),frustum);
			//g.DrawLines(new Pen(Color.Red,0),new PointF[] {frustum[0],frustum[1],frustum[1],frustum[2],frustum[2],frustum[3],frustum[3],frustum[0]});
			
		}

		public void SetFrustum (View v)
		{
			Matrix inverse = g.Transform.Clone();
			inverse.Invert();
			frustum = new PointF[4] {new PointF(-1,-1), new PointF(-1,1), new PointF(1,1), new PointF(1,-1)};
			inverse.TransformPoints(frustum);
		}

		public static bool IsOutOfFrustum (float x, float y, float w, float h)
		{
			bool outside;

			/////////////////////////////////////////////////////
			/// Test frustum aginst box's planes
			/////////////////////////////////////////////////////
			
			// Left
			outside = true;
			for (int i = 0; i < 4; i++)
				if (frustum[i].X > x)
					outside = false;
			if (outside) return true;

			// Right
			outside = true;
			for (int i = 0; i < 4; i++)
				if (frustum[i].X < x + w)
					outside = false;
			if (outside) return true;

			// Top
			outside = true;
			for (int i = 0; i < 4; i++)
				if (frustum[i].Y > y)
					outside = false;
			if (outside) return true;

			// Buttom
			outside = true;
			for (int i = 0; i < 4; i++)
				if (frustum[i].Y < y + h)
					outside = false;
			if (outside) return true;

			////////////////////////////////////////
			/// Alternate culling 
			////////////////////////////////////////
			PointF[] coner = new PointF[] {new PointF(x,y), new PointF(x+w,y), new PointF(x,y+h), new PointF(x+w,y+h)};
			//PointF[] midpoint = new PointF[] {new PointF(x+w/2,y+h/2)};
			transform.TransformPoints(coner);

			// Left
			outside = true;
			for (int i = 0; i < 4; i++)
				if (coner[i].X > -1)
					outside = false;
			if (outside) return true;

			// Right
			outside = true;
			for (int i = 0; i < 4; i++)
				if (coner[i].X < 1)
					outside = false;
			if (outside) return true;

			// Top
			outside = true;
			for (int i = 0; i < 4; i++)
				if (coner[i].Y > -1)
					outside = false;
			if (outside) return true;

			// Buttom
			outside = true;
			for (int i = 0; i < 4; i++)
				if (coner[i].Y < 1)
					outside = false;
			if (outside) return true;


			// Approximatly inside
			return false;
		}
	}
}
