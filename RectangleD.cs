using System;
using System.Drawing;

namespace Fractals
{
	public struct RectangleD
	{
		double x;
		double y;
		double width;
		double height;
		
		public double X {
			get {
				return x;
			}
		}
		
		public double Y {
			get {
				return y;
			}
		}
		
		public double Width {
			get {
				return width;
			}
		}
		
		public double Height {
			get {
				return height;
			}
		}
		
		public double Size {
			get {
				return width;
			}
		}
		
		public RectangleD(double x, double y, double width, double height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}
		
		public RectangleD LeftTopQuater {
			get {
				return new RectangleD(X,
				                      Y,
				                      Width / 2,
				                      Height / 2);
			}
		}
		
		public RectangleD RightTopQuater {
			get {
				return new RectangleD(X + Width / 2,
				                      Y,
				                      Width / 2,
				                      Height / 2);
			}
		}
		
		public RectangleD LeftBottomQuater {
			get {
				return new RectangleD(X,
				                      Y + Height / 2,
				                      Width / 2,
				                      Height / 2);
			}
		}
		
		public RectangleD RightBottomQuater {
			get {
				return new RectangleD(X + Width / 2,
				                      Y + Height / 2,
				                      Width / 2,
				                      Height / 2);
			}
		}
		
		public PointF LeftTopCornerF {
			get {
				return new PointF((float)X,
				                  (float)Y);
			}
		}
		
		public PointF RightTopCornerF {
			get {
				return new PointF((float)(X + Width),
				                  (float)Y);
			}
		}
		
		public PointF LeftBottomCornerF {
			get {
				return new PointF((float)X,
				                  (float)(Y + Height));
			}
		}
		
		public PointF RightBottomCornerF {
			get {
				return new PointF((float)(X + Width),
				                  (float)(Y + Height));
			}
		}
		
		public PointF CentreF {
			get {
				return new PointF((float)(X + Width / 2),
				                  (float)(Y + Height / 2));
			}
		}
	}
}
