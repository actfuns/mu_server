using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class RebornEquip : SingletonTemplate<RebornEquip>, IManager, ICmdProcessor
	{
		
		public bool initialize()
		{
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

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(14123, SingletonTemplate<RebornEquip>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14124, SingletonTemplate<RebornEquip>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14125, SingletonTemplate<RebornEquip>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14126, SingletonTemplate<RebornEquip>.Instance());
			return true;
		}

		
		public static TCPProcessCmdResults ProcessUpdateRoleRebornBagNumCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int extGridNum = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateRoleRebornBagNum(dbMgr, roleID, extGridNum);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, ret);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色随身仓库信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.RebornBagNum = extGridNum;
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public static TCPProcessCmdResults ProcessUpdateRebornStorageInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int extGridNum = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateRoleRebornStorageInfo(dbMgr, roleID, extGridNum);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, ret);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色随身仓库信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.RebornGirdData.ExtGridNum = extGridNum;
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public static TCPProcessCmdResults ProcessUpdateRoleRebornShowEquipCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int Show = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateRoleRebornShowEquip(dbMgr, roleID, Show);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, ret);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色显示装备信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.RebornShowEquip = Show;
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public static TCPProcessCmdResults ProcessUpdateRoleRebornShowModelCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int Show = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateRoleRebornShowModel(dbMgr, roleID, Show);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, ret);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色显示时装信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.RebornShowModel = Show;
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			switch (nID)
			{
			case 14123:
				this.InsertHoleInfo(client, nID, cmdParams, count);
				break;
			case 14124:
				this.UpdateHoleInfo(client, nID, cmdParams, count);
				break;
			case 14125:
				this.InsertMazingerStoreInfo(client, nID, cmdParams, count);
				break;
			case 14126:
				this.UpdateMazingerStoreInfo(client, nID, cmdParams, count);
				break;
			}
		}

		
		public static Dictionary<int, RebornEquipData> GetRebornEquipHoleData(DBRoleInfo dbRoleInfo)
		{
			Dictionary<int, RebornEquipData> data = null;
			DBQuery.GetAllRebornEquipHole(DBManager.getInstance(), dbRoleInfo.RoleID, out data);
			return data;
		}

		
		public void InsertHoleInfo(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				RebornEquipData data = DataHelper.BytesToObject<RebornEquipData>(cmdParams, 0, count);
				if (data != null)
				{
					if (!DBWriter.InsertRebornEquipHoleInfo(data.RoleID, data.HoleID, data.Level, data.Able))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("插入重生装备槽信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, data.RoleID), null, true);
						client.sendCmd<int>(nID, -1);
					}
				}
			}
			catch (Exception ex)
			{
				client.sendCmd<int>(nID, -1);
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<int>(nID, 1);
		}

		
		public void UpdateHoleInfo(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				RebornEquipData data = DataHelper.BytesToObject<RebornEquipData>(cmdParams, 0, count);
				if (data != null)
				{
					if (!DBWriter.UpdateRebornEquipHoleInfo(data.RoleID, data.HoleID, data.Level, data.Able))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新重生装备槽信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, data.RoleID), null, true);
						client.sendCmd<int>(nID, -1);
					}
				}
			}
			catch (Exception ex)
			{
				client.sendCmd<int>(nID, -1);
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<int>(nID, 1);
		}

		
		public static Dictionary<int, MazingerStoreData> GetMazingerStoreData(DBRoleInfo dbRoleInfo)
		{
			Dictionary<int, MazingerStoreData> data = null;
			DBQuery.GetMazingerStoreInfo(DBManager.getInstance(), dbRoleInfo.RoleID, out data);
			return data;
		}

		
		public void InsertMazingerStoreInfo(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				MazingerStoreData data = DataHelper.BytesToObject<MazingerStoreData>(cmdParams, 0, count);
				if (data != null)
				{
					if (!DBWriter.InsertMazingerStoreInfo(data.RoleID, data.Type, data.Stage, data.StarLevel, data.Exp))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("插入魔神秘宝信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, data.RoleID), null, true);
						client.sendCmd<int>(nID, -1);
					}
				}
			}
			catch (Exception ex)
			{
				client.sendCmd<int>(nID, -1);
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<int>(nID, 1);
		}

		
		public void UpdateMazingerStoreInfo(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				MazingerStoreData data = DataHelper.BytesToObject<MazingerStoreData>(cmdParams, 0, count);
				if (data != null)
				{
					if (!DBWriter.UpdateMazingerStoreInfo(data.RoleID, data.Type, data.Stage, data.StarLevel, data.Exp))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新魔神秘宝信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, data.RoleID), null, true);
						client.sendCmd<int>(nID, -1);
					}
				}
			}
			catch (Exception ex)
			{
				client.sendCmd<int>(nID, -1);
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<int>(nID, 1);
		}
	}
}
