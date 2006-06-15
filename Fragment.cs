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
		
		
		// NOTE: Fragmet always has half the size of parent
		
		ColorIndex[,] data = new ColorIndex[FragmentSize, FragmentSize];
		public bool done;
		public bool allTheSame;
		
		public ColorIndex GetColorIndex(int x, int y)
		{
			return data[x, y];
		}
		
		public void SetColorIndex(int x, int y, ColorIndex val)
		{
			data[x, y] = val;
		}
		
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
}
