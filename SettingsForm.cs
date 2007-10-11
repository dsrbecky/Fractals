using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace Fractals
{
	public partial class SettingsForm
	{
		static XmlSerializer xmlFractalSerializer = new XmlSerializer(typeof(Fractal));
		static XmlSerializer xmlEquationSerializer = new XmlSerializer(typeof(Equation));
		static XmlSerializer xmlColorMapSerializer = new XmlSerializer(typeof(ColorMap));
		
		public SettingsForm()
		{
			InitializeComponent();
			
			string directory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
			
			fractalFileComboBox.Directory = directory;
			fractalFileComboBox.Extension = ".fractal.xml";
			equationFileComboBox.Directory = directory;
			equationFileComboBox.Extension = ".equation.xml";
			colorFileComboBox.Directory = directory;
			colorFileComboBox.Extension = ".colormap.xml";
			
			ApplyCodeChanges();
		}
		
		void FractalFileComboBoxLoading(object sender, Util.TextReaderEventArgs e)
		{
			CurrentFractalSingleton.Instance = (Fractal)xmlFractalSerializer.Deserialize(e.TextReader);
			equationCode.Text = CurrentFractalSingleton.Instance.Equation.Code.Replace("\n", "\r\n");
			colorCode.Text = CurrentFractalSingleton.Instance.ColorMap.Code.Replace("\n", "\r\n");
		}
		
		void FractalFileComboBoxSaving(object sender, Util.TextWriterEventArgs e)
		{
			xmlFractalSerializer.Serialize(e.TextWriter, CurrentFractalSingleton.Instance);
		}
		
		void EquationFileComboBoxLoading(object sender, Util.TextReaderEventArgs e)
		{
			CurrentFractalSingleton.Instance.Equation = (Equation)xmlEquationSerializer.Deserialize(e.TextReader);
			equationCode.Text = CurrentFractalSingleton.Instance.Equation.Code.Replace("\n", "\r\n");
		}
		
		void EquationFileComboBoxSaving(object sender, Util.TextWriterEventArgs e)
		{
			xmlEquationSerializer.Serialize(e.TextWriter, CurrentFractalSingleton.Instance.Equation);
		}
		
		void ColorFileComboBoxLoading(object sender, Util.TextReaderEventArgs e)
		{
			CurrentFractalSingleton.Instance.ColorMap = (ColorMap)xmlColorMapSerializer.Deserialize(e.TextReader);
			colorCode.Text = CurrentFractalSingleton.Instance.ColorMap.Code.Replace("\n", "\r\n");
		}
		
		void ColorFileComboBoxSaving(object sender, Util.TextWriterEventArgs e)
		{
			xmlColorMapSerializer.Serialize(e.TextWriter, CurrentFractalSingleton.Instance.ColorMap);
		}
		
		void DebugModeCheckedChanged(object sender, System.EventArgs e)
		{
			ApplyCodeChanges();
		}
		
		void ApplyCodeChanges()
		{
			CurrentFractalSingleton.Instance.Equation = new Equation(equationCode.Text);
			CurrentFractalSingleton.Instance.ColorMap = new ColorMap(colorCode.Text);
		}
		
		void BtnApplyClick(object sender, System.EventArgs e)
		{
			ApplyCodeChanges();
		}
		
		void SettingsFormFormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}
	}
}
