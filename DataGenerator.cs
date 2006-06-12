using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Fractals
{
	public class DataGenerator
	{
		public delegate void dlgtGetIndex(double p, double q,out double r,out double g,out double b);

		dlgtGetIndex _GetIndex;

		// Data identifiers
		public Fragment root = new Fragment();
		public double minX = -2d;
		public double minY = -2d;
		public double size = 4;
		public bool hasExPalette = false;  // TRUE - data use 2 bytes; FALSE - data use 1 byte

		// Temp rendring data
		Graphics g;
		PointF[] frustum = new PointF[4];
		Matrix transform = new Matrix();

		double zoom = 1;

		double minSize;

		public DataGenerator(dlgtGetIndex functionGetIndex)
		{
			_GetIndex = functionGetIndex;
		}

		byte GetIndex(double p, double q)
		{
			double r,g,index;
			_GetIndex (p,q,out r,out g,out index);
			return (byte) Math.Min(255,Math.Max(0,index));
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
					if (color4 != first) f.allTheSame = false;
				}
			}
			f.done = 0xFFFF;

			// Update
		/*	if ((DateTime.Now.Ticks - lastUpdate.Ticks)/10000 > UpdateRate)
			{
				//_Update();
				lastUpdate = DateTime.Now;
			}*/
		}

		unsafe void UpdateBmp(Fragment f, int depth)
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

		void RenderFragmentsRecrusivly (Fragment f,double x, double y, double size,int depht)
		{
			if (f == null) return;
			if (IsOutOfFrustum(x,y,size)) return;

			//if (f.bitmap == null) return;
			bool tooSmall = (size < minSize);

			/*if (!f.allTheSame)
				for(int i = 0; i < 4; i++)
					if (f.childs[i] == null)
						f.MakeChild(i);*/
			UpdateFragment(f,x,y,size/Fragment.FragmentSize);
			if (!f.allTheSame)
				if (!tooSmall)
					for(int i = 0; i < 4; i++)
						if (f.childLT == null)
							f.MakeChilds();
			
			if (f.childLT == null || f.childRT == null || f.childLB == null || f.childRB == null || tooSmall)
			{
				//g.FillRectangle(new SolidBrush (Color.Red),x,y,w,h);
				//g.DrawImage(f.bitmap ,x ,y ,w, h);
                
				UpdateFragment(f,x,y,size/Fragment.FragmentSize);

				UpdateBmp(f,depht);
				//UpdateBmp(root,depht);
//				g.FillRectangle(new SolidBrush(Color.Purple), new RectangleF((float)(x*zoom),(float)(y*zoom),(float)(size*zoom)/2,(float)(size*zoom)/2));
				g.DrawImage(f.bitmap, new RectangleF((float)(x*zoom),(float)(y*zoom),(float)(size*zoom),(float)(size*zoom)), 
							new RectangleF(0,0,Fragment.FragmentSize,Fragment.FragmentSize),
							GraphicsUnit.Pixel);
			}
			if (tooSmall) return;
			RenderFragmentsRecrusivly (f.childLT, x         , y         , size/2, depht + 1);
			RenderFragmentsRecrusivly (f.childRT, x + size/2, y         , size/2, depht + 1);
			RenderFragmentsRecrusivly (f.childLB, x         , y + size/2, size/2, depht + 1);
			RenderFragmentsRecrusivly (f.childRB, x + size/2, y + size/2, size/2, depht + 1);
		}

		public void Render(View v, Graphics destGraphic, int w, int h)
		{
			g = destGraphic;
/*			g.ResetTransform();
			g.Clear(Color.White);
			g.ScaleTransform((float)v.Xzoom,(float)v.Xzoom);
			UpdateFragment(root,-2d,-2d,4d/Fragment.FragmentSize);
			UpdateBmp(root,0);
			g.DrawImage(root.bitmap, new RectangleF(0,0,(float)(100f/(float)v.Xzoom),(float)(100f/(float)v.Xzoom)), 
						new RectangleF(0,0,Fragment.FragmentSize,Fragment.FragmentSize),
						GraphicsUnit.Pixel);
			g.FillRectangle(new SolidBrush(Color.Purple),new RectangleF(0,(float)(150f/(float)v.Xzoom),(float)(100f/(float)v.Xzoom),(float)(100f/(float)v.Xzoom)));

			return;*/



			transform = v.matrix;
			g.ResetTransform();
			//g.Transform = v.matrix;

			//g.TranslateTransform(-w/2, -h/2, MatrixOrder.Append);
			g.ScaleTransform(1/(float)v.Xzoom,1/(float)v.Yzoom, MatrixOrder.Append);
			zoom = v.Xzoom;
			//g.TranslateTransform(w/2, h/2, MatrixOrder.Append);

            g.MultiplyTransform (v.matrix, MatrixOrder.Append);

			g.InterpolationMode = InterpolationMode.Bilinear;

			SetFrustum(v);

			// From [-1,1] to [0,w]
			g.ScaleTransform(w/2, h/2, MatrixOrder.Append);
			g.TranslateTransform(w/2, h/2, MatrixOrder.Append);
			
			g.Clear(Color.White);

			minSize = (double)(16/v.Xzoom/w);

			/*g.TranslateTransform(-w/2, -h/2, MatrixOrder.Append);
			g.ScaleTransform(0.5f,0.5f, MatrixOrder.Append);
			g.TranslateTransform(w/2, h/2, MatrixOrder.Append);*/

			RenderFragmentsRecrusivly(root, -2f, -2f, 4,0);

			/*
			g.TranslateTransform(-w/2, -h/2, MatrixOrder.Append);
			g.ScaleTransform(0.9f,0.9f, MatrixOrder.Append);
			g.TranslateTransform(w/2, h/2, MatrixOrder.Append);
			//g.FillPolygon(new SolidBrush(Color.FromArgb(64,255,0,0)),frustum);
			g.DrawLines(new Pen(Color.Red,0),new PointF[] {frustum[0],frustum[1],frustum[1],frustum[2],frustum[2],frustum[3],frustum[3],frustum[0]});
			*/
			
		}

		void SetFrustum (View v)
		{
			Matrix inverse = transform.Clone();
			inverse.Invert();
			frustum = new PointF[4] {new PointF(-1,-1), new PointF(-1,1), new PointF(1,1), new PointF(1,-1)};
			inverse.TransformPoints(frustum);
		}

		bool IsOutOfFrustum (double x, double y, double size)
		{
			//return false;
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
				if (frustum[i].X < x + size)
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
				if (frustum[i].Y < y + size)
					outside = false;
			if (outside) return true;
			
			// Approximatly inside
			return false;
/*
			////////////////////////////////////////
			/// Test box aginst frustum planes
			////////////////////////////////////////
			PointF[] coner = new PointF[] {new PointF(x,y), new PointF(x+size,y), new PointF(x,y+size), new PointF(x+size,y+size)};
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
			return false;*/
		}
	}
}
