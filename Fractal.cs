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
		
		public Equation Equation {
			get {
				return equation;
			}
			set {
				equation = value;
				FireUpdate();
			}
		}
		
		public View View {
			get {
				return view;
			}
			set {
				view = value;
			}
		}
		
		public ColorMap ColorMap {
			get {
				return colorMap;
			}
			set {
				colorMap = value;
			}
		}
		
		DataGenerator.dlgtGetIndex getColorIndex;
		
		public DataGenerator.dlgtGetIndex GetColorIndex {
			get {
				if (method == null)
					UpdateMethod();
				return getColorIndex;
			}
		}
		
		MethodInfo method;
		
		public MethodInfo Method {
			get {
				if (method == null)
					UpdateMethod();
				return method;
			}
		}
		
		[XmlIgnore]
		public Color[] colorPalette;
		
		private void FireUpdate()
		{
			method = null;
		}
		
		private void UpdateMethod()
		{
			// Make source code
			string tmpCode = "using System; using System.Drawing; using System.Drawing.Imaging; " +
				             "namespace Fractals { class Main { \n" +
				             equation.Code + "\n" + colorMap.Code + "\n}}";
			
			// set parameters
			CompilerParameters param = new CompilerParameters();
			//param.IncludeDebugInformation = true;
			param.GenerateInMemory = true;
			param.CompilerOptions += " /unsafe";
			foreach(AssemblyName name in Assembly.GetCallingAssembly().GetReferencedAssemblies())
				param.ReferencedAssemblies.Add(name.Name + ".dll");
			param.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
			
			// compile
			ICodeCompiler comp = new Microsoft.CSharp.CSharpCodeProvider().CreateCompiler();
			CompilerResults res = comp.CompileAssemblyFromSource (param,tmpCode);
			string output = "";
			foreach (string s in res.Output) output += s + "\n";
			if (res.Errors.HasErrors) 
			{ // Errors	
				MessageBox.Show(output);
			}
			else
			{ // Success
				Type main = res.CompiledAssembly.GetType("Fractals.Main");
				method = main.GetMethod("GetColor");
				MethodInfo MakeColors = main.GetMethod("MakeColors");
				FieldInfo cInfo = main.GetField("c");
				if (MakeColors == null || cInfo == null) {
					colorPalette = new Color[256];
					for (int i = 0; i < 256; i++) colorPalette[i] = Color.FromArgb(0,0,i);
				} else {
					MakeColors.Invoke(null, null);
					colorPalette = (Color[])cInfo.GetValue(null);
				}
				getColorIndex = (DataGenerator.dlgtGetIndex)Delegate.CreateDelegate(typeof(DataGenerator.dlgtGetIndex),method);
			}
		}
	}
}
