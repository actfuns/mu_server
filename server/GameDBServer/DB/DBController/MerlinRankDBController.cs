using System;
using System.Collections.Generic;
using GameDBServer.Logic.MerlinMagicBook;
using Server.Data;

namespace GameDBServer.DB.DBController
{
	// Token: 0x020000E7 RID: 231
	public class MerlinRankDBController : DBController<MerlinRankingInfo>
	{
		// Token: 0x06000208 RID: 520 RVA: 0x0000B556 File Offset: 0x00009756
		private MerlinRankDBController()
		{
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000B564 File Offset: 0x00009764
		public static MerlinRankDBController getInstance()
		{
			return MerlinRankDBController.instance;
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000B57C File Offset: 0x0000977C
		public MerlinRankingInfo getMerlinDataByRoleID(int nRoleID)
		{
			string sql = string.Format("select *,(SELECT rname  FROM t_roles WHERE rid = roleID ) AS roleName from t_merlin_magic_book where roleID = {0};", nRoleID);
			return base.queryForObject(sql);
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000B5A8 File Offset: 0x000097A8
		public List<MerlinRankingInfo> getPlayerMerlinDataList()
		{
			string sql = string.Format("select *,(SELECT rname  FROM t_roles WHERE rid = roleID ) AS roleName from t_merlin_magic_book order by level desc, starNum desc, addTime asc limit {0};", MerlinRankManager.RankingList_Max_Num);
			return base.queryForList(sql);
		}

		// Token: 0x04000629 RID: 1577
		private static MerlinRankDBController instance = new MerlinRankDBController();
	}
}
