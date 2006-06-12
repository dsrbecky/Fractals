using System;

namespace Fractals
{
	public class Fragment
	{
		public const int FragmentSize = 32;
		// Memory usage: FragmentSize*FragmentSize*[2-ExPalette]

		public Fragment childLeftTop = null;
		public Fragment childRightTop = null;
		public Fragment childLeftButtom = null;
		public Fragment childRightButtom = null;
		public Fragment[] childs
		{
			get 
			{
				return new Fragment[] {childLeftTop, childRightTop, childLeftButtom, childRightButtom};
			}
		}

		// NOTE: Fragmet always has half the area of parent
		// NOTE: Fist byte in chunk is always copy of parent byte (+25% memory usage)

		public uint[] data;
		public ushort done = 0; // map of 4x4 done bits
		public bool hasExPalette;  // TRUE - data use 2 bytes; FALSE - data use 1 byte
		// public int depth = 0; // -1 means [-2,2]; 0 means [-1,1]; 1 means [-0.5,0.5]; etc;

		public double srcX;
		public double srcY;
		public double srcWidth;

		//public Fragment parent = null;

		public Fragment(bool _hasExPalette,double _srcX,double _srcY,double _srcWidth)
		{
			hasExPalette = _hasExPalette;
			srcX = _srcX;
			srcY = _srcY;
			srcWidth = _srcWidth;
			
			data = new UInt32[FragmentSize*FragmentSize/(hasExPalette?2:4)];
		}

		public void MakeChilds()
		{
			MakeChildLT();
			MakeChildRT();
			MakeChildLB();
			MakeChildRB();
		}

		public void MakeChildLT()
		{childLeftTop = new Fragment(hasExPalette, srcX, srcY, srcWidth/2);}

		public void MakeChildRT()
		{childRightTop = new Fragment(hasExPalette, srcX + srcWidth*(FragmentSize/2), srcY, srcWidth/2);}

		public void MakeChildLB()
		{childLeftButtom = new Fragment(hasExPalette, srcX, srcY + srcWidth*(FragmentSize/2), srcWidth/2);}

		public void MakeChildRB()
		{childRightButtom = new Fragment(hasExPalette, srcX + srcWidth*(FragmentSize/2), srcY + srcWidth*(FragmentSize/2), srcWidth/2);}
	}

	public class TempData
	{
		public Fragment root;
		public TempData()
		{
			root = new Fragment(false, -2, -2, 4d/Fragment.FragmentSize);
		}

		public void ExtendRoot()
		{
			// TODO: Copy data of root to new root
/*			Fragment newRoot = new Fragment(root.hasExPalette, root.depth-1);

			newRoot.childLeftTop = new Fragment(root.hasExPalette, root.depth);
			newRoot.childLeftTop.childRightButtom = root.childLeftTop;

			newRoot.childRightTop = new Fragment(root.hasExPalette, root.depth);
			newRoot.childRightTop.childLeftButtom = root.childRightTop;

			newRoot.childLeftButtom = new Fragment(root.hasExPalette, root.depth);
			newRoot.childLeftButtom.childRightTop = root.childLeftButtom;

			newRoot.childRightButtom = new Fragment(root.hasExPalette, root.depth);
			newRoot.childRightButtom.childLeftTop = root.childRightButtom;

            root = newRoot;*/
		}
	}
}
