using System;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Collections.Specialized;

namespace Fractals
{
	[Serializable]
	public class Settings
	{
		public Hashtable equations;
		public string lastEquation;
		
		public Hashtable colorPaletes;
		public string lastColorPalete;
	}
}
