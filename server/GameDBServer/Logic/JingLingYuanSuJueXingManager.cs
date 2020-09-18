using System;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class JingLingYuanSuJueXingManager : SingletonTemplate<JingLingYuanSuJueXingManager>, IManager, ICmdProcessor
	{
		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(1452, SingletonTemplate<JingLingYuanSuJueXingManager>.Instance());
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
			if (nID == 1452)
			{
				this.ProcessArmorLevelStarUpCmd(client, nID, cmdParams, count);
			}
		}

		
		private void ProcessArmorLevelStarUpCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int ret = 0;
			RoleDataCmdT<JingLingYuanSuJueXingData> data = DataHelper.BytesToObject<RoleDataCmdT<JingLingYuanSuJueXingData>>(cmdParams, 0, count);
			if (data != null && data.RoleID > 0)
			{
				DBManager dbMgr = DBManager.getInstance();
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref data.RoleID);
				if (null != dbRoleInfo)
				{
					dbRoleInfo.JingLingYuanSuJueXingData = data.Value;
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string cmdText = string.Format("insert into t_juexing_jlys(rid,activetype,activeids) values({0},{1},'{2}') on duplicate key update activetype={1},activeids='{2}'", data.RoleID, data.Value.ActiveType, string.Join<int>(",", data.Value.ActiveIDs));
						ret = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
					}
				}
			}
			client.sendCmd<int>(nID, ret);
		}

		
		private object Mutex = new object();
	}
}
