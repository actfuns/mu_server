using System;
using System.Collections.Generic;
using GameDBServer.Logic.Rank;
using Server.Data;

namespace GameDBServer.Logic
{
	
	public class GameDBManager
	{
		
		public const int StatisticsMode = 3;

		
		public const int KuaFuServerIdStartValue = 9000;

		
		public const int ServerLineIdAllIncludeKuaFu = 0;

		
		public const int ServerLineIdAllLineExcludeSelf = -1000;

		
		public static int ZoneID = 1;

		
		public static int PTID = 0;

		
		public static ServerDBInfo serverDBInfo = null;

		
		public static string DBName = "mu_game";

		
		public static ServerEvents SystemServerSQLEvents = new ServerEvents
		{
			EventRootPath = "SQLs",
			EventPreFileName = "sql"
		};

		
		public static DJPointsHotList SysDJPointsHotList = new DJPointsHotList();

		
		public static BulletinMsgManager BulletinMsgMgr = new BulletinMsgManager();

		
		public static GameConfig GameConfigMgr = new GameConfig();

		
		public static BangHuiJunQiManager BangHuiJunQiMgr = new BangHuiJunQiManager();

		
		public static PreDeleteRoleMgr PreDelRoleMgr = new PreDeleteRoleMgr();

		
		public static BangHuiListMgr BangHuiListMgr = new BangHuiListMgr();

		
		public static BangHuiLingDiManager BangHuiLingDiMgr = new BangHuiLingDiManager();

		
		public static MarryPartyDataCache MarryPartyDataC = new MarryPartyDataCache();

		
		public static RankCacheManager RankCacheMgr = new RankCacheManager();

		
		public static int DBAutoIncreaseStepValue = 1000000;

		
		public static DayRechargeRankManager DayRechargeRankMgr = new DayRechargeRankManager();

		
		public static bool Flag_t_goods_delete_immediately = false;

		
		public static int Flag_Splite_RoleParams_Table = 0;

		
		public static int PreDeleteRoleDelaySeconds = 7200;

		
		public static bool DisableSomeLog = true;

		
		public static int Flag_Query_Total_UserMoney_Minute = 60;

		
		public static Dictionary<string, int> IPRange2AutoIncreaseStepDict = new Dictionary<string, int>
		{
			{
				"101.251",
				1000000
			},
			{
				"192.168",
				0
			}
		};
	}
}
