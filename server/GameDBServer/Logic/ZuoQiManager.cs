using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class ZuoQiManager : SingletonTemplate<ZuoQiManager>, IManager, ICmdProcessor
	{
		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(20319, SingletonTemplate<ZuoQiManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20320, SingletonTemplate<ZuoQiManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20321, SingletonTemplate<ZuoQiManager>.Instance());
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
			if (nID == 20319)
			{
				this.GetRoleZuoQiData(client, nID, cmdParams, count);
			}
			else if (nID == 20320)
			{
				this.SetRoleZuoQiData(client, nID, cmdParams, count);
			}
			else if (nID == 20321)
			{
				this.CheckRoleZuoQiData(client, nID, cmdParams, count);
			}
		}

		
		private void GetRoleZuoQiData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
				int roleID = Convert.ToInt32(cmdData);
				DBRoleInfo dbRoleInfo = DBManager.getInstance().FindDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					client.sendCmd<List<MountData>>(nID, dbRoleInfo.MountList);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<int>(nID, -8);
			}
		}

		
		private void SetRoleZuoQiData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					client.sendCmd<int>(nID, -4);
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsID = Convert.ToInt32(fields[1]);
				int isNew = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = DBManager.getInstance().FindDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string cmdText = string.Format("REPLACE INTO t_zuoqi(rid, goodsid, isnew) VALUES('{0}', '{1}', '{2}')", roleID, goodsID, isNew);
						conn.ExecuteNonQuery(cmdText, 0);
					}
					MountData mountData = dbRoleInfo.MountList.Find((MountData _g) => _g.GoodsID == goodsID);
					if (null == mountData)
					{
						mountData = new MountData
						{
							GoodsID = goodsID,
							IsNew = (1 == isNew)
						};
						dbRoleInfo.MountList.Add(mountData);
					}
					else
					{
						mountData.IsNew = (1 == isNew);
					}
					client.sendCmd<int>(nID, 1);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<int>(nID, -8);
			}
		}

		
		private void CheckRoleZuoQiData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
				int roleID = Convert.ToInt32(cmdData);
				DBRoleInfo dbRoleInfo = DBManager.getInstance().FindDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string cmdText = string.Format("update t_zuoqi set isnew=0 where rid={0}", roleID);
						conn.ExecuteNonQuery(cmdText, 0);
					}
					foreach (MountData item in dbRoleInfo.MountList)
					{
						item.IsNew = false;
					}
					client.sendCmd<int>(nID, 1);
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
