using System;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.IO;

namespace Fractals
{
	[Serializable]
	public struct View
	{
		public double Xpos,Ypos,Xzoom,Yzoom;
		public int AAX,AAY;

		public View (double _Xpos,double _Ypos,double _Xzoom,double _Yzoom,string _code)
		{
			Xpos = _Xpos;
			Ypos = _Ypos;
			Xzoom = _Xzoom;
			Yzoom = _Yzoom;
			AAX = 4;
			AAY = 4;
			method = null;
			code = _code;
		}

		public double makeX (double pos, double range)
		{
			return (((double)pos)/range - 0.5) * (2d/Xzoom) + Xpos;
		}

		public double makeY (double pos, double range)
		{
			return (((double)pos)/range - 0.5) * (2d/Yzoom) + Ypos;
		}

		private string code;
		public string Code
		{
			get{return code;}
			set
			{
				code =  value;
				method = null;
			}
		}

		[NonSerialized]
		private MethodInfo method;
		public MethodInfo Method 
		{
			get
			{
				if (method == null)
					UpdateMethod();
				return method;
			}
		}

		private void UpdateMethod()
		{
			// Make source code
			string tmpCode = "";
			Stream st = Assembly.GetCallingAssembly().GetManifestResourceStream("Fractals.Code.cs");
			byte[] resCode = new byte[(int)st.Length];
			st.Read(resCode,0,(int)st.Length);
			tmpCode = new System.Text.ASCIIEncoding().GetString(resCode);
			tmpCode = tmpCode.Replace (@"//<CODE GOES HERE>//",code);

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
			{ // Succes
				Type main = res.CompiledAssembly.GetType("Fractals.Main");
				method = main.GetMethod("CalcImage");
			}
		}			
	}
}
