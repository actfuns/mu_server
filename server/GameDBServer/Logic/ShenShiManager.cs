using System;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x02000177 RID: 375
	public class ShenShiManager : SingletonTemplate<ShenShiManager>, IManager, ICmdProcessor
	{
		// Token: 0x06000697 RID: 1687 RVA: 0x0003C110 File Offset: 0x0003A310
		public bool initialize()
		{
			return true;
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x0003C124 File Offset: 0x0003A324
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(20315, SingletonTemplate<ShenShiManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20316, SingletonTemplate<ShenShiManager>.Instance());
			return true;
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x0003C164 File Offset: 0x0003A364
		public bool showdown()
		{
			return true;
		}

		// Token: 0x0600069A RID: 1690 RVA: 0x0003C178 File Offset: 0x0003A378
		public bool destroy()
		{
			return true;
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x0003C18C File Offset: 0x0003A38C
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 20315)
			{
				this.AddRoleFuWenTab(client, nID, cmdParams, count);
			}
			else if (nID == 20316)
			{
				this.UpdateRoleFuWenTab(client, nID, cmdParams, count);
			}
		}

		// Token: 0x0600069C RID: 1692 RVA: 0x0003C1D8 File Offset: 0x0003A3D8
		private void AddRoleFuWenTab(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				FuWenTabData newTab = DataHelper.BytesToObject<FuWenTabData>(cmdParams, 0, count);
				int roleID = newTab.OwnerID;
				DBRoleInfo dbRoleInfo = DBManager.getInstance().FindDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					bool ret = false;
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string fuWenEquip = (newTab.FuWenEquipList == null) ? "" : string.Join<int>(",", newTab.FuWenEquipList);
						string shenShiActive = (newTab.ShenShiActiveList == null) ? "" : string.Join<int>(",", newTab.ShenShiActiveList);
						string cmdText = string.Format("REPLACE INTO t_fuwen(rid, tabid, name, fuwenequip, shenshiactive, skillequip) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", new object[]
						{
							roleID,
							newTab.TabID,
							newTab.Name,
							fuWenEquip,
							shenShiActive,
							newTab.SkillEquip
						});
						ret = conn.ExecuteNonQueryBool(cmdText, 0);
					}
					if (!ret)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色插入符文页数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						client.sendCmd<int>(nID, -8);
					}
					else
					{
						lock (dbRoleInfo.FuWenTabList)
						{
							dbRoleInfo.FuWenTabList.Add(newTab);
						}
						client.sendCmd<int>(nID, ret ? 0 : -8);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<int>(nID, -8);
			}
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x0003C3FC File Offset: 0x0003A5FC
		private void UpdateRoleFuWenTab(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				FuWenTabData newTab = DataHelper.BytesToObject<FuWenTabData>(cmdParams, 0, count);
				int roleID = newTab.OwnerID;
				DBRoleInfo dbRoleInfo = DBManager.getInstance().FindDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					bool ret = false;
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string fuWenEquip = (newTab.FuWenEquipList == null) ? "" : string.Join<int>(",", newTab.FuWenEquipList);
						string shenShiActive = (newTab.ShenShiActiveList == null) ? "" : string.Join<int>(",", newTab.ShenShiActiveList);
						string cmdText = string.Format("REPLACE INTO t_fuwen(rid, tabid, name, fuwenequip, shenshiactive, skillequip) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", new object[]
						{
							roleID,
							newTab.TabID,
							newTab.Name,
							fuWenEquip,
							shenShiActive,
							newTab.SkillEquip
						});
						ret = conn.ExecuteNonQueryBool(cmdText, 0);
					}
					if (!ret)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色插入符文页数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						client.sendCmd<int>(nID, -8);
					}
					else
					{
						lock (dbRoleInfo.FuWenTabList)
						{
							dbRoleInfo.FuWenTabList[newTab.TabID] = newTab;
						}
						client.sendCmd<int>(nID, ret ? 0 : -8);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<int>(nID, -8);
			}
		}
	}
}
