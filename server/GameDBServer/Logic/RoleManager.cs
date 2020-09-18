using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class RoleManager : SingletonTemplate<RoleManager>, IManager, ICmdProcessor
	{
		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(10230, SingletonTemplate<RoleManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(10232, SingletonTemplate<RoleManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(10233, SingletonTemplate<RoleManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(694, SingletonTemplate<RoleManager>.Instance());
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID != 694)
			{
				switch (nID)
				{
				case 10230:
					this.RoleCustomDataQuery(client, nID, cmdParams, count);
					break;
				case 10232:
					this.RoleDataSelectorQuery(client, nID, cmdParams, count);
					break;
				case 10233:
					this.RoleCustomDataUpdate(client, nID, cmdParams, count);
					break;
				}
			}
			else
			{
				this.FastCacheDataUpdate(client, nID, cmdParams, count);
			}
		}

		
		public RoleData4Selector GetRoleData4Selector(int roleID, bool mainOccupation = false)
		{
			DBManager dbMgr = DBManager.getInstance();
			RoleData4Selector roleData4Selector = new RoleData4Selector();
			roleData4Selector.RoleID = -1;
			try
			{
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null != dbRoleInfo)
				{
					if (mainOccupation && dbRoleInfo.OccupationList[0] != dbRoleInfo.Occupation)
					{
						if (dbRoleInfo.roleCustomData == null || null == dbRoleInfo.roleCustomData.roleData4Selector)
						{
							dbRoleInfo.roleCustomData = this.QueryRoleCustomData(roleID);
						}
						if (null != dbRoleInfo.roleCustomData)
						{
							return dbRoleInfo.roleCustomData.roleData4Selector;
						}
					}
					else
					{
						lock (dbRoleInfo)
						{
							Global.DBRoleInfo2RoleData4Selector(dbMgr, dbRoleInfo, roleData4Selector);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return roleData4Selector;
		}

		
		private void RoleCustomDataQuery(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			DBManager dbMgr = DBManager.getInstance();
			int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			RoleCustomData roleCustomData = new RoleCustomData();
			roleCustomData.roleId = roleId;
			try
			{
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleId);
				if (null != dbRoleInfo)
				{
					if (null == dbRoleInfo.roleCustomData)
					{
						dbRoleInfo.roleCustomData = this.QueryRoleCustomData(roleId);
					}
					if (null != dbRoleInfo.roleCustomData)
					{
						roleCustomData.customDataList = dbRoleInfo.roleCustomData.customDataList;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<RoleCustomData>(nID, roleCustomData);
		}

		
		private void RoleDataSelectorQuery(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			RoleData4Selector roleData4Selector = this.GetRoleData4Selector(roleId, true);
			client.sendCmd<RoleData4Selector>(nID, roleData4Selector);
		}

		
		private void RoleCustomDataUpdate(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int ret = 0;
			RoleCustomData data = DataHelper.BytesToObject<RoleCustomData>(cmdParams, 0, count);
			if (data != null)
			{
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					if (data.customDataList != null && data.roleData4Selector != null)
					{
						string p = DataHelper.ObjectToHexString<RoleData4Selector>(data.roleData4Selector);
						string p2 = DataHelper.ObjectToHexString<List<RoleCustomDataItem>>(data.customDataList);
						string cmdText = string.Format("insert into t_roledata(`rid`,`roledata4selector`,`occu_data`) VALUE({0},{1},{2}) on duplicate key update `roledata4selector`={1},`occu_data`={2}", data.roleId, p, p2);
						ret = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
					}
					else if (data.roleData4Selector != null)
					{
						string p = DataHelper.ObjectToHexString<RoleData4Selector>(data.roleData4Selector);
						string cmdText = string.Format("INSERT INTO t_roledata(`rid`,`roledata4selector`) VALUE({0},{1}) on duplicate key update `roledata4selector`={1}", data.roleId, p);
						ret = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
					}
					else if (data.customDataList != null)
					{
						string p = DataHelper.ObjectToHexString<List<RoleCustomDataItem>>(data.customDataList);
						string cmdText = string.Format("INSERT INTO t_roledata(`rid`,`occu_data`) VALUE({0},{1}) on duplicate key update `occu_data`={1}", data.roleId, p);
						ret = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
					}
				}
				DBRoleInfo dbRoleInfo = DBManager.getInstance().GetDBRoleInfo(ref data.roleId);
				if (null != dbRoleInfo)
				{
					if (null == data.roleData4Selector)
					{
						data.roleData4Selector = dbRoleInfo.roleCustomData.roleData4Selector;
					}
					dbRoleInfo.roleCustomData = data;
				}
			}
			client.sendCmd<int>(nID, ret);
		}

		
		private RoleCustomData QueryRoleCustomData(int roleId)
		{
			RoleCustomData roleCustomData = null;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				try
				{
					string cmdText = string.Format("select `occu_data`,`roledata4selector` from t_roledata where rid={0}", roleId);
					MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
					if (reader.Read())
					{
						byte[] bytes0 = reader[0] as byte[];
						byte[] bytes = reader[1] as byte[];
						roleCustomData = new RoleCustomData();
						roleCustomData.roleId = roleId;
						if (null != bytes0)
						{
							roleCustomData.customDataList = DataHelper.BytesToObject<List<RoleCustomDataItem>>(bytes0, 0, bytes0.Length);
						}
						if (null != bytes)
						{
							roleCustomData.roleData4Selector = DataHelper.BytesToObject<RoleData4Selector>(bytes, 0, bytes.Length);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return roleCustomData;
		}

		
		private void FastCacheDataUpdate(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int ret = 0;
			FastCacheData data = DataHelper.BytesToObject<FastCacheData>(cmdParams, 0, count);
			if (data != null)
			{
				int roleID = data.ID;
				DBRoleInfo dbRoleInfo = DBManager.getInstance().GetDBRoleInfo(ref roleID);
				if (null != dbRoleInfo)
				{
					if (data.Flag_BaseInfo)
					{
						dbRoleInfo.CombatForce = (int)data.ZhanLi;
						dbRoleInfo.Position = data.Position;
					}
				}
			}
			client.sendCmd<int>(nID, ret);
		}

		
		private const int ALLY_LOG_COUNT_MAX = 20;

		
		private object Mutex = new object();
	}
}
