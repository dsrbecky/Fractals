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
	public partial class MainForm
	{
		static double FPS = 20d;
		public static SettingsForm settingsForm = new SettingsForm();
		bool zooming = false;
		bool restartRenderLoop = false;
		
		public MainForm()
		{
			InitializeComponent();
			CurrentFractalSingleton.CurrentFractalChanged += delegate{ RestartRenderLoop(); };
			MouseWheel += new MouseEventHandler (picture_MouseWheel);
			ClientSize = new Size(512,512);
		}
		
		[STAThread]
		static void Main() 
		{
			MainForm mainForm = new MainForm();
			mainForm.Show();
			mainForm.RenderLoop();
		}
		
		private void Form1_Resize(object sender, System.EventArgs e)
		{
			RestartRenderLoop();
		}
		
		private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			RestartRenderLoop();
		}
		
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			// Prevent painting
		}
		
		private void picture_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			zooming = true;
			RestartRenderLoop();
		}
		
		private void picture_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			zooming = false;
		}
		
		private void picture_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			CurrentFractalSingleton.Instance.View.Angle += 10 * (e.Delta / 120);
			RestartRenderLoop();
		}
		
		private void UpdateMotion()
		{
			if (CurrentFractalSingleton.Instance.View.Rotating) {
				CurrentFractalSingleton.Instance.View.AnimateRotation();
			}
			
			if (zooming) {
				double zoomSpeed = 1;
				if (MouseButtons == MouseButtons.Left)   zoomSpeed = 1+8f/128;
				if (MouseButtons == MouseButtons.Middle) zoomSpeed = 1;
				if (MouseButtons == MouseButtons.Right)  zoomSpeed = 1-8f/128;
				
				View view = CurrentFractalSingleton.Instance.View;
				PointF pos = PointToClient(MousePosition);
				PointF logicalPos = new PointF(); // [-1,1] mapping
				logicalPos.X = (((float)pos.X) / ClientRectangle.Width - 0.5f) * 2f;
				logicalPos.Y = (((float)pos.Y) / ClientRectangle.Height - 0.5f) * 2f;
				logicalPos.X /= 8f;
				logicalPos.Y /= 8f;
				
				CurrentFractalSingleton.Instance.View.ZoomIn(logicalPos, zoomSpeed);
			}
		}
		
		public void RestartRenderLoop()
		{
			restartRenderLoop = true;
		}
		
		void RenderLoop()
		{
			while(!this.IsDisposed) {
				Graphics graphics = CreateGraphics();
				restartRenderLoop = false;
				if (CurrentFractalSingleton.Instance.Compiles) {
					DataGenerator dataGenerator = new DataGenerator(CurrentFractalSingleton.Instance);
					dataGenerator.debugMode = settingsForm.debugMode.Checked;
					dataGenerator.UserThreadAction += delegate {
						Application.DoEvents();
						if (this.IsDisposed || restartRenderLoop) {
							dataGenerator.Abort();
						}
					};
					
					while (!dataGenerator.Aborted && (zooming || CurrentFractalSingleton.Instance.View.Rotating)) {
						UpdateMotion();
						long time = dataGenerator.Render(CurrentFractalSingleton.Instance.View,
						                                 graphics,
						                                 ClientRectangle.Width,
						                                 ClientRectangle.Height,
						                                 FPS + 1);
					}
					
					dataGenerator.Render(CurrentFractalSingleton.Instance.View,
					                     graphics,
					                     ClientRectangle.Width,
					                     ClientRectangle.Height,
					                     0);
				} else {
					graphics.Clear(Color.White);
				}
				while(!restartRenderLoop && !this.IsDisposed) {
					Application.DoEvents();
					Thread.Sleep(10);
				}
			}
		}
		
		
		private void MainForm_Load(object sender, EventArgs e)
		{
			menuItemSettings.PerformClick();
		}
		
		private void menuItemSettings_Click(object sender, System.EventArgs e)
		{
			settingsForm.Show();
			settingsForm.BringToFront();
		}
		
		private void menuItemAbout_Click(object sender, System.EventArgs e)
		{
			MessageBox.Show("Made by David Srbecky","About");
		}
	}
}
