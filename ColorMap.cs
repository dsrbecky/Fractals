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
	public class ColorMap
	{
		string code;
		string compileError;
		Color[] palette;
		
		public string Code {
			get {
				return code;
			}
			set {
				code = value;
				try {
					Type mainClass = Compiler.CompileClassBody(code);
					MethodInfo methodMakeColors = mainClass.GetMethod("MakeColors");
					if (methodMakeColors == null) {
						throw new CompileException("Method not found");
					}
					FieldInfo cInfo = mainClass.GetField("c");
					methodMakeColors.Invoke(null, null);
					palette = (Color[])cInfo.GetValue(null);
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
		
		public ColorMap() {}
		
		public ColorMap(string code)
		{
			this.Code = code;
		}
		
		public Color GetColorFromIndex(ColorIndex index)
		{
			return palette[index.Index];
		}
	}
}
