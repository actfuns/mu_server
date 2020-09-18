using System;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class BianShenManager : SingletonTemplate<BianShenManager>, IManager, ICmdProcessor
	{
		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(1449, SingletonTemplate<BianShenManager>.Instance());
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
			if (nID == 1449)
			{
				this.ProcessBianShenLevelStarUpCmd(client, nID, cmdParams, count);
			}
		}

		
		private void ProcessBianShenLevelStarUpCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int ret = 0;
			RoleDataCmdT<RoleBianShenData> data = DataHelper.BytesToObject<RoleDataCmdT<RoleBianShenData>>(cmdParams, 0, count);
			if (data != null && data.RoleID > 0)
			{
				DBManager dbMgr = DBManager.getInstance();
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref data.RoleID);
				if (null != dbRoleInfo)
				{
					if (dbRoleInfo.BianShenData.BianShen != data.Value.BianShen || dbRoleInfo.BianShenData.Exp != data.Value.Exp)
					{
						dbRoleInfo.BianShenData.BianShen = data.Value.BianShen;
						dbRoleInfo.BianShenData.Exp = data.Value.Exp;
						using (MyDbConnection3 conn = new MyDbConnection3(false))
						{
							string cmdText = string.Format("update t_roles set bianshen={1},bianshenexp={2} where rid={0}", data.RoleID, data.Value.BianShen, data.Value.Exp);
							ret = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
						}
					}
				}
			}
			client.sendCmd(nID, string.Format("{0}", ret));
		}

		
		private object Mutex = new object();
	}
}
