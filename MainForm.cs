using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
	public delegate void EventHandlerNoArg();

	public class MainForm : System.Windows.Forms.Form
	{		
		int mouseX,mouseY;
		Thread refreshThread = null;
		public static SettingsDlg setDlg = new SettingsDlg();
		public Bitmap bitmap;
		public object BmpSyncRoot = new object();
		float zoomSpeed = 1;

		DataGenerator dataGenerator;

		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItemSave;
		private System.Windows.Forms.MenuItem menuItemSettings;
		private System.Windows.Forms.MenuItem menuItemAbout;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.Timer timerZooming;
		private System.ComponentModel.IContainer components;

		public MainForm()
		{
            /*this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);*/

            InitializeComponent();
			setDlg.ViewChanged += new EventHandlerNoArg(RefreshImage);
			MouseWheel += new MouseEventHandler (picture_MouseWheel);
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
			this.components = new System.ComponentModel.Container();
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuItemSave = new System.Windows.Forms.MenuItem();
			this.menuItemSettings = new System.Windows.Forms.MenuItem();
			this.menuItemAbout = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.timerZooming = new System.Windows.Forms.Timer(this.components);
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
			// menuItem1
			// 
			this.menuItem1.Index = 3;
			this.menuItem1.Text = "Refresh";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 4;
			this.menuItem2.Text = "GC";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// timerZooming
			// 
			this.timerZooming.Interval = 50;
			this.timerZooming.Tick += new System.EventHandler(this.timerZooming_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 345);
			this.Menu = this.mainMenu;
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "MainForm";
			this.Text = "Fractals - BETA - Unstable";
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
			Application.Run(new MainForm());
		}		

		private void picture_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) zoomSpeed = 1+8f/128;
			if (e.Button == MouseButtons.Middle) zoomSpeed = 1;
			if (e.Button == MouseButtons.Right) zoomSpeed = 1-8f/128;

			timerZooming.Enabled = true;
			mouseX = e.X;
			mouseY = e.Y;
		}

		private void picture_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			timerZooming.Enabled = false;
		}

		private void picture_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			View tmp = setDlg.view;
			tmp.Angle += 10*(e.Delta/120);
			setDlg.view = tmp;

			Invalidate();		
		}

		private void timerZooming_Tick(object sender, System.EventArgs e)
		{
            /*View tmp = setDlg.view;
			Point pos = PointToClient(MousePosition);
            pos.X = (int)(pos.X * 1f/8 + (ClientRectangle.Width/2) * 7f/8);
            pos.Y = (int)(pos.Y * 1f/8 + (ClientRectangle.Height/2) * 7f/8);
            tmp.Move(
                tmp.makeX(pos.X, ClientRectangle.Width),
                tmp.makeY(pos.Y, ClientRectangle.Height),
				tmp.Xzoom * zoomSpeed,
				tmp.Yzoom * zoomSpeed);
			setDlg.view = tmp;

			Invalidate();*/

            View tmp = setDlg.view;
			PointF pos = PointToClient(MousePosition);
			//pos.X = (int)(pos.X * 1f/8 + (ClientRectangle.Width/2) * 7f/8);
			//pos.Y = (int)(pos.Y * 1f/8 + (ClientRectangle.Height/2) * 7f/8);
            PointF[] dest = new PointF[] {pos}; // [-1,1] mapping
            dest[0].X = (((float)pos.X) / ClientRectangle.Width - 0.5f) * 2f / 8;
            dest[0].Y = (((float)pos.Y) / ClientRectangle.Height - 0.5f) * 2f / 8;

            Matrix matrix = new Matrix();
            matrix.Rotate((float)(tmp.Angle), MatrixOrder.Append);
            //matrix.Translate(1, 1, MatrixOrder.Append);
            //matrix.Scale(ClientRectangle.Width / 2, ClientRectangle.Height / 2, MatrixOrder.Append);
            matrix.Invert();

            matrix.TransformPoints(dest);
            tmp.Move(
                tmp.makeX(dest[0].X+1d, 2d),
                tmp.makeY(dest[0].Y+1d, 2d),
                tmp.Xzoom * zoomSpeed,
				tmp.Yzoom * zoomSpeed);
			setDlg.view = tmp;

			Invalidate();
        }

        private void Form1_Resize(object sender, System.EventArgs e)
		{
			//RefreshImage();
			Invalidate();
		}

		public void RefreshImage()
		{
			if (setDlg.GetColorIndex != null)
				dataGenerator = new DataGenerator(setDlg.GetColorIndex);
			GC.Collect();
			Invalidate();
			return;

			lock (BmpSyncRoot)
				if (refreshThread != null)
				{
					refreshThread.Abort();
					refreshThread = null;
				}

			if (ClientRectangle.Width == 0 || ClientRectangle.Height == 0) return;

			//TODO: correct size w/h
			lock (BmpSyncRoot) bitmap = new Bitmap(ClientRectangle.Width/8, ClientRectangle.Height/8);
			refreshThread = new Thread(new ThreadStart(ThreatRefreshEnteryPoint));
			refreshThread.Name = "Refresh";
			refreshThread.Priority = ThreadPriority.BelowNormal;
			refreshThread.Start();
		}
		public void ThreatRefreshEnteryPoint()
		{
			//Fractals.Algorihtm.CalcImage(bitmap, BmpSyncRoot, setDlg.view, new EventHandlerNoArg(IvalidateMe));
			System.Diagnostics.Debug.WriteLine("Refresh finished");
		}

		private void IvalidateMe()
		{
			Invalidate();
		}

		private void menuSave_Click(object sender, System.EventArgs e)
		{
			Thread t = new Thread(new ThreadStart(ThreatSaveEnteryPoint));
			t.Priority = ThreadPriority.AboveNormal;
			t.Name = "Save";
			t.Start();
		}

		public void ThreatSaveEnteryPoint()
		{
			Settings s = setDlg.settings;
			if (setDlg.Method == null)
			{
				MessageBox.Show ("Please fix code first");
				menuItemSettings_Click (null,null);
				return;
			}
			View tmpview = setDlg.view;
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "Bitmaps | *.bmp";
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				Bitmap tbmp = new Bitmap(1024, 768);
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				setDlg.Method.Invoke (null, new object[] {tbmp,new object(), tmpview, null});
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
			if (dataGenerator != null)
				dataGenerator.Render(setDlg.view,e.Graphics,ClientRectangle.Width, ClientRectangle.Height);
			//new Rendrer(Algorihtm.d).Render(setDlg.view, e.Graphics, ClientRectangle.Width, ClientRectangle.Height);
			/*
			lock (BmpSyncRoot)
			{
				Graphics g = e.Graphics;
				//g.RotateTransform(45);
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.DrawImage(bitmap ,0 ,0 ,ClientRectangle.Width, ClientRectangle.Height);
			}*/
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
			setDlg.Show();
			setDlg.BringToFront();
		}

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
			Invalidate();
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			GC.Collect();
		}

	}
}
