using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class JueXingManager : SingletonTemplate<JueXingManager>, IManager, ICmdProcessor
	{
		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(20317, SingletonTemplate<JueXingManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20318, SingletonTemplate<JueXingManager>.Instance());
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
			if (nID == 20317)
			{
				this.GetRoleJueXingData(client, nID, cmdParams, count);
			}
			else if (nID == 20318)
			{
				this.UpdateRoleJueXingData(client, nID, cmdParams, count);
			}
		}

		
		private void GetRoleJueXingData(GameServerClient client, int nID, byte[] cmdParams, int count)
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
					client.sendCmd<List<TaoZhuangData>>(nID, dbRoleInfo.JueXingTaoZhuangList);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<int>(nID, -8);
			}
		}

		
		private void UpdateRoleJueXingData(GameServerClient client, int nID, byte[] cmdParams, int count)
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
				int suitid = Convert.ToInt32(fields[1]);
				string activite = fields[2];
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
						string cmdText = string.Format("REPLACE INTO t_juexing(rid, suitid, activite) VALUES('{0}', '{1}', '{2}')", roleID, suitid, activite);
						conn.ExecuteNonQuery(cmdText, 0);
					}
					TaoZhuangData suitData = dbRoleInfo.JueXingTaoZhuangList.Find((TaoZhuangData _g) => _g.ID == suitid);
					if (null == suitData)
					{
						suitData = new TaoZhuangData
						{
							ID = suitid,
							ActiviteList = new List<int>()
						};
						dbRoleInfo.JueXingTaoZhuangList.Add(suitData);
					}
					suitData.ActiviteList = Array.ConvertAll<string, int>(activite.Split(new char[]
					{
						','
					}), (string x) => Convert.ToInt32(x)).ToList<int>();
					client.sendCmd<int>(nID, 0);
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
