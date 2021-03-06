using System;

namespace Fractals
{
	public class DataTree
	{
		Fragment root = new Fragment(0);
		double size = 2d;
		
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
		
		public RectangleD Area {
			get {
				return new RectangleD(-size / 2, -size / 2, size, size);
			}
		}
		
		public void ExtendRoot()
		{
			Fragment newRoot = new Fragment(root.Depth - 1);
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
