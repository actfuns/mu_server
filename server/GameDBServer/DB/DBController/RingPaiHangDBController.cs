using System;
using System.Collections.Generic;
using GameDBServer.Logic;
using Server.Data;

namespace GameDBServer.DB.DBController
{
	// Token: 0x020000E6 RID: 230
	public class RingPaiHangDBController : DBController<RingRankingInfo>
	{
		// Token: 0x06000203 RID: 515 RVA: 0x0000B4CA File Offset: 0x000096CA
		private RingPaiHangDBController()
		{
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000B4D8 File Offset: 0x000096D8
		public static RingPaiHangDBController getInstance()
		{
			return RingPaiHangDBController.instance;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000B4F0 File Offset: 0x000096F0
		public RingRankingInfo getRingDataById(int Id)
		{
			string sql = string.Format("SELECT roleid, (SELECT rname  FROM t_roles WHERE rid = roleid ) AS rolename, goodwilllevel, ringid, goodwillstar, changtime FROM t_marry WHERE roleid = {0}", Id);
			return base.queryForObject(sql);
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000B51C File Offset: 0x0000971C
		public List<RingRankingInfo> getPlayerRingDataList()
		{
			string sql = string.Format("SELECT roleid, (SELECT rname  FROM t_roles WHERE rid = roleid ) AS rolename, goodwilllevel, goodwillstar, ringid, changtime FROM t_marry WHERE goodwilllevel != 0 order by goodwilllevel desc, goodwillstar desc, changtime asc limit {0};", RingPaiHangManager.RankingList_Max_Num);
			return base.queryForList(sql);
		}

		// Token: 0x04000628 RID: 1576
		private static RingPaiHangDBController instance = new RingPaiHangDBController();
	}
}
