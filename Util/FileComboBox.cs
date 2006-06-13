using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Fractals.Util
{
	public class TextWriterEventArgs: EventArgs
	{
		TextWriter textWriter;
		
		public TextWriter TextWriter {
			get {
				return textWriter;
			}
		}
		
		public TextWriterEventArgs(TextWriter textWriter)
		{
			this.textWriter = textWriter;
		}
	}
	
	public class TextReaderEventArgs: EventArgs
	{
		TextReader textReader;
		
		public TextReader TextReader {
			get {
				return textReader;
			}
		}
		
		public TextReaderEventArgs(TextReader textReader)
		{
			this.textReader = textReader;
		}
	}
	
	public partial class FileComboBox
	{
		string directory;
		string extension;
		public event EventHandler<TextWriterEventArgs> Saving;
		public event EventHandler<TextReaderEventArgs> Loading;
		
		public string Directory {
			get {
				return directory;
			}
			set {
				directory = value;
			}
		}
		
		public string Extension {
			get {
				return extension;
			}
			set {
				extension = value;
			}
		}
		
		protected virtual void OnSaving(TextWriterEventArgs e)
		{
			if (Saving != null) {
				Saving(this, e);
			}
		}
		
		protected virtual void OnLoading(TextReaderEventArgs e)
		{
			if (Loading != null) {
				Loading(this, e);
			}
		}
		
		
		public FileComboBox()
		{
			InitializeComponent();
		}
		
		void SaveFile(string filename)
		{
			TextWriter writer = new StreamWriter(filename);
			try {
				OnSaving(new TextWriterEventArgs(writer));
			} finally {
				writer.Close();
			}
		}
		
		void LoadFile(string filename)
		{
			TextReader reader = new StreamReader(filename);
			try {
				OnLoading(new TextReaderEventArgs(reader));
			} finally {
				reader.Close();
			}
		}
		
		string GetFilename(string name)
		{
			return Path.Combine(directory, name + extension);
		}
		
		void CmbBoxDropDown(object sender, System.EventArgs e)
		{
			string selected = cmbBox.Text;
			cmbBox.Items.Clear();
			foreach (string file in System.IO.Directory.GetFiles(directory, "*" + extension)) {
				cmbBox.Items.Add(Path.GetFileName(file).Replace(extension,""));
			}
			cmbBox.Text = selected;
		}
		
		void CmbBoxSelectedIndexChanged(object sender, System.EventArgs e)
		{
			LoadFile(GetFilename(cmbBox.Text));
		}
		
		void SaveClick(object sender, System.EventArgs e)
		{
			if (cmbBox.Text != "") {
				SaveFile(GetFilename(cmbBox.Text));
			} else {
				saveAs.PerformClick();
			}
		}
		
		void SaveAsClick(object sender, System.EventArgs e)
		{
			string name = InputBox.Show("Please enter name", cmbBox.Text, "Save as...");
			if (name == string.Empty || name == "") return;
			string filename = GetFilename(name);
			if (File.Exists(filename)) {
				if (MessageBox.Show("Overwrite " + name + "?",
				                    "File already exists",
				                    MessageBoxButtons.YesNo,
				                    MessageBoxIcon.Question) == DialogResult.No) {
					return;
				}
			}
			SaveFile(filename);
		}
		
		void DeleteClick(object sender, System.EventArgs e)
		{
			string filename = GetFilename(cmbBox.Text);
			if (File.Exists(filename)) {
				if (MessageBox.Show("Do you want to delete " + cmbBox.Text + "?",
				                    "Delete",
				                    MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No) return;
				File.Delete(filename);
			}
		}
	}
}
