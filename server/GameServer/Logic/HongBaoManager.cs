using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class HongBaoManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		
		public static HongBaoManager getInstance()
		{
			return HongBaoManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("HongBaoManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 1000);
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1420, 0, 1, HongBaoManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1421, 1, 3, HongBaoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1422, 2, 3, HongBaoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1423, 4, 4, HongBaoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1424, 4, 4, HongBaoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1428, 2, 2, HongBaoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1429, 1, 1, HongBaoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(14, HongBaoManager.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(14, HongBaoManager.getInstance());
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
			case 1420:
				return this.ProcessFaHongBaoCmd(client, nID, bytes, cmdParams);
			case 1421:
				return this.GetHongBaoRankCmd(client, nID, bytes, cmdParams);
			case 1422:
				return this.ProcessGetHongBaoDataListCmd(client, nID, bytes, cmdParams);
			case 1423:
				return this.ShowHongBaoCmd(client, nID, bytes, cmdParams);
			case 1424:
				return this.GetHongBaoDetailCmd(client, nID, bytes, cmdParams);
			case 1428:
				return this.GetJirRiHongBaoBangAwardsCmd(client, nID, bytes, cmdParams);
			case 1429:
				return this.GetJirRiHongBaoBangDataCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		
		public void processEvent(EventObject eventObject)
		{
			int nID = eventObject.getEventType();
			int num = nID;
			if (num == 14)
			{
				PlayerInitGameEventObject playerInitGameEventObject = eventObject as PlayerInitGameEventObject;
				if (null != playerInitGameEventObject)
				{
					this.OnInitGame(playerInitGameEventObject.getPlayer());
				}
			}
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num == 10001)
			{
				KuaFuNotifyEnterGameEvent e = eventObject as KuaFuNotifyEnterGameEvent;
				if (null != e)
				{
					KuaFuServerLoginData kuaFuServerLoginData = e.Arg as KuaFuServerLoginData;
					if (null != kuaFuServerLoginData)
					{
						lock (this.RuntimeData.Mutex)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("通知角色ID={0}拥有进入勇者战场资格,跨服GameID={1}", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
						}
					}
					eventObject.Handled = true;
				}
			}
		}

		
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.RedPacketsTime = (int)GameManager.systemParamsList.GetParamValueIntByName("RedPacketsTime", -1);
					this.RuntimeData.RedPacketsNumMax = (int)GameManager.systemParamsList.GetParamValueIntByName("RedPacketsNumMax", -1);
					this.RuntimeData.RedPacketsRight = (int)GameManager.systemParamsList.GetParamValueIntByName("RedPacketsRight", -1);
					this.RuntimeData.RedPacketsMessage = (int)GameManager.systemParamsList.GetParamValueIntByName("RedPacketsMessage", -1);
					this.RuntimeData.RedPacketsInfomationLimit = GameManager.systemParamsList.GetParamValueIntArrayByName("RedPacketsInfomationLimit", ',');
					this.RuntimeData.RedPacketsLimit = GameManager.systemParamsList.GetParamValueIntArrayByName("RedPacketsLimit", ',');
					this.RuntimeData.RedPacketsRequest = (int)GameManager.systemParamsList.GetParamValueIntByName("RedPacketsRequest", -1);
					this.RuntimeData.RedPacketsAutomaticRecordMax = (int)GameManager.systemParamsList.GetParamValueIntByName("RedPacketsAutomaticRecordMax", -1);
					this.RuntimeData.RedPacketsQuanMinMessage = GameManager.systemParamsList.GetParamValueByName("RedPacketsQuanMinMessage");
					this.RuntimeData.RedPacketsChongZhiMessage = GameManager.systemParamsList.GetParamValueByName("RedPacketsChongZhiMessage");
					this.RuntimeData.RedPacketsTeQuanMessage = GameManager.systemParamsList.GetParamValueByName("RedPacketsTeQuanMessage");
					JieRiChongZhiHongBaoActivity.getInstance().Init();
					JieRiHongBaoActivity.getInstance().Init();
					JieriHongBaoKingActivity.getInstance().Init();
					this.RuntimeData.Initialized = false;
					this.RuntimeData.ZhanMengHongBaoInitialized = false;
					this.RuntimeData.JieRiHongBaoInitialized = false;
					this.RuntimeData.JieRiHongBaoBangInitialized = false;
				}
				catch (Exception ex)
				{
					success = false;
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		
		public bool ProcessGetHongBaoDataListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				MyHongBaoData data = new MyHongBaoData();
				int type = Global.SafeConvertToInt32(cmdParams[1]);
				long age = Global.SafeConvertToInt64(cmdParams[2]);
				data.type = type;
				data.flag = 1L;
				if (this.IsGongNengOpened(client, false))
				{
					int rid = client.ClientData.RoleID;
					int bhid = client.ClientData.Faction;
					if (bhid > 0 && type >= 0 && type <= 2)
					{
						if (this.RuntimeData.ZhanMengHongBaoInitialized)
						{
							int secs = Global.GetRoleParamsInt32FromDB(client, "EnterBangHuiUnixSecs");
							long enterTicks = TimeUtil.UnixSecondsToTicks(secs) * 10000L;
							DateTime now = TimeUtil.NowDateTime();
							long nowTicks = TimeUtil.NOW();
							lock (this.RuntimeData.Mutex)
							{
								data.flag = TimeUtil.NOW();
								data.items = new List<HongBaoItemData>();
								ZhanMengHongBaoData listData;
								if (this.RuntimeData.ZhanMengHongBaoDict.TryGetValue(bhid, out listData))
								{
									foreach (HongBaoSendData hongbao in listData.HongBaoList)
									{
										if (enterTicks < hongbao.sendTime.Ticks)
										{
											HongBaoItemData item2 = new HongBaoItemData();
											item2.sender = hongbao.sender;
											item2.beginTime = hongbao.sendTime;
											item2.endTime = hongbao.endTime;
											item2.diamondSumCount = hongbao.sumDiamondNum;
											item2.hongBaoID = hongbao.hongBaoID;
											item2.type = hongbao.type;
											item2.hongBaoStatus = hongbao.hongBaoStatus;
											HongBaoRecvData recvData = null;
											if (null != hongbao.RecvList)
											{
												recvData = hongbao.RecvList.Find((HongBaoRecvData x) => x.RoleId == client.ClientData.RoleID);
											}
											if (null != recvData)
											{
												item2.diamondCount = recvData.ZuanShi;
												item2.hongBaoStatus = 1;
											}
											if (type > 0 || item2.hongBaoStatus == 0)
											{
												if (hongbao.leftCount <= 0)
												{
													item2.hongBaoStatus = 3;
												}
												else if (item2.endTime <= now)
												{
													item2.hongBaoStatus = 2;
												}
											}
											if (type == 0)
											{
												if (item2.hongBaoStatus == 0)
												{
													data.items.Add(item2);
												}
											}
											else if (type == 1)
											{
												if (hongbao.senderID == rid)
												{
													data.items.Add(item2);
												}
											}
											else
											{
												data.items.Add(item2);
											}
										}
									}
								}
							}
							data.items.Sort((HongBaoItemData x, HongBaoItemData y) => y.hongBaoID - x.hongBaoID);
							int maxCount = this.RuntimeData.RedPacketsInfomationLimit[type];
							if (data.items.Count >= maxCount)
							{
								if (data.items.Count > maxCount)
								{
									data.items.RemoveRange(maxCount, data.items.Count - maxCount);
								}
							}
							else
							{
								if (client.ClientData.UpdateHongBaoLogTicks[type] <= nowTicks)
								{
									List<string> queryData = new List<string>
									{
										type.ToString(),
										bhid.ToString(),
										rid.ToString(),
										this.RuntimeData.RedPacketsInfomationLimit[type].ToString()
									};
									client.ClientData.UpdateHongBaoLogTicks[type] = nowTicks + (long)(this.RuntimeData.RedPacketsRequest * 1000);
									client.ClientData.HongBaoLogLists[type] = Global.sendToDB<List<HongBaoItemData>, List<string>>(1438, queryData, client.ServerId);
								}
								List<HongBaoItemData> list = client.ClientData.HongBaoLogLists[type];
								if (null != list)
								{
									using (List<HongBaoItemData>.Enumerator enumerator2 = list.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											HongBaoItemData item = enumerator2.Current;
											if (data.items.Count >= this.RuntimeData.RedPacketsInfomationLimit[type])
											{
												break;
											}
											if (enterTicks <= item.beginTime.Ticks)
											{
												if (!data.items.Exists((HongBaoItemData x) => x.hongBaoID == item.hongBaoID))
												{
													lock (this.RuntimeData.Mutex)
													{
														HongBaoSendData sendData;
														if (!this.RuntimeData.OldHongBaoDict.TryGetValue(item.hongBaoID, out sendData))
														{
															sendData = new HongBaoSendData();
															sendData.hongBaoID = item.hongBaoID;
															sendData.hongBaoStatus = -1;
															this.RuntimeData.OldHongBaoDict[item.hongBaoID] = sendData;
														}
													}
													data.items.Add(item);
													if (type == 0 && item.diamondCount > 0)
													{
														item.hongBaoStatus = 1;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				client.sendCmd<MyHongBaoData>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool GetHongBaoRankCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				HongBaoRankData rankData = new HongBaoRankData();
				TCPOutPacket packet = null;
				int type = Global.SafeConvertToInt32(cmdParams[1]);
				long age = Global.SafeConvertToInt64(cmdParams[2]);
				if (type == 0 || type == 1)
				{
					if (!this.IsGongNengOpened(client, false) || !this.RuntimeData.ZhanMengHongBaoInitialized)
					{
						packet = DataHelper.ObjectToTCPOutPacket<HongBaoRankData>(rankData, TCPOutPacketPool.getInstance(), nID);
					}
					else
					{
						int bhid = client.ClientData.Faction;
						rankData.type = type;
						rankData.flag = 1L;
						long nowTicks = TimeUtil.NOW();
						lock (this.RuntimeData.Mutex)
						{
							ZhanMengHongBaoData data;
							if (this.RuntimeData.ZhanMengHongBaoDict.TryGetValue(bhid, out data))
							{
								if (nowTicks < data.LastUpdateTicks[type] + (long)this.RuntimeData.LoadFromDBInterval2)
								{
									if (type == 0)
									{
										rankData.items = data.RecvRankList;
									}
									else
									{
										rankData.items = data.SendRankList;
									}
									rankData.flag = data.LastUpdateTicks[type];
									packet = DataHelper.ObjectToTCPOutPacket<HongBaoRankData>(rankData, TCPOutPacketPool.getInstance(), nID);
									goto IL_25A;
								}
							}
						}
						List<HongBaoRankItemData> list = Global.sendToDB<List<HongBaoRankItemData>, ZhanMengHongBaoRankListQueryData>(1430, new ZhanMengHongBaoRankListQueryData
						{
							Bhid = bhid,
							Count = this.RuntimeData.RedPacketsRankLimit,
							Type = type,
							StartTime = TimeUtil.GetWeekStartTimeNow()
						}, client.ServerId);
						lock (this.RuntimeData.Mutex)
						{
							ZhanMengHongBaoData data;
							if (!this.RuntimeData.ZhanMengHongBaoDict.TryGetValue(bhid, out data))
							{
								data = new ZhanMengHongBaoData();
								this.RuntimeData.ZhanMengHongBaoDict[bhid] = data;
							}
							data.LastUpdateTicks[type] = nowTicks;
							if (type == 0)
							{
								rankData.items = (data.RecvRankList = list);
							}
							else
							{
								rankData.items = (data.SendRankList = list);
							}
							rankData.flag = data.LastUpdateTicks[type];
							packet = DataHelper.ObjectToTCPOutPacket<HongBaoRankData>(rankData, TCPOutPacketPool.getInstance(), nID);
						}
					}
				}
				IL_25A:
				if (null != packet)
				{
					client.sendCmd(packet, true);
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessFaHongBaoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				if (GameManager.IsKuaFuServer)
				{
					result = -12;
				}
				else
				{
					FaHongBaoData data = DataHelper.BytesToObject<FaHongBaoData>(bytes, 0, bytes.Length);
					if (null == data)
					{
						result = -18;
					}
					else if (!this.RuntimeData.Initialized)
					{
						result = -11000;
					}
					else
					{
						if (data.type == 0)
						{
							if (data.diamondNum < data.count || data.diamondNum % data.count != 0)
							{
								result = -18;
								goto IL_5B2;
							}
						}
						else
						{
							if (data.type != 1)
							{
								result = -18;
								goto IL_5B2;
							}
							if (data.diamondNum < data.count)
							{
								result = -18;
								goto IL_5B2;
							}
						}
						if (data.count < this.RuntimeData.RedPacketsLimit[0] || data.count > this.RuntimeData.RedPacketsLimit[1] || data.diamondNum < this.RuntimeData.RedPacketsLimit[2] || data.diamondNum > this.RuntimeData.RedPacketsLimit[3])
						{
							result = -18;
						}
						else
						{
							int bhid = client.ClientData.Faction;
							if (bhid <= 0)
							{
								result = -1033;
							}
							else
							{
								if (!string.IsNullOrEmpty(data.message))
								{
									if (data.message.Length > 60)
									{
										result = -18;
										goto IL_5B2;
									}
									result = NameServerNamager.CheckInvalidCharacters(data.message, false);
									if (result < 0)
									{
										result = -40;
										goto IL_5B2;
									}
								}
								DateTime now = TimeUtil.NowDateTime();
								int nowHour = TimeUtil.UnixSecondsNow() / 3600 % 10000;
								int sendFlags = Global.GetRoleParamsInt32FromDB(client, "10199");
								int hour = sendFlags / 10000;
								int sendNum = sendFlags % 10000;
								if (hour != nowHour)
								{
									hour = nowHour;
									sendNum = 0;
								}
								if (sendNum >= this.RuntimeData.RedPacketsNumMax)
								{
									result = -41;
								}
								else
								{
									sendFlags = hour * 10000 + sendNum + 1;
									if (!this.IsGongNengOpened(client, false))
									{
										result = -12;
									}
									else if (this.RuntimeData.RedPacketsRight > 0 && (!Global.CanTrade(client) || Global.TradeLevelLimit(client)))
									{
										result = -37;
									}
									else if (client.ClientData.UserMoney < data.diamondNum)
									{
										result = -10;
									}
									else if (!GameManager.ClientMgr.SubUserMoney(client, data.diamondNum, "发战盟红包", false, false, false, true, DaiBiSySType.None))
									{
										result = -10;
									}
									else
									{
										HongBaoSendData hongbao = new HongBaoSendData();
										hongbao.bhid = bhid;
										hongbao.type = data.type;
										hongbao.senderID = client.ClientData.RoleID;
										hongbao.sender = Global.FormatRoleName4(client);
										hongbao.sendTime = now;
										hongbao.endTime = now.AddHours((double)this.RuntimeData.RedPacketsTime);
										hongbao.message = data.message;
										hongbao.sumDiamondNum = data.diamondNum;
										hongbao.sumCount = data.count;
										hongbao.leftZuanShi = data.diamondNum;
										hongbao.leftCount = data.count;
										int hongbaoId = Global.sendToDB<int, HongBaoSendData>(1432, hongbao, client.ServerId);
										if (hongbaoId <= 0)
										{
											if (!GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, data.diamondNum, "发战盟红包失败退回", ActivityTypes.None, ""))
											{
												LogManager.WriteLog(LogTypes.System, string.Format("发战盟红包失败#退回钻石失败#zuanshi={0}", data.diamondNum), null, true);
											}
											result = -15;
										}
										else
										{
											lock (this.RuntimeData.Mutex)
											{
												hongbao.hongBaoID = hongbaoId;
												ZhanMengHongBaoData zhanMengHongBaoData;
												if (!this.RuntimeData.ZhanMengHongBaoDict.TryGetValue(bhid, out zhanMengHongBaoData))
												{
													zhanMengHongBaoData = new ZhanMengHongBaoData();
													this.RuntimeData.ZhanMengHongBaoDict[bhid] = zhanMengHongBaoData;
												}
												zhanMengHongBaoData.HongBaoList.Add(hongbao);
												this.RuntimeData.HongBaoDict[hongbao.hongBaoID] = hongbao;
												this.AddFaHongBaoRank(client, zhanMengHongBaoData, 1, data.diamondNum);
											}
											Global.UpdateRoleParamByName(client, "10199", sendFlags.ToString(), true);
											GameManager.ClientMgr.SendBangHuiCmd<string>(bhid, 1425, string.Format("{0}:{1}", hongbao.type, hongbao.hongBaoID), true, false);
											client.ClientData.UpdateHongBaoLogTicks[1] = 0L;
											client.ClientData.UpdateHongBaoLogTicks[2] = 0L;
											int salePrice = data.diamondNum;
											int tradelog_num_minamount = GameManager.GameConfigMgr.GetGameConfigItemInt("tradelog_num_minamount", 5000);
											if (salePrice >= tradelog_num_minamount)
											{
												GameManager.logDBCmdMgr.AddTradeNumberInfo(3, salePrice, 0, client.ClientData.RoleID, client.ServerId);
											}
											int freqNumber = Global.IncreaseTradeCount(client, "SaleTradeDayID", "SaleTradeCount", 1);
											int tradelog_freq_sale = GameManager.GameConfigMgr.GetGameConfigItemInt("tradelog_freq_sale", 10);
											if (freqNumber >= tradelog_freq_sale)
											{
												GameManager.logDBCmdMgr.AddTradeFreqInfo(3, freqNumber, client.ClientData.RoleID, 0);
											}
										}
									}
								}
							}
						}
					}
				}
				IL_5B2:
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ShowHongBaoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleId = client.ClientData.RoleID;
				int type = Global.SafeConvertToInt32(cmdParams[1]);
				int hongBaoId = Global.SafeConvertToInt32(cmdParams[2]);
				int tips = Global.SafeConvertToInt32(cmdParams[3]);
				ShowHongBaoData showHongBaoData = new ShowHongBaoData();
				showHongBaoData.hongBaoID = hongBaoId;
				showHongBaoData.type = type;
				showHongBaoData.tips = tips;
				if (!this.RuntimeData.Initialized)
				{
					showHongBaoData.result = -11000;
				}
				else
				{
					DateTime now = TimeUtil.NowDateTime();
					long nowTicks = TimeUtil.NOW();
					if (type == 101)
					{
						lock (this.RuntimeData.Mutex)
						{
							SystemHongBaoData sendData;
							if (!this.RuntimeData.ChongZhiHongBaoDict.TryGetValue(hongBaoId, out sendData) || sendData.LeftZuanShi <= 0 || sendData.StartTime + (long)sendData.DurationTime < nowTicks)
							{
								showHongBaoData.result = 3;
							}
						}
						showHongBaoData.message = this.RuntimeData.RedPacketsChongZhiMessage;
					}
					else if (type == 102)
					{
						lock (this.RuntimeData.Mutex)
						{
							HongBaoSendData sendData2;
							if (!this.RuntimeData.JieRiHongBaoDict.TryGetValue(hongBaoId, out sendData2) || sendData2.leftZuanShi <= 0 || sendData2.endTime < now)
							{
								showHongBaoData.result = 3;
							}
						}
						showHongBaoData.message = this.RuntimeData.RedPacketsQuanMinMessage;
					}
					else if (type == 103)
					{
						lock (this.RuntimeData.Mutex)
						{
							HongBaoSendData sendData2;
							if (!this.RuntimeData.SpecPHongBaoDict.TryGetValue(hongBaoId, out sendData2) || sendData2.leftZuanShi <= 0 || sendData2.endTime < now)
							{
								showHongBaoData.result = 3;
							}
						}
						showHongBaoData.message = this.RuntimeData.RedPacketsTeQuanMessage;
					}
					else
					{
						lock (this.RuntimeData.Mutex)
						{
							HongBaoSendData sendData2;
							if (!this.RuntimeData.HongBaoDict.TryGetValue(hongBaoId, out sendData2))
							{
								showHongBaoData.result = 3;
							}
							else
							{
								showHongBaoData.type = sendData2.type;
								showHongBaoData.message = sendData2.message;
								showHongBaoData.sender = sendData2.sender;
								showHongBaoData.SumHongBaoNum = sendData2.sumCount;
								showHongBaoData.yiLingNum = sendData2.sumCount - sendData2.leftCount;
								if (sendData2.leftCount <= 0)
								{
									showHongBaoData.result = 3;
								}
								else if (now >= sendData2.endTime)
								{
									showHongBaoData.result = 2;
								}
							}
						}
					}
				}
				client.sendCmd<ShowHongBaoData>(nID, showHongBaoData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		
		public bool GetHongBaoDetailCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleId = client.ClientData.RoleID;
				int bhid = client.ClientData.Faction;
				int type = Global.SafeConvertToInt32(cmdParams[1]);
				int hongBaoId = Global.SafeConvertToInt32(cmdParams[2]);
				int operation = Global.SafeConvertToInt32(cmdParams[3]);
				HongBaoDeatilsData data = new HongBaoDeatilsData();
				data.hongBaoID = hongBaoId;
				data.type = type;
				if (!this.RuntimeData.Initialized)
				{
					data.hongBaoStatus = -11000;
				}
				else if (!this.IsGongNengOpened(client, false))
				{
					data.hongBaoStatus = -12;
				}
				else
				{
					DateTime now = TimeUtil.NowDateTime();
					long nowTicks = TimeUtil.NOW();
					lock (this.RuntimeData.Mutex)
					{
						if (type == 101)
						{
							data.message = this.RuntimeData.RedPacketsChongZhiMessage;
							SystemHongBaoData hongBaoData;
							if (!this.RuntimeData.ChongZhiHongBaoDict.TryGetValue(hongBaoId, out hongBaoData) || hongBaoData.LeftZuanShi <= 0)
							{
								data.hongBaoStatus = 3;
							}
							else if (hongBaoData.StartTime > nowTicks)
							{
								data.hongBaoStatus = 3;
							}
							else
							{
								List<HongBaoRecvData> list;
								if (!this.RuntimeData.ChongZhiHongBaoRecvDict.TryGetValue(hongBaoId, out list))
								{
									list = new List<HongBaoRecvData>();
									this.RuntimeData.ChongZhiHongBaoRecvDict[hongBaoId] = list;
								}
								HongBaoRecvData recvData = list.Find((HongBaoRecvData x) => x.RoleId == roleId);
								if (null == recvData)
								{
									int zuanShi = JunTuanClient.getInstance().OpenHongBao(hongBaoData.HongBaoId, roleId, client.ClientData.ZoneID, client.strUserID, client.ClientData.RoleName);
									if (hongBaoData.LeftZuanShi < zuanShi)
									{
										zuanShi = hongBaoData.LeftZuanShi;
									}
									if (zuanShi > 0)
									{
										recvData = new HongBaoRecvData();
										recvData.HongBaoID = hongBaoId;
										recvData.RoleId = roleId;
										recvData.ZuanShi = zuanShi;
										recvData.RecvTime = now;
										list.Add(recvData);
										hongBaoData.LeftZuanShi -= zuanShi;
										data.diamondNum = zuanShi;
										if (!GameManager.ClientMgr.AddUserGold(client, zuanShi, "领取充值红包"))
										{
											LogManager.WriteLog(LogTypes.System, string.Format("领取红包失败#无法给予领取者绑定钻石#rid={0},zuanshi={1}", client.ClientData.RoleID, zuanShi), null, true);
										}
										JieriHongBaoKingActivity.getInstance().OnRecv(client, zuanShi, "领取充值红包");
										data.hongBaoStatus = 1;
									}
									else
									{
										hongBaoData.LeftZuanShi = 0;
										if (zuanShi == 0)
										{
											data.hongBaoStatus = 3;
										}
										else
										{
											if (zuanShi == -200)
											{
												recvData = new HongBaoRecvData();
												recvData.HongBaoID = hongBaoId;
												recvData.RoleId = roleId;
												recvData.ZuanShi = 0;
												recvData.RecvTime = now;
												list.Add(recvData);
											}
											data.hongBaoStatus = zuanShi;
										}
									}
								}
								else
								{
									data.hongBaoStatus = 1;
								}
							}
						}
						else if (type == 102)
						{
							if (GameManager.IsKuaFuServer)
							{
								return false;
							}
							data.message = this.RuntimeData.RedPacketsQuanMinMessage;
							HongBaoSendData hongBaoData2;
							if (!this.RuntimeData.JieRiHongBaoDict.TryGetValue(hongBaoId, out hongBaoData2) || hongBaoData2.leftZuanShi <= 0)
							{
								data.hongBaoStatus = 3;
							}
							else if (now > hongBaoData2.endTime)
							{
								data.hongBaoStatus = 2;
							}
							else
							{
								List<HongBaoRecvData> list = hongBaoData2.RecvList;
								if (null == list)
								{
									list = new List<HongBaoRecvData>();
									hongBaoData2.RecvList = list;
								}
								HongBaoRecvData recvData = list.Find((HongBaoRecvData x) => x.RoleId == roleId);
								if (null == recvData)
								{
									int zuanShi = JieRiHongBaoActivity.getInstance().OpenHongBao(hongBaoData2.senderID);
									if (hongBaoData2.leftZuanShi < zuanShi)
									{
										zuanShi = hongBaoData2.leftZuanShi;
									}
									if (zuanShi > 0)
									{
										recvData = new HongBaoRecvData();
										recvData.HongBaoID = hongBaoId;
										recvData.RoleId = roleId;
										recvData.ZuanShi = zuanShi;
										recvData.RecvTime = now;
										list.Add(recvData);
										hongBaoData2.leftZuanShi -= zuanShi;
										int ret0 = Global.sendToDB<int, HongBaoSendData>(1435, hongBaoData2, client.ServerId);
										if (ret0 < 0)
										{
											hongBaoData2.leftZuanShi += zuanShi;
											LogManager.WriteLog(LogTypes.Error, "领取全民红包失败#更新红包数据失败", null, true);
											data.hongBaoStatus = -15;
										}
										else
										{
											data.diamondNum = zuanShi;
											if (!GameManager.ClientMgr.AddUserGold(client, zuanShi, "领取全民红包"))
											{
												LogManager.WriteLog(LogTypes.System, string.Format("领取红包失败#无法给予领取者绑定钻石#rid={0},zuanshi={1}", client.ClientData.RoleID, zuanShi), null, true);
											}
											JieriHongBaoKingActivity.getInstance().OnRecv(client, zuanShi, "领取全民红包");
											data.hongBaoStatus = 1;
										}
									}
									else
									{
										data.hongBaoStatus = 3;
									}
								}
								else
								{
									data.hongBaoStatus = 1;
								}
							}
						}
						else if (type == 103)
						{
							if (GameManager.IsKuaFuServer)
							{
								return false;
							}
							data.message = this.RuntimeData.RedPacketsQuanMinMessage;
							HongBaoSendData hongBaoData2;
							if (!this.RuntimeData.SpecPHongBaoDict.TryGetValue(hongBaoId, out hongBaoData2) || hongBaoData2.leftZuanShi <= 0)
							{
								data.hongBaoStatus = 3;
							}
							else if (now > hongBaoData2.endTime)
							{
								data.hongBaoStatus = 2;
							}
							else
							{
								List<HongBaoRecvData> list = hongBaoData2.RecvList;
								if (null == list)
								{
									list = new List<HongBaoRecvData>();
									hongBaoData2.RecvList = list;
								}
								SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
								if (list.Find((HongBaoRecvData x) => x.RoleId == roleId) == null && null != spAct)
								{
									int zuanShi = spAct.OpenHongBao(hongBaoData2.senderID);
									if (hongBaoData2.leftZuanShi < zuanShi)
									{
										zuanShi = hongBaoData2.leftZuanShi;
									}
									if (zuanShi > 0)
									{
										HongBaoRecvData recvData = new HongBaoRecvData();
										recvData.HongBaoID = hongBaoId;
										recvData.RoleId = roleId;
										recvData.ZuanShi = zuanShi;
										recvData.RecvTime = now;
										list.Add(recvData);
										hongBaoData2.leftZuanShi -= zuanShi;
										int ret0 = Global.sendToDB<int, HongBaoSendData>(1435, hongBaoData2, client.ServerId);
										if (ret0 < 0)
										{
											hongBaoData2.leftZuanShi += zuanShi;
											LogManager.WriteLog(LogTypes.Error, "领取特权红包失败#更新红包数据失败", null, true);
											data.hongBaoStatus = -15;
										}
										else
										{
											data.diamondNum = zuanShi;
											if (!GameManager.ClientMgr.AddUserGold(client, zuanShi, "领取特权红包"))
											{
												LogManager.WriteLog(LogTypes.System, string.Format("领取特权红包失败#无法给予领取者绑定钻石#rid={0},zuanshi={1}", client.ClientData.RoleID, zuanShi), null, true);
											}
											spAct.OnRecvHongBao(client, hongBaoData2);
											data.hongBaoStatus = 1;
										}
									}
									else
									{
										data.hongBaoStatus = 3;
									}
								}
								else
								{
									data.hongBaoStatus = 1;
								}
							}
						}
						else
						{
							if (GameManager.IsKuaFuServer)
							{
								return false;
							}
							HongBaoSendData sendData;
							if (!this.RuntimeData.HongBaoDict.TryGetValue(hongBaoId, out sendData))
							{
								if (bhid == 0)
								{
									data.hongBaoStatus = 2;
									goto IL_E50;
								}
								if (this.RuntimeData.OldHongBaoDict.TryGetValue(hongBaoId, out sendData))
								{
									if (sendData.hongBaoStatus < 0)
									{
										sendData = Global.sendToDB<HongBaoSendData, int>(1439, hongBaoId, client.ServerId);
										if (null == sendData)
										{
											data.hongBaoStatus = -11000;
											goto IL_E50;
										}
										this.RuntimeData.OldHongBaoDict[hongBaoId] = sendData;
									}
								}
								else
								{
									if (operation > 0)
									{
										data.hongBaoStatus = 3;
										goto IL_E50;
									}
									data.hongBaoStatus = -20;
									goto IL_E50;
								}
							}
							if (data.type != sendData.type)
							{
								data.hongBaoStatus = 2;
							}
							else if (bhid != sendData.bhid)
							{
								data.hongBaoStatus = 2;
							}
							else
							{
								int secs = Global.GetRoleParamsInt32FromDB(client, "EnterBangHuiUnixSecs");
								long enterTicks = TimeUtil.UnixSecondsToTicks(secs) * 10000L;
								if (enterTicks > sendData.sendTime.Ticks)
								{
									data.hongBaoStatus = -1009;
								}
								else
								{
									data.hongBaoID = hongBaoId;
									data.sender = sendData.sender;
									data.leftCount = sendData.leftCount;
									data.message = sendData.message;
									data.sendTime = sendData.sendTime;
									data.sumCount = sendData.sumCount;
									data.sumDiamondNum = sendData.sumDiamondNum;
									data.type = sendData.type;
									bool get = true;
									List<HongBaoRecvData> recvList = sendData.RecvList;
									if (null != recvList)
									{
										HongBaoRecvData recv = recvList.Find((HongBaoRecvData x) => x.RoleId == roleId);
										if (null != recv)
										{
											data.diamondNum = recv.ZuanShi;
											data.hongBaoStatus = 1;
											get = false;
										}
									}
									else
									{
										recvList = new List<HongBaoRecvData>();
										sendData.RecvList = recvList;
									}
									if (get)
									{
										if (sendData.leftCount <= 0)
										{
											data.hongBaoStatus = 3;
											sendData.hongBaoStatus = 2;
											get = false;
										}
										else if (now >= sendData.endTime)
										{
											data.hongBaoStatus = 2;
											sendData.hongBaoStatus = 2;
											get = false;
										}
									}
									if (operation > 0 && get)
									{
										int zuanShi;
										if (sendData.type == 1)
										{
											zuanShi = sendData.leftZuanShi;
											if (sendData.leftCount > 1)
											{
												zuanShi = Global.GetRandomNumber(1, 2 * zuanShi / sendData.leftCount - 1);
											}
										}
										else
										{
											if (sendData.type != 0)
											{
												data.hongBaoStatus = 0;
												goto IL_E50;
											}
											zuanShi = sendData.sumDiamondNum / sendData.sumCount;
										}
										sendData.leftCount--;
										sendData.leftZuanShi -= zuanShi;
										if (sendData.leftZuanShi <= 0)
										{
											sendData.hongBaoStatus = 3;
										}
										int ret0 = Global.sendToDB<int, HongBaoSendData>(1432, sendData, client.ServerId);
										if (ret0 < 0)
										{
											sendData.leftCount++;
											sendData.leftZuanShi += zuanShi;
											LogManager.WriteLog(LogTypes.Error, "领取战盟红包失败#更新红包数据失败", null, true);
											data.hongBaoStatus = -15;
											goto IL_E50;
										}
										HongBaoRecvData recvData = new HongBaoRecvData();
										recvData.HongBaoID = hongBaoId;
										recvData.RoleId = roleId;
										recvData.RoleName = Global.FormatRoleName4(client);
										recvData.ZuanShi = zuanShi;
										recvData.RecvTime = now;
										recvData.BhId = bhid;
										recvList.Add(recvData);
										data.leftCount = sendData.leftCount;
										data.diamondNum = zuanShi;
										data.hongBaoStatus = 1;
										int ret = Global.sendToDB<int, HongBaoRecvData>(1433, recvData, client.ServerId);
										if (ret < 0)
										{
											LogManager.WriteLog(LogTypes.System, string.Format("领取战盟红包失败#红包钻石已扣减但无法记录领取者#rid={0},zuanshi={1},hongbaoid={2}", client.ClientData.RoleID, zuanShi, sendData.hongBaoID), null, true);
										}
										if (!GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, zuanShi, "领取战盟红包", ActivityTypes.None, ""))
										{
											LogManager.WriteLog(LogTypes.System, string.Format("领取战盟红包失败#红包钻石已扣减但无法给予领取者钻石#rid={0},zuanshi={1},hongbaoid={2}", client.ClientData.RoleID, zuanShi, sendData.hongBaoID), null, true);
										}
										ZhanMengHongBaoData zhanMengHongBaoData;
										if (!this.RuntimeData.ZhanMengHongBaoDict.TryGetValue(bhid, out zhanMengHongBaoData))
										{
											zhanMengHongBaoData = new ZhanMengHongBaoData();
											this.RuntimeData.ZhanMengHongBaoDict[bhid] = zhanMengHongBaoData;
										}
										this.AddFaHongBaoRank(client, zhanMengHongBaoData, 0, data.diamondNum);
										client.ClientData.UpdateHongBaoLogTicks[0] = 0L;
										client.ClientData.UpdateHongBaoLogTicks[2] = 0L;
									}
									data.infos = new List<SingleHongBaoRankInfo>();
									foreach (HongBaoRecvData r in recvList)
									{
										data.infos.Add(new SingleHongBaoRankInfo
										{
											roleName = r.RoleName,
											diamondNum = r.ZuanShi,
											recvTime = r.RecvTime
										});
									}
								}
							}
						}
					}
				}
				IL_E50:
				client.sendCmd<HongBaoDeatilsData>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		
		public bool GetJirRiHongBaoBangDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				JieriHongBaoKingActivity.getInstance().QueryActivityInfo(client);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool GetJirRiHongBaoBangAwardsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleId = client.ClientData.RoleID;
				int awardId = Global.SafeConvertToInt32(cmdParams[1]);
				JieriHongBaoKingActivity.getInstance().GetAward(client, awardId);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot5) || true;
		}

		
		public void OnInitGame(GameClient client)
		{
			this.NotifyHongBaoData(client);
		}

		
		public void AddChargeValue(long value)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (value > 0L)
				{
					this.RuntimeData.AddChargeValue += value;
				}
			}
		}

		
		public void UpdateChongZhiHongBaoDataList(KuaFuCmdData cmdData)
		{
			JieRiChongZhiHongBaoActivity activity = JieRiChongZhiHongBaoActivity.getInstance();
			if (activity.InActivityTime())
			{
				Dictionary<int, SystemHongBaoData> list = null;
				long serverNowTicks = 0L;
				if (null != cmdData)
				{
					serverNowTicks = cmdData.Age;
					list = DataHelper.BytesToObject<Dictionary<int, SystemHongBaoData>>(cmdData.Bytes0, 0, cmdData.Bytes0.Length);
				}
				lock (this.RuntimeData.Mutex)
				{
					if (list == null)
					{
						list = new Dictionary<int, SystemHongBaoData>();
					}
					foreach (KeyValuePair<int, SystemHongBaoData> kv in list)
					{
						kv.Value.StartTime += TimeUtil.NOW() - serverNowTicks;
						SystemHongBaoData oldData;
						if (this.RuntimeData.ChongZhiHongBaoDict.TryGetValue(kv.Key, out oldData))
						{
							kv.Value.State = oldData.State;
						}
						this.RuntimeData.ChongZhiHongBaoDict[kv.Key] = kv.Value;
					}
					List<int> removeList = new List<int>();
					foreach (KeyValuePair<int, SystemHongBaoData> kv in this.RuntimeData.ChongZhiHongBaoDict)
					{
						if (!list.ContainsKey(kv.Key))
						{
							removeList.Add(kv.Key);
						}
					}
					foreach (int hongBaoId in removeList)
					{
						this.RuntimeData.ChongZhiHongBaoDict.Remove(hongBaoId);
					}
				}
			}
		}

		
		private void TimerProc(object sender, EventArgs e)
		{
			DateTime now = TimeUtil.NowDateTime();
			long nowTicks = TimeUtil.NOW();
			this.InitHongBaoData();
			if (this.RuntimeData.NextTicks1 < nowTicks)
			{
				this.RuntimeData.NextTicks1 = nowTicks + 1000L;
				lock (this.RuntimeData.Mutex)
				{
					foreach (SystemHongBaoData hongBaoData in this.RuntimeData.ChongZhiHongBaoDict.Values)
					{
						if (hongBaoData.State == 0)
						{
							if (hongBaoData.StartTime <= nowTicks)
							{
								hongBaoData.State = 1;
								GameManager.ClientMgr.BroadcastServerCmd(1426, string.Format("{0}:{1}", 101, hongBaoData.HongBaoId), true);
							}
						}
						else if (hongBaoData.State == 1)
						{
							if (hongBaoData.LeftZuanShi <= 0 || hongBaoData.StartTime + (long)hongBaoData.DurationTime < nowTicks)
							{
								hongBaoData.State = 2;
							}
						}
					}
				}
			}
			if (!GameManager.IsKuaFuServer)
			{
				if (this.RuntimeData.NextTicks3 < nowTicks)
				{
					this.RuntimeData.NextTicks3 = nowTicks + 3000L;
					JieRiHongBaoActivity jieRiHongBaoActivity = JieRiHongBaoActivity.getInstance();
					if (jieRiHongBaoActivity.InActivityTime())
					{
						lock (this.RuntimeData.Mutex)
						{
							List<HongBaoSendData> hongBaoList = jieRiHongBaoActivity.SendHongBaoProc(now, this.RuntimeData.JieRiHongBaoDict);
							if (null != hongBaoList)
							{
								foreach (HongBaoSendData hongbao in hongBaoList)
								{
									this.RuntimeData.JieRiHongBaoDict[hongbao.hongBaoID] = hongbao;
									GameManager.ClientMgr.BroadcastServerCmd(1426, string.Format("{0}:{1}", hongbao.type, hongbao.hongBaoID), true);
								}
							}
						}
					}
					SpecPriorityActivity specPActivity = HuodongCachingMgr.GetSpecPriorityActivity();
					if (specPActivity.InActivityTime())
					{
						lock (this.RuntimeData.Mutex)
						{
							List<HongBaoSendData> hongBaoList = specPActivity.SendHongBaoProc(now, this.RuntimeData.SpecPHongBaoDict);
							if (null != hongBaoList)
							{
								foreach (HongBaoSendData hongbao in hongBaoList)
								{
									this.RuntimeData.SpecPHongBaoDict[hongbao.hongBaoID] = hongbao;
									GameManager.ClientMgr.BroadcastServerCmd(1426, string.Format("{0}:{1}", hongbao.type, hongbao.hongBaoID), true);
								}
							}
						}
					}
					JieRiChongZhiHongBaoActivity activity = JieRiChongZhiHongBaoActivity.getInstance();
					if (activity.InActivityTime())
					{
						long addValue = 0L;
						string keyStr = activity.GetKeyStr();
						lock (this.RuntimeData.Mutex)
						{
							addValue = this.RuntimeData.AddChargeValue;
							this.RuntimeData.AddChargeValue = 0L;
						}
						if (addValue > 0L && !string.IsNullOrEmpty(keyStr))
						{
							if (!JunTuanClient.getInstance().AddChargeValue(keyStr, addValue))
							{
								this.AddChargeValue(addValue);
							}
						}
					}
				}
				if (this.RuntimeData.NextCheckExpireTicks < nowTicks)
				{
					this.RuntimeData.NextCheckExpireTicks = nowTicks + 30000L;
					lock (this.RuntimeData.Mutex)
					{
						List<HongBaoSendData> removeList = new List<HongBaoSendData>();
						foreach (HongBaoSendData hongBaoData2 in this.RuntimeData.JieRiHongBaoDict.Values)
						{
							if (hongBaoData2.leftZuanShi <= 0)
							{
								hongBaoData2.hongBaoStatus = 3;
								removeList.Add(hongBaoData2);
							}
							else if (hongBaoData2.endTime < now)
							{
								hongBaoData2.hongBaoStatus = 2;
								removeList.Add(hongBaoData2);
							}
						}
						foreach (HongBaoSendData hongBaoData2 in removeList)
						{
							int ret0 = Global.sendToDB<int, HongBaoSendData>(1435, hongBaoData2, 0);
							if (ret0 >= 0)
							{
								this.RuntimeData.JieRiHongBaoDict.Remove(hongBaoData2.hongBaoID);
							}
						}
					}
					lock (this.RuntimeData.Mutex)
					{
						List<HongBaoSendData> removeList = new List<HongBaoSendData>();
						foreach (HongBaoSendData hongBaoData2 in this.RuntimeData.SpecPHongBaoDict.Values)
						{
							if (hongBaoData2.leftZuanShi <= 0)
							{
								hongBaoData2.hongBaoStatus = 3;
								removeList.Add(hongBaoData2);
							}
							else if (hongBaoData2.endTime < now)
							{
								hongBaoData2.hongBaoStatus = 2;
								removeList.Add(hongBaoData2);
							}
						}
						foreach (HongBaoSendData hongBaoData2 in removeList)
						{
							int ret0 = Global.sendToDB<int, HongBaoSendData>(1435, hongBaoData2, 0);
							if (ret0 >= 0)
							{
								this.RuntimeData.SpecPHongBaoDict.Remove(hongBaoData2.hongBaoID);
							}
						}
					}
					lock (this.RuntimeData.Mutex)
					{
						List<HongBaoSendData> removeList = new List<HongBaoSendData>();
						foreach (HongBaoSendData hongBaoData2 in this.RuntimeData.HongBaoDict.Values)
						{
							if (hongBaoData2.leftZuanShi <= 0)
							{
								hongBaoData2.hongBaoStatus = 3;
								removeList.Add(hongBaoData2);
							}
							else if (hongBaoData2.endTime < now || this.RuntimeData.DestoryBhIds.Contains(hongBaoData2.bhid))
							{
								hongBaoData2.endTime = now;
								hongBaoData2.hongBaoStatus = 2;
								removeList.Add(hongBaoData2);
								int leftZuanShi = hongBaoData2.leftZuanShi;
								if (leftZuanShi > 0)
								{
									hongBaoData2.leftZuanShi = 0;
									this.AddRoleZuanShiBySendMail(hongBaoData2.senderID, leftZuanShi, GLang.GetLang(2606, new object[0]), string.Format(GLang.GetLang(2607, new object[0]), leftZuanShi), "红包过期");
								}
							}
						}
						foreach (HongBaoSendData hongBaoData2 in removeList)
						{
							int ret0 = Global.sendToDB<int, HongBaoSendData>(1432, hongBaoData2, 0);
							if (ret0 >= 0)
							{
								this.RuntimeData.HongBaoDict.Remove(hongBaoData2.hongBaoID);
								this.RuntimeData.OldHongBaoDict[hongBaoData2.hongBaoID] = hongBaoData2;
							}
						}
						this.RuntimeData.DestoryBhIds.Clear();
					}
				}
			}
		}

		
		public void OnDestoryZhanMeng(int bhid)
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.DestoryBhIds.Add(bhid);
			}
		}

		
		private bool AddRoleZuanShiBySendMail(int roleID, int zuanShi, string subject, string content, string msg)
		{
			string mailGoodsString = "";
			string strDbCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
			{
				-1,
				GLang.GetLang(112, new object[0]),
				roleID,
				"",
				subject,
				content,
				0,
				0,
				zuanShi,
				mailGoodsString
			});
			string[] fieldsData = Global.ExecuteDBCmd(10086, strDbCmd, GameManager.ServerId);
			bool result = fieldsData == null;
			if (!result)
			{
				LogManager.WriteLog(LogTypes.System, string.Format("邮件发钻石#{0}#失败#rid={1},zuanshi={2}", msg, roleID, zuanShi), null, true);
			}
			else
			{
				LogManager.WriteLog(LogTypes.System, string.Format("邮件发钻石#{0}#成功#rid={1},zuanshi={2}", msg, roleID, zuanShi), null, true);
			}
			return result;
		}

		
		private int RankCompareProc(HongBaoRankItemData x, HongBaoRankItemData y)
		{
			int ret = y.daimondNum - x.daimondNum;
			int result;
			if (ret != 0)
			{
				result = ret;
			}
			else
			{
				result = x.rankID - y.rankID;
			}
			return result;
		}

		
		private void AddFaHongBaoRank(GameClient client, ZhanMengHongBaoData data, int type, int addValue)
		{
			List<HongBaoRankItemData> list;
			if (type == 0)
			{
				list = data.RecvRankList;
			}
			else
			{
				list = data.SendRankList;
			}
			int index = list.FindIndex((HongBaoRankItemData x) => x.roleId == client.ClientData.RoleID);
			if (index >= 0)
			{
				HongBaoRankItemData rankData = list[index];
				rankData.daimondNum += addValue;
			}
			else
			{
				list.Add(new HongBaoRankItemData
				{
					roleName = Global.FormatRoleName4(client),
					roleId = client.ClientData.RoleID,
					daimondNum = addValue
				});
			}
			list.Sort(new Comparison<HongBaoRankItemData>(this.RankCompareProc));
			for (int i = 0; i < list.Count; i++)
			{
				list[i].rankID = i + 1;
			}
		}

		
		public void NotifyHongBaoData(GameClient client)
		{
			int bhid = client.ClientData.Faction;
			int roleId = client.ClientData.RoleID;
			int secs = Global.GetRoleParamsInt32FromDB(client, "EnterBangHuiUnixSecs");
			long enterTicks = TimeUtil.UnixSecondsToTicks(secs) * 10000L;
			DateTime now = TimeUtil.NowDateTime();
			long nowTicks = TimeUtil.NOW();
			lock (this.RuntimeData.Mutex)
			{
				if (bhid > 0)
				{
					ZhanMengHongBaoData listData;
					if (this.RuntimeData.ZhanMengHongBaoDict.TryGetValue(bhid, out listData))
					{
						foreach (HongBaoSendData hongbao in listData.HongBaoList)
						{
							if (enterTicks < hongbao.sendTime.Ticks && hongbao.leftCount > 0 && now < hongbao.endTime)
							{
								bool flag2;
								if (hongbao.RecvList != null)
								{
									flag2 = hongbao.RecvList.Exists((HongBaoRecvData x) => x.RoleId == roleId);
								}
								else
								{
									flag2 = false;
								}
								if (!flag2)
								{
									client.SendCmdAfterStartPlayGame(1425, string.Format("{0}:{1}", hongbao.type, hongbao.hongBaoID));
								}
							}
						}
					}
				}
				foreach (HongBaoSendData hongbao in this.RuntimeData.JieRiHongBaoDict.Values)
				{
					if (hongbao.leftZuanShi > 0 && now < hongbao.endTime)
					{
						bool flag3;
						if (hongbao.RecvList != null)
						{
							flag3 = hongbao.RecvList.Exists((HongBaoRecvData x) => x.RoleId == roleId);
						}
						else
						{
							flag3 = false;
						}
						if (!flag3)
						{
							client.SendCmdAfterStartPlayGame(1426, string.Format("{0}:{1}", hongbao.type, hongbao.hongBaoID));
						}
					}
				}
				SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
				foreach (HongBaoSendData hongbao in this.RuntimeData.SpecPHongBaoDict.Values)
				{
					if (hongbao.leftZuanShi > 0 && now < hongbao.endTime)
					{
						bool flag4;
						if (hongbao.RecvList != null)
						{
							flag4 = hongbao.RecvList.Exists((HongBaoRecvData x) => x.RoleId == roleId);
						}
						else
						{
							flag4 = false;
						}
						if (!flag4)
						{
							if (spAct != null && spAct.CanGetHongBao(client, hongbao))
							{
								client.SendCmdAfterStartPlayGame(1426, string.Format("{0}:{1}", hongbao.type, hongbao.hongBaoID));
							}
						}
					}
				}
				foreach (SystemHongBaoData hongbao2 in this.RuntimeData.ChongZhiHongBaoDict.Values)
				{
					if (hongbao2.LeftZuanShi > 0 && nowTicks >= hongbao2.StartTime && nowTicks < hongbao2.StartTime + (long)hongbao2.DurationTime)
					{
						List<HongBaoRecvData> recvList;
						bool flag5;
						if (this.RuntimeData.ChongZhiHongBaoRecvDict.TryGetValue(hongbao2.HongBaoId, out recvList))
						{
							flag5 = recvList.Exists((HongBaoRecvData x) => x.RoleId == roleId);
						}
						else
						{
							flag5 = false;
						}
						if (!flag5)
						{
							client.SendCmdAfterStartPlayGame(1426, string.Format("{0}:{1}", 101, hongbao2.HongBaoId));
						}
					}
				}
			}
		}

		
		private void InitHongBaoData()
		{
			bool zhanMengHongBaoInitialized = false;
			bool jieRiHongBaoInitialized = false;
			bool jieRiHongBaoBangInitialized = false;
			bool specpHongBaoInitialized = false;
			lock (this.RuntimeData.Mutex)
			{
				if (GameManager.IsKuaFuServer)
				{
					this.RuntimeData.ZhanMengHongBaoInitialized = false;
					this.RuntimeData.JieRiHongBaoInitialized = true;
					this.RuntimeData.JieRiHongBaoBangInitialized = true;
					this.RuntimeData.SpecPHongBaoInitialized = true;
					this.RuntimeData.Initialized = true;
					return;
				}
				zhanMengHongBaoInitialized = this.RuntimeData.ZhanMengHongBaoInitialized;
				jieRiHongBaoInitialized = this.RuntimeData.JieRiHongBaoInitialized;
				jieRiHongBaoBangInitialized = this.RuntimeData.JieRiHongBaoBangInitialized;
				specpHongBaoInitialized = this.RuntimeData.SpecPHongBaoInitialized;
			}
			if (!zhanMengHongBaoInitialized)
			{
				HongBaoListQueryData queryData = new HongBaoListQueryData();
				queryData = Global.sendToDB<HongBaoListQueryData, HongBaoListQueryData>(1434, queryData, 0);
				if (queryData != null && queryData.Success > 0)
				{
					lock (this.RuntimeData.Mutex)
					{
						if (null != queryData.List)
						{
							using (List<HongBaoSendData>.Enumerator enumerator = queryData.List.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									HongBaoSendData data = enumerator.Current;
									ZhanMengHongBaoData zhanMengHongBaoData;
									if (!this.RuntimeData.ZhanMengHongBaoDict.TryGetValue(data.bhid, out zhanMengHongBaoData))
									{
										zhanMengHongBaoData = new ZhanMengHongBaoData();
										this.RuntimeData.ZhanMengHongBaoDict[data.bhid] = zhanMengHongBaoData;
									}
									if (!zhanMengHongBaoData.HongBaoList.Exists((HongBaoSendData x) => x.hongBaoID == data.hongBaoID))
									{
										zhanMengHongBaoData.HongBaoList.Add(data);
									}
									if (!this.RuntimeData.HongBaoDict.ContainsKey(data.hongBaoID))
									{
										this.RuntimeData.HongBaoDict[data.hongBaoID] = data;
									}
								}
							}
						}
						this.RuntimeData.ZhanMengHongBaoInitialized = true;
						zhanMengHongBaoInitialized = true;
					}
				}
			}
			if (!jieRiHongBaoInitialized)
			{
				jieRiHongBaoInitialized = true;
				HongBaoListQueryData queryData = JieRiHongBaoActivity.getInstance().QueryHongBaoList();
				if (queryData != null && queryData.Success > 0)
				{
					lock (this.RuntimeData.Mutex)
					{
						if (null != queryData.List)
						{
							foreach (HongBaoSendData data2 in queryData.List)
							{
								if (!this.RuntimeData.JieRiHongBaoDict.ContainsKey(data2.hongBaoID))
								{
									this.RuntimeData.JieRiHongBaoDict[data2.hongBaoID] = data2;
								}
							}
						}
						this.RuntimeData.JieRiHongBaoInitialized = true;
						jieRiHongBaoInitialized = true;
					}
				}
			}
			if (!specpHongBaoInitialized)
			{
				specpHongBaoInitialized = true;
				SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
				if (null != spAct)
				{
					HongBaoListQueryData queryData = spAct.QueryHongBaoList();
					if (queryData != null && queryData.Success > 0)
					{
						lock (this.RuntimeData.Mutex)
						{
							if (null != queryData.List)
							{
								foreach (HongBaoSendData data2 in queryData.List)
								{
									if (!this.RuntimeData.SpecPHongBaoDict.ContainsKey(data2.hongBaoID))
									{
										this.RuntimeData.SpecPHongBaoDict[data2.hongBaoID] = data2;
									}
								}
							}
							this.RuntimeData.SpecPHongBaoInitialized = true;
							specpHongBaoInitialized = true;
						}
					}
				}
			}
			if (!jieRiHongBaoBangInitialized)
			{
				jieRiHongBaoBangInitialized = JieriHongBaoKingActivity.getInstance().LoadRankFromDB();
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.JieRiHongBaoBangInitialized = jieRiHongBaoBangInitialized;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.JieRiHongBaoInitialized = jieRiHongBaoInitialized;
				this.RuntimeData.Initialized = (zhanMengHongBaoInitialized && jieRiHongBaoInitialized && jieRiHongBaoBangInitialized);
			}
		}

		
		private static HongBaoManager instance = new HongBaoManager();

		
		public HongBaoRuntimeData RuntimeData = new HongBaoRuntimeData();
	}
}
