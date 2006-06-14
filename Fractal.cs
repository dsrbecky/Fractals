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
	delegate void GetColorIndex(double p, double q,out double r,out double g,out double b);
	
	public class Equation
	{
		public string Code;
		
		public Equation():this("") {}
		
		public Equation(string code)
		{
			this.Code = code;
		}
	}
	
	public class ColorMap
	{
		public string Code;
		
		public ColorMap():this("") {}
		
		public ColorMap(string code)
		{
			this.Code = code;
		}
	}
		
	public class Fractal
	{
		Equation equation = new Equation();
		ColorMap colorMap = new ColorMap();
		View view = new View();
		string compileError;
		
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
			CompileCode();
			if (FractalChanged != null) {
				FractalChanged(this, e);
			}
		}
		
		public bool Compiles {
			get {
				return getColorIndex != null;
			}
		}
		
		public string CompileError {
			get {
				return compileError;
			}
		}
		
		GetColorIndex getColorIndex;
		[XmlIgnore]
		public Color[] palette;
		
		public void GetColorIndex(double p, double q,out double r,out double g,out double b)
		{
			getColorIndex(p, q, out r, out g, out b);
		}
		
		void CompileCode()
		{
			Assembly assembly;
			getColorIndex = null;
			palette = null;
			
			// Make source code
			string tmpCode = "using System; using System.Drawing; using System.Drawing.Imaging; " +
				             "namespace Fractals { class Main { \n" +
				             equation.Code + "\n" + colorMap.Code + "\n}}";
			
			// Set parameters
			CompilerParameters param = new CompilerParameters();
			//param.IncludeDebugInformation = true;
			param.GenerateInMemory = true;
			param.CompilerOptions += " /unsafe";
			foreach(AssemblyName name in Assembly.GetCallingAssembly().GetReferencedAssemblies())
				param.ReferencedAssemblies.Add(name.Name + ".dll");
			param.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
			
			// Compile
			ICodeCompiler comp = new Microsoft.CSharp.CSharpCodeProvider().CreateCompiler();
			CompilerResults res = comp.CompileAssemblyFromSource (param,tmpCode);
			compileError = "";
			foreach (string s in res.Output) {
				compileError += s + "\n";
			}
			if (res.Errors.HasErrors) return;
			assembly = res.CompiledAssembly;
			
			// Update method delegate
			Type mainClass = assembly.GetType("Fractals.Main");
			MethodInfo methodGetColor = mainClass.GetMethod("GetColor");
			getColorIndex = (GetColorIndex)Delegate.CreateDelegate(typeof(GetColorIndex), methodGetColor);
			
			// Update color map
			MethodInfo methodMakeColors = mainClass.GetMethod("MakeColors");
			FieldInfo cInfo = mainClass.GetField("c");
			if (methodMakeColors == null || cInfo == null) {
				Color[] colors = new Color[256];
				for (int i = 0; i < 256; i++) { 
					colors[i] = Color.FromArgb(0,0,i);
				}
				palette = colors;
			} else {
				methodMakeColors.Invoke(null, null);
				palette = (Color[])cInfo.GetValue(null);
			}
		}
	}
}
