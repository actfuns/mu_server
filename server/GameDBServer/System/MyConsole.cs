using System;

namespace System
{
	// Token: 0x0200021E RID: 542
	public class MyConsole
	{
		// Token: 0x06000CC9 RID: 3273 RVA: 0x000A2A7C File Offset: 0x000A0C7C
		public static void WriteLine<T>(T value)
		{
			lock (MyConsole.mutex)
			{
				Console.WriteLine(value);
			}
		}

		// Token: 0x06000CCA RID: 3274 RVA: 0x000A2ACC File Offset: 0x000A0CCC
		public static void WriteLine(string format, params object[] arg)
		{
			lock (MyConsole.mutex)
			{
				Console.WriteLine(format, arg);
			}
		}

		// Token: 0x04001244 RID: 4676
		private static object mutex = new object();
	}
}
