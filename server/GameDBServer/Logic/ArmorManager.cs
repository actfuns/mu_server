using System;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x0200016F RID: 367
	public class ArmorManager : SingletonTemplate<ArmorManager>, IManager, ICmdProcessor
	{
		// Token: 0x0600066E RID: 1646 RVA: 0x0003B5AC File Offset: 0x000397AC
		public bool initialize()
		{
			return true;
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x0003B5C0 File Offset: 0x000397C0
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(1447, SingletonTemplate<ArmorManager>.Instance());
			return true;
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x0003B5E8 File Offset: 0x000397E8
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x0003B5FC File Offset: 0x000397FC
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x0003B610 File Offset: 0x00039810
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 1447)
			{
				this.ProcessArmorLevelStarUpCmd(client, nID, cmdParams, count);
			}
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x0003B63C File Offset: 0x0003983C
		private void ProcessArmorLevelStarUpCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int ret = 0;
			RoleDataCmdT<RoleArmorData> data = DataHelper.BytesToObject<RoleDataCmdT<RoleArmorData>>(cmdParams, 0, count);
			if (data != null && data.RoleID > 0)
			{
				DBManager dbMgr = DBManager.getInstance();
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref data.RoleID);
				if (null != dbRoleInfo)
				{
					if (dbRoleInfo.ArmorData.Armor != data.Value.Armor || dbRoleInfo.ArmorData.Exp != data.Value.Exp)
					{
						dbRoleInfo.ArmorData.Armor = data.Value.Armor;
						dbRoleInfo.ArmorData.Exp = data.Value.Exp;
						using (MyDbConnection3 conn = new MyDbConnection3(false))
						{
							string cmdText = string.Format("update t_roles set armor={1},armorexp={2} where rid={0}", data.RoleID, data.Value.Armor, data.Value.Exp);
							ret = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
						}
					}
				}
			}
			client.sendCmd(nID, string.Format("{0}", ret));
		}

		// Token: 0x0400089A RID: 2202
		private object Mutex = new object();
	}
}
