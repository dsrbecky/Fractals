using System;
using System.Drawing;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Xml.Serialization;
using Microsoft.CSharp;

namespace Fractals
{
	public static class Compiler
	{
		public static Type CompileClassBody(string classBody)
		{
			// Make source code
			string sourceCode = "using System; using System.Drawing; using System.Drawing.Imaging; \n" +
			                    "namespace Fractals { \n" +
			                    "class Main { \n" +
			                    "#line 1 \n" +
			                    classBody + " \n" +
			                    "} \n} \n";
			
			// Set parameters
			CompilerParameters param = new CompilerParameters();
			param.GenerateInMemory = true;
			foreach(AssemblyName name in Assembly.GetCallingAssembly().GetReferencedAssemblies()) {
				param.ReferencedAssemblies.Add(name.Name + ".dll");
			}
			param.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
			
			// Compile
			CompilerResults res = new CSharpCodeProvider().CompileAssemblyFromSource(param, sourceCode);
			if (res.Errors.HasErrors) {
				string output = "";
				foreach (string s in res.Output) {
					output += s + "\n";
				}
				throw new CompileException(output);
			} 
			return res.CompiledAssembly.GetType("Fractals.Main");
		}
	}
}
