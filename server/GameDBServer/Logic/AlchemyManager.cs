using System;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using GameDBServer.Tools;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class AlchemyManager : SingletonTemplate<AlchemyManager>, IManager, ICmdProcessor
	{
		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13097, SingletonTemplate<AlchemyManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13098, SingletonTemplate<AlchemyManager>.Instance());
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
			switch (nID)
			{
			case 13097:
				this.AlchemyModify(client, nID, cmdParams, count);
				break;
			case 13098:
				this.AlchemyRollBack(client, nID, cmdParams, count);
				break;
			}
		}

		
		public TCPProcessCmdResults ProcessUpdateAlchemyElement(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int addOrSubElement = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (dbRoleInfo == null || null == dbRoleInfo.AlchemyInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool failed = false;
				int roleElement = 0;
				lock (dbRoleInfo)
				{
					dbRoleInfo.AlchemyInfo.BaseData.Element += addOrSubElement;
					roleElement = dbRoleInfo.AlchemyInfo.BaseData.Element;
				}
				string strcmd;
				if (failed)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (addOrSubElement != 0)
				{
					bool ret = false;
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string cmdText = string.Format("UPDATE t_alchemy SET element='{0}' WHERE rid={1};", roleElement, roleID);
						ret = conn.ExecuteNonQueryBool(cmdText, 0);
					}
					if (!ret)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色炼金元素值失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						strcmd = string.Format("{0}:{1}", roleID, -2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}", roleID, roleElement);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		private void AlchemyRollBack(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string[] fields = null;
				int length = 2;
				if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out fields, length))
				{
					client.sendCmd<bool>(nID, false);
				}
				else
				{
					int roleID = int.Parse(fields[0]);
					string rollbackType = fields[1];
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
							string cmdText = string.Format("UPDATE t_alchemy SET rollback='{0}' WHERE rid={1};", rollbackType, roleID);
							ret = conn.ExecuteNonQueryBool(cmdText, 0);
						}
						lock (dbRoleInfo)
						{
							if (null != dbRoleInfo.AlchemyInfo)
							{
								dbRoleInfo.AlchemyInfo.rollbackType = rollbackType;
							}
						}
						client.sendCmd<bool>(nID, ret);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
		}

		
		private void AlchemyModify(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				AlchemyDataDB dbData = DataHelper.BytesToObject<AlchemyDataDB>(cmdParams, 0, count);
				DBRoleInfo dbRoleInfo = DBManager.getInstance().FindDBRoleInfo(ref dbData.RoleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("炼金系统，找不到玩家 roleid={0}", dbData.RoleID), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					bool ret = false;
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string cmdText = string.Format("REPLACE INTO t_alchemy(rid, element, dayid, value, todaycost, histcost) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", new object[]
						{
							dbData.RoleID,
							dbData.BaseData.Element,
							dbData.ElementDayID,
							dbData.getStringValue(dbData.BaseData.AlchemyValue),
							dbData.getStringValue(dbData.BaseData.ToDayCost),
							dbData.getStringValue(dbData.HistCost)
						});
						ret = conn.ExecuteNonQueryBool(cmdText, 0);
					}
					if (!ret)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色炼金数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, dbData.RoleID), null, true);
						client.sendCmd<bool>(nID, ret);
					}
					else
					{
						lock (dbRoleInfo)
						{
							dbRoleInfo.AlchemyInfo = dbData;
						}
						client.sendCmd<bool>(nID, ret);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
		}
	}
}
