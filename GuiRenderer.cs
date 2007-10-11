using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Fractals
{
	public class GuiRenderer: Renderer
	{
		public GuiRenderer(Fractal fractal, Graphics graphics, Rectangle renderRectangle)
			:base(fractal, graphics, renderRectangle)
		{
		}
	}
}
