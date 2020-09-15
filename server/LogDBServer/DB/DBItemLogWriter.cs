using System;
using System.Collections.Generic;
using System.Text;
using LogDBServer.Logic;
using MySQLDriverCS;
using Server.Tools;

namespace LogDBServer.DB
{
	// Token: 0x0200000F RID: 15
	public class DBItemLogWriter
	{
		// Token: 0x06000034 RID: 52 RVA: 0x00002C34 File Offset: 0x00000E34
		public static DBItemLogWriter getInstance()
		{
			return DBItemLogWriter.instance;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002C4C File Offset: 0x00000E4C
		public int ConformTableColumns(MySQLConnection conn, string strTableName)
		{
			int ret = 0;
			try
			{
				lock (this.Mutex)
				{
					DBItemLogWriter.TableColumnInfo tableColumnInfo;
					if (!this.TableColumnInfoDict.TryGetValue(strTableName, out tableColumnInfo))
					{
						tableColumnInfo = new DBItemLogWriter.TableColumnInfo();
						this.TableColumnInfoDict.Add(strTableName, tableColumnInfo);
					}
					if (!tableColumnInfo.HasColumnOptSurplus || !tableColumnInfo.HasColumnExtData)
					{
						string cmdText = string.Format("describe {0}", strTableName);
						MySQLCommand cmd = new MySQLCommand(cmdText, conn);
						MySQLDataReader reader = cmd.ExecuteReaderEx();
						while (reader.Read())
						{
							if (0 == string.Compare(reader[0].ToString(), "optSurplus", true))
							{
								tableColumnInfo.HasColumnOptSurplus = true;
							}
							else if (0 == string.Compare(reader[0].ToString(), "extData", true))
							{
								tableColumnInfo.HasColumnExtData = true;
								byte[] btype = reader[1] as byte[];
								string ctype = Encoding.UTF8.GetString(btype);
								if (0 == string.Compare(ctype, "varchar(255)", true))
								{
									tableColumnInfo.HasAlterColumnExtData = true;
								}
							}
						}
						cmd.Dispose();
						if (!tableColumnInfo.HasColumnOptSurplus)
						{
							cmdText = string.Format("ALTER TABLE `{0}` ADD COLUMN `optSurplus`  int(11) NULL DEFAULT 0 COMMENT '属性操作后的剩余值' AFTER `zoneID`", strTableName);
							cmd = new MySQLCommand(cmdText, conn);
							try
							{
								ret = cmd.ExecuteNonQuery();
								tableColumnInfo.HasColumnOptSurplus = true;
							}
							catch (Exception)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("更新数据库失败: {0}", cmdText));
							}
							cmd.Dispose();
						}
						if (!tableColumnInfo.HasColumnExtData)
						{
							cmdText = string.Format("ALTER TABLE `{0}` ADD COLUMN `extData` CHAR(32) COMMENT '物品附加属性信息' NULL DEFAULT NULL AFTER `optSurplus`", strTableName);
							cmd = new MySQLCommand(cmdText, conn);
							try
							{
								ret = cmd.ExecuteNonQuery();
								tableColumnInfo.HasColumnExtData = true;
							}
							catch (Exception)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("更新数据库失败: {0}", cmdText));
							}
							cmd.Dispose();
						}
						if (!tableColumnInfo.HasAlterColumnExtData)
						{
							cmdText = string.Format("ALTER TABLE `{0}` MODIFY COLUMN `extData` varchar(255) COMMENT '物品附加属性信息' NULL DEFAULT NULL AFTER `optSurplus`", strTableName);
							cmd = new MySQLCommand(cmdText, conn);
							try
							{
								ret = cmd.ExecuteNonQuery();
								tableColumnInfo.HasColumnExtData = true;
							}
							catch (Exception)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("更新数据库失败: {0}", cmdText));
							}
							cmd.Dispose();
						}
					}
				}
			}
			catch (Exception ex)
			{
				ret = -5;
				LogManager.WriteException(ex.ToString());
			}
			return ret;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002F3C File Offset: 0x0000113C
		public void AddItemLogTable(DBManager dbMgr)
		{
			MySQLConnection conn = null;
			for (int i = 0; i < 3; i++)
			{
				try
				{
					string strTableName = DateTime.Now.AddDays((double)i).ToString("yyyyMMdd");
					conn = dbMgr.DBConns.PopDBConnection();
					string strCmd = string.Format("CREATE TABLE if not exists `t_log_{0}` (`Id` int(11) NOT NULL AUTO_INCREMENT,`DBId` int(11) DEFAULT NULL COMMENT '物品在物品表的数据库ID，不是物品时Id为-1',`ObjName` varchar(255) DEFAULT NULL COMMENT '操作对象名称',`optFrom` varchar(255) DEFAULT NULL COMMENT '操作产生点',`currEnvName` varchar(255) DEFAULT NULL COMMENT '对象所在当前环境名称',`tarEnvName` varchar(255) DEFAULT NULL COMMENT '对象将要到达的环境名称',`optType` char(6) DEFAULT NULL COMMENT '操作类型，如下：增加、销毁、修改、移动',`optTime` datetime DEFAULT NULL COMMENT '操作时间',`optAmount` int(11) DEFAULT NULL COMMENT '操作数量',`zoneID` int(11) DEFAULT NULL COMMENT '区编号',`optSurplus` int(11) DEFAULT NULL COMMENT '属性操作后剩余值',`extData` varchar(255) DEFAULT NULL COMMENT '物品附加属性信息',PRIMARY KEY (`Id`),KEY `DBId` (`DBId`),KEY `tarEnvName` (`tarEnvName`),KEY `idx_optTime` (`optTime`)) ENGINE=MyISAM AUTO_INCREMENT=76528 DEFAULT CHARSET=utf8 COMMENT='物品操作日志表';", strTableName);
					MySQLCommand cmd = new MySQLCommand(strCmd, conn);
					try
					{
						cmd.ExecuteNonQuery();
					}
					catch (Exception)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", strCmd));
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
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003014 File Offset: 0x00001214
		public void DelItemLogTable(DBManager dbMgr)
		{
			MySQLConnection conn = null;
			for (int i = 0; i < 3; i++)
			{
				try
				{
					string strTableName = DateTime.Now.AddDays((double)(-16 - i)).ToString("yyyyMMdd");
					conn = dbMgr.DBConns.PopDBConnection();
					string strCmd = string.Format("DROP TABLE IF EXISTS `t_log_{0}`;", strTableName);
					MySQLCommand cmd = new MySQLCommand(strCmd, conn);
					try
					{
						cmd.ExecuteNonQuery();
					}
					catch (Exception)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", strCmd));
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
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000030F0 File Offset: 0x000012F0
		public int insertItemLog(DBManager dbMgr, string[] fields)
		{
			int ret = -1;
			MySQLConnection conn = null;
			try
			{
				string strTableName = "t_log_" + DateTime.Now.ToString("yyyyMMdd");
				conn = dbMgr.DBConns.PopDBConnection();
				this.ConformTableColumns(conn, strTableName);
				string cmdText;
				if (fields.Length >= 10 && !string.IsNullOrEmpty(fields[9]))
				{
					cmdText = string.Format("INSERT INTO {0} (DBId, ObjName, optFrom, currEnvName, tarEnvName, optType, optTime, optAmount, zoneID, optSurplus, extData) VALUES({1}, '{2}', '{3}', '{4}', '{5}', '{6}', now(), {7}, {8}, {9}, '{10}')", new object[]
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
				}
				else
				{
					cmdText = string.Format("INSERT INTO {0} (DBId, ObjName, optFrom, currEnvName, tarEnvName, optType, optTime, optAmount, zoneID, optSurplus, extData) VALUES({1}, '{2}', '{3}', '{4}', '{5}', '{6}', now(), {7}, {8}, {9}, NULL)", new object[]
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
						fields[8]
					});
				}
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				cmd = new MySQLCommand(cmdText, conn);
				try
				{
					ret = cmd.ExecuteNonQuery();
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
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
			return ret;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000032B4 File Offset: 0x000014B4
		public int insertTradeFreqLog(DBManager dbMgr, string[] fields)
		{
			int ret = -1;
			MySQLConnection conn = null;
			try
			{
				string strTableName = "t_trademoney_freq_log";
				int type = Convert.ToInt32(fields[0]);
				int count = Convert.ToInt32(fields[1]);
				long updatetTick = Convert.ToInt64(fields[2]);
				int serverID = Convert.ToInt32(fields[3]);
				string userID = fields[4];
				int roleID = Convert.ToInt32(fields[5]);
				string roleName = fields[6];
				int inputMoney = Convert.ToInt32(fields[7]);
				int usedMoney = Convert.ToInt32(fields[8]);
				int currMoney = Convert.ToInt32(fields[9]);
				int onlineSec = Convert.ToInt32(fields[10]);
				int level = Convert.ToInt32(fields[11]);
				long regTick = Convert.ToInt64(fields[12]);
				string ip = fields[13];
				DateTime updateTime = new DateTime(updatetTick * 10000L);
				DateTime regTime = new DateTime(regTick * 10000L);
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format("INSERT INTO {0} (dayid, roleid, zoneid, userid, rname, type, count, updatetime, \r\n                                                    level, regtime, online_minute, inputmoney, usedmoney, currmoney, ip\r\n                                                    ) VALUES({1}, {2}, {3}, '{4}', '{5}', {6}, {7}, '{8}',\r\n                                                    {9}, '{10}', {11}, {12}, {13}, {14}, '{15}')", new object[]
				{
					strTableName,
					Global.GetOffsetDay(updateTime),
					roleID,
					serverID,
					userID,
					roleName,
					type,
					count,
					updateTime.ToString("yyyy-MM-dd HH:mm:ss"),
					level,
					regTime.ToString("yyyy-MM-dd HH:mm:ss"),
					onlineSec / 60,
					inputMoney,
					usedMoney,
					currMoney,
					ip
				});
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				try
				{
					ret = cmd.ExecuteNonQuery();
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
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
			return ret;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000034DC File Offset: 0x000016DC
		public int insertTradeNumLog(DBManager dbMgr, string[] fields)
		{
			int ret = -1;
			MySQLConnection conn = null;
			try
			{
				string strTableName = "t_trademoney_num_log";
				int type = Convert.ToInt32(fields[0]);
				int money = Convert.ToInt32(fields[1]);
				long updatetTick = Convert.ToInt64(fields[2]);
				int serverID = Convert.ToInt32(fields[3]);
				string userID = fields[4];
				int roleID = Convert.ToInt32(fields[5]);
				string roleName = fields[6];
				int inputMoney = Convert.ToInt32(fields[7]);
				int usedMoney = Convert.ToInt32(fields[8]);
				int currMoney = Convert.ToInt32(fields[9]);
				int onlineSec = Convert.ToInt32(fields[10]);
				int level = Convert.ToInt32(fields[11]);
				long regTick = Convert.ToInt64(fields[12]);
				string ip = fields[13];
				string userID2 = fields[14];
				int roleID2 = Convert.ToInt32(fields[15]);
				string roleName2 = fields[16];
				int inputMoney2 = Convert.ToInt32(fields[17]);
				int usedMoney2 = Convert.ToInt32(fields[18]);
				int currMoney2 = Convert.ToInt32(fields[19]);
				int onlineSec2 = Convert.ToInt32(fields[20]);
				int level2 = Convert.ToInt32(fields[21]);
				long regTick2 = Convert.ToInt64(fields[22]);
				string ip2 = fields[23];
				DateTime updateTime = new DateTime(updatetTick * 10000L);
				conn = dbMgr.DBConns.PopDBConnection();
				string cmdText = string.Format("INSERT INTO {0} (type, money, updatetime, zoneid, \r\n                                                    userid1, roleid1, rname1, inputmoney1, usedmoney1, currmoney1,online_minute1, ip1\r\n                                                    , userid2, roleid2, rname2, inputmoney2, usedmoney2, currmoney2, online_minute2, ip2) \r\n                                                    VALUES({1}, {2}, '{3}', {4},\r\n                                                    '{5}', {6}, '{7}', {8}, {9}, {10}, {11}, '{12}', \r\n                                                    '{13}', {14}, '{15}', {16}, {17}, {18}, {19}, '{20}')", new object[]
				{
					strTableName,
					type,
					money,
					updateTime.ToString("yyyy-MM-dd HH:mm:ss"),
					serverID,
					userID,
					roleID,
					roleName,
					inputMoney,
					usedMoney,
					currMoney,
					onlineSec / 60,
					ip,
					userID2,
					roleID2,
					roleName2,
					inputMoney2,
					usedMoney2,
					currMoney2,
					onlineSec2 / 60,
					ip2
				});
				MySQLCommand cmd = new MySQLCommand(cmdText, conn);
				try
				{
					ret = cmd.ExecuteNonQuery();
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
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
			return ret;
		}

		// Token: 0x0400001D RID: 29
		private const int cMaxAddDay = 3;

		// Token: 0x0400001E RID: 30
		private const int cSaveTableNum = 16;

		// Token: 0x0400001F RID: 31
		private static DBItemLogWriter instance = new DBItemLogWriter();

		// Token: 0x04000020 RID: 32
		private object Mutex = new object();

		// Token: 0x04000021 RID: 33
		private Dictionary<string, DBItemLogWriter.TableColumnInfo> TableColumnInfoDict = new Dictionary<string, DBItemLogWriter.TableColumnInfo>();

		// Token: 0x02000010 RID: 16
		internal class TableColumnInfo
		{
			// Token: 0x04000022 RID: 34
			public bool HasColumnOptSurplus = false;

			// Token: 0x04000023 RID: 35
			public bool HasColumnExtData = false;

			// Token: 0x04000024 RID: 36
			public bool HasAlterColumnExtData = false;
		}
	}
}
