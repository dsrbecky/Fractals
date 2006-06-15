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
	public class Equation
	{
		delegate void GetColorIndexDelegate(double p, double q,out double r,out double g,out double b);
		
		string code;
		string compileError;
		GetColorIndexDelegate getColorIndexMethod;
		
		public string Code {
			get {
				return code;
			}
			set {
				code = value;
				try {
					Type mainClass = Compiler.CompileClassBody(code);
					MethodInfo getColorIndexMethodInfo = mainClass.GetMethod("GetColor");
					if (getColorIndexMethodInfo == null) {
						throw new CompileException("Method not found");
					}
					getColorIndexMethod = (GetColorIndexDelegate)Delegate.CreateDelegate(typeof(GetColorIndexDelegate), getColorIndexMethodInfo);
				} catch (CompileException e) {
					compileError = e.ErrorMessage;
					MessageBox.Show(compileError, "Can not compile equation");
				}
			}
		}
		
		[XmlIgnore]
		public bool Compiles {
			get {
				return compileError == null;
			}
		}
		
		[XmlIgnore]
		public string CompileError {
			get {
				return compileError;
			}
		}
		
		public Equation() {}
		
		public Equation(string code)
		{
			this.Code = code;
		}
		
		public ColorIndex GetColorIndex(double p, double q)
		{
			double r, g, b;
			getColorIndexMethod(p, q, out r, out g, out b);
			return new ColorIndex(0, r, g, b);
		}
	}
}
