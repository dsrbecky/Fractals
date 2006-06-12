using System;
using System.Drawing;

namespace Fractals
{
	public unsafe class Fragment
	{
		public const int FragmentSize = 16;
		// Memory usage: FragmentSize*FragmentSize*[2-ExPalette]

		//public Fragment[] childs = new Fragment[4];

		public Fragment childLT;
		public Fragment childRT;
		public Fragment childLB;
		public Fragment childRB;

		// NOTE: Fragmet always has half the area of parent
		// NOTE: Fist byte in chunk is always copy of parent byte (+25% memory usage)

		public uint[] data;
		public ushort done = 0; // map of 4x4 done bits
		// public int depth = 0; // -1 means [-2,2]; 0 means [-1,1]; 1 means [-0.5,0.5]; etc;
		public Bitmap bitmap;

		public bool allTheSame;

		//public double srcX;
		//public double srcY;
		//public double srcWidth;

		//public Fragment parent = null;

	/*	public int SizeOf()
		{
			int result = 0;
			result += 7*sizeof(int);
			result += sizeof(ushort);
			result += 2*sizeof(bool);
			result += 3*sizeof(double);
			//result += FragmentSize*FragmentSize/(hasExPalette?2:4)*sizeof(uint);
			result += FragmentSize*FragmentSize/(4)*sizeof(uint);
			for(int i = 0; i < 4; i++)
				if (childs[i] != null)
					result += childs[i].SizeOf();
			return result;
		}*/

		/*public Fragment(bool _hasExPalette,double _srcX,double _srcY,double _srcWidth)
		{
			hasExPalette = _hasExPalette;
			srcX = _srcX;
			srcY = _srcY;
			srcWidth = _srcWidth;
			
			data = new UInt32[FragmentSize*FragmentSize/(hasExPalette?2:4)];
		}*/

		public Fragment()//bool hasExPalette)
		{
			//hasExPalette = _hasExPalette;
		
			data = new UInt32[FragmentSize*FragmentSize/4];
		}

		public void MakeChilds()
		{
			MakeChildLT();
			MakeChildRT();
			MakeChildLB();
			MakeChildRB();
		}

        public bool HasAllChilds
        {
            get
            {
                return childLT != null && childRT != null && childLB != null && childRB != null;
            }
        }

        /*public void MakeChild(int i)
		{
			//childs[i] = new Fragment(hasExPalette);
			// TODO - lame
			childs[i] = new Fragment(false);
		}*/


		public void MakeChildLT()
		{childLT = new Fragment();}

		public void MakeChildRT()
		{childRT = new Fragment();}

		public void MakeChildLB()
		{childLB = new Fragment();}

		public void MakeChildRB()
		{childRB = new Fragment();}
	
	}

	public class TempData
	{
		public Fragment root;
		public double minX;
		public double minY;
		public double size;
		public bool hasExPalette;  // TRUE - data use 2 bytes; FALSE - data use 1 byte
		public object syncRoot;

		public TempData()
		{
			root = new Fragment();
			minX = -1d;
			minY = -1d;
			size = 2d;
			hasExPalette = false;
			syncRoot = new object();

            for (int i = 0; i < 5; i++) {
                ExtendRoot();
            }
		}

		public void ExtendRoot()
		{
			// TODO: Copy data of root to new root
			Fragment newRoot = new Fragment();

            minX *= 2;
            minY *= 2;
            size *= 2;

            newRoot.childLT = new Fragment();
            newRoot.childLT.childRB = root.childLT;

            newRoot.childRT = new Fragment();
            newRoot.childRT.childLB = root.childRT;

            newRoot.childLB = new Fragment();
            newRoot.childLB.childRT = root.childLB;

            newRoot.childRB = new Fragment();
            newRoot.childRB.childLT = root.childRB;

            root = newRoot;
		}
	}
}
