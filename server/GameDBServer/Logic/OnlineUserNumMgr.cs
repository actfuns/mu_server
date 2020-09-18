using System;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
	
	public class OnlineUserNumMgr
	{
		
		public static void WriteTotalOnlineNumToDB(DBManager dbMgr)
		{
			DateTime dateTime = DateTime.Now;
			long nowTicks = dateTime.Ticks / 10000L;
			if (nowTicks - OnlineUserNumMgr.LastWriteDBTicks >= 120000L)
			{
				OnlineUserNumMgr.LastWriteDBTicks = nowTicks;
				int totalNum = LineManager.GetTotalOnlineNum();
				string strMapOnlineInfo = LineManager.GetMapOnlineNum();
				DBWriter.AddNewOnlineNumItem(dbMgr, totalNum, dateTime, strMapOnlineInfo);
			}
		}

		
		public static void NotifyTotalOnlineNumToServer(DBManager dbMgr)
		{
			long nowTicks = DateTime.Now.Ticks / 10000L;
			if (nowTicks - OnlineUserNumMgr.LastNotifyDBTicks >= 30000L)
			{
				OnlineUserNumMgr.LastNotifyDBTicks = nowTicks;
				int totalNum = LineManager.GetTotalOnlineNum();
			}
		}

		
		private static long LastWriteDBTicks = DateTime.Now.Ticks / 10000L;

		
		private static long LastNotifyDBTicks = DateTime.Now.Ticks / 10000L;
	}
}
