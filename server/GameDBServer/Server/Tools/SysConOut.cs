using System;

namespace Server.Tools
{
	// Token: 0x02000223 RID: 547
	internal class SysConOut
	{
		// Token: 0x06000CEA RID: 3306 RVA: 0x000A388C File Offset: 0x000A1A8C
		public static void WriteLine(string value)
		{
			lock (SysConOut.SystemConsoleOutMutex)
			{
				Console.WriteLine(value);
			}
		}

		// Token: 0x04001247 RID: 4679
		private static object SystemConsoleOutMutex = new object();
	}
}
