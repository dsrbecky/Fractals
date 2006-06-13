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
		
		Fractal currentFractal = new Fractal();
		
		public event EventHandlerNoArg CurrentFractalChanged;
		
		public Fractal CurrentFractal {
			get {
				return currentFractal;
			}
			set {
				currentFractal = value;
				OnCurrentFractalChanged();
			}
		}
		
		protected virtual void OnCurrentFractalChanged()
		{
			if (CurrentFractalChanged != null) {
				CurrentFractalChanged();
			}
		}
		
		
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
		
		void FractalFileComboBoxLoading(object sender, Fractals.Util.TextReaderEventArgs e)
		{
			currentFractal = (Fractal)xmlFractalSerializer.Deserialize(e.TextReader);
			equationCode.Text = currentFractal.Equation.Code.Replace("\n", "\r\n");
			colorCode.Text = currentFractal.ColorMap.Code.Replace("\n", "\r\n");
			OnCurrentFractalChanged();
		}
		
		void FractalFileComboBoxSaving(object sender, Fractals.Util.TextWriterEventArgs e)
		{
			xmlFractalSerializer.Serialize(e.TextWriter, currentFractal);
		}
		
		void EquationFileComboBoxLoading(object sender, Fractals.Util.TextReaderEventArgs e)
		{
			currentFractal.Equation = (Equation)xmlEquationSerializer.Deserialize(e.TextReader);
			equationCode.Text = currentFractal.Equation.Code.Replace("\n", "\r\n");
			OnCurrentFractalChanged();
		}
		
		void EquationFileComboBoxSaving(object sender, Fractals.Util.TextWriterEventArgs e)
		{
			xmlEquationSerializer.Serialize(e.TextWriter, currentFractal.Equation);
		}
		
		void ColorFileComboBoxLoading(object sender, Fractals.Util.TextReaderEventArgs e)
		{
			currentFractal.ColorMap = (ColorMap)xmlColorMapSerializer.Deserialize(e.TextReader);
			colorCode.Text = currentFractal.ColorMap.Code.Replace("\n", "\r\n");
			OnCurrentFractalChanged();
		}
		
		void ColorFileComboBoxSaving(object sender, Fractals.Util.TextWriterEventArgs e)
		{
			xmlColorMapSerializer.Serialize(e.TextWriter, currentFractal.ColorMap);
		}
		
		void DebugModeCheckedChanged(object sender, System.EventArgs e)
		{
			OnCurrentFractalChanged();
		}
		
		void ApplyCodeChanges()
		{
			currentFractal.Equation = new Equation(equationCode.Text);
			currentFractal.ColorMap = new ColorMap(colorCode.Text);
			OnCurrentFractalChanged();
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
