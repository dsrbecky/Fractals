using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.CodeDom.Compiler;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;


namespace Fractals
{
	public class SettingsDlg : System.Windows.Forms.Form
	{
		static string path = Assembly.GetCallingAssembly().Location;
		static string dir = Path.GetDirectoryName (path);
		static string ext = ".fractal.xml";
		
		public event EventHandlerNoArg ViewChanged;
		
		public Settings settings = new Settings();
		public bool suspendUpdate = false;
		
		Hashtable datamap;
		Hashtable codemap;
		Hashtable cbmap;
		
		public View view = new View();
		
		#region Controls

        private System.Windows.Forms.Button buttonApply;
        public System.Windows.Forms.ComboBox MainCmbBox;
        private System.Windows.Forms.Button MainBtnDelete;
        private System.Windows.Forms.Label labelFile;
        private System.Windows.Forms.Button MainBtnSaveAs;
        private TabPage TabColors;
        public TextBox TabColorsCode;
        private CheckBox TabColorsOtherFiles;
        private Button TabColorsBtnDelete;
        private Button TabColorsBtnSaveAs;
        private Button TabColorsBtnSave;
        private ComboBox TabColorsCmbBox;
        private TabPage TabEq;
        private Button TabEqBtnDelete;
        private Button TabEqBtnSaveAs;
        private Button TabEqBtnSave;
        private ComboBox TabEqCmbBox;
        public TextBox TabEqCode;
        private TabControl Tabs;
        public CheckBox chkBoxDebugMode;

        private System.ComponentModel.Container components = null;

		#endregion
		
		public SettingsDlg()
		{
			InitializeComponent();
			
			settings.equations = new Hashtable();
			settings.colorPaletes = new Hashtable();
			SetMaps();
		}
		
