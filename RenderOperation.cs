using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Fractals
{
	public class RenderOperation: IComparable<RenderOperation>
	{
		static long nextFreeId = 0;
		long id;
		
		RenderOperation parent;
		Fragment fragment;
		RectangleD dataSource;
		RectangleD renderDestination;
		Matrix rotation;
		float distanceFromScreenCentre;
		double priority;
		
		public RenderOperation Parent {
			get {
				return parent;
			}
		}
		
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
				return new RenderOperation(this, fragment.ChildLT, dataSource.LeftTopQuater, renderDestination.LeftTopQuater, rotation);
			}
		}
		
		public RenderOperation RightTopQuater {
			get {
				return new RenderOperation(this, fragment.ChildRT, dataSource.RightTopQuater, renderDestination.RightTopQuater, rotation);
			}
		}
		
		public RenderOperation LeftBottomQuater {
			get {
				return new RenderOperation(this, fragment.ChildLB, dataSource.LeftBottomQuater , renderDestination.LeftBottomQuater, rotation);
			}
		}
		
		public RenderOperation RightBottomQuater {
			get {
				return new RenderOperation(this, fragment.ChildRB, dataSource.RightBottomQuater, renderDestination.RightBottomQuater, rotation);
			}
		}
		
		public double Priority {
			get {
				return priority;
			}
		}
		
		float DistanceFromScreenCentre {
			get {
				return distanceFromScreenCentre;
			}
		}
		
		public double TexelSize {
			get {
				return renderDestination.Width / Fragment.FragmentSize * 1.0d;
			}
		}
		
		public bool IsInViewFrustum {
			get {
				return DistanceFromScreenCentre < 1 + 0.72d * renderDestination.Size;
			}
		}
		
		public RenderOperation(RenderOperation parent, Fragment fragment, RectangleD dataSource, RectangleD renderDestination, Matrix rotation)
		{
			this.parent = parent;
			this.fragment = fragment;
			this.dataSource = dataSource;
			this.renderDestination = renderDestination;
			this.rotation = rotation;
			this.id = nextFreeId++;
			
			PointF[] center = new PointF[] {renderDestination.CentreF};
			rotation.TransformPoints(center);
			distanceFromScreenCentre = Math.Max(Math.Abs(center[0].X), Math.Abs(center[0].Y));
			
			// Small means high-priority; big means low-priority
			this.priority = fragment.Depth +
			                Math.Min(DistanceFromScreenCentre, 1d) * 0.5d +
			                -Math.Min(fragment.MaxColorDifference / 64d, 1d) * 2.5d;
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
	}
}
