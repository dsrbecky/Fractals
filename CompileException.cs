using System;

namespace Fractals
{
	public class CompileException: Exception
	{
		string errorMessage;
		
		public string ErrorMessage {
			get {
				return errorMessage;
			}
		}
		
		public CompileException(string errorMessage)
		{
			this.errorMessage = errorMessage;
		}
	}
}
