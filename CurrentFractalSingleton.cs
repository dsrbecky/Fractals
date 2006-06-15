using System;

namespace Fractals
{
	public static class CurrentFractalSingleton
	{
		static Fractal instance;
		
		public static event EventHandler CurrentFractalChanged;
		
		public static Fractal Instance {
			get {
				if (instance == null) {
					instance = new Fractal();
				}
				return instance;
			}
			set {
				instance = value;
				OnCurrentFractalChanged(EventArgs.Empty);
				instance.FractalChanged += delegate {
					OnCurrentFractalChanged(EventArgs.Empty);
				};
			}
		}
		
		static void OnCurrentFractalChanged(EventArgs e)
		{
			if (CurrentFractalChanged != null) {
				CurrentFractalChanged(null, e);
			}
		}
	}
}
