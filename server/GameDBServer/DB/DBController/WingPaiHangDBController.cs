using System;
using System.Collections.Generic;
using GameDBServer.Logic.Wing;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB.DBController
{
	// Token: 0x020000E9 RID: 233
	public class WingPaiHangDBController : DBController<WingRankingInfo>
	{
		// Token: 0x06000215 RID: 533 RVA: 0x0000B8A9 File Offset: 0x00009AA9
		private WingPaiHangDBController()
		{
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000B8B4 File Offset: 0x00009AB4
		public static WingPaiHangDBController getInstance()
		{
			return WingPaiHangDBController.instance;
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000B8CC File Offset: 0x00009ACC
		public WingRankingInfo getWingDataById(int Id)
		{
			string sql = string.Format("select * from t_wings where rid = {0};", Id);
			return base.queryForObject(sql);
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000B8F8 File Offset: 0x00009AF8
		public List<WingRankingInfo> getPlayerWingDataList()
		{
			string sql = string.Format("select * from t_wings order by wingid desc, forgeLevel desc, addtime asc limit {0};", WingPaiHangManager.RankingList_Max_Num);
			return base.queryForList(sql);
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000B928 File Offset: 0x00009B28
		internal void OnChangeName(int roleid, string oldName, string newName)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_wings SET rname='{0}' WHERE rid={1}", newName, roleid);
				if (conn.ExecuteNonQuery(cmdText, 0) < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("角色改名，更新t_wings失败, roleId={0}, oldName={1}, newName={2}", roleid, oldName, newName), null, true);
				}
			}
		}

		// Token: 0x0400062D RID: 1581
		private static WingPaiHangDBController instance = new WingPaiHangDBController();
	}
}