		void SetMaps()
		{
			datamap = new Hashtable();
			datamap.Add (TabEq, settings.equations);
			datamap.Add (TabColors, settings.colorPaletes);
			
			cbmap = new Hashtable();
			cbmap.Add (TabEq, TabEqCmbBox);
			cbmap.Add (TabColors, TabColorsCmbBox);
			
			codemap = new Hashtable();
			codemap.Add (TabEq, TabEqCode);
			codemap.Add (TabColors, TabColorsCode);
		}
		
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDlg));
			this.MainCmbBox = new System.Windows.Forms.ComboBox();
			this.buttonApply = new System.Windows.Forms.Button();
			this.MainBtnDelete = new System.Windows.Forms.Button();
			this.labelFile = new System.Windows.Forms.Label();
			this.MainBtnSaveAs = new System.Windows.Forms.Button();
			this.TabColors = new System.Windows.Forms.TabPage();
			this.TabColorsCode = new System.Windows.Forms.TextBox();
			this.TabColorsOtherFiles = new System.Windows.Forms.CheckBox();
			this.TabColorsBtnDelete = new System.Windows.Forms.Button();
			this.TabColorsBtnSaveAs = new System.Windows.Forms.Button();
			this.TabColorsBtnSave = new System.Windows.Forms.Button();
			this.TabColorsCmbBox = new System.Windows.Forms.ComboBox();
			this.TabEq = new System.Windows.Forms.TabPage();
			this.TabEqBtnDelete = new System.Windows.Forms.Button();
			this.TabEqBtnSaveAs = new System.Windows.Forms.Button();
			this.TabEqBtnSave = new System.Windows.Forms.Button();
			this.TabEqCmbBox = new System.Windows.Forms.ComboBox();
			this.TabEqCode = new System.Windows.Forms.TextBox();
			this.Tabs = new System.Windows.Forms.TabControl();
			this.chkBoxDebugMode = new System.Windows.Forms.CheckBox();
			this.TabColors.SuspendLayout();
			this.TabEq.SuspendLayout();
			this.Tabs.SuspendLayout();
			this.SuspendLayout();
			// 
			// MainCmbBox
			// 
			this.MainCmbBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.MainCmbBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.MainCmbBox.FormattingEnabled = true;
			this.MainCmbBox.Location = new System.Drawing.Point(89, 11);
			this.MainCmbBox.Name = "MainCmbBox";
			this.MainCmbBox.Size = new System.Drawing.Size(310, 29);
			this.MainCmbBox.Sorted = true;
			this.MainCmbBox.TabIndex = 2;
			this.MainCmbBox.SelectedIndexChanged += new System.EventHandler(this.MainCmbBox_SelectedIndexChanged);
			this.MainCmbBox.DropDown += new System.EventHandler(this.MainCmbBox_DropDown);
			// 
			// buttonApply
			// 
			this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonApply.Location = new System.Drawing.Point(411, 623);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(256, 31);
			this.buttonApply.TabIndex = 5;
			this.buttonApply.Text = "&Apply code changes";
			this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
			// 
			// MainBtnDelete
			// 
			this.MainBtnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MainBtnDelete.Location = new System.Drawing.Point(539, 11);
			this.MainBtnDelete.Name = "MainBtnDelete";
			this.MainBtnDelete.Size = new System.Drawing.Size(120, 30);
			this.MainBtnDelete.TabIndex = 12;
			this.MainBtnDelete.Text = "Delete";
			this.MainBtnDelete.Click += new System.EventHandler(this.MainBtnDelete_Click);
			// 
			// labelFile
			// 
			this.labelFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelFile.Location = new System.Drawing.Point(13, 11);
			this.labelFile.Name = "labelFile";
			this.labelFile.Size = new System.Drawing.Size(76, 30);
			this.labelFile.TabIndex = 13;
			this.labelFile.Text = "File:";
			// 
			// MainBtnSaveAs
			// 
			this.MainBtnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MainBtnSaveAs.Location = new System.Drawing.Point(411, 11);
			this.MainBtnSaveAs.Name = "MainBtnSaveAs";
			this.MainBtnSaveAs.Size = new System.Drawing.Size(120, 30);
			this.MainBtnSaveAs.TabIndex = 12;
			this.MainBtnSaveAs.Text = "Save as ...";
			this.MainBtnSaveAs.Click += new System.EventHandler(this.MainBtnSaveAs_Click);
			// 
			// TabColors
			// 
			this.TabColors.Controls.Add(this.TabColorsCode);
			this.TabColors.Controls.Add(this.TabColorsOtherFiles);
			this.TabColors.Controls.Add(this.TabColorsBtnDelete);
			this.TabColors.Controls.Add(this.TabColorsBtnSaveAs);
			this.TabColors.Controls.Add(this.TabColorsBtnSave);
			this.TabColors.Controls.Add(this.TabColorsCmbBox);
			this.TabColors.Location = new System.Drawing.Point(4, 30);
			this.TabColors.Name = "TabColors";
			this.TabColors.Size = new System.Drawing.Size(646, 527);
			this.TabColors.TabIndex = 6;
			this.TabColors.Text = "Color palete";
			// 
			// TabColorsCode
			// 
			this.TabColorsCode.AcceptsReturn = true;
			this.TabColorsCode.AcceptsTab = true;
			this.TabColorsCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.TabColorsCode.Location = new System.Drawing.Point(13, 85);
			this.TabColorsCode.Multiline = true;
			this.TabColorsCode.Name = "TabColorsCode";
			this.TabColorsCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.TabColorsCode.Size = new System.Drawing.Size(617, 422);
			this.TabColorsCode.TabIndex = 11;
			this.TabColorsCode.Text = resources.GetString("TabColorsCode.Text");
			this.TabColorsCode.WordWrap = false;
			// 
			// TabColorsOtherFiles
			// 
			this.TabColorsOtherFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.TabColorsOtherFiles.Location = new System.Drawing.Point(13, 43);
			this.TabColorsOtherFiles.Name = "TabColorsOtherFiles";
			this.TabColorsOtherFiles.Size = new System.Drawing.Size(617, 26);
			this.TabColorsOtherFiles.TabIndex = 10;
			this.TabColorsOtherFiles.Text = "Display pletes from other files";
			// 
			// TabColorsBtnDelete
			// 
			this.TabColorsBtnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabColorsBtnDelete.Location = new System.Drawing.Point(514, 11);
			this.TabColorsBtnDelete.Name = "TabColorsBtnDelete";
			this.TabColorsBtnDelete.Size = new System.Drawing.Size(120, 30);
			this.TabColorsBtnDelete.TabIndex = 9;
			this.TabColorsBtnDelete.Text = "Delete";
			this.TabColorsBtnDelete.Click += new System.EventHandler(this.Delete_Click);
			// 
			// TabColorsBtnSaveAs
			// 
			this.TabColorsBtnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabColorsBtnSaveAs.Location = new System.Drawing.Point(386, 11);
			this.TabColorsBtnSaveAs.Name = "TabColorsBtnSaveAs";
			this.TabColorsBtnSaveAs.Size = new System.Drawing.Size(120, 30);
			this.TabColorsBtnSaveAs.TabIndex = 8;
			this.TabColorsBtnSaveAs.Text = "Save as ...";
			this.TabColorsBtnSaveAs.Click += new System.EventHandler(this.SaveAs_Click);
			// 
			// TabColorsBtnSave
			// 
			this.TabColorsBtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabColorsBtnSave.Location = new System.Drawing.Point(258, 11);
			this.TabColorsBtnSave.Name = "TabColorsBtnSave";
			this.TabColorsBtnSave.Size = new System.Drawing.Size(120, 30);
			this.TabColorsBtnSave.TabIndex = 7;
			this.TabColorsBtnSave.Text = "Save";
			this.TabColorsBtnSave.Click += new System.EventHandler(this.Save_Click);
			// 
			// TabColorsCmbBox
			// 
			this.TabColorsCmbBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.TabColorsCmbBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TabColorsCmbBox.FormattingEnabled = true;
			this.TabColorsCmbBox.Location = new System.Drawing.Point(13, 11);
			this.TabColorsCmbBox.Name = "TabColorsCmbBox";
			this.TabColorsCmbBox.Size = new System.Drawing.Size(233, 29);
			this.TabColorsCmbBox.Sorted = true;
			this.TabColorsCmbBox.TabIndex = 6;
			this.TabColorsCmbBox.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
			this.TabColorsCmbBox.DropDown += new System.EventHandler(this.DropDown);
			// 
			// TabEq
			// 
			this.TabEq.Controls.Add(this.TabEqBtnDelete);
			this.TabEq.Controls.Add(this.TabEqBtnSaveAs);
			this.TabEq.Controls.Add(this.TabEqBtnSave);
			this.TabEq.Controls.Add(this.TabEqCmbBox);
			this.TabEq.Controls.Add(this.TabEqCode);
			this.TabEq.Location = new System.Drawing.Point(4, 30);
			this.TabEq.Name = "TabEq";
			this.TabEq.Size = new System.Drawing.Size(646, 527);
			this.TabEq.TabIndex = 5;
			this.TabEq.Text = "Equation";
			// 
			// TabEqBtnDelete
			// 
			this.TabEqBtnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabEqBtnDelete.Location = new System.Drawing.Point(514, 11);
			this.TabEqBtnDelete.Name = "TabEqBtnDelete";
			this.TabEqBtnDelete.Size = new System.Drawing.Size(120, 30);
			this.TabEqBtnDelete.TabIndex = 5;
			this.TabEqBtnDelete.Text = "Delete";
			this.TabEqBtnDelete.Click += new System.EventHandler(this.Delete_Click);
			// 
			// TabEqBtnSaveAs
			// 
			this.TabEqBtnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabEqBtnSaveAs.Location = new System.Drawing.Point(386, 11);
			this.TabEqBtnSaveAs.Name = "TabEqBtnSaveAs";
			this.TabEqBtnSaveAs.Size = new System.Drawing.Size(120, 30);
			this.TabEqBtnSaveAs.TabIndex = 4;
			this.TabEqBtnSaveAs.Text = "Save as ...";
			this.TabEqBtnSaveAs.Click += new System.EventHandler(this.SaveAs_Click);
			// 
			// TabEqBtnSave
			// 
			this.TabEqBtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabEqBtnSave.Location = new System.Drawing.Point(258, 11);
			this.TabEqBtnSave.Name = "TabEqBtnSave";
			this.TabEqBtnSave.Size = new System.Drawing.Size(120, 30);
			this.TabEqBtnSave.TabIndex = 3;
			this.TabEqBtnSave.Text = "Save";
			this.TabEqBtnSave.Click += new System.EventHandler(this.Save_Click);
			// 
			// TabEqCmbBox
			// 
			this.TabEqCmbBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.TabEqCmbBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TabEqCmbBox.FormattingEnabled = true;
			this.TabEqCmbBox.Location = new System.Drawing.Point(13, 11);
			this.TabEqCmbBox.Name = "TabEqCmbBox";
			this.TabEqCmbBox.Size = new System.Drawing.Size(233, 29);
			this.TabEqCmbBox.Sorted = true;
			this.TabEqCmbBox.TabIndex = 2;
			this.TabEqCmbBox.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
			this.TabEqCmbBox.DropDown += new System.EventHandler(this.DropDown);
			// 
			// TabEqCode
			// 
			this.TabEqCode.AcceptsReturn = true;
			this.TabEqCode.AcceptsTab = true;
			this.TabEqCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.TabEqCode.Location = new System.Drawing.Point(13, 52);
			this.TabEqCode.Multiline = true;
			this.TabEqCode.Name = "TabEqCode";
			this.TabEqCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.TabEqCode.Size = new System.Drawing.Size(617, 455);
			this.TabEqCode.TabIndex = 1;
			this.TabEqCode.Text = resources.GetString("TabEqCode.Text");
			this.TabEqCode.WordWrap = false;
			// 
			// Tabs
			// 
			this.Tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.Tabs.Controls.Add(this.TabEq);
			this.Tabs.Controls.Add(this.TabColors);
			this.Tabs.Location = new System.Drawing.Point(13, 52);
			this.Tabs.Name = "Tabs";
			this.Tabs.SelectedIndex = 0;
			this.Tabs.Size = new System.Drawing.Size(654, 561);
			this.Tabs.TabIndex = 8;
			// 
			// chkBoxDebugMode
			// 
			this.chkBoxDebugMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkBoxDebugMode.AutoSize = true;
			this.chkBoxDebugMode.Location = new System.Drawing.Point(13, 629);
			this.chkBoxDebugMode.Name = "chkBoxDebugMode";
			this.chkBoxDebugMode.Size = new System.Drawing.Size(126, 25);
			this.chkBoxDebugMode.TabIndex = 14;
			this.chkBoxDebugMode.Text = "Debug mode";
			// 
			// SettingsDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(8, 21);
			this.ClientSize = new System.Drawing.Size(681, 663);
			this.Controls.Add(this.chkBoxDebugMode);
			this.Controls.Add(this.labelFile);
			this.Controls.Add(this.MainBtnDelete);
			this.Controls.Add(this.Tabs);
			this.Controls.Add(this.buttonApply);
			this.Controls.Add(this.MainCmbBox);
			this.Controls.Add(this.MainBtnSaveAs);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MinimumSize = new System.Drawing.Size(691, 402);
			this.Name = "SettingsDlg";
			this.Text = "Settings";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.SettingsDlg_Closing);
			this.TabColors.ResumeLayout(false);
			this.TabColors.PerformLayout();
			this.TabEq.ResumeLayout(false);
			this.TabEq.PerformLayout();
			this.Tabs.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
        }
		#endregion
		
		#region Main
		
		private void SaveSettings()
		{
			if (MainCmbBox.Text != "")
				SaveTo(dir + @"\" + MainCmbBox.Text + ext);
		} 
		
		void SaveTo(string filename)
		{
			settings.lastEquation = TabEqCmbBox.Text;
			settings.lastColorPalete = TabColorsCmbBox.Text;

			Stream stream = File.Open(filename, FileMode.Create);
			SoapFormatter formatter = new SoapFormatter();
			formatter.Serialize(stream, settings);
			stream.Close();
		}
		
		void LoadFrom(string filename)
		{
			if (!File.Exists(filename)) return;
			suspendUpdate = true;
			Stream stream = File.Open(filename, FileMode.Open);
			SoapFormatter formatter = new SoapFormatter();
			settings = (Settings)formatter.Deserialize(stream);
			stream.Close();
			
			SetMaps();
			
			FillList(TabEqCmbBox);
			FillList(TabColorsCmbBox);
			
			TabEqCmbBox.Text = settings.lastEquation;
			TabColorsCmbBox.Text = settings.lastColorPalete;
			suspendUpdate = false;
		}
		
		private void MainCmbBox_DropDown(object sender, System.EventArgs e)
		{
			string selected = MainCmbBox.Text;
			MainCmbBox.Items.Clear();
			foreach (string file in System.IO.Directory.GetFiles(dir,"*" + ext))
				MainCmbBox.Items.Add(Path.GetFileName(file).Replace(ext,""));		
			 MainCmbBox.Text = selected;
		}
		
		private void MainCmbBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string filename = dir + @"\" + MainCmbBox.Text + ext;
			LoadFrom(filename);
			
			FireUpdate();
		}
		
		private void MainBtnSaveAs_Click(object sender, System.EventArgs e)
		{
			string filename = InputBox.Show("Please enter name",MainCmbBox.Text,"Save as...");
			if (filename == string.Empty) return;
			if (filename == "") return;			
			if (File.Exists(filename))
				if (MessageBox.Show("Overwrite file " + filename + " ?","File already exists",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No)
					return;
			
			SaveTo(dir + @"\" + filename + ext);
		}
		
		private void MainBtnDelete_Click(object sender, System.EventArgs e)
		{
			string filename = dir + @"\" + MainCmbBox.Text + ext;
			if (!File.Exists(filename)) return;
			if (MessageBox.Show("Do you want to delete file " + filename + " ?","Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No) return;
			File.Delete (filename);		
			MainCmbBox_DropDown(null,null);
		}	
		#endregion
		
		#region Tabs
		
		private void DropDown(object sender, System.EventArgs e)
		{
			//if (settings.equations[TabEqCmbBox.Text] != TabEqCode.Text)
			//	if (MessageBox.Show("Your code has changed. Do you want to save it now?","Change",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
			//		TabEqBtnSaveAs.PerformClick();
			
			FillList((ComboBox)sender);
		}
		
		private void FillList (ComboBox cb)
		{
			string selected = cb.Text;
			cb.Items.Clear();
			foreach (DictionaryEntry e in ((Hashtable)datamap[cb.Parent]))
				cb.Items.Add(e.Key);
			cb.Text = selected;
		}
		
		private void SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ComboBox cb = (ComboBox)sender;
			Control tab = cb.Parent;
			
			if (cb.Text == null) return;
			if (cb.Text == "") return;
			
			if (codemap[tab] != null)
				((TextBox)codemap[tab]).Text = (string)((Hashtable)datamap[tab])[cb.Text];
			
			FireUpdate();
		}
		
		private void Save_Click(object sender, System.EventArgs e)
		{
			Control tab = ((Control)sender).Parent;
			ComboBox cb = ((ComboBox)cbmap[tab]);
			
			if (cb.Text != null)
				SaveTo(tab,cb.Text);
		}
		
		private void SaveAs_Click(object sender, System.EventArgs e)
		{
			Control tab = ((Control)sender).Parent;
			ComboBox cb = ((ComboBox)cbmap[tab]);
			
			string name = InputBox.Show("Input name:", cb.Text, "Save as...");
			if (name == String.Empty) return;
			if (name == "") return;
			if (((Hashtable)datamap[tab])[name] != null)
				if (MessageBox.Show("Overwrite " + name + " ?","Already exists",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No)
					return;
			
			SaveTo(tab,name);
			
			SaveSettings();
			FillList(cb);
			cb.Text = name;
		}
		
		private void SaveTo(object tab, string name)
		{
			ComboBox cb = ((ComboBox)cbmap[tab]);
			
			if (codemap[tab] != null)
				((Hashtable)datamap[tab])[name] = ((TextBox)codemap[tab]).Text;
			
			SaveSettings();
		}
		
		private void Delete_Click(object sender, System.EventArgs e)
		{
			Control tab = ((Control)sender).Parent;
			ComboBox cb = ((ComboBox)cbmap[tab]);
			
			if (MessageBox.Show("Delete " + cb.Text + " ?","Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No)
				return;
			
			((Hashtable)datamap[tab]).Remove (cb.Text);
			cb.Text = null;
			
			SaveSettings();
		}
		
		#endregion
		
		#region Code Compiler
		
		private DataGenerator.dlgtGetIndex getColorIndex;
		public DataGenerator.dlgtGetIndex GetColorIndex
		{
			get
			{
				if (method == null)
					UpdateMethod();
				return getColorIndex;
			}
		}
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
		
		public Color[] colorPalette;
		
		private void FireUpdate()
		{
			method = null;
			if (ViewChanged != null)
				ViewChanged();
		}
		
		private void UpdateMethod()
		{
			if (suspendUpdate) return;
			// Make source code
			string tmpCode = "using System;using System.Drawing;using System.Drawing.Imaging;" +
				             "namespace Fractals {class Main{" +
				             TabEqCode.Text + "\n" + TabColorsCode.Text + "\n}}";
			
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
		
		#endregion
		
		private void buttonApply_Click(object sender, System.EventArgs e)
		{
			FireUpdate();
		}
		
		private void SettingsDlg_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}
	}
}
