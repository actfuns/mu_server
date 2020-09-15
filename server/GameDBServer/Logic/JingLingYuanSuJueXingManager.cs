using System;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x02000170 RID: 368
	public class JingLingYuanSuJueXingManager : SingletonTemplate<JingLingYuanSuJueXingManager>, IManager, ICmdProcessor
	{
		// Token: 0x06000675 RID: 1653 RVA: 0x0003B7A8 File Offset: 0x000399A8
		public bool initialize()
		{
			return true;
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x0003B7BC File Offset: 0x000399BC
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(1452, SingletonTemplate<JingLingYuanSuJueXingManager>.Instance());
			return true;
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x0003B7E4 File Offset: 0x000399E4
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x0003B7F8 File Offset: 0x000399F8
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x0003B80C File Offset: 0x00039A0C
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 1452)
			{
				this.ProcessArmorLevelStarUpCmd(client, nID, cmdParams, count);
			}
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x0003B838 File Offset: 0x00039A38
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

		// Token: 0x0400089B RID: 2203
		private object Mutex = new object();
	}
}
