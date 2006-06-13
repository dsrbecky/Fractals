/*
 * Created by SharpDevelop.
 * User: ${USER}
 * Date: ${DATE}
 * Time: ${TIME}
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Fractals.Util
{
	partial class FileComboBox : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
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
			this.save = new System.Windows.Forms.Button();
			this.delete = new System.Windows.Forms.Button();
			this.cmbBox = new System.Windows.Forms.ComboBox();
			this.saveAs = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// save
			// 
			this.save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.save.Location = new System.Drawing.Point(195, 3);
			this.save.Name = "save";
			this.save.Size = new System.Drawing.Size(103, 29);
			this.save.TabIndex = 19;
			this.save.Text = "Save";
			this.save.Click += new System.EventHandler(this.SaveClick);
			// 
			// delete
			// 
			this.delete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.delete.Location = new System.Drawing.Point(407, 3);
			this.delete.Name = "delete";
			this.delete.Size = new System.Drawing.Size(108, 29);
			this.delete.TabIndex = 18;
			this.delete.Text = "Delete";
			this.delete.Click += new System.EventHandler(this.DeleteClick);
			// 
			// cmbBox
			// 
			this.cmbBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.cmbBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbBox.FormattingEnabled = true;
			this.cmbBox.Location = new System.Drawing.Point(3, 3);
			this.cmbBox.Name = "cmbBox";
			this.cmbBox.Size = new System.Drawing.Size(186, 29);
			this.cmbBox.Sorted = true;
			this.cmbBox.TabIndex = 16;
			this.cmbBox.SelectedIndexChanged += new System.EventHandler(this.CmbBoxSelectedIndexChanged);
			this.cmbBox.DropDown += new System.EventHandler(this.CmbBoxDropDown);
			// 
			// saveAs
			// 
			this.saveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.saveAs.Location = new System.Drawing.Point(304, 3);
			this.saveAs.Name = "saveAs";
			this.saveAs.Size = new System.Drawing.Size(97, 29);
			this.saveAs.TabIndex = 17;
			this.saveAs.Text = "Save as ...";
			this.saveAs.Click += new System.EventHandler(this.SaveAsClick);
			// 
			// FileComboBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.save);
			this.Controls.Add(this.delete);
			this.Controls.Add(this.cmbBox);
			this.Controls.Add(this.saveAs);
			this.Name = "FileComboBox";
			this.Size = new System.Drawing.Size(518, 37);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button saveAs;
		public System.Windows.Forms.ComboBox cmbBox;
		private System.Windows.Forms.Button delete;
		private System.Windows.Forms.Button save;
	}
}
