using System;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
	// Token: 0x020001AC RID: 428
	public class OnlineUserNumMgr
	{
		// Token: 0x06000916 RID: 2326 RVA: 0x00054ED0 File Offset: 0x000530D0
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

		// Token: 0x06000917 RID: 2327 RVA: 0x00054F2C File Offset: 0x0005312C
		public static void NotifyTotalOnlineNumToServer(DBManager dbMgr)
		{
			long nowTicks = DateTime.Now.Ticks / 10000L;
			if (nowTicks - OnlineUserNumMgr.LastNotifyDBTicks >= 30000L)
			{
				OnlineUserNumMgr.LastNotifyDBTicks = nowTicks;
				int totalNum = LineManager.GetTotalOnlineNum();
			}
		}

		// Token: 0x040009BC RID: 2492
		private static long LastWriteDBTicks = DateTime.Now.Ticks / 10000L;

		// Token: 0x040009BD RID: 2493
		private static long LastNotifyDBTicks = DateTime.Now.Ticks / 10000L;
	}
}
