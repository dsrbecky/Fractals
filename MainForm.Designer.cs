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
	partial class MainForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItemSave;
		private System.Windows.Forms.MenuItem menuItemSettings;
		private System.Windows.Forms.MenuItem menuItemAbout;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		
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
			this.components = new System.ComponentModel.Container();
			this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
			this.menuItemSave = new System.Windows.Forms.MenuItem();
			this.menuItemSettings = new System.Windows.Forms.MenuItem();
			this.menuItemAbout = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
									this.menuItemSave,
									this.menuItemSettings,
									this.menuItemAbout,
									this.menuItem1,
									this.menuItem2});
			// 
			// menuItemSave
			// 
			this.menuItemSave.Index = 0;
			this.menuItemSave.Text = "Save image";
			// 
			// menuItemSettings
			// 
			this.menuItemSettings.Index = 1;
			this.menuItemSettings.Text = "Setting";
			// 
			// menuItemAbout
			// 
			this.menuItemAbout.Index = 2;
			this.menuItemAbout.Text = "About";
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 3;
			this.menuItem1.Text = "Refresh";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 4;
			this.menuItem2.Text = "GC";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(8, 21);
			this.ClientSize = new System.Drawing.Size(512, 512);
			this.MinimumSize = new System.Drawing.Size(160, 146);
			this.Name = "MainForm";
			this.Text = "Fractals 0.5 BETA";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
			this.Resize += new System.EventHandler(this.Form1_Resize);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picture_MouseUp);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainFormMouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picture_MouseDown);
			this.ResumeLayout(false);
		}
	}
}
