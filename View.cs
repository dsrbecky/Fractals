using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Fractals
{
	[Serializable]
	public struct View
	{
		public double Xpos,Ypos,Xzoom,Yzoom;

		// TODO: [Serializable]
		[NonSerialized]
		public double Angle;

		// TODO: remove>>>
		public int antiAliasingLevel;
		public bool edgeOnlyAntiAliasing;
		public int AA {get {return antiAliasingLevel;}}
		// TODO: remove<<<

		
		// Transformation matrix from space to texture coordinates
		//[NonSerialized] public System.Drawing.Drawing2D.Matrix  matrinx;

		/*[NonSerialized] public double m11;
		[NonSerialized] public double m12;
		[NonSerialized] public double m21;
		[NonSerialized] public double m22;

		public double deltaUpartX{get{return m11;}} //m11
		public double deltaUpartY{get{return m12;}} //m12
		public double deltaVpartX{get{return m21;}} //m21
		public double deltaVpartY{get{return m22;}} //m22

		public Matrix matrix 
		{
			get
			{
				Matrix m = new Matrix();
				m.Translate((float)-Xpos, (float)-Ypos, MatrixOrder.Append);
				m.Scale((float)Xzoom, (float)Yzoom, MatrixOrder.Append);
				m.Rotate((float)(Angle), MatrixOrder.Append);
				return m;
			}
		}

		public Matrix matrixWithoutZoom
		{
			get
			{
				Matrix m = new Matrix();
				m.Translate((float)-Xpos, (float)-Ypos, MatrixOrder.Append);
				//m.Scale((float)Xzoom, (float)Yzoom, MatrixOrder.Append);
				m.Rotate((float)(Angle), MatrixOrder.Append);
				return m;
			}
		}*/

		public void Move (double _Xpos,double _Ypos,double _Xzoom,double _Yzoom)
		{
			Xpos = _Xpos;
			Ypos = _Ypos;
			Xzoom = _Xzoom;
			Yzoom = _Yzoom;

			//UpdateTransformationMatrix();
		}

		/*public void UpdateTransformationMatrix()
		{
			m11 = Math.Cos(-Angle) / Xzoom;
			m12 = Math.Sin(-Angle) / Yzoom;
			m21 = - Math.Sin(-Angle) / Xzoom;
			m22 = Math.Cos(-Angle) / Yzoom;
		}*/

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
