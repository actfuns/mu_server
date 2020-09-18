using System;
using System.Collections.Generic;
using GameDBServer.Logic.MerlinMagicBook;
using Server.Data;

namespace GameDBServer.DB.DBController
{
	
	public class MerlinRankDBController : DBController<MerlinRankingInfo>
	{
		
		private MerlinRankDBController()
		{
		}

		
		public static MerlinRankDBController getInstance()
		{
			return MerlinRankDBController.instance;
		}

		
		public MerlinRankingInfo getMerlinDataByRoleID(int nRoleID)
		{
			string sql = string.Format("select *,(SELECT rname  FROM t_roles WHERE rid = roleID ) AS roleName from t_merlin_magic_book where roleID = {0};", nRoleID);
			return base.queryForObject(sql);
		}

		
		public List<MerlinRankingInfo> getPlayerMerlinDataList()
		{
			string sql = string.Format("select *,(SELECT rname  FROM t_roles WHERE rid = roleID ) AS roleName from t_merlin_magic_book order by level desc, starNum desc, addTime asc limit {0};", MerlinRankManager.RankingList_Max_Num);
			return base.queryForList(sql);
		}

		
		private static MerlinRankDBController instance = new MerlinRankDBController();
	}
}
