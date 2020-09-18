using System;
using Server.Tools;

namespace GameDBServer.Logic.GoldAuction
{
	
	public class LjlLog
	{
		
		public static void WriteLog(LogTypes type, string log, string info = "")
		{
			LogManager.WriteLog(type, string.Format("{0}{1}{2}", "[ljl] ", info, log), null, true);
		}

		
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

		
		private const string LogCheckStr = "[ljl] ";
	}
}
