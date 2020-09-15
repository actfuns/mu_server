using System;
using GameDBServer.DB;
using GameDBServer.Server;
using GameDBServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x02000174 RID: 372
	public class ShenJiManager : SingletonTemplate<ShenJiManager>, IManager, ICmdProcessor
	{
		// Token: 0x0600068D RID: 1677 RVA: 0x0003BD38 File Offset: 0x00039F38
		public bool initialize()
		{
			return true;
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x0003BD4C File Offset: 0x00039F4C
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13095, SingletonTemplate<ShenJiManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13096, SingletonTemplate<ShenJiManager>.Instance());
			return true;
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x0003BD8C File Offset: 0x00039F8C
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x0003BDA0 File Offset: 0x00039FA0
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x0003BDB4 File Offset: 0x00039FB4
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			switch (nID)
			{
			case 13095:
				this.ShenJiModify(client, nID, cmdParams, count);
				break;
			case 13096:
				this.ShenJiClear(client, nID, cmdParams, count);
				break;
			}
		}

		// Token: 0x06000692 RID: 1682 RVA: 0x0003BDF8 File Offset: 0x00039FF8
		private void ShenJiModify(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string[] fields = null;
			int length = 3;
			if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out fields, length))
			{
				client.sendCmd<bool>(nID, false);
			}
			else
			{
				int roleID = int.Parse(fields[0]);
				int shenjiID = int.Parse(fields[1]);
				int shenjiLev = int.Parse(fields[2]);
				DBRoleInfo dbRoleInfo = DBManager.getInstance().FindDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("精灵神迹，找不到玩家 roleid={0}", roleID), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					bool ret = false;
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string cmdText = string.Format("REPLACE INTO t_shenjifuwen(rid, sjID, level) VALUES('{0}', '{1}', '{2}')", roleID, shenjiID, shenjiLev);
						ret = conn.ExecuteNonQueryBool(cmdText, 0);
					}
					if (!ret)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色更新精灵神迹数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						client.sendCmd<bool>(nID, ret);
					}
					else
					{
						lock (dbRoleInfo.ShenJiDict)
						{
							ShenJiFuWenData data = new ShenJiFuWenData
							{
								ShenJiID = shenjiID,
								Level = shenjiLev
							};
							dbRoleInfo.ShenJiDict[shenjiID] = data;
						}
						client.sendCmd<bool>(nID, ret);
					}
				}
			}
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x0003BF98 File Offset: 0x0003A198
		private void ShenJiClear(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string[] fields = null;
			int length = 1;
			if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out fields, length))
			{
				client.sendCmd<bool>(nID, false);
			}
			else
			{
				int roleID = int.Parse(fields[0]);
				DBRoleInfo dbRoleInfo = DBManager.getInstance().FindDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("精灵神迹，找不到玩家 roleid={0}", roleID), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					bool ret = false;
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string cmdText = string.Format("DELETE FROM t_shenjifuwen where rid={0}", roleID);
						ret = conn.ExecuteNonQueryBool(cmdText, 0);
					}
					if (!ret)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色更新精灵神迹数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						client.sendCmd<bool>(nID, ret);
					}
					else
					{
						lock (dbRoleInfo.ShenJiDict)
						{
							dbRoleInfo.ShenJiDict.Clear();
						}
						client.sendCmd<bool>(nID, ret);
					}
				}
			}
		}
	}
}
