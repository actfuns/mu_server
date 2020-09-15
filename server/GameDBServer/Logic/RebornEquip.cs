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
	// Token: 0x02000165 RID: 357
	public class RebornEquip : SingletonTemplate<RebornEquip>, IManager, ICmdProcessor
	{
		// Token: 0x0600061C RID: 1564 RVA: 0x00036BF4 File Offset: 0x00034DF4
		public bool initialize()
		{
			return true;
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x00036C08 File Offset: 0x00034E08
		public bool showdown()
		{
			return true;
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x00036C1C File Offset: 0x00034E1C
		public bool destroy()
		{
			return true;
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x00036C30 File Offset: 0x00034E30
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(14123, SingletonTemplate<RebornEquip>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14124, SingletonTemplate<RebornEquip>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14125, SingletonTemplate<RebornEquip>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14126, SingletonTemplate<RebornEquip>.Instance());
			return true;
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x00036C98 File Offset: 0x00034E98
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

		// Token: 0x06000621 RID: 1569 RVA: 0x00036EF4 File Offset: 0x000350F4
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

		// Token: 0x06000622 RID: 1570 RVA: 0x00037154 File Offset: 0x00035354
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

		// Token: 0x06000623 RID: 1571 RVA: 0x000373B0 File Offset: 0x000355B0
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

		// Token: 0x06000624 RID: 1572 RVA: 0x0003760C File Offset: 0x0003580C
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

		// Token: 0x06000625 RID: 1573 RVA: 0x00037674 File Offset: 0x00035874
		public static Dictionary<int, RebornEquipData> GetRebornEquipHoleData(DBRoleInfo dbRoleInfo)
		{
			Dictionary<int, RebornEquipData> data = null;
			DBQuery.GetAllRebornEquipHole(DBManager.getInstance(), dbRoleInfo.RoleID, out data);
			return data;
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x0003769C File Offset: 0x0003589C
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

		// Token: 0x06000627 RID: 1575 RVA: 0x0003774C File Offset: 0x0003594C
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

		// Token: 0x06000628 RID: 1576 RVA: 0x000377FC File Offset: 0x000359FC
		public static Dictionary<int, MazingerStoreData> GetMazingerStoreData(DBRoleInfo dbRoleInfo)
		{
			Dictionary<int, MazingerStoreData> data = null;
			DBQuery.GetMazingerStoreInfo(DBManager.getInstance(), dbRoleInfo.RoleID, out data);
			return data;
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x00037824 File Offset: 0x00035A24
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

		// Token: 0x0600062A RID: 1578 RVA: 0x000378D8 File Offset: 0x00035AD8
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
