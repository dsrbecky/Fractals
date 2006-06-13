using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fractals.Util
{
	public partial class InputBox
	{
		public InputBox()
		{
			InitializeComponent();
		}
		
		public static string Show(string label, string defaultValue, string caption)
		{
			InputBox form;
			form = new InputBox();
			form.label1.Text = label;
			form.textBox1.Text = defaultValue;
			form.Text = caption;
			if (form.ShowDialog() == DialogResult.OK) return form.textBox1.Text;
												 else return String.Empty;
		}
		
		void ButtonOKClick(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			Close();
		}
		
		void ButtonCancelClick(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
