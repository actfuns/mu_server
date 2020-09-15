using System;

namespace KF.Remoting
{
	// Token: 0x02000008 RID: 8
	internal class SysConOut
	{
		// Token: 0x06000046 RID: 70 RVA: 0x00004834 File Offset: 0x00002A34
		public static void WriteLine(string value)
		{
			lock (SysConOut.SystemConsoleOutMutex)
			{
				Console.WriteLine(value);
			}
		}

		// Token: 0x04000032 RID: 50
		private static object SystemConsoleOutMutex = new object();
	}
}
