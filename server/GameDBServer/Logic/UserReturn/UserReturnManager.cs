using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using GameDBServer.Tools;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic.UserReturn
{
	
	public class UserReturnManager : SingletonTemplate<UserReturnManager>, IManager, ICmdProcessor
	{
		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13100, SingletonTemplate<UserReturnManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13101, SingletonTemplate<UserReturnManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13102, SingletonTemplate<UserReturnManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13103, SingletonTemplate<UserReturnManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13104, SingletonTemplate<UserReturnManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13105, SingletonTemplate<UserReturnManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13106, SingletonTemplate<UserReturnManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13107, SingletonTemplate<UserReturnManager>.Instance());
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
			case 13100:
				this.UserReturnIsOpen(client, nID, cmdParams, count);
				break;
			case 13101:
				this.UserReturnDataGet(client, nID, cmdParams, count);
				break;
			case 13102:
				this.UserReturnDataUpdate(client, nID, cmdParams, count);
				break;
			case 13103:
				this.UserReturnDataDel(client, nID, cmdParams, count);
				break;
			case 13104:
				this.UserReturnDataList(client, nID, cmdParams, count);
				break;
			case 13105:
				this.UserReturnAwardList(client, nID, cmdParams, count);
				break;
			case 13106:
				this.UserReturnAwardUpdate(client, nID, cmdParams, count);
				break;
			case 13107:
				this.UserReturnCheck(client, nID, cmdParams, count);
				break;
			}
		}

		
		public void UserReturnIsOpen(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			lock (UserReturnManager.Mutex)
			{
				UserReturnManager._activityInfo = DataHelper.BytesToObject<ReturnActivity>(cmdParams, 0, count);
				UserReturnManager._userReturnIsOpen = UserReturnManager._activityInfo.IsOpen;
			}
			client.sendCmd<int>(nID, 1);
		}

		
		public void UserReturnDataGet(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			ReturnData result = null;
			string[] fields = null;
			try
			{
				int length = 2;
				if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out fields, length))
				{
					client.sendCmd<ReturnData>(nID, result);
					return;
				}
				string userID = fields[0];
				int zoneID = int.Parse(fields[1]);
				result = this.GetUserReturnData(userID, zoneID);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			client.sendCmd<ReturnData>(nID, result);
		}

		
		public void UserReturnDataList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			List<ReturnData> result = new List<ReturnData>();
			string[] fields = null;
			try
			{
				int length = 4;
				if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out fields, length))
				{
					client.sendCmd<List<ReturnData>>(nID, result);
					return;
				}
				string activityDay = fields[0];
				int activityID = int.Parse(fields[1]);
				int zoneID = int.Parse(fields[2]);
				int roleID = int.Parse(fields[3]);
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText = string.Format("SELECT id,activityID,activityDay,pzoneID,proleID,czoneID,croleID,vip,`level`,logTime,checkState,logState \r\n                                                        FROM t_user_return_back \r\n                                                        WHERE activityDay='{0}' AND activityID={1} AND pzoneID={2} AND proleID={3} AND logState=0 ", new object[]
					{
						activityDay,
						activityID,
						zoneID,
						roleID
					});
					MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
					while (reader.Read())
					{
						result.Add(new ReturnData
						{
							DBID = Convert.ToInt32(reader["id"].ToString()),
							ActivityID = Convert.ToInt32(reader["activityID"].ToString()),
							ActivityDay = reader["activityDay"].ToString(),
							PZoneID = Convert.ToInt32(reader["pzoneID"].ToString()),
							PRoleID = Convert.ToInt32(reader["proleID"].ToString()),
							CZoneID = Convert.ToInt32(reader["czoneID"].ToString()),
							CRoleID = Convert.ToInt32(reader["croleID"].ToString()),
							Vip = Convert.ToInt32(reader["vip"].ToString()),
							Level = Convert.ToInt32(reader["level"].ToString()),
							LogTime = Convert.ToDateTime(reader["logTime"].ToString()),
							StateCheck = Convert.ToInt32(reader["checkState"].ToString()),
							StateLog = Convert.ToInt32(reader["logState"].ToString())
						});
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			client.sendCmd<List<ReturnData>>(nID, result);
		}

		
		public void UserReturnDataUpdate(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool result = false;
			try
			{
				ReturnData data = DataHelper.BytesToObject<ReturnData>(cmdParams, 0, count);
				this.UpdateUserReturnData(data);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			client.sendCmd<bool>(nID, result);
		}

		
		public void UserReturnDataDel(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool result = false;
			try
			{
				ReturnData data = DataHelper.BytesToObject<ReturnData>(cmdParams, 0, count);
				result = this.DelUserReturnData(data.DBID);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			client.sendCmd<bool>(nID, result);
		}

		
		public void UserReturnAwardList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			Dictionary<int, int[]> result = new Dictionary<int, int[]>();
			string[] fields = null;
			try
			{
				int length = 3;
				if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out fields, length))
				{
					client.sendCmd<Dictionary<int, int[]>>(nID, result);
					return;
				}
				string activityDay = fields[0];
				int activityID = int.Parse(fields[1]);
				string userID = fields[2];
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText = string.Format("SELECT type,state FROM t_user_return_award WHERE activityDay = '{0}' AND activityID = '{1}' AND userid='{2}'", activityDay, activityID, userID);
					MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
					while (reader.Read())
					{
						int type = Convert.ToInt32(reader["type"].ToString());
						string[] awardArr = reader["state"].ToString().Split(new char[]
						{
							'*'
						});
						List<int> list = new List<int>();
						foreach (string s in awardArr)
						{
							list.Add(Convert.ToInt32(s));
						}
						if (!result.ContainsKey(type))
						{
							result.Add(type, list.ToArray());
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			client.sendCmd<Dictionary<int, int[]>>(nID, result);
		}

		
		public void UserReturnAwardUpdate(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool result = false;
			string[] fields = null;
			try
			{
				int length = 6;
				if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out fields, length))
				{
					client.sendCmd<bool>(nID, result);
					return;
				}
				string activityDay = fields[0];
				int activityID = int.Parse(fields[1]);
				int zoneID = int.Parse(fields[2]);
				string userID = fields[3];
				int awardType = int.Parse(fields[4]);
				string award = fields[5];
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText = string.Format("REPLACE INTO t_user_return_award (activityID,activityDay,zoneID,userid,type,state) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", new object[]
					{
						activityID,
						activityDay,
						zoneID,
						userID,
						awardType,
						award
					});
					result = conn.ExecuteNonQueryBool(cmdText, 0);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			client.sendCmd<bool>(nID, result);
		}

		
		public void UserReturnCheck(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int isOldUser = 0;
			string[] fields = null;
			try
			{
				if (CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out fields, 3))
				{
					if (UserReturnManager._activityInfo.IsOpen)
					{
						string userid = fields[0];
						string zoneid = fields[1];
						string logtime = fields[2];
						int level = 0;
						using (MyDbConnection3 conn = new MyDbConnection3(false))
						{
							string cmdText = string.Format("select * from t_user_return where userid='{0}' and activityDay='{1}' and activityID='{2}' and zoneID='{3}'", new object[]
							{
								userid,
								UserReturnManager._activityInfo.ActivityDay,
								UserReturnManager._activityInfo.ActivityID,
								zoneid
							});
							if (null == conn.GetSingle(cmdText, 0, new MySQLParameter[0]))
							{
								int inputMoneyInPeriod = DBQuery.GetUserInputMoney(TCPManager.getInstance().DBMgr, userid, 0, "2000-01-01 00:00:00", "2050-01-01 00:00:00");
								int roleYuanBaoInPeriod = Global.TransMoneyToYuanBao(inputMoneyInPeriod);
								if (UserReturnManager._activityInfo.VIPNeedExp <= roleYuanBaoInPeriod)
								{
									cmdText = string.Format("select regtime from t_roles where userid='{0}' and regtime<'{1}'", userid, UserReturnManager._activityInfo.NotLoggedInBegin);
									if (null != conn.GetSingle(cmdText, 0, new MySQLParameter[0]))
									{
										cmdText = string.Format("select dayid from t_login where userid='{0}' and dayid>={1} and dayid<={2}", userid, Global.GetOffsetDay(UserReturnManager._activityInfo.NotLoggedInBegin), Global.GetOffsetDay(UserReturnManager._activityInfo.NotLoggedInFinish));
										if (null == conn.GetSingle(cmdText, 0, new MySQLParameter[0]))
										{
											cmdText = string.Format("select changelifecount*100+level as l from t_roles where userid='{0}' and zoneid='{1}' and isdel=0 order by l desc limit 1", userid, zoneid);
											level = conn.GetSingleInt(cmdText, 0, new MySQLParameter[0]);
											if (UserReturnManager._activityInfo.Level <= level)
											{
												isOldUser = 1;
											}
										}
									}
								}
								cmdText = string.Format("insert into t_user_return values (0,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}' )", new object[]
								{
									UserReturnManager._activityInfo.ActivityID,
									UserReturnManager._activityInfo.ActivityDay,
									zoneid,
									userid,
									UserReturnManager._activityInfo.VIPNeedExp,
									level,
									logtime,
									isOldUser
								});
								conn.ExecuteNonQuery(cmdText, 0);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			client.sendCmd<int>(nID, 1);
		}

		
		public ReturnData GetUserReturnData(string userID, int zoneID)
		{
			ReturnData result = null;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("select vip,level,checkState,logTime from t_user_return where userid='{0}' and zoneID='{1}' and activityDay='{2}' and activityID='{3}'", new object[]
				{
					userID,
					zoneID,
					UserReturnManager._activityInfo.ActivityDay,
					UserReturnManager._activityInfo.ActivityID
				});
				MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
				if (reader.Read())
				{
					int inputMoneyInPeriod = DBQuery.GetUserInputMoney(TCPManager.getInstance().DBMgr, userID, 0, UserReturnManager._activityInfo.ActivityDay, "2050-01-01 00:00:00");
					if (inputMoneyInPeriod < 0)
					{
						inputMoneyInPeriod = 0;
					}
					int roleYuanBaoInPeriod = Global.TransMoneyToYuanBao(inputMoneyInPeriod);
					result = new ReturnData
					{
						Vip = Convert.ToInt32(reader["vip"].ToString()),
						Level = Convert.ToInt32(reader["level"].ToString()),
						StateCheck = Convert.ToInt32(reader["checkState"].ToString()),
						LogTime = DateTime.Parse(reader["logTime"].ToString()),
						LeiJiChongZhi = roleYuanBaoInPeriod
					};
				}
			}
			return result;
		}

		
		public bool UpdateUserReturnData(ReturnData data)
		{
			bool result;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("REPLACE INTO t_user_return_back(activityID,activityDay,pzoneID,proleID,czoneID,croleID,vip,`level`,logTime,checkState,logState) \r\n                                                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}','{6}','{7}','{8}','{9}','{10}')", new object[]
				{
					data.ActivityID,
					data.ActivityDay,
					data.PZoneID,
					data.PRoleID,
					data.CZoneID,
					data.CRoleID,
					data.Vip,
					data.Level,
					data.LogTime,
					data.StateCheck,
					data.StateLog
				});
				result = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return result;
		}

		
		public bool DelUserReturnData(int id)
		{
			bool result;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE FROM t_user_return_back WHERE id={0}", id);
				result = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return result;
		}

		
		public void ScanLastUserReturn(DBManager dbMgr)
		{
		}

		
		public List<ReturnData> ReturnList()
		{
			List<ReturnData> list = new List<ReturnData>();
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("SELECT id,activityID,activityDay,pzoneID,proleID,czoneID,croleID,vip,`level`,logTime,checkState,logState FROM t_user_return order by logTime", new object[0]);
				MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
				while (reader.Read())
				{
					list.Add(new ReturnData
					{
						DBID = Convert.ToInt32(reader["id"].ToString()),
						ActivityID = Convert.ToInt32(reader["activityID"].ToString()),
						ActivityDay = reader["activityDay"].ToString(),
						PZoneID = Convert.ToInt32(reader["pzoneID"].ToString()),
						PRoleID = Convert.ToInt32(reader["proleID"].ToString()),
						CZoneID = Convert.ToInt32(reader["czoneID"].ToString()),
						CRoleID = Convert.ToInt32(reader["croleID"].ToString()),
						Vip = Convert.ToInt32(reader["vip"].ToString()),
						Level = Convert.ToInt32(reader["level"].ToString()),
						LogTime = Convert.ToDateTime(reader["logTime"].ToString()),
						StateCheck = Convert.ToInt32(reader["checkState"].ToString()),
						StateLog = Convert.ToInt32(reader["logState"].ToString())
					});
				}
			}
			return list;
		}

		
		public bool ReturnDel(int dbID)
		{
			bool result;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("DELETE FROM t_user_return WHERE id={0}", dbID);
				result = conn.ExecuteNonQueryBool(cmdText, 0);
			}
			return result;
		}

		
		private static object Mutex = new object();

		
		private static bool _userReturnIsOpen = false;

		
		private static ReturnActivity _activityInfo = new ReturnActivity
		{
			IsOpen = false
		};

		
		private long LastScanTicks = DateTime.Now.Ticks / 10000L;
	}
}
