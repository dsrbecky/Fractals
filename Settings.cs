using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;


namespace Fractals
{
	/// <summary>
	/// Summary description for Settings.
	/// </summary>
	public class Settings : System.Windows.Forms.Form
	{
		string path = Assembly.GetCallingAssembly().Location;
		string dir = Path.GetDirectoryName (Assembly.GetCallingAssembly().Location);
		string ext = ".xml";

		public Form formToUpdate = null;

		public View view
		{
			get
			{
				View tmp;
				tmp =  new View((double)Xpos.Value,
								(double)Ypos.Value,
								(double)Xzoom.Value,
								(double)Yzoom.Value,
								textBoxCode.Text);
				tmp.AAX = (int)AAlevel.Value;
				tmp.AAY = (int)AAlevel.Value;
				return tmp;
			}
			set
			{
				Xpos.Maximum = Ypos.Maximum = Xzoom.Maximum = Yzoom.Maximum = decimal.MaxValue;
				Xpos.Minimum = Ypos.Minimum = Xzoom.Minimum = Yzoom.Minimum = decimal.MinValue;
				Xpos.Value = (decimal)value.Xpos;
				Ypos.Value = (decimal)value.Ypos;
				Xzoom.Value = (decimal)value.Xzoom;
				Yzoom.Value = (decimal)value.Yzoom;
				AAlevel.Value = (decimal)value.AAX;
				textBoxCode.Text = value.Code;
			}
		}

