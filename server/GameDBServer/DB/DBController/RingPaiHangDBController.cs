using System;
using System.Collections.Generic;
using GameDBServer.Logic;
using Server.Data;

namespace GameDBServer.DB.DBController
{
	
	public class RingPaiHangDBController : DBController<RingRankingInfo>
	{
		
		private RingPaiHangDBController()
		{
		}

		
		public static RingPaiHangDBController getInstance()
		{
			return RingPaiHangDBController.instance;
		}

		
		public RingRankingInfo getRingDataById(int Id)
		{
			string sql = string.Format("SELECT roleid, (SELECT rname  FROM t_roles WHERE rid = roleid ) AS rolename, goodwilllevel, ringid, goodwillstar, changtime FROM t_marry WHERE roleid = {0}", Id);
			return base.queryForObject(sql);
		}

		
		public List<RingRankingInfo> getPlayerRingDataList()
		{
			string sql = string.Format("SELECT roleid, (SELECT rname  FROM t_roles WHERE rid = roleid ) AS rolename, goodwilllevel, goodwillstar, ringid, changtime FROM t_marry WHERE goodwilllevel != 0 order by goodwilllevel desc, goodwillstar desc, changtime asc limit {0};", RingPaiHangManager.RankingList_Max_Num);
			return base.queryForList(sql);
		}

		
		private static RingPaiHangDBController instance = new RingPaiHangDBController();
	}
}
