using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x02000172 RID: 370
	public class RoleMapper : SingletonTemplate<RoleMapper>, IManager, ICmdProcessor
	{
		// Token: 0x06000683 RID: 1667 RVA: 0x0003BB34 File Offset: 0x00039D34
		public bool initialize()
		{
			return true;
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x0003BB48 File Offset: 0x00039D48
		public bool startup()
		{
			return true;
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x0003BB5C File Offset: 0x00039D5C
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000686 RID: 1670 RVA: 0x0003BB70 File Offset: 0x00039D70
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000687 RID: 1671 RVA: 0x0003BB84 File Offset: 0x00039D84
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 10230)
			{
				this.RoleCustomDataQuery(client, nID, cmdParams, count);
			}
		}

		// Token: 0x06000688 RID: 1672 RVA: 0x0003BBB0 File Offset: 0x00039DB0
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
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<RoleCustomData>(nID, roleCustomData);
		}

		// Token: 0x06000689 RID: 1673 RVA: 0x0003BC24 File Offset: 0x00039E24
		public void SetTempRoleID(int localRoleID, int tempRoleID)
		{
			lock (this.Mutex)
			{
				KuaFuWorldRoleData data = new KuaFuWorldRoleData
				{
					LocalRoleID = localRoleID,
					TempRoleID = tempRoleID
				};
				this.KuaFuWorldRoleDataDict[tempRoleID] = data;
			}
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x0003BC90 File Offset: 0x00039E90
		public int GetLocalRoleIDByTempID(int tempRoleID)
		{
			lock (this.Mutex)
			{
				KuaFuWorldRoleData data;
				if (this.KuaFuWorldRoleDataDict.TryGetValue(tempRoleID, out data))
				{
					return data.LocalRoleID;
				}
			}
			return 0;
		}

		// Token: 0x0400089D RID: 2205
		public const int MinLocalRoleID = 200000;

		// Token: 0x0400089E RID: 2206
		private object Mutex = new object();

		// Token: 0x0400089F RID: 2207
		private Dictionary<int, KuaFuWorldRoleData> KuaFuWorldRoleDataDict = new Dictionary<int, KuaFuWorldRoleData>();
	}
}
