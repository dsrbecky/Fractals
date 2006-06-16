using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Fractals
{
	public class RenderOperation: IComparable<RenderOperation>
	{
		static long nextFreeId = 0;
		long id;
		
		Fragment fragment;
		RectangleD dataSource;
		RectangleD renderDestination;
		Matrix rotation;
		
		public Fragment Fragment {
			get {
				return fragment;
			}
		}
		
		public RectangleD DataSource {
			get {
				return dataSource;
			}
		}
		
		public RectangleD RenderDestination {
			get {
				return renderDestination;
			}
		}
		
		public Matrix Rotation {
			get {
				return rotation;
			}
		}
		
		public RenderOperation LeftTopQuater {
			get {
				return new RenderOperation(fragment.ChildLT, dataSource.LeftTopQuater, renderDestination.LeftTopQuater, rotation);
			}
		}
		
		public RenderOperation RightTopQuater {
			get {
				return new RenderOperation(fragment.ChildRT, dataSource.RightTopQuater, renderDestination.RightTopQuater, rotation);
			}
		}
		
		public RenderOperation LeftBottomQuater {
			get {
				return new RenderOperation(fragment.ChildLB, dataSource.LeftBottomQuater , renderDestination.LeftBottomQuater, rotation);
			}
		}
		
		public RenderOperation RightBottomQuater {
			get {
				return new RenderOperation(fragment.ChildRB, dataSource.RightBottomQuater, renderDestination.RightBottomQuater, rotation);
			}
		}
		
		public double Priority {
			get {
				return fragment.Depth;
			}
		}
		
		public RenderOperation(Fragment fragment, RectangleD dataSource, RectangleD renderDestination, Matrix rotation)
		{
			this.fragment = fragment;
			this.dataSource = dataSource;
			this.renderDestination = renderDestination;
			this.rotation = rotation;
			this.id = nextFreeId++;
		}
		
		public int CompareTo(RenderOperation op)
		{
			int comp = Priority.CompareTo(op.Priority);
			if (comp == 0) {
				return id.CompareTo(op.id);
			} else {
				return comp;
			}
		}
		
		public bool IsInViewFrustum {
			get {
				PointF[] center = new PointF[] {renderDestination.CentreF};
				rotation.TransformPoints(center);
				return Math.Max(Math.Abs(center[0].X), Math.Abs(center[0].Y)) < 1 + 0.72d * renderDestination.Size;
			}
		}
	}
}
