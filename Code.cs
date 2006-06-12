using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Fractals
{
	class Main
	{
		public static unsafe void CalcImage(Bitmap bmp,object BmpSyncRoot,View v,EventHandlerNoArg Updated)
		{
			Console.WriteLine("Rendring...");

			int UpdateRate = 5000;
			int UpdateCounter = 0;
			bool redrawFilterOn = false;

			int w,h;
			lock (BmpSyncRoot)
			{
				w = bmp.Width;
				h = bmp.Height;
			}

			byte[,,] data = new byte[h,w,4];

			for(int boxsize = 8; boxsize >= 1;boxsize /= 2)
			{
				Console.WriteLine(" - level "+boxsize.ToString());
				for(int y = 0;y < h;y += boxsize)
					for(int x = 0;x < w;x += boxsize)
					{
						if ((x % (boxsize*2) == 0) && 
							(y % (boxsize*2) == 0) &&
							redrawFilterOn) continue;
						double r,g,b;
						GetColor( 
							v.makeX(x, w),
							v.makeY(y, h),
							out r, out g, out b
							);
						for(int by=y; by < Math.Min(y+boxsize,h); by ++)
							for(int bx=x; bx < Math.Min(x+boxsize,w); bx++)
							{
								data[by,bx,3] = 255;
								data[by,bx,2] = (byte) Math.Min(255,Math.Max(0,r));
								data[by,bx,1] = (byte) Math.Min(255,Math.Max(0,g));
								data[by,bx,0] = (byte) Math.Min(255,Math.Max(0,b));
							}

						UpdateCounter++;
						if (UpdateCounter >= UpdateRate)
						{
							Bitmap newbmp;
							fixed(byte* buffer = data)
								newbmp = new Bitmap(w,h,w*4,PixelFormat.Format32bppArgb, (IntPtr)buffer);
							lock (BmpSyncRoot)
								Graphics.FromImage(bmp).DrawImage(newbmp,0,0);
							UpdateCounter = 0;
							Updated();
						}
				}
				redrawFilterOn = true;

				UpdateCounter++;
				if (UpdateCounter >= UpdateRate)
				{
					Bitmap newbmp;
					fixed(byte* buffer = data)
						newbmp = new Bitmap(w,h,w*4,PixelFormat.Format32bppArgb, (IntPtr)buffer);
					lock (BmpSyncRoot)
						Graphics.FromImage(bmp).DrawImage(newbmp,0,0);
					UpdateCounter = 0;
					Updated();
				}
			}

			if (v.AA == 1) return;

			Console.WriteLine(" - antialiasing");
			Console.WriteLine("   - setting flag tables");
			// Set doAA
			bool[,] AAdone = new bool[h,w];
			bool[,] checkAA = new bool[h,w];
			for(int y = 0;y < h;y++)
				for(int x = 0;x < w;x++)
				{
					checkAA[y,x] = true;
					AAdone[y,x] = false;
				}

			Console.WriteLine("   - performing AA");
			// Update where doAA = true
			for(int y = 0;y < h;y++)
				for(int x = 0;x < w;x++)
					if (checkAA[y,x] && (x < w-1) && (y < h-1))
					{
						if ((data[y,x,0] != data[y+0,x+1,0]) ||
							(data[y,x,0] != data[y+1,x+0,0]) ||
							(data[y,x,0] != data[y+1,x+1,0]) ||
							(data[y,x,1] != data[y+0,x+1,1]) ||
							(data[y,x,1] != data[y+1,x+0,1]) ||
							(data[y,x,1] != data[y+1,x+1,1]) ||
							(data[y,x,2] != data[y+0,x+1,2]) ||
							(data[y,x,2] != data[y+1,x+0,2]) ||
							(data[y,x,2] != data[y+1,x+1,2]) )
						{
							checkAA[y,x] = false;							
							if (x > 0)
							{
								checkAA[y,x-1] = true;
								checkAA[y+1,x-1] = true;
							}
							if (y > 0)
							{
								checkAA[y-1,x] = true;
								checkAA[y-1,x+1] = true;
							}
							if (x > 0 && y > 0)
								checkAA[y-1,x-1] = true;
							checkAA[y,x+1] = true;
							checkAA[y+1,x] = true;
							checkAA[y+1,x+1] = true;

							bool backtracing;
							backtracing = false;
							for (int Y=0;Y<=1;Y++)
								for (int X=0;X<=1;X++)
									if (!AAdone[y+Y,x+X])
									{
										double aar,aag,aab;
										double r,g,b;
										aar=0;aag=0;aab=0;
										for (int aaX=0;aaX<v.AA;aaX++)
											for (int aaY=0;aaY<v.AA;aaY++)
											{
												GetColor( 
													v.makeX(((x+X)*v.AA) + aaX, w*v.AA),
													v.makeY(((y+Y)*v.AA) + aaY, h*v.AA),
													out r, out g, out b
													);
												aar += ((Math.Min(255,Math.Max(0,r))/v.AA)/v.AA);
												aag += ((Math.Min(255,Math.Max(0,g))/v.AA)/v.AA);
												aab += ((Math.Min(255,Math.Max(0,b))/v.AA)/v.AA);

												UpdateCounter++;
												if (UpdateCounter >= UpdateRate)
												{
													Bitmap newbmp;
													fixed(byte* buffer = data)
														newbmp = new Bitmap(w,h,w*4,PixelFormat.Format32bppArgb, (IntPtr)buffer);
													lock (BmpSyncRoot)
														Graphics.FromImage(bmp).DrawImage(newbmp,0,0);
													UpdateCounter = 0;
													Updated();
												}
											}

										data[y+Y,x+X,3] = 255;
										data[y+Y,x+X,2] = (byte) Math.Min(255,Math.Max(0,aar));
										data[y+Y,x+X,1] = (byte) Math.Min(255,Math.Max(0,aag));
										data[y+Y,x+X,0] = (byte) Math.Min(255,Math.Max(0,aab));

										AAdone[y+Y,x+X] = true;
										backtracing = true;
									} // AA pass
							if (backtracing)
							{
								if (x != 0) x--;
								if (y != 0) y--;
							}
						}// If - Color check
					} // if (checkAA[y,x] && (x < w-1) && (y < h-1))

			Console.WriteLine("   - AA done");

			{
				Bitmap newbmp;
				fixed(byte* buffer = data)
					newbmp = new Bitmap(w,h,w*4,PixelFormat.Format32bppArgb, (IntPtr)buffer);
				lock (BmpSyncRoot)
					Graphics.FromImage(bmp).DrawImage(newbmp,0,0);
				UpdateCounter = 0;
				Updated();
			}
			Console.WriteLine("Finished!");
		}
		//<CODE GOES HERE>//

	}
}