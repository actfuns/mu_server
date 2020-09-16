using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using GameServer.Server;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class RechargeRepayActiveMgr
	{
		
		private static int GetBtnIndexState(int money, int minMoney, bool recode)
		{
			int ret = 0;
			if (money >= minMoney && recode)
			{
				ret = 2;
			}
			if (money >= minMoney && !recode)
			{
				ret = 1;
			}
			return ret;
		}

		
		private static string GetBtnIndexStateListStr(GameClient client, int money, ActivityTypes type, string[] records)
		{
			Activity instActivity = Global.GetActivity(type);
			string result;
			if (null == instActivity)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("GetBtnIndexStateListStr Params Error: type={0}", type), null, true);
				result = "";
			}
			else
			{
				List<int> condision = instActivity.GetAwardMinConditionlist();
				string ret = "";
				for (int i = 0; i < condision.Count; i++)
				{
					bool rec = false;
					if (i < records.Length)
					{
						rec = (records[i] == "2");
					}
					ret += RechargeRepayActiveMgr.GetBtnIndexState(money, condision[i], rec);
					if (i < condision.Count - 1)
					{
						ret += ",";
					}
				}
				result = ret;
			}
			return result;
		}

		
		public static TCPProcessCmdResults QueryAllRechargeRepayActiveInfo(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] fields = null;
			TCPProcessCmdResults result;
			if (!RechargeRepayActiveMgr.GetCmdDataField(socket, nID, data, count, out fields))
			{
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else if (fields.Length != 1)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					result = TCPProcessCmdResults.RESULT_FAILED;
				}
				else
				{
					int totalChongZhiMoney = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
					totalChongZhiMoney = Global.TransMoneyToYuanBao(totalChongZhiMoney);
					int totalChongZhiMoneyToday = GameManager.ClientMgr.QueryTotaoChongZhiMoneyToday(client);
					totalChongZhiMoneyToday = Global.TransMoneyToYuanBao(totalChongZhiMoneyToday);
					string[] dbFields = null;
					TCPProcessCmdResults retcmd = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", roleID, 39), out dbFields, client.ServerId);
					if (null == dbFields)
					{
						result = TCPProcessCmdResults.RESULT_FAILED;
					}
					else if (dbFields.Length != 3)
					{
						result = TCPProcessCmdResults.RESULT_FAILED;
					}
					else
					{
						int totalusedmoney = Global.SafeConvertToInt32(dbFields[2]);
						string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							totalChongZhiMoney,
							totalChongZhiMoneyToday,
							totalChongZhiMoney,
							totalusedmoney
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						result = TCPProcessCmdResults.RESULT_DATA;
					}
				}
			}
			return result;
		}

		
		public static TCPProcessCmdResults QueryRechargeRepayActive(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] fields = null;
			TCPProcessCmdResults result;
			if (!RechargeRepayActiveMgr.GetCmdDataField(socket, nID, data, count, out fields))
			{
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else if (fields.Length != 2)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				int roleID = Convert.ToInt32(fields[0]);
				int activeid = Global.SafeConvertToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					result = TCPProcessCmdResults.RESULT_FAILED;
				}
				else
				{
					string resoult = "";
					string[] dbFields = null;
					ActivityTypes activityTypes = (ActivityTypes)activeid;
					if (activityTypes <= ActivityTypes.MeiRiChongZhiHaoLi)
					{
						if (activityTypes != ActivityTypes.InputFirst)
						{
							switch (activityTypes)
							{
							case ActivityTypes.HeFuLogin:
								resoult = string.Format("{0}:0", Global.GetRoleParamsInt32FromDB(client, "HeFuLoginFlag").ToString());
								break;
							case ActivityTypes.HeFuTotalLogin:
								resoult = string.Format("{0}:{1}", Global.GetRoleParamsInt32FromDB(client, "HeFuTotalLoginNum").ToString(), Global.GetRoleParamsInt32FromDB(client, "HeFuTotalLoginFlag").ToString());
								break;
							case ActivityTypes.HeFuRecharge:
							{
								HeFuRechargeActivity instance = HuodongCachingMgr.GetHeFuRechargeActivity();
								if (null == instance)
								{
									resoult = string.Format("{0}:{1}", "0", "0");
								}
								else if (!instance.InActivityTime() && !instance.InAwardTime())
								{
									resoult = string.Format("{0}:{1}", "0", "0");
								}
								else
								{
									int hefuday = Global.GetOffsetDay(Global.GetHefuStartDay());
									TCPProcessCmdResults retcmd = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
									{
										fields[0],
										fields[1],
										hefuday,
										Global.GetOffsetDay(DateTime.Parse(instance.ToDate)),
										instance.strcoe
									}), out dbFields, client.ServerId);
									if (null == dbFields)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									if (dbFields == null || 4 != dbFields.Length)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									resoult = string.Format("{0}:{1}", dbFields[2], dbFields[3]);
								}
								break;
							}
							case ActivityTypes.HeFuPKKing:
								resoult = string.Format("{0}:{1}", HuodongCachingMgr.GetHeFuPKKingRoleID(), Global.GetRoleParamsInt32FromDB(client, "HeFuPKKingFlag").ToString());
								break;
							case ActivityTypes.MeiRiChongZhiHaoLi:
							{
								int totalChongZhiMoneyToday = GameManager.ClientMgr.QueryTotaoChongZhiMoneyToday(client);
								int totalChongZhiMoney = Global.TransMoneyToYuanBao(totalChongZhiMoneyToday);
								TCPProcessCmdResults retcmd = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", fields[0], 27), out dbFields, client.ServerId);
								if (null == dbFields)
								{
									return TCPProcessCmdResults.RESULT_FAILED;
								}
								if (dbFields.Length != 3)
								{
									return TCPProcessCmdResults.RESULT_FAILED;
								}
								string[] rec = dbFields[1].Split(new char[]
								{
									','
								});
								resoult = RechargeRepayActiveMgr.GetBtnIndexStateListStr(client, totalChongZhiMoney, ActivityTypes.MeiRiChongZhiHaoLi, rec);
								resoult = resoult + ":" + totalChongZhiMoney;
								break;
							}
							}
						}
						else
						{
							int totalChongZhiMoney = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
							totalChongZhiMoney = Global.TransMoneyToYuanBao(totalChongZhiMoney);
							resoult = RechargeRepayActiveMgr.GetBtnIndexState(totalChongZhiMoney, 1, !Global.CanGetFirstChongZhiDaLiByUserID(client)) + ":" + totalChongZhiMoney;
						}
					}
					else
					{
						switch (activityTypes)
						{
						case ActivityTypes.TotalCharge:
						{
							int totalChongZhiMoney = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
							totalChongZhiMoney = Global.TransMoneyToYuanBao(totalChongZhiMoney);
							TCPProcessCmdResults retcmd = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", fields[0], 38), out dbFields, client.ServerId);
							if (null == dbFields)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							if (dbFields.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							string[] rec = dbFields[1].Split(new char[]
							{
								','
							});
							resoult = RechargeRepayActiveMgr.GetBtnIndexStateListStr(client, totalChongZhiMoney, ActivityTypes.TotalCharge, rec);
							resoult = string.Format("{0}:{1}", resoult, totalChongZhiMoney);
							break;
						}
						case ActivityTypes.TotalConsume:
						{
							TCPProcessCmdResults retcmd = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", fields[0], fields[1]), out dbFields, client.ServerId);
							if (null == dbFields)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							if (dbFields.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							int totalusedmoney = Global.SafeConvertToInt32(dbFields[2]);
							string[] rec = dbFields[1].Split(new char[]
							{
								','
							});
							resoult = RechargeRepayActiveMgr.GetBtnIndexStateListStr(client, totalusedmoney, ActivityTypes.TotalConsume, rec);
							resoult = string.Format("{0}:{1}", resoult, totalusedmoney);
							break;
						}
						default:
							if (activityTypes != ActivityTypes.HeFuLuoLan)
							{
								switch (activityTypes)
								{
								case ActivityTypes.OneDollarChongZhi:
								{
									OneDollarChongZhi instance2 = HuodongCachingMgr.GetOneDollarChongZhiActivity();
									if (null == instance2)
									{
										resoult = string.Format("{0}:{1}", "0", "0");
									}
									else if (!instance2.InActivityTime() && !instance2.InAwardTime())
									{
										resoult = string.Format("{0}:{1}", "0", "0");
									}
									else
									{
										TCPProcessCmdResults retcmd = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}:{2}:{3}", new object[]
										{
											fields[0],
											46,
											instance2.FromDate.Replace(':', '$'),
											instance2.ToDate.Replace(':', '$')
										}), out dbFields, client.ServerId);
										if (null == dbFields)
										{
											return TCPProcessCmdResults.RESULT_FAILED;
										}
										if (dbFields.Length != 3)
										{
											return TCPProcessCmdResults.RESULT_FAILED;
										}
										int totalchargemoney = Global.SafeConvertToInt32(dbFields[2]);
										string[] rec = dbFields[1].Split(new char[]
										{
											','
										});
										resoult = RechargeRepayActiveMgr.GetBtnIndexStateListStr(client, totalchargemoney, ActivityTypes.OneDollarChongZhi, rec);
										resoult = string.Format("{0}:{1}", resoult, totalchargemoney);
									}
									break;
								}
								case ActivityTypes.InputFanLiNew:
								{
									InputFanLiNew instance3 = HuodongCachingMgr.GetInputFanLiNewActivity();
									if (instance3 == null || !instance3.InActivityTime())
									{
										resoult = string.Format("{0}:{1},{2}", "0", "0", "0");
									}
									else
									{
										TCPProcessCmdResults retcmd = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", fields[0], fields[1]), out dbFields, client.ServerId);
										if (null == dbFields)
										{
											return TCPProcessCmdResults.RESULT_FAILED;
										}
										if (dbFields == null || 3 != dbFields.Length)
										{
											return TCPProcessCmdResults.RESULT_FAILED;
										}
										resoult = string.Format("{0}:{1}", dbFields[1], dbFields[2]);
									}
									break;
								}
								}
							}
							else
							{
								string strHefuLuolanGuildid = GameManager.GameConfigMgr.GetGameConfigItemStr("hefu_luolan_guildid", "");
								resoult = string.Format("{0}:{1}", strHefuLuolanGuildid, Global.GetRoleParamsInt32FromDB(client, "HeFuLuoLanAwardFlag").ToString());
							}
							break;
						}
					}
					string strcmd = string.Format("{0}:{1}", resoult, activeid);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
			}
			return result;
		}

		
		public static bool CheckRechargeReplay(GameClient client, ActivityTypes type, out bool hasGet)
		{
			hasGet = false;
			try
			{
				string cmd;
				if (type == ActivityTypes.OneDollarChongZhi)
				{
					OneDollarChongZhi instance = HuodongCachingMgr.GetOneDollarChongZhiActivity();
					if (instance == null || !instance.InActivityTime() || !instance.CanGiveAward(client, 1, 0))
					{
						return false;
					}
					cmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						client.ClientData.RoleID,
						46,
						instance.FromDate.Replace(':', '$'),
						instance.ToDate.Replace(':', '$')
					});
				}
				else
				{
					cmd = string.Format("{0}:{1}", client.ClientData.RoleID, (int)type);
				}
				string[] dbFields = null;
				TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10160, cmd, out dbFields, client.ServerId);
				if (retcmd != TCPProcessCmdResults.RESULT_DATA || dbFields == null)
				{
					return false;
				}
				if (type == ActivityTypes.InputFanLiNew)
				{
					InputFanLiNew iflAct = HuodongCachingMgr.GetInputFanLiNewActivity();
					if (iflAct == null || !iflAct.InActivityTime())
					{
						return false;
					}
					int hasgettimes = Global.SafeConvertToInt32(dbFields[1]);
					string[] moneyFields = dbFields[2].Split(new char[]
					{
						','
					});
					if (moneyFields.Length != 2)
					{
						return false;
					}
					int chargeMoney = Global.SafeConvertToInt32(moneyFields[0]);
					int consumeMoney = Global.SafeConvertToInt32(moneyFields[1]);
					int nBtnIndex = iflAct.GetAwardIndex(client, chargeMoney, consumeMoney);
					if (hasgettimes > 0 || !iflAct.CanGiveAward(client, nBtnIndex, 0))
					{
						return false;
					}
					return true;
				}
				else
				{
					int extdata = Global.SafeConvertToInt32(dbFields[2]);
					string[] rec = dbFields[1].Split(new char[]
					{
						','
					});
					string resoult = RechargeRepayActiveMgr.GetBtnIndexStateListStr(client, extdata, type, rec);
					string[] retlist = resoult.Split(new char[]
					{
						','
					});
					bool flag;
					if (retlist.Length > 0)
					{
						flag = retlist.All((string x) => x.Equals("2"));
					}
					else
					{
						flag = false;
					}
					hasGet = flag;
					foreach (string st in retlist)
					{
						if (st.Equals("1"))
						{
							return true;
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				return false;
			}
			return false;
		}

		
		private static string GetFirstChargeInfo(GameClient client)
		{
			int totalChongZhiMoney = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
			string resoult = string.Concat(new object[]
			{
				Global.CanGetFirstChongZhiDaLiByUserID(client) ? 1 : 0,
				totalChongZhiMoney,
				":",
				1
			});
			return string.Format("{0}", resoult);
		}

		
		private static string GetDailyChargeActiveInfo(GameClient client)
		{
			string strcmd = "";
			int totalChongZhiMoney = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
			return strcmd;
		}

		
		private static bool GetCmdDataField(TMSKSocket socket, int nID, byte[] data, int count, out string[] fields)
		{
			string cmdData = null;
			fields = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return false;
			}
			fields = cmdData.Split(new char[]
			{
				':'
			});
			return true;
		}

		
		private static TCPProcessCmdResults GetFirstChargeAward(TMSKSocket socket, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] fields = null;
			TCPProcessCmdResults result;
			if (!RechargeRepayActiveMgr.GetCmdDataField(socket, nID, data, count, out fields))
			{
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				try
				{
					if (fields.Length != 3)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					int roleID = Convert.ToInt32(fields[0]);
					GameClient client = GameManager.ClientMgr.FindClient(socket);
					if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					string strcmd;
					if (client.ClientData.CZTaskID > 0)
					{
						strcmd = string.Format("{0}:{1}:{2}:", -10, 1, 1);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					Activity instActivity = Global.GetActivity(ActivityTypes.InputFirst);
					if (null == instActivity)
					{
						strcmd = string.Format("{0}:{1}:{2}:", -1, 1, 0);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (!Global.CanGetFirstChongZhiDaLiByUserID(client))
					{
						strcmd = string.Format("{0}:{1}:{2}:", -10, 1, 0);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (!instActivity.HasEnoughBagSpaceForAwardGoods(client))
					{
						strcmd = string.Format("{0}:{1}:{2}:", -20, 1, 0);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int totalChongZhiMoney = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
					if (totalChongZhiMoney <= 0)
					{
						strcmd = string.Format("{0}:{1}:{2}:", -30, 1, 0);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int nOcc = Global.CalcOriginalOccupationID(client);
					instActivity.GiveAward(client);
					Global.JugeCompleteChongZhiSecondTask(client, 1);
					Global.BroadcastShouChongDaLiHint(client);
					client._IconStateMgr.CheckShouCiChongZhi(client);
					strcmd = string.Format("{0}:{1}:{2}:", 0, 1, 2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
				}
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			return result;
		}

		
		public static string BuildWriteActiveRecordStr(string record, int nBtnIndex)
		{
			string activeRecord = "";
			string[] recordlist = record.Split(new char[]
			{
				','
			});
			List<string> writeRecord = new List<string>();
			int cout = nBtnIndex;
			if (nBtnIndex < recordlist.Length)
			{
				cout = recordlist.Length;
			}
			for (int i = 0; i < cout; i++)
			{
				if (i < recordlist.Length)
				{
					writeRecord.Add(recordlist[i]);
				}
				else
				{
					writeRecord.Add("1");
				}
			}
			writeRecord[nBtnIndex - 1] = "2";
			for (int i = 0; i < writeRecord.Count; i++)
			{
				activeRecord += writeRecord[i];
				if (i < writeRecord.Count - 1)
				{
					activeRecord += ",";
				}
			}
			return activeRecord;
		}

		
		public static TCPProcessCmdResults ProcessGetRepayAwardCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] fields = null;
			TCPProcessCmdResults result2;
			if (!RechargeRepayActiveMgr.GetCmdDataField(socket, nID, data, count, out fields))
			{
				result2 = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				try
				{
					int nRoleID = Convert.ToInt32(fields[0]);
					int nActivityType = Global.SafeConvertToInt32(fields[1]);
					int nBtnIndex = Convert.ToInt32(fields[2]);
					GameClient client = GameManager.ClientMgr.FindClient(socket);
					if (client == null || client.ClientData.RoleID != nRoleID)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), nRoleID), null, true);
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					Activity instActivity = Global.GetActivity((ActivityTypes)nActivityType);
					string strcmd;
					if (null == instActivity)
					{
						strcmd = string.Format("{0}:{1}::", -1, nActivityType);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int result = 0;
					string nRetValue = "";
					ActivityTypes tmpActType = (ActivityTypes)nActivityType;
					ActivityTypes activityTypes = tmpActType;
					if (activityTypes <= ActivityTypes.MeiRiChongZhiHaoLi)
					{
						if (activityTypes == ActivityTypes.InputFirst)
						{
							return RechargeRepayActiveMgr.GetFirstChargeAward(socket, pool, nID, data, count, out tcpOutPacket);
						}
						switch (activityTypes)
						{
						case ActivityTypes.HeFuLogin:
						{
							if (!instActivity.InAwardTime())
							{
								strcmd = string.Format("{0}:{1}::", -40, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (null == instActivity.GetAward(nBtnIndex))
							{
								strcmd = string.Format("{0}:{1}::", -50, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							HeFuLoginAwardType tmpType = (HeFuLoginAwardType)nBtnIndex;
							HeFuLoginFlagTypes HefuFlag;
							if (HeFuLoginAwardType.NormalAward == tmpType)
							{
								HefuFlag = HeFuLoginFlagTypes.HeFuLogin_NormalAward;
							}
							else
							{
								if (HeFuLoginAwardType.VIPAward != tmpType)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("TCPProcessCmdResults ProcessGetRepayAwardCmd 领取合服登陆奖励收到无效的领取类型 CMD={0}, Client={1}, RoleID={2}, nBtnIndex={3}", new object[]
									{
										(TCPGameServerCmds)nID,
										Global.GetSocketRemoteEndPoint(socket, false),
										nRoleID,
										nBtnIndex
									}), null, true);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								if (!Global.IsVip(client))
								{
									strcmd = string.Format("{0}:{1}::", -100, nActivityType);
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								HefuFlag = HeFuLoginFlagTypes.HeFuLogin_VIPAward;
							}
							int nFlag = Global.GetRoleParamsInt32FromDB(client, "HeFuLoginFlag");
							int nValue = Global.GetIntSomeBit(nFlag, 1);
							if (0 == nValue)
							{
								strcmd = string.Format("{0}:{1}::", -30, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							nValue = Global.GetIntSomeBit(nFlag, (int)HefuFlag);
							if (0 != nValue)
							{
								strcmd = string.Format("{0}:{1}::", -10, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (!instActivity.HasEnoughBagSpaceForAwardGoods(client, nBtnIndex))
							{
								strcmd = string.Format("{0}:{1}::", -20, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							instActivity.GiveAward(client, nBtnIndex);
							nFlag = Global.SetIntSomeBit((int)HefuFlag, nFlag, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "HeFuLoginFlag", nFlag, true);
							nRetValue = string.Format("{0}", nFlag);
							if (client._IconStateMgr.CheckHeFuActivity(client))
							{
								client._IconStateMgr.SendIconStateToClient(client);
							}
							break;
						}
						case ActivityTypes.HeFuTotalLogin:
						{
							if (!instActivity.InAwardTime())
							{
								strcmd = string.Format("{0}:{1}::", -40, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (null == instActivity.GetAward(nBtnIndex))
							{
								strcmd = string.Format("{0}:{1}::", -50, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int totalloginnum = Global.GetRoleParamsInt32FromDB(client, "HeFuTotalLoginNum");
							if (totalloginnum < nBtnIndex)
							{
								strcmd = string.Format("{0}:{1}::", -30, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int nFlag = Global.GetRoleParamsInt32FromDB(client, "HeFuTotalLoginFlag");
							int nValue = Global.GetIntSomeBit(nFlag, nBtnIndex);
							if (0 != nValue)
							{
								strcmd = string.Format("{0}:{1}::", -10, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (!instActivity.HasEnoughBagSpaceForAwardGoods(client, nBtnIndex))
							{
								strcmd = string.Format("{0}:{1}::", -20, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							instActivity.GiveAward(client, nBtnIndex);
							nFlag = Global.SetIntSomeBit(nBtnIndex, nFlag, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "HeFuTotalLoginFlag", nFlag, true);
							nRetValue = string.Format("{0}", nFlag);
							if (client._IconStateMgr.CheckHeFuActivity(client))
							{
								client._IconStateMgr.SendIconStateToClient(client);
							}
							break;
						}
						case ActivityTypes.HeFuRecharge:
						{
							int currday = Global.GetOffsetDay(TimeUtil.NowDateTime());
							int hefuday = Global.GetOffsetDay(Global.GetHefuStartDay());
							if (currday == hefuday)
							{
								strcmd = string.Format("{0}:{1}::", -40, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							HeFuRechargeActivity instance = HuodongCachingMgr.GetHeFuRechargeActivity();
							if (null == instance)
							{
								strcmd = string.Format("{0}:{1}::", -1, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (!instance.InAwardTime())
							{
								strcmd = string.Format("{0}:{1}::", -40, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string[] dbFields = null;
							Global.RequestToDBServer(tcpClientPool, pool, 10161, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								nRoleID,
								(int)tmpActType,
								hefuday,
								Global.GetOffsetDay(DateTime.Parse(instance.ToDate)),
								instance.strcoe
							}), out dbFields, client.ServerId);
							if (dbFields == null || 3 != dbFields.Length)
							{
								strcmd = string.Format("{0}:{1}::", -60, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (instance.ActivityType != Convert.ToInt32(dbFields[1]))
							{
								strcmd = string.Format("{0}:{1}::", -60, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int userrebate = Convert.ToInt32(dbFields[2]);
							if (userrebate <= 0)
							{
								strcmd = string.Format("{0}:{1}::", -30, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string huoDongKeyStr = hefuday + "_" + Global.GetOffsetDay(DateTime.Parse(instance.ToDate));
							if (!GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, userrebate, string.Format("领取{0}活动奖励", instance.ActivityType), ActivityTypes.HeFuRecharge, huoDongKeyStr))
							{
								strcmd = string.Format("{0}:{1}::", -30, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(386, new object[0]), new object[]
							{
								userrebate
							}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyErr, 0);
							GameManager.DBCmdMgr.AddDBCmd(10113, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, userrebate, string.Format("领取{0}活动奖励", instance.ActivityType)), null, client.ServerId);
							if (client._IconStateMgr.CheckHeFuActivity(client))
							{
								client._IconStateMgr.SendIconStateToClient(client);
							}
							break;
						}
						case ActivityTypes.HeFuPKKing:
						{
							if (!instActivity.InAwardTime())
							{
								strcmd = string.Format("{0}:{1}::", -40, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (nRoleID != HuodongCachingMgr.GetHeFuPKKingRoleID())
							{
								strcmd = string.Format("{0}:{1}::", -30, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int nFlag = Global.GetRoleParamsInt32FromDB(client, "HeFuPKKingFlag");
							if (nFlag != 0)
							{
								strcmd = string.Format("{0}:{1}::", -10, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							if (!instActivity.HasEnoughBagSpaceForAwardGoods(client, nBtnIndex))
							{
								strcmd = string.Format("{0}:{1}::", -20, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							instActivity.GiveAward(client);
							Global.SaveRoleParamsInt32ValueToDB(client, "HeFuPKKingFlag", 1, true);
							nRetValue = string.Format("{0}", Global.GetRoleParamsInt32FromDB(client, "HeFuPKKingFlag").ToString());
							if (client._IconStateMgr.CheckHeFuActivity(client))
							{
								client._IconStateMgr.SendIconStateToClient(client);
							}
							break;
						}
						case ActivityTypes.MeiRiChongZhiHaoLi:
						{
							if (!instActivity.HasEnoughBagSpaceForAwardGoods(client, nBtnIndex))
							{
								strcmd = string.Format("{0}:{1}::", -20, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string[] dbFields = null;
							TCPProcessCmdResults retcmd = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", nRoleID, (int)tmpActType), out dbFields, client.ServerId);
							if (dbFields == null)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							if (dbFields != null && dbFields.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							int retcode = Global.SafeConvertToInt32(dbFields[0]);
							if (retcode != 1)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							string[] retIndexarry = dbFields[1].Split(new char[]
							{
								','
							});
							if (nBtnIndex > 0 && nBtnIndex <= retIndexarry.Length && retIndexarry[nBtnIndex - 1] == "2")
							{
								strcmd = string.Format("{0}:{1}::", -10, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							AwardItem tmp = instActivity.GetAward(client, nBtnIndex);
							if (tmp == null)
							{
								strcmd = string.Format("{0}:{1}::", -1, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int totalChongZhiMoneyToday = GameManager.ClientMgr.QueryTotaoChongZhiMoneyToday(client);
							totalChongZhiMoneyToday = Global.TransMoneyToYuanBao(totalChongZhiMoneyToday);
							if (totalChongZhiMoneyToday < tmp.MinAwardCondionValue)
							{
								strcmd = string.Format("{0}:{1}::", -5, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string[] dbFields2 = null;
							string writerec = RechargeRepayActiveMgr.BuildWriteActiveRecordStr(dbFields[1], nBtnIndex);
							Global.RequestToDBServer(tcpClientPool, pool, 10161, string.Format("{0}:{1}:{2}", nRoleID, (int)tmpActType, writerec.Replace(",", "")), out dbFields2, client.ServerId);
							if (dbFields2 == null || dbFields2.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							instActivity.GiveAward(client, nBtnIndex);
							Global.BroadcastDayChongDaLiHint(client);
							client._IconStateMgr.CheckMeiRiChongZhi(client);
							nRetValue = writerec;
							break;
						}
						}
					}
					else
					{
						switch (activityTypes)
						{
						case ActivityTypes.TotalCharge:
						{
							if (!instActivity.HasEnoughBagSpaceForAwardGoods(client, nBtnIndex))
							{
								strcmd = string.Format("{0}:{1}::", -20, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string[] dbFields = null;
							TCPProcessCmdResults retcmd = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", nRoleID, (int)tmpActType), out dbFields, client.ServerId);
							if (dbFields == null)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							if (dbFields != null && dbFields.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							int retcode = Global.SafeConvertToInt32(dbFields[0]);
							if (retcode != 1)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							string[] retIndexarry = dbFields[1].Split(new char[]
							{
								','
							});
							if (nBtnIndex > 0 && nBtnIndex <= retIndexarry.Length && retIndexarry[nBtnIndex - 1] == "2")
							{
								strcmd = string.Format("{0}:{1}::", -10, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int totalMoney = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
							totalMoney = Global.TransMoneyToYuanBao(totalMoney);
							if (!instActivity.CanGiveAward(client, nBtnIndex, totalMoney))
							{
								strcmd = string.Format("{0}:{1}::", -30, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string[] dbFields2 = null;
							string writerec = RechargeRepayActiveMgr.BuildWriteActiveRecordStr(dbFields[1], nBtnIndex);
							Global.RequestToDBServer(tcpClientPool, pool, 10161, string.Format("{0}:{1}:{2}", nRoleID, 38, writerec.Replace(",", "")), out dbFields2, client.ServerId);
							if (dbFields2 == null || dbFields2.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							instActivity.GiveAward(client, nBtnIndex);
							RechargeRepayActiveMgr.BroadcastActiveHint(client, ActivityTypes.TotalCharge);
							client._IconStateMgr.CheckLeiJiChongZhi(client);
							nRetValue = writerec;
							break;
						}
						case ActivityTypes.TotalConsume:
						{
							if (!instActivity.HasEnoughBagSpaceForAwardGoods(client, nBtnIndex))
							{
								strcmd = string.Format("{0}:{1}::", -20, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string[] dbFields = null;
							TCPProcessCmdResults retcmd = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", nRoleID, 39), out dbFields, client.ServerId);
							if (dbFields == null)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							if (dbFields != null && dbFields.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							int retcode = Global.SafeConvertToInt32(dbFields[0]);
							if (retcode != 1)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							string[] retIndexarry = dbFields[1].Split(new char[]
							{
								','
							});
							if (nBtnIndex <= retIndexarry.Length && retIndexarry[nBtnIndex - 1] == "2")
							{
								strcmd = string.Format("{0}:{1}::", -10, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int totalMoney = Global.SafeConvertToInt32(dbFields[2]);
							if (!instActivity.CanGiveAward(client, nBtnIndex, totalMoney))
							{
								strcmd = string.Format("{0}:{1}::", -30, nActivityType);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string[] dbFields2 = null;
							string writerec = RechargeRepayActiveMgr.BuildWriteActiveRecordStr(dbFields[1], nBtnIndex);
							Global.RequestToDBServer(tcpClientPool, pool, 10161, string.Format("{0}:{1}:{2}", nRoleID, (int)tmpActType, writerec.Replace(",", "")), out dbFields2, client.ServerId);
							if (dbFields2 == null || dbFields2.Length != 3)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
							instActivity.GiveAward(client, nBtnIndex);
							RechargeRepayActiveMgr.BroadcastActiveHint(client, ActivityTypes.TotalConsume);
							client._IconStateMgr.CheckLeiJiXiaoFei(client);
							nRetValue = writerec;
							break;
						}
						default:
							if (activityTypes != ActivityTypes.HeFuLuoLan)
							{
								switch (activityTypes)
								{
								case ActivityTypes.OneDollarChongZhi:
								{
									if (!instActivity.HasEnoughBagSpaceForAwardGoods(client, nBtnIndex))
									{
										strcmd = string.Format("{0}:{1}::", -20, nActivityType);
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
										return TCPProcessCmdResults.RESULT_DATA;
									}
									string[] dbFields = null;
									TCPProcessCmdResults retcmd = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										nRoleID,
										(int)tmpActType,
										instActivity.FromDate.Replace(':', '$'),
										instActivity.ToDate.Replace(':', '$')
									}), out dbFields, client.ServerId);
									if (dbFields == null)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									if (dbFields != null && dbFields.Length != 3)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									int retcode = Global.SafeConvertToInt32(dbFields[0]);
									if (retcode != 1)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									string[] retIndexarry = dbFields[1].Split(new char[]
									{
										','
									});
									if (nBtnIndex > 0 && nBtnIndex <= retIndexarry.Length && retIndexarry[nBtnIndex - 1] == "2")
									{
										strcmd = string.Format("{0}:{1}::", -10, nActivityType);
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
										return TCPProcessCmdResults.RESULT_DATA;
									}
									int totalMoney = Global.SafeConvertToInt32(dbFields[2]);
									if (!instActivity.CanGiveAward(client, nBtnIndex, totalMoney))
									{
										strcmd = string.Format("{0}:{1}::", -30, nActivityType);
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
										return TCPProcessCmdResults.RESULT_DATA;
									}
									string[] dbFields2 = null;
									string writerec = RechargeRepayActiveMgr.BuildWriteActiveRecordStr(dbFields[1], nBtnIndex);
									Global.RequestToDBServer(tcpClientPool, pool, 10161, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
									{
										nRoleID,
										46,
										writerec.Replace(",", ""),
										instActivity.FromDate.Replace(':', '$'),
										instActivity.ToDate.Replace(':', '$')
									}), out dbFields2, client.ServerId);
									if (dbFields2 == null || dbFields2.Length != 3)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									instActivity.GiveAward(client);
									client._IconStateMgr.CheckOneDollarChongZhi(client);
									nRetValue = writerec;
									break;
								}
								case ActivityTypes.InputFanLiNew:
								{
									string[] dbFields = null;
									TCPProcessCmdResults retcmd = Global.RequestToDBServer(tcpClientPool, pool, 10160, string.Format("{0}:{1}", nRoleID, (int)tmpActType), out dbFields, client.ServerId);
									if (dbFields == null)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									if (dbFields != null && dbFields.Length != 3)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									int retcode = Global.SafeConvertToInt32(dbFields[0]);
									if (retcode != 1)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									int hasgettimes = Global.SafeConvertToInt32(dbFields[1]);
									if (hasgettimes > 0)
									{
										strcmd = string.Format("{0}:{1}::", -10, nActivityType);
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
										return TCPProcessCmdResults.RESULT_DATA;
									}
									string[] moneyFields = dbFields[2].Split(new char[]
									{
										','
									});
									if (moneyFields.Length != 2)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									int chargeMoney = Global.SafeConvertToInt32(moneyFields[0]);
									int consumeMoney = Global.SafeConvertToInt32(moneyFields[1]);
									InputFanLiNew iflAct = instActivity as InputFanLiNew;
									nBtnIndex = iflAct.GetAwardIndex(client, chargeMoney, consumeMoney);
									if (!instActivity.CanGiveAward(client, nBtnIndex, 0))
									{
										strcmd = string.Format("{0}:{1}::", -30, nActivityType);
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
										return TCPProcessCmdResults.RESULT_DATA;
									}
									string[] dbFields2 = null;
									Global.RequestToDBServer(tcpClientPool, pool, 10161, string.Format("{0}:{1}:{2}", nRoleID, (int)tmpActType, nBtnIndex), out dbFields2, client.ServerId);
									if (dbFields2 == null || dbFields2.Length != 3)
									{
										return TCPProcessCmdResults.RESULT_FAILED;
									}
									nRetValue = string.Format("{0}", nBtnIndex);
									instActivity.GiveAward(client, nBtnIndex);
									if (client._IconStateMgr.CheckInputFanLiNewActivity(client))
									{
										client._IconStateMgr.SendIconStateToClient(client);
									}
									break;
								}
								}
							}
							else
							{
								if (!instActivity.InAwardTime())
								{
									strcmd = string.Format("{0}:{1}::", -40, nActivityType);
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								HeFuLuoLanAward hefuLuoLanAward = (instActivity as HeFuLuoLanActivity).GetHeFuLuoLanAward(nBtnIndex);
								if (null == hefuLuoLanAward)
								{
									strcmd = string.Format("{0}:{1}::", -50, nActivityType);
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								int guildwinnum = 0;
								int chengzhuwinnum = 0;
								int guizuwinnum = 0;
								string strHefuLuolanGuildid = GameManager.GameConfigMgr.GetGameConfigItemStr("hefu_luolan_guildid", "");
								string[] strFields = strHefuLuolanGuildid.Split(new char[]
								{
									'|'
								});
								for (int i = 0; i < strFields.Length; i++)
								{
									string[] strInfos = strFields[i].Split(new char[]
									{
										','
									});
									if (2 == strInfos.Length)
									{
										if (Convert.ToInt32(strInfos[0]) == client.ClientData.Faction)
										{
											guildwinnum++;
											if (Convert.ToInt32(strInfos[1]) != client.ClientData.RoleID)
											{
												guizuwinnum++;
											}
										}
										if (Convert.ToInt32(strInfos[1]) == client.ClientData.RoleID)
										{
											chengzhuwinnum++;
										}
									}
								}
								if (guildwinnum < hefuLuoLanAward.winNum)
								{
									strcmd = string.Format("{0}:{1}::", -30, nActivityType);
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								if (1 == hefuLuoLanAward.status)
								{
									if (chengzhuwinnum < hefuLuoLanAward.winNum)
									{
										strcmd = string.Format("{0}:{1}::", -30, nActivityType);
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
										return TCPProcessCmdResults.RESULT_DATA;
									}
								}
								else if (2 == hefuLuoLanAward.status)
								{
									if (guizuwinnum < hefuLuoLanAward.winNum)
									{
										strcmd = string.Format("{0}:{1}::", -30, nActivityType);
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
										return TCPProcessCmdResults.RESULT_DATA;
									}
								}
								int nFlag = Global.GetRoleParamsInt32FromDB(client, "HeFuLuoLanAwardFlag");
								int nValue = Global.GetIntSomeBit(nFlag, nBtnIndex);
								if (0 != nValue)
								{
									strcmd = string.Format("{0}:{1}::", -10, nActivityType);
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								if (!instActivity.HasEnoughBagSpaceForAwardGoods(client, nBtnIndex))
								{
									strcmd = string.Format("{0}:{1}::", -20, nActivityType);
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								instActivity.GiveAward(client, nBtnIndex);
								nFlag = Global.SetIntSomeBit(nBtnIndex, nFlag, true);
								Global.SaveRoleParamsInt32ValueToDB(client, "HeFuLuoLanAwardFlag", nFlag, true);
								nRetValue = string.Format("{0}", nBtnIndex);
								if (client._IconStateMgr.CheckHeFuActivity(client))
								{
									client._IconStateMgr.SendIconStateToClient(client);
								}
							}
							break;
						}
					}
					strcmd = string.Format("{0}:{1}:{2}", result, nActivityType, nRetValue);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
				}
				result2 = TCPProcessCmdResults.RESULT_FAILED;
			}
			return result2;
		}

		
		public static void BroadcastActiveHint(GameClient client, ActivityTypes activeType)
		{
			string activeStr = "";
			switch (activeType)
			{
			case ActivityTypes.TotalCharge:
				activeStr = GLang.GetLang(528, new object[0]);
				break;
			case ActivityTypes.TotalConsume:
				activeStr = GLang.GetLang(529, new object[0]);
				break;
			}
			string broadCastMsg = StringUtil.substitute(GLang.GetLang(530, new object[0]), new object[]
			{
				Global.FormatRoleName(client, client.ClientData.RoleName),
				activeStr
			});
			Global.BroadcastRoleActionMsg(client, RoleActionsMsgTypes.Bulletin, broadCastMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
		}
	}
}
