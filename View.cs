using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml.Serialization;

namespace Fractals
{
	[Serializable]
	public class View
	{
		public double Xpos = -0.5;
		public double Ypos = 0;
		public double Xzoom = 1;
		public double Yzoom = 1;
		
		double currentAngle = 0;
		double targetAngle = 0;
		
		[XmlIgnore]
		public double CurrentAngle {
			get {
				return currentAngle;
			}
		}
		
		public double Angle {
			get {
				return targetAngle;
			}
			set {
				targetAngle = value;
			}
		}
		
		public double BoundingBoxSize {
			get {
				double positionDistance = Math.Max(Math.Abs(Xpos), Math.Abs(Ypos));
				double zoomDistance = Math.Max(1d / Xzoom, 1d / Yzoom);
				return 2 * (positionDistance + zoomDistance);
			}
		}
		
		public void Move (double Xpos, double Ypos, double Xzoom, double Yzoom)
		{
			this.Xpos = Xpos;
			this.Ypos = Ypos;
			this.Xzoom = Xzoom;
			this.Yzoom = Yzoom;
		}
		
		public bool Rotating { 
			get {
				return targetAngle != currentAngle;
			}
		}
		
		public void AnimateRotation()
		{
			while (currentAngle - targetAngle > 180) {
				currentAngle -= 360;
			}
			while (currentAngle - targetAngle < -180) {
				currentAngle += 360;
			}
			
			currentAngle = (5 * currentAngle + targetAngle) / 6;
			currentAngle -= Math.Sign(currentAngle - targetAngle) * Math.Min(0.5, Math.Abs(currentAngle - targetAngle));
			
			if (Math.Abs(currentAngle - targetAngle) < 0.1) {
				currentAngle = targetAngle;
			}
		}
		
		public void ZoomIn(PointF logicalPosition, double zoomFactor)
		{
			PointF[] pos = new PointF[] {logicalPosition};
			Matrix matrix = new Matrix();
			matrix.Rotate((float)(-currentAngle), MatrixOrder.Append);
			matrix.TransformPoints(pos);
			
			Move(Xpos + pos[0].X / Xzoom,
			     Ypos + pos[0].Y / Yzoom,
			     Xzoom * zoomFactor,
			     Yzoom * zoomFactor);
		}
	}
}
