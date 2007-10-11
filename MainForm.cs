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
		static int FPS = 25;
		bool restartRenderLoop = false;
		Renderer dataGenerator;
		
		public MainForm()
		{
			InitializeComponent();
			CurrentFractalSingleton.CurrentFractalChanged += delegate{ RestartRenderLoop(); };
			MouseWheel += new MouseEventHandler (picture_MouseWheel);
			ClientSize = new Size(512,512);
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
			dataGenerator.mouseButtons = e.Button;
			RestartRenderLoop();
		}
		
		private void picture_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			dataGenerator.mouseButtons = MouseButtons.None;
		}
		
		private void picture_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			dataGenerator.Rotate(Math.Sign(e.Delta));
			RestartRenderLoop();
		}
		
		private void UpdateMotion()
		{
			dataGenerator.Move();
		}
		
		public void RestartRenderLoop()
		{
			restartRenderLoop = true;
		}
		
		public void RenderLoop()
		{
			while(!this.IsDisposed) {
				Graphics graphics = CreateGraphics();
				restartRenderLoop = false;
				if (CurrentFractalSingleton.Instance.Compiles) {
					if (dataGenerator == null) {
						dataGenerator = new OpenGlRenderer(CurrentFractalSingleton.Instance, graphics, ClientRectangle);
					}
					dataGenerator.Fractal = CurrentFractalSingleton.Instance;
					dataGenerator.TargetRenderTime = TimeSpan.FromSeconds(1d / FPS);
					dataGenerator.UserThreadAction += delegate {
						Application.DoEvents();
						if (this.IsDisposed || restartRenderLoop) {
							dataGenerator.Abort();
						}
					};
					
					while (!dataGenerator.Aborted && (dataGenerator.mouseButtons != MouseButtons.None || dataGenerator.Fractal.View.Rotating)) {
						UpdateMotion();
						dataGenerator.Render();
						Text = String.Format("{0:F0} fps; {1} fragments; {2:F3} ms/frag",
						                     dataGenerator.FPS,
						                     dataGenerator.NumberOfFragmentsRendered,
						                     dataGenerator.ActualRenderTime.TotalMilliseconds / dataGenerator.NumberOfFragmentsRendered);
					}
					
					dataGenerator.HighQualityRender();
				} else {
					graphics.Clear(Color.White);
				}
				while(!restartRenderLoop && !this.IsDisposed) {
					Application.DoEvents();
					Thread.Sleep(10);
				}
			}
		}
		
		void MainFormMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			dataGenerator.mousePosition = new PointF((float)e.X / ClientRectangle.Width, (float)e.Y / ClientRectangle.Height);
		}
	}
}
