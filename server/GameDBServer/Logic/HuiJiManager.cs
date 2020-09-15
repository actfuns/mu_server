using System;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x02000171 RID: 369
	public class HuiJiManager : SingletonTemplate<HuiJiManager>, IManager, ICmdProcessor
	{
		// Token: 0x0600067C RID: 1660 RVA: 0x0003B938 File Offset: 0x00039B38
		public bool initialize()
		{
			return true;
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x0003B94C File Offset: 0x00039B4C
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(1446, SingletonTemplate<HuiJiManager>.Instance());
			return true;
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x0003B974 File Offset: 0x00039B74
		public bool showdown()
		{
			return true;
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x0003B988 File Offset: 0x00039B88
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x0003B99C File Offset: 0x00039B9C
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 1446)
			{
				this.ProcessHuiJiLevelStarUpCmd(client, nID, cmdParams, count);
			}
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x0003B9C8 File Offset: 0x00039BC8
		private void ProcessHuiJiLevelStarUpCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int ret = 0;
			RoleDataCmdT<RoleHuiJiData> data = DataHelper.BytesToObject<RoleDataCmdT<RoleHuiJiData>>(cmdParams, 0, count);
			if (data != null && data.RoleID > 0)
			{
				DBManager dbMgr = DBManager.getInstance();
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref data.RoleID);
				if (null != dbRoleInfo)
				{
					if (dbRoleInfo.HuiJiData.huiji != data.Value.huiji || dbRoleInfo.HuiJiData.Exp != data.Value.Exp)
					{
						dbRoleInfo.HuiJiData.huiji = data.Value.huiji;
						dbRoleInfo.HuiJiData.Exp = data.Value.Exp;
						using (MyDbConnection3 conn = new MyDbConnection3(false))
						{
							string cmdText = string.Format("update t_roles set huiji={1},huijiexp={2} where rid={0}", data.RoleID, data.Value.huiji, data.Value.Exp);
							ret = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
						}
					}
				}
			}
			client.sendCmd(nID, string.Format("{0}", ret));
		}

		// Token: 0x0400089C RID: 2204
		private object Mutex = new object();
	}
}
