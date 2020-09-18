using System;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class ShenShiManager : SingletonTemplate<ShenShiManager>, IManager, ICmdProcessor
	{
		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(20315, SingletonTemplate<ShenShiManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20316, SingletonTemplate<ShenShiManager>.Instance());
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
			if (nID == 20315)
			{
				this.AddRoleFuWenTab(client, nID, cmdParams, count);
			}
			else if (nID == 20316)
			{
				this.UpdateRoleFuWenTab(client, nID, cmdParams, count);
			}
		}

		
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
