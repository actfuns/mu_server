﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Core;
using GameDBServer.Data;
using GameDBServer.Logic;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB
{
	// Token: 0x020001A1 RID: 417
	public class DBWriter
	{
		// Token: 0x170000D0 RID: 208
		
		public static string CurrentGoodsBakTableName
		{
			get
			{
				return DBWriter.GoodsBakTableNames[DBWriter.GoodsBakTableIndex % DBWriter.GoodsBakTableNames.Length];
			}
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x00047DB8 File Offset: 0x00045FB8
		public static int ExecuteSQLNoQuery(DBManager dbMgr, string sqlText, MySQLConnection conn = null)
		{
			int result = 0;
			bool keepConn = true;
			try
			{
				if (conn == null)
				{
					keepConn = false;
					conn = dbMgr.DBConns.PopDBConnection();
				}
				MySQLCommand mySQLCommand;
				MySQLCommand cmd = mySQLCommand = new MySQLCommand(sqlText, conn);
				try
				{
					cmd.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
					return -1;
				}
				finally
				{
					if (mySQLCommand != null)
					{
						((IDisposable)mySQLCommand).Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return -2;
			}
			finally
			{
				if (!keepConn && null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return result;
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x00047EA8 File Offset: 0x000460A8
		public static bool CheckRoleCountFull(DBManager dbMgr)
		{
			bool bFull = true;
			MySQLConnection conn = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				MySQLCommand cmd = new MySQLCommand("SELECT max(rid) AS LastID from t_roles", conn);
				MySQLDataReader reader = cmd.ExecuteReaderEx();
				int nCount = 0;
				while (reader.Read())
				{
					nCount = Global.SafeConvertToInt32(reader[0].ToString(), 10);
				}
				if (null != cmd)
				{
					cmd.Dispose();
				}
				if (nCount % GameDBManager.DBAutoIncreaseStepValue >= GameDBManager.DBAutoIncreaseStepValue - 500)
				{
					int extId = GameDBManager.GameConfigMgr.GetGameConfigItemInt("role_ext_auto_increment", 0);
					bFull = (extId < 1500000000 || nCount >= extId || 0 != DBWriter.ChangeTablesAutoIncrementValue(dbMgr, "t_roles", extId));
				}
				else
				{
					bFull = false;
				}
			}
			catch (MySQLException)
			{
				bFull = true;
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return bFull;
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x00047FDC File Offset: 0x000461DC
		public static int CreateRole(DBManager dbMgr, string userID, string userName, int sex, int occup, string roleName, int zoneID, int bagnum, int isflashplayer, int nMagicSwordParam, int rebornbagnum)
		{
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			string mapPos = "-1:0:-1:-1";
			int roleID = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_roles (userid, rname, sex, occupation, position, regtime, lasttime, biguantime, zoneid, bhname, chenghao, username, bagnum, isflashplayer, magic_sword_param, reborn_bagnum) VALUES('{0}', '{1}', {2}, {3}, '{4}', '{5}', '{6}', '{7}', {8}, '', '', '{9}', {10}, {11}, {12}, {13})", new object[]
				{
					userID,
					roleName,
					sex,
					occup,
					mapPos,
					today,
					today,
					today,
					zoneID,
					userName,
					bagnum,
					isflashplayer,
					nMagicSwordParam,
					rebornbagnum
				});
				if (!conn.ExecuteNonQueryBool(cmdText, 0))
				{
					return roleID;
				}
				try
				{
					roleID = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
				}
				catch (MySQLException)
				{
					roleID = -2;
				}
			}
			return roleID;
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x00048108 File Offset: 0x00046308
		public static bool UnPreRemoveRole(DBManager dbMgr, int roleID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET predeltime=NULL WHERE rid={0} and predeltime IS NOT NULL", roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x00048168 File Offset: 0x00046368
		public static bool PreRemoveRole(DBManager dbMgr, int roleID, DateTime Now)
		{
			bool ret = false;
			string today = Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET predeltime='{0}' WHERE rid={1} and predeltime IS NULL", today, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x000481D8 File Offset: 0x000463D8
		public static bool RemoveRole(DBManager dbMgr, int roleID)
		{
			bool ret = false;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET isdel=1, deltime='{0}', predeltime=NULL WHERE rid={1}", today, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x0004824C File Offset: 0x0004644C
		public static bool RemoveRoleByName(DBManager dbMgr, string roleName)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET isdel=1 WHERE rname='{0}'", roleName);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007D0 RID: 2000 RVA: 0x000482A4 File Offset: 0x000464A4
		public static bool UnRemoveRole(DBManager dbMgr, string roleName)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET isdel=0 WHERE rname='{0}'", roleName);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x000482FC File Offset: 0x000464FC
		public static bool UnRemoveRoleByID(DBManager dbMgr, int rid)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET isdel=0 WHERE rid='{0}'", rid);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x0004835C File Offset: 0x0004655C
		public static bool UpdateRoleLoginInfo(DBManager dbMgr, int roleID, int loginNum, int loginDayID, int loginDayNum, string userid, int zoneid, string ip)
		{
			bool ret = false;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET lasttime='{0}', loginnum={1}, logindayid={2}, logindaynum={3} WHERE rid={4}", new object[]
				{
					today,
					loginNum,
					loginDayID,
					loginDayNum,
					roleID
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
				DateTime now = DateTime.Now;
				cmdText = string.Format("INSERT INTO t_login (userid,dayid,rid,logintime,logouttime,ip,mac,zoneid,onlinesecs,loginnum) VALUES('{0}',{1},{2},'{3}','{4}','{5}','{6}',{7},0,1) ON DUPLICATE KEY UPDATE loginnum=loginnum+1,rid={2}", new object[]
				{
					userid,
					Global.GetOffsetDay(now),
					roleID,
					today,
					Global.GetDayEndTime(now),
					ip,
					null,
					zoneid
				});
				conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x00048468 File Offset: 0x00046668
		public static bool UpdateRoleLogOff(DBManager dbMgr, int roleID, string userid, int zoneid, string ip, int onlineSecs)
		{
			bool ret = false;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET logofftime='{0}' WHERE rid={1}", today, roleID);
				conn.ExecuteNonQuery(cmdText, 0);
				DateTime now = DateTime.Now;
				cmdText = string.Format("INSERT INTO t_login (userid,dayid,rid,logintime,logouttime,ip,mac,zoneid,onlinesecs,loginnum) VALUES('{0}',{1},{2},'{3}','{4}','{5}','{6}',{7},{8},1) ON DUPLICATE KEY UPDATE logouttime='{4}',onlinesecs=LEAST(onlineSecs+{8},86400);", new object[]
				{
					userid,
					Global.GetOffsetDay(now),
					roleID,
					Global.GetDayStartTime(now),
					today,
					ip,
					null,
					zoneid,
					onlineSecs
				});
				conn.ExecuteNonQuery(cmdText, 0);
				ret = true;
			}
			return ret;
		}

		// Token: 0x060007D4 RID: 2004 RVA: 0x00048550 File Offset: 0x00046750
		public static bool UpdateRoleOnlineSecs(DBManager dbMgr, int roleID, int totalOnlineSecs, int antiAddictionSecs)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET totalonlinesecs={0}, antiaddictionsecs={1} WHERE rid={2}", totalOnlineSecs, antiAddictionSecs, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x000485BC File Offset: 0x000467BC
		public static bool UpdateRoleBiGuanTime(DBManager dbMgr, int roleID, long biguanTime)
		{
			bool ret = false;
			string today = new DateTime(biguanTime * 10000L).ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET biguantime='{0}' WHERE rid={1}", today, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x00048638 File Offset: 0x00046838
		public static bool UpdateRoleBattleNameInfo(DBManager dbMgr, int roleID, long startTime, int nameIndex)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET battlenamestart={0}, battlenameindex={1} WHERE rid={2}", startTime, nameIndex, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x000486A4 File Offset: 0x000468A4
		public static bool UpdateRoleCZTaskID(DBManager dbMgr, int roleID, int czTaskID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET cztaskid={0} WHERE rid={1}", czTaskID, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x00048708 File Offset: 0x00046908
		public static bool UpdateRoleOnceAwardFlag(DBManager dbMgr, int roleID, long onceawardflag)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET onceawardflag={0} WHERE rid={1}", onceawardflag, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x0004876C File Offset: 0x0004696C
		public static bool UpdateRoleBanProps(DBManager dbMgr, int roleID, string colName, long value)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET {0}={1} WHERE rid={2}", colName, value, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007DA RID: 2010 RVA: 0x000487D0 File Offset: 0x000469D0
		public static bool UpdateRoleBangHuiInfo(DBManager dbMgr, int roleID, int faction, string bhName, int bhZhiWu)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET faction={0}, bhname='{1}', bhzhiwu={2} WHERE rid={3}", new object[]
				{
					faction,
					bhName,
					bhZhiWu,
					roleID
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007DB RID: 2011 RVA: 0x00048858 File Offset: 0x00046A58
		public static bool UpdateRoleJunTuanInfo(DBManager dbMgr, int roleID, int junTuanZhiWu)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET juntuanzhiwu={1} WHERE rid={0}", roleID, junTuanZhiWu);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007DC RID: 2012 RVA: 0x000488BC File Offset: 0x00046ABC
		public static bool UpdateAllRoleBangHuiName(DBManager dbMgr, int bhid, string newName)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET bhname='{0}' WHERE faction={1}", newName, bhid);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007DD RID: 2013 RVA: 0x0004891C File Offset: 0x00046B1C
		public static bool ClearAllRoleBangHuiInfo(DBManager dbMgr, int bhid)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET faction=0, bhname='', bhzhiwu=0, chenghao='' WHERE faction={0}", bhid);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007DE RID: 2014 RVA: 0x0004897C File Offset: 0x00046B7C
		public static bool ClearLastBangHuiInfoByRoleID(DBManager dbMgr, int roleID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET chenghao='' WHERE rid={0}", roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007DF RID: 2015 RVA: 0x000489DC File Offset: 0x00046BDC
		public static bool UpdateRoleBangHuiVerify(DBManager dbMgr, int roleID, int toVerify)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET bhverify={0} WHERE rid={1}", toVerify, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x00048A40 File Offset: 0x00046C40
		public static bool UpdateBHMatchSupportFlagData(DBManager dbMgr, BHMatchSupportData cmdData)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_banghui_match_support_flag (season, round, rid, bhid1, bhid2, guess, is_award, time)\r\n                                            VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, NOW()) ON DUPLICATE KEY UPDATE is_award={6}, time=NOW();", new object[]
				{
					cmdData.season,
					cmdData.round,
					cmdData.rid,
					cmdData.bhid1,
					cmdData.bhid2,
					cmdData.guess,
					cmdData.isaward
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x00048B0C File Offset: 0x00046D0C
		public static int NewTask(DBManager dbMgr, int roleID, int npcID, int taskID, string addtime, int focus, int nStarLevel)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_tasks (taskid, rid, value1, value2, focus, isdel, addtime, starlevel) VALUES({0}, {1}, {2}, {3}, {4}, {5}, '{6}', {7})", new object[]
				{
					taskID,
					roleID,
					0,
					0,
					focus,
					0,
					addtime,
					nStarLevel
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
				if (ret < 0)
				{
					return ret;
				}
				ret = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
			}
			return ret;
		}

		// Token: 0x060007E2 RID: 2018 RVA: 0x00048BE0 File Offset: 0x00046DE0
		public static bool UpdateRolePosition(DBManager dbMgr, int roleID, string position)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET position='{0}' WHERE rid={1}", position, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x00048C40 File Offset: 0x00046E40
		public static bool UpdateRoleExpLevel(DBManager dbMgr, int roleID, int level, long experience)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET level={0}, experience={1} WHERE rid={2}", level, experience, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x00048CAC File Offset: 0x00046EAC
		public static bool UpdateRoleInterPower(DBManager dbMgr, int roleID, int interPower)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET interpower={0} WHERE rid={1}", interPower, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x00048D10 File Offset: 0x00046F10
		public static bool UpdateRoleMoney1(DBManager dbMgr, int roleID, int money)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET money1={0} WHERE rid={1}", money, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x00048D74 File Offset: 0x00046F74
		public static bool UpdateRoleYinLiang(DBManager dbMgr, int roleID, int yinLiang)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET yinliang={0} WHERE rid={1}", yinLiang, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x00048DD8 File Offset: 0x00046FD8
		public static bool UpdateRoleGold(DBManager dbMgr, int roleID, int gold)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET money2={0} WHERE rid={1}", gold, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x00048E3C File Offset: 0x0004703C
		public static bool UpdateRoleStoreYinLiang(DBManager dbMgr, int roleID, long yinLiang)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET store_yinliang={0} WHERE rid={1}", yinLiang, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007E9 RID: 2025 RVA: 0x00048EA0 File Offset: 0x000470A0
		public static bool UpdateRoleStoreMoney(DBManager dbMgr, int roleID, long money)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET store_money={0} WHERE rid={1}", money, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007EA RID: 2026 RVA: 0x00048F04 File Offset: 0x00047104
		public static bool UpdateRoleOccupationBranch(DBManager dbMgr, int nRoleID, int nMagicSwordParam)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET magic_sword_param={0} WHERE rid={1}", nMagicSwordParam, nRoleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x00048F68 File Offset: 0x00047168
		public static bool UpdateRoleBangGong(DBManager dbMgr, int roleID, int bgDayID1, int bgMoney, int bgDayID2, int bgGoods, int bangGong)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET bgdayid1={0}, bgmoney={1}, bgdayid2={2}, bggoods={3}, banggong={4} WHERE rid={5}", new object[]
				{
					bgDayID1,
					bgMoney,
					bgDayID2,
					bgGoods,
					bangGong,
					roleID
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x00049008 File Offset: 0x00047208
		public static bool UpdateUserInfo(DBManager dbMgr, DBUserInfo dbUserInfo)
		{
			string userID = dbUserInfo.UserID;
			long userMoney = (long)dbUserInfo.Money;
			long realMoney = (long)dbUserInfo.RealMoney;
			long giftID = (long)dbUserInfo.GiftID;
			long giftJiFen = (long)dbUserInfo.GiftJiFen;
			string cc = Global.GCC(3, new object[]
			{
				userID,
				userMoney,
				realMoney
			});
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_money (userid,money,realmoney,giftid,giftjifen,cc) VALUES('{0}',{1},{2},{3},{4},'{5}') ON DUPLICATE KEY UPDATE money={1},realmoney={2},giftid={3},giftjifen={4},cc='{5}'", new object[]
				{
					userID,
					userMoney,
					realMoney,
					giftID,
					giftJiFen,
					cc
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x00049104 File Offset: 0x00047304
		public static bool UpdateUserSpecJiFen(DBManager dbMgr, string userID, int specjifen)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_money set specjifen={1} where userid='{0}'", userID, specjifen);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x00049168 File Offset: 0x00047368
		public static bool UpdateUserEveryJiFen(DBManager dbMgr, string userID, int specjifen)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_money set everyjifen={1} where userid='{0}'", userID, specjifen);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x000491CC File Offset: 0x000473CC
		public static bool UpdateUserInputPoints(DBManager dbMgr, string userID, int ipoints)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_money set points={1} where userid='{0}'", userID, ipoints);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x00049230 File Offset: 0x00047430
		public static bool UpdateUserGiftJiFen(DBManager dbMgr, string userID, int giftJiFen)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_money set giftjifen={1} where userid='{0}'", userID, giftJiFen);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x00049294 File Offset: 0x00047494
		public static bool UpdateRoleDefSkillID(DBManager dbMgr, int roleID, int defSkillID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET skillid={0} WHERE rid={1}", defSkillID, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x000492F8 File Offset: 0x000474F8
		public static bool UpdateRoleJieBiaoInfo(DBManager dbMgr, int roleID, int jieBiaoDayID, int jieBiaoDayNum)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET jiebiaodayid={0}, jiebiaonum={1} WHERE rid={2}", jieBiaoDayID, jieBiaoDayNum, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x00049364 File Offset: 0x00047564
		public static bool UpdateRoleAutoDrink(DBManager dbMgr, int roleID, int autoLifeV, int autoMagicV)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET autolife={0}, automagic={1} WHERE rid={2}", autoLifeV, autoMagicV, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x000493D0 File Offset: 0x000475D0
		public static int MoveGoods(DBManager dbMgr, int roleID, int goodsDbID, int oldRid, int site = 0)
		{
			int ret = -10;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_goods SET rid={0}, site={3} WHERE Id={1} and rid={2}", new object[]
				{
					roleID,
					goodsDbID,
					oldRid,
					site
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x0004945C File Offset: 0x0004765C
		public static int NewGoods(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int quality, string props, int forgeLevel, int binding, int site, string jewelList, int bagindex, string startTime, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				try
				{
					string cmdText = string.Format("INSERT INTO t_goods (rid, goodsid, quality, Props, gcount, forge_level, binding, site, jewellist, bagindex, starttime, endtime, addpropindex, bornindex, lucky, strong, excellenceinfo, appendproplev, equipchangelife, ehinfo) VALUES({0}, {1}, {2}, '{3}', {4}, {5}, {6}, {7}, '{8}', {9}, '{10}', '{11}', {12}, {13}, {14}, {15}, {16}, {17}, {18}, '')", new object[]
					{
						roleID,
						goodsID,
						quality,
						props,
						goodsNum,
						forgeLevel,
						binding,
						site,
						jewelList,
						bagindex,
						startTime,
						endTime,
						addPropIndex,
						bornIndex,
						lucky,
						strong,
						ExcellenceProperty,
						nAppendPropLev,
						nEquipChangeLife
					});
					ret = conn.ExecuteNonQuery(cmdText, 0);
					if (ret < 0)
					{
						return ret;
					}
					ret = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
				}
				catch (MySQLException)
				{
					ret = -2;
				}
			}
			return ret;
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x000495D8 File Offset: 0x000477D8
		public static string FormatUpdateSQL(int id, string[] fields, int startIndex, string[] fieldNames, string tableName, byte[] fieldTypes, string idName = "Id")
		{
			StringBuilder sb = new StringBuilder(256);
			sb.Append("UPDATE ").Append(tableName).Append(" SET ");
			for (int i = 0; i < fieldNames.Count<string>(); i++)
			{
				if (!(fields[startIndex + i] == "*"))
				{
					if (fieldTypes[i] == 0)
					{
						sb.AppendFormat("{0}={1}", fieldNames[i], fields[startIndex + i]).Append(',');
					}
					else if (fieldTypes[i] == 1)
					{
						sb.AppendFormat("{0}='{1}'", fieldNames[i], fields[startIndex + i]).Append(',');
					}
					else if (fieldTypes[i] == 2)
					{
						sb.AppendFormat("{0}={1}+{2}", fieldNames[i], fieldNames[i], fields[startIndex + i]).Append(',');
					}
					else if (fieldTypes[i] == 3)
					{
						sb.AppendFormat("{0}='{1}'", fieldNames[i], fields[startIndex + i].Replace('$', ':')).Append(',');
					}
				}
			}
			sb[sb.Length - 1] = ' ';
			sb.AppendFormat(" WHERE {0}={1}", idName, id);
			if (sb.Length > 100)
			{
			}
			return sb.ToString();
		}

		// Token: 0x060007F7 RID: 2039 RVA: 0x0004974C File Offset: 0x0004794C
		public static int UpdateGoods(DBManager dbMgr, int id, string[] fields, int startIndex)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = DBWriter.FormatUpdateSQL(id, fields, startIndex, DBWriter._UpdateGoods_fieldNames, "t_goods", DBWriter._UpdateGoods_fieldTypes, "Id");
				cmdText += string.Format(" and rid={0}", fields[0]);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x000497CC File Offset: 0x000479CC
		public static int RemoveGoods(DBManager dbMgr, int id)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_goods SET gcount=0 WHERE Id={0}", id);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x0004982C File Offset: 0x00047A2C
		public static int MoveGoodsDataToBackupTable(DBManager dbMgr, int id)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO {1} SELECT *,0,NOW(),0 FROM t_goods WHERE Id={0}", id, DBWriter.CurrentGoodsBakTableName);
				conn.ExecuteNonQuery(cmdText, 0);
				cmdText = string.Format("DELETE FROM t_goods WHERE Id={0}", id);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x000498A8 File Offset: 0x00047AA8
		public static int SwitchGoodsBackupTable(DBManager dbMgr)
		{
			MySQLConnection conn = null;
			try
			{
				lock (DBWriter.GoodsBakTableMutex)
				{
					conn = dbMgr.DBConns.PopDBConnection();
					int needIndex = TimeUtil.NowDateTime().Month % DBWriter.GoodsBakTableNames.Length;
					if (DBWriter.GoodsBakTableIndex < 0)
					{
						DBWriter.GoodsBakTableIndex = needIndex;
					}
					int currentIndex = DBWriter.GoodsBakTableIndex % DBWriter.GoodsBakTableNames.Length;
					if (needIndex != currentIndex)
					{
						string needTableName = DBWriter.GoodsBakTableNames[needIndex];
						string sqlText = string.Format("SELECT id FROM {0} limit 1;", needTableName);
						int result = DBWriter.ExecuteSQLReadInt(dbMgr, sqlText, conn);
						if (result > 0)
						{
							sqlText = string.Format("TRUNCATE TABLE {0};", needTableName);
							result = DBWriter.ExecuteSQLNoQuery(dbMgr, sqlText, conn);
							if (result < 0)
							{
								LogManager.WriteLog(LogTypes.Error, "阶段物品备份表失败，不切换数据库", null, true);
							}
						}
						else
						{
							DBWriter.GoodsBakTableIndex = needIndex;
						}
					}
				}
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return 0;
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x000499F0 File Offset: 0x00047BF0
		public static bool WirterAutoCompletionTaskByTaskID(DBManager dbMgr, int roleID, List<int> taskIDList)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				for (int i = 1; i < taskIDList.Count; i++)
				{
					string cmdText = string.Format("INSERT INTO t_taskslog (taskid, rid, count) VALUES({0}, {1}, 1) ON DUPLICATE KEY UPDATE count=count+1", taskIDList[i], roleID);
					conn.ExecuteNonQuery(cmdText, 0);
				}
				ret = true;
			}
			return ret;
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x00049A78 File Offset: 0x00047C78
		public static int UpdateTask(DBManager dbMgr, int dbID, string[] fields, int startIndex)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = DBWriter.FormatUpdateSQL(dbID, fields, startIndex, DBWriter._UpdateTask_fieldNames, "t_tasks", DBWriter._UpdateTask_fieldTypes, "Id");
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007FD RID: 2045 RVA: 0x00049AE4 File Offset: 0x00047CE4
		public static bool CompleteTask(DBManager dbMgr, int roleID, int npcID, int taskID, int dbID, int isMainTask)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_taskslog (taskid, rid, count) VALUES({0}, {1}, 1) ON DUPLICATE KEY UPDATE count=count+1", taskID, roleID);
				if (isMainTask == 0)
				{
					conn.ExecuteNonQueryBool(cmdText, 0);
				}
				cmdText = string.Format("delete from t_tasks WHERE Id={0}", dbID);
				conn.ExecuteNonQueryBool(cmdText, 0);
				ret = true;
			}
			return ret;
		}

		// Token: 0x060007FE RID: 2046 RVA: 0x00049B74 File Offset: 0x00047D74
		public static bool DeleteTask(DBManager dbMgr, int roleID, int taskID, int dbID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_tasks SET isdel=1 WHERE Id={0}", dbID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060007FF RID: 2047 RVA: 0x00049BD4 File Offset: 0x00047DD4
		public static bool GMSetTask(DBManager dbMgr, int roleID, int taskID, List<int> taskIDList)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				for (int i = 1; i < taskIDList.Count - 1; i++)
				{
					string cmdText = string.Format("INSERT INTO t_taskslog (taskid, rid, count) VALUES({0}, {1}, 1) ON DUPLICATE KEY UPDATE count=count+1", taskIDList[i], roleID);
					conn.ExecuteNonQuery(cmdText, 0);
				}
				ret = true;
			}
			return ret;
		}

		// Token: 0x06000800 RID: 2048 RVA: 0x00049C5C File Offset: 0x00047E5C
		public static int AddFriend(DBManager dbMgr, int dbID, int roleID, int otherID, int friendType)
		{
			int ret = dbID;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				try
				{
					string cmdText = string.Format("INSERT INTO t_friends (myid, otherid, friendType) VALUES({0}, {1}, {2}) ON DUPLICATE KEY UPDATE friendType={3}", new object[]
					{
						roleID,
						otherID,
						friendType,
						friendType
					});
					if (conn.ExecuteNonQueryBool(cmdText, 0) && dbID < 0)
					{
						ret = conn.GetSingleInt(string.Format("SELECT Id FROM t_friends where myid={0} and otherid={1}", roleID, otherID), 0, new MySQLParameter[0]);
					}
				}
				catch (MySQLException)
				{
					ret = -2;
				}
			}
			return ret;
		}

		// Token: 0x06000801 RID: 2049 RVA: 0x00049D44 File Offset: 0x00047F44
		public static bool RemoveFriend(DBManager dbMgr, int dbID, int roleID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE FROM t_friends WHERE Id={0}", dbID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000802 RID: 2050 RVA: 0x00049DA4 File Offset: 0x00047FA4
		public static bool UpdatePKMode(DBManager dbMgr, int roleID, int pkMode)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET pkmode={0} WHERE rid={1}", pkMode, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000803 RID: 2051 RVA: 0x00049E08 File Offset: 0x00048008
		public static bool UpdatePKValues(DBManager dbMgr, int roleID, int pkValue, int pkPoint)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET pkvalue={0}, pkpoint={1} WHERE rid={2}", pkValue, pkPoint, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000804 RID: 2052 RVA: 0x00049E74 File Offset: 0x00048074
		public static bool UpdateLianZhan(DBManager dbMgr, int roleID, int lianzhan)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET lianzhan={0} WHERE rid={1}", lianzhan, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000805 RID: 2053 RVA: 0x00049ED8 File Offset: 0x000480D8
		public static bool UpdateKillBoss(DBManager dbMgr, int roleID, int killBoss)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET killboss={0} WHERE rid={1}", killBoss, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x00049F3C File Offset: 0x0004813C
		public static bool UpdateBattleNum(DBManager dbMgr, int roleID, int battleNum)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET battlenum={0} WHERE rid={1}", battleNum, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x00049FA0 File Offset: 0x000481A0
		public static bool UpdateHeroIndex(DBManager dbMgr, int roleID, int heroIndex)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET heroindex={0} WHERE rid={1}", heroIndex, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000808 RID: 2056 RVA: 0x0004A004 File Offset: 0x00048204
		public static bool UpdateRoleStat(DBManager dbMgr, int roleID, int equipJiFen, int xueWeiNum, int skillLearnedNum, int horseJiFen)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET equipjifen={0}, xueweinum={1}, skilllearnednum={2}, horsejifen={3} WHERE rid={4}", new object[]
				{
					equipJiFen,
					xueWeiNum,
					skillLearnedNum,
					horseJiFen,
					roleID
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x0004A09C File Offset: 0x0004829C
		public static bool UpdateRoleKeys(DBManager dbMgr, int roleID, int type, string keys)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText;
				if (0 == type)
				{
					cmdText = string.Format("UPDATE t_roles SET main_quick_keys='{0}' WHERE rid={1}", keys, roleID);
				}
				else
				{
					cmdText = string.Format("UPDATE t_roles SET other_quick_keys='{0}' WHERE rid={1}", keys, roleID);
				}
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x0004A124 File Offset: 0x00048324
		public static bool UpdateRoleLeftFightSecs(DBManager dbMgr, int roleID, int leftFightSecs)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET leftfightsecs={0} WHERE rid={1}", leftFightSecs, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x0004A188 File Offset: 0x00048388
		public static int NewHorse(DBManager dbMgr, int roleID, int horseID, int bodyID, string addtime)
		{
			int ret = -1;
			string defVal = "0,0,0,0,0,0,0,0,0";
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				try
				{
					string cmdText = string.Format("INSERT INTO t_horses (rid, horseid, bodyid, propsNum, PropsVal, addtime, isdel, failednum) VALUES({0}, {1}, {2}, '{3}', '{4}', '{5}', {6}, {7})", new object[]
					{
						roleID,
						horseID,
						bodyID,
						defVal,
						defVal,
						addtime,
						0,
						0
					});
					ret = conn.ExecuteNonQuery(cmdText, 0);
					if (ret < 0)
					{
						return ret;
					}
					ret = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
				}
				catch (MySQLException)
				{
					ret = -2;
				}
			}
			return ret;
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x0004A274 File Offset: 0x00048474
		public static int NewPet(DBManager dbMgr, int roleID, int petID, string petName, int petType, string props, string addtime)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				try
				{
					string cmdText = string.Format("INSERT INTO t_pets (rid, petid, petname, pettype, feednum, realivenum, addtime, props, isdel) VALUES({0}, {1}, '{2}', {3}, {4}, {5}, '{6}', '{7}', {8})", new object[]
					{
						roleID,
						petID,
						petName,
						petType,
						0,
						0,
						addtime,
						props,
						0
					});
					ret = conn.ExecuteNonQuery(cmdText, 0);
					if (ret < 0)
					{
						return ret;
					}
					ret = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
				}
				catch (MySQLException)
				{
					ret = -2;
				}
			}
			return ret;
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x0004A374 File Offset: 0x00048574
		public static int UpdateHorse(DBManager dbMgr, int id, string[] fields, int startIndex)
		{
			int ret = -1;
			string[] fieldNames = new string[]
			{
				"isdel",
				"horseid",
				"bodyid",
				"propsNum",
				"PropsVal",
				"failednum",
				"temptime",
				"tempnum",
				"faileddayid"
			};
			byte[] fieldTypes = new byte[]
			{
				0,
				0,
				0,
				1,
				1,
				0,
				3,
				0,
				0
			};
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = DBWriter.FormatUpdateSQL(id, fields, startIndex, fieldNames, "t_horses", fieldTypes, "Id");
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x0004A44C File Offset: 0x0004864C
		public static bool UpdateRoleHorse(DBManager dbMgr, int roleID, int horseDbID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText;
				if (horseDbID > 0)
				{
					cmdText = string.Format("UPDATE t_roles SET horseid={0}, lasthorseid={1} WHERE rid={2}", horseDbID, horseDbID, roleID);
				}
				else
				{
					cmdText = string.Format("UPDATE t_roles SET horseid={0} WHERE rid={1}", horseDbID, roleID);
				}
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x0004A4E4 File Offset: 0x000486E4
		public static int UpdatePet(DBManager dbMgr, int id, string[] fields, int startIndex)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = DBWriter.FormatUpdateSQL(id, fields, startIndex, DBWriter._UpdatePet_fieldNames, "t_pets", DBWriter._UpdatePet_fieldTypes, "Id");
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x0004A550 File Offset: 0x00048750
		public static bool UpdateRolePet(DBManager dbMgr, int roleID, int petDbID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET petid={0} WHERE rid={1}", petDbID, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x0004A5B4 File Offset: 0x000487B4
		public static int AddRoleDJPoint(DBManager dbMgr, int dbID, int roleID, int djPoint)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				try
				{
					bool insert = false;
					string cmdText;
					if (dbID <= 0)
					{
						insert = true;
						cmdText = string.Format("INSERT INTO t_djpoints (rid, djpoint, total, wincnt) VALUES({0}, {1}, {2}, {3})", new object[]
						{
							roleID,
							djPoint,
							1,
							(djPoint > 0) ? 1 : 0
						});
					}
					else
					{
						cmdText = string.Format("UPDATE t_djpoints SET djpoint=djpoint+{0}, total=total+1, wincnt=wincnt+{1} WHERE Id={2}", djPoint, (djPoint > 0) ? 1 : 0, dbID);
					}
					ret = conn.ExecuteNonQuery(cmdText, 0);
					if (ret >= 0)
					{
						if (insert)
						{
							ret = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
						}
						else
						{
							ret = dbID;
						}
					}
				}
				catch (MySQLException)
				{
					ret = -2;
				}
			}
			return ret;
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x0004A6D0 File Offset: 0x000488D0
		public static int UpRoleJingMai(DBManager dbMgr, int roleID, int dbID, int jingMaiBodyLevel, int jingMaiID, int jingMaiLevel)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				try
				{
					bool insert = false;
					string cmdText;
					if (dbID <= 0)
					{
						insert = true;
						cmdText = string.Format("INSERT INTO t_jingmai (rid, jmid, jmlevel, bodylevel) VALUES({0}, {1}, {2}, {3})", new object[]
						{
							roleID,
							jingMaiID,
							jingMaiLevel,
							jingMaiBodyLevel
						});
					}
					else
					{
						cmdText = string.Format("UPDATE t_jingmai SET jmlevel={0} WHERE Id={1}", jingMaiLevel, dbID);
					}
					ret = conn.ExecuteNonQuery(cmdText, 0);
					if (ret >= 0)
					{
						if (insert)
						{
							ret = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
						}
						else
						{
							ret = dbID;
						}
					}
				}
				catch (MySQLException)
				{
					ret = -2;
				}
			}
			return ret;
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x0004A7DC File Offset: 0x000489DC
		public static int NewBulletinText(DBManager dbMgr, string msgID, string fromDate, string toDate, int interval, string bulletinText)
		{
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_bulletin (msgid, intervals, bulletintext, fromdate, todate, opttime) VALUES('{0}', {1}, '{2}', '{3}', '{4}', '{5}')\r\n                                                ON DUPLICATE KEY UPDATE intervals={1}, bulletintext='{2}', fromdate='{3}', todate='{4}', opttime='{5}'", new object[]
				{
					msgID,
					interval,
					bulletinText,
					fromDate,
					toDate,
					today
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000814 RID: 2068 RVA: 0x0004A878 File Offset: 0x00048A78
		public static int RemoveBulletinText(DBManager dbMgr, string msgID)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE FROM t_bulletin WHERE msgid='{0}'", msgID);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000815 RID: 2069 RVA: 0x0004A8D0 File Offset: 0x00048AD0
		public static int UpdateGameConfig(DBManager dbMgr, string paramName, string paramValue)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_config (paramname, paramvalue) VALUES('{0}', '{1}') ON DUPLICATE KEY UPDATE paramvalue='{2}'", paramName, paramValue, paramValue);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x0004A92C File Offset: 0x00048B2C
		public static int AddSkill(DBManager dbMgr, int roleID, int skillID, int skillLevel)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				try
				{
					string cmdText = string.Format("INSERT INTO t_skills (rid, skillid, skilllevel, usednum) VALUES({0}, {1}, {2}, 0)", roleID, skillID, skillLevel);
					ret = conn.ExecuteNonQuery(cmdText, 0);
					if (ret >= 0)
					{
						ret = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
					}
				}
				catch (MySQLException)
				{
					ret = -2;
				}
			}
			return ret;
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x0004A9D0 File Offset: 0x00048BD0
		public static bool UpdateSkillInfo(DBManager dbMgr, int skillDbID, int skillLevel, int usedNum)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_skills SET skilllevel={0}, usednum={1} WHERE Id={2}", skillLevel, usedNum, skillDbID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x0004AA3C File Offset: 0x00048C3C
		public static bool UpdateJingMaiExp(DBManager dbMgr, int roleID, int jingMaiExpNum, int totalJingMaiExp)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET jingmai_exp_num={0}, total_jingmai_exp={1} WHERE rid={2}", jingMaiExpNum, totalJingMaiExp, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000819 RID: 2073 RVA: 0x0004AAA8 File Offset: 0x00048CA8
		public static int UpdateRoleBufferItem(DBManager dbMgr, int roleID, int bufferID, long startTime, int bufferSecs, long bufferVal)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_buffer (rid, bufferid, starttime, buffersecs, bufferval) VALUES({0}, {1}, {2}, {3}, {4}) ON DUPLICATE KEY UPDATE starttime={5}, buffersecs={6}, bufferval={7}", new object[]
				{
					roleID,
					bufferID,
					startTime,
					bufferSecs,
					bufferVal,
					startTime,
					bufferSecs,
					bufferVal,
					roleID,
					bufferID
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600081A RID: 2074 RVA: 0x0004AB74 File Offset: 0x00048D74
		public static int UpdateRoleDailyTaskData(DBManager dbMgr, int roleID, int huanID, string rectime, int recnum, int taskClass, int extDayID, int extNum)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_dailytasks (rid, huanid, rectime, recnum, taskClass, extdayid, extnum) VALUES({0}, {1}, '{2}', {3}, {4}, {5}, {6}) ON DUPLICATE KEY UPDATE huanid={1}, rectime='{2}', recnum={3}, taskClass={4}, extdayid={5}, extnum={6}", new object[]
				{
					roleID,
					huanID,
					rectime,
					recnum,
					taskClass,
					extDayID,
					extNum
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600081B RID: 2075 RVA: 0x0004AC1C File Offset: 0x00048E1C
		public static int UpdateRoleDailyJingMaiData(DBManager dbMgr, int roleID, string jmTime, int jmNum)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_dailyjingmai (rid, jmtime, jmnum) VALUES({0}, '{1}', {2}) ON DUPLICATE KEY UPDATE jmtime='{3}', jmnum={4}", new object[]
				{
					roleID,
					jmTime,
					jmNum,
					jmTime,
					jmNum
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600081C RID: 2076 RVA: 0x0004ACA8 File Offset: 0x00048EA8
		public static bool UpdateRoleNumSkillID(DBManager dbMgr, int roleID, int numSkillID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET numskillid={0} WHERE rid={1}", numSkillID, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x0004AD0C File Offset: 0x00048F0C
		public static bool UpdateRoleMainTaskID(DBManager dbMgr, int roleID, int mainTaskID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET maintaskid={0} WHERE rid={1}", mainTaskID, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600081E RID: 2078 RVA: 0x0004AD70 File Offset: 0x00048F70
		public static int UpdateRolePBInfo(DBManager dbMgr, int roleID, int extGridNum)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_ptbag (rid, extgridnum) VALUES({0}, {1}) ON DUPLICATE KEY UPDATE extgridnum={2}", roleID, extGridNum, extGridNum);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600081F RID: 2079 RVA: 0x0004ADDC File Offset: 0x00048FDC
		public static int UpdateRoleRebornStorageInfo(DBManager dbMgr, int roleID, int extGridNum)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.InsertRebornGridNum, roleID, extGridNum, extGridNum);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000820 RID: 2080 RVA: 0x0004AE48 File Offset: 0x00049048
		public static int UpdateRoleBagNum(DBManager dbMgr, int roleID, int bagNum)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles set bagnum={0} where rid={1}", bagNum, roleID);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000821 RID: 2081 RVA: 0x0004AEAC File Offset: 0x000490AC
		public static int UpdateRoleRebornBagNum(DBManager dbMgr, int roleID, int bagNum)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.UpdateRebornBagNum, bagNum, roleID);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x0004AF10 File Offset: 0x00049110
		public static int UpdateRoleRebornShowEquip(DBManager dbMgr, int roleID, int bagNum)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.UpdateRebornShow, bagNum, roleID);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000823 RID: 2083 RVA: 0x0004AF74 File Offset: 0x00049174
		public static int UpdateRoleRebornShowModel(DBManager dbMgr, int roleID, int bagNum)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.UpdateRebornShowModel, bagNum, roleID);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000824 RID: 2084 RVA: 0x0004AFD8 File Offset: 0x000491D8
		public static void CreateHuoDong(DBManager dbMgr, int roleID)
		{
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_huodong (rid, loginweekid, logindayid, loginnum, newstep, steptime, lastmtime, curmid, curmtime, songliid, logingiftstate, onlinegiftstate) VALUES({0}, '{1}', '{2}', {3}, {4}, '{5}', {6}, '{7}', {8}, {9}, {10}, {11})", new object[]
				{
					roleID,
					"",
					"",
					0,
					0,
					today,
					0,
					"",
					0,
					0,
					0,
					0
				});
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000825 RID: 2085 RVA: 0x0004B0BC File Offset: 0x000492BC
		public static int UpdateHuoDong(DBManager dbMgr, int id, string[] fields, int startIndex)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = DBWriter.FormatUpdateSQL(id, fields, startIndex, DBWriter._UpdateActivity_fieldNames, "t_huodong", DBWriter._UpdateActivity_fieldTypes, "rid");
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x0004B128 File Offset: 0x00049328
		public static int ClearAllLiPinMa(DBManager dbMgr)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE FROM t_linpinma", new object[0]);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x0004B188 File Offset: 0x00049388
		public static int InsertNewLiPinMa(DBManager dbMgr, string liPinMa, string songLiID, string maxNum, string ptid, string ptRepeat, string usedNum = "0")
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_linpinma (lipinma, huodongid, maxnum, usednum, ptid, ptrepeat) VALUES('{0}', {1}, {2}, {3}, {4}, {5})", new object[]
				{
					liPinMa,
					songLiID,
					maxNum,
					usedNum,
					ptid,
					ptRepeat
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x0004B20C File Offset: 0x0004940C
		public static int UpdateLiPinMaUsedNum(DBManager dbMgr, string liPinMa, int usedNum)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_linpinma SET usednum={0} WHERE lipinma='{1}'", usedNum, liPinMa);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x0004B26C File Offset: 0x0004946C
		public static int DeleteSpecialActivityData(DBManager dbMgr, int roleID, int groupID)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText;
				if (groupID == 0)
				{
					cmdText = string.Format("DELETE FROM t_special_activity WHERE rid={0}", roleID);
				}
				else
				{
					cmdText = string.Format("DELETE FROM t_special_activity WHERE rid={0} AND groupid={1}", roleID, groupID);
				}
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x0004B2FC File Offset: 0x000494FC
		public static int DeleteSpecialPriorityActivityData(DBManager dbMgr, int roleID, int tequanID)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText;
				if (tequanID == 0)
				{
					cmdText = string.Format("DELETE FROM t_special_priority_activity WHERE rid={0}", roleID);
				}
				else
				{
					cmdText = string.Format("DELETE FROM t_special_priority_activity WHERE rid={0} AND tequanid={1}", roleID, tequanID);
				}
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x0004B38C File Offset: 0x0004958C
		public static int DeleteEverydayActivityData(DBManager dbMgr, int roleID, int groupID, int actID)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = "";
				if (groupID == 0)
				{
					cmdText = string.Format("DELETE FROM t_everyday_activity WHERE rid={0}", roleID);
				}
				else if (groupID != 0 && actID == 0)
				{
					cmdText = string.Format("DELETE FROM t_everyday_activity WHERE rid={0} AND groupid={1}", roleID, groupID);
				}
				else if (groupID != 0 && actID != 0)
				{
					cmdText = string.Format("DELETE FROM t_everyday_activity WHERE rid={0} AND groupid={1} AND actid={2}", roleID, groupID, actID);
				}
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x0004B460 File Offset: 0x00049660
		public static int UpdateSpecialActivityData(DBManager dbMgr, int roleID, SpecActInfoDB SpecAct)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_special_activity (rid, groupid, actid, purchaseNum, countNum, active) VALUES({0}, {1}, {2}, {3}, {4}, {5})\r\n                        ON DUPLICATE KEY UPDATE groupid={1}, actid={2}, purchaseNum={3}, countNum={4}, active={5}", new object[]
				{
					roleID,
					SpecAct.GroupID,
					SpecAct.ActID,
					SpecAct.PurNum,
					SpecAct.CountNum,
					SpecAct.Active
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x0004B518 File Offset: 0x00049718
		public static int UpdateSpecialPriorityActivityData(DBManager dbMgr, int roleID, SpecPriorityActInfoDB SpecAct)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_special_priority_activity (rid, tequanid, actid, purchaseNum, countNum) VALUES({0}, {1}, {2}, {3}, {4})\r\n                        ON DUPLICATE KEY UPDATE tequanid={1}, actid={2}, purchaseNum={3}, countNum={4}", new object[]
				{
					roleID,
					SpecAct.TeQuanID,
					SpecAct.ActID,
					SpecAct.PurNum,
					SpecAct.CountNum
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x0004B5C0 File Offset: 0x000497C0
		public static int UpdateEverydayActivityData(DBManager dbMgr, int roleID, EverydayActInfoDB EveryAct)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_everyday_activity (rid, groupid, actid, purchaseNum, countNum, activeDay) VALUES({0}, {1}, {2}, {3}, {4}, {5})\r\n                        ON DUPLICATE KEY UPDATE groupid={1}, actid={2}, purchaseNum={3}, countNum={4}, activeDay={5}", new object[]
				{
					roleID,
					EveryAct.GroupID,
					EveryAct.ActID,
					EveryAct.PurNum,
					EveryAct.CountNum,
					EveryAct.ActiveDay
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x0004B678 File Offset: 0x00049878
		public static int UpdateFuBenData(DBManager dbMgr, int roleID, int fuBenID, int dayID, int enterNum, int nQuickPassTimeSec, int nFinishNum)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_fuben (rid, fubenid, dayid, enternum, quickpasstimer, finishnum) VALUES({0}, {1}, {2}, {3}, {4}, {5}) ON DUPLICATE KEY UPDATE fubenid={1}, dayid={2}, enternum={3}, quickpasstimer={4}, finishnum={5}", new object[]
				{
					roleID,
					fuBenID,
					dayID,
					enterNum,
					nQuickPassTimeSec,
					nFinishNum
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x0004B718 File Offset: 0x00049918
		public static int InsertNewPreName(DBManager dbMgr, string preName, int sex)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_prenames (name, sex, used) VALUES('{0}', {1}, {2})", preName, sex, 0);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x0004B77C File Offset: 0x0004997C
		public static int UpdatePreNameUsedState(DBManager dbMgr, string preName, int used)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_prenames SET used={0} WHERE name='{1}'", used, preName);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x0004B7DC File Offset: 0x000499DC
		public static int InsertNewFuBenHist(DBManager dbMgr, int fuBenID, int roleID, string roleName, int usedSecs)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_fubenhist (fubenid, rid, rname, usedsecs) VALUES({0}, {1}, '{2}', {3}) ON DUPLICATE KEY UPDATE rid={4}, rname='{5}', usedsecs={6}", new object[]
				{
					fuBenID,
					roleID,
					roleName,
					usedSecs,
					roleID,
					roleName,
					usedSecs
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000833 RID: 2099 RVA: 0x0004B87C File Offset: 0x00049A7C
		public static int UpdateRoleDailyData(DBManager dbMgr, int roleID, int expDayID, int todayExp, int lingLiDayID, int todayLingLi, int killBossDayID, int todayKillBoss, int fuBenDayID, int todayFuBenNum, int wuXingDayID, int wuXingNum, int rebornExpDayID, int rebornExpMonster, int rebornExpSale)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_dailydata (rid, expdayid, todayexp, linglidayid, todaylingli, killbossdayid, todaykillboss, fubendayid, todayfubennum, wuxingdayid, wuxingnum, reborndayid, rebornexpmonster, rebornexpsale) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}) ON DUPLICATE KEY UPDATE expdayid={1}, todayexp={2}, linglidayid={3}, todaylingli={4}, killbossdayid={5}, todaykillboss={6}, fubendayid={7}, todayfubennum={8}, wuxingdayid={9}, wuxingnum={10}, reborndayid={11}, rebornexpmonster={12}, rebornexpsale={13}", new object[]
				{
					roleID,
					expDayID,
					todayExp,
					lingLiDayID,
					todayLingLi,
					killBossDayID,
					todayKillBoss,
					fuBenDayID,
					todayFuBenNum,
					wuXingDayID,
					wuXingNum,
					rebornExpDayID,
					rebornExpMonster,
					rebornExpSale
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x0004B97C File Offset: 0x00049B7C
		public static int UpdateYaBiaoData(DBManager dbMgr, int roleID, int yaBiaoID, long startTime, int state, int lineID, int touBao, int yaBiaoDayID, int yaBiaoNum, int takeGoods)
		{
			int ret = -1;
			string today = "1900-01-01 12:00:00";
			if (startTime > 0L)
			{
				today = new DateTime(startTime * 10000L).ToString("yyyy-MM-dd HH:mm:ss");
			}
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_yabiao (rid, yabiaoid, starttime, state, lineid, toubao, yabiaodayid, yabiaonum, takegoods) VALUES({0}, {1}, '{2}', {3}, {4}, {5}, {6}, {7}, {8}) ON DUPLICATE KEY UPDATE yabiaoid={1}, starttime='{2}', state={3}, lineid={4}, toubao={5}, yabiaodayid={6}, yabiaonum={7}, takegoods={8}", new object[]
				{
					roleID,
					yaBiaoID,
					today,
					state,
					lineID,
					touBao,
					yaBiaoDayID,
					yaBiaoNum,
					takeGoods
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x0004BA70 File Offset: 0x00049C70
		public static int UpdateYaBiaoDataState(DBManager dbMgr, int roleID, int state)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_yabiao SET state={0} WHERE rid={1}", state, roleID);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000836 RID: 2102 RVA: 0x0004BAD4 File Offset: 0x00049CD4
		public static int AddNewMallBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftMoney)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_mallbuy (rid, goodsid, goodsnum, totalprice, leftmoney, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')", new object[]
				{
					roleID,
					goodsID,
					goodsNum,
					totalPrice,
					leftMoney,
					today
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x0004BB84 File Offset: 0x00049D84
		public static int AddNewQiZhenGeBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftMoney)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_qizhengebuy (rid, goodsid, goodsnum, totalprice, leftmoney, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')", new object[]
				{
					roleID,
					goodsID,
					goodsNum,
					totalPrice,
					leftMoney,
					today
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x0004BC34 File Offset: 0x00049E34
		public static int AddNewShengXiaoGuessHistory(DBManager dbMgr, int roleID, string roleName, int zoneID, int guessKey, int mortgage, int resultKey, int gainNum, int leftMortgage)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_shengxiaoguesshist (rid, rname, guesskey, mortgage, resultkey, gainnum, leftmortgage, zoneid, guesstime) VALUES({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, '{8}')", new object[]
				{
					roleID,
					roleName,
					guessKey,
					mortgage,
					resultKey,
					gainNum,
					leftMortgage,
					zoneID,
					today
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x0004BD04 File Offset: 0x00049F04
		public static int AddNewYinPiaoBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftYinPiaoNum)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_yinpiaobuy (rid, goodsid, goodsnum, totalprice, leftyinpiao, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')", new object[]
				{
					roleID,
					goodsID,
					goodsNum,
					totalPrice,
					leftYinPiaoNum,
					today
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x0004BDB4 File Offset: 0x00049FB4
		public static int AddNewOnlineNumItem(DBManager dbMgr, int totalNum, DateTime dateTime, string strMapOnlineInfo)
		{
			int ret = -1;
			string today = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_onlinenum (num, rectime, mapnum) VALUES({0}, '{1}', '{2}')", totalNum, today, strMapOnlineInfo);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x0004BE24 File Offset: 0x0004A024
		public static int CreateBangHui(DBManager dbMgr, int roleID, int zoneID, int totalLevel, string bhName, string bhBulletin, int nMoney = 0)
		{
			int bhid = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			MySQLConnection conn = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				MySQLConnection conn2 = conn;
				string[] fields = new string[]
				{
					"zoneid"
				};
				string[] tables = new string[]
				{
					"t_banghui"
				};
				object[,] array = new object[1, 3];
				array[0, 0] = "bhname";
				array[0, 1] = "=";
				array[0, 2] = bhName;
				MySQLSelectCommand SelectCmd = new MySQLSelectCommand(conn2, fields, tables, array, null, null);
				if (SelectCmd.Table.Rows.Count > 0)
				{
					bhid = -1;
					LogManager.WriteLog(LogTypes.Error, string.Format("帮会已存在: {0}", bhName), null, true);
					return bhid;
				}
				bool error = false;
				string cmdText = string.Format("INSERT INTO t_banghui (bhname, zoneid, rid, totalnum, totallevel, bhbulletin, buildtime, qiname, tongqian) VALUES('{0}', {1}, {2}, {3}, {4}, '{5}', '{6}', '{7}', {8})", new object[]
				{
					bhName,
					zoneID,
					roleID,
					1,
					totalLevel,
					bhBulletin,
					today,
					bhName,
					nMoney
				});
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				try
				{
					cmd.ExecuteNonQuery();
				}
				catch (MySQLException)
				{
					error = true;
					bhid = -1;
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText), null, true);
				}
				cmd.Dispose();
				cmd = null;
				try
				{
					if (!error)
					{
						cmd = new MySQLCommand("SELECT LAST_INSERT_ID() AS LastID", conn);
						MySQLDataReader reader = cmd.ExecuteReaderEx();
						if (reader.Read())
						{
							bhid = Convert.ToInt32(reader[0].ToString());
						}
						cmd.Dispose();
						cmd = null;
					}
				}
				catch (MySQLException)
				{
					bhid = -2;
				}
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return bhid;
		}

		// Token: 0x0600083C RID: 2108 RVA: 0x0004C088 File Offset: 0x0004A288
		public static void DeleteBangHui(DBManager dbMgr, int bhid)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_banghui SET isdel=1 WHERE bhid={0}", bhid);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x0004C0DC File Offset: 0x0004A2DC
		public static void UpdateZhengDuoUsedTime(DBManager dbMgr, int bhid, int weekDay, int usedTime)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_banghui SET zhengduousedtime={1},zhengduoweek={2} WHERE bhid={0}", bhid, usedTime, weekDay);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x0004C13C File Offset: 0x0004A33C
		public static void UpdateBangHuiBulletin(DBManager dbMgr, int bhid, string bhBulletinMsg)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_banghui SET bhbulletin='{0}' WHERE bhid={1} AND isdel=0", bhBulletinMsg, bhid);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x0004C194 File Offset: 0x0004A394
		public static void UpdateBangHuiVerify(DBManager dbMgr, int roleID, int bhid, int isVerify)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_banghui SET isverfiy={0} WHERE bhid={1} AND rid={2}", isVerify, bhid, roleID);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x0004C1F4 File Offset: 0x0004A3F4
		public static void UpdateBangHuiRoleID(DBManager dbMgr, int roleID, int bhid)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_banghui SET rid={0} WHERE bhid={1}", roleID, bhid);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x0004C250 File Offset: 0x0004A450
		public static void ClearBangHuiMemberZhiWu(DBManager dbMgr, int bhid, int zhiWu)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET bhzhiwu=0 WHERE faction={0} AND bhzhiwu={1}", bhid, zhiWu);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000842 RID: 2114 RVA: 0x0004C2AC File Offset: 0x0004A4AC
		public static void ChangeJunTuanZhiWuList(DBManager dbMgr, int bhid, int zhiwu, List<int> list)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET juntuanzhiwu={1} WHERE faction={0}", bhid, zhiwu);
				if (list.Count > 0)
				{
					string ridListStr = string.Join<int>(",", list);
					cmdText += string.Format(" AND rid in({0});", ridListStr);
					conn.ExecuteNonQuery(cmdText, 0);
				}
			}
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x0004C338 File Offset: 0x0004A538
		public static void ClearBangHuiZhiWuNotInList(DBManager dbMgr, int bhid, List<int> list)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET juntuanzhiwu=0 WHERE faction={0}", bhid);
				if (list.Count > 0)
				{
					string ridListStr = string.Join<int>(",", list);
					cmdText += string.Format(" AND rid not in({0});", ridListStr);
				}
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000844 RID: 2116 RVA: 0x0004C3BC File Offset: 0x0004A5BC
		public static void UpdateBangHuiMemberZhiWu(DBManager dbMgr, int bhid, int otherRoleID, int zhiWu)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET bhzhiwu={0} WHERE faction={1} AND rid={2}", zhiWu, bhid, otherRoleID);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000845 RID: 2117 RVA: 0x0004C41C File Offset: 0x0004A61C
		public static void UpdateBangHuiMemberChengHao(DBManager dbMgr, int bhid, int otherRoleID, string chengHao)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET chenghao='{0}' WHERE faction={1} AND rid={2}", chengHao, bhid, otherRoleID);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x0004C478 File Offset: 0x0004A678
		public static void UpdateBangHuiBangGong(DBManager dbMgr, int bhid, int goods1Num, int goods2Num, int goods3Num, int goods4Num, int goods5Num, int tongQian)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_banghui SET goods1num=goods1num+{0}, goods2num=goods2num+{1}, goods3num=goods3num+{2}, goods4num=goods4num+{3}, goods5num=goods5num+{4}, tongqian=tongqian+{5} WHERE bhid={6}", new object[]
				{
					goods1Num,
					goods2Num,
					goods3Num,
					goods4Num,
					goods5Num,
					tongQian,
					bhid
				});
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x0004C514 File Offset: 0x0004A714
		public static void AddBangGongHistItem(DBManager dbMgr, int roleID, int bhid, int goods1Num, int goods2Num, int goods3Num, int goods4Num, int goods5Num, int tongQian, int banggong)
		{
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_banggonghist (bhid, rid, goods1num, goods2num, goods3num, goods4num, goods5num, tongqian, banggong, addtime) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}') ON DUPLICATE KEY UPDATE goods1num=goods1num+{2}, goods2num=goods2num+{3}, goods3num=goods3num+{4}, goods4num=goods4num+{5}, goods5num=goods5num+{6}, tongqian=tongqian+{7}, banggong=banggong+{8}, addtime='{9}'", new object[]
				{
					roleID,
					bhid,
					goods1Num,
					goods2Num,
					goods3Num,
					goods4Num,
					goods5Num,
					tongQian,
					banggong,
					today
				});
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x0004C5E8 File Offset: 0x0004A7E8
		public static void UpdateBangHuiQiName(DBManager dbMgr, int bhid, string qiName, int needMoney)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_banghui SET qiname='{0}', tongqian=tongqian-{1} WHERE bhid={2}", qiName, needMoney, bhid);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000849 RID: 2121 RVA: 0x0004C644 File Offset: 0x0004A844
		public static void UpdateBangHuiQiLevel(DBManager dbMgr, int bhid, int toLevel, int goods1Num, int goods2Num, int goods3Num, int goods4Num, int goods5Num, int needMoney)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_banghui SET qilevel={0}, goods1num=goods1num-{1}, goods2num=goods2num-{2}, goods3num=goods3num-{3}, goods4num=goods4num-{4}, goods5num=goods5num-{5}, tongqian=tongqian-{6} WHERE bhid={7}", new object[]
				{
					toLevel,
					goods1Num,
					goods2Num,
					goods3Num,
					goods4Num,
					goods5Num,
					needMoney,
					bhid
				});
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x0600084A RID: 2122 RVA: 0x0004C6E8 File Offset: 0x0004A8E8
		public static void UpdateOnePieceTreasureLog(DBManager dbMgr, string KeyTime, int LogType, int addValue)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText;
				switch (LogType)
				{
				case 0:
					cmdText = string.Format("INSERT INTO t_treasure_log (time, role) VALUES('{0}', {1}) ON DUPLICATE KEY UPDATE role=role+{1}", KeyTime, addValue);
					break;
				case 1:
					cmdText = string.Format("INSERT INTO t_treasure_log (time, dice) VALUES('{0}', {1}) ON DUPLICATE KEY UPDATE dice=dice+{1}", KeyTime, addValue);
					break;
				case 2:
					cmdText = string.Format("INSERT INTO t_treasure_log (time, superdice) VALUES('{0}', {1}) ON DUPLICATE KEY UPDATE superdice=superdice+{1}", KeyTime, addValue);
					break;
				case 3:
					cmdText = string.Format("INSERT INTO t_treasure_log (time, movenum) VALUES('{0}', {1}) ON DUPLICATE KEY UPDATE movenum=movenum+{1}", KeyTime, addValue);
					break;
				default:
					throw new ArgumentException("The UpdateOnePieceTreasureLog RPC OnePieceTreasureLogType Wrong");
				}
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x0004C7B0 File Offset: 0x0004A9B0
		public static void UpdateBuildingLog(DBManager dbMgr, string KeyTime, BuildingLogType LogType)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText;
				switch (LogType)
				{
				case BuildingLogType.BuildLog_TaskRole:
					cmdText = string.Format("INSERT INTO t_building_log (time, task_role) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE task_role=task_role+1", KeyTime);
					break;
				case BuildingLogType.BuildLog_Task:
					cmdText = string.Format("INSERT INTO t_building_log (time, task) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE task=task+1", KeyTime);
					break;
				case BuildingLogType.BuildLog_RefreshRole:
					cmdText = string.Format("INSERT INTO t_building_log (time, refresh_role) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE refresh_role=refresh_role+1", KeyTime);
					break;
				case BuildingLogType.BuildLog_Refresh:
					cmdText = string.Format("INSERT INTO t_building_log (time, refresh) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE refresh=refresh+1", KeyTime);
					break;
				case BuildingLogType.BuildLog_OpenRole:
					cmdText = string.Format("INSERT INTO t_building_log (time, open_role) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE open_role=open_role+1", KeyTime);
					break;
				case BuildingLogType.BuildLog_Open:
					cmdText = string.Format("INSERT INTO t_building_log (time, open) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE open=open+1", KeyTime);
					break;
				case BuildingLogType.BuildLog_Push:
					cmdText = string.Format("INSERT INTO t_building_log (time, push) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE push=push+1", KeyTime);
					break;
				case BuildingLogType.BuildLog_PushUse:
					cmdText = string.Format("INSERT INTO t_building_log (time, pushuse) VALUES('{0}', 1) ON DUPLICATE KEY UPDATE pushuse=pushuse+1", KeyTime);
					break;
				default:
					throw new ArgumentException("The UpdateBuildingLog RPC BuildingLogType Wrong");
				}
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x0600084C RID: 2124 RVA: 0x0004C8B4 File Offset: 0x0004AAB4
		public static void UpdateBuildingData(DBManager dbMgr, int rid, BuildingData myBuildData)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_building (rid, buildid, taskid_1, taskid_2, taskid_3, taskid_4, level, exp, developtime) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}') ON DUPLICATE KEY UPDATE taskid_1 = {2}, taskid_2 = {3}, taskid_3 = {4}, taskid_4 = {5}, level = {6}, exp = {7}, developtime = '{8}'", new object[]
				{
					rid,
					myBuildData.BuildId,
					myBuildData.TaskID_1,
					myBuildData.TaskID_2,
					myBuildData.TaskID_3,
					myBuildData.TaskID_4,
					myBuildData.BuildLev,
					myBuildData.BuildExp,
					myBuildData.BuildTime
				});
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x0004C980 File Offset: 0x0004AB80
		public static void UpdateOramentData(DBManager dbMgr, OrnamentUpdateDbData myOrnamentData)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_ornament (roleid, goodsid, param1, param2) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE param1 = {2}, param2 = {3}", new object[]
				{
					myOrnamentData.RoleId,
					myOrnamentData.Data.ID,
					myOrnamentData.Data.Param1,
					myOrnamentData.Data.Param2
				});
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x0600084E RID: 2126 RVA: 0x0004CA20 File Offset: 0x0004AC20
		public static void UpdateZhanMengBuildLevel(DBManager dbMgr, int bhid, int toLevel, int needMoney, string fieldName)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_banghui SET {0}={1}, tongqian=tongqian-{2} WHERE bhid={3}", new object[]
				{
					fieldName,
					toLevel,
					needMoney,
					bhid
				});
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x0004CA98 File Offset: 0x0004AC98
		public static void AddBangHuiTongQian(DBManager dbMgr, int bhid, int addMoney)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_banghui SET tongqian=tongqian+{0} WHERE bhid={1}", addMoney, bhid);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x0004CAF4 File Offset: 0x0004ACF4
		public static void SubBangHuiTongQian(DBManager dbMgr, int bhid, int subMoney)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_banghui SET tongqian=tongqian-{0} WHERE bhid={1}", subMoney, bhid);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x0004CB50 File Offset: 0x0004AD50
		public static void UpdateBangHuiSumData(DBManager dbMgr)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = "UPDATE t_banghui b,(SELECT SUM(combatforce) AS totalcombatforce,SUM(LEVEL) AS totallevel,COUNT(rid) AS totalnum,faction AS bhid FROM t_roles WHERE isdel=0 and faction>0 GROUP BY faction) r SET b.totalcombatforce=r.totalcombatforce,b.totallevel=r.totallevel,b.totalnum=r.totalnum WHERE b.`bhid`=r.bhid and b.isdel=0";
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000852 RID: 2130 RVA: 0x0004CB9C File Offset: 0x0004AD9C
		public static void UpdateBangHuiNumLevel(DBManager dbMgr, int bhid, int totalNum, int totalLevel, int TotalCombatForce)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_banghui SET totalnum={0}, totallevel={1}, totalcombatforce={3} WHERE bhid={2}", new object[]
				{
					totalNum,
					totalLevel,
					bhid,
					TotalCombatForce
				});
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000853 RID: 2131 RVA: 0x0004CC18 File Offset: 0x0004AE18
		public static void UpdateBHLingDi(DBManager dbMgr, BangHuiLingDiInfoData bangHuiLingDiInfoData)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_lingdi (lingdi, bhid, tax, takedayid, takedaynum, yestodaytax, taxdayid, todaytax, totaltax, warrequest, awardfetchday) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}', {10}) ON DUPLICATE KEY UPDATE bhid={1}, tax={2}, takedayid={3}, takedaynum={4}, yestodaytax={5}, taxdayid={6}, todaytax={7}, totaltax={8}, warrequest='{9}', awardfetchday={10}", new object[]
				{
					bangHuiLingDiInfoData.LingDiID,
					bangHuiLingDiInfoData.BHID,
					bangHuiLingDiInfoData.LingDiTax,
					bangHuiLingDiInfoData.TakeDayID,
					bangHuiLingDiInfoData.TakeDayNum,
					bangHuiLingDiInfoData.YestodayTax,
					bangHuiLingDiInfoData.TaxDayID,
					bangHuiLingDiInfoData.TodayTax,
					bangHuiLingDiInfoData.TotalTax,
					bangHuiLingDiInfoData.WarRequest,
					bangHuiLingDiInfoData.AwardFetchDay
				});
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x0004CD08 File Offset: 0x0004AF08
		public static void ClearBHLingDiByID(DBManager dbMgr, int bhid)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_lingdi SET bhid=0, tax=0, takedayid=0, takedaynum=0, yestodaytax=0, taxdayid=0, todaytax=0, totaltax=0 WHERE bhid={0}", bhid);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x0004CD5C File Offset: 0x0004AF5C
		public static void ClearBHLingDiTotalTaxByID(DBManager dbMgr, int lingDiID)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_lingdi SET totaltax=0 WHERE lingdi={0}", lingDiID);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000856 RID: 2134 RVA: 0x0004CDB0 File Offset: 0x0004AFB0
		public static void SubBangHuiTongQianByQiLevel(DBManager dbMgr, int moneyPerLevel)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_banghui SET tongqian=tongqian-{0} where isdel=0", moneyPerLevel);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000857 RID: 2135 RVA: 0x0004CE04 File Offset: 0x0004B004
		public static void UpdateBangHuiFuBen(DBManager dbMgr, int bhid, int fubenid, int state, int openday, string killers)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_banghui SET fubenid={0}, fubenstate={1}, openday={2}, killers='{3}' WHERE bhid={4} AND isdel=0", new object[]
				{
					fubenid,
					state,
					openday,
					killers,
					bhid
				});
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000858 RID: 2136 RVA: 0x0004CE84 File Offset: 0x0004B084
		public static void UpdateRoleToHuangFei(DBManager dbMgr, int roleID, int huangHou)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET huanghou={0} WHERE rid={1}", huangHou, roleID);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000859 RID: 2137 RVA: 0x0004CEE0 File Offset: 0x0004B0E0
		public static void UpdateHuangDiTeQuan(DBManager dbMgr, HuangDiTeQuanItem hangDiTeQuanItem)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_hdtequan (Id, tolaofangdayid, tolaofangnum, offlaofangdayid, offlaofangnum, bancatdayid, bancatnum) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}) ON DUPLICATE KEY UPDATE Id={0}, tolaofangdayid={1}, tolaofangnum={2}, offlaofangdayid={3}, offlaofangnum={4}, bancatdayid={5}, bancatnum={6}", new object[]
				{
					hangDiTeQuanItem.ID,
					hangDiTeQuanItem.ToLaoFangDayID,
					hangDiTeQuanItem.ToLaoFangNum,
					hangDiTeQuanItem.OffLaoFangDayID,
					hangDiTeQuanItem.OffLaoFangNum,
					hangDiTeQuanItem.BanCatDayID,
					hangDiTeQuanItem.BanCatNum
				});
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x0600085A RID: 2138 RVA: 0x0004CF98 File Offset: 0x0004B198
		public static void AddRefreshQiZhenGeRec(DBManager dbMgr, int roleID, int oldUserMoney, int leftUserMoney)
		{
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_refreshqizhen (rid, oldusermoney, leftusermoney, refreshtime) VALUES({0}, {1}, {2}, '{3}')", new object[]
				{
					roleID,
					oldUserMoney,
					leftUserMoney,
					today
				});
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x0004D028 File Offset: 0x0004B228
		public static void AddUsedLiPinMa(DBManager dbMgr, int huodongID, string lipinMa, int pingTaiID, int roleID)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_usedlipinma (lipinma, huodongid, ptid, rid) VALUES('{0}', {1}, {2}, {3})", new object[]
				{
					lipinMa,
					huodongID,
					pingTaiID,
					roleID
				});
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x0600085C RID: 2140 RVA: 0x0004D0A0 File Offset: 0x0004B2A0
		public static void AddMoneyWarning(DBManager dbMgr, int roleID, int usedMoney, int goodsMoney)
		{
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_warning (rid, usedmoney, goodsmoney, warningtime) VALUES({0}, {1}, {2}, '{3}')", new object[]
				{
					roleID,
					usedMoney,
					goodsMoney,
					today
				});
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x0600085D RID: 2141 RVA: 0x0004D130 File Offset: 0x0004B330
		public static int AddNewBuyItemFromNpc(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftMoney, int moneyType)
		{
			int result;
			if (GameDBManager.DisableSomeLog)
			{
				result = 1;
			}
			else
			{
				int ret = -1;
				string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText = string.Format("INSERT INTO t_npcbuy (rid, goodsid, goodsnum, totalprice, leftmoney, buytime, moneytype) VALUES({0}, {1}, {2}, {3}, {4}, '{5}', {6})", new object[]
					{
						roleID,
						goodsID,
						goodsNum,
						totalPrice,
						leftMoney,
						today,
						moneyType
					});
					ret = conn.ExecuteNonQuery(cmdText, 0);
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x0600085E RID: 2142 RVA: 0x0004D204 File Offset: 0x0004B404
		public static int AddNewYinLiangBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftYinLiang)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_yinliangbuy (rid, goodsid, goodsnum, totalprice, leftyinliang, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')", new object[]
				{
					roleID,
					goodsID,
					goodsNum,
					totalPrice,
					leftYinLiang,
					today
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600085F RID: 2143 RVA: 0x0004D2B4 File Offset: 0x0004B4B4
		public static int AddNewGoldBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftGold)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_goldbuy (rid, goodsid, goodsnum, totalprice, leftgold, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')", new object[]
				{
					roleID,
					goodsID,
					goodsNum,
					totalPrice,
					leftGold,
					today
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000860 RID: 2144 RVA: 0x0004D364 File Offset: 0x0004B564
		public static int AddNewBangGongBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftBangGong)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_banggongbuy (rid, goodsid, goodsnum, totalprice, leftbanggong, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')", new object[]
				{
					roleID,
					goodsID,
					goodsNum,
					totalPrice,
					leftBangGong,
					today
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000861 RID: 2145 RVA: 0x0004D414 File Offset: 0x0004B614
		public static void UpdateLastScanInputLogID(DBManager dbMgr, int lastid)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_inputhist (Id, lastid) VALUES(1, {0}) ON DUPLICATE KEY UPDATE lastid={0}", lastid);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000862 RID: 2146 RVA: 0x0004D468 File Offset: 0x0004B668
		public static void UpdateMailHasReadFlag(DBManager dbMgr, int mailID, int rid)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_mail SET isread=1, readtime=now() where mailid={0} and receiverrid={1}", mailID, rid);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000863 RID: 2147 RVA: 0x0004D4C4 File Offset: 0x0004B6C4
		public static bool UpdateMailHasFetchGoodsFlag(DBManager dbMgr, int mailID, int rid)
		{
			bool ret = true;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_mail SET hasfetchattachment=1 where mailid={0} and receiverrid={1}", mailID, rid);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000864 RID: 2148 RVA: 0x0004D528 File Offset: 0x0004B728
		public static bool DeleteMailDataItemExcludeGoodsList(DBManager dbMgr, int mailID, int rid)
		{
			bool ret = true;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE from t_mail where mailid={0} and receiverrid={1}", mailID, rid);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000865 RID: 2149 RVA: 0x0004D58C File Offset: 0x0004B78C
		public static void DeleteMailGoodsList(DBManager dbMgr, int mailID)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE from t_mailgoods where mailid={0}", mailID);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000866 RID: 2150 RVA: 0x0004D5E0 File Offset: 0x0004B7E0
		public static int AddMailBody(DBManager dbMgr, int senderrid, string senderrname, int receiverrid, string reveiverrname, string subject, string content, int yinliang, int tongqian, int yuanbao)
		{
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			int mailID = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_mail (senderrid, senderrname, sendtime, receiverrid, reveiverrname, readtime, isread, mailtype, hasfetchattachment, subject,content, yinliang, tongqian, yuanbao) VALUES ({0},'{1}','{2}', {3}, '{4}','{5}',{6},{7},{8},'{9}','{10}',{11},{12}, {13})", new object[]
				{
					senderrid,
					senderrname,
					today,
					receiverrid,
					reveiverrname,
					"2000-11-11 11:11:11",
					0,
					1,
					0,
					subject,
					content,
					yinliang,
					tongqian,
					yuanbao
				});
				int ret = conn.ExecuteNonQuery(cmdText, 0);
				if (ret < 0)
				{
					mailID = -2;
					return mailID;
				}
				try
				{
					mailID = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
				}
				catch (MySQLException)
				{
					mailID = -3;
				}
			}
			return mailID;
		}

		// Token: 0x06000867 RID: 2151 RVA: 0x0004D71C File Offset: 0x0004B91C
		public static bool AddMailGoodsDataItem(DBManager dbMgr, int mailID, int goodsid, int forge_level, int quality, string Props, int gcount, int origholenum, int rmbholenum, string jewellist, int addpropindex, int binding, int bornindex, int lucky, int strong, int ExcellenceInfo, int nAppendPropLev, int nEquipChangeLife)
		{
			bool ret = true;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_mailgoods (mailid,goodsid,forge_level, quality,Props,gcount,origholenum,rmbholenum,jewellist,addpropindex,binding,bornindex,lucky,strong, excellenceinfo, appendproplev, equipchangelife) VALUES ({0}, {1}, {2}, {3}, '{4}', {5}, {6}, {7}, '{8}', {9},{10},{11},{12},{13},{14},{15},{16})", new object[]
				{
					mailID,
					goodsid,
					forge_level,
					quality,
					Props,
					gcount,
					origholenum,
					rmbholenum,
					jewellist,
					addpropindex,
					binding,
					bornindex,
					lucky,
					strong,
					ExcellenceInfo,
					nAppendPropLev,
					nEquipChangeLife
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000868 RID: 2152 RVA: 0x0004D83C File Offset: 0x0004BA3C
		public static void UpdateLastScanMailID(DBManager dbMgr, int roleID, int mailID)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_mailtemp (receiverrid, mailid) VALUES ({0}, {1})", roleID, mailID);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x06000869 RID: 2153 RVA: 0x0004D898 File Offset: 0x0004BA98
		public static void ClearOverdueMails(DBManager dbMgr, DateTime overdueTime)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE FROM t_mailgoods WHERE mailid IN (SELECT mailid FROM t_mail WHERE sendtime < '{0}');", overdueTime.ToString("yyyy-MM-dd HH:mm:ss"));
				if (conn.ExecuteNonQuery(cmdText, 0) >= 0)
				{
					cmdText = string.Format("DELETE from t_mail where sendtime < '{0}'", overdueTime.ToString("yyyy-MM-dd HH:mm:ss"));
					conn.ExecuteNonQuery(cmdText, 0);
				}
			}
		}

		// Token: 0x0600086A RID: 2154 RVA: 0x0004D920 File Offset: 0x0004BB20
		public static void DeleteLastScanMailIDs(DBManager dbMgr, Dictionary<int, int> lastMailDict)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string sWhere = "";
				foreach (KeyValuePair<int, int> item in lastMailDict)
				{
					if (sWhere.Length > 0)
					{
						sWhere += " or ";
					}
					else
					{
						sWhere = " where ";
					}
					sWhere += string.Format(" (mailid<={0} and receiverrid={1}) ", item.Value, item.Key);
				}
				string cmdText = string.Format("DELETE from t_mailtemp {0}", sWhere);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x0004DA0C File Offset: 0x0004BC0C
		public static void DeleteMailIDInMailTemp(DBManager dbMgr, int mailID)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE from t_mailtemp where mailid={0}", mailID);
				conn.ExecuteNonQuery(cmdText, 0);
			}
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0004DA60 File Offset: 0x0004BC60
		public static bool UpdateRoleLastMail(DBManager dbMgr, int roleID, int mailID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET lastmailid={0} WHERE rid={1}", mailID, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600086D RID: 2157 RVA: 0x0004DAC4 File Offset: 0x0004BCC4
		public static int AddHongDongPaiHangRecord(DBManager dbMgr, int rid, string rname, int zoneid, int huoDongType, int paihang, string paihangtime, int phvalue)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_huodongpaihang (rid, rname, zoneid, type, paihang, paihangtime, phvalue) VALUES({0}, '{1}', {2}, {3}, {4}, '{5}', {6})", new object[]
				{
					rid,
					rname,
					zoneid,
					huoDongType,
					paihang,
					paihangtime,
					phvalue
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600086E RID: 2158 RVA: 0x0004DB68 File Offset: 0x0004BD68
		public static int AddHongDongAwardRecordForRole(DBManager dbMgr, int rid, int zoneid, int activitytype, string keystr, int hasgettimes, string lastgettime)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_huodongawardrolehist (rid, zoneid, activitytype, keystr, hasgettimes,lastgettime) VALUES({0}, {1}, {2}, '{3}', {4}, '{5}')", new object[]
				{
					rid,
					zoneid,
					activitytype,
					keystr,
					hasgettimes,
					lastgettime
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600086F RID: 2159 RVA: 0x0004DC00 File Offset: 0x0004BE00
		public static int AddHongDongAwardRecordForUser(DBManager dbMgr, string userid, int activitytype, string keystr, long hasgettimes, string lastgettime)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_huodongawarduserhist (userid, activitytype, keystr, hasgettimes,lastgettime) VALUES('{0}', {1}, '{2}', {3}, '{4}')", new object[]
				{
					userid,
					activitytype,
					keystr,
					hasgettimes,
					lastgettime
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000870 RID: 2160 RVA: 0x0004DC88 File Offset: 0x0004BE88
		public static int AddRegressHongDongAwardRecordForUser(DBManager dbMgr, string userid, int activitytype, string keystr, long hasgettimes, string lastgettime, string activedata, string stage)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.RegressInsertDaySignData, new object[]
				{
					userid,
					activitytype,
					keystr,
					hasgettimes,
					lastgettime,
					activedata,
					stage
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000871 RID: 2161 RVA: 0x0004DD1C File Offset: 0x0004BF1C
		public static int UpdateRegressHongDongAwardRecordForUser(DBManager dbMgr, string userid, int activitytype, string keystr, long hasgettimes, string lastgettime, string activedata, string stage)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.RegressUpdateDaySignData, new object[]
				{
					activedata,
					lastgettime,
					userid,
					activitytype,
					keystr,
					stage
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000872 RID: 2162 RVA: 0x0004DDA4 File Offset: 0x0004BFA4
		public static int UpdateHongDongAwardRecordForRole(DBManager dbMgr, int rid, int zoneid, int activitytype, string keystr, int hasgettimes, string lastgettime)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_huodongawardrolehist SET hasgettimes={0}, lastgettime='{1}' WHERE rid={2} AND zoneid={3} AND activitytype={4} AND keystr='{5}' AND hasgettimes!={0}", new object[]
				{
					hasgettimes,
					lastgettime,
					rid,
					zoneid,
					activitytype,
					keystr,
					hasgettimes
				});
				ret = ((conn.ExecuteNonQuery(cmdText, 0) > 0) ? 0 : -1);
			}
			return ret;
		}

		// Token: 0x06000873 RID: 2163 RVA: 0x0004DE50 File Offset: 0x0004C050
		public static int UpdateHongDongAwardRecordForUser(DBManager dbMgr, string userid, int activitytype, string keystr, long hasgettimes, string lastgettime)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("update t_huodongawarduserhist set hasgettimes={0}, lastgettime='{1}' where userid='{2}' and activitytype={3} and keystr='{4}' and hasgettimes!={5}", new object[]
				{
					hasgettimes,
					lastgettime,
					userid,
					activitytype,
					keystr,
					hasgettimes
				});
				ret = ((conn.ExecuteNonQuery(cmdText, 0) > 0) ? 0 : -1);
			}
			return ret;
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x0004DEEC File Offset: 0x0004C0EC
		public static int AddLimitGoodsBuyItem(DBManager dbMgr, int roleID, int goodsID, int dayID, int usedNum)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_limitgoodsbuy (rid, goodsid, dayid, usednum) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE dayID={2}, usedNum={3}", new object[]
				{
					roleID,
					goodsID,
					dayID,
					usedNum
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000875 RID: 2165 RVA: 0x0004DF78 File Offset: 0x0004C178
		public static int AddVipDailyData(DBManager dbMgr, int roleID, int priorityType, int dayID, int usedTimes)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_vipdailydata (rid, prioritytype, dayid, usedtimes) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE dayid={2}, usedtimes={3}", new object[]
				{
					roleID,
					priorityType,
					dayID,
					usedTimes
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x0004E004 File Offset: 0x0004C204
		public static int AddYangGongBKDailyJiFenData(DBManager dbMgr, int roleID, int jifen, int dayID, long awardhistory)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_yangguangbkdailydata (rid, jifen, dayid, awardhistory) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE jifen={1}, dayid={2}, awardhistory={3}", new object[]
				{
					roleID,
					jifen,
					dayID,
					awardhistory
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x0004E090 File Offset: 0x0004C290
		public static int ChangeTablesAutoIncrementValue(DBManager dbMgr, string sTableName, int nAutoIncrementValue)
		{
			int result;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("alter table {0} auto_increment={1}", sTableName, nAutoIncrementValue);
				conn.ExecuteNonQuery(cmdText, 0);
				result = 0;
			}
			return result;
		}

		// Token: 0x06000878 RID: 2168 RVA: 0x0004E0E8 File Offset: 0x0004C2E8
		public static bool UpdateGoodsLimit(DBManager dbMgr, int roleID, int goodsID, int dayID, int usedNum)
		{
			bool result;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_goodslimit (rid, goodsid, dayid, usednum) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE dayid={2}, usednum={3}", new object[]
				{
					roleID,
					goodsID,
					dayID,
					usedNum
				});
				result = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return result;
		}

		// Token: 0x06000879 RID: 2169 RVA: 0x0004E168 File Offset: 0x0004C368
		public static bool UpdateRoleParams(DBManager dbMgr, int roleID, string name, string value, RoleParamType roleParamType = null)
		{
			if (roleParamType == null)
			{
				roleParamType = RoleParamNameInfo.GetRoleParamType(name, value);
			}
			bool result;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO `{3}` (`rid`, `{4}`, `{5}`) VALUES({0}, {1}, '{2}') ON DUPLICATE KEY UPDATE `{5}`='{2}'", new object[]
				{
					roleID,
					roleParamType.KeyString,
					value,
					roleParamType.TableName,
					roleParamType.IdxName,
					roleParamType.ColumnName
				});
				result = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return result;
		}

		// Token: 0x0600087A RID: 2170 RVA: 0x0004E214 File Offset: 0x0004C414
		public static string GetRoleParams(int roleID, string name)
		{
			RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType(name, null);
			string result;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("SELECT {2} FROM `{3}` WHERE rid={0} AND idx={1}", new object[]
				{
					roleID,
					roleParamType.IdxKey,
					roleParamType.ColumnName,
					roleParamType.TableName
				});
				object ret = conn.GetSingle(cmdText, 0, new MySQLParameter[0]);
				result = ((ret == null) ? "" : ret.ToString());
			}
			return result;
		}

		// Token: 0x0600087B RID: 2171 RVA: 0x0004E2C0 File Offset: 0x0004C4C0
		public static bool UpdateWebOldPlayer(string roleID, string chouJiangType, string addDay)
		{
			bool result;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO `t_weboldplayer` SET rid={0}, {1}=1, addday='{2}' ON DUPLICATE KEY UPDATE {1}=1", roleID, chouJiangType, addDay);
				result = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return result;
		}

		// Token: 0x0600087C RID: 2172 RVA: 0x0004E314 File Offset: 0x0004C514
		public static int AddNewQiangGouBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftMoney, int qiangGouId, int actStartDay)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_qianggoubuy (rid, goodsid, goodsnum, totalprice, leftmoney, buytime, qianggouid, actstartday) VALUES({0}, {1}, {2}, {3}, {4}, '{5}', {6}, {7})", new object[]
				{
					roleID,
					goodsID,
					goodsNum,
					totalPrice,
					leftMoney,
					today,
					qiangGouId,
					actStartDay
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600087D RID: 2173 RVA: 0x0004E3DC File Offset: 0x0004C5DC
		public static int AddNewQiangGouItem(DBManager dbMgr, int group, int random, int itemid, int goodsid, int origprice, int price, int singlepurchase, int fullpurchase, int daystime)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_qianggouitem (itemgroup, random, itemid, goodsid, origprice, price, singlepurchase, fullpurchase, daystime, starttime, endtime, istimeover) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}','{10}', {11})", new object[]
				{
					group,
					random,
					itemid,
					goodsid,
					origprice,
					price,
					singlepurchase,
					fullpurchase,
					daystime,
					today,
					today,
					0
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
				try
				{
					if (ret >= 0)
					{
						ret = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
					}
				}
				catch (MySQLException)
				{
					ret = -2;
				}
			}
			return ret;
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x0004E504 File Offset: 0x0004C704
		public static bool UpdateQiangGouItemTimeOverFlag(DBManager dbMgr, int qiangGouId)
		{
			bool ret = false;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_qianggouitem SET istimeover=1, endtime='{0}' WHERE Id={1}", today, qiangGouId);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600087F RID: 2175 RVA: 0x0004E578 File Offset: 0x0004C778
		public static int AddNewZaJinDanHistory(DBManager dbMgr, int roleID, string roleName, int zoneID, int timesselected, int usedyuanbao, int usedjindan, int gaingoodsid, int gaingoodsnum, int gaingold, int gainyinliang, int gainexp, string srtProp)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_zajindanhist (rid, rname, zoneid, timesselected, usedyuanbao, usedjindan, gaingoodsid, gaingoodsnum, gaingold, gainyinliang, gainexp, strprop, operationtime) VALUES ({0},'{1}',{2},{3},{4},{5},{6},{7},{8},{9},{10},'{11}','{12}')", new object[]
				{
					roleID,
					roleName,
					zoneID,
					timesselected,
					usedyuanbao,
					usedjindan,
					gaingoodsid,
					gaingoodsnum,
					gaingold,
					gainyinliang,
					gainexp,
					srtProp,
					today
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000880 RID: 2176 RVA: 0x0004E670 File Offset: 0x0004C870
		public static int AddKaiFuOnlineAward(DBManager dbMgr, int rid, int dayID, int yuanBao, int totalRoleNum, int zoneID)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_kfonlineawards (rid, dayid, yuanbao, totalrolenum, zoneid) VALUES ({0},{1},{2},{3},{4}) ON DUPLICATE KEY UPDATE rid={0}, yuanbao={2}, totalrolenum={3}", new object[]
				{
					rid,
					dayID,
					yuanBao,
					totalRoleNum,
					zoneID
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000881 RID: 2177 RVA: 0x0004E708 File Offset: 0x0004C908
		public static int AddSystemGiveUserMoney(DBManager dbMgr, int rid, int yuanBao, string giveType)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_givemoney (rid, yuanbao, rectime, givetype) VALUES ({0},{1},'{2}','{3}')", new object[]
				{
					rid,
					yuanBao,
					today,
					giveType
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000882 RID: 2178 RVA: 0x0004E7A0 File Offset: 0x0004C9A0
		public static int AddExchange1Item(DBManager dbMgr, int rid, int goodsid, int goodsnum, int leftgoodsnum, int otherroleid, string result)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_exchange1 (rid, goodsid, goodsnum, leftgoodsnum, otherroleid, result, rectime) VALUES ({0},{1},{2},{3},{4},'{5}','{6}')", new object[]
				{
					rid,
					goodsid,
					goodsnum,
					leftgoodsnum,
					otherroleid,
					result,
					today
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x0004E858 File Offset: 0x0004CA58
		public static int AddExchange2Item(DBManager dbMgr, int rid, int yinliang, int leftyinliang, int otherroleid)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_exchange2 (rid, yinliang, leftyinliang, otherroleid, rectime) VALUES ({0},{1},{2},{3},'{4}')", new object[]
				{
					rid,
					yinliang,
					leftyinliang,
					otherroleid,
					today
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000884 RID: 2180 RVA: 0x0004E900 File Offset: 0x0004CB00
		public static int AddExchange3Item(DBManager dbMgr, int rid, int yuanbao, int leftyuanbao, int otherroleid)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_exchange3 (rid, yuanbao, leftyuanbao, otherroleid, rectime) VALUES ({0},{1},{2},{3},'{4}')", new object[]
				{
					rid,
					yuanbao,
					leftyuanbao,
					otherroleid,
					today
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000885 RID: 2181 RVA: 0x0004E9A8 File Offset: 0x0004CBA8
		public static int AddFallGoodsItem(DBManager dbMgr, int rid, int autoid, int goodsdbid, int goodsid, int goodsnum, int binding, int quality, int forgelevel, string jewellist, string mapname, string goodsgrid, string fromname)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_fallgoods (rid, autoid, goodsdbid, goodsid, goodsnum, binding, quality, forgelevel, jewellist, mapname, goodsgrid, fromname, rectime) VALUES ({0},{1},{2},{3},{4},{5},{6},{7},'{8}','{9}','{10}','{11}','{12}')", new object[]
				{
					rid,
					autoid,
					goodsdbid,
					goodsid,
					goodsnum,
					binding,
					quality,
					forgelevel,
					jewellist,
					mapname,
					goodsgrid,
					fromname,
					today
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000886 RID: 2182 RVA: 0x0004EA98 File Offset: 0x0004CC98
		public static int AddNewYueDuChouJiangHistory(DBManager dbMgr, int roleID, string roleName, int zoneID, int gaingoodsid, int gaingoodsnum, int gaingold, int gainyinliang, int gainexp)
		{
			int ret = -1;
			string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_yueduchoujianghist (rid, rname, zoneid, gaingoodsid, gaingoodsnum, gaingold, gainyinliang, gainexp, operationtime) VALUES ({0},'{1}',{2},{3},{4},{5},{6},{7},'{8}')", new object[]
				{
					roleID,
					roleName,
					zoneID,
					gaingoodsid,
					gaingoodsnum,
					gaingold,
					gainyinliang,
					gainexp,
					today
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000887 RID: 2183 RVA: 0x0004EB68 File Offset: 0x0004CD68
		public static bool UpdateRoleOccupation(DBManager dbMgr, int roleID, int nOccu)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET occupation={0} WHERE rid={1}", nOccu, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x0004EBCC File Offset: 0x0004CDCC
		public static bool UpdateBloodCastleEnterCount(DBManager dbMgr, int nRoleID, int nDate, int nType, int nCount, string lastgettime)
		{
			bool bRet = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_dayactivityinfo(roleid, activityid, timeinfo, triggercount, lastgettime) VALUES({0}, {1}, {2}, {3}, '{4}')", new object[]
				{
					nRoleID,
					nType,
					nDate,
					nCount,
					lastgettime
				});
				bRet = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return bRet;
		}

		// Token: 0x06000889 RID: 2185 RVA: 0x0004EC5C File Offset: 0x0004CE5C
		public static bool UpdateRoleInfoForFlashPlayerFlag(DBManager dbMgr, int nRoleID, int isflashplayer)
		{
			bool bRet = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET isflashplayer={1} WHERE rid={0}", nRoleID, isflashplayer);
				bRet = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return bRet;
		}

		// Token: 0x0600088A RID: 2186 RVA: 0x0004ECC0 File Offset: 0x0004CEC0
		public static bool UpdateRoleExpForFlashPlayerWhenLogOut(DBManager dbMgr, int nRoleID)
		{
			bool bRet = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string strQuickKey = "";
				string cmdText = string.Format("UPDATE t_roles SET experience={1}, maintaskid={1}, main_quick_keys='{2}' WHERE rid={0}", nRoleID, 0, strQuickKey);
				bRet = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return bRet;
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x0004ED2C File Offset: 0x0004CF2C
		public static bool UpdateRoleLevForFlashPlayerWhenLogOut(DBManager dbMgr, int nRoleID)
		{
			bool bRet = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET level={1} WHERE rid={0}", nRoleID, 1);
				bRet = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return bRet;
		}

		// Token: 0x0600088C RID: 2188 RVA: 0x0004ED90 File Offset: 0x0004CF90
		public static bool UpdateRoleGoodsForFlashPlayerWhenLogOut(DBManager dbMgr, int nRoleID)
		{
			bool bRet = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE FROM t_goods WHERE rid = {0}", nRoleID);
				bRet = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return bRet;
		}

		// Token: 0x0600088D RID: 2189 RVA: 0x0004EDF0 File Offset: 0x0004CFF0
		public static bool UpdateRoleTasksForFlashPlayerWhenLogOut(DBManager dbMgr, int nRoleID)
		{
			bool bRet = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE FROM t_tasks WHERE rid = {0}", nRoleID);
				conn.ExecuteNonQuery(cmdText, 0);
				cmdText = string.Format("DELETE FROM t_taskslog WHERE rid = {0}", nRoleID);
				conn.ExecuteNonQuery(cmdText, 0);
				bRet = true;
			}
			return bRet;
		}

		// Token: 0x0600088E RID: 2190 RVA: 0x0004EE6C File Offset: 0x0004D06C
		public static bool UpdateRoleTasksStarLevel(DBManager dbMgr, int nRoleID, int taskid, int StarLevel)
		{
			bool bRet = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_tasks SET starlevel={2} WHERE rid = {0} AND Id = {1}", nRoleID, taskid, StarLevel);
				bRet = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return bRet;
		}

		// Token: 0x0600088F RID: 2191 RVA: 0x0004EED8 File Offset: 0x0004D0D8
		public static bool UpdateRoleChangeLifeInfo(DBManager dbMgr, int nRoleID, int ChangeCount)
		{
			bool bRet = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET changelifecount={1} WHERE rid={0}", nRoleID, ChangeCount);
				bRet = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return bRet;
		}

		// Token: 0x06000890 RID: 2192 RVA: 0x0004EF3C File Offset: 0x0004D13C
		public static bool UpdateRoleAdmiredInfo1(DBManager dbMgr, int nRoleID, int nCount)
		{
			bool bRet = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET admiredcount={1} WHERE rid={0}", nRoleID, nCount);
				bRet = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return bRet;
		}

		// Token: 0x06000891 RID: 2193 RVA: 0x0004EFA0 File Offset: 0x0004D1A0
		public static bool UpdateRoleAdmiredInfo2(DBManager dbMgr, int nRoleID1, int nRoleID2, int nDate)
		{
			bool bRet = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_adorationinfo(roleid, adorationroleid, dayid) VALUES({0}, {1}, {2})", nRoleID1, nRoleID2, nDate);
				bRet = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return bRet;
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x0004F00C File Offset: 0x0004D20C
		public static bool UpdateRoleCombatForce(DBManager dbMgr, int roleID, int CombatForce)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET combatforce={0} WHERE rid={1}", CombatForce, roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000893 RID: 2195 RVA: 0x0004F070 File Offset: 0x0004D270
		public static bool UpdateRoleLevel(DBManager dbMgr, int nRoleID, int lev)
		{
			bool bRet = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET level={1} WHERE rid={0}", nRoleID, lev);
				bRet = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return bRet;
		}

		// Token: 0x06000894 RID: 2196 RVA: 0x0004F0D4 File Offset: 0x0004D2D4
		public static bool UpdateRoleDayActivityPoint(DBManager dbMgr, int nRoleID, int nDate, int nType, int nCount, long nValue)
		{
			bool bRet = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_dayactivityinfo(roleid, activityid, timeinfo, triggercount, totalpoint, lastgettime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')", new object[]
				{
					nRoleID,
					nType,
					nDate,
					nCount,
					nValue,
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
				});
				bRet = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return bRet;
		}

		// Token: 0x06000895 RID: 2197 RVA: 0x0004F180 File Offset: 0x0004D380
		public static bool DeleteRoleDayActivityInfo(DBManager dbMgr, int roleID, int activityid)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE FROM t_dayactivityinfo WHERE roleid = {0} AND activityid = {1}", roleID, activityid);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x06000896 RID: 2198 RVA: 0x0004F1E4 File Offset: 0x0004D3E4
		public static int SetRoleAutoAssignPropertyPoint(DBManager dbMgr, int roleID, int nFlag)
		{
			int ret = 0;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET autoassignpropertypoint = {0} WHERE rid = {1}", nFlag, roleID);
				ret = (conn.ExecuteNonQueryBool(cmdText, 0) ? 1 : 0);
			}
			return ret;
		}

		// Token: 0x06000897 RID: 2199 RVA: 0x0004F250 File Offset: 0x0004D450
		public static int SetUserPushMessageID(DBManager dbMgr, string strUser, string strPushMegID)
		{
			int ret = 0;
			string today = DateTime.Now.ToString("yyyy-MM-dd");
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_pushmessageinfo(userid, pushid, lastlogintime) VALUE('{0}', '{1}', '{2}')", strUser, strPushMegID, today);
				ret = (conn.ExecuteNonQueryBool(cmdText, 0) ? 1 : 0);
			}
			return ret;
		}

		// Token: 0x06000898 RID: 2200 RVA: 0x0004F2C8 File Offset: 0x0004D4C8
		public static int NewWing(DBManager dbMgr, int roleID, int wingID, int forgeLevel, string addtime, string strRoleName, int nOccupation)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_wings (rid, rname, occupation, wingid, forgeLevel, addtime, isdel, failednum, equiped, zhulingnum, zhuhunnum) VALUES({0}, '{1}', {2}, {3}, {4}, '{5}', {6}, {7}, {8}, {9}, {10})", new object[]
				{
					roleID,
					strRoleName,
					nOccupation,
					wingID,
					forgeLevel,
					addtime,
					0,
					0,
					0,
					0,
					0
				});
				bool error = !conn.ExecuteNonQueryBool(cmdText, 0);
				try
				{
					if (!error)
					{
						ret = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
					}
				}
				catch (MySQLException)
				{
					ret = -2;
				}
			}
			return ret;
		}

		// Token: 0x06000899 RID: 2201 RVA: 0x0004F3D0 File Offset: 0x0004D5D0
		public static int UpdateWing(DBManager dbMgr, int id, string[] fields, int startIndex)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = DBWriter.FormatUpdateSQL(id, fields, startIndex, DBWriter._UpdateWing_fieldNames, "t_wings", DBWriter._UpdateWing_fieldTypes, "Id");
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600089A RID: 2202 RVA: 0x0004F43C File Offset: 0x0004D63C
		public static int UpdateLingYu(DBManager dbMgr, int roleID, int type, int level, int suit)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_lingyu(roleid, type, level, suit) VALUES({0}, {1}, {2}, {3})", new object[]
				{
					roleID,
					type,
					level,
					suit
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600089B RID: 2203 RVA: 0x0004F4C8 File Offset: 0x0004D6C8
		public static int UpdateRoleReferPictureJudgeInfo(DBManager dbMgr, int roleid, int nPictureJudgeID, int nNum)
		{
			int ret = 0;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_picturejudgeinfo(roleid, picturejudgeid, refercount) VALUES({0}, {1}, {2})", roleid, nPictureJudgeID, nNum);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600089C RID: 2204 RVA: 0x0004F534 File Offset: 0x0004D734
		public static int UpdateMoJingExchangeDict(DBManager dbMgr, int nRoleid, int nExchangeid, int nDayID, int nNum)
		{
			int ret = 0;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_mojingexchangeinfo(roleid, exchangeid, exchangenum, dayid) VALUES({0}, {1}, {2}, {3})", new object[]
				{
					nRoleid,
					nExchangeid,
					nNum,
					nDayID
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x0004F5C0 File Offset: 0x0004D7C0
		public static int UpdateResourceGetInfo(DBManager dbMgr, int nRoleid, int type, OldResourceInfo info)
		{
			int ret = 0;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText;
				if (info == null)
				{
					cmdText = string.Format("REPLACE INTO t_resourcegetinfo(roleid, type, exp, leftCount,mojing,bandmoney,zhangong,chengjiu,shengwang,bangzuan,xinghun,hasget,yuansufenmo) VALUES({0}, {1}, {2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12})", new object[]
					{
						nRoleid,
						type,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						1,
						0
					});
				}
				else
				{
					cmdText = string.Format("REPLACE INTO t_resourcegetinfo(roleid, type, exp, leftCount,mojing,bandmoney,zhangong,chengjiu,shengwang,bangzuan,xinghun,hasget,yuansufenmo) VALUES({0}, {1}, {2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12})", new object[]
					{
						nRoleid,
						info.type,
						info.exp,
						info.leftCount,
						info.mojing,
						info.bandmoney,
						info.zhangong,
						info.chengjiu,
						info.shengwang,
						info.bandDiamond,
						info.xinghun,
						0,
						info.yuanSuFenMo
					});
				}
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x0004F7A8 File Offset: 0x0004D9A8
		public static int UpdateGoods2(DBManager dbMgr, int roleID, GoodsData gd, UpdateGoodsArgs args)
		{
			int result = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				bool update = false;
				StringBuilder sb = new StringBuilder("UPDATE t_goods SET ");
				foreach (UpdatePropIndexes idx in args.ChangedIndexes)
				{
					if (idx == UpdatePropIndexes.WashProps)
					{
						if (update)
						{
							sb.Append(", ");
						}
						string washPropsStr = Convert.ToBase64String(DataHelper.ObjectToBytes<List<int>>(args.WashProps));
						sb.AppendFormat("{0}='{1}'", "washprops", washPropsStr);
						update = true;
					}
					else if (idx == UpdatePropIndexes.ElementhrtsProps)
					{
						if (update)
						{
							sb.Append(", ");
						}
						string elementThrtsPropsStr = Convert.ToBase64String(DataHelper.ObjectToBytes<List<int>>(args.ElementhrtsProps));
						sb.AppendFormat("{0}='{1}'", "ehinfo", elementThrtsPropsStr);
						update = true;
					}
					else if (idx == UpdatePropIndexes.JuHun)
					{
						if (update)
						{
							sb.Append(", ");
						}
						sb.AppendFormat("{0}='{1}'", "juhun", args.JuHunProps);
						update = true;
					}
					else if (idx < UpdatePropIndexes.MaxBaseIndex)
					{
						if (idx == UpdatePropIndexes.binding)
						{
							DBWriter.AppendSQLForGoodsProps(sb, (int)idx, args.Binding, DBWriter._UpdateGoods2_fieldNames, DBWriter._UpdateGoods2_fieldTypes, update);
							update = true;
						}
					}
				}
				if (update)
				{
					sb.AppendFormat(" WHERE {0}={1} and rid={2}", "Id", args.DbID, args.RoleID);
					string sql = sb.ToString();
					result = conn.ExecuteNonQuery(sql, 0);
					if (result >= 0)
					{
						args.CopyPropsTo(gd);
					}
				}
			}
			return result;
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x0004F9F0 File Offset: 0x0004DBF0
		public static void AppendSQLForGoodsProps(StringBuilder sb, int index, object value, string[] fieldNames, byte[] fieldTypes, bool hasAppend)
		{
			if (hasAppend)
			{
				sb.Append(", ");
			}
			if (0 == fieldTypes[index])
			{
				sb.AppendFormat("{0}={1}", fieldNames[index], value);
			}
			else if (1 == fieldTypes[index])
			{
				sb.AppendFormat("{0}='{1}'", fieldNames[index], value);
			}
			else if (2 == fieldTypes[index])
			{
				sb.AppendFormat("{0}={0}+{1}", fieldNames[index], value);
			}
			else if (3 == fieldTypes[index])
			{
				sb.AppendFormat("{0}='{1}'", fieldNames[index], value.ToString().Replace('$', ':'));
			}
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x0004FAAC File Offset: 0x0004DCAC
		public static int UpdateRoleStarConstellationInfo(DBManager dbMgr, int roleid, int nStarSite, int nStarSlot)
		{
			int ret = 0;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_starconstellationinfo(roleid, starsiteid, starslotid) VALUES({0}, {1}, {2})", roleid, nStarSite, nStarSlot);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x0004FB18 File Offset: 0x0004DD18
		public static int SaveConsumeLog(DBManager dbMgr, int roleid, string cdate, int amount)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_consumelog (rid, amount, cdate) VALUES({0}, {1}, '{2}') ON DUPLICATE KEY UPDATE amount=amount+{1};", roleid, amount, cdate);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x0004FB7C File Offset: 0x0004DD7C
		public static bool UpdateVipLevelAwardFlagInfo(DBManager dbMgr, string strUserid, int nFlag, int nZoneID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET vipawardflag={0} WHERE userid='{1}' and zoneid = {2}", nFlag, strUserid, nZoneID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x0004FBE0 File Offset: 0x0004DDE0
		public static bool UpdateVipLevelAwardFlagInfoByRoleID(DBManager dbMgr, int nRoleid, int nFlag, int nZoneID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_roles SET vipawardflag={0} WHERE rid={1} and zoneid = {2}", nFlag, nRoleid, nZoneID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x0004FC4C File Offset: 0x0004DE4C
		public static int ExecuteSQLReadInt(DBManager dbMgr, string sqlText, MySQLConnection conn = null)
		{
			int result = 0;
			bool keepConn = true;
			try
			{
				if (conn == null)
				{
					keepConn = false;
					conn = dbMgr.DBConns.PopDBConnection();
				}
				MySQLCommand mySQLCommand;
				MySQLCommand cmd = mySQLCommand = new MySQLCommand(sqlText, conn);
				try
				{
					MySQLDataReader reader = cmd.ExecuteReaderEx();
					if (reader.Read())
					{
						result = Convert.ToInt32(reader[0].ToString());
					}
					reader.Close();
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
					return -1;
				}
				finally
				{
					if (mySQLCommand != null)
					{
						((IDisposable)mySQLCommand).Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return -2;
			}
			finally
			{
				if (!keepConn && null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return result;
		}

		// Token: 0x060008A5 RID: 2213 RVA: 0x0004FD6C File Offset: 0x0004DF6C
		public static int ValidateDatabase(DBManager dbMgr, string dbName)
		{
			MySQLConnection conn = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				int flag_t_roles_auto_increment = GameDBManager.GameConfigMgr.GetGameConfigItemInt("flag_t_roles_auto_increment", 0);
				if (flag_t_roles_auto_increment < 200000 || flag_t_roles_auto_increment % 100000 != 0)
				{
					Global.LogAndExitProcess("flag_t_roles_auto_increment 未设置");
				}
				int t_roles_auto_increment = 0;
				string[] ips = Global.GetLocalAddressIPs().Split(new char[]
				{
					'_'
				});
				foreach (string ip in ips)
				{
					int idx = ip.IndexOf('.');
					if (idx > 0)
					{
						idx = ip.IndexOf('.', idx + 1);
						if (idx > 0)
						{
							string ipPrefix = ip.Substring(0, idx);
							if (GameDBManager.IPRange2AutoIncreaseStepDict.TryGetValue(ipPrefix, out t_roles_auto_increment))
							{
								break;
							}
						}
					}
				}
				if (t_roles_auto_increment > 0)
				{
					if (t_roles_auto_increment != flag_t_roles_auto_increment)
					{
						Global.LogAndExitProcess("flag_t_roles_auto_increment 不正确");
					}
				}
				else if (t_roles_auto_increment != 0 && 200000 != flag_t_roles_auto_increment)
				{
					Global.LogAndExitProcess("flag_t_roles_auto_increment 不正确");
				}
				GameDBManager.DBAutoIncreaseStepValue = flag_t_roles_auto_increment;
				foreach (string[] item in DBWriter.ValidateDatabaseTables)
				{
					string sqlText = string.Format("SELECT COUNT(*) FROM information_schema.columns WHERE table_schema='{0}' AND table_name = '{1}' AND column_name='{2}' limit 1;", dbName, item[0], item[1]);
					int result = DBWriter.ExecuteSQLReadInt(dbMgr, sqlText, conn);
					if (result <= 0)
					{
						Global.LogAndExitProcess(string.Format("表 '{0}' 不存在或缺少列：'{1}'", item[0], item[1]));
					}
				}
				foreach (string[] item in DBWriter.ValidateDatabaseTableRows)
				{
					string sqlText = string.Format("SELECT COUNT(*) FROM {0} {1} limit 1;", item[0], item[1]);
					int result = DBWriter.ExecuteSQLReadInt(dbMgr, sqlText, conn);
					if (result <= 0)
					{
						Global.LogAndExitProcess(string.Format("表 '{0}' 缺少：{1}", item[0], item[2]));
					}
				}
				foreach (string[] item in DBWriter.ValidateDatabaseTableIndexes)
				{
					string sqlText = string.Format("SELECT COUNT(*) FROM information_schema.statistics WHERE table_schema='{0}' AND TABLE_NAME='{1}' AND INDEX_NAME='{2}' limit 1;", dbName, item[0], item[1]);
					int result = DBWriter.ExecuteSQLReadInt(dbMgr, sqlText, conn);
					if (result <= 0)
					{
						Global.LogAndExitProcess(string.Format("表 '{0}' 不存在或缺少索引：'{1}'", item[0], item[1]));
					}
				}
				DBWriter.SwitchGoodsBackupTable(dbMgr);
				DBWriter.ClearOverdueTianTiItemLog(dbMgr, TimeUtil.NowDateTime().AddMonths(-3));
				DBWriter.ClearBigTable_NameCheck(dbMgr);
				if (!DBWriter.CheckMoneyCC(dbMgr, dbName, conn))
				{
					return 0;
				}
			}
			catch (MySQLException ex)
			{
				LogManager.WriteException(string.Format("检查数据库是否正确时发生异常: {0}", ex.ToString()));
				throw new Exception(string.Format("检查数据库是否正确时发生异常: {0}", ex.ToString()));
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return 1;
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x000500C8 File Offset: 0x0004E2C8
		private static bool CheckTableColumn(DBManager dbMgr, string dbName, string tableName, string columnName, MySQLConnection conn = null)
		{
			string sqlText = string.Format("SELECT COUNT(*) FROM information_schema.columns WHERE table_schema='{0}' AND table_name = '{1}' AND column_name='{2}' limit 1;", dbName, tableName, columnName);
			int result = DBWriter.ExecuteSQLReadInt(dbMgr, sqlText, conn);
			return result > 0;
		}

		// Token: 0x060008A7 RID: 2215 RVA: 0x000500F8 File Offset: 0x0004E2F8
		public static bool CheckMoneyCC(DBManager dbMgr, string dbName, MySQLConnection conn = null)
		{
			if (!DBWriter.CheckTableColumn(dbMgr, dbName, "t_money", "cc", conn))
			{
				if (0 > DBWriter.ExecuteSQLNoQuery(dbMgr, "ALTER TABLE `t_money` ADD COLUMN `cc` CHAR(64) CHARSET utf8 COLLATE utf8_general_ci NULL AFTER `realmoney`;", conn))
				{
					Global.LogAndExitProcess(string.Format("表 '{0}' 不存在或缺少索引：'{1}'", "t_money", "cc"));
					return false;
				}
				string cmdText = string.Format("SELECT userid, money, realmoney,cc FROM t_money", new object[0]);
				using (MySQLCommand cmd = new MySQLCommand(cmdText, conn))
				{
					using (MySQLDataReader reader = cmd.ExecuteReaderEx())
					{
						while (reader.Read())
						{
							string userId = reader["userid"].ToString();
							int userMoney = Convert.ToInt32(reader["money"].ToString());
							int realMoney = Convert.ToInt32(reader["realmoney"].ToString());
							string c0 = reader["cc"].ToString();
							if (string.IsNullOrEmpty(c0))
							{
								string cc = Global.GCC(3, new object[]
								{
									userId,
									userMoney,
									realMoney
								});
								cmdText = string.Format("update t_money set cc='{0}' where userid='{1}';", cc, userId);
								if (DBWriter.ExecuteSQLNoQuery(dbMgr, cmdText, conn) >= 0)
								{
									LogManager.WriteLog(LogTypes.DataCheck, string.Format("DataCorrect#t_money#userid={0},money={1},cc={2}", userId, userMoney, c0), null, true);
									GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
								}
							}
						}
					}
				}
			}
			if (!DBWriter.CheckTableColumn(dbMgr, dbName, "t_tempmoney", "cc", conn))
			{
				if (0 > DBWriter.ExecuteSQLNoQuery(dbMgr, "ALTER TABLE `t_tempmoney` ADD COLUMN `cc` CHAR(64) CHARSET utf8 COLLATE utf8_general_ci NULL AFTER `id`;", conn))
				{
					Global.LogAndExitProcess(string.Format("表 '{0}' 不存在或缺少索引：'{1}'", "t_tempmoney", "cc"));
					return false;
				}
			}
			return true;
		}

		// Token: 0x060008A8 RID: 2216 RVA: 0x00050338 File Offset: 0x0004E538
		public static int ClearUnusedGoodsData(DBManager dbMgr, bool clearAll = false)
		{
			int maxGoodsDBID = -1;
			int minGoodsDBID = -1;
			MySQLConnection conn = null;
			try
			{
				int toClearDBID = 0;
				conn = dbMgr.DBConns.PopDBConnection();
				try
				{
					MySQLCommand mySQLCommand;
					MySQLCommand cmd = mySQLCommand = new MySQLCommand("SELECT MAX(id) FROM t_goods", conn);
					try
					{
						MySQLDataReader reader = cmd.ExecuteReaderEx();
						if (reader.Read())
						{
							maxGoodsDBID = Convert.ToInt32(reader[0].ToString());
						}
						reader.Close();
					}
					catch
					{
						return -1;
					}
					finally
					{
						if (mySQLCommand != null)
						{
							((IDisposable)mySQLCommand).Dispose();
						}
					}
					cmd = (mySQLCommand = new MySQLCommand("SELECT MIN(id) FROM t_goods", conn));
					try
					{
						MySQLDataReader reader = cmd.ExecuteReaderEx();
						if (reader.Read())
						{
							minGoodsDBID = Convert.ToInt32(reader[0].ToString());
						}
						reader.Close();
					}
					catch
					{
						return -1;
					}
					finally
					{
						if (mySQLCommand != null)
						{
							((IDisposable)mySQLCommand).Dispose();
						}
					}
				}
				catch (MySQLException ex)
				{
					LogManager.WriteException(string.Format("清理t_goods表时异常: {0}", ex.ToString()));
					return -1;
				}
				try
				{
					int last_goods_dbid = GameDBManager.GameConfigMgr.GetGameConfigItemInt("last_clear_goods_dbid", 0);
					if (last_goods_dbid > minGoodsDBID)
					{
						minGoodsDBID = last_goods_dbid;
					}
					int max_clear_goods_count = GameDBManager.GameConfigMgr.GetGameConfigItemInt("max_clear_goods_count", 1);
					toClearDBID = minGoodsDBID + max_clear_goods_count;
					if (maxGoodsDBID < toClearDBID)
					{
						return 0;
					}
					if (clearAll)
					{
						toClearDBID = maxGoodsDBID;
					}
					GameDBManager.GameConfigMgr.UpdateGameConfigItem("last_goods_dbid", toClearDBID.ToString());
					DBWriter.UpdateGameConfig(dbMgr, "last_goods_dbid", toClearDBID.ToString());
					string cmdText = string.Format("INSERT INTO t_goods_bak SELECT *,0,NOW(),0 FROM t_goods WHERE gcount <= 0 AND id > {0} AND id <= {1}", minGoodsDBID, toClearDBID);
					MySQLCommand mySQLCommand;
					MySQLCommand cmd = mySQLCommand = new MySQLCommand(cmdText, conn);
					try
					{
						cmd.ExecuteNonQuery();
					}
					finally
					{
						if (mySQLCommand != null)
						{
							((IDisposable)mySQLCommand).Dispose();
						}
					}
					cmdText = string.Format("DELETE FROM t_goods WHERE gcount <= 0 AND id > {0} AND id <= {1}", minGoodsDBID, toClearDBID);
					cmd = (mySQLCommand = new MySQLCommand(cmdText, conn));
					try
					{
						cmd.ExecuteNonQuery();
					}
					finally
					{
						if (mySQLCommand != null)
						{
							((IDisposable)mySQLCommand).Dispose();
						}
					}
				}
				catch (MySQLException ex)
				{
					LogManager.WriteException(string.Format("清理t_goods表时异常: {0}", ex.ToString()));
					return -1;
				}
			}
			catch (Exception ex2)
			{
				LogManager.WriteException(string.Format("清理t_goods表时异常: {0}", ex2.ToString()));
				return -1;
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return 1;
		}

		// Token: 0x060008A9 RID: 2217 RVA: 0x000506DC File Offset: 0x0004E8DC
		public static int ClearBigTable_NameCheck(DBManager dbMgr)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE FROM t_name_check;", new object[0]);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008AA RID: 2218 RVA: 0x0005073C File Offset: 0x0004E93C
		public static int insertItemLog(DBManager dbMgr, string[] fields)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string strTableName = "t_usemoney_log";
				string cmdText = string.Format("INSERT INTO {0} (DBId, userid, ObjName, optFrom, currEnvName, tarEnvName, optType, optTime, optAmount, zoneID, optSurplus) VALUES({1}, '{9}', '{2}', '{3}', '{4}', '{5}', '{6}', now(), {7}, {8}, {10})", new object[]
				{
					strTableName,
					fields[0],
					fields[1],
					fields[2],
					fields[3],
					fields[4],
					fields[5],
					fields[6],
					fields[7],
					fields[8],
					fields[9]
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008AB RID: 2219 RVA: 0x000507F4 File Offset: 0x0004E9F4
		public static int UpdateRoleKuaFuDayLog(DBManager dbMgr, RoleKuaFuDayLogData data)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_kf_day_role_log (gametype,day,rid,zoneid,signup_count, start_game_count, success_count, faild_count) VALUES({0},'{1}',{2},{3},{4},{5},{6},{7}) on duplicate key update zoneid={3},signup_count=signup_count+{4},start_game_count=start_game_count+{5},success_count=success_count+{6},faild_count=faild_count+{7}", new object[]
				{
					data.GameType,
					data.Day,
					data.RoleID,
					data.ZoneId,
					data.SignupCount,
					data.StartGameCount,
					data.SuccessCount,
					data.FaildCount
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008AC RID: 2220 RVA: 0x000508C8 File Offset: 0x0004EAC8
		public static int InsertTianTiItemLog(DBManager dbMgr, TianTiLogItemData data)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string strTableName = "t_kf_tianti_game_log";
				string cmdText = string.Format("INSERT INTO {0} (rid,zoneid1,rolename1,zoneid2,rolename2,success,duanweijifenaward,rongyaoaward,endtime) VALUES({1},{2},'{3}',{4},'{5}',{6},{7},{8},'{9}')", new object[]
				{
					strTableName,
					data.RoleId,
					data.ZoneId1,
					data.RoleName1,
					data.ZoneId2,
					data.RoleName2,
					data.Success,
					data.DuanWeiJiFenAward,
					data.RongYaoAward,
					data.EndTime.ToString("yyyy-MM-dd HH:mm:ss")
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008AD RID: 2221 RVA: 0x000509BC File Offset: 0x0004EBBC
		public static int InsertKF5v5ItemLog(DBManager dbMgr, TianTiLogItemData data)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string strTableName = "t_5v5_game_log";
				string cmdText = string.Format("INSERT INTO {0} (rid,zoneid1,rolename1,zoneid2,rolename2,success,duanweijifenaward,rongyaoaward,endtime) VALUES({1},{2},'{3}',{4},'{5}',{6},{7},{8},'{9}')", new object[]
				{
					strTableName,
					data.RoleId,
					data.ZoneId1,
					data.RoleName1,
					data.ZoneId2,
					data.RoleName2,
					data.Success,
					data.DuanWeiJiFenAward,
					data.RongYaoAward,
					data.EndTime.ToString("yyyy-MM-dd HH:mm:ss")
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008AE RID: 2222 RVA: 0x00050AB0 File Offset: 0x0004ECB0
		public static int ClearOverdueTianTiItemLog(DBManager dbMgr, DateTime minTime)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE FROM t_kf_tianti_game_log WHERE endtime < '{0}'", minTime.ToString("yyyy-MM-dd"));
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008AF RID: 2223 RVA: 0x00050B14 File Offset: 0x0004ED14
		public static int UpdateTianTiRoleData(DBManager dbMgr, RoleTianTiData data)
		{
			int result;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_kf_tianti_role (rid,duanweiid,duanweijifen,duanweirank,liansheng,fightcount,successcount,todayfightcount,lastfightdayid,monthduanweirank,fetchmonthawarddate,rongyao) VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},'{10}',{11}) ", new object[]
				{
					data.RoleId,
					data.DuanWeiId,
					data.DuanWeiJiFen,
					data.DuanWeiRank,
					data.LianSheng,
					data.FightCount,
					data.SuccessCount,
					data.TodayFightCount,
					data.LastFightDayId,
					data.MonthDuanWeiRank,
					data.FetchMonthDuanWeiRankAwardsTime.ToString("yyyy-MM-dd"),
					data.RongYao
				});
				result = conn.ExecuteNonQuery(cmdText, 0);
			}
			return result;
		}

		// Token: 0x060008B0 RID: 2224 RVA: 0x00050C20 File Offset: 0x0004EE20
		public static int UpdateTianTiRoleRongYao(DBManager dbMgr, int rid, int rongYao)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_kf_tianti_role SET rongyao={1} WHERE rid={0};", rid, rongYao);
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008B1 RID: 2225 RVA: 0x00050C84 File Offset: 0x0004EE84
		public static void InsertCityInfo(DBManager dbMgr, string ip, string userid)
		{
			IPInfo ipInfo = CityNameManager.ParseIP(ip);
			if (null != ipInfo)
			{
				string region = ipInfo.RegionName;
				string cityName = ipInfo.CityName;
				int dayID = Global.GetOffsetDay(DateTime.Now);
				string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText = string.Format("INSERT INTO t_cityinfo (userid, dayid, region, cityname, lastip, starttime, logouttime) VALUES('{0}', {1}, '{2}', '{3}', '{4}', '{5}', '{6}') ON DUPLICATE KEY UPDATE lastip='{4}'", new object[]
					{
						userid,
						dayID,
						region,
						cityName,
						ip,
						today,
						Global.GetDayEndTime(DateTime.Now)
					});
					conn.ExecuteNonQuery(cmdText, 0);
				}
			}
		}

		// Token: 0x060008B2 RID: 2226 RVA: 0x00050D60 File Offset: 0x0004EF60
		public static void UpdateCityInfoItem(DBManager dbMgr, string ip, string userid, string fieldName, int addVal)
		{
			IPInfo ipInfo = CityNameManager.ParseIP(ip);
			if (null != ipInfo)
			{
				string region = ipInfo.RegionName;
				string cityName = ipInfo.CityName;
				int dayID = Global.GetOffsetDay(DateTime.Now);
				string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText = string.Format("INSERT INTO t_cityinfo (userid, dayid, region, cityname, lastip, starttime, logouttime, {7}) VALUES('{0}', {1}, '{2}', '{3}', '{4}', '{5}', '{6}', {8}) ON DUPLICATE KEY UPDATE lastip='{4}', {7}={7}+{8}", new object[]
					{
						userid,
						dayID,
						region,
						cityName,
						ip,
						Global.GetDayStartTime(DateTime.Now),
						Global.GetDayEndTime(DateTime.Now),
						fieldName,
						addVal
					});
					conn.ExecuteNonQuery(cmdText, 0);
				}
			}
		}

		// Token: 0x060008B3 RID: 2227 RVA: 0x00050E54 File Offset: 0x0004F054
		public static void UpdateCityInfoLogoutTime(DBManager dbMgr, string ip, string userid, int onlineSecs, int activeVal)
		{
			IPInfo ipInfo = CityNameManager.ParseIP(ip);
			if (null != ipInfo)
			{
				string region = ipInfo.RegionName;
				string cityName = ipInfo.CityName;
				int dayID = Global.GetOffsetDay(DateTime.Now);
				string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText = string.Format("INSERT INTO t_cityinfo (userid, dayid, region, cityname, lastip, starttime, logouttime, onlinesecs, activeval) VALUES('{0}', {1}, '{2}', '{3}', '{4}', '{5}', '{6}', {7}, {8}) ON DUPLICATE KEY UPDATE logouttime='{6}', onlinesecs=onlinesecs+{7}, activeval=activeval+{8}", new object[]
					{
						userid,
						dayID,
						region,
						cityName,
						ip,
						Global.GetDayStartTime(DateTime.Now),
						today,
						onlineSecs,
						activeVal
					});
					conn.ExecuteNonQuery(cmdText, 0);
				}
			}
		}

		// Token: 0x060008B4 RID: 2228 RVA: 0x00050F44 File Offset: 0x0004F144
		public static bool UpdateUsrSecondPassword(DBManager dbMgr, string usrid, string secPwd)
		{
			bool result;
			if (string.IsNullOrEmpty(usrid))
			{
				result = false;
			}
			else
			{
				bool ret = false;
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText;
					if (!string.IsNullOrEmpty(secPwd))
					{
						cmdText = string.Format("REPLACE INTO t_secondpassword(userid, secpwd) VALUES('{0}','{1}')", usrid, secPwd);
					}
					else
					{
						cmdText = string.Format("DELETE FROM t_secondpassword WHERE userid='{0}'", usrid);
					}
					ret = conn.ExecuteNonQueryBool(cmdText, 0);
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x060008B5 RID: 2229 RVA: 0x00050FD8 File Offset: 0x0004F1D8
		public static bool UpdateMarriageData(DBManager dbMgr, int nRoleID, int nSpouseID, sbyte byMarrytype, int nRingID, int nGoodwillexp, sbyte byGoodwillStar, sbyte byGoodwilllevel, int nGivenrose, string strLovemessage, sbyte byAutoReject, string changtime)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_marry(roleid, spouseid, marrytype, ringid, goodwillexp, goodwillstar, goodwilllevel, givenrose, lovemessage, autoreject, changtime) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}', {9}, '{10}')", new object[]
				{
					nRoleID,
					nSpouseID,
					byMarrytype,
					nRingID,
					nGoodwillexp,
					byGoodwillStar,
					byGoodwilllevel,
					nGivenrose,
					strLovemessage,
					byAutoReject,
					changtime
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008B6 RID: 2230 RVA: 0x000510A8 File Offset: 0x0004F2A8
		public static bool AddMarryParty(DBManager dbMgr, int roleID, int partyType, string startTime, int husbandRoleID, int wifeRoleID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_marryparty (roleid, partytype, joinCount, startTime, husbandid, wifeid) VALUES({0}, {1}, {2}, '{3}', {4}, {5})", new object[]
				{
					roleID,
					partyType,
					0,
					startTime,
					husbandRoleID,
					wifeRoleID
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008B7 RID: 2231 RVA: 0x00051144 File Offset: 0x0004F344
		public static bool RemoveMarryParty(DBManager dbMgr, int roleID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE FROM t_marryparty WHERE roleid={0}", roleID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008B8 RID: 2232 RVA: 0x000511A4 File Offset: 0x0004F3A4
		public static bool IncMarryPartyJoin(DBManager dbMgr, int roleID, int joinerID, int joinCount)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_marryparty SET joincount=joincount+1 WHERE roleid={0}", roleID);
				if (conn.ExecuteNonQueryBool(cmdText, 0))
				{
					ret = true;
				}
				cmdText = string.Format("REPLACE INTO t_marryparty_join(roleid, partyroleid, joincount) VALUES({0}, {1}, {2})", joinerID, roleID, joinCount);
				if (conn.ExecuteNonQueryBool(cmdText, 0))
				{
					ret = true;
				}
			}
			return ret;
		}

		// Token: 0x060008B9 RID: 2233 RVA: 0x0005123C File Offset: 0x0004F43C
		public static bool ClearMarryPartyJoin(DBManager dbMgr, int roleID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText;
				if (roleID <= 0)
				{
					cmdText = string.Format("delete from t_marryparty_join", new object[0]);
				}
				else
				{
					cmdText = string.Format("delete from t_marryparty_join WHERE roleid = {0}", roleID);
				}
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008BA RID: 2234 RVA: 0x000512BC File Offset: 0x0004F4BC
		public static bool DeleteChargeItemInfo(DBManager dbMgr, int id, byte isDel)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_tempitem SET isdel = {0} WHERE id = {1}", isDel, id);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008BB RID: 2235 RVA: 0x00051320 File Offset: 0x0004F520
		public static int ModifyGMailRecord(DBManager dbMgr, int roleID, int gmailID, int mailID)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string strTableName = "t_rolegmail_record";
				string cmdText = string.Format("REPLACE INTO {0} (roleid, gmailid, mailid) VALUES({1}, {2}, {3})", new object[]
				{
					strTableName,
					roleID,
					gmailID,
					mailID
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008BC RID: 2236 RVA: 0x000513AC File Offset: 0x0004F5AC
		public static bool UpdateHolyItemData(DBManager dbMgr, int nRoleID, sbyte sShengwu_type, sbyte sPart_slot, sbyte sPart_suit, int nPart_slice, int nFail_count)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_holyitem(roleid, shengwu_type, part_slot, part_suit, part_slice, fail_count) VALUES({0}, {1}, {2}, {3}, {4}, {5})", new object[]
				{
					nRoleID,
					sShengwu_type,
					sPart_slot,
					sPart_suit,
					nPart_slice,
					nFail_count
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x0005144C File Offset: 0x0004F64C
		public static bool UpdateTarotData(DBManager dbMgr, int nRoleID, string tarotInfo, string kingInfo)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_tarot(roleid, tarotinfo, kingbuff) VALUES({0}, '{1}','{2}')", nRoleID, tarotInfo, kingInfo);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x000514AC File Offset: 0x0004F6AC
		public static bool UpdateTenState(DBManager dbMgr, int id, int state)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_ten SET state={0} WHERE id={1}", state, id);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x00051510 File Offset: 0x0004F710
		public static bool ActivateStateSet(DBManager dbMgr, int zoneID, string userID, int roleID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_activate (userID, zoneID, roleID, logTime) VALUES('{0}', {1}, {2}, '{3}')", new object[]
				{
					userID,
					zoneID,
					roleID,
					DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008C0 RID: 2240 RVA: 0x000515A4 File Offset: 0x0004F7A4
		public static bool UpdateGiftCodeState(DBManager dbMgr, int id, int mailid, string time)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("UPDATE t_giftcode SET mailid={0},usetime='{1}' WHERE id={2}", mailid, time, id);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008C1 RID: 2241 RVA: 0x00051608 File Offset: 0x0004F808
		public static int InsertChargeTempItem(DBManager dbMgr, string UserID, int chargeRoleID, int addUserMoney, int zhigouID, string chargeTm)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("INSERT INTO t_tempitem (uid, rid, addmoney, itemid, chargetime) VALUES('{0}', {1}, {2}, {3}, '{4}')", new object[]
				{
					UserID,
					chargeRoleID,
					addUserMoney,
					zhigouID,
					chargeTm
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008C2 RID: 2242 RVA: 0x00051694 File Offset: 0x0004F894
		public static bool Insert2OrderNo(DBManager dbMgr, string order_no)
		{
			MySQLConnection conn = null;
			bool error = false;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format("INSERT INTO `t_order` (`order_no`) VALUES ('{0}');", order_no);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				try
				{
					cmd.ExecuteNonQuery();
					error = true;
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText), null, true);
				}
				cmd.Dispose();
				cmd = null;
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return error;
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x00051754 File Offset: 0x0004F954
		public static bool Insert2InputLog(DBManager dbMgr, RechargeData rechargeData)
		{
			bool result;
			if (rechargeData.BudanFlag == 2)
			{
				rechargeData.RoleID = -1;
				result = true;
			}
			else
			{
				MySQLConnection conn = null;
				bool error = false;
				try
				{
					conn = dbMgr.DBConns.PopDBConnection();
					string cmdText = string.Format("INSERT INTO `{12}` (`amount`, `u`, `rid`, `order_no`, `cporder_no`, \r\n                    `time`, `sign`, `inputtime`, `result`, `zoneid`, `itemid`, `chargetime`) \r\n                    VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', \r\n                    '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');", new object[]
					{
						rechargeData.Amount,
						rechargeData.UserID,
						rechargeData.RoleID,
						rechargeData.order_no,
						rechargeData.cporder_no,
						rechargeData.Time,
						rechargeData.Sign,
						rechargeData.ChargeTime,
						"success",
						rechargeData.ZoneID,
						rechargeData.ItemId,
						rechargeData.ChargeTime,
						(rechargeData.BudanFlag != 1) ? "t_inputlog" : "t_inputlog2"
					});
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
					MySQLCommand cmd = new MySQLCommand(cmdText, conn);
					try
					{
						cmd.ExecuteNonQuery();
						error = true;
					}
					catch (Exception)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText), null, true);
					}
					cmd.Dispose();
					cmd = null;
				}
				finally
				{
					if (null != conn)
					{
						dbMgr.DBConns.PushDBConnection(conn);
					}
				}
				result = error;
			}
			return result;
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x00051904 File Offset: 0x0004FB04
		public static bool Insert2TempMoney(DBManager dbMgr, TempMoneyInfo info)
		{
			MySQLConnection conn = null;
			bool error = false;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format("INSERT INTO `t_tempmoney` (`cc`, `uid`, `rid`, `addmoney`, `itemid`, `budanflag`, `chargetime`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');", new object[]
				{
					info.cc,
					info.userID,
					info.chargeRoleID,
					info.addUserMoney,
					info.addUserItem,
					info.budanflag,
					info.chargeTm
				});
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				try
				{
					cmd.ExecuteNonQuery();
					error = true;
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText), null, true);
				}
				cmd.Dispose();
				cmd = null;
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return error;
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x00051A24 File Offset: 0x0004FC24
		public static bool UpdateFacebookState(DBManager dbMgr, int id, int state)
		{
			bool ret = false;
			MySQLConnection conn = null;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format("UPDATE t_facebook SET state={0} WHERE id={1}", state, id);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				try
				{
					cmd.ExecuteNonQuery();
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText), null, true);
				}
				cmd.Dispose();
				cmd = null;
				ret = true;
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return ret;
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x00051AEC File Offset: 0x0004FCEC
		public static bool InsertFoMoMailData(DBManager dbMgr, int sendid, string sendername, int sendjob, int recrid, int num, string content, string today)
		{
			MySQLConnection conn = null;
			bool error = false;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format(SqlDefineManager.InsertOneMail, new object[]
				{
					sendid,
					sendername,
					sendjob,
					today,
					recrid,
					0,
					"2000-11-11 11:11:11",
					num,
					content
				});
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				try
				{
					cmd.ExecuteNonQuery();
					error = true;
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText), null, true);
				}
				cmd.Dispose();
				cmd = null;
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return error;
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x00051C04 File Offset: 0x0004FE04
		public static bool UpdateIsReadFoMoMailData(DBManager dbMgr, int mailid, string today)
		{
			MySQLConnection conn = null;
			bool error = false;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format(SqlDefineManager.UpdateReadState, 1, today, mailid);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				try
				{
					cmd.ExecuteNonQuery();
					error = true;
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText), null, true);
				}
				cmd.Dispose();
				cmd = null;
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return error;
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x00051CD0 File Offset: 0x0004FED0
		public static bool InsertFoMoMailDataTemp(DBManager dbMgr, int sendid, string recrid_list, int nDate, int accept, int give)
		{
			MySQLConnection conn = null;
			bool error = false;
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format(SqlDefineManager.InsertMapList, new object[]
				{
					nDate,
					sendid,
					recrid_list,
					accept,
					give
				});
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				try
				{
					cmd.ExecuteNonQuery();
					error = true;
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText), null, true);
				}
				cmd.Dispose();
				cmd = null;
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return error;
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x00051DC8 File Offset: 0x0004FFC8
		public static bool UpdateRoleStoreFuMoMoneyAcceptNum(DBManager dbMgr, int roleID, int nDate, int acceptnum)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.UpdateMapAccept, acceptnum, roleID, nDate);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x00051E34 File Offset: 0x00050034
		public static bool UpdateRoleStoreFuMoMoneyGiveNum(DBManager dbMgr, int roleID, int givenum, int nDate, string recid_list)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.UpdateMapGive, new object[]
				{
					givenum,
					recid_list,
					roleID,
					nDate
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008CB RID: 2251 RVA: 0x00051EBC File Offset: 0x000500BC
		public static bool DeleteMailFuMoByMailID(DBManager dbMgr, int mailID)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.DeleteMailByID, mailID);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008CC RID: 2252 RVA: 0x00051F1C File Offset: 0x0005011C
		public static bool DeleteMailFuMoByMailIDList(DBManager dbMgr, int rid, string mailIDList)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.DeleteMailByIDList, mailIDList, rid);
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008CD RID: 2253 RVA: 0x00051F7C File Offset: 0x0005017C
		public static bool UpdateRebornYinJiInfo(int RoleID, string StampInfo, int ResetNum, int UsePoint)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.UpdateRebornYinJi, new object[]
				{
					RoleID,
					StampInfo,
					ResetNum,
					UsePoint
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008CE RID: 2254 RVA: 0x00052000 File Offset: 0x00050200
		public static bool InsertRebornYinJiInfo(int RoleID, string StampInfo, int ResetNum, int UsePoint)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.InsertRebornYinJi, new object[]
				{
					RoleID,
					StampInfo,
					ResetNum,
					UsePoint
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x00052084 File Offset: 0x00050284
		public static int AddUserLimitGoodsBuyItem(DBManager dbMgr, string UserID, int goodsID, int dayID, int usedNum, string stage)
		{
			int ret = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.RegressInsertStore, new object[]
				{
					UserID,
					goodsID,
					dayID,
					usedNum,
					stage
				});
				ret = conn.ExecuteNonQuery(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x00052110 File Offset: 0x00050310
		public static List<string> GetUserZoneID(DBManager dbMgr, int roleID)
		{
			MySQLConnection conn = null;
			string userId = "";
			int zoneId = -1;
			List<string> lData = new List<string>();
			try
			{
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format("SELECT userid,zoneid FROM t_roles WHERE rid = {0}", roleID);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				try
				{
					MySQLDataReader reader = cmd.ExecuteReaderEx();
					if (reader.Read())
					{
						userId = reader[0].ToString();
						zoneId = Convert.ToInt32(reader[1].ToString());
					}
					cmd.Dispose();
					cmd = null;
				}
				catch (MySQLException)
				{
					if (null != cmd)
					{
						cmd.Dispose();
						cmd = null;
					}
				}
			}
			finally
			{
				if (null != conn)
				{
					dbMgr.DBConns.PushDBConnection(conn);
				}
				lData.Add(userId);
				lData.Add(zoneId.ToString());
			}
			return lData;
		}

		// Token: 0x060008D1 RID: 2257 RVA: 0x00052234 File Offset: 0x00050434
		public static bool InsertRebornEquipHoleInfo(int RoleID, int HoleID, int Level, int Able)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.RebornEquipHoleInsertInfo, new object[]
				{
					RoleID,
					HoleID,
					Level,
					Able
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008D2 RID: 2258 RVA: 0x000522C0 File Offset: 0x000504C0
		public static bool UpdateRebornEquipHoleInfo(int RoleID, int HoleID, int Level, int Able)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.RebornEquipHoleUpdateInfo, new object[]
				{
					Level,
					Able,
					RoleID,
					HoleID
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008D3 RID: 2259 RVA: 0x0005234C File Offset: 0x0005054C
		public static bool InsertMazingerStoreInfo(int RoleID, int TypeID, int Stage, int Level, int Exp)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.MazingerStoreInsertInfo, new object[]
				{
					RoleID,
					TypeID,
					Stage,
					Level,
					Exp
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008D4 RID: 2260 RVA: 0x000523E0 File Offset: 0x000505E0
		public static bool UpdateMazingerStoreInfo(int RoleID, int TypeID, int Stage, int Level, int Exp)
		{
			bool ret = false;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format(SqlDefineManager.MazingerStoreUpdateInfo, new object[]
				{
					RoleID,
					TypeID,
					Stage,
					Level,
					Exp
				});
				ret = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return ret;
		}

		// Token: 0x060008D6 RID: 2262 RVA: 0x000524C8 File Offset: 0x000506C8
		// Note: this type is marked as 'beforefieldinit'.
		static DBWriter()
		{
			byte[] array = new byte[3];
			DBWriter._UpdateTask_fieldTypes = array;
			DBWriter._UpdatePet_fieldNames = new string[]
			{
				"petname",
				"pettype",
				"feednum",
				"realivenum",
				"props",
				"isdel",
				"addtime",
				"level"
			};
			DBWriter._UpdatePet_fieldTypes = new byte[]
			{
				1,
				0,
				0,
				0,
				1,
				0,
				3,
				0
			};
			DBWriter._UpdateActivity_fieldNames = new string[]
			{
				"loginweekid",
				"logindayid",
				"loginnum",
				"newstep",
				"steptime",
				"lastmtime",
				"curmid",
				"curmtime",
				"songliid",
				"logingiftstate",
				"onlinegiftstate",
				"lastlimittimehuodongid",
				"lastlimittimedayid",
				"limittimeloginnum",
				"limittimegiftstate",
				"everydayonlineawardstep",
				"geteverydayonlineawarddayid",
				"serieslogingetawardstep",
				"seriesloginawarddayid",
				"seriesloginawardgoodsid",
				"everydayonlineawardgoodsid"
			};
			DBWriter._UpdateActivity_fieldTypes = new byte[]
			{
				1,
				1,
				0,
				0,
				3,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				1
			};
			DBWriter._UpdateWing_fieldNames = new string[]
			{
				"equiped",
				"wingid",
				"forgeLevel",
				"failednum",
				"starexp",
				"zhulingnum",
				"zhuhunnum"
			};
			array = new byte[7];
			DBWriter._UpdateWing_fieldTypes = array;
			DBWriter._UpdateGoods2_fieldNames = new string[]
			{
				"isusing",
				"forge_level",
				"starttime",
				"endtime",
				"site",
				"quality",
				"Props",
				"gcount",
				"jewellist",
				"bagindex",
				"salemoney1",
				"saleyuanbao",
				"saleyinpiao",
				"binding",
				"addpropindex",
				"bornindex",
				"lucky",
				"strong",
				"excellenceinfo",
				"appendproplev",
				"equipchangelife"
			};
			DBWriter._UpdateGoods2_fieldTypes = new byte[]
			{
				0,
				0,
				1,
				1,
				0,
				0,
				1,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			};
			DBWriter.GoodsBakTableMutex = new object();
			DBWriter.GoodsBakTableIndex = -1;
			DBWriter.GoodsBakTableNames = new string[]
			{
				"t_goods_bak",
				"t_goods_bak_1"
			};
		}

		// Token: 0x0400098A RID: 2442
		private const string RoleExtIdKey = "role_ext_auto_increment";

		// Token: 0x0400098B RID: 2443
		private const int RoleExtIdValidStart = 1500000000;

		// Token: 0x0400098C RID: 2444
		private static readonly string[][] ValidateDatabaseTables = new string[][]
		{
			new string[]
			{
				"t_login",
				"userid"
			},
			new string[]
			{
				"t_usemoney_log",
				"Id"
			},
			new string[]
			{
				"t_goods_bak",
				"id"
			},
			new string[]
			{
				"t_goods_bak_1",
				"id"
			},
			new string[]
			{
				"t_goods",
				"washprops"
			},
			new string[]
			{
				"t_goods",
				"ehinfo"
			},
			new string[]
			{
				"t_goods_bak",
				"washprops"
			},
			new string[]
			{
				"t_goods_bak",
				"ehinfo"
			}
		};

		// Token: 0x0400098D RID: 2445
		private static readonly string[][] ValidateDatabaseTableRows = new string[][]
		{
			new string[]
			{
				"t_lingdi",
				"where lingdi=7",
				"罗兰城战配置信息(lingdi=7)"
			}
		};

		// Token: 0x0400098E RID: 2446
		private static readonly string[][] ValidateDatabaseTableIndexes = new string[][]
		{
			new string[]
			{
				"t_lingyu",
				"PRIMARY"
			}
		};

		// Token: 0x0400098F RID: 2447
		private static readonly string[] _UpdateGoods_fieldNames = new string[]
		{
			"isusing",
			"forge_level",
			"starttime",
			"endtime",
			"site",
			"quality",
			"Props",
			"gcount",
			"jewellist",
			"bagindex",
			"salemoney1",
			"saleyuanbao",
			"saleyinpiao",
			"binding",
			"addpropindex",
			"bornindex",
			"lucky",
			"strong",
			"excellenceinfo",
			"appendproplev",
			"equipchangelife"
		};

		// Token: 0x04000990 RID: 2448
		private static readonly byte[] _UpdateGoods_fieldTypes = new byte[]
		{
			0,
			0,
			1,
			1,
			0,
			0,
			1,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};

		// Token: 0x04000991 RID: 2449
		private static readonly string[] _UpdateTask_fieldNames = new string[]
		{
			"focus",
			"value1",
			"value2"
		};

		// Token: 0x04000992 RID: 2450
		private static readonly byte[] _UpdateTask_fieldTypes;

		// Token: 0x04000993 RID: 2451
		private static readonly string[] _UpdatePet_fieldNames;

		// Token: 0x04000994 RID: 2452
		private static readonly byte[] _UpdatePet_fieldTypes;

		// Token: 0x04000995 RID: 2453
		private static readonly string[] _UpdateActivity_fieldNames;

		// Token: 0x04000996 RID: 2454
		private static readonly byte[] _UpdateActivity_fieldTypes;

		// Token: 0x04000997 RID: 2455
		private static readonly string[] _UpdateWing_fieldNames;

		// Token: 0x04000998 RID: 2456
		private static readonly byte[] _UpdateWing_fieldTypes;

		// Token: 0x04000999 RID: 2457
		private static readonly string[] _UpdateGoods2_fieldNames;

		// Token: 0x0400099A RID: 2458
		private static readonly byte[] _UpdateGoods2_fieldTypes;

		// Token: 0x0400099B RID: 2459
		private static object GoodsBakTableMutex;

		// Token: 0x0400099C RID: 2460
		private static int GoodsBakTableIndex;

		// Token: 0x0400099D RID: 2461
		private static readonly string[] GoodsBakTableNames;
	}
}
