using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Runtime;
using System.Reflection;
using System.CodeDom.Compiler;

namespace Fractals
{
	public class Form : System.Windows.Forms.Form
	{		
		public View view=new View(0,0,1,1,new Settings().textBoxCode.Text);
		int mouseX,mouseY;
		Thread refreshThread = null;
		Settings setDlg = null;

		Bitmap bitmap;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItemSave;
		private System.Windows.Forms.MenuItem menuItemSettings;
		private System.Windows.Forms.MenuItem menuItemAbout;
		
		private System.ComponentModel.Container components = null;

		public Form()
		{
			InitializeComponent();
		}

		
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuItemSave = new System.Windows.Forms.MenuItem();
			this.menuItemSettings = new System.Windows.Forms.MenuItem();
			this.menuItemAbout = new System.Windows.Forms.MenuItem();
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuItemSave,
																					 this.menuItemSettings,
																					 this.menuItemAbout});
			// 
			// menuItemSave
			// 
			this.menuItemSave.Index = 0;
			this.menuItemSave.Text = "Save image";
			this.menuItemSave.Click += new System.EventHandler(this.menuSave_Click);
			// 
			// menuItemSettings
			// 
			this.menuItemSettings.Index = 1;
			this.menuItemSettings.Text = "Setting";
			this.menuItemSettings.Click += new System.EventHandler(this.menuItemSettings_Click);
			// 
			// menuItemAbout
			// 
			this.menuItemAbout.Index = 2;
			this.menuItemAbout.Text = "About";
			this.menuItemAbout.Click += new System.EventHandler(this.menuItemAbout_Click);
			// 
			// Form
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 345);
			this.Menu = this.mainMenu;
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "Form";
			this.Text = "Fractals";
			this.Resize += new System.EventHandler(this.Form1_Resize);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picture_MouseDown);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picture_MouseUp);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new Form());
		}		

		private void picture_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			mouseX = e.X;
			mouseY = e.Y;
		}

		private void picture_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (Math.Abs(mouseX - e.X) <= 5 || Math.Abs(mouseY - e.Y) <= 5)
			{ // simple zoom
				double zoom = 1;
				if (e.Button == MouseButtons.Left) zoom = 4;
				if (e.Button == MouseButtons.Middle) zoom = 1;
				if (e.Button == MouseButtons.Right) zoom = 0.25;
				view.Xpos = view.makeX(e.X,ClientRectangle.Width);
				view.Ypos = view.makeY(e.Y,ClientRectangle.Height);
				view.Xzoom *= zoom;
				view.Yzoom *= zoom;
			}
			else
			{ // box zoom
				/*view = new View(view.makeX(mouseX, ClientRectangle.Width),
					view.makeX(e.X, ClientRectangle.Width),
					view.makeY(mouseY, ClientRectangle.Height),
					view.makeY(e.Y, ClientRectangle.Height));*/
			}
			RefreshImage();
			if (setDlg != null)
				if (!setDlg.IsDisposed)
				{
					setDlg.Xpos.Value = (decimal)view.Xpos;
					setDlg.Ypos.Value = (decimal)view.Ypos;
					setDlg.Xzoom.Value = (decimal)view.Xzoom;
					setDlg.Yzoom.Value = (decimal)view.Yzoom;
				}
		}

		private void menuSave_Click(object sender, System.EventArgs e)
		{
			Thread t = new Thread(new ThreadStart(ThreatSaveEnteryPoint));
			t.Priority = ThreadPriority.AboveNormal;
			t.Name = "Save";
			t.Start();
		}

		private void Form1_Resize(object sender, System.EventArgs e)
		{
			RefreshImage();
		}

		public void RefreshImage()
		{
			if (refreshThread != null)
			{
				refreshThread.Abort();
				refreshThread = null;
			}
			if (ClientRectangle.Width == 0 || ClientRectangle.Height == 0) return;

			Bitmap newbmp = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
			if (bitmap == null)
				bitmap = newbmp;
			else lock (bitmap) bitmap = newbmp;
			refreshThread = new Thread(new ThreadStart(ThreatRefreshEnteryPoint));
			refreshThread.Name = "Refresh";
			refreshThread.Priority = ThreadPriority.BelowNormal;
			refreshThread.Start();
		}
		public void ThreatRefreshEnteryPoint()
		{
			if (view.Method != null)
				view.Method.Invoke(null,new object[] {bitmap, view, this});
			System.Diagnostics.Debug.WriteLine("Refresh finished");
		}

		public void ThreatSaveEnteryPoint()
		{
			if (view.Method == null)
			{
				MessageBox.Show ("Please fix code first");
				menuItemSettings_Click (null,null);
				return;
			}
			View tmpview = view;
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "Bitmaps | *.bmp";
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				Bitmap tbmp = new Bitmap(1024, 768);
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				view.Method.Invoke (null, new object[] {tbmp, tmpview, this});
				tbmp.Save(dlg.FileName);
			}
		}

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (refreshThread != null)
				refreshThread.Abort();
		}

		private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			lock (bitmap) e.Graphics.DrawImage(bitmap, 0, 0);
		}
	
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			//base.OnPaintBackground (pevent);
		}

		private void menuItemAbout_Click(object sender, System.EventArgs e)
		{
			MessageBox.Show("Made by David Srbecky","About");
		}

		private void menuItemSettings_Click(object sender, System.EventArgs e)
		{
			if (setDlg != null)
				if (!setDlg.IsDisposed)
				{
					setDlg.BringToFront();
					return;
				}
			setDlg = new Settings();
			setDlg.view = view;
			setDlg.formToUpdate = this;
			setDlg.Show();
		}
	}
}
