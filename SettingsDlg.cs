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
		string path = Assembly.GetCallingAssembly().Location;
		string dir = Path.GetDirectoryName (Assembly.GetCallingAssembly().Location);
		string ext = ".fractal.xml";

		public event EventHandlerNoArg ViewChanged;

		public Settings settings;
		public bool suspendUpdate = false;

		Hashtable datamap;
		Hashtable codemap;
		Hashtable cbmap;

		public SavingRules savingRules {get{return new SavingRules();}set{;}}
		public View view
		{
			get
			{
				View _view;
				_view.Xpos = (double)TabViewXpos.Value;
				_view.Ypos = (double)TabViewYpos.Value;
				_view.Xzoom = (double)TabViewXzoom.Value;
				_view.Yzoom = (double)TabViewYzoom.Value;

				//TODO: do the matrix stuf
				_view.Angle = TabViewAngle.Value;
				_view.m11 = 0;_view.m12 = 0;_view.m21 = 0;_view.m22 = 0;
				//TODO: set proper values
				_view.antiAliasingLevel = 4;
				_view.edgeOnlyAntiAliasing = true;
				
				return _view;
			}
			set
			{
				TabViewXpos.Maximum = TabViewYpos.Maximum = TabViewXzoom.Maximum = TabViewYzoom.Maximum = decimal.MaxValue;
				TabViewXpos.Minimum = TabViewYpos.Minimum = TabViewXzoom.Minimum = TabViewYzoom.Minimum = decimal.MinValue;
				TabViewXpos.Value = (decimal)value.Xpos;
				TabViewYpos.Value = (decimal)value.Ypos;
				TabViewXzoom.Value = (decimal)value.Xzoom;
				TabViewYzoom.Value = (decimal)value.Yzoom;
				TabViewAngle.Value = ((int)value.Angle+360)%360;
				//TODO: set proper values of AA
				
			}
		}

		#region Controls

		private System.Windows.Forms.Button buttonApply;
		private System.Windows.Forms.Label labelAA;
		private System.Windows.Forms.Label labelYzoom;
		private System.Windows.Forms.Label labelXzoom;
		private System.Windows.Forms.Label labelYpos;
		private System.Windows.Forms.Label labelXpos;
		public System.Windows.Forms.ComboBox MainCmbBox;
		private System.Windows.Forms.TabPage TabView;
		private System.Windows.Forms.TabPage TabEq;
		private System.Windows.Forms.TabPage TabColors;
		public System.Windows.Forms.NumericUpDown TabViewYzoom;
		public System.Windows.Forms.NumericUpDown TabViewXzoom;
		public System.Windows.Forms.NumericUpDown TabViewYpos;
		public System.Windows.Forms.NumericUpDown TabViewXpos;
		private System.Windows.Forms.ComboBox TabViewAA;
		public System.Windows.Forms.TextBox TabEqCode;
		private System.Windows.Forms.TabPage TabAnim;
		private System.Windows.Forms.TabPage TabSave;
		private System.Windows.Forms.ComboBox TabEqCmbBox;
		private System.Windows.Forms.Button TabEqBtnSave;
		private System.Windows.Forms.Button TabEqBtnSaveAs;
		private System.Windows.Forms.Button TabEqBtnDelete;
		private System.Windows.Forms.Button TabViewBtnDelete;
		private System.Windows.Forms.Button TabViewBtnSaveAs;
		private System.Windows.Forms.Button TabViewBtnSave;
		private System.Windows.Forms.ComboBox TabViewCmbBox;
		private System.Windows.Forms.Button TabColorsBtnDelete;
		private System.Windows.Forms.Button TabColorsBtnSaveAs;
		private System.Windows.Forms.Button TabColorsBtnSave;
		private System.Windows.Forms.ComboBox TabColorsCmbBox;
		private System.Windows.Forms.Button TabAnimBtnDelete;
		private System.Windows.Forms.Button TabAnimBtnSaveAs;
		private System.Windows.Forms.Button TabAnimBtnSave;
		private System.Windows.Forms.ComboBox TabAnimCmbBox;
		private System.Windows.Forms.Button TabSaveBtnDelete;
		private System.Windows.Forms.Button TabSaveBtnSaveAs;
		private System.Windows.Forms.Button TabSaveBtnSave;
		private System.Windows.Forms.ComboBox TabSaveCmbBox;
		private System.Windows.Forms.CheckBox TabColorsOtherFiles;
		public System.Windows.Forms.TextBox TabColorsCode;
		private System.Windows.Forms.TabControl Tabs;
		private System.Windows.Forms.Button MainBtnDelete;
		private System.Windows.Forms.Label labelFile;
		private System.Windows.Forms.TabPage TabCustom;
		private System.Windows.Forms.Button TabCustomBtnDelete;
		private System.Windows.Forms.Button TabCustomBtnSaveAs;
		private System.Windows.Forms.Button TabCustomBtnSave;
		private System.Windows.Forms.ComboBox TabCustomCmbBox;
		public System.Windows.Forms.TextBox TabCustomCode;
		private System.Windows.Forms.TabPage TabComments;
		public System.Windows.Forms.TextBox TabCommentsTextbox;
		private System.Windows.Forms.Label labelFilename;
		private System.Windows.Forms.TextBox TabSaveFilename;
		private System.Windows.Forms.CheckBox TabSaveUniqueFileName;
		private System.Windows.Forms.Label labelResulution;
		private System.Windows.Forms.Panel TabSavePanelResulution;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.NumericUpDown numericUpDown2;
		private System.Windows.Forms.Label labelX;
		private System.Windows.Forms.RadioButton TabSaveRBtnResCustom;
		private System.Windows.Forms.RadioButton TabSaveRBtnResDestop;
		private System.Windows.Forms.RadioButton TabSaveRBtnResCurrect;
		private System.Windows.Forms.Label labelSaveAA;
		private System.Windows.Forms.ComboBox TabSaveAA;
		private System.Windows.Forms.Label labelFormat;
		private System.Windows.Forms.ComboBox TabSaveFileformat;
		private System.Windows.Forms.Button MainBtnSaveAs;
		private System.Windows.Forms.Label labelAngle;
		private System.Windows.Forms.TrackBar TabViewAngle;
		
		private System.ComponentModel.Container components = null;

		#endregion

		public SettingsDlg()
		{
			InitializeComponent();

			settings.equations = new Hashtable();
			settings.views = new Hashtable();
			settings.colorPaletes = new Hashtable();
			settings.customCodes = new Hashtable();
			settings.savingRules = new Hashtable();
			SetMaps();
		}

		void SetMaps()
		{
			datamap = new Hashtable();
			datamap.Add (TabEq, settings.equations);
			datamap.Add (TabView, settings.views);
			datamap.Add (TabColors, settings.colorPaletes);
			datamap.Add (TabCustom, settings.customCodes);
			datamap.Add (TabSave, settings.savingRules);

			cbmap = new Hashtable();
			cbmap.Add (TabEq, TabEqCmbBox);
			cbmap.Add (TabView, TabViewCmbBox);
			cbmap.Add (TabColors, TabColorsCmbBox);
			cbmap.Add (TabCustom, TabCustomCmbBox);
			cbmap.Add (TabSave, TabSaveCmbBox);

			codemap = new Hashtable();
			codemap.Add (TabEq, TabEqCode);
			codemap.Add (TabColors, TabColorsCode);
			codemap.Add (TabCustom, TabCustomCode);
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
			this.MainCmbBox = new System.Windows.Forms.ComboBox();
			this.buttonApply = new System.Windows.Forms.Button();
			this.Tabs = new System.Windows.Forms.TabControl();
			this.TabEq = new System.Windows.Forms.TabPage();
			this.TabEqBtnDelete = new System.Windows.Forms.Button();
			this.TabEqBtnSaveAs = new System.Windows.Forms.Button();
			this.TabEqBtnSave = new System.Windows.Forms.Button();
			this.TabEqCmbBox = new System.Windows.Forms.ComboBox();
			this.TabEqCode = new System.Windows.Forms.TextBox();
			this.TabView = new System.Windows.Forms.TabPage();
			this.TabViewBtnDelete = new System.Windows.Forms.Button();
			this.TabViewBtnSaveAs = new System.Windows.Forms.Button();
			this.TabViewBtnSave = new System.Windows.Forms.Button();
			this.TabViewCmbBox = new System.Windows.Forms.ComboBox();
			this.TabViewAA = new System.Windows.Forms.ComboBox();
			this.labelAA = new System.Windows.Forms.Label();
			this.labelYzoom = new System.Windows.Forms.Label();
			this.labelXzoom = new System.Windows.Forms.Label();
			this.labelYpos = new System.Windows.Forms.Label();
			this.labelXpos = new System.Windows.Forms.Label();
			this.TabViewYzoom = new System.Windows.Forms.NumericUpDown();
			this.TabViewXzoom = new System.Windows.Forms.NumericUpDown();
			this.TabViewYpos = new System.Windows.Forms.NumericUpDown();
			this.TabViewXpos = new System.Windows.Forms.NumericUpDown();
			this.TabAnim = new System.Windows.Forms.TabPage();
			this.TabAnimBtnDelete = new System.Windows.Forms.Button();
			this.TabAnimBtnSaveAs = new System.Windows.Forms.Button();
			this.TabAnimBtnSave = new System.Windows.Forms.Button();
			this.TabAnimCmbBox = new System.Windows.Forms.ComboBox();
			this.TabColors = new System.Windows.Forms.TabPage();
			this.TabColorsCode = new System.Windows.Forms.TextBox();
			this.TabColorsOtherFiles = new System.Windows.Forms.CheckBox();
			this.TabColorsBtnDelete = new System.Windows.Forms.Button();
			this.TabColorsBtnSaveAs = new System.Windows.Forms.Button();
			this.TabColorsBtnSave = new System.Windows.Forms.Button();
			this.TabColorsCmbBox = new System.Windows.Forms.ComboBox();
			this.TabCustom = new System.Windows.Forms.TabPage();
			this.TabCustomCode = new System.Windows.Forms.TextBox();
			this.TabCustomBtnDelete = new System.Windows.Forms.Button();
			this.TabCustomBtnSaveAs = new System.Windows.Forms.Button();
			this.TabCustomBtnSave = new System.Windows.Forms.Button();
			this.TabCustomCmbBox = new System.Windows.Forms.ComboBox();
			this.TabSave = new System.Windows.Forms.TabPage();
			this.TabSaveFileformat = new System.Windows.Forms.ComboBox();
			this.labelFormat = new System.Windows.Forms.Label();
			this.TabSaveAA = new System.Windows.Forms.ComboBox();
			this.labelSaveAA = new System.Windows.Forms.Label();
			this.TabSavePanelResulution = new System.Windows.Forms.Panel();
			this.TabSaveRBtnResCurrect = new System.Windows.Forms.RadioButton();
			this.TabSaveRBtnResDestop = new System.Windows.Forms.RadioButton();
			this.TabSaveRBtnResCustom = new System.Windows.Forms.RadioButton();
			this.labelX = new System.Windows.Forms.Label();
			this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.labelResulution = new System.Windows.Forms.Label();
			this.TabSaveUniqueFileName = new System.Windows.Forms.CheckBox();
			this.TabSaveFilename = new System.Windows.Forms.TextBox();
			this.labelFilename = new System.Windows.Forms.Label();
			this.TabSaveBtnDelete = new System.Windows.Forms.Button();
			this.TabSaveBtnSaveAs = new System.Windows.Forms.Button();
			this.TabSaveBtnSave = new System.Windows.Forms.Button();
			this.TabSaveCmbBox = new System.Windows.Forms.ComboBox();
			this.TabComments = new System.Windows.Forms.TabPage();
			this.TabCommentsTextbox = new System.Windows.Forms.TextBox();
			this.MainBtnDelete = new System.Windows.Forms.Button();
			this.labelFile = new System.Windows.Forms.Label();
			this.MainBtnSaveAs = new System.Windows.Forms.Button();
			this.labelAngle = new System.Windows.Forms.Label();
			this.TabViewAngle = new System.Windows.Forms.TrackBar();
			this.Tabs.SuspendLayout();
			this.TabEq.SuspendLayout();
			this.TabView.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.TabViewYzoom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.TabViewXzoom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.TabViewYpos)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.TabViewXpos)).BeginInit();
			this.TabAnim.SuspendLayout();
			this.TabColors.SuspendLayout();
			this.TabCustom.SuspendLayout();
			this.TabSave.SuspendLayout();
			this.TabSavePanelResulution.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.TabComments.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.TabViewAngle)).BeginInit();
			this.SuspendLayout();
			// 
			// MainCmbBox
			// 
			this.MainCmbBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.MainCmbBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.MainCmbBox.Location = new System.Drawing.Point(56, 8);
			this.MainCmbBox.Name = "MainCmbBox";
			this.MainCmbBox.Size = new System.Drawing.Size(192, 21);
			this.MainCmbBox.Sorted = true;
			this.MainCmbBox.TabIndex = 2;
			this.MainCmbBox.DropDown += new System.EventHandler(this.MainCmbBox_DropDown);
			this.MainCmbBox.SelectedIndexChanged += new System.EventHandler(this.MainCmbBox_SelectedIndexChanged);
			// 
			// buttonApply
			// 
			this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonApply.Location = new System.Drawing.Point(256, 432);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(160, 23);
			this.buttonApply.TabIndex = 5;
			this.buttonApply.Text = "&Apply code changes";
			this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
			// 
			// Tabs
			// 
			this.Tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.Tabs.Controls.Add(this.TabEq);
			this.Tabs.Controls.Add(this.TabView);
			this.Tabs.Controls.Add(this.TabAnim);
			this.Tabs.Controls.Add(this.TabColors);
			this.Tabs.Controls.Add(this.TabCustom);
			this.Tabs.Controls.Add(this.TabSave);
			this.Tabs.Controls.Add(this.TabComments);
			this.Tabs.Location = new System.Drawing.Point(8, 40);
			this.Tabs.Name = "Tabs";
			this.Tabs.SelectedIndex = 0;
			this.Tabs.Size = new System.Drawing.Size(408, 384);
			this.Tabs.TabIndex = 8;
			// 
			// TabEq
			// 
			this.TabEq.Controls.Add(this.TabEqBtnDelete);
			this.TabEq.Controls.Add(this.TabEqBtnSaveAs);
			this.TabEq.Controls.Add(this.TabEqBtnSave);
			this.TabEq.Controls.Add(this.TabEqCmbBox);
			this.TabEq.Controls.Add(this.TabEqCode);
			this.TabEq.Location = new System.Drawing.Point(4, 22);
			this.TabEq.Name = "TabEq";
			this.TabEq.Size = new System.Drawing.Size(400, 358);
			this.TabEq.TabIndex = 5;
			this.TabEq.Text = "Equation";
			// 
			// TabEqBtnDelete
			// 
			this.TabEqBtnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabEqBtnDelete.Location = new System.Drawing.Point(320, 8);
			this.TabEqBtnDelete.Name = "TabEqBtnDelete";
			this.TabEqBtnDelete.TabIndex = 5;
			this.TabEqBtnDelete.Text = "Delete";
			this.TabEqBtnDelete.Click += new System.EventHandler(this.Delete_Click);
			// 
			// TabEqBtnSaveAs
			// 
			this.TabEqBtnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabEqBtnSaveAs.Location = new System.Drawing.Point(240, 8);
			this.TabEqBtnSaveAs.Name = "TabEqBtnSaveAs";
			this.TabEqBtnSaveAs.TabIndex = 4;
			this.TabEqBtnSaveAs.Text = "Save as ...";
			this.TabEqBtnSaveAs.Click += new System.EventHandler(this.SaveAs_Click);
			// 
			// TabEqBtnSave
			// 
			this.TabEqBtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabEqBtnSave.Location = new System.Drawing.Point(160, 8);
			this.TabEqBtnSave.Name = "TabEqBtnSave";
			this.TabEqBtnSave.TabIndex = 3;
			this.TabEqBtnSave.Text = "Save";
			this.TabEqBtnSave.Click += new System.EventHandler(this.Save_Click);
			// 
			// TabEqCmbBox
			// 
			this.TabEqCmbBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabEqCmbBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TabEqCmbBox.Location = new System.Drawing.Point(8, 8);
			this.TabEqCmbBox.Name = "TabEqCmbBox";
			this.TabEqCmbBox.Size = new System.Drawing.Size(144, 21);
			this.TabEqCmbBox.Sorted = true;
			this.TabEqCmbBox.TabIndex = 2;
			this.TabEqCmbBox.DropDown += new System.EventHandler(this.DropDown);
			this.TabEqCmbBox.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
			// 
			// TabEqCode
			// 
			this.TabEqCode.AcceptsReturn = true;
			this.TabEqCode.AcceptsTab = true;
			this.TabEqCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabEqCode.Location = new System.Drawing.Point(8, 40);
			this.TabEqCode.Multiline = true;
			this.TabEqCode.Name = "TabEqCode";
			this.TabEqCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.TabEqCode.Size = new System.Drawing.Size(384, 312);
			this.TabEqCode.TabIndex = 1;
			this.TabEqCode.Text = @"public static void GetColor(double p, double q,out double r,out double g,out double b)
{
	double bigX,bigY;
	double x=0,y=0;
	r = 0; g = 0; b = 255;/*
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
	}*/
	return;
}";
			this.TabEqCode.WordWrap = false;
			// 
			// TabView
			// 
			this.TabView.AutoScroll = true;
			this.TabView.Controls.Add(this.TabViewAngle);
			this.TabView.Controls.Add(this.labelAngle);
			this.TabView.Controls.Add(this.TabViewBtnDelete);
			this.TabView.Controls.Add(this.TabViewBtnSaveAs);
			this.TabView.Controls.Add(this.TabViewBtnSave);
			this.TabView.Controls.Add(this.TabViewCmbBox);
			this.TabView.Controls.Add(this.TabViewAA);
			this.TabView.Controls.Add(this.labelAA);
			this.TabView.Controls.Add(this.labelYzoom);
			this.TabView.Controls.Add(this.labelXzoom);
			this.TabView.Controls.Add(this.labelYpos);
			this.TabView.Controls.Add(this.labelXpos);
			this.TabView.Controls.Add(this.TabViewYzoom);
			this.TabView.Controls.Add(this.TabViewXzoom);
			this.TabView.Controls.Add(this.TabViewYpos);
			this.TabView.Controls.Add(this.TabViewXpos);
			this.TabView.Location = new System.Drawing.Point(4, 22);
			this.TabView.Name = "TabView";
			this.TabView.Size = new System.Drawing.Size(400, 358);
			this.TabView.TabIndex = 4;
			this.TabView.Text = "Views";
			// 
			// TabViewBtnDelete
			// 
			this.TabViewBtnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabViewBtnDelete.Location = new System.Drawing.Point(320, 8);
			this.TabViewBtnDelete.Name = "TabViewBtnDelete";
			this.TabViewBtnDelete.TabIndex = 23;
			this.TabViewBtnDelete.Text = "Delete";
			this.TabViewBtnDelete.Click += new System.EventHandler(this.Delete_Click);
			// 
			// TabViewBtnSaveAs
			// 
			this.TabViewBtnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabViewBtnSaveAs.Location = new System.Drawing.Point(240, 8);
			this.TabViewBtnSaveAs.Name = "TabViewBtnSaveAs";
			this.TabViewBtnSaveAs.TabIndex = 22;
			this.TabViewBtnSaveAs.Text = "Save as ...";
			this.TabViewBtnSaveAs.Click += new System.EventHandler(this.SaveAs_Click);
			// 
			// TabViewBtnSave
			// 
			this.TabViewBtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabViewBtnSave.Location = new System.Drawing.Point(160, 8);
			this.TabViewBtnSave.Name = "TabViewBtnSave";
			this.TabViewBtnSave.TabIndex = 21;
			this.TabViewBtnSave.Text = "Save";
			this.TabViewBtnSave.Click += new System.EventHandler(this.Save_Click);
			// 
			// TabViewCmbBox
			// 
			this.TabViewCmbBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabViewCmbBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TabViewCmbBox.Location = new System.Drawing.Point(8, 8);
			this.TabViewCmbBox.Name = "TabViewCmbBox";
			this.TabViewCmbBox.Size = new System.Drawing.Size(144, 21);
			this.TabViewCmbBox.Sorted = true;
			this.TabViewCmbBox.TabIndex = 20;
			this.TabViewCmbBox.DropDown += new System.EventHandler(this.DropDown);
			this.TabViewCmbBox.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
			// 
			// TabViewAA
			// 
			this.TabViewAA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabViewAA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TabViewAA.Items.AddRange(new object[] {
														   "Disabled",
														   "2x2 fullscreen",
														   "2x2 edge only",
														   "4x4 fullscreen",
														   "4x4 edge only",
														   "8x8 fullscreen",
														   "8x8 edge only",
														   "16x16 fullscreen",
														   "16x16 edge only",
														   "32x32 fullscreen",
														   "32x32 edge only"});
			this.TabViewAA.Location = new System.Drawing.Point(112, 136);
			this.TabViewAA.Name = "TabViewAA";
			this.TabViewAA.Size = new System.Drawing.Size(280, 21);
			this.TabViewAA.TabIndex = 19;
			// 
			// labelAA
			// 
			this.labelAA.Location = new System.Drawing.Point(8, 136);
			this.labelAA.Name = "labelAA";
			this.labelAA.TabIndex = 18;
			this.labelAA.Text = "Anti-aliasing level:";
			// 
			// labelYzoom
			// 
			this.labelYzoom.Location = new System.Drawing.Point(8, 112);
			this.labelYzoom.Name = "labelYzoom";
			this.labelYzoom.TabIndex = 17;
			this.labelYzoom.Text = "Y zoom:";
			// 
			// labelXzoom
			// 
			this.labelXzoom.Location = new System.Drawing.Point(8, 88);
			this.labelXzoom.Name = "labelXzoom";
			this.labelXzoom.TabIndex = 16;
			this.labelXzoom.Text = "X zoom:";
			// 
			// labelYpos
			// 
			this.labelYpos.Location = new System.Drawing.Point(8, 64);
			this.labelYpos.Name = "labelYpos";
			this.labelYpos.TabIndex = 15;
			this.labelYpos.Text = "Y position:";
			// 
			// labelXpos
			// 
			this.labelXpos.Location = new System.Drawing.Point(8, 40);
			this.labelXpos.Name = "labelXpos";
			this.labelXpos.TabIndex = 14;
			this.labelXpos.Text = "X position:";
			// 
			// TabViewYzoom
			// 
			this.TabViewYzoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabViewYzoom.DecimalPlaces = 2;
			this.TabViewYzoom.Location = new System.Drawing.Point(112, 112);
			this.TabViewYzoom.Maximum = new System.Decimal(new int[] {
																		 100000000,
																		 0,
																		 0,
																		 0});
			this.TabViewYzoom.Minimum = new System.Decimal(new int[] {
																		 1,
																		 0,
																		 0,
																		 196608});
			this.TabViewYzoom.Name = "TabViewYzoom";
			this.TabViewYzoom.Size = new System.Drawing.Size(280, 20);
			this.TabViewYzoom.TabIndex = 13;
			this.TabViewYzoom.Value = new System.Decimal(new int[] {
																	   1,
																	   0,
																	   0,
																	   0});
			// 
			// TabViewXzoom
			// 
			this.TabViewXzoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabViewXzoom.DecimalPlaces = 2;
			this.TabViewXzoom.Location = new System.Drawing.Point(112, 88);
			this.TabViewXzoom.Maximum = new System.Decimal(new int[] {
																		 1000000000,
																		 0,
																		 0,
																		 0});
			this.TabViewXzoom.Minimum = new System.Decimal(new int[] {
																		 1,
																		 0,
																		 0,
																		 196608});
			this.TabViewXzoom.Name = "TabViewXzoom";
			this.TabViewXzoom.Size = new System.Drawing.Size(280, 20);
			this.TabViewXzoom.TabIndex = 12;
			this.TabViewXzoom.Value = new System.Decimal(new int[] {
																	   1,
																	   0,
																	   0,
																	   0});
			// 
			// TabViewYpos
			// 
			this.TabViewYpos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabViewYpos.DecimalPlaces = 10;
			this.TabViewYpos.Increment = new System.Decimal(new int[] {
																		  1,
																		  0,
																		  0,
																		  65536});
			this.TabViewYpos.Location = new System.Drawing.Point(112, 64);
			this.TabViewYpos.Minimum = new System.Decimal(new int[] {
																		100,
																		0,
																		0,
																		-2147483648});
			this.TabViewYpos.Name = "TabViewYpos";
			this.TabViewYpos.Size = new System.Drawing.Size(280, 20);
			this.TabViewYpos.TabIndex = 11;
			// 
			// TabViewXpos
			// 
			this.TabViewXpos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabViewXpos.DecimalPlaces = 10;
			this.TabViewXpos.Increment = new System.Decimal(new int[] {
																		  1,
																		  0,
																		  0,
																		  65536});
			this.TabViewXpos.Location = new System.Drawing.Point(112, 40);
			this.TabViewXpos.Minimum = new System.Decimal(new int[] {
																		100,
																		0,
																		0,
																		-2147483648});
			this.TabViewXpos.Name = "TabViewXpos";
			this.TabViewXpos.Size = new System.Drawing.Size(280, 20);
			this.TabViewXpos.TabIndex = 10;
			this.TabViewXpos.Value = new System.Decimal(new int[] {
																	  5,
																	  0,
																	  0,
																	  -2147418112});
			// 
			// TabAnim
			// 
			this.TabAnim.Controls.Add(this.TabAnimBtnDelete);
			this.TabAnim.Controls.Add(this.TabAnimBtnSaveAs);
			this.TabAnim.Controls.Add(this.TabAnimBtnSave);
			this.TabAnim.Controls.Add(this.TabAnimCmbBox);
			this.TabAnim.Location = new System.Drawing.Point(4, 22);
			this.TabAnim.Name = "TabAnim";
			this.TabAnim.Size = new System.Drawing.Size(400, 358);
			this.TabAnim.TabIndex = 7;
			this.TabAnim.Text = "Animation";
			// 
			// TabAnimBtnDelete
			// 
			this.TabAnimBtnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabAnimBtnDelete.Location = new System.Drawing.Point(320, 8);
			this.TabAnimBtnDelete.Name = "TabAnimBtnDelete";
			this.TabAnimBtnDelete.TabIndex = 9;
			this.TabAnimBtnDelete.Text = "Delete";
			this.TabAnimBtnDelete.Click += new System.EventHandler(this.Delete_Click);
			// 
			// TabAnimBtnSaveAs
			// 
			this.TabAnimBtnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabAnimBtnSaveAs.Location = new System.Drawing.Point(240, 8);
			this.TabAnimBtnSaveAs.Name = "TabAnimBtnSaveAs";
			this.TabAnimBtnSaveAs.TabIndex = 8;
			this.TabAnimBtnSaveAs.Text = "Save as ...";
			this.TabAnimBtnSaveAs.Click += new System.EventHandler(this.SaveAs_Click);
			// 
			// TabAnimBtnSave
			// 
			this.TabAnimBtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabAnimBtnSave.Location = new System.Drawing.Point(160, 8);
			this.TabAnimBtnSave.Name = "TabAnimBtnSave";
			this.TabAnimBtnSave.TabIndex = 7;
			this.TabAnimBtnSave.Text = "Save";
			this.TabAnimBtnSave.Click += new System.EventHandler(this.Save_Click);
			// 
			// TabAnimCmbBox
			// 
			this.TabAnimCmbBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabAnimCmbBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TabAnimCmbBox.Location = new System.Drawing.Point(8, 8);
			this.TabAnimCmbBox.Name = "TabAnimCmbBox";
			this.TabAnimCmbBox.Size = new System.Drawing.Size(144, 20);
			this.TabAnimCmbBox.Sorted = true;
			this.TabAnimCmbBox.TabIndex = 6;
			this.TabAnimCmbBox.DropDown += new System.EventHandler(this.DropDown);
			this.TabAnimCmbBox.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
			// 
			// TabColors
			// 
			this.TabColors.Controls.Add(this.TabColorsCode);
			this.TabColors.Controls.Add(this.TabColorsOtherFiles);
			this.TabColors.Controls.Add(this.TabColorsBtnDelete);
			this.TabColors.Controls.Add(this.TabColorsBtnSaveAs);
			this.TabColors.Controls.Add(this.TabColorsBtnSave);
			this.TabColors.Controls.Add(this.TabColorsCmbBox);
			this.TabColors.Location = new System.Drawing.Point(4, 22);
			this.TabColors.Name = "TabColors";
			this.TabColors.Size = new System.Drawing.Size(400, 358);
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
			this.TabColorsCode.Location = new System.Drawing.Point(8, 64);
			this.TabColorsCode.Multiline = true;
			this.TabColorsCode.Name = "TabColorsCode";
			this.TabColorsCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.TabColorsCode.Size = new System.Drawing.Size(384, 288);
			this.TabColorsCode.TabIndex = 11;
			this.TabColorsCode.Text = "";
			this.TabColorsCode.WordWrap = false;
			// 
			// TabColorsOtherFiles
			// 
			this.TabColorsOtherFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabColorsOtherFiles.Location = new System.Drawing.Point(8, 32);
			this.TabColorsOtherFiles.Name = "TabColorsOtherFiles";
			this.TabColorsOtherFiles.Size = new System.Drawing.Size(384, 20);
			this.TabColorsOtherFiles.TabIndex = 10;
			this.TabColorsOtherFiles.Text = "Display pletes from other files";
			// 
			// TabColorsBtnDelete
			// 
			this.TabColorsBtnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabColorsBtnDelete.Location = new System.Drawing.Point(320, 8);
			this.TabColorsBtnDelete.Name = "TabColorsBtnDelete";
			this.TabColorsBtnDelete.TabIndex = 9;
			this.TabColorsBtnDelete.Text = "Delete";
			this.TabColorsBtnDelete.Click += new System.EventHandler(this.Delete_Click);
			// 
			// TabColorsBtnSaveAs
			// 
			this.TabColorsBtnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabColorsBtnSaveAs.Location = new System.Drawing.Point(240, 8);
			this.TabColorsBtnSaveAs.Name = "TabColorsBtnSaveAs";
			this.TabColorsBtnSaveAs.TabIndex = 8;
			this.TabColorsBtnSaveAs.Text = "Save as ...";
			this.TabColorsBtnSaveAs.Click += new System.EventHandler(this.SaveAs_Click);
			// 
			// TabColorsBtnSave
			// 
			this.TabColorsBtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabColorsBtnSave.Location = new System.Drawing.Point(160, 8);
			this.TabColorsBtnSave.Name = "TabColorsBtnSave";
			this.TabColorsBtnSave.TabIndex = 7;
			this.TabColorsBtnSave.Text = "Save";
			this.TabColorsBtnSave.Click += new System.EventHandler(this.Save_Click);
			// 
			// TabColorsCmbBox
			// 
			this.TabColorsCmbBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabColorsCmbBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TabColorsCmbBox.Location = new System.Drawing.Point(8, 8);
			this.TabColorsCmbBox.Name = "TabColorsCmbBox";
			this.TabColorsCmbBox.Size = new System.Drawing.Size(144, 20);
			this.TabColorsCmbBox.Sorted = true;
			this.TabColorsCmbBox.TabIndex = 6;
			this.TabColorsCmbBox.DropDown += new System.EventHandler(this.DropDown);
			this.TabColorsCmbBox.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
			// 
			// TabCustom
			// 
			this.TabCustom.Controls.Add(this.TabCustomCode);
			this.TabCustom.Controls.Add(this.TabCustomBtnDelete);
			this.TabCustom.Controls.Add(this.TabCustomBtnSaveAs);
			this.TabCustom.Controls.Add(this.TabCustomBtnSave);
			this.TabCustom.Controls.Add(this.TabCustomCmbBox);
			this.TabCustom.Location = new System.Drawing.Point(4, 22);
			this.TabCustom.Name = "TabCustom";
			this.TabCustom.Size = new System.Drawing.Size(400, 358);
			this.TabCustom.TabIndex = 9;
			this.TabCustom.Text = "Custom code";
			// 
			// TabCustomCode
			// 
			this.TabCustomCode.AcceptsReturn = true;
			this.TabCustomCode.AcceptsTab = true;
			this.TabCustomCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabCustomCode.Location = new System.Drawing.Point(8, 40);
			this.TabCustomCode.Multiline = true;
			this.TabCustomCode.Name = "TabCustomCode";
			this.TabCustomCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.TabCustomCode.Size = new System.Drawing.Size(384, 312);
			this.TabCustomCode.TabIndex = 12;
			this.TabCustomCode.Text = "";
			this.TabCustomCode.WordWrap = false;
			// 
			// TabCustomBtnDelete
			// 
			this.TabCustomBtnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabCustomBtnDelete.Location = new System.Drawing.Point(320, 8);
			this.TabCustomBtnDelete.Name = "TabCustomBtnDelete";
			this.TabCustomBtnDelete.TabIndex = 9;
			this.TabCustomBtnDelete.Text = "Delete";
			this.TabCustomBtnDelete.Click += new System.EventHandler(this.Delete_Click);
			// 
			// TabCustomBtnSaveAs
			// 
			this.TabCustomBtnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabCustomBtnSaveAs.Location = new System.Drawing.Point(240, 8);
			this.TabCustomBtnSaveAs.Name = "TabCustomBtnSaveAs";
			this.TabCustomBtnSaveAs.TabIndex = 8;
			this.TabCustomBtnSaveAs.Text = "Save as ...";
			this.TabCustomBtnSaveAs.Click += new System.EventHandler(this.SaveAs_Click);
			// 
			// TabCustomBtnSave
			// 
			this.TabCustomBtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabCustomBtnSave.Location = new System.Drawing.Point(160, 8);
			this.TabCustomBtnSave.Name = "TabCustomBtnSave";
			this.TabCustomBtnSave.TabIndex = 7;
			this.TabCustomBtnSave.Text = "Save";
			this.TabCustomBtnSave.Click += new System.EventHandler(this.Save_Click);
			// 
			// TabCustomCmbBox
			// 
			this.TabCustomCmbBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabCustomCmbBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TabCustomCmbBox.Location = new System.Drawing.Point(8, 8);
			this.TabCustomCmbBox.Name = "TabCustomCmbBox";
			this.TabCustomCmbBox.Size = new System.Drawing.Size(144, 20);
			this.TabCustomCmbBox.Sorted = true;
			this.TabCustomCmbBox.TabIndex = 6;
			this.TabCustomCmbBox.DropDown += new System.EventHandler(this.DropDown);
			this.TabCustomCmbBox.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
			// 
			// TabSave
			// 
			this.TabSave.AutoScroll = true;
			this.TabSave.Controls.Add(this.TabSaveFileformat);
			this.TabSave.Controls.Add(this.labelFormat);
			this.TabSave.Controls.Add(this.TabSaveAA);
			this.TabSave.Controls.Add(this.labelSaveAA);
			this.TabSave.Controls.Add(this.TabSavePanelResulution);
			this.TabSave.Controls.Add(this.labelResulution);
			this.TabSave.Controls.Add(this.TabSaveUniqueFileName);
			this.TabSave.Controls.Add(this.TabSaveFilename);
			this.TabSave.Controls.Add(this.labelFilename);
			this.TabSave.Controls.Add(this.TabSaveBtnDelete);
			this.TabSave.Controls.Add(this.TabSaveBtnSaveAs);
			this.TabSave.Controls.Add(this.TabSaveBtnSave);
			this.TabSave.Controls.Add(this.TabSaveCmbBox);
			this.TabSave.Location = new System.Drawing.Point(4, 22);
			this.TabSave.Name = "TabSave";
			this.TabSave.Size = new System.Drawing.Size(400, 358);
			this.TabSave.TabIndex = 8;
			this.TabSave.Text = "Saving";
			// 
			// TabSaveFileformat
			// 
			this.TabSaveFileformat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabSaveFileformat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TabSaveFileformat.Location = new System.Drawing.Point(104, 216);
			this.TabSaveFileformat.Name = "TabSaveFileformat";
			this.TabSaveFileformat.Size = new System.Drawing.Size(288, 20);
			this.TabSaveFileformat.TabIndex = 18;
			// 
			// labelFormat
			// 
			this.labelFormat.Location = new System.Drawing.Point(8, 216);
			this.labelFormat.Name = "labelFormat";
			this.labelFormat.Size = new System.Drawing.Size(88, 23);
			this.labelFormat.TabIndex = 17;
			this.labelFormat.Text = "File format:";
			// 
			// TabSaveAA
			// 
			this.TabSaveAA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabSaveAA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TabSaveAA.Location = new System.Drawing.Point(104, 192);
			this.TabSaveAA.Name = "TabSaveAA";
			this.TabSaveAA.Size = new System.Drawing.Size(288, 20);
			this.TabSaveAA.TabIndex = 16;
			// 
			// labelSaveAA
			// 
			this.labelSaveAA.Location = new System.Drawing.Point(8, 192);
			this.labelSaveAA.Name = "labelSaveAA";
			this.labelSaveAA.Size = new System.Drawing.Size(88, 24);
			this.labelSaveAA.TabIndex = 15;
			this.labelSaveAA.Text = "Anti-aliasing:";
			// 
			// TabSavePanelResulution
			// 
			this.TabSavePanelResulution.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabSavePanelResulution.Controls.Add(this.TabSaveRBtnResCurrect);
			this.TabSavePanelResulution.Controls.Add(this.TabSaveRBtnResDestop);
			this.TabSavePanelResulution.Controls.Add(this.TabSaveRBtnResCustom);
			this.TabSavePanelResulution.Controls.Add(this.labelX);
			this.TabSavePanelResulution.Controls.Add(this.numericUpDown2);
			this.TabSavePanelResulution.Controls.Add(this.numericUpDown1);
			this.TabSavePanelResulution.Location = new System.Drawing.Point(104, 88);
			this.TabSavePanelResulution.Name = "TabSavePanelResulution";
			this.TabSavePanelResulution.Size = new System.Drawing.Size(288, 96);
			this.TabSavePanelResulution.TabIndex = 14;
			// 
			// TabSaveRBtnResCurrect
			// 
			this.TabSaveRBtnResCurrect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabSaveRBtnResCurrect.Location = new System.Drawing.Point(0, 24);
			this.TabSaveRBtnResCurrect.Name = "TabSaveRBtnResCurrect";
			this.TabSaveRBtnResCurrect.Size = new System.Drawing.Size(288, 24);
			this.TabSaveRBtnResCurrect.TabIndex = 5;
			this.TabSaveRBtnResCurrect.Text = "Use resulution of  main window";
			// 
			// TabSaveRBtnResDestop
			// 
			this.TabSaveRBtnResDestop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabSaveRBtnResDestop.Checked = true;
			this.TabSaveRBtnResDestop.Location = new System.Drawing.Point(0, 0);
			this.TabSaveRBtnResDestop.Name = "TabSaveRBtnResDestop";
			this.TabSaveRBtnResDestop.Size = new System.Drawing.Size(288, 24);
			this.TabSaveRBtnResDestop.TabIndex = 4;
			this.TabSaveRBtnResDestop.TabStop = true;
			this.TabSaveRBtnResDestop.Text = "Use resulution of desktop";
			// 
			// TabSaveRBtnResCustom
			// 
			this.TabSaveRBtnResCustom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabSaveRBtnResCustom.Location = new System.Drawing.Point(0, 48);
			this.TabSaveRBtnResCustom.Name = "TabSaveRBtnResCustom";
			this.TabSaveRBtnResCustom.Size = new System.Drawing.Size(288, 24);
			this.TabSaveRBtnResCustom.TabIndex = 3;
			this.TabSaveRBtnResCustom.Text = "Use custom resulution";
			// 
			// labelX
			// 
			this.labelX.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.labelX.Location = new System.Drawing.Point(146, 76);
			this.labelX.Name = "labelX";
			this.labelX.Size = new System.Drawing.Size(8, 16);
			this.labelX.TabIndex = 2;
			this.labelX.Text = "x";
			// 
			// numericUpDown2
			// 
			this.numericUpDown2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.numericUpDown2.Location = new System.Drawing.Point(160, 72);
			this.numericUpDown2.Name = "numericUpDown2";
			this.numericUpDown2.Size = new System.Drawing.Size(128, 20);
			this.numericUpDown2.TabIndex = 1;
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(16, 72);
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(128, 20);
			this.numericUpDown1.TabIndex = 0;
			// 
			// labelResulution
			// 
			this.labelResulution.Location = new System.Drawing.Point(8, 88);
			this.labelResulution.Name = "labelResulution";
			this.labelResulution.Size = new System.Drawing.Size(96, 23);
			this.labelResulution.TabIndex = 13;
			this.labelResulution.Text = "Resulution:";
			// 
			// TabSaveUniqueFileName
			// 
			this.TabSaveUniqueFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabSaveUniqueFileName.Checked = true;
			this.TabSaveUniqueFileName.CheckState = System.Windows.Forms.CheckState.Checked;
			this.TabSaveUniqueFileName.Location = new System.Drawing.Point(104, 64);
			this.TabSaveUniqueFileName.Name = "TabSaveUniqueFileName";
			this.TabSaveUniqueFileName.Size = new System.Drawing.Size(288, 24);
			this.TabSaveUniqueFileName.TabIndex = 12;
			this.TabSaveUniqueFileName.Text = "Atomaticly make unique filename";
			// 
			// TabSaveFilename
			// 
			this.TabSaveFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabSaveFilename.Location = new System.Drawing.Point(104, 40);
			this.TabSaveFilename.Name = "TabSaveFilename";
			this.TabSaveFilename.Size = new System.Drawing.Size(288, 20);
			this.TabSaveFilename.TabIndex = 11;
			this.TabSaveFilename.Text = "Fractal";
			// 
			// labelFilename
			// 
			this.labelFilename.Location = new System.Drawing.Point(8, 40);
			this.labelFilename.Name = "labelFilename";
			this.labelFilename.Size = new System.Drawing.Size(96, 23);
			this.labelFilename.TabIndex = 10;
			this.labelFilename.Text = "Default filename:";
			// 
			// TabSaveBtnDelete
			// 
			this.TabSaveBtnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabSaveBtnDelete.Location = new System.Drawing.Point(320, 8);
			this.TabSaveBtnDelete.Name = "TabSaveBtnDelete";
			this.TabSaveBtnDelete.TabIndex = 9;
			this.TabSaveBtnDelete.Text = "Delete";
			this.TabSaveBtnDelete.Click += new System.EventHandler(this.Delete_Click);
			// 
			// TabSaveBtnSaveAs
			// 
			this.TabSaveBtnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabSaveBtnSaveAs.Location = new System.Drawing.Point(240, 8);
			this.TabSaveBtnSaveAs.Name = "TabSaveBtnSaveAs";
			this.TabSaveBtnSaveAs.TabIndex = 8;
			this.TabSaveBtnSaveAs.Text = "Save as ...";
			this.TabSaveBtnSaveAs.Click += new System.EventHandler(this.SaveAs_Click);
			// 
			// TabSaveBtnSave
			// 
			this.TabSaveBtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TabSaveBtnSave.Location = new System.Drawing.Point(160, 8);
			this.TabSaveBtnSave.Name = "TabSaveBtnSave";
			this.TabSaveBtnSave.TabIndex = 7;
			this.TabSaveBtnSave.Text = "Save";
			this.TabSaveBtnSave.Click += new System.EventHandler(this.Save_Click);
			// 
			// TabSaveCmbBox
			// 
			this.TabSaveCmbBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabSaveCmbBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TabSaveCmbBox.Location = new System.Drawing.Point(8, 8);
			this.TabSaveCmbBox.Name = "TabSaveCmbBox";
			this.TabSaveCmbBox.Size = new System.Drawing.Size(144, 20);
			this.TabSaveCmbBox.Sorted = true;
			this.TabSaveCmbBox.TabIndex = 6;
			this.TabSaveCmbBox.DropDown += new System.EventHandler(this.DropDown);
			this.TabSaveCmbBox.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
			// 
			// TabComments
			// 
			this.TabComments.Controls.Add(this.TabCommentsTextbox);
			this.TabComments.Location = new System.Drawing.Point(4, 22);
			this.TabComments.Name = "TabComments";
			this.TabComments.Size = new System.Drawing.Size(400, 358);
			this.TabComments.TabIndex = 10;
			this.TabComments.Text = "Comments";
			// 
			// TabCommentsTextbox
			// 
			this.TabCommentsTextbox.AcceptsReturn = true;
			this.TabCommentsTextbox.AcceptsTab = true;
			this.TabCommentsTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TabCommentsTextbox.Location = new System.Drawing.Point(0, 0);
			this.TabCommentsTextbox.Multiline = true;
			this.TabCommentsTextbox.Name = "TabCommentsTextbox";
			this.TabCommentsTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.TabCommentsTextbox.Size = new System.Drawing.Size(400, 360);
			this.TabCommentsTextbox.TabIndex = 13;
			this.TabCommentsTextbox.Text = "";
			this.TabCommentsTextbox.WordWrap = false;
			// 
			// MainBtnDelete
			// 
			this.MainBtnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MainBtnDelete.Location = new System.Drawing.Point(336, 8);
			this.MainBtnDelete.Name = "MainBtnDelete";
			this.MainBtnDelete.TabIndex = 12;
			this.MainBtnDelete.Text = "Delete";
			this.MainBtnDelete.Click += new System.EventHandler(this.MainBtnDelete_Click);
			// 
			// labelFile
			// 
			this.labelFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelFile.Location = new System.Drawing.Point(8, 8);
			this.labelFile.Name = "labelFile";
			this.labelFile.Size = new System.Drawing.Size(48, 23);
			this.labelFile.TabIndex = 13;
			this.labelFile.Text = "File:";
			// 
			// MainBtnSaveAs
			// 
			this.MainBtnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MainBtnSaveAs.Location = new System.Drawing.Point(256, 8);
			this.MainBtnSaveAs.Name = "MainBtnSaveAs";
			this.MainBtnSaveAs.TabIndex = 12;
			this.MainBtnSaveAs.Text = "Save as ...";
			this.MainBtnSaveAs.Click += new System.EventHandler(this.MainBtnSaveAs_Click);
			// 
			// labelAngle
			// 
			this.labelAngle.Location = new System.Drawing.Point(8, 168);
			this.labelAngle.Name = "labelAngle";
			this.labelAngle.Size = new System.Drawing.Size(100, 16);
			this.labelAngle.TabIndex = 24;
			this.labelAngle.Text = "Angle:";
			// 
			// TabViewAngle
			// 
			this.TabViewAngle.LargeChange = 10;
			this.TabViewAngle.Location = new System.Drawing.Point(112, 160);
			this.TabViewAngle.Maximum = 360;
			this.TabViewAngle.Name = "TabViewAngle";
			this.TabViewAngle.Size = new System.Drawing.Size(280, 45);
			this.TabViewAngle.TabIndex = 25;
			this.TabViewAngle.TickFrequency = 10;
			// 
			// SettingsDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(424, 462);
			this.Controls.Add(this.labelFile);
			this.Controls.Add(this.MainBtnDelete);
			this.Controls.Add(this.Tabs);
			this.Controls.Add(this.buttonApply);
			this.Controls.Add(this.MainCmbBox);
			this.Controls.Add(this.MainBtnSaveAs);
			this.MinimumSize = new System.Drawing.Size(432, 304);
			this.Name = "SettingsDlg";
			this.Text = "Settings";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.SettingsDlg_Closing);
			this.Tabs.ResumeLayout(false);
			this.TabEq.ResumeLayout(false);
			this.TabView.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.TabViewYzoom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.TabViewXzoom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.TabViewYpos)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.TabViewXpos)).EndInit();
			this.TabAnim.ResumeLayout(false);
			this.TabColors.ResumeLayout(false);
			this.TabCustom.ResumeLayout(false);
			this.TabSave.ResumeLayout(false);
			this.TabSavePanelResulution.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.TabComments.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.TabViewAngle)).EndInit();
			this.ResumeLayout(false);

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
			settings.lastView = TabViewCmbBox.Text;
			settings.lastColorPalete = TabColorsCmbBox.Text;
			settings.lastCustomCode = TabCustomCmbBox.Text;
			settings.lastSavingRule = TabSaveCmbBox.Text;
			settings.commentes = TabCommentsTextbox.Text;

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
			FillList(TabViewCmbBox);
			FillList(TabColorsCmbBox);
			FillList(TabCustomCmbBox);
			FillList(TabSaveCmbBox);

			TabEqCmbBox.Text = settings.lastEquation;
			TabViewCmbBox.Text = settings.lastView;
			TabColorsCmbBox.Text = settings.lastColorPalete;
			TabCustomCmbBox.Text = settings.lastCustomCode;
			TabSaveCmbBox.Text = settings.lastSavingRule;
			TabCommentsTextbox.Text = settings.commentes;
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

			if (tab == TabView)
				view = (View)settings.views[cb.Text];

			if (tab == TabSave)
				savingRules = (SavingRules)settings.savingRules[cb.Text];
			
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

			if (tab == TabView)
				settings.views[name] = view;

			if (tab == TabSave)
				settings.savingRules[name] = savingRules;

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
				             TabEqCode.Text + "\n" + TabColorsCode.Text + "\n" + TabCustomCode.Text+
						     "\n}}";

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
			{ // Succes
				Type main = res.CompiledAssembly.GetType("Fractals.Main");
				method = main.GetMethod("GetColor");
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
