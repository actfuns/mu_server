using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	// Token: 0x020001AE RID: 430
	public class DJPointsHotList
	{
		// Token: 0x06000925 RID: 2341 RVA: 0x00055A30 File Offset: 0x00053C30
		public List<DJPointData> GetDJPointsHostList(DBManager dbMgr)
		{
			List<DJPointData> djPointsHostList = null;
			lock (this)
			{
				long ticks = DateTime.Now.Ticks / 10000L;
				if (ticks - this.LastQueryTicks <= 300000L)
				{
					djPointsHostList = this.DJPointsHostList;
				}
				else
				{
					this.LastQueryTicks = ticks;
					this.DJPointsHostList = DBQuery.QueryDJPointData(dbMgr);
					djPointsHostList = this.DJPointsHostList;
				}
			}
			return djPointsHostList;
		}

		// Token: 0x040009D4 RID: 2516
		private List<DJPointData> DJPointsHostList = new List<DJPointData>();

		// Token: 0x040009D5 RID: 2517
		private long LastQueryTicks = 0L;
	}
}
