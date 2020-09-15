using System;
using Server.Tools;

namespace GameDBServer.Logic.GoldAuction
{
	// Token: 0x0200013E RID: 318
	public class LjlLog
	{
		// Token: 0x0600054A RID: 1354 RVA: 0x0002C714 File Offset: 0x0002A914
		public static void WriteLog(LogTypes type, string log, string info = "")
		{
			LogManager.WriteLog(type, string.Format("{0}{1}{2}", "[ljl] ", info, log), null, true);
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x0002C734 File Offset: 0x0002A934
		public static void WriteLogFormat(LogTypes type, params string[] args)
		{
			try
			{
				string msgLog = "[ljl] ";
				for (int i = 0; i < args.Length; i++)
				{
					msgLog += args[i];
				}
				LogManager.WriteLog(type, msgLog, null, true);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("{0}{1}", "[ljl] ", ex.ToString()), null, true);
			}
		}

		// Token: 0x04000809 RID: 2057
		private const string LogCheckStr = "[ljl] ";
	}
}
