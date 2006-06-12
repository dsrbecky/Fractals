using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Fractals
{
	public class Algorihtm
	{
		public const int UpdateRate = 250;

		public static DateTime lastUpdate;

		public static float minSize;

//		public static TempData d;

		public static EventHandlerNoArg _Update;





		/*static void UpdateFragmentsRecrusivly (Fragment f, int levels, double x, double y, double size)
		{
			if (f == null) return;
			UpdateFragment(f,x,y,size);
			if (levels != 0)
			{
				UpdateFragmentsRecrusivly (f.childs[0], levels - 1, x, y, size/2);
				UpdateFragmentsRecrusivly (f.childs[1], levels - 1, x + size*(Fragment.FragmentSize/2), y, size/2);
				UpdateFragmentsRecrusivly (f.childs[2], levels - 1, x, y + size*(Fragment.FragmentSize/2), size/2);
				UpdateFragmentsRecrusivly (f.childs[3], levels - 1, x + size*(Fragment.FragmentSize/2), y + size*(Fragment.FragmentSize/2), size/2);
			}
		}*/

		/*static void AddChildsRecrusivly (Fragment f,int obligations, double x, double y, double size)
		{
			if (f.allTheSame && obligations <= 0) return;
			//if (size < minSize) return;
			//if (Rendrer.IsOutOfFrustum((float)x,(float)y,(float)size,(float)size)) return;

			for(int i = 0; i < 4; i++)
				if (f.childs[i] == null)
					f.MakeChild(i);
				else
				{
					//	AddChildsRecrusivly(f.childs[i], obligations - 1,0,0,0);
					switch (i)
					{
						case 0:AddChildsRecrusivly (f.childs[0], obligations - 1, x, y, size/2);break;
						case 1:AddChildsRecrusivly (f.childs[1], obligations - 1, x + size*(Fragment.FragmentSize/2), y, size/2);break;
						case 2:AddChildsRecrusivly (f.childs[2], obligations - 1, x, y + size*(Fragment.FragmentSize/2), size/2);break;
						case 3:AddChildsRecrusivly (f.childs[3], obligations - 1, x + size*(Fragment.FragmentSize/2), y + size*(Fragment.FragmentSize/2), size/2);break;
					}
				}
		}*/

		/*public static unsafe void CalcImage(Bitmap bmp,object BmpSyncRoot,View v,EventHandlerNoArg Update)
		{
			DateTime begin;
			DateTime beginTotal = DateTime.Now;
			Console.WriteLine("Rendring...");
			begin = DateTime.Now;

			_Update = Update;
			return;
			d = new TempData();
			return;

			UpdateFragment(d.root,-2,-2,4d/Fragment.FragmentSize);

			//for(int depth = 1;;depth++)
			for(int depth = 1;depth <= 11;depth++)
			{
				minSize = (float)(32/MainForm.setDlg.view.Xzoom/150);

				Console.WriteLine(" - subdivision level " + (depth).ToString());
				Console.WriteLine("   - Adding childs");
				lock (d.syncRoot)
					AddChildsRecrusivly(d.root, depth - 5,-2,-2,4);
				Console.WriteLine("     - fragments: " + (d.root.Count()).ToString());
				Console.WriteLine("     - used memory: " + (d.root.SizeOf()).ToString());
				Console.WriteLine("   - Updating");
				lock (d.syncRoot)
					UpdateFragmentsRecrusivly(d.root,-1,-2,-2,4d/Fragment.FragmentSize);
                Console.WriteLine("   - Rendring");
				Update();
				//System.Threading.Thread.Sleep(1000);
				Console.WriteLine("   * done in: " + ((DateTime.Now.Ticks-begin.Ticks)/10000).ToString() + "ms");
				begin = DateTime.Now;
			}
			Console.WriteLine(" * done in: " + ((DateTime.Now.Ticks-beginTotal.Ticks)/10000).ToString() + "ms");
		}*/
	}
}