using System;
using System.Windows.Forms;

namespace Fractals
{
	public static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			
			SettingsForm settingsForm = new SettingsForm();
			settingsForm.Show();
			settingsForm.BringToFront();
			
			MainForm mainForm = new MainForm();
			mainForm.Show();
			mainForm.RenderLoop();
		}
	}
}
