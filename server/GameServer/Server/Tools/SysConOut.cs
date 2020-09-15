using System;

namespace Server.Tools
{
	// Token: 0x020008FA RID: 2298
	internal class SysConOut
	{
		// Token: 0x0600428A RID: 17034 RVA: 0x003C9C14 File Offset: 0x003C7E14
		public static void WriteLine(string value)
		{
			lock (SysConOut.SystemConsoleOutMutex)
			{
				Console.WriteLine(value);
			}
		}

		// Token: 0x0400503C RID: 20540
		private static object SystemConsoleOutMutex = new object();
	}
}