		private System.Windows.Forms.GroupBox groupBoxView;
		private System.Windows.Forms.GroupBox groupBoxCode;
		public System.Windows.Forms.ComboBox comboBoxSettings;
		private System.Windows.Forms.Button buttonDelete;
		public System.Windows.Forms.NumericUpDown Xpos;
		public System.Windows.Forms.NumericUpDown Ypos;
		public System.Windows.Forms.NumericUpDown Xzoom;
		public System.Windows.Forms.NumericUpDown Yzoom;
		public System.Windows.Forms.TextBox textBoxCode;
		private System.Windows.Forms.Label labelXpos;
		private System.Windows.Forms.Label labelYpos;
		private System.Windows.Forms.Label labelXzoom;
		private System.Windows.Forms.Label labelYzoom;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonApply;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label labelAA;
		private System.Windows.Forms.NumericUpDown AAlevel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Settings()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			this.groupBoxView = new System.Windows.Forms.GroupBox();
			this.labelYzoom = new System.Windows.Forms.Label();
			this.labelXzoom = new System.Windows.Forms.Label();
			this.labelYpos = new System.Windows.Forms.Label();
			this.labelXpos = new System.Windows.Forms.Label();
			this.Yzoom = new System.Windows.Forms.NumericUpDown();
			this.Xzoom = new System.Windows.Forms.NumericUpDown();
			this.Ypos = new System.Windows.Forms.NumericUpDown();
			this.Xpos = new System.Windows.Forms.NumericUpDown();
			this.groupBoxCode = new System.Windows.Forms.GroupBox();
			this.textBoxCode = new System.Windows.Forms.TextBox();
			this.comboBoxSettings = new System.Windows.Forms.ComboBox();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonApply = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.labelAA = new System.Windows.Forms.Label();
			this.AAlevel = new System.Windows.Forms.NumericUpDown();
			this.groupBoxView.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Yzoom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Xzoom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Ypos)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Xpos)).BeginInit();
			this.groupBoxCode.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.AAlevel)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBoxView
			// 
			this.groupBoxView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxView.Controls.Add(this.AAlevel);
			this.groupBoxView.Controls.Add(this.labelAA);
			this.groupBoxView.Controls.Add(this.labelYzoom);
			this.groupBoxView.Controls.Add(this.labelXzoom);
			this.groupBoxView.Controls.Add(this.labelYpos);
			this.groupBoxView.Controls.Add(this.labelXpos);
			this.groupBoxView.Controls.Add(this.Yzoom);
			this.groupBoxView.Controls.Add(this.Xzoom);
			this.groupBoxView.Controls.Add(this.Ypos);
			this.groupBoxView.Controls.Add(this.Xpos);
			this.groupBoxView.Location = new System.Drawing.Point(8, 32);
			this.groupBoxView.Name = "groupBoxView";
			this.groupBoxView.Size = new System.Drawing.Size(544, 144);
			this.groupBoxView.TabIndex = 0;
			this.groupBoxView.TabStop = false;
			this.groupBoxView.Text = "View";
			// 
			// labelYzoom
			// 
			this.labelYzoom.Location = new System.Drawing.Point(8, 88);
			this.labelYzoom.Name = "labelYzoom";
			this.labelYzoom.TabIndex = 7;
			this.labelYzoom.Text = "Y zoom:";
			// 
			// labelXzoom
			// 
			this.labelXzoom.Location = new System.Drawing.Point(8, 64);
			this.labelXzoom.Name = "labelXzoom";
			this.labelXzoom.TabIndex = 6;
			this.labelXzoom.Text = "X zoom:";
			// 
			// labelYpos
			// 
			this.labelYpos.Location = new System.Drawing.Point(8, 40);
			this.labelYpos.Name = "labelYpos";
			this.labelYpos.TabIndex = 5;
			this.labelYpos.Text = "Y position:";
			// 
			// labelXpos
			// 
			this.labelXpos.Location = new System.Drawing.Point(8, 16);
			this.labelXpos.Name = "labelXpos";
			this.labelXpos.TabIndex = 4;
			this.labelXpos.Text = "X position:";
			// 
			// Yzoom
			// 
			this.Yzoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.Yzoom.Location = new System.Drawing.Point(112, 88);
			this.Yzoom.Maximum = new System.Decimal(new int[] {
																  100000000,
																  0,
																  0,
																  0});
			this.Yzoom.Minimum = new System.Decimal(new int[] {
																  1,
																  0,
																  0,
																  196608});
			this.Yzoom.Name = "Yzoom";
			this.Yzoom.Size = new System.Drawing.Size(424, 20);
			this.Yzoom.TabIndex = 3;
			this.Yzoom.Value = new System.Decimal(new int[] {
																1,
																0,
																0,
																196608});
			// 
			// Xzoom
			// 
			this.Xzoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.Xzoom.Location = new System.Drawing.Point(112, 64);
			this.Xzoom.Maximum = new System.Decimal(new int[] {
																  1000000000,
																  0,
																  0,
																  0});
			this.Xzoom.Minimum = new System.Decimal(new int[] {
																  1,
																  0,
																  0,
																  196608});
			this.Xzoom.Name = "Xzoom";
			this.Xzoom.Size = new System.Drawing.Size(424, 20);
			this.Xzoom.TabIndex = 2;
			this.Xzoom.Value = new System.Decimal(new int[] {
																1,
																0,
																0,
																196608});
			// 
			// Ypos
			// 
			this.Ypos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.Ypos.DecimalPlaces = 10;
			this.Ypos.Increment = new System.Decimal(new int[] {
																   1,
																   0,
																   0,
																   65536});
			this.Ypos.Location = new System.Drawing.Point(112, 40);
			this.Ypos.Minimum = new System.Decimal(new int[] {
																 100,
																 0,
																 0,
																 -2147483648});
			this.Ypos.Name = "Ypos";
			this.Ypos.Size = new System.Drawing.Size(424, 20);
			this.Ypos.TabIndex = 1;
			// 
			// Xpos
			// 
			this.Xpos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.Xpos.DecimalPlaces = 10;
			this.Xpos.Increment = new System.Decimal(new int[] {
																   1,
																   0,
																   0,
																   65536});
			this.Xpos.Location = new System.Drawing.Point(112, 16);
			this.Xpos.Minimum = new System.Decimal(new int[] {
																 100,
																 0,
																 0,
																 -2147483648});
			this.Xpos.Name = "Xpos";
			this.Xpos.Size = new System.Drawing.Size(424, 20);
			this.Xpos.TabIndex = 0;
			// 
			// groupBoxCode
			// 
			this.groupBoxCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxCode.Controls.Add(this.textBoxCode);
			this.groupBoxCode.Location = new System.Drawing.Point(8, 184);
			this.groupBoxCode.Name = "groupBoxCode";
			this.groupBoxCode.Size = new System.Drawing.Size(544, 344);
			this.groupBoxCode.TabIndex = 1;
			this.groupBoxCode.TabStop = false;
			this.groupBoxCode.Text = "Code (C# language)";
			// 
			// textBoxCode
			// 
			this.textBoxCode.AcceptsReturn = true;
			this.textBoxCode.AcceptsTab = true;
			this.textBoxCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxCode.Location = new System.Drawing.Point(8, 16);
			this.textBoxCode.Multiline = true;
			this.textBoxCode.Name = "textBoxCode";
			this.textBoxCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxCode.Size = new System.Drawing.Size(528, 320);
			this.textBoxCode.TabIndex = 0;
			this.textBoxCode.Text = @"public static void GetColor(double p, double q,out double r,out double g,out double b)
{
	double bigX,bigY;
	double x=0,y=0;
	r = 0; g = 0; b = 255;
	for (int i = 0;i <= 64;i++)
	{
		bigX = x*x - y*y + p;
		bigY = 2*x*y + q;
		if (double.IsNaN(bigX) || double.IsNaN(bigY))
		{
			b = i*6;
			return;
		}
		x = bigX;
		y = bigY;
	}
	return;
}";
			this.textBoxCode.WordWrap = false;
			// 
			// comboBoxSettings
			// 
			this.comboBoxSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxSettings.Location = new System.Drawing.Point(8, 8);
			this.comboBoxSettings.Name = "comboBoxSettings";
			this.comboBoxSettings.Size = new System.Drawing.Size(512, 21);
			this.comboBoxSettings.TabIndex = 2;
			this.comboBoxSettings.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxSettings_KeyDown);
			this.comboBoxSettings.DropDown += new System.EventHandler(this.comboBoxSettings_DropDown);
			this.comboBoxSettings.SelectedIndexChanged += new System.EventHandler(this.comboBoxSettings_SelectedIndexChanged);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDelete.Location = new System.Drawing.Point(528, 8);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(24, 23);
			this.buttonDelete.TabIndex = 3;
			this.buttonDelete.Text = "X";
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(312, 536);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "&OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonApply
			// 
			this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonApply.Location = new System.Drawing.Point(392, 536);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.TabIndex = 5;
			this.buttonApply.Text = "&Apply";
			this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(472, 536);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// labelAA
			// 
			this.labelAA.Location = new System.Drawing.Point(8, 112);
			this.labelAA.Name = "labelAA";
			this.labelAA.TabIndex = 8;
			this.labelAA.Text = "Anti-aliasing level:";
			// 
			// AAlevel
			// 
			this.AAlevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.AAlevel.Location = new System.Drawing.Point(112, 112);
			this.AAlevel.Maximum = new System.Decimal(new int[] {
																	16,
																	0,
																	0,
																	0});
			this.AAlevel.Minimum = new System.Decimal(new int[] {
																	1,
																	0,
																	0,
																	0});
			this.AAlevel.Name = "AAlevel";
			this.AAlevel.Size = new System.Drawing.Size(424, 20);
			this.AAlevel.TabIndex = 9;
			this.AAlevel.Value = new System.Decimal(new int[] {
																  1,
																  0,
																  0,
																  0});
			// 
			// Settings
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(560, 566);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonApply);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonDelete);
			this.Controls.Add(this.comboBoxSettings);
			this.Controls.Add(this.groupBoxCode);
			this.Controls.Add(this.groupBoxView);
			this.Name = "Settings";
			this.Text = "Settings";
			this.groupBoxView.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.Yzoom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Xzoom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Ypos)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Xpos)).EndInit();
			this.groupBoxCode.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.AAlevel)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void comboBoxSettings_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				SaveTo(dir + @"\" + comboBoxSettings.Text + ext);
		}

		private void comboBoxSettings_DropDown(object sender, System.EventArgs e)
		{
			FillComboBox();
		}

		void FillComboBox()
		{
			comboBoxSettings.Items.Clear();
			foreach (string file in System.IO.Directory.GetFiles(dir,"*" + ext))
				comboBoxSettings.Items.Add(Path.GetFileNameWithoutExtension(file));
		}

		void SaveTo(string filename)
		{
			if (File.Exists(filename))
				if (MessageBox.Show("Overwrite file :" + filename + " ?","File already exists",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No)
					return;

			Stream stream = File.Open(filename, FileMode.Create);
			SoapFormatter formatter = new SoapFormatter();
			formatter.Serialize(stream, view);
			stream.Close();
			MessageBox.Show ("Saved to: " + filename);
		}

		void LoadFrom(string filename)
		{
			if (!File.Exists(filename)) return;
			Stream stream = File.Open(filename, FileMode.Open);
			SoapFormatter formatter = new SoapFormatter();
			view = (View)formatter.Deserialize(stream);
			stream.Close();
		}

		private void comboBoxSettings_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string filename = dir + @"\" + comboBoxSettings.Text + ext;
			LoadFrom(filename);
			formToUpdate.view = view;
			formToUpdate.RefreshImage();
		}

		private void buttonDelete_Click(object sender, System.EventArgs e)
		{
			string filename = dir + @"\" + comboBoxSettings.Text + ext;
			if (!File.Exists(filename)) return;
			if (MessageBox.Show("Delete file :" + filename + " ?","File already exists",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No)
				return;
			File.Delete (filename);
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			buttonApply.PerformClick();
		}

		private void buttonApply_Click(object sender, System.EventArgs e)
		{
			formToUpdate.view = view;
			formToUpdate.RefreshImage();
		}

	}
}
