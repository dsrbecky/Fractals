using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Fractals
{
	class Main
	{
		public static int UpdateRate = 100;
		public static int AAUpdateRate = 10;
		public static bool SmartAA = true;
		public static int SmartAArange = 2;
		public static int SmartAApasses = 50;
		public static int UpdateCounter = 0;

		public static unsafe void CalcImage(Bitmap bmp,View v,Form form)
		{
			UpdateCounter = 0;
			bool redrawFilterOn = false;
			BitmapData bitmapData;
			byte* data;
			int w,h;
			lock (bmp)
			{
				w = bmp.Width;
				h = bmp.Height;
			}
			for(int boxsize = 8; boxsize >= 1;boxsize /= 2)
			{
				for(int y = 0;y < h;y += boxsize)
					lock(bmp) 
					{
						int strip=Math.Min((h-y),boxsize);
						bitmapData = bmp.LockBits(new Rectangle(0,y,w,strip),ImageLockMode.WriteOnly,PixelFormat.Format32bppArgb);
						data = (byte*)bitmapData.Scan0;
						for(int x = 0;x < w;x += boxsize)
						{
							if ((x % (boxsize*2) == 0) && 
								(y % (boxsize*2) == 0) &&
								redrawFilterOn) continue;
							double r,g,b;
							r=0;g=0;b=0;
							GetColor( 
								v.makeX(x, w),
								v.makeY(y, h),
								out r, out g, out b
								);
							for(int bx=x; bx < Math.Min(x+boxsize,w); bx++)
								for(int by=0; by < strip; by ++)
								{
									data[(bx+by*w)*4+3] = 255;
									data[(bx+by*w)*4+2] = (byte) Math.Min(255,Math.Max(0,r));
									data[(bx+by*w)*4+1] = (byte) Math.Min(255,Math.Max(0,g));
									data[(bx+by*w)*4+0] = (byte) Math.Min(255,Math.Max(0,b));
								}
						}
						bmp.UnlockBits(bitmapData);
						UpdateCounter++;
						if (UpdateCounter >= UpdateRate)
						{
							UpdateCounter = 0;
							lock (bmp)
							{
								bitmapData = bmp.LockBits(new Rectangle(0,0,w,h),ImageLockMode.WriteOnly,PixelFormat.Format32bppArgb);
								bmp.UnlockBits(bitmapData);
							}
							form.Invalidate();
						}
					}
				redrawFilterOn = true;
			}

			if (v.AAX == 1 && v.AAY == 1) return;

			Console.WriteLine("Set bool[,]s");
			// Set doAA
			bool[,] doAA = new bool[w,h];
			bool[,] AAdone = new bool[w,h];
			for(int y = 0;y < h;y++)
				for(int x = 0;x < w;x++)
				{
					doAA[x,y] = false;
					AAdone[x,y] = false;
				}

			for (int AApass = 0; AApass < SmartAApasses;AApass++)
			{
				Console.WriteLine("Check if need AA");
				// Set doAA tag if AA reqired
				lock(bmp) 
				{
					bitmapData = bmp.LockBits(new Rectangle(0,0,w,h),ImageLockMode.ReadOnly,PixelFormat.Format32bppArgb);
					data = (byte*)bitmapData.Scan0;
					for(int y = 0;y < h;y++)
						for(int x = 0;x < w;x++)
							if (AAdone[x,y] == false)
							{
								for (int tx=Math.Max(0,x-SmartAArange);tx<=Math.Min(w-1,x+SmartAArange);tx++)
									for (int ty=Math.Max(0,y-SmartAArange);ty<Math.Min(h-1,y+SmartAArange);ty++)
										for (int tc=0;tc<3;tc++)
										{
											if (Math.Abs((int)data[(w*ty+tx)*4+tc] - (int)data[(w*y+x)*4+tc]) > 2)
												doAA[x,y] = true;
										}
							}
					bmp.UnlockBits(bitmapData);
				}

				Console.WriteLine("Do AA where needed");
				// Update where doAA = true
				for(int y = 0;y < h;y++)
					lock(bmp) 
					{
						bitmapData = bmp.LockBits(new Rectangle(0,y,w,1),ImageLockMode.WriteOnly,PixelFormat.Format32bppArgb);
						data = (byte*)bitmapData.Scan0;
						for(int x = 0;x < w;x++)
						{
							double aar=0,aag=0,aab=0;
							double r,g,b;
		
							if (doAA[x,y] && AAdone[x,y] == false)
							{
								aar=0;aag=0;aab=0;
								for (int aaX=0;aaX<v.AAX;aaX++)
									for (int aaY=0;aaY<v.AAY;aaY++)
									{
										r = 0; g = 0; b = 0;
										GetColor( 
											v.makeX((x*v.AAX) + aaX, w*v.AAX),
											v.makeY((y*v.AAY) + aaY, h*v.AAY),
											out r, out g, out b
											);
										aar += ((Math.Min(255,Math.Max(0,r))/v.AAX)/v.AAY);
										aag += ((Math.Min(255,Math.Max(0,g))/v.AAX)/v.AAY);
										aab += ((Math.Min(255,Math.Max(0,b))/v.AAX)/v.AAY);
									}

								data[x*4+3] = 255;
								data[x*4+2] = (byte) Math.Min(255,Math.Max(0,aar));
								data[x*4+1] = (byte) Math.Min(255,Math.Max(0,aag));
								data[x*4+0] = (byte) Math.Min(255,Math.Max(0,aab));

/*															data[x*4+2] = 0;
															data[x*4+1] = 0;
															data[x*4+0] = 0;

*/								AAdone[x,y] = true;
							}
						}
						bmp.UnlockBits(bitmapData);
						UpdateCounter++;
						if (UpdateCounter >= AAUpdateRate)
						{
							UpdateCounter = 0;
							form.Invalidate();
						}
					}
			}
			Console.WriteLine("AA done");

		}

		//<CODE GOES HERE>//

	}
}