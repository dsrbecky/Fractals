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
	public struct SavingRules
	{
		public enum ResulutionMode:int {Desktop = 0, MainWindow = 1, Custom = 2};

		public string filename;
		public bool makeUniqueFilename;
		public int antiAliasing;
		public bool edgeOnlyAntiAliasing;
		public ResulutionMode resulutionMode;
		public int resulutionX;
		public int resulutionY;
		public string fileformat;
	}

	[Serializable]
	public struct Settings
	{
		public Hashtable equations;
		public string lastEquation;

		public Hashtable views;
		public string lastView;

		public Hashtable colorPaletes;
		public string lastColorPalete;

		public Hashtable customCodes;
		public string lastCustomCode;

		public Hashtable savingRules;
		public string lastSavingRule;

		public string commentes;
	}
}