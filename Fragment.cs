using System;
using System.Drawing;

namespace Fractals
{
	public class Fragment
	{
		public const int FragmentSize = 16;
		public const int BitmapSize = Fragment.FragmentSize + 1;
		
		Fragment childLT;
		Fragment childRT;
		Fragment childLB;
		Fragment childRB;
		
		public Fragment ChildLT {
			get {
				return childLT;
			}
			set {
				childLT = value;
			}
		}
		
		public Fragment ChildRT {
			get {
				return childRT;
			}
			set {
				childRT = value;
			}
		}
		
		public Fragment ChildLB {
			get {
				return childLB;
			}
			set {
				childLB = value;
			}
		}
		
		public Fragment ChildRB {
			get {
				return childRB;
			}
			set {
				childRB = value;
			}
		}
		
		
		// NOTE: Fragmet always has half the area of parent
		
		public uint[] data = new UInt32[FragmentSize*FragmentSize/4];
		public ushort done = 0; // map of 4x4 done bits
		public bool allTheSame;
		
		public void MakeChilds()
		{
			childLT = new Fragment();
			childRT = new Fragment();
			childLB = new Fragment();
			childRB = new Fragment();
		}
		
		public bool HasAllChilds {
			get {
				return childLT != null && childRT != null && childLB != null && childRB != null;
			}
		}
	}
	
	public class DataTree
	{
		Fragment root = new Fragment();
		double size = 64d;
		
		public Fragment Root {
			get {
				return root;
			}
		}
		
		public double Size {
			get {
				return size;
			}
		}
		
		public void ExtendRoot()
		{
			Fragment newRoot = new Fragment();
			newRoot.MakeChilds();
			newRoot.ChildLT.ChildRB = Root.ChildLT;
			newRoot.ChildRT.ChildLB = Root.ChildRT;
			newRoot.ChildLB.ChildRT = Root.ChildLB;
			newRoot.ChildRB.ChildLT = Root.ChildRB;
			
			root = newRoot;
			size *= 2;
		}
	}
}
