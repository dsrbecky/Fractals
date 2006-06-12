using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Fractals
{
	public class Algorihtm
	{
		public delegate void dlgtGetColor(double p, double q,out double r,out double g,out double b);

		static byte GetIndex(double p, double q)
		{
			double r,g,index;
			MainForm.setDlg.GetColorDelegate (p,q,out r,out g,out index);
			return (byte) Math.Min(255,Math.Max(0,index));
		}
		
		static uint GetColor4(double p, double q, double width)
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

		static void UpdateFragment(Fragment f)
		{
			for(int y = 0;y < Fragment.FragmentSize;y += 1)
			{
				for(int x = 0;x < Fragment.FragmentSize;x += 4)
				{
					f.data[(x + Fragment.FragmentSize*y)/4] = 
						GetColor4(f.srcX+x*f.srcWidth,
						          f.srcY+y*f.srcWidth,
						          f.srcWidth);
				}
			}
			f.done = 0xFFFF;
		}

		static void UpdateFragmentsRecrusivly (Fragment f, int levels)
		{
			if (f == null) return;
			UpdateFragment(f);
			if (levels != 0)
			{
				UpdateFragmentsRecrusivly (f.childLeftTop , levels - 1);
				UpdateFragmentsRecrusivly (f.childRightTop , levels - 1);
				UpdateFragmentsRecrusivly (f.childLeftButtom , levels - 1);
				UpdateFragmentsRecrusivly (f.childRightButtom , levels - 1);
			}
		}

		static void AddChildsRecrusivly (Fragment f)
		{
			if (f.childLeftTop == null)
				f.MakeChildLT();
			else
				AddChildsRecrusivly(f.childLeftTop);

			if (f.childLeftTop == null)
				f.MakeChildLT();
			else
				AddChildsRecrusivly(f.childLeftTop);

			if (f.childLeftTop == null)
				f.MakeChildLT();
			else
				AddChildsRecrusivly(f.childLeftTop);

			if (f.childLeftTop == null)
				f.MakeChildLT();
			else
				AddChildsRecrusivly(f.childLeftTop);
		}


		public static unsafe void CalcImage(Bitmap bmp,object BmpSyncRoot,View v,EventHandlerNoArg Updated)
		{
			DateTime begin;
			DateTime beginTotal = DateTime.Now;
			Console.WriteLine("Rendring...");
			begin = DateTime.Now;

			int w,h;
			lock (BmpSyncRoot)
			{
				w = bmp.Width;
				h = bmp.Height;
			}

			TempData d = new TempData();
			Rendrer sr = new Rendrer(d,w,h);
			int depth = 0;

			for(;;)
			{
				Console.WriteLine(" - level " + depth.ToString());

				UpdateFragment(d.root);
				d.root.MakeChilds();
				UpdateFragmentsRecrusivly(d.root,-1);

				{ // Update display
					lock (BmpSyncRoot)
						Graphics.FromImage(bmp).DrawImage(sr.GetBitmap(v),0,0);
					Updated();
				}				

				Console.WriteLine(" (done in " + ((DateTime.Now.Ticks-begin.Ticks)/10000).ToString() + "ms)");
				begin = DateTime.Now;
			}
			Console.WriteLine(" (tatal time: " + ((DateTime.Now.Ticks-beginTotal.Ticks)/10000).ToString() + "ms)");
		}
	}
}