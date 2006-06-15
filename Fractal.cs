using System;
using System.Drawing;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Xml.Serialization;

namespace Fractals
{
	public class Fractal
	{
		Equation equation = new Equation();
		ColorMap colorMap = new ColorMap();
		View view = new View();
		
		public event EventHandler FractalChanged;
		
		public Equation Equation {
			get {
				return equation;
			}
			set {
				equation = value;
				OnFractalChanged(EventArgs.Empty);
			}
		}
		
		public View View {
			get {
				return view;
			}
			set {
				view = value;
				OnFractalChanged(EventArgs.Empty);
			}
		}
		
		public ColorMap ColorMap {
			get {
				return colorMap;
			}
			set {
				colorMap = value;
				OnFractalChanged(EventArgs.Empty);
			}
		}
		
		protected virtual void OnFractalChanged(EventArgs e)
		{
			if (FractalChanged != null) {
				FractalChanged(this, e);
			}
		}
		
		public bool Compiles {
			get {
				return equation.Compiles && colorMap.Compiles;
			}
		}
	}
}
