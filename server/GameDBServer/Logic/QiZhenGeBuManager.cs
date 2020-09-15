using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	// Token: 0x020001D5 RID: 469
	public class QiZhenGeBuManager
	{
		// Token: 0x060009DD RID: 2525 RVA: 0x0005EEB0 File Offset: 0x0005D0B0
		public static List<QizhenGeBuItemData> GetQizhenGeBuItemDataList(DBManager dbMgr)
		{
			long ticks = DateTime.Now.Ticks / 10000L;
			if (ticks - QiZhenGeBuManager.LastQueryTicks >= 600000L)
			{
				QiZhenGeBuManager.QizhenGeBuItemDataList = DBQuery.QueryQizhenGeBuItemDataList(dbMgr);
				QiZhenGeBuManager.LastQueryTicks = ticks;
			}
			return QiZhenGeBuManager.QizhenGeBuItemDataList;
		}

		// Token: 0x04000C02 RID: 3074
		private static List<QizhenGeBuItemData> QizhenGeBuItemDataList = null;

		// Token: 0x04000C03 RID: 3075
		private static long LastQueryTicks = 0L;
	}
}
