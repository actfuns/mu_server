using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Logic.Rank;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class RechargeRepayActiveMgr
	{
		
		private static bool GetCmdDataField(int nID, byte[] data, int count, out string[] fields)
		{
			string cmdData = null;
			fields = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return false;
			}
			fields = cmdData.Split(new char[]
			{
				':'
			});
			return true;
		}

		
		public static TCPProcessCmdResults ProcessQueryActiveInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] fields = null;
			string strcmd;
			try
			{
				if (!RechargeRepayActiveMgr.GetCmdDataField(nID, data, count, out fields))
				{
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int activeid = Global.SafeConvertToInt32(fields[1], 10);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				long hasgettimes = 0L;
				string lastgettime = "";
				string huoDongKeyStr = "not_limit";
				string extData = "";
				int num = activeid;
				if (num <= 27)
				{
					if (num != 23)
					{
						if (num == 27)
						{
							DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
							DateTime endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
							huoDongKeyStr = Global.GetHuoDongKeyString(startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
							RankDataKey key = new RankDataKey(RankType.Charge, startTime, endTime, null);
							int money = roleInfo.RankValue.GetRankValue(key);
							extData = string.Concat(money);
						}
					}
					else
					{
						if (5 != fields.Length)
						{
							return TCPProcessCmdResults.RESULT_DATA;
						}
						int hefutime = Global.SafeConvertToInt32(fields[2], 10);
						int hefuEndtime = Global.SafeConvertToInt32(fields[3], 10);
						huoDongKeyStr = Global.GetHuoDongKeyString(hefutime.ToString(), hefuEndtime.ToString());
						Dictionary<int, float> CoeDict = new Dictionary<int, float>();
						string strconfig = fields[4];
						string[] strattr = strconfig.Split(new char[]
						{
							'|'
						});
						for (int i = 0; i < strattr.Length; i++)
						{
							string[] strcoe = strattr[i].Split(new char[]
							{
								','
							});
							if (2 == strcoe.Length)
							{
								int rankcfg = Global.SafeConvertToInt32(strcoe[0], 10);
								float coe = (float)Convert.ToDouble(strcoe[1]);
								CoeDict[rankcfg] = coe;
							}
						}
						DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 23, huoDongKeyStr, out hasgettimes, out lastgettime);
						int lastgetday = 0;
						if (!string.IsNullOrEmpty(lastgettime))
						{
							lastgetday = Global.GetOffsetDay(DateTime.Parse(lastgettime));
						}
						int userdayflag;
						if (lastgetday < hefutime)
						{
							userdayflag = hefutime;
						}
						else if (hasgettimes > 0L)
						{
							userdayflag = lastgetday;
						}
						else
						{
							userdayflag = hefutime;
						}
						int currday = Global.GetOffsetDay(DateTime.Now);
						int overDay = currday - 1;
						if (overDay > hefuEndtime)
						{
							overDay = hefuEndtime;
						}
						int userrebate = 0;
						if (userdayflag == currday)
						{
							userrebate = 0;
						}
						else
						{
							for (int i = userdayflag; i <= overDay; i++)
							{
								DateTime now = Global.GetRealDate(i);
								string startTime2 = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
								string endTime2 = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
								int userrank = GameDBManager.DayRechargeRankMgr.GetRoleRankByDay(dbMgr, roleInfo.UserID, i);
								if (CoeDict.ContainsKey(userrank))
								{
									RankDataKey key = new RankDataKey(RankType.Charge, startTime2, endTime2, null);
									int input = roleInfo.RankValue.GetRankValue(key);
									userrebate += (int)((float)input * CoeDict[userrank]);
								}
							}
						}
						extData += userrebate;
						extData += ":";
						if (currday > hefuEndtime)
						{
							extData += "0";
						}
						else
						{
							List<InputKingPaiHangData> ranklist = GameDBManager.DayRechargeRankMgr.GetRankByDay(dbMgr, currday);
							extData += ranklist.Count;
							int rank = 1;
							foreach (InputKingPaiHangData item in ranklist)
							{
								extData += "|";
								extData += rank;
								extData += ",";
								extData += item.MaxLevelRoleZoneID;
								extData += ",";
								extData += item.MaxLevelRoleName;
								rank++;
							}
						}
					}
				}
				else
				{
					switch (num)
					{
					case 38:
					{
						int realmoney = 0;
						int usermoney = 0;
						DBQuery.QueryUserMoneyByUserID(dbMgr, roleInfo.UserID, out usermoney, out realmoney);
						realmoney = Global.TransMoneyToYuanBao(realmoney);
						extData = string.Concat(realmoney);
						break;
					}
					case 39:
					{
						string startTime2 = "2011-11-11";
						string endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
						extData = DBQuery.GetUserUsedMoney(dbMgr, roleID, startTime2, endtime).ToString();
						break;
					}
					default:
						switch (num)
						{
						case 46:
						{
							if (fields.Length != 4)
							{
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string fromDateCmd = fields[2].Replace('$', ':');
							string toDateCmd = fields[3].Replace('$', ':');
							huoDongKeyStr = Global.GetHuoDongKeyString(fromDateCmd, toDateCmd);
							RankDataKey key = new RankDataKey(RankType.Charge, fromDateCmd, toDateCmd, null);
							int money = roleInfo.RankValue.GetRankValue(key);
							extData = string.Concat(money);
							break;
						}
						case 48:
						{
							DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
							DateTime endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
							huoDongKeyStr = Global.GetHuoDongKeyString(startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
							RankDataKey chargeKey = new RankDataKey(RankType.Charge, startTime, endTime, null);
							int chargeMoney = roleInfo.RankValue.GetRankValue(chargeKey);
							RankDataKey consumeKey = new RankDataKey(RankType.Consume, startTime, endTime, null);
							int consumeMoney = roleInfo.RankValue.GetRankValue(consumeKey);
							extData = string.Format("{0},{1}", chargeMoney, consumeMoney);
							break;
						}
						}
						break;
					}
				}
				lock (roleInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, activeid, huoDongKeyStr, out hasgettimes, out lastgettime);
					string temp = "";
					if (activeid == 48)
					{
						temp = hasgettimes.ToString();
					}
					else
					{
						string getIndexstr = hasgettimes.ToString();
						if (hasgettimes != 0L)
						{
							int i = 0;
							foreach (char item2 in getIndexstr.ToCharArray())
							{
								temp += item2;
								i++;
								if (i < getIndexstr.Length)
								{
									temp += ",";
								}
							}
						}
						if (string.IsNullOrEmpty(temp))
						{
							temp = "1";
						}
					}
					strcmd = string.Format("{0}:{1}:{2}", 1, temp, extData);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			strcmd = string.Format("{0}:{1}:{2}", 0, "", "");
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public static TCPProcessCmdResults ProcessGetActiveAwards(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] fields = null;
			TCPProcessCmdResults result;
			if (!RechargeRepayActiveMgr.GetCmdDataField(nID, data, count, out fields))
			{
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				int roleID = Convert.ToInt32(fields[0]);
				int activeid = Global.SafeConvertToInt32(fields[1], 10);
				int hasgettimes = Global.SafeConvertToInt32(fields[2], 10);
				long hasgettimesLong = Global.SafeConvertToInt64(fields[2], 10);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
				else
				{
					string huoDongKeyStr = "not_limit";
					int num = activeid;
					string strcmd;
					if (num <= 27)
					{
						if (num != 23)
						{
							if (num == 27)
							{
								DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
								DateTime endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
								huoDongKeyStr = Global.GetHuoDongKeyString(startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
							}
						}
						else
						{
							if (5 != fields.Length)
							{
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int hefutime = Global.SafeConvertToInt32(fields[2], 10);
							int hefuEndtime = Global.SafeConvertToInt32(fields[3], 10);
							huoDongKeyStr = Global.GetHuoDongKeyString(hefutime.ToString(), hefuEndtime.ToString());
							Dictionary<int, float> CoeDict = new Dictionary<int, float>();
							string strconfig = fields[4];
							string[] strattr = strconfig.Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < strattr.Length; i++)
							{
								string[] strcoe = strattr[i].Split(new char[]
								{
									','
								});
								if (2 == strcoe.Length)
								{
									int rank = Global.SafeConvertToInt32(strcoe[0], 10);
									float coe = (float)Convert.ToDouble(strcoe[1]);
									CoeDict[rank] = coe;
								}
							}
							int ifhastime = 0;
							string lastgettime = "";
							DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 23, huoDongKeyStr, out ifhastime, out lastgettime);
							int lastgetday = 0;
							if (!string.IsNullOrEmpty(lastgettime))
							{
								lastgetday = Global.GetOffsetDay(DateTime.Parse(lastgettime));
							}
							int userdayflag;
							if (lastgetday < hefutime)
							{
								userdayflag = hefutime;
							}
							else if (ifhastime > 0)
							{
								userdayflag = lastgetday;
							}
							else
							{
								userdayflag = hefutime;
							}
							int currday = Global.GetOffsetDay(DateTime.Now);
							if (userdayflag == currday)
							{
								strcmd = string.Format("{0}:{1}:{2}", 1, activeid, 0);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int overDay = Global.GetOffsetDay(DateTime.Now) - 1;
							if (overDay > hefuEndtime)
							{
								overDay = hefuEndtime;
							}
							int userrebate = 0;
							for (int i = userdayflag; i <= overDay; i++)
							{
								DateTime now = Global.GetRealDate(i);
								string startTime2 = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
								string endTime2 = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
								int rank = GameDBManager.DayRechargeRankMgr.GetRoleRankByDay(dbMgr, roleInfo.UserID, i);
								if (CoeDict.ContainsKey(rank))
								{
									RankDataKey key = new RankDataKey(RankType.Charge, startTime2, endTime2, null);
									int input = roleInfo.RankValue.GetRankValue(key);
									userrebate += (int)((float)input * CoeDict[rank]);
								}
							}
							strcmd = string.Format("{0}:{1}:{2}", 1, activeid, userrebate);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						switch (num)
						{
						case 38:
						case 39:
							lock (roleInfo)
							{
								int ret = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, activeid, huoDongKeyStr, hasgettimesLong, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
								if (ret < 0)
								{
									ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, activeid, huoDongKeyStr, hasgettimesLong, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
								}
								if (ret < 0)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
									return TCPProcessCmdResults.RESULT_FAILED;
								}
							}
							strcmd = string.Format("{0}:{1}:{2}", 1, activeid, hasgettimesLong);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						default:
							switch (num)
							{
							case 46:
							{
								if (fields.Length != 5)
								{
									return TCPProcessCmdResults.RESULT_DATA;
								}
								string fromDateCmd = fields[3].Replace('$', ':');
								string toDateCmd = fields[4].Replace('$', ':');
								huoDongKeyStr = Global.GetHuoDongKeyString(fromDateCmd, toDateCmd);
								break;
							}
							case 48:
							{
								DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
								DateTime endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
								huoDongKeyStr = Global.GetHuoDongKeyString(startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
								break;
							}
							}
							break;
						}
					}
					lock (roleInfo)
					{
						int ret = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, activeid, huoDongKeyStr, (long)hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, activeid, huoDongKeyStr, (long)hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						}
						if (ret < 0)
						{
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
							return TCPProcessCmdResults.RESULT_FAILED;
						}
					}
					strcmd = string.Format("{0}:{1}:{2}", 1, activeid, hasgettimes);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
			}
			return result;
		}
	}
}
