using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using Tao.FreeGlut;
using Tao.OpenGl;

namespace Fractals
{
	public class OpenGlRenderer: Renderer
	{
		static OpenGlRenderer i;
		static Size windowSize = new Size(400, 400);
		
		static OpenGlRenderer()
		{
			Glut.glutInit();
			Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_SINGLE);
			Glut.glutCreateWindow("OpenGL");
			Glut.glutDisplayFunc(Display);
			Glut.glutReshapeFunc(Reshape);
			Glut.glutMotionFunc(Motion);
			Glut.glutPassiveMotionFunc(Motion);
			Glut.glutMouseFunc(Mouse);
			Glut.glutMouseWheelFunc(MouseWheel);
			Init();
			new Thread(new ThreadStart(delegate {
				Glut.glutMainLoop();
			})).Start();
		}
		
		static void Motion(int x, int y)
		{
			i.mousePosition = new PointF((float)x / windowSize.Width, (float)y / windowSize.Height);
		}
		
		static void Mouse(int button, int state, int x, int y)
		{
			if (state == Glut.GLUT_DOWN) {
				switch (button) {
					case Glut.GLUT_LEFT_BUTTON:
						i.mouseButtons = MouseButtons.Left;
						break;
					case Glut.GLUT_MIDDLE_BUTTON:
						i.mouseButtons = MouseButtons.Middle;
						break;
					case Glut.GLUT_RIGHT_BUTTON:
						i.mouseButtons = MouseButtons.Right;
						break;
				}
			} else {
				i.mouseButtons = MouseButtons.None;
			}
		}
		
		static void MouseWheel(int wheel, int direction, int x, int y)
		{
			i.Rotate(direction);
		}
		
		public OpenGlRenderer(Fractal fractal, Graphics graphics, Rectangle renderRectangle)
			:base(fractal, graphics, renderRectangle)
		{
			i = this;
		}
		
		static int textureName;
		
		public static void Init()
		{
			Gl.glShadeModel(Gl.GL_SMOOTH);
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			Gl.glClearColor(0, 0, 0, 0);
			
			Gl.glGenTextures(1, out textureName);
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureName);
			byte[,,] image = new byte[16, 16, 4];
			for(int x = 0; x < 16; x++) {
				for(int y = 0; y < 16; y++) {
					image[x,y,0] = (byte)(x * 14);
					image[x,y,1] = (byte)(y * 14);
					image[x,y,2] = 0;
					image[x,y,3] = 255;
				}
			}
			
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
			
			Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, 16, 16, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, image);
		}
		
		public static void Display()
		{
//			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
//			
//			Gl.glBegin(Gl.GL_QUADS);
//			{
//				Gl.glTexCoord2f(0.0f ,0.0f); Gl.glVertex2d(0.1, 0.1);
//				Gl.glTexCoord2f(1.0f ,0.0f); Gl.glVertex2d(0.9, 0.1);
//				Gl.glTexCoord2f(1.0f ,1.0f); Gl.glVertex2d(0.9, 0.9);
//				Gl.glTexCoord2f(0.0f ,1.0f); Gl.glVertex2d(0.1, 0.9);
//			}
//			Gl.glEnd();
//			
//			Glut.glutSwapBuffers();
		}
		
		protected override void Draw(Texture tex, RectangleF src, RectangleD dest)
		{
			base.Draw(tex, src, dest);
			
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			Gl.glRotated(Fractal.View.CurrentAngle, 0, 0, 1);
			
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, tex.TextureName);
			
			Gl.glBegin(Gl.GL_QUADS);
			{
				int texSize = BitmapCache.BitmapSize;
				
				Gl.glTexCoord2f(src.Left / texSize, src.Top / texSize);
				Gl.glVertex2d(dest.X, dest.Y);
				
				Gl.glTexCoord2f((src.Left) / texSize, (src.Top + src.Height) / texSize);
				Gl.glVertex2d(dest.X + dest.Width, dest.Y);
			
				Gl.glTexCoord2f((src.Left + src.Width) / texSize, (src.Top + src.Height) / texSize);
				Gl.glVertex2d(dest.X + dest.Width, dest.Y + dest.Height);
				
				Gl.glTexCoord2f((src.Left + src.Width) / texSize, src.Top / texSize);
				Gl.glVertex2d(dest.X, dest.Y + dest.Height);
			}
			Gl.glEnd();
			
			Gl.glFlush();
		}
		
		public static void Reshape(int width, int height)
		{
			windowSize = new Size(width, height);
			Gl.glViewport(0, 0, width, height);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Gl.glOrtho(-1.0, 1.0, 1.0, -1.0, -1.0, 1.0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
		}
	}
}
