using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Fractals
{
	[Serializable]
	public class View
	{
		public double Xpos = -0.5;
		public double Ypos = 0;
		public double Xzoom = 1;
		public double Yzoom = 1;
		
		public double Angle = 0;
		public double TargetAngle = 0;
		
		public void Move (double Xpos,double Ypos,double Xzoom,double Yzoom)
		{
			this.Xpos = Xpos;
			this.Ypos = Ypos;
			this.Xzoom = Xzoom;
			this.Yzoom = Yzoom;
		}
		
		public double makeX (double pos, double range)
		{
			// return [-1,1]mapping * width + origin
			return (pos/range - 0.5)*2d /Xzoom + Xpos;
		}
		
		public double makeY (double pos, double range)
		{
			// return [-1,1]mapping * width + origin
			return (pos/range - 0.5)*2d /Yzoom + Ypos;
		}

	}
}
