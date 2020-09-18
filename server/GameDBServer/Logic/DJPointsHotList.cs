using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	
	public class DJPointsHotList
	{
		
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

		
		private List<DJPointData> DJPointsHostList = new List<DJPointData>();

		
		private long LastQueryTicks = 0L;
	}
}
