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
        Bitmap emptyBitmap = new Bitmap(Fragment.FragmentSize+1, Fragment.FragmentSize+1, PixelFormat.Format32bppRgb);

        public bool debugMode = false;

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

        ImageAttributes imageAttributes = new ImageAttributes();

        const int fragmentsToDisponseCount = 0;//10000;
        System.Collections.Queue fragmentsToDisponse = new System.Collections.Queue(fragmentsToDisponseCount * 4);

        //public Fragment root = new Fragment();
		//public double minX = -2d;
		//public double minY = -2d;
		//public double size = 4;
		//public bool hasExPalette = false;  // TRUE - data use 2 bytes; FALSE - data use 1 byte

		public DataGenerator(dlgtGetIndex functionGetIndex, Color[] palette)
		{
			_GetIndex = functionGetIndex;
            this.palette = new Color[256];
            for (int i = 0; i < 256; i++) this.palette[i] = Color.Black;
            palette.CopyTo(this.palette, 0);
            emptyBitmap.SetPixel(0,0,Color.Red);
            emptyBitmap.SetPixel(1,1,Color.Green);
            emptyBitmap.SetPixel(2,2,Color.Blue);
            emptyBitmap.SetPixel(3,3,Color.White);

            imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
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
                    //if (Math.Abs(color4 - first) > 5) f.allTheSame = false;
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

			f.bitmap = new Bitmap(Fragment.FragmentSize, Fragment.FragmentSize, PixelFormat.Format32bppRgb);
			BitmapData bmpData = f.bitmap.LockBits(new Rectangle(0,0,Fragment.FragmentSize, Fragment.FragmentSize),ImageLockMode.WriteOnly,PixelFormat.Format32bppArgb);
			//f.bitmap = new Bitmap(Fragment.FragmentSize+1, Fragment.FragmentSize+1, PixelFormat.Format32bppRgb);
			//BitmapData bmpData = f.bitmap.LockBits(new Rectangle(0,0,Fragment.FragmentSize+1, Fragment.FragmentSize+1),ImageLockMode.WriteOnly,PixelFormat.Format32bppArgb);
			UInt32* ptr = (UInt32*) bmpData.Scan0.ToPointer();
			for(int y = 0;y < Fragment.FragmentSize;y += 1)
			{
				for(int x = 0;x < Fragment.FragmentSize;x += 1)
				{
                    uint k;
                    if (debugMode) {
                        k = (uint)(16*depth)*0x100;
                    } else {
                        k = 0;
                    }

                    *ptr = k + (uint)GetColorAA(f, x, y, 0).ToArgb();ptr++;
					/*if (x == Fragment.FragmentSize)
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
					}*/

					/*uint color4;
					color4 = f.data[(x + Fragment.FragmentSize*y)/4];
					uint k;
                    if (debugMode) {
                        k = (uint)(16*depth)*0x100;
                    } else {
                        k = 0;
                    }

                    *ptr = k + (uint)palette[(color4 & 0xFF000000) / 0x01000000].ToArgb();ptr++;
					*ptr = k + (uint)palette[(color4 & 0x00FF0000) / 0x00010000].ToArgb();ptr++;
					*ptr = k + (uint)palette[(color4 & 0x0000FF00) / 0x00000100].ToArgb();ptr++;
					*ptr = k + (uint)palette[(color4 & 0x000000FF) / 0x00000001].ToArgb();ptr++;*/

                    /*
                    *ptr = 0xFF000000 + k + (color4 & 0xFF000000) / 0x01000000;ptr++;
					*ptr = 0xFF000000 + k + (color4 & 0x00FF0000) / 0x00010000;ptr++;
					*ptr = 0xFF000000 + k + (color4 & 0x0000FF00) / 0x00000100;ptr++;
					*ptr = 0xFF000000 + k + (color4 & 0x000000FF) / 0x00000001;ptr++;
					*/
				}
			}
			f.bitmap.UnlockBits(bmpData);
            fragmentsToDisponse.Enqueue(f);
		}

        Color GetColorAA(Fragment f, int x, int y, int levelsOfAA)
        {
            if (levelsOfAA <= 0) {
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
            if (srcF != null) {
                Color color1 = GetColorAA(srcF, srcX + 0, srcY + 0, levelsOfAA - 1);
                Color color2 = GetColorAA(srcF, srcX + 0, srcY + 1, levelsOfAA - 1);
                Color color3 = GetColorAA(srcF, srcX + 1, srcY + 0, levelsOfAA - 1);
                Color color4 = GetColorAA(srcF, srcX + 1, srcY + 1, levelsOfAA - 1);

                int r = (color1.R + color2.R + color3.R + color4.R) / 4;
                int g = (color1.G + color2.G + color3.G + color4.G) / 4;
                int b = (color1.B + color2.B + color3.B + color4.B) / 4;

                return Color.FromArgb(255,g,b);
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

        long     timeToAbort;
        bool     abortAllowed;

        double   initalSize;
        double   terminalSize;

        bool     simulation;
        Graphics g;

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

            bool lastRecrusion = (renderSize <= terminalSize);
            bool skipDrawing   = (renderSize > initalSize);

            UpdateFragment(f, dataX, dataY, dataSize / Fragment.FragmentSize);
            if (!f.allTheSame || (renderSize > 8*terminalSize))
                if (!lastRecrusion)
                    if (!f.HasAllChilds)
                        f.MakeChilds();

            if ((!f.HasAllChilds || lastRecrusion || abortAllowed) && !skipDrawing)
            {
                UpdateBmp(f, depht);
                if (simulation == false) {
                    g.DrawImage(/*emptyBitmap,//*/f.bitmap,
                                //new RectangleF((float)renderX, (float)renderY, (float)renderSize, (float)renderSize),
                                new PointF[] {new PointF( (float)renderX, (float)renderY ),
                                              new PointF( (float)renderX + (float)renderSize, (float)renderY ),
                                              new PointF( (float)renderX, (float)renderY + (float)renderSize )},
                                new RectangleF(0, 0, Fragment.FragmentSize, Fragment.FragmentSize),
                                GraphicsUnit.Pixel,
                                imageAttributes);
                    //g.DrawRectangle(Pens.Blue,(float)renderX, (float)renderY, (float)renderSize, (float)renderSize);

                    //System.Diagnostics.Debug.WriteLine(fragmentsToDisponse.Count.ToString());
                    while (fragmentsToDisponse.Count > fragmentsToDisponseCount) {
                        Fragment fragment = (Fragment)fragmentsToDisponse.Dequeue();
                        fragment.bitmap.Dispose();
                        fragment.bitmap = null;
                    }
                }
            }
            if (lastRecrusion) return;
            RenderFragmentsRecrusivly (f.childLT, dataX              , dataY              , dataSize/2, renderX                , renderY                , renderSize/2, depht + 1, !completlyInside);
			RenderFragmentsRecrusivly (f.childRT, dataX + dataSize/2 , dataY              , dataSize/2, renderX + renderSize/2 , renderY                , renderSize/2, depht + 1, !completlyInside);
			RenderFragmentsRecrusivly (f.childLB, dataX              , dataY + dataSize/2 , dataSize/2, renderX                , renderY + renderSize/2 , renderSize/2, depht + 1, !completlyInside);
			RenderFragmentsRecrusivly (f.childRB, dataX + dataSize/2 , dataY + dataSize/2 , dataSize/2, renderX + renderSize/2 , renderY + renderSize/2 , renderSize/2, depht + 1, !completlyInside);
		}

        static Bitmap tmpImage = new Bitmap(2048, 1024);
        static Graphics tmpG = Graphics.FromImage(tmpImage);

        double compulsorySize = 1 / 1d;

        long startTime;

        public long Render(View v, Graphics destGraphics, int w, int h, double FPS)
		{
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
                abortAllowed = false;
                initalSize   = double.MaxValue;
                terminalSize = compulsorySize;

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

                /// Multiple passes to incerase quality
                /// ----------------------------------
                while (finishedInTime && !abortFlag)
                {
                    abortAllowed = true;
                    initalSize = terminalSize; // Start where we finished
                    terminalSize = initalSize / 2d; // Do one more level

                    RenderFragmentsRecrusivly(data.root,
                                        data.minX, data.minY, data.size,
                                        (-v.Xpos + data.minX) * v.Xzoom, (-v.Ypos + data.minY) * v.Xzoom, 2 * v.Xzoom * data.size / 2,
                                        0, true);

                    finishedInTime = GetTicks() < timeToAbort;

                    if (finishedInTime && FPS != 0)
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
            System.Diagnostics.Debug.WriteLine("Time to draw: " + miliseconds.ToString() +
                                               " quality " + (1d/compulsorySize).ToString());
            
            //destGraphics.DrawImage(tmpImage,0,0);

            return miliseconds;
        }

        bool IsOutOfFrustum(double x, double y, double size, PointF[] frustum)
        {
            return false;
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
