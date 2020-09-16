using System;

namespace System
{
	
	public class MyConsole
	{
		
		public static void WriteLine<T>(T value)
		{
			lock (MyConsole.mutex)
			{
				Console.WriteLine(value);
			}
		}

		
		public static void WriteLine(string format, params object[] arg)
		{
			lock (MyConsole.mutex)
			{
				Console.WriteLine(format, arg);
			}
		}

		
		private static object mutex = new object();
	}
}
