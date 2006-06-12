using System;
using System.Drawing;

namespace Fractals
{
	public unsafe class Fragment
	{       
		public const int FragmentSize = 16;
		public const int BitmapSize = Fragment.FragmentSize + 1;
		
		public Fragment childLT;
		public Fragment childRT;
		public Fragment childLB;
		public Fragment childRB;
		
		// NOTE: Fragmet always has half the area of parent
		// NOTE: Fist byte in chunk is always copy of parent byte (+25% memory usage)
		
		public uint[] data;
		public ushort done = 0; // map of 4x4 done bits
		public int bitmapID = -1;
		
		public bool allTheSame;
		
		public Fragment()
		{
			data = new UInt32[FragmentSize*FragmentSize/4];
		}
		
		public void MakeChilds()
		{
			MakeChildLT();
			MakeChildRT();
			MakeChildLB();
			MakeChildRB();
		}
		
		public bool HasAllChilds {
			get {
				return childLT != null && childRT != null && childLB != null && childRB != null;
		}
		}
		
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
		public object syncRoot;
		
		public TempData()
		{
			root = new Fragment();
			minX = -1d;
			minY = -1d;
			size = 2d;
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
