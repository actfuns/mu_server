using System;
using System.Collections.Generic;
using GameDBServer.Logic.WanMoTa;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB.DBController
{
	// Token: 0x020000E8 RID: 232
	public class WanMoTaDBController : DBController<WanMotaInfo>
	{
		// Token: 0x0600020D RID: 525 RVA: 0x0000B5E2 File Offset: 0x000097E2
		private WanMoTaDBController()
		{
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000B5F0 File Offset: 0x000097F0
		public static WanMoTaDBController getInstance()
		{
			return WanMoTaDBController.instance;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000B608 File Offset: 0x00009808
		public WanMotaInfo getPlayerWanMoTaDataById(int Id)
		{
			string sql = string.Format("select * from t_wanmota where roleID = {0};", Id);
			return base.queryForObject(sql);
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000B634 File Offset: 0x00009834
		public List<WanMotaInfo> getPlayerWanMotaDataList()
		{
			string sql = string.Format("select * from t_wanmota order by passLayerCount desc, flushTime asc limit {0};", WanMoTaManager.RankingList_Max_Num);
			return base.queryForList(sql);
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000B664 File Offset: 0x00009864
		public static int updateWanMoTaData(DBManager dbMgr, int nRoleID, string[] fields, int startIndex)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = DBWriter.FormatUpdateSQL(nRoleID, fields, startIndex, WanMoTaDBController._fieldNames, "t_wanmota", WanMoTaDBController._fieldTypes, "roleID");
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000B6D0 File Offset: 0x000098D0
		public int insertWanMoTaData(DBManager dbMgr, WanMotaInfo data)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				try
				{
					string cmdText = string.Format("INSERT INTO t_wanmota (roleID, roleName, flushTime, passLayerCount, sweepLayer, sweepReward, sweepBeginTime) VALUES({0}, '{1}', {2}, {3}, {4}, '{5}', {6})", new object[]
					{
						data.nRoleID,
						data.strRoleName,
						data.lFlushTime,
						data.nPassLayerCount,
						data.nSweepLayer,
						data.strSweepReward,
						data.lSweepBeginTime
					});
					ret = conn.ExecuteNonQuery(cmdText, 0);
					if (ret < 0)
					{
						return ret;
					}
					ret = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
				}
				catch (MySQLException)
				{
					ret = -2;
				}
			}
			return ret;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000B7D0 File Offset: 0x000099D0
		internal void OnChangeName(int roleid, string oldName, string newName)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_wanmota SET roleName='{0}' WHERE roleId={1}", newName, roleid);
				if (conn.ExecuteNonQuery(cmdText, 0) < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("角色改名，更新t_wanmota失败, roleId={0}, oldName={1}, newName={2}", roleid, oldName, newName), null, true);
				}
			}
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000B84C File Offset: 0x00009A4C
		// Note: this type is marked as 'beforefieldinit'.
		static WanMoTaDBController()
		{
			byte[] array = new byte[5];
			array[3] = 1;
			WanMoTaDBController._fieldTypes = array;
		}

		// Token: 0x0400062A RID: 1578
		private static WanMoTaDBController instance = new WanMoTaDBController();

		// Token: 0x0400062B RID: 1579
		private static readonly string[] _fieldNames = new string[]
		{
			"flushTime",
			"passLayerCount",
			"sweepLayer",
			"sweepReward",
			"sweepBeginTime"
		};

		// Token: 0x0400062C RID: 1580
		private static readonly byte[] _fieldTypes;
	}
}
