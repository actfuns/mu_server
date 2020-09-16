using System;

namespace Server.Tools
{
	
	internal class SysConOut
	{
		
		public static void WriteLine(string value)
		{
			lock (SysConOut.SystemConsoleOutMutex)
			{
				Console.WriteLine(value);
			}
		}

		
		private static object SystemConsoleOutMutex = new object();
	}
}
