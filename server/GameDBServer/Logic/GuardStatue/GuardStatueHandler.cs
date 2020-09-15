using System;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.GuardStatue
{
	// Token: 0x0200013F RID: 319
	public class GuardStatueHandler : SingletonTemplate<GuardStatueHandler>
	{
		// Token: 0x0600054D RID: 1357 RVA: 0x0002C7B4 File Offset: 0x0002A9B4
		private GuardStatueHandler()
		{
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0002C7C0 File Offset: 0x0002A9C0
		public TCPProcessCmdResults ProcUpdateRoleGuardStatue(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 7)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int slotCnt = Convert.ToInt32(fields[1]);
				int level = Convert.ToInt32(fields[2]);
				int suit = Convert.ToInt32(fields[3]);
				int totalGuardPoint = Convert.ToInt32(fields[4]);
				int lastdayRecoverPoint = Convert.ToInt32(fields[5]);
				int lastdayRecoverOffset = Convert.ToInt32(fields[6]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1001), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret;
				if (this._UpdateRoleGuardStatue(dbMgr, roleID, slotCnt, level, suit, totalGuardPoint, lastdayRecoverPoint, lastdayRecoverOffset))
				{
					lock (roleInfo)
					{
						if (roleInfo.MyGuardStatueDetail == null)
						{
							roleInfo.MyGuardStatueDetail = new GuardStatueDetail();
							roleInfo.MyGuardStatueDetail.IsActived = true;
						}
						roleInfo.MyGuardStatueDetail.GuardStatue.Level = level;
						roleInfo.MyGuardStatueDetail.GuardStatue.Suit = suit;
						roleInfo.MyGuardStatueDetail.GuardStatue.HasGuardPoint = totalGuardPoint;
						roleInfo.MyGuardStatueDetail.LastdayRecoverPoint = lastdayRecoverPoint;
						roleInfo.MyGuardStatueDetail.LastdayRecoverOffset = lastdayRecoverOffset;
						roleInfo.MyGuardStatueDetail.ActiveSoulSlot = slotCnt;
					}
					ret = 1;
				}
				else
				{
					ret = -1;
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", ret), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x0002CA8C File Offset: 0x0002AC8C
		private bool _UpdateRoleGuardStatue(DBManager dbMgr, int roleID, int slotCnt, int level, int suit, int totalGuardPoint, int lastdayRecoverPoint, int lastdayRecoverOffset)
		{
			bool bSuccess = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_guard_statue(roleid,slot_cnt,level,suit,total_guard_point,lastday_recover_point,lastday_recover_offset) VALUES({0},{1},{2},{3},{4},{5},{6})", new object[]
				{
					roleID,
					slotCnt,
					level,
					suit,
					totalGuardPoint,
					lastdayRecoverPoint,
					lastdayRecoverOffset
				});
				if (conn.ExecuteNonQuery(cmdText, 0) > 0)
				{
					bSuccess = true;
				}
			}
			return bSuccess;
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x0002CB70 File Offset: 0x0002AD70
		public TCPProcessCmdResults ProcUpdateRoleGuardSoul(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int soulType = Convert.ToInt32(fields[1]);
				int equipSlot = Convert.ToInt32(fields[2]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1001), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret;
				if (this._UpdateRoleGuardSoul(dbMgr, roleID, soulType, equipSlot))
				{
					lock (roleInfo)
					{
						if (roleInfo.MyGuardStatueDetail == null)
						{
							roleInfo.MyGuardStatueDetail = new GuardStatueDetail();
							roleInfo.MyGuardStatueDetail.IsActived = true;
						}
						roleInfo.MyGuardStatueDetail.GuardStatue.GuardSoulList.RemoveAll((GuardSoulData soul) => soul.Type == soulType);
						roleInfo.MyGuardStatueDetail.GuardStatue.GuardSoulList.Add(new GuardSoulData
						{
							Type = soulType,
							EquipSlot = equipSlot
						});
					}
					ret = 1;
				}
				else
				{
					ret = 1;
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", ret), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x0002CE20 File Offset: 0x0002B020
		private bool _UpdateRoleGuardSoul(DBManager dbMgr, int roleID, int soulType, int equipSlot)
		{
			bool bSuccess = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_guard_soul(roleid,soul_type,equip_slot) VALUES({0},{1},{2});", roleID, soulType, equipSlot);
				if (conn.ExecuteNonQuery(cmdText, 0) > 0)
				{
					bSuccess = true;
				}
			}
			return bSuccess;
		}

		// Token: 0x0400080A RID: 2058
		public const string TableGuardStatue = "t_guard_statue";

		// Token: 0x0400080B RID: 2059
		public const string TableGuardSoul = "t_guard_soul";
	}
}
