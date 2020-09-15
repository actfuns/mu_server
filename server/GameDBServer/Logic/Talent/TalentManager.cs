using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using GameDBServer.Tools;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Talent
{
	// Token: 0x02000179 RID: 377
	public class TalentManager
	{
		// Token: 0x060006A2 RID: 1698 RVA: 0x0003C7D4 File Offset: 0x0003A9D4
		public static TCPProcessCmdResults ProcTalentModify(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] fields = null;
			try
			{
				int length = 6;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out fields, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int totalCount = Convert.ToInt32(fields[1]);
				long exp = Convert.ToInt64(fields[2]);
				long expAdd = Convert.ToInt64(fields[3]);
				int isUp = Convert.ToInt32(fields[4]);
				int zoneID = Convert.ToInt32(fields[5]);
				string strcmd = TalentManager.TalentInfoModify(dbMgr, roleID, totalCount, exp, expAdd, isUp, zoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x0003C8BC File Offset: 0x0003AABC
		public static TCPProcessCmdResults ProcTalentEffectModify(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] fields = null;
			try
			{
				int length = 5;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out fields, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int talentType = Convert.ToInt32(fields[1]);
				int effectID = Convert.ToInt32(fields[2]);
				int effectLevel = Convert.ToInt32(fields[3]);
				int zoneID = Convert.ToInt32(fields[4]);
				string strcmd = TalentManager.TalentEffectModify(dbMgr, roleID, talentType, effectID, effectLevel, zoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x0003C998 File Offset: 0x0003AB98
		public static TCPProcessCmdResults ProcTalentEffectClear(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] fields = null;
			try
			{
				int length = 2;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out fields, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int zoneID = Convert.ToInt32(fields[1]);
				string strcmd = TalentManager.TalentEffectClear(dbMgr, roleID, zoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x0003CA4C File Offset: 0x0003AC4C
		public static string TalentInfoModify(DBManager dbMgr, int roleID, int totalCount, long exp, long expAdd, int isUp, int zoneID)
		{
			int result = 1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_talent(roleID, tatalCount, exp, zoneID) VALUES({0}, {1}, {2}, {3})", new object[]
				{
					roleID,
					totalCount,
					exp,
					zoneID
				});
				int count = conn.ExecuteNonQuery(cmdText, 0);
				if (count > 0)
				{
					result = 0;
					TalentManager.TalentLogAdd(dbMgr, zoneID, roleID, 1, expAdd);
					if (isUp > 0)
					{
						TalentManager.TalentLogAdd(dbMgr, zoneID, roleID, 2, 1L);
					}
					TalentManager.DbUpdateTalent(dbMgr, roleID, totalCount, exp);
				}
			}
			return result.ToString();
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x0003CB24 File Offset: 0x0003AD24
		public static void TalentLogAdd(DBManager dbMgr, int zoneID, int roleID, int logType, long logValue)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_talent_log (zoneID, roleID, logType, logValue, logTime) VALUES('{0}', '{1}', '{2}', '{3}', '{4}')", new object[]
				{
					zoneID,
					roleID,
					logType,
					logValue,
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
				});
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x0003CBB8 File Offset: 0x0003ADB8
		public static string TalentEffectModify(DBManager dbMgr, int roleID, int talentType, int effectID, int effectLevel, int zoneID)
		{
			int result = 1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_talent_effect(roleID, talentType, effectID, effectLevel, zoneID) VALUES({0}, {1}, {2}, {3}, {4})", new object[]
				{
					roleID,
					talentType,
					effectID,
					effectLevel,
					zoneID
				});
				int count = conn.ExecuteNonQuery(cmdText, 0);
				if (count > 0)
				{
					result = 0;
					TalentManager.DbUpdateTalentEffect(dbMgr, roleID, talentType, effectID, effectLevel);
				}
			}
			return result.ToString();
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x0003CC74 File Offset: 0x0003AE74
		public static string TalentEffectClear(DBManager dbMgr, int roleID, int zoneID)
		{
			int result = 1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE FROM t_talent_effect where roleID={0}", roleID);
				int count = conn.ExecuteNonQuery(cmdText, 0);
				if (count > 0)
				{
					TalentManager.TalentLogAdd(dbMgr, zoneID, roleID, 3, 1L);
					result = 0;
					TalentManager.DbTalentClear(dbMgr, roleID);
				}
			}
			return result.ToString();
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x0003CD00 File Offset: 0x0003AF00
		private static void DbUpdateTalent(DBManager dbMgr, int roleID, int totalCount, long exp)
		{
			DBRoleInfo dbRoleInfo = dbMgr.DBRoleMgr.FindDBRoleInfo(ref roleID);
			if (null != dbRoleInfo)
			{
				lock (dbRoleInfo)
				{
					dbRoleInfo.MyTalentData.TotalCount = totalCount;
					dbRoleInfo.MyTalentData.Exp = exp;
				}
			}
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x0003CD74 File Offset: 0x0003AF74
		private static void DbUpdateTalentEffect(DBManager dbMgr, int roleID, int talentType, int effectID, int effectLevel)
		{
			DBRoleInfo dbRoleInfo = dbMgr.DBRoleMgr.FindDBRoleInfo(ref roleID);
			if (null != dbRoleInfo)
			{
				lock (dbRoleInfo)
				{
					TalentEffectItem item = null;
					foreach (TalentEffectItem i in dbRoleInfo.MyTalentData.EffectList)
					{
						if (i.ID == effectID)
						{
							item = i;
							break;
						}
					}
					int add;
					if (item == null)
					{
						add = effectLevel;
						item = new TalentEffectItem
						{
							ID = effectID,
							Level = effectLevel,
							TalentType = talentType
						};
						dbRoleInfo.MyTalentData.EffectList.Add(item);
					}
					else
					{
						add = effectLevel - item.Level;
						item.Level = effectLevel;
					}
					Dictionary<int, int> countList;
					(countList = dbRoleInfo.MyTalentData.CountList)[talentType] = countList[talentType] + add;
				}
			}
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x0003CEC0 File Offset: 0x0003B0C0
		private static void DbTalentClear(DBManager dbMgr, int roleID)
		{
			DBRoleInfo dbRoleInfo = dbMgr.DBRoleMgr.FindDBRoleInfo(ref roleID);
			if (null != dbRoleInfo)
			{
				lock (dbRoleInfo)
				{
					dbRoleInfo.MyTalentData.CountList[1] = 0;
					dbRoleInfo.MyTalentData.CountList[2] = 0;
					dbRoleInfo.MyTalentData.CountList[3] = 0;
					dbRoleInfo.MyTalentData.EffectList = new List<TalentEffectItem>();
				}
			}
		}

		// Token: 0x0200017A RID: 378
		private enum TalentLogType
		{
			// Token: 0x040008AB RID: 2219
			Exp = 1,
			// Token: 0x040008AC RID: 2220
			Talent,
			// Token: 0x040008AD RID: 2221
			Wash
		}
	}
}
