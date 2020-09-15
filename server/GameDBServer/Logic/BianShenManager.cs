using System;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x0200016E RID: 366
	public class BianShenManager : SingletonTemplate<BianShenManager>, IManager, ICmdProcessor
	{
		// Token: 0x06000667 RID: 1639 RVA: 0x0003B3B0 File Offset: 0x000395B0
		public bool initialize()
		{
			return true;
		}

		// Token: 0x06000668 RID: 1640 RVA: 0x0003B3C4 File Offset: 0x000395C4
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(1449, SingletonTemplate<BianShenManager>.Instance());
			return true;
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x0003B3EC File Offset: 0x000395EC
		public bool showdown()
		{
			return true;
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x0003B400 File Offset: 0x00039600
		public bool destroy()
		{
			return true;
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x0003B414 File Offset: 0x00039614
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 1449)
			{
				this.ProcessBianShenLevelStarUpCmd(client, nID, cmdParams, count);
			}
		}

		// Token: 0x0600066C RID: 1644 RVA: 0x0003B440 File Offset: 0x00039640
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

		// Token: 0x04000899 RID: 2201
		private object Mutex = new object();
	}
}
