using System;
using System.Collections.Generic;
using GameDBServer.Logic.Rank;
using Server.Data;

namespace GameDBServer.Logic
{
	// Token: 0x020001CB RID: 459
	public class GameDBManager
	{
		// Token: 0x04000BBD RID: 3005
		public const int StatisticsMode = 3;

		// Token: 0x04000BBE RID: 3006
		public const int KuaFuServerIdStartValue = 9000;

		// Token: 0x04000BBF RID: 3007
		public const int ServerLineIdAllIncludeKuaFu = 0;

		// Token: 0x04000BC0 RID: 3008
		public const int ServerLineIdAllLineExcludeSelf = -1000;

		// Token: 0x04000BC1 RID: 3009
		public static int ZoneID = 1;

		// Token: 0x04000BC2 RID: 3010
		public static int PTID = 0;

		// Token: 0x04000BC3 RID: 3011
		public static ServerDBInfo serverDBInfo = null;

		// Token: 0x04000BC4 RID: 3012
		public static string DBName = "mu_game";

		// Token: 0x04000BC5 RID: 3013
		public static ServerEvents SystemServerSQLEvents = new ServerEvents
		{
			EventRootPath = "SQLs",
			EventPreFileName = "sql"
		};

		// Token: 0x04000BC6 RID: 3014
		public static DJPointsHotList SysDJPointsHotList = new DJPointsHotList();

		// Token: 0x04000BC7 RID: 3015
		public static BulletinMsgManager BulletinMsgMgr = new BulletinMsgManager();

		// Token: 0x04000BC8 RID: 3016
		public static GameConfig GameConfigMgr = new GameConfig();

		// Token: 0x04000BC9 RID: 3017
		public static BangHuiJunQiManager BangHuiJunQiMgr = new BangHuiJunQiManager();

		// Token: 0x04000BCA RID: 3018
		public static PreDeleteRoleMgr PreDelRoleMgr = new PreDeleteRoleMgr();

		// Token: 0x04000BCB RID: 3019
		public static BangHuiListMgr BangHuiListMgr = new BangHuiListMgr();

		// Token: 0x04000BCC RID: 3020
		public static BangHuiLingDiManager BangHuiLingDiMgr = new BangHuiLingDiManager();

		// Token: 0x04000BCD RID: 3021
		public static MarryPartyDataCache MarryPartyDataC = new MarryPartyDataCache();

		// Token: 0x04000BCE RID: 3022
		public static RankCacheManager RankCacheMgr = new RankCacheManager();

		// Token: 0x04000BCF RID: 3023
		public static int DBAutoIncreaseStepValue = 1000000;

		// Token: 0x04000BD0 RID: 3024
		public static DayRechargeRankManager DayRechargeRankMgr = new DayRechargeRankManager();

		// Token: 0x04000BD1 RID: 3025
		public static bool Flag_t_goods_delete_immediately = false;

		// Token: 0x04000BD2 RID: 3026
		public static int Flag_Splite_RoleParams_Table = 0;

		// Token: 0x04000BD3 RID: 3027
		public static int PreDeleteRoleDelaySeconds = 7200;

		// Token: 0x04000BD4 RID: 3028
		public static bool DisableSomeLog = true;

		// Token: 0x04000BD5 RID: 3029
		public static int Flag_Query_Total_UserMoney_Minute = 60;

		// Token: 0x04000BD6 RID: 3030
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
