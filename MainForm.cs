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
		Thread refreshThread = null;
		public static SettingsDlg setDlg = new SettingsDlg();
        bool zooming = false;
        bool rendering = false;
        bool rotating {
            get {
                View tmp = setDlg.view;
                return tmp.TargetAngle != tmp.Angle;
            }
        }
		//public Bitmap bitmap;
		//public object BmpSyncRoot = new object();
		//float zoomSpeed = 1;

		DataGenerator dataGenerator;

		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItemSave;
		private System.Windows.Forms.MenuItem menuItemSettings;
		private System.Windows.Forms.MenuItem menuItemAbout;
		private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
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
            ClientSize = new Size(512,512);
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
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItemSave = new System.Windows.Forms.MenuItem();
            this.menuItemSettings = new System.Windows.Forms.MenuItem();
            this.menuItemAbout = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
// 
// mainMenu
// 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemSave,
            this.menuItemSettings,
            this.menuItemAbout,
            this.menuItem1,
            this.menuItem2});
            this.mainMenu.Name = "mainMenu";
// 
// menuItemSave
// 
            this.menuItemSave.Index = 0;
            this.menuItemSave.Name = "menuItemSave";
            this.menuItemSave.Text = "Save image";
            this.menuItemSave.Click += new System.EventHandler(this.menuSave_Click);
// 
// menuItemSettings
// 
            this.menuItemSettings.Index = 1;
            this.menuItemSettings.Name = "menuItemSettings";
            this.menuItemSettings.Text = "Setting";
            this.menuItemSettings.Click += new System.EventHandler(this.menuItemSettings_Click);
// 
// menuItemAbout
// 
            this.menuItemAbout.Index = 2;
            this.menuItemAbout.Name = "menuItemAbout";
            this.menuItemAbout.Text = "About";
            this.menuItemAbout.Click += new System.EventHandler(this.menuItemAbout_Click);
// 
// menuItem1
// 
            this.menuItem1.Index = 3;
            this.menuItem1.Name = "menuItem1";
            this.menuItem1.Text = "Refresh";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
// 
// menuItem2
// 
            this.menuItem2.Index = 4;
            this.menuItem2.Name = "menuItem2";
            this.menuItem2.Text = "GC";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
// 
// MainForm
// 
            this.AutoScaleBaseSize = new System.Drawing.Size(7, 19);
            this.ClientSize = new System.Drawing.Size(512, 512);
            this.MinimumSize = new System.Drawing.Size(140, 146);
            this.Name = "MainForm";
            this.Text = "Fractals 0.5 BETA";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picture_MouseUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picture_MouseDown);

        }
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}		

		private void picture_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            zooming = true;
            RenderLoop();
		}

		private void picture_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            zooming = false;
            RenderLoop();
		}

		private void picture_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			View tmp = setDlg.view;
			tmp.TargetAngle += 10*(e.Delta/120);
			setDlg.view = tmp;

		    RenderLoop();
		}

		private void UpdateMotion()
		{      
            if (rotating) {
                View tmp = setDlg.view;
                while (tmp.Angle - tmp.TargetAngle > 180) {
                    tmp.Angle -= 360;
                }
                while (tmp.Angle - tmp.TargetAngle < -180) {
                    tmp.Angle += 360;
                }

                tmp.Angle = (5*tmp.Angle + tmp.TargetAngle) / 6;
                tmp.Angle -= Math.Sign(tmp.Angle - tmp.TargetAngle) * Math.Min(0.5, Math.Abs(tmp.Angle - tmp.TargetAngle));

                if (Math.Abs(tmp.Angle - tmp.TargetAngle) < 0.1) {
                    tmp.Angle = tmp.TargetAngle;
                }
                setDlg.view = tmp;

                Invalidate();
            }

            if (zooming) {
                double zoomSpeed = 1;

                if (MouseButtons == MouseButtons.Left) zoomSpeed = 1+8f/128;
			    if (MouseButtons == MouseButtons.Middle) zoomSpeed = 1;
			    if (MouseButtons == MouseButtons.Right) zoomSpeed = 1-8f/128;

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
            
        }

        private void Form1_Resize(object sender, System.EventArgs e)
		{
			Invalidate();
		}

        public void RefreshImage()
        {
            if (refreshThread != null) {
                dataGenerator.abortFlag = true;
                refreshThread.Join();
                dataGenerator.abortFlag = false;
                refreshThread = null;
            }
            if (dataGenerator != null) {
                dataGenerator.abortFlag = true;
                dataGenerator = null;
            }


            if (setDlg.GetColorIndex != null)
                dataGenerator = new DataGenerator(setDlg.GetColorIndex, setDlg.colorPalette);
            GC.Collect();
            Invalidate();
        }

        //private void IvalidateMe()
		//{
		//	Invalidate();
		//}

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

        static double FPS = 20d;

        private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            RenderLoop();
        }

        void RenderLoop()
		{
            if (rendering) return; // no recusrsion
            rendering = true;

            if (refreshThread != null) {
                dataGenerator.abortFlag = true;
                refreshThread.Join();
                dataGenerator.abortFlag = false;
                refreshThread = null;
            }
            if (dataGenerator == null) {
                CreateGraphics().Clear(Color.White);
                rendering = false;
                return;
            }
            dataGenerator.debugMode = setDlg.chkBoxDebugMode.Checked;

            while (zooming || rotating) {
                UpdateMotion();
                long time = dataGenerator.Render(setDlg.view, CreateGraphics(), ClientRectangle.Width, ClientRectangle.Height, FPS + 1);
                Text = "Fractals 0.5 BETA - " + ((int)(1000d/time)).ToString() + " fps";
                Application.DoEvents();
                if (this.IsDisposed) return;
            } 
            {
                Text = "Fractals 0.5 BETA - static";
                _view = setDlg.view;
                _w = ClientRectangle.Width;
                _h = ClientRectangle.Height;
                refreshThread = new Thread(new ThreadStart(ThreatRefreshEnteryPoint));
                refreshThread.Name = "Refresh";
                refreshThread.Priority = ThreadPriority.BelowNormal;
                refreshThread.Start();
            }

            rendering = false;
        }

        View _view;
        int _w;
        int _h;

        public void ThreatRefreshEnteryPoint()
        {
            dataGenerator.Render(_view , this.CreateGraphics() , _w, _h, 0);
            System.Diagnostics.Debug.WriteLine("Refresh finished");
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            setDlg.Show();
			setDlg.BringToFront();
        }

	}
}
