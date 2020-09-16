using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic.Reborn;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class UserRegressActiveManager : IManager, ICmdProcessorEx, ICmdProcessor, IManager2
	{
		
		public static UserRegressActiveManager getInstance()
		{
			return UserRegressActiveManager.instance;
		}

		
		public bool InitConfig()
		{
			return true;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2070, 1, 1, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2071, 1, 1, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2072, 2, 2, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2073, 1, 1, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2074, 4, 4, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2075, 1, 1, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2076, 2, 2, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2077, 2, 2, UserRegressActiveManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 2070:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int RoleID = Convert.ToInt32(cmdParams[0]);
					string ToClientRegtime;
					int ToClientID;
					int ToClientLevel;
					int CurrDay;
					int ActiveMoney;
					int result = Convert.ToInt32(this.ProcessGetRegressAcitveFile(client, out ToClientRegtime, out ToClientID, out ToClientLevel, out CurrDay, out ActiveMoney));
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						result,
						ToClientRegtime,
						ToClientID,
						ToClientLevel,
						CurrDay,
						ActiveMoney
					}), false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REGRESSACTIVE_GETFILE", false, false);
				}
				break;
			case 2071:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int RoleID = Convert.ToInt32(cmdParams[0]);
					Dictionary<int, int> SignInfo;
					int result = Convert.ToInt32(this.ProcessRegressSignInfo(client, out SignInfo));
					client.sendCmd<Dictionary<int, int>>(nID, SignInfo, false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REGRESSACTIVE_GETSIGNINFO", false, false);
				}
				break;
			case 2072:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int Level = Convert.ToInt32(cmdParams[0]);
					int Day = Convert.ToInt32(cmdParams[1]);
					int result = Convert.ToInt32(this.ProcessRegressAcitveDaySignAward(client, Level, Day));
					client.sendCmd(nID, string.Format("{0}", result), false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REGRESSACTIVE_SING", false, false);
				}
				break;
			case 2073:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int RoleID = Convert.ToInt32(cmdParams[0]);
					Dictionary<int, int> GoodInfo;
					int result = Convert.ToInt32(this.ProcessRegressAcitveGetStoreInfo(client, out GoodInfo));
					client.sendCmd<Dictionary<int, int>>(nID, GoodInfo, false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REGRESSACTIVE_GETSTOREINFO", false, false);
				}
				break;
			case 2074:
				if (cmdParams == null || cmdParams.Length != 4)
				{
					return false;
				}
				try
				{
					int StoreConfID = Convert.ToInt32(cmdParams[0]);
					int Level = Convert.ToInt32(cmdParams[1]);
					int GoodsID = Convert.ToInt32(cmdParams[2]);
					int Count = Convert.ToInt32(cmdParams[3]);
					int result = Convert.ToInt32(this.ProcessRegressAcitveStore(client, StoreConfID, Level, GoodsID, Count));
					client.sendCmd(nID, string.Format("{0}", result), false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REGRESSACTIVE_STOREBUY", false, false);
				}
				break;
			case 2075:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int RoleID = Convert.ToInt32(cmdParams[0]);
					int Money;
					string ConfIDList;
					int result = Convert.ToInt32(this.ProcessRegressAcitveRechargeInfo(client, RoleID, out Money, out ConfIDList));
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, Money, ConfIDList), false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REGRESSACTIVE_INPUTINFO", false, false);
				}
				break;
			case 2076:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int Level = Convert.ToInt32(cmdParams[0]);
					int RechargeConfID = Convert.ToInt32(cmdParams[1]);
					int result = Convert.ToInt32(this.ProcessRegressAcitveRecharge(client, Level, RechargeConfID));
					client.sendCmd(nID, string.Format("{0}", result), false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REGRESSACTIVE_INPUT", false, false);
				}
				break;
			case 2077:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int RoleID = Convert.ToInt32(cmdParams[0]);
					int Level = Convert.ToInt32(cmdParams[1]);
					Dictionary<int, int> ZhiGouDict;
					int result = Convert.ToInt32(this.ProcessRegressAcitveDayBuy(client, RoleID, Level, out ZhiGouDict));
					client.sendCmd<Dictionary<int, int>>(nID, ZhiGouDict, false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_REGRESSACTIVE_ZHIGOU_QUERY", false, false);
				}
				break;
			}
			return true;
		}

		
		public static bool GetRegressMinRegtime(GameClient client, out string Regtime)
		{
			string[] dbFields = null;
			Regtime = "";
			string gameInfo = string.Format("{0}:{1}", client.ClientData.RoleID, client.strUserID);
			TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14130, gameInfo, out dbFields, client.ServerId);
			bool result;
			if (retcmd == TCPProcessCmdResults.RESULT_FAILED)
			{
				result = false;
			}
			else if (dbFields == null || dbFields.Length != 2)
			{
				result = false;
			}
			else if (!dbFields[0].Equals(client.strUserID))
			{
				result = false;
			}
			else
			{
				Regtime = dbFields[1].Replace("$", ":");
				result = true;
			}
			return result;
		}

		
		public RegressActiveOpcode ProcessGetRegressAcitveFile(GameClient client, out string ToClientRegtime, out int ToClientID, out int ToClientLevel, out int CurrDay, out int ActiveMoney)
		{
			ToClientRegtime = "";
			ToClientID = 0;
			ToClientLevel = 0;
			CurrDay = 0;
			ActiveMoney = 0;
			string UserID = client.strUserID;
			RegressActiveOpcode result;
			if (UserID == null || UserID.Equals("") || !UserID.Equals(client.strUserID))
			{
				result = RegressActiveOpcode.RegressActiveUserInfoErr;
			}
			else
			{
				RegressActiveOpen iflAct = HuodongCachingMgr.GetRegressActiveOpen();
				string Regtime;
				if (iflAct == null || !iflAct.InActivityTime())
				{
					result = RegressActiveOpcode.RegressActiveOpenErr;
				}
				else if (!iflAct.CanGiveAward())
				{
					result = RegressActiveOpcode.RegressActiveNotIn;
				}
				else if (!UserRegressActiveManager.GetRegressMinRegtime(client, out Regtime) || Regtime == null || Regtime.Equals(""))
				{
					result = RegressActiveOpcode.RegressActiveGetRegTime;
				}
				else
				{
					int ConfID;
					int Level = iflAct.GetUserActiveFile(Regtime, out ConfID);
					if (0 == Level)
					{
						result = RegressActiveOpcode.RegressActiveGetFile;
					}
					else
					{
						ToClientRegtime = Regtime.Replace(":", "$");
						ToClientID = ConfID;
						ToClientLevel = Level;
						DateTime nowDateTime = TimeUtil.NowDateTime();
						int Day = Global.GetOffsetDay(nowDateTime) - Global.GetOffsetDay(DateTime.Parse(iflAct.FromDate));
						CurrDay = Day + 1;
						string ZeroTime = "2011-11-11 00$00$00";
						string GetInfoStr = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, ZeroTime, iflAct.FromDate.Replace(':', '$'));
						string[] dbResult;
						if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14136, GetInfoStr, out dbResult, 0))
						{
							result = RegressActiveOpcode.RegressActiveGetFile;
						}
						else if (dbResult == null || dbResult.Length != 2 || Convert.ToInt32(dbResult[0]) != client.ClientData.RoleID)
						{
							result = RegressActiveOpcode.RegressActiveGetFile;
						}
						else
						{
							int Money = Convert.ToInt32(dbResult[1]);
							if (Money < 0)
							{
								Money = 0;
							}
							ActiveMoney = Money;
							result = RegressActiveOpcode.RegressActiveSucc;
						}
					}
				}
			}
			return result;
		}

		
		public Dictionary<int, int> ProcessSignInfo(Dictionary<string, string> SignInfoStr, int ThisDay, out string SignDay)
		{
			Dictionary<int, int> Info = new Dictionary<int, int>();
			SignDay = "";
			foreach (KeyValuePair<string, string> it in SignInfoStr)
			{
				if (ThisDay - Global.GetOffsetDay(DateTime.Parse(it.Key)) == 0)
				{
					SignDay = it.Value;
				}
				if (!it.Value.Equals(""))
				{
					string[] list = it.Value.Split(new char[]
					{
						'|'
					});
					foreach (string iter in list)
					{
						if (!iter.Equals(""))
						{
							int key = Convert.ToInt32(iter);
							if (Info.ContainsKey(key))
							{
								return null;
							}
							Info.Add(key, 1);
						}
					}
				}
			}
			return Info;
		}

		
		public Dictionary<int, int> ChangeData(Dictionary<string, string> SignInfoStr, Dictionary<int, int> Info)
		{
			Dictionary<int, int> Dict = new Dictionary<int, int>();
			Dictionary<int, int> result;
			if (SignInfoStr.Count == 0 && Info.Count == 0)
			{
				Dict.Add(1, 0);
				result = Dict;
			}
			else
			{
				for (int i = 1; i <= SignInfoStr.Count; i++)
				{
					if (!Info.ContainsKey(i))
					{
						Dict.Add(i, 0);
					}
					else
					{
						Dict.Add(i, Info[i]);
					}
				}
				result = Dict;
			}
			return result;
		}

		
		public RegressActiveOpcode ProcessRegressSignInfo(GameClient client, out Dictionary<int, int> SignInfo)
		{
			SignInfo = new Dictionary<int, int>();
			RegressActiveOpcode result;
			if (!client.strUserID.Equals(client.strUserID))
			{
				result = RegressActiveOpcode.RegressActiveSignGetInfoFail;
			}
			else
			{
				RegressActiveOpen iflAct = HuodongCachingMgr.GetRegressActiveOpen();
				if (iflAct == null || !iflAct.InActivityTime())
				{
					result = RegressActiveOpcode.RegressActiveOpenErr;
				}
				else
				{
					string stage = string.Format("{0}_{1}", Global.GetOffsetDay(DateTime.Parse(iflAct.FromDate)), Global.GetOffsetDay(DateTime.Parse(iflAct.ToDate)));
					string GetInfoStr = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 111, stage);
					Dictionary<string, string> SignInfoStr = Global.sendToDB<Dictionary<string, string>, string>(14132, GetInfoStr, 0);
					if (SignInfoStr == null)
					{
						result = RegressActiveOpcode.RegressActiveSignSelectFail;
					}
					else
					{
						DateTime nowDateTime = TimeUtil.NowDateTime();
						int ThisDay = Global.GetOffsetDay(nowDateTime);
						string SignStrInfo;
						Dictionary<int, int> Info = this.ProcessSignInfo(SignInfoStr, ThisDay, out SignStrInfo);
						if (Info == null)
						{
							result = RegressActiveOpcode.RegressActiveGetSignInfoErr;
						}
						else if (Info.Count > SignInfoStr.Count)
						{
							result = RegressActiveOpcode.RegressActiveGetSignInfoErr;
						}
						else
						{
							Dictionary<int, int> Dict = this.ChangeData(SignInfoStr, Info);
							SignInfo = Dict;
							result = RegressActiveOpcode.RegressActiveSucc;
						}
					}
				}
			}
			return result;
		}

		
		public RegressActiveOpcode ProcessRegressAcitveDaySignAward(GameClient client, int Level, int Day)
		{
			RegressActiveOpen iflAct = HuodongCachingMgr.GetRegressActiveOpen();
			RegressActiveOpcode result;
			string Regtime;
			if (iflAct == null || !iflAct.InActivityTime())
			{
				result = RegressActiveOpcode.RegressActiveOpenErr;
			}
			else if (!iflAct.CanGiveAward())
			{
				result = RegressActiveOpcode.RegressActiveNotIn;
			}
			else if (!UserRegressActiveManager.GetRegressMinRegtime(client, out Regtime) || Regtime == null || Regtime.Equals(""))
			{
				result = RegressActiveOpcode.RegressActiveGetRegTime;
			}
			else
			{
				int ConfID;
				int CaleLevel = iflAct.GetUserActiveFile(Regtime, out ConfID);
				if (0 == CaleLevel)
				{
					result = RegressActiveOpcode.RegressActiveGetFile;
				}
				else if (CaleLevel != Level)
				{
					result = RegressActiveOpcode.RegressActiveSignCheckFail;
				}
				else
				{
					string stage = string.Format("{0}_{1}", Global.GetOffsetDay(DateTime.Parse(iflAct.FromDate)), Global.GetOffsetDay(DateTime.Parse(iflAct.ToDate)));
					string GetInfoStr = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 111, stage);
					Dictionary<string, string> SignInfoStr = Global.sendToDB<Dictionary<string, string>, string>(14132, GetInfoStr, 0);
					if (SignInfoStr == null)
					{
						result = RegressActiveOpcode.RegressActiveSignSelectFail;
					}
					else
					{
						DateTime nowDateTime = TimeUtil.NowDateTime();
						DateTime FromActDate = new DateTime(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day, 0, 0, 0);
						DateTime ToActDate = new DateTime(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day, 23, 59, 59);
						string SignInfo;
						Dictionary<int, int> Info = this.ProcessSignInfo(SignInfoStr, Global.GetOffsetDay(nowDateTime), out SignInfo);
						if (Info == null)
						{
							result = RegressActiveOpcode.RegressActiveGetSignInfoErr;
						}
						else
						{
							Dictionary<int, int> Dict = this.ChangeData(SignInfoStr, Info);
							int state;
							if (!Dict.TryGetValue(Day, out state))
							{
								result = RegressActiveOpcode.RegressActiveSignNotDay;
							}
							else if (state == 1)
							{
								result = RegressActiveOpcode.RegressActiveSignHas;
							}
							else
							{
								RegressActiveSignGift SignAct = HuodongCachingMgr.GetRegressActiveSignGift();
								List<GoodsData> OutGoodsList;
								int DBDay;
								if (SignAct == null || !SignAct.InActivityTime())
								{
									result = RegressActiveOpcode.RegressActiveSignConfErr;
								}
								else if (!SignAct.GetAwardGoodsList(client, Level, Day, out OutGoodsList, out DBDay))
								{
									result = RegressActiveOpcode.RegressActiveSignGetAwardFail;
								}
								else
								{
									int BagInt;
									if (!RebornEquip.MoreIsCanIntoRebornOrBaseBag(client, OutGoodsList, out BagInt))
									{
										if (BagInt == 1)
										{
											return RegressActiveOpcode.RegressActiveSignRebornBagFail;
										}
										if (BagInt == 2)
										{
											return RegressActiveOpcode.RegressActiveSignBaseBagFail;
										}
									}
									string beginStr = FromActDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
									string endStr = ToActDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
									string keyHasStr = string.Format("{0}_{1}", beginStr, endStr);
									SignInfo = SignInfo + "|" + DBDay.ToString();
									string WriteInfoStr = string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										client.ClientData.RoleID,
										111,
										SignInfo,
										stage
									});
									string[] resData = Global.ExecuteDBCmd(14138, WriteInfoStr, 0);
									if (resData == null || resData.Length != 4)
									{
										result = RegressActiveOpcode.RegressActiveSignRecordFail;
									}
									else if (!SignAct.GiveAward(client, OutGoodsList))
									{
										result = RegressActiveOpcode.RegressActiveSignGiveAwardFail;
									}
									else
									{
										result = RegressActiveOpcode.RegressActiveSucc;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		
		public RegressActiveOpcode ProcessRegressAcitveRechargeInfo(GameClient client, int RoleID, out int Money, out string ConfIDList)
		{
			Money = 0;
			ConfIDList = "";
			RegressActiveOpen iflAct = HuodongCachingMgr.GetRegressActiveOpen();
			RegressActiveOpcode result;
			if (iflAct == null || !iflAct.InActivityTime())
			{
				result = RegressActiveOpcode.RegressActiveOpenErr;
			}
			else if (!iflAct.CanGiveAward())
			{
				result = RegressActiveOpcode.RegressActiveNotIn;
			}
			else
			{
				DateTime nowDateTime = TimeUtil.NowDateTime();
				string endtime = new DateTime(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day, nowDateTime.Hour, nowDateTime.Minute, nowDateTime.Second).ToString("yyyy-MM-dd HH:mm:ss");
				string GetInfoStr = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, iflAct.FromDate.Replace(':', '$'), endtime.Replace(':', '$'));
				string[] dbResult;
				if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14136, GetInfoStr, out dbResult, 0))
				{
					result = RegressActiveOpcode.RegressActiveInputGetInfoErr;
				}
				else if (dbResult == null || dbResult.Length != 2 || Convert.ToInt32(dbResult[0]) != client.ClientData.RoleID)
				{
					result = RegressActiveOpcode.RegressActiveInputGetInfoErr;
				}
				else
				{
					Money = Convert.ToInt32(dbResult[1]);
					if (Money < 0)
					{
						Money = 0;
					}
					string stage = string.Format("{0}_{1}", Global.GetOffsetDay(DateTime.Parse(iflAct.FromDate)), Global.GetOffsetDay(DateTime.Parse(iflAct.ToDate)));
					string GetAwardInfoStr = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 112, stage);
					string[] dbResult2;
					Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14137, GetAwardInfoStr, out dbResult2, 0);
					if (dbResult2 == null || dbResult2.Length != 4 || Convert.ToInt32(dbResult2[0]) != client.ClientData.RoleID)
					{
						result = RegressActiveOpcode.RegressActiveInputGetInfoErr;
					}
					else
					{
						if (!dbResult2[3].Equals(""))
						{
							string[] idList = dbResult2[3].Split(new char[]
							{
								'|'
							});
							int i = 0;
							foreach (string it in idList)
							{
								i++;
								ConfIDList += it;
								if (i == idList.Length)
								{
									break;
								}
								ConfIDList += "_";
							}
						}
						result = RegressActiveOpcode.RegressActiveSucc;
					}
				}
			}
			return result;
		}

		
		public void RoleOnlineHandler(GameClient client)
		{
			RegressActiveOpen RegressAct = HuodongCachingMgr.GetRegressActiveOpen();
			if (null != RegressAct)
			{
				RegressAct.OnRoleLogin(client);
			}
			else
			{
				RegressAct.Init();
			}
			if (RegressActiveOpen.OpenStateVavle == 1)
			{
				string stage = string.Format("{0}_{1}", Global.GetOffsetDay(DateTime.Parse(RegressAct.FromDate)), Global.GetOffsetDay(DateTime.Parse(RegressAct.ToDate)));
				string GetInfoStr = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 111, stage);
				string[] dbResult;
				Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14137, GetInfoStr, out dbResult, 0);
			}
		}

		
		public string RepairRegressStoreData(string[] idList)
		{
			string temp = "";
			string result;
			if (idList.Length == 0)
			{
				result = temp;
			}
			else
			{
				List<string> str = new List<string>();
				foreach (string it in idList)
				{
					if (!it.Equals(""))
					{
						if (-1 == str.IndexOf(it))
						{
							str.Add(it);
						}
					}
				}
				foreach (string it in str)
				{
					temp += "_";
					temp += it;
				}
				result = temp;
			}
			return result;
		}

		
		public RegressActiveOpcode ProcessRegressAcitveRecharge(GameClient client, int Level, int RechargeConfID)
		{
			RegressActiveOpen iflAct = HuodongCachingMgr.GetRegressActiveOpen();
			RegressActiveOpcode result;
			string Regtime;
			if (iflAct == null || !iflAct.InActivityTime())
			{
				result = RegressActiveOpcode.RegressActiveOpenErr;
			}
			else if (!iflAct.CanGiveAward())
			{
				result = RegressActiveOpcode.RegressActiveNotIn;
			}
			else if (!UserRegressActiveManager.GetRegressMinRegtime(client, out Regtime) || Regtime == null || Regtime.Equals(""))
			{
				result = RegressActiveOpcode.RegressActiveGetRegTime;
			}
			else
			{
				int ConfID;
				int CaleLevel = iflAct.GetUserActiveFile(Regtime, out ConfID);
				if (0 == CaleLevel)
				{
					result = RegressActiveOpcode.RegressActiveGetFile;
				}
				else if (CaleLevel != Level)
				{
					result = RegressActiveOpcode.RegressActiveGetFile;
				}
				else
				{
					DateTime nowDateTime = TimeUtil.NowDateTime();
					string endtime = new DateTime(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day, nowDateTime.Hour, nowDateTime.Minute, nowDateTime.Second).ToString("yyyy-MM-dd HH:mm:ss");
					string GetInfoStr = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, iflAct.FromDate.Replace(':', '$'), endtime.Replace(':', '$'));
					string[] dbResult;
					if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14136, GetInfoStr, out dbResult, 0))
					{
						result = RegressActiveOpcode.RegressActiveInputGetInfoErr;
					}
					else if (dbResult == null || dbResult.Length != 2 || Convert.ToInt32(dbResult[0]) != client.ClientData.RoleID)
					{
						result = RegressActiveOpcode.RegressActiveInputGetInfoErr;
					}
					else
					{
						int Money = Convert.ToInt32(dbResult[1]);
						if (Money < 0)
						{
							Money = 0;
						}
						string stage = string.Format("{0}_{1}", Global.GetOffsetDay(DateTime.Parse(iflAct.FromDate)), Global.GetOffsetDay(DateTime.Parse(iflAct.ToDate)));
						string GetAwardInfoStr = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 112, stage);
						string Repair = "";
						string[] dbResult2;
						Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14137, GetAwardInfoStr, out dbResult2, 0);
						if (dbResult2 == null || dbResult2.Length != 4 || Convert.ToInt32(dbResult2[0]) != client.ClientData.RoleID)
						{
							result = RegressActiveOpcode.RegressActiveInputGetInfoErr;
						}
						else
						{
							if (!dbResult2[3].Equals(""))
							{
								string[] idList = dbResult2[3].Split(new char[]
								{
									'_'
								});
								foreach (string it in idList)
								{
									if (!it.Equals("") && Convert.ToInt32(it) == RechargeConfID)
									{
										return RegressActiveOpcode.RegressActiveInputHas;
									}
								}
								Repair = this.RepairRegressStoreData(idList);
								if (!string.IsNullOrEmpty(Repair) && !Repair.Equals(dbResult2[3]))
								{
									string SqlStr = string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										client.ClientData.RoleID,
										112,
										Repair,
										stage
									});
									string[] SqlCmd = Global.ExecuteDBCmd(14138, SqlStr, 0);
									if (SqlCmd == null || SqlCmd.Length != 4)
									{
										return RegressActiveOpcode.RegressActiveUpdateInputInfoErr;
									}
								}
							}
							RegressActiveTotalRecharge SignAct = HuodongCachingMgr.GetRegressActiveTotalRecharge();
							List<GoodsData> goodsList;
							if (SignAct == null || !SignAct.InActivityTime())
							{
								result = RegressActiveOpcode.RegressActiveInputConfErr;
							}
							else if (SignAct.GiveAwardCheck(client, Level, Money, RechargeConfID, out goodsList))
							{
								result = RegressActiveOpcode.RegressActiveInputCheckAwardErr;
							}
							else if (goodsList == null)
							{
								result = RegressActiveOpcode.RegressActiveInputCheckAwardErr;
							}
							else
							{
								int BagInt;
								if (!RebornEquip.MoreIsCanIntoRebornOrBaseBag(client, goodsList, out BagInt))
								{
									if (BagInt == 1)
									{
										return RegressActiveOpcode.RegressActiveSignRebornBagFail;
									}
									if (BagInt == 2)
									{
										return RegressActiveOpcode.RegressActiveSignBaseBagFail;
									}
								}
								DateTime FromActDate = new DateTime(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day, 0, 0, 0);
								DateTime ToActDate = new DateTime(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day, 23, 59, 59);
								string beginStr = FromActDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
								string endStr = ToActDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
								string keyHasStr = string.Format("{0}_{1}", beginStr, endStr);
								string ativedata = Repair + "_" + RechargeConfID.ToString();
								string WriteInfoStr = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									client.ClientData.RoleID,
									112,
									ativedata,
									stage
								});
								string[] resData = Global.ExecuteDBCmd(14138, WriteInfoStr, 0);
								if (resData == null || resData.Length != 4)
								{
									result = RegressActiveOpcode.RegressActiveUpdateInputInfoErr;
								}
								else if (!SignAct.GiveAward(client, goodsList))
								{
									result = RegressActiveOpcode.RegressActiveInputGiveAwardErr;
								}
								else
								{
									result = RegressActiveOpcode.RegressActiveSucc;
								}
							}
						}
					}
				}
			}
			return result;
		}

		
		public RegressActiveOpcode ProcessRegressAcitveDayBuy(GameClient client, int Role, int Level, out Dictionary<int, int> ZhiGouDict)
		{
			ZhiGouDict = null;
			RegressActiveOpen iflAct = HuodongCachingMgr.GetRegressActiveOpen();
			RegressActiveOpcode result;
			string Regtime;
			if (iflAct == null || !iflAct.InActivityTime())
			{
				result = RegressActiveOpcode.RegressActiveOpenErr;
			}
			else if (!iflAct.CanGiveAward())
			{
				result = RegressActiveOpcode.RegressActiveNotIn;
			}
			else if (!UserRegressActiveManager.GetRegressMinRegtime(client, out Regtime) || Regtime == null || Regtime.Equals(""))
			{
				result = RegressActiveOpcode.RegressActiveGetRegTime;
			}
			else
			{
				int ConfID;
				int CaleLevel = iflAct.GetUserActiveFile(Regtime, out ConfID);
				if (0 == CaleLevel)
				{
					result = RegressActiveOpcode.RegressActiveGetFile;
				}
				else if (CaleLevel != Level)
				{
					result = RegressActiveOpcode.RegressActiveGetFile;
				}
				else
				{
					Dictionary<int, int> ZhiGouInfoDict = new Dictionary<int, int>();
					RegressActiveDayBuy act = HuodongCachingMgr.GetRegressActiveDayBuy();
					if (null == act)
					{
						result = RegressActiveOpcode.RegressActiveBuyGetInfoErr;
					}
					else
					{
						ZhiGouInfoDict = act.BuildRegressZhiGouInfoForClient(client);
						ZhiGouDict = ZhiGouInfoDict;
						result = RegressActiveOpcode.RegressActiveSucc;
					}
				}
			}
			return result;
		}

		
		public RegressActiveOpcode ProcessRegressAcitveGetStoreInfo(GameClient client, out Dictionary<int, int> GoodInfo)
		{
			GoodInfo = null;
			RegressActiveOpen iflAct = HuodongCachingMgr.GetRegressActiveOpen();
			RegressActiveOpcode result;
			if (iflAct == null || !iflAct.InActivityTime())
			{
				result = RegressActiveOpcode.RegressActiveOpenErr;
			}
			else
			{
				DateTime nowDateTime = TimeUtil.NowDateTime();
				string CurrDate = new DateTime(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day, nowDateTime.Hour, nowDateTime.Minute, nowDateTime.Second).ToString("yyyy-MM-dd HH:mm:ss");
				int CDate = Global.GetOffsetDay(DateTime.Parse(CurrDate)) - Global.GetOffsetDay(DateTime.Parse(iflAct.FromDate));
				if (CDate < 0)
				{
					result = RegressActiveOpcode.RegressActiveStoreCheckDayFail;
				}
				else
				{
					string stage = string.Format("{0}_{1}", Global.GetOffsetDay(DateTime.Parse(iflAct.FromDate)), Global.GetOffsetDay(DateTime.Parse(iflAct.ToDate)));
					string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, CDate, stage);
					GoodInfo = Global.sendToDB<Dictionary<int, int>, string>(14133, strcmd, 0);
					result = RegressActiveOpcode.RegressActiveSucc;
				}
			}
			return result;
		}

		
		public RegressActiveOpcode ProcessRegressAcitveStore(GameClient client, int StoreConfID, int Level, int GoodsID, int Count)
		{
			RegressActiveOpen iflAct = HuodongCachingMgr.GetRegressActiveOpen();
			RegressActiveOpcode result;
			string Regtime;
			if (iflAct == null || !iflAct.InActivityTime())
			{
				result = RegressActiveOpcode.RegressActiveOpenErr;
			}
			else if (!iflAct.CanGiveAward())
			{
				result = RegressActiveOpcode.RegressActiveNotIn;
			}
			else if (!UserRegressActiveManager.GetRegressMinRegtime(client, out Regtime) || Regtime == null || Regtime.Equals(""))
			{
				result = RegressActiveOpcode.RegressActiveGetRegTime;
			}
			else
			{
				int ConfID;
				int CaleLevel = iflAct.GetUserActiveFile(Regtime, out ConfID);
				if (0 == CaleLevel)
				{
					result = RegressActiveOpcode.RegressActiveGetFile;
				}
				else if (CaleLevel != Level)
				{
					result = RegressActiveOpcode.RegressActiveStoreCheckFail;
				}
				else
				{
					DateTime nowDateTime = TimeUtil.NowDateTime();
					string CurrDate = new DateTime(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day, nowDateTime.Hour, nowDateTime.Minute, nowDateTime.Second).ToString("yyyy-MM-dd HH:mm:ss");
					int CDate = Global.GetOffsetDay(DateTime.Parse(CurrDate)) - Global.GetOffsetDay(DateTime.Parse(iflAct.FromDate));
					if (CDate < 0)
					{
						result = RegressActiveOpcode.RegressActiveStoreCheckDayFail;
					}
					else
					{
						RegressActiveStore StoreAct = HuodongCachingMgr.GetRegressActiveStore();
						if (null == StoreAct)
						{
							result = RegressActiveOpcode.RegressActiveStoreConfErr;
						}
						else
						{
							string stage = string.Format("{0}_{1}", Global.GetOffsetDay(DateTime.Parse(iflAct.FromDate)), Global.GetOffsetDay(DateTime.Parse(iflAct.ToDate)));
							int needYuanBao;
							int Sum;
							GoodsData goodData;
							if (!StoreAct.RegressStoreGoodsBuyCheck(client, StoreConfID, Level, CDate + 1, GoodsID, Count, stage, out needYuanBao, out Sum, out goodData))
							{
								result = RegressActiveOpcode.RegressActiveStoreBuyFail;
							}
							else if (goodData == null)
							{
								result = RegressActiveOpcode.RegressActiveStoreCheckGoodFail;
							}
							else if (Sum <= 0 || needYuanBao <= 0)
							{
								result = RegressActiveOpcode.RegressActiveStoreCheckParmErr;
							}
							else
							{
								int BagInt;
								if (!RebornEquip.OneIsCanIntoRebornOrBaseBag(client, goodData, out BagInt))
								{
									if (BagInt == 1)
									{
										return RegressActiveOpcode.RegressActiveSignRebornBagFail;
									}
									if (BagInt == 2)
									{
										return RegressActiveOpcode.RegressActiveSignBaseBagFail;
									}
								}
								if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, needYuanBao, "三周年商城购买物品", true, true, false, DaiBiSySType.None))
								{
									result = RegressActiveOpcode.RegressActiveStoreUserYuanBaoFail;
								}
								else
								{
									string GetInfoStr = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
									{
										client.ClientData.RoleID,
										StoreConfID,
										CDate,
										Sum,
										stage
									});
									string[] dbResult;
									if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14135, GetInfoStr, out dbResult, 0))
									{
										result = RegressActiveOpcode.RegressActiveStoreInsertInfoErr;
									}
									else if (dbResult == null || dbResult.Length != 2 || Convert.ToInt32(dbResult[1]) != 0)
									{
										result = RegressActiveOpcode.RegressActiveStoreInsertInfoErr;
									}
									else
									{
										if (Global.GetGoodsRebornEquip(goodData.GoodsID) == 1)
										{
											Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodData.GoodsID, Count, goodData.Quality, goodData.Props, goodData.Forge_level, goodData.Binding, 15000, goodData.Jewellist, true, 1, "三周年商城购买", false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
										}
										else
										{
											Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodData.GoodsID, Count, goodData.Quality, goodData.Props, goodData.Forge_level, goodData.Binding, 0, goodData.Jewellist, true, 1, "三周年商城购买", false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
										}
										result = RegressActiveOpcode.RegressActiveSucc;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		
		private static UserRegressActiveManager instance = new UserRegressActiveManager();
	}
}
