/*
 * Created by SharpDevelop.
 * User: ${USER}
 * Date: ${DATE}
 * Time: ${TIME}
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Fractals
{
	partial class SettingsForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
			this.equationCode = new System.Windows.Forms.TextBox();
			this.TabEquation = new System.Windows.Forms.TabPage();
			this.equationFileComboBox = new Fractals.Util.FileComboBox();
			this.debugMode = new System.Windows.Forms.CheckBox();
			this.Tabs = new System.Windows.Forms.TabControl();
			this.TabColors = new System.Windows.Forms.TabPage();
			this.colorFileComboBox = new Fractals.Util.FileComboBox();
			this.colorCode = new System.Windows.Forms.TextBox();
			this.labelFractal = new System.Windows.Forms.Label();
			this.btnApply = new System.Windows.Forms.Button();
			this.fractalFileComboBox = new Fractals.Util.FileComboBox();
			this.TabEquation.SuspendLayout();
			this.Tabs.SuspendLayout();
			this.TabColors.SuspendLayout();
			this.SuspendLayout();
			// 
			// equationCode
			// 
			this.equationCode.AcceptsReturn = true;
			this.equationCode.AcceptsTab = true;
			this.equationCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.equationCode.Location = new System.Drawing.Point(13, 44);
			this.equationCode.Multiline = true;
			this.equationCode.Name = "equationCode";
			this.equationCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.equationCode.Size = new System.Drawing.Size(590, 465);
			this.equationCode.TabIndex = 1;
			this.equationCode.Text = resources.GetString("equationCode.Text");
			this.equationCode.WordWrap = false;
			// 
			// TabEquation
			// 
			this.TabEquation.Controls.Add(this.equationFileComboBox);
			this.TabEquation.Controls.Add(this.equationCode);
			this.TabEquation.Location = new System.Drawing.Point(4, 30);
			this.TabEquation.Name = "TabEquation";
			this.TabEquation.Size = new System.Drawing.Size(619, 535);
			this.TabEquation.TabIndex = 5;
			this.TabEquation.Text = "Equation";
			// 
			// equationFileComboBox
			// 
			this.equationFileComboBox.Directory = null;
			this.equationFileComboBox.Extension = null;
			this.equationFileComboBox.Location = new System.Drawing.Point(10, 3);
			this.equationFileComboBox.Name = "equationFileComboBox";
			this.equationFileComboBox.Size = new System.Drawing.Size(593, 35);
			this.equationFileComboBox.TabIndex = 2;
			this.equationFileComboBox.Saving += new System.EventHandler<Fractals.Util.TextWriterEventArgs>(this.EquationFileComboBoxSaving);
			this.equationFileComboBox.Loading += new System.EventHandler<Fractals.Util.TextReaderEventArgs>(this.EquationFileComboBoxLoading);
			// 
			// debugMode
			// 
			this.debugMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.debugMode.AutoSize = true;
			this.debugMode.Location = new System.Drawing.Point(12, 623);
			this.debugMode.Name = "debugMode";
			this.debugMode.Size = new System.Drawing.Size(126, 25);
			this.debugMode.TabIndex = 18;
			this.debugMode.Text = "Debug mode";
			this.debugMode.CheckedChanged += new System.EventHandler(this.DebugModeCheckedChanged);
			// 
			// Tabs
			// 
			this.Tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.Tabs.Controls.Add(this.TabEquation);
			this.Tabs.Controls.Add(this.TabColors);
			this.Tabs.Location = new System.Drawing.Point(12, 45);
			this.Tabs.Name = "Tabs";
			this.Tabs.SelectedIndex = 0;
			this.Tabs.Size = new System.Drawing.Size(627, 569);
			this.Tabs.TabIndex = 16;
			// 
			// TabColors
			// 
			this.TabColors.Controls.Add(this.colorFileComboBox);
			this.TabColors.Controls.Add(this.colorCode);
			this.TabColors.Location = new System.Drawing.Point(4, 30);
			this.TabColors.Name = "TabColors";
			this.TabColors.Size = new System.Drawing.Size(619, 535);
			this.TabColors.TabIndex = 6;
			this.TabColors.Text = "Color map";
			// 
			// colorFileComboBox
			// 
			this.colorFileComboBox.Directory = null;
			this.colorFileComboBox.Extension = null;
			this.colorFileComboBox.Location = new System.Drawing.Point(10, 3);
			this.colorFileComboBox.Name = "colorFileComboBox";
			this.colorFileComboBox.Size = new System.Drawing.Size(593, 35);
			this.colorFileComboBox.TabIndex = 12;
			this.colorFileComboBox.Saving += new System.EventHandler<Fractals.Util.TextWriterEventArgs>(this.ColorFileComboBoxSaving);
			this.colorFileComboBox.Loading += new System.EventHandler<Fractals.Util.TextReaderEventArgs>(this.ColorFileComboBoxLoading);
			// 
			// colorCode
			// 
			this.colorCode.AcceptsReturn = true;
			this.colorCode.AcceptsTab = true;
			this.colorCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.colorCode.Location = new System.Drawing.Point(13, 44);
			this.colorCode.Multiline = true;
			this.colorCode.Name = "colorCode";
			this.colorCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.colorCode.Size = new System.Drawing.Size(590, 473);
			this.colorCode.TabIndex = 11;
			this.colorCode.Text = resources.GetString("colorCode.Text");
			this.colorCode.WordWrap = false;
			// 
			// labelFractal
			// 
			this.labelFractal.AutoSize = true;
			this.labelFractal.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelFractal.Location = new System.Drawing.Point(12, 8);
			this.labelFractal.Name = "labelFractal";
			this.labelFractal.Size = new System.Drawing.Size(77, 25);
			this.labelFractal.TabIndex = 17;
			this.labelFractal.Text = "Fractal:";
			// 
			// btnApply
			// 
			this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnApply.Location = new System.Drawing.Point(383, 619);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(256, 28);
			this.btnApply.TabIndex = 15;
			this.btnApply.Text = "&Apply code changes";
			this.btnApply.Click += new System.EventHandler(this.BtnApplyClick);
			// 
			// fractalFileComboBox
			// 
			this.fractalFileComboBox.Directory = null;
			this.fractalFileComboBox.Extension = null;
			this.fractalFileComboBox.Location = new System.Drawing.Point(87, 4);
			this.fractalFileComboBox.Name = "fractalFileComboBox";
			this.fractalFileComboBox.Size = new System.Drawing.Size(552, 35);
			this.fractalFileComboBox.TabIndex = 19;
			this.fractalFileComboBox.Saving += new System.EventHandler<Fractals.Util.TextWriterEventArgs>(this.FractalFileComboBoxSaving);
			this.fractalFileComboBox.Loading += new System.EventHandler<Fractals.Util.TextReaderEventArgs>(this.FractalFileComboBoxLoading);
			// 
			// SettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(651, 660);
			this.Controls.Add(this.fractalFileComboBox);
			this.Controls.Add(this.debugMode);
			this.Controls.Add(this.Tabs);
			this.Controls.Add(this.labelFractal);
			this.Controls.Add(this.btnApply);
			this.Name = "SettingsForm";
			this.Text = "SettingsForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsFormFormClosing);
			this.TabEquation.ResumeLayout(false);
			this.TabEquation.PerformLayout();
			this.Tabs.ResumeLayout(false);
			this.TabColors.ResumeLayout(false);
			this.TabColors.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button btnApply;
		private Fractals.Util.FileComboBox fractalFileComboBox;
		private System.Windows.Forms.Label labelFractal;
		public System.Windows.Forms.TextBox colorCode;
		private Fractals.Util.FileComboBox colorFileComboBox;
		public System.Windows.Forms.CheckBox debugMode;
		private Fractals.Util.FileComboBox equationFileComboBox;
		private System.Windows.Forms.TabPage TabEquation;
		public System.Windows.Forms.TextBox equationCode;
		private System.Windows.Forms.TabPage TabColors;
		private System.Windows.Forms.TabControl Tabs;
	}
}
