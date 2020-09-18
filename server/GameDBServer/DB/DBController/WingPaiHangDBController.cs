using System;
using System.Collections.Generic;
using GameDBServer.Logic.Wing;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB.DBController
{
	
	public class WingPaiHangDBController : DBController<WingRankingInfo>
	{
		
		private WingPaiHangDBController()
		{
		}

		
		public static WingPaiHangDBController getInstance()
		{
			return WingPaiHangDBController.instance;
		}

		
		public WingRankingInfo getWingDataById(int Id)
		{
			string sql = string.Format("select * from t_wings where rid = {0};", Id);
			return base.queryForObject(sql);
		}

		
		public List<WingRankingInfo> getPlayerWingDataList()
		{
			string sql = string.Format("select * from t_wings order by wingid desc, forgeLevel desc, addtime asc limit {0};", WingPaiHangManager.RankingList_Max_Num);
			return base.queryForList(sql);
		}

		
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

		
		private static WingPaiHangDBController instance = new WingPaiHangDBController();
	}
}
