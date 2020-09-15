using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Reborn;
using GameServer.Logic.UserMoneyCharge;
using KF.Client;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x02000055 RID: 85
	public class SpecPriorityActivity : Activity, IEventListener
	{
		// Token: 0x06000106 RID: 262 RVA: 0x00010A78 File Offset: 0x0000EC78
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 36)
			{
				int ZhiGouTeQuanID = -1;
				int ZhiGouActID = -1;
				ChargeItemBaseEventObject obj = eventObject as ChargeItemBaseEventObject;
				foreach (KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> kvp in obj.Player.ClientData.SpecPriorityActInfoDict)
				{
					SpecPriorityActInfoDB myData = kvp.Value;
					SpecPActivityConfig actProto = this.GetSpecPActivityConfig(kvp.Value.TeQuanID, kvp.Value.ActID);
					if (actProto != null && actProto.ActType == SpecPActivityType.SPAT_ZhiGou && actProto.ZhiGouID == obj.ChargeItemConfig.ChargeItemID)
					{
						ZhiGouTeQuanID = myData.TeQuanID;
						ZhiGouActID = myData.ActID;
						break;
					}
				}
				if (ZhiGouTeQuanID != -1 && ZhiGouActID != -1)
				{
					string cmd = this.BuildFetchSpecActAwardCmd(obj.Player, 0, ZhiGouTeQuanID, ZhiGouActID);
					obj.Player.sendCmd<string>(1497, cmd, false);
				}
				this.ConditionNumTrigger(obj.Player.strUserID, obj.Player.ClientData.RoleID, SpecPConditionType.SPCT_ZhiGou, obj.ChargeItemConfig.ChargeItemID, null);
				this.ConditionNumTrigger(obj.Player.strUserID, obj.Player.ClientData.RoleID, SpecPConditionType.SPCT_ZhiGouKF, obj.ChargeItemConfig.ChargeItemID, null);
			}
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00010BFC File Offset: 0x0000EDFC
		public void Dispose()
		{
			GlobalEventSource.getInstance().removeListener(36, this);
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00010C10 File Offset: 0x0000EE10
		public void OnMoneyChargeEvent(string userid, int roleid, int addMoney)
		{
			int yuanbao = Global.TransMoneyToYuanBao(addMoney);
			this.ConditionNumTrigger(userid, roleid, SpecPConditionType.SPCT_SingleCharge, yuanbao, null);
			this.ConditionNumTrigger(userid, roleid, SpecPConditionType.SPCT_SingleChargeKF, yuanbao, null);
			this.ConditionNumTrigger(userid, roleid, SpecPConditionType.SPCT_Charge, yuanbao, null);
			this.ConditionNumTrigger(userid, roleid, SpecPConditionType.SPCT_ChargeKF, yuanbao, null);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00010C55 File Offset: 0x0000EE55
		public void MoneyConst(GameClient client, int YuanBaoCost)
		{
			this.ConditionNumTrigger(client.strUserID, client.ClientData.RoleID, SpecPConditionType.SPCT_Consume, YuanBaoCost, client);
			this.ConditionNumTrigger(client.strUserID, client.ClientData.RoleID, SpecPConditionType.SPCT_ConsumeKF, YuanBaoCost, client);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00010C90 File Offset: 0x0000EE90
		public bool IsMultiOpen(SpecPActivityBuffType type, out SpecPActivityConfig proto)
		{
			proto = null;
			DateTime nowDateTm = TimeUtil.NowDateTime();
			List<SpecPConditionConfig> specpConditionList = this.CalSpecPConditionListByNow(nowDateTm);
			bool result;
			if (specpConditionList.Count == 0)
			{
				result = false;
			}
			else
			{
				foreach (SpecPConditionConfig condition in specpConditionList)
				{
					foreach (int tequanid in condition.ActiveSpecPList)
					{
						if (this.CanActiveTeQuanID(condition.GroupID, tequanid))
						{
							List<SpecPActivityConfig> actList = this.GetSpecPActivityListByTequanID(tequanid);
							foreach (SpecPActivityConfig act in actList)
							{
								if (act.ActType == SpecPActivityType.SPAT_Buff)
								{
									if (act.Param1 == (int)type)
									{
										proto = act;
										return true;
									}
								}
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00010DD8 File Offset: 0x0000EFD8
		public double GetMult(SpecPActivityBuffType type)
		{
			SpecPActivityConfig actProto;
			double result;
			if (!this.IsMultiOpen(type, out actProto))
			{
				result = 0.0;
			}
			else
			{
				result = actProto.MultiNum;
			}
			return result;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00010E0C File Offset: 0x0000F00C
		public string MackHongBaoActivityKeyStr(DateTime FromDate, DateTime ToDate)
		{
			return string.Format("SP_{0}_{1}", FromDate.ToString("yyyy-MM-dd HH$mm$ss"), ToDate.ToString("yyyy-MM-dd HH$mm$ss"));
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00010E40 File Offset: 0x0000F040
		public HongBaoListQueryData QueryHongBaoList()
		{
			DateTime nowDateTm = TimeUtil.NowDateTime();
			List<SpecPConditionConfig> specpConditionList = this.CalSpecPConditionListByNow(nowDateTm);
			HongBaoListQueryData result;
			if (specpConditionList.Count == 0)
			{
				result = null;
			}
			else
			{
				string ActivityKeyStr = this.MackHongBaoActivityKeyStr(specpConditionList[0].FromDate, specpConditionList[0].ToDate);
				try
				{
					HongBaoListQueryData queryData = new HongBaoListQueryData
					{
						KeyStr = ActivityKeyStr
					};
					return Global.sendToDB<HongBaoListQueryData, HongBaoListQueryData>(1437, queryData, 0);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				result = null;
			}
			return result;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00010EE4 File Offset: 0x0000F0E4
		public List<HongBaoSendData> SendHongBaoProc(DateTime now, Dictionary<int, HongBaoSendData> dict)
		{
			List<HongBaoSendData> result;
			if (GameManager.IsKuaFuServer)
			{
				result = null;
			}
			else
			{
				List<SpecPConditionConfig> specpConditionList = this.CalSpecPConditionListByNow(now);
				if (specpConditionList.Count == 0)
				{
					result = null;
				}
				else
				{
					string ActivityKeyStr = this.MackHongBaoActivityKeyStr(specpConditionList[0].FromDate, specpConditionList[0].ToDate);
					List<HongBaoSendData> list = new List<HongBaoSendData>();
					lock (SpecPriorityActivity.Mutex)
					{
						foreach (SpecPConditionConfig condition in specpConditionList)
						{
							foreach (int tequanid in condition.ActiveSpecPList)
							{
								if (this.CanActiveTeQuanID(condition.GroupID, tequanid))
								{
									List<SpecPActivityConfig> actList = this.GetSpecPActivityListByTequanID(tequanid);
									foreach (SpecPActivityConfig act in actList)
									{
										if (act.ActType == SpecPActivityType.SPAT_HongBao)
										{
											if (!this.HongBaoIdSended.Contains(act.ActID))
											{
												DateTime endTime = specpConditionList[0].ToDate;
												if (!(now >= endTime))
												{
													foreach (HongBaoSendData hongbao in dict.Values)
													{
														if (hongbao.senderID == act.ActID)
														{
															this.HongBaoIdSended.Add(act.ActID);
														}
													}
													if (!this.HongBaoIdSended.Contains(act.ActID))
													{
														HongBaoSendData sendData = new HongBaoSendData();
														sendData.senderID = act.ActID;
														sendData.sender = ActivityKeyStr;
														sendData.sendTime = specpConditionList[0].FromDate;
														sendData.type = 103;
														sendData.endTime = endTime;
														sendData.message = this.RedPacketsTeQuanMessage;
														sendData.sumDiamondNum = act.Param1;
														sendData.leftZuanShi = act.Param1;
														int hongbaoId = Global.sendToDB<int, HongBaoSendData>(1435, sendData, GameManager.ServerId);
														if (hongbaoId > 0)
														{
															sendData.hongBaoID = hongbaoId;
															this.HongBaoIdSended.Add(act.ActID);
															list.Add(sendData);
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
					result = list;
				}
			}
			return result;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00011240 File Offset: 0x0000F440
		public bool CanGetHongBao(GameClient client, HongBaoSendData hongBaoData)
		{
			bool ret = false;
			lock (SpecPriorityActivity.Mutex)
			{
				foreach (KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> kvp in client.ClientData.SpecPriorityActInfoDict)
				{
					SpecPriorityActInfoDB myActInfoDB = kvp.Value;
					SpecPActivityConfig myActConfig = this.GetSpecPActivityConfig(myActInfoDB.TeQuanID, myActInfoDB.ActID);
					if (myActConfig != null && myActConfig.ActType == SpecPActivityType.SPAT_HongBao)
					{
						if (myActInfoDB.ActID == hongBaoData.senderID && myActInfoDB.PurNum == 0)
						{
							ret = true;
							break;
						}
					}
				}
			}
			return ret;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0001133C File Offset: 0x0000F53C
		public bool OnRecvHongBao(GameClient client, HongBaoSendData hongBaoData)
		{
			bool ret = false;
			lock (SpecPriorityActivity.Mutex)
			{
				foreach (KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> kvp in client.ClientData.SpecPriorityActInfoDict)
				{
					SpecPriorityActInfoDB myActInfoDB = kvp.Value;
					SpecPActivityConfig myActConfig = this.GetSpecPActivityConfig(myActInfoDB.TeQuanID, myActInfoDB.ActID);
					if (myActConfig != null && myActConfig.ActType == SpecPActivityType.SPAT_HongBao)
					{
						if (myActInfoDB.ActID == hongBaoData.senderID)
						{
							ret = true;
							myActInfoDB.PurNum = 1;
							this.UpdateClientSpecPriorityActData(client, myActInfoDB);
							break;
						}
					}
				}
			}
			return ret;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00011438 File Offset: 0x0000F638
		public int OpenHongBao(int id)
		{
			DateTime nowDateTm = TimeUtil.NowDateTime();
			List<SpecPConditionConfig> specpConditionList = this.CalSpecPConditionListByNow(nowDateTm);
			int result;
			if (specpConditionList.Count == 0)
			{
				result = 0;
			}
			else
			{
				lock (SpecPriorityActivity.Mutex)
				{
					foreach (SpecPConditionConfig condition in specpConditionList)
					{
						foreach (int tequanid in condition.ActiveSpecPList)
						{
							if (this.CanActiveTeQuanID(condition.GroupID, tequanid))
							{
								List<SpecPActivityConfig> actList = this.GetSpecPActivityListByTequanID(tequanid);
								foreach (SpecPActivityConfig act in actList)
								{
									if (act.ActType == SpecPActivityType.SPAT_HongBao)
									{
										if (id == act.ActID)
										{
											return Global.GetRandomNumber(act.HongBaoRange[0], act.HongBaoRange[1]);
										}
									}
								}
							}
						}
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x000115F0 File Offset: 0x0000F7F0
		public bool IsChouJiangOpen(SpecPActivityChouJiangType type)
		{
			DateTime nowDateTm = TimeUtil.NowDateTime();
			List<SpecPConditionConfig> specpConditionList = this.CalSpecPConditionListByNow(nowDateTm);
			bool result;
			if (specpConditionList.Count == 0)
			{
				result = false;
			}
			else
			{
				foreach (SpecPConditionConfig condition in specpConditionList)
				{
					foreach (int tequanid in condition.ActiveSpecPList)
					{
						if (this.CanActiveTeQuanID(condition.GroupID, tequanid))
						{
							List<SpecPActivityConfig> actList = this.GetSpecPActivityListByTequanID(tequanid);
							foreach (SpecPActivityConfig act in actList)
							{
								if (act.ActType == SpecPActivityType.SPAT_ChouJiang)
								{
									if (act.ChouJiangTypeSet.Contains((int)type))
									{
										return true;
									}
								}
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00011738 File Offset: 0x0000F938
		public bool CheckIconState(GameClient client)
		{
			bool bFlush = false;
			bool result;
			if (client.ClientData.SpecPriorityActInfoDict == null || client.ClientData.SpecPriorityActInfoDict.Count == 0)
			{
				result = bFlush;
			}
			else
			{
				foreach (KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> kvp in client.ClientData.SpecPriorityActInfoDict)
				{
					SpecPriorityActInfoDB myActInfoDB = kvp.Value;
					SpecPActivityConfig myActConfig = this.GetSpecPActivityConfig(myActInfoDB.TeQuanID, myActInfoDB.ActID);
					if (myActConfig != null && myActConfig.ActType == SpecPActivityType.SPAT_Award)
					{
						int ErrCode = this.SpecActCheckCondition(client, myActInfoDB.TeQuanID, myActInfoDB.ActID, 1, false);
						if (ErrCode == 0)
						{
							bFlush = true;
							break;
						}
					}
				}
				result = bFlush;
			}
			return result;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00011830 File Offset: 0x0000FA30
		public void OnRoleLogin(GameClient client, bool isLogin)
		{
			this.GenerateSpecialPriorityActivity(client);
			this.NotifyActivityState(client);
			this.AutoGiveSpecialAward(client, isLogin);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0001184C File Offset: 0x0000FA4C
		public void AutoGiveSpecialAward(GameClient client, bool isLogin = false)
		{
			if (null != client.ClientData.SpecPriorityActInfoDict)
			{
				int dayID = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
				lock (client.ClientData.SpecPriorityActMutex)
				{
					foreach (KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> kvp in client.ClientData.SpecPriorityActInfoDict)
					{
						SpecPriorityActInfoDB myActInfoDB = kvp.Value;
						SpecPActivityConfig myActConfig = this.GetSpecPActivityConfig(myActInfoDB.TeQuanID, myActInfoDB.ActID);
						if (null != myActConfig)
						{
							if (myActConfig.ActType == SpecPActivityType.SPAT_Buff && myActConfig.Param1 == 7)
							{
								int todayID = TimeUtil.NowDateTime().DayOfYear;
								int lastGiveDayID = Global.GetRoleParamsInt32FromDB(client, "GuMuAwardDayID");
								if (myActInfoDB.PurNum != dayID && todayID == lastGiveDayID)
								{
									myActInfoDB.PurNum = dayID;
									int addSeconds = (int)myActConfig.MultiNum * 60;
									Global.AddGuMuMapTime(client, Global.GetAutoGiveGuMuTime(client) + addSeconds, 0);
									this.UpdateClientSpecPriorityActData(client, myActInfoDB);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00011A0C File Offset: 0x0000FC0C
		public int Donate(GameClient client, int groupID, int useMoney)
		{
			DateTime nowDateTm = TimeUtil.NowDateTime();
			List<SpecPConditionConfig> specpConditionList = this.CalSpecPConditionListByNow(nowDateTm);
			int result;
			if (specpConditionList.Count == 0)
			{
				result = -12;
			}
			else
			{
				SpecPConditionConfig conditionConfig = specpConditionList.Find((SpecPConditionConfig x) => x.GroupID == groupID);
				if (conditionConfig == null || (conditionConfig.ConditionType != SpecPConditionType.SPCT_Donate && conditionConfig.ConditionType != SpecPConditionType.SPCT_DonateKF))
				{
					result = -12;
				}
				else
				{
					List<int> specpinfo = this.GetSpecPRoleInfo(client);
					if (conditionConfig.ConditionType == SpecPConditionType.SPCT_Donate)
					{
						if (conditionConfig.EveryDayFinishNum > 0 && specpinfo[1] >= conditionConfig.EveryDayFinishNum)
						{
							return -12;
						}
					}
					else if (conditionConfig.EveryDayFinishNum > 0 && specpinfo[2] >= conditionConfig.EveryDayFinishNum)
					{
						return -12;
					}
					List<GoodsData> goodsDataList = null;
					List<FallGoodsItem> fallGoodsItemList = conditionConfig.fallGoodsItemList as List<FallGoodsItem>;
					if (null != fallGoodsItemList)
					{
						List<FallGoodsItem> tempItemList2 = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(fallGoodsItemList, 1, 1, 1.0);
						if (tempItemList2 != null && tempItemList2.Count > 0)
						{
							goodsDataList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(tempItemList2);
						}
					}
					int BagInt;
					if (!RebornEquip.MoreIsCanIntoRebornOrBaseBagAward(client, goodsDataList, out BagInt))
					{
						if (BagInt == 1)
						{
							result = -101;
						}
						else
						{
							result = -100;
						}
					}
					else
					{
						if (useMoney > 0)
						{
							if (client.ClientData.UserMoney < conditionConfig.ConditionList[2])
							{
								return -10;
							}
							if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, conditionConfig.ConditionList[2], "特权活动Donate", true, true, false, DaiBiSySType.None))
							{
								return -10;
							}
						}
						else
						{
							int value = Global.GetTotalGoodsCountByID(client, conditionConfig.ConditionList[0]);
							if (value < conditionConfig.ConditionList[1])
							{
								return -6;
							}
							bool usedBinding = false;
							bool usedTimeLimited = false;
							GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, conditionConfig.ConditionList[0], conditionConfig.ConditionList[1], false, out usedBinding, out usedTimeLimited, false);
						}
						if (null != goodsDataList)
						{
							for (int i = 0; i < goodsDataList.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsDataList[i].Quality, goodsDataList[i].Props, goodsDataList[i].Forge_level, goodsDataList[i].Binding, 0, "", true, 1, "特权活动捐献", goodsDataList[i].Endtime, goodsDataList[i].AddPropIndex, goodsDataList[i].BornIndex, goodsDataList[i].Lucky, goodsDataList[i].Strong, goodsDataList[i].ExcellenceInfo, goodsDataList[i].AppendPropLev, goodsDataList[i].ChangeLifeLevForEquip, null, null, 0, true);
							}
						}
						if (conditionConfig.ConditionType == SpecPConditionType.SPCT_Donate)
						{
							this.ConditionNumTrigger(client.strUserID, client.ClientData.RoleID, SpecPConditionType.SPCT_Donate, -1, null);
							List<int> list;
							(list = specpinfo)[1] = list[1] + 1;
						}
						else
						{
							this.ConditionNumTrigger(client.strUserID, client.ClientData.RoleID, SpecPConditionType.SPCT_DonateKF, -1, null);
							List<int> list;
							(list = specpinfo)[2] = list[2] + 1;
						}
						this.SaveSpecPRoleInfo(client, specpinfo);
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00011E38 File Offset: 0x00010038
		public SpecPriorityActivityData GetSpecPriorityActivityDataForClient(GameClient client)
		{
			SpecPriorityActivityData myActData = new SpecPriorityActivityData();
			SpecPriorityActivityData result;
			if (null == client.ClientData.SpecPriorityActInfoDict)
			{
				result = myActData;
			}
			else
			{
				lock (SpecPriorityActivity.Mutex)
				{
					myActData.ConditionDict = this.ActConditionInfoDict;
				}
				List<int> specpinfo = this.GetSpecPRoleInfo(client);
				myActData.DonateNum = specpinfo[1];
				myActData.DonateNumKF = specpinfo[2];
				lock (client.ClientData.SpecPriorityActMutex)
				{
					foreach (KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> kvp in client.ClientData.SpecPriorityActInfoDict)
					{
						SpecPriorityActInfoDB mySaveData = kvp.Value;
						SpecPActivityConfig myActConfig = this.GetSpecPActivityConfig(kvp.Value.TeQuanID, kvp.Value.ActID);
						if (null != myActConfig)
						{
							SpecPriorityActInfo ActInfo = new SpecPriorityActInfo();
							ActInfo.TeQuanID = mySaveData.TeQuanID;
							ActInfo.ActID = mySaveData.ActID;
							if (myActConfig.ActType == SpecPActivityType.SPAT_Award)
							{
								List<SpecPConditionConfig> specpConditionList = this.CalSpecPConditionListByNow(TimeUtil.NowDateTime());
								if (specpConditionList.Count != 0)
								{
									SpecPConditionConfig myConfig = specpConditionList[0];
									ActInfo.ShowNum = GameManager.ClientMgr.QueryTotalChongZhiMoneyPeriod(client, myConfig.FromDate, myConfig.ToDate);
								}
							}
							int PurNum = mySaveData.PurNum;
							if (myActConfig.ActType == SpecPActivityType.SPAT_ZhiGou)
							{
								PurNum = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, myActConfig.ZhiGouID);
							}
							if (myActConfig.PurchaseNum == -1)
							{
								ActInfo.State = ((PurNum == 1) ? 1 : 0);
							}
							else
							{
								ActInfo.LeftPurNum = myActConfig.PurchaseNum - PurNum;
								ActInfo.State = ((ActInfo.LeftPurNum <= 0) ? 1 : 0);
								if (ActInfo.LeftPurNum < 0)
								{
									ActInfo.LeftPurNum = 0;
								}
							}
							if (myActConfig.ActType == SpecPActivityType.SPAT_Award)
							{
								if (ActInfo.ShowNum < myActConfig.Param1)
								{
									ActInfo.State = -1;
								}
							}
							myActData.SpecActInfoList.Add(ActInfo);
						}
					}
				}
				result = myActData;
			}
			return result;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0001212C File Offset: 0x0001032C
		public bool CanActiveTeQuanID(int groupID, int tequanID)
		{
			int conditionNum = 0;
			SpecPActiveConfig activeConfig = null;
			lock (SpecPriorityActivity.Mutex)
			{
				if (!this.SpecPActiveDict.TryGetValue(tequanID, out activeConfig))
				{
					return false;
				}
				if (!this.ActConditionInfoDict.TryGetValue(groupID, out conditionNum))
				{
					return false;
				}
			}
			return conditionNum >= activeConfig.ConditonNum;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x000121FC File Offset: 0x000103FC
		public void GenerateSpecialPriorityActivity(GameClient client)
		{
			if (!client.ClientSocket.IsKuaFuLogin)
			{
				DateTime nowDateTm = TimeUtil.NowDateTime();
				List<SpecPConditionConfig> specpConditionList = this.CalSpecPConditionListByNow(nowDateTm);
				lock (client.ClientData.SpecPriorityActMutex)
				{
					if (null == client.ClientData.SpecPriorityActInfoDict)
					{
						client.ClientData.SpecPriorityActInfoDict = new Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB>();
					}
					Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB> SpecPActInfoForUpdate = new Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB>(client.ClientData.SpecPriorityActInfoDict);
					using (Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB>.Enumerator enumerator = client.ClientData.SpecPriorityActInfoDict.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> kvp = enumerator.Current;
							bool flag2;
							if (!specpConditionList.Exists(delegate(SpecPConditionConfig x)
							{
								IEnumerable<int> activeSpecPList2 = x.ActiveSpecPList;
								KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> kvp2 = kvp;
								return activeSpecPList2.Contains(kvp2.Value.TeQuanID);
							}))
							{
								KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> kvp3 = kvp;
								flag2 = (kvp3.Value.TeQuanID > 0);
							}
							else
							{
								flag2 = false;
							}
							if (flag2)
							{
								KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> kvp3 = kvp;
								this.DeleteClientSpecPriorityActData(client, kvp3.Value.TeQuanID);
								Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB> dictionary = SpecPActInfoForUpdate;
								kvp3 = kvp;
								dictionary.Remove(kvp3.Key);
							}
						}
					}
					foreach (SpecPConditionConfig condition in specpConditionList)
					{
						foreach (int tequanid in condition.ActiveSpecPList)
						{
							if (this.CanActiveTeQuanID(condition.GroupID, tequanid))
							{
								List<SpecPActivityConfig> actList = this.GetSpecPActivityListByTequanID(tequanid);
								foreach (SpecPActivityConfig act in actList)
								{
									if (act.ActType != SpecPActivityType.SPAT_Boss)
									{
										KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(act.TeQuanID, act.ActID);
										SpecPriorityActInfoDB SpecActData = null;
										if (!SpecPActInfoForUpdate.TryGetValue(kvpKey, out SpecActData))
										{
											SpecActData = new SpecPriorityActInfoDB
											{
												TeQuanID = act.TeQuanID,
												ActID = act.ActID
											};
											SpecPActInfoForUpdate[kvpKey] = SpecActData;
											this.UpdateClientSpecPriorityActData(client, SpecActData);
										}
									}
								}
							}
						}
					}
					for (int actIDLoop = -2; actIDLoop < 0; actIDLoop++)
					{
						KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(0, actIDLoop);
						SpecPriorityActInfoDB SpecActData = null;
						if (!SpecPActInfoForUpdate.TryGetValue(kvpKey, out SpecActData))
						{
							SpecActData = new SpecPriorityActInfoDB
							{
								TeQuanID = 0,
								ActID = actIDLoop
							};
							SpecPActInfoForUpdate[kvpKey] = SpecActData;
							this.UpdateClientSpecPriorityActData(client, SpecActData);
						}
					}
					client.ClientData.SpecPriorityActInfoDict = SpecPActInfoForUpdate;
				}
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0001258C File Offset: 0x0001078C
		public void NotifyActivityState(GameClient client)
		{
			DateTime now = TimeUtil.NowDateTime();
			bool bNotifyOpen = false;
			List<SpecPConditionConfig> specpConditionList = this.CalSpecPConditionListByNow(now);
			if (specpConditionList.Count != 0)
			{
				bNotifyOpen = true;
			}
			if (client.ClientSocket.IsKuaFuLogin)
			{
				bNotifyOpen = false;
			}
			if (bNotifyOpen)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					17,
					1,
					"",
					0,
					0
				});
				client.sendCmd(770, strcmd, false);
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					17,
					0,
					"",
					0,
					0
				});
				client.sendCmd(770, strcmd, false);
			}
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00012690 File Offset: 0x00010890
		public int SpecActCheckCondition(GameClient client, int TeQuanID, int ActID, int PurNum, bool CheckCost = true)
		{
			DateTime now = TimeUtil.NowDateTime();
			List<SpecPConditionConfig> specpConditionList = this.CalSpecPConditionListByNow(now);
			int result;
			if (specpConditionList.Count == 0)
			{
				result = -2001;
			}
			else
			{
				KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(TeQuanID, ActID);
				SpecPriorityActInfoDB mySaveData = null;
				if (!client.ClientData.SpecPriorityActInfoDict.TryGetValue(kvpKey, out mySaveData))
				{
					result = -2;
				}
				else
				{
					SpecPActivityConfig myActConfig = this.GetSpecPActivityConfig(TeQuanID, ActID);
					if (null == myActConfig)
					{
						result = -2;
					}
					else if (myActConfig.ActType == SpecPActivityType.SPAT_ZhiGou || myActConfig.ActType == SpecPActivityType.SPAT_Boss)
					{
						result = -12;
					}
					else if (PurNum <= 0)
					{
						result = -12;
					}
					else
					{
						if (myActConfig.PurchaseNum == -1)
						{
							if (mySaveData.PurNum == 1)
							{
								return -200;
							}
						}
						else if (myActConfig.PurchaseNum - mySaveData.PurNum < PurNum)
						{
							return -200;
						}
						if (CheckCost && myActConfig.ActType == SpecPActivityType.SPAT_Mall)
						{
							if (client.ClientData.UserMoney < myActConfig.Param1 * PurNum)
							{
								return -10;
							}
						}
						if (myActConfig.ActType == SpecPActivityType.SPAT_Award)
						{
							SpecPConditionConfig myConfig = specpConditionList[0];
							int inputYuanBao = GameManager.ClientMgr.QueryTotalChongZhiMoneyPeriod(client, myConfig.FromDate, myConfig.ToDate);
							if (inputYuanBao < myActConfig.Param1)
							{
								return -10;
							}
						}
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00012854 File Offset: 0x00010A54
		public bool HasEnoughBagSpaceForAwardGoods(GameClient client, int TeQuanID, int ActID, int PurNum)
		{
			KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(TeQuanID, ActID);
			SpecPriorityActInfoDB mySaveData = null;
			bool result;
			if (!client.ClientData.SpecPriorityActInfoDict.TryGetValue(kvpKey, out mySaveData))
			{
				result = false;
			}
			else
			{
				SpecPActivityConfig myActConfig = this.GetSpecPActivityConfig(TeQuanID, ActID);
				if (null == myActConfig)
				{
					result = false;
				}
				else
				{
					int nOccu = Global.CalcOriginalOccupationID(client);
					List<GoodsData> lData = new List<GoodsData>();
					foreach (GoodsData item in myActConfig.GoodsDataListOne)
					{
						GoodsData cloneItem = new GoodsData(item);
						cloneItem.GCount *= PurNum;
						lData.Add(cloneItem);
					}
					int count = myActConfig.GoodsDataListTwo.Count;
					for (int i = 0; i < count; i++)
					{
						GoodsData data = myActConfig.GoodsDataListTwo[i];
						if (Global.IsRoleOccupationMatchGoods(nOccu, data.GoodsID))
						{
							GoodsData cloneItem = new GoodsData(data);
							cloneItem.GCount *= PurNum;
							lData.Add(cloneItem);
						}
					}
					AwardItem tmpAwardItem = myActConfig.GoodsDataListThr.ToAwardItem();
					foreach (GoodsData item in tmpAwardItem.GoodsDataList)
					{
						GoodsData cloneItem = new GoodsData(item);
						cloneItem.Starttime = item.Starttime;
						cloneItem.Endtime = item.Endtime;
						cloneItem.GCount *= PurNum;
						lData.Add(cloneItem);
					}
					result = Global.CanAddGoodsDataList(client, lData);
				}
			}
			return result;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00012A3C File Offset: 0x00010C3C
		public int SpecActGiveAward(GameClient client, int TeQuanID, int ActID, int PurNum)
		{
			KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(TeQuanID, ActID);
			SpecPriorityActInfoDB mySaveData = null;
			int result;
			if (!client.ClientData.SpecPriorityActInfoDict.TryGetValue(kvpKey, out mySaveData))
			{
				result = -2;
			}
			else
			{
				SpecPActivityConfig myActConfig = this.GetSpecPActivityConfig(TeQuanID, ActID);
				if (null == myActConfig)
				{
					result = -2;
				}
				else
				{
					string castResList = "";
					if (myActConfig.ActType == SpecPActivityType.SPAT_Mall && myActConfig.Param1 > 0)
					{
						int subMoney = myActConfig.Param1 * PurNum;
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, subMoney, "特权活动Mall", true, true, false, DaiBiSySType.None))
						{
							return -10;
						}
						castResList = EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
						{
							-subMoney,
							client.ClientData.UserMoney + subMoney,
							client.ClientData.UserMoney
						});
					}
					AwardItem myAwardItem = new AwardItem();
					myAwardItem.GoodsDataList.Clear();
					foreach (GoodsData item in myActConfig.GoodsDataListOne)
					{
						GoodsData cloneItem = new GoodsData(item);
						cloneItem.GCount *= PurNum;
						myAwardItem.GoodsDataList.Add(cloneItem);
					}
					base.GiveAward(client, myAwardItem);
					myAwardItem.GoodsDataList.Clear();
					foreach (GoodsData item in myActConfig.GoodsDataListTwo)
					{
						GoodsData cloneItem = new GoodsData(item);
						cloneItem.GCount *= PurNum;
						myAwardItem.GoodsDataList.Add(cloneItem);
					}
					base.GiveAward(client, myAwardItem);
					myAwardItem.GoodsDataList.Clear();
					foreach (GoodsData item in myActConfig.GoodsDataListThr.ToAwardItem().GoodsDataList)
					{
						GoodsData cloneItem = new GoodsData(item);
						item.GCount *= PurNum;
						myAwardItem.GoodsDataList.Add(cloneItem);
					}
					base.GiveEffectiveTimeAward(client, myAwardItem);
					if (myActConfig.PurchaseNum == -1)
					{
						mySaveData.PurNum = 1;
					}
					else
					{
						mySaveData.PurNum += PurNum;
					}
					this.UpdateClientSpecPriorityActData(client, mySaveData);
					if (client._IconStateMgr.CheckSpecPriorityActivity(client))
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
					string strResList = EventLogManager.MakeGoodsDataPropString(myAwardItem.GoodsDataList);
					EventLogManager.AddPurchaseEvent(client, 9, mySaveData.ActID, castResList, strResList);
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00012DA4 File Offset: 0x00010FA4
		public string BuildFetchSpecActAwardCmd(GameClient client, int ErrCode, int tequanID, int actID)
		{
			int roleID = client.ClientData.RoleID;
			KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(tequanID, actID);
			SpecPriorityActInfoDB mySaveData = null;
			string result;
			if (!client.ClientData.SpecPriorityActInfoDict.TryGetValue(kvpKey, out mySaveData))
			{
				result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
				{
					-2,
					roleID,
					tequanID,
					actID,
					0,
					0
				});
			}
			else
			{
				List<SpecPActivityConfig> myActConfigList = null;
				if (!this.SpecPActivityDict.TryGetValue(tequanID, out myActConfigList))
				{
					result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-2,
						roleID,
						tequanID,
						actID,
						0,
						0
					});
				}
				else
				{
					SpecPActivityConfig myActConfig = myActConfigList.Find((SpecPActivityConfig x) => x.ActID == actID);
					if (null == myActConfig)
					{
						result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
						{
							-2,
							roleID,
							tequanID,
							actID,
							0,
							0
						});
					}
					else
					{
						if (myActConfig.ActType == SpecPActivityType.SPAT_ZhiGou)
						{
							mySaveData.PurNum = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, myActConfig.ZhiGouID);
						}
						int LeftPurNum = myActConfig.PurchaseNum - mySaveData.PurNum;
						int ShowNum = 0;
						result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
						{
							ErrCode,
							roleID,
							tequanID,
							actID,
							LeftPurNum,
							ShowNum
						});
					}
				}
			}
			return result;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00012FEC File Offset: 0x000111EC
		public void ModifySpecialPriorityActConitionInfo(int key, int add)
		{
			lock (SpecPriorityActivity.Mutex)
			{
				int curValue = 0;
				if (this.ActConditionInfoDict.TryGetValue(key, out curValue))
				{
					curValue += add;
				}
				else
				{
					curValue = add;
				}
				this.ActConditionInfoDict[key] = curValue;
				this.SaveSpecialPriorityActConitionInfo();
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x000130AC File Offset: 0x000112AC
		public List<SpecPConditionConfig> CalSpecPConditionListByNow(DateTime now)
		{
			return this.SpecPConditionList.FindAll((SpecPConditionConfig x) => x.FromDate <= now && now <= x.ToDate);
		}

		// Token: 0x06000121 RID: 289 RVA: 0x000130E4 File Offset: 0x000112E4
		public void OnConditionNumChangeBefore()
		{
			lock (SpecPriorityActivity.Mutex)
			{
				this.ActConditionInfoDictOld = new Dictionary<int, int>(this.ActConditionInfoDict);
			}
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0001316C File Offset: 0x0001136C
		public void OnConditionNumChangeAfter()
		{
			bool tryActive = false;
			lock (SpecPriorityActivity.Mutex)
			{
				using (Dictionary<int, int>.Enumerator enumerator = this.ActConditionInfoDict.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, int> kvp = enumerator.Current;
						int conditionNumOld = 0;
						Dictionary<int, int> actConditionInfoDictOld = this.ActConditionInfoDictOld;
						KeyValuePair<int, int> kvp3 = kvp;
						actConditionInfoDictOld.TryGetValue(kvp3.Key, out conditionNumOld);
						kvp3 = kvp;
						if (kvp3.Value != conditionNumOld)
						{
							SpecPConditionConfig conditionConfig = this.SpecPConditionList.Find(delegate(SpecPConditionConfig x)
							{
								int groupID = x.GroupID;
								KeyValuePair<int, int> kvp2 = kvp;
								return groupID == kvp2.Key;
							});
							if (null != conditionConfig)
							{
								foreach (int tequanid in conditionConfig.ActiveSpecPList)
								{
									SpecPActiveConfig activeConfig = null;
									if (this.SpecPActiveDict.TryGetValue(tequanid, out activeConfig))
									{
										bool flag2;
										if (conditionNumOld < activeConfig.ConditonNum)
										{
											int conditonNum = activeConfig.ConditonNum;
											kvp3 = kvp;
											flag2 = (conditonNum > kvp3.Value);
										}
										else
										{
											flag2 = true;
										}
										if (!flag2)
										{
											if (activeConfig.ActType == SpecPActivityType.SPAT_Boss)
											{
												this.TryRefreshBoss(activeConfig);
											}
											else
											{
												tryActive = true;
											}
										}
									}
								}
							}
						}
					}
				}
				this.ActConditionInfoDictOld = this.ActConditionInfoDict;
			}
			if (tryActive)
			{
				GameManager.ClientMgr.ReGenerateSpecPriorityActGroup();
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00013360 File Offset: 0x00011560
		public bool CheckValidChargeItem(int zhigouID)
		{
			bool result;
			if (!this.ActZhiGouIDSet.Contains(zhigouID))
			{
				result = true;
			}
			else
			{
				DateTime nowDateTm = TimeUtil.NowDateTime();
				List<SpecPConditionConfig> specpConditionList = this.CalSpecPConditionListByNow(nowDateTm);
				foreach (SpecPConditionConfig condition in specpConditionList)
				{
					foreach (int tequanid in condition.ActiveSpecPList)
					{
						if (this.CanActiveTeQuanID(condition.GroupID, tequanid))
						{
							List<SpecPActivityConfig> actList = this.GetSpecPActivityListByTequanID(tequanid);
							foreach (SpecPActivityConfig act in actList)
							{
								if (act.ActType == SpecPActivityType.SPAT_ZhiGou)
								{
									if (zhigouID == act.ZhiGouID)
									{
										return true;
									}
								}
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x000134D0 File Offset: 0x000116D0
		private SpecPActivityConfig GetSpecPActivityConfig(int tequanID, int actID)
		{
			SpecPActivityConfig myActConfig = null;
			List<SpecPActivityConfig> myActConfigList = null;
			SpecPActivityConfig result;
			if (!this.SpecPActivityDict.TryGetValue(tequanID, out myActConfigList))
			{
				result = myActConfig;
			}
			else
			{
				myActConfig = myActConfigList.Find((SpecPActivityConfig x) => x.ActID == actID);
				result = myActConfig;
			}
			return result;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00013520 File Offset: 0x00011720
		private List<SpecPActivityConfig> GetSpecPActivityListByTequanID(int tequanID)
		{
			List<SpecPActivityConfig> actList = null;
			lock (SpecPriorityActivity.Mutex)
			{
				this.SpecPActivityDict.TryGetValue(tequanID, out actList);
			}
			if (null == actList)
			{
				actList = new List<SpecPActivityConfig>();
				LogManager.WriteLog(LogTypes.Error, string.Format("特权活动找不到对应的活动数据, TeQuanID={0}", tequanID), null, true);
			}
			return actList;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x000135AC File Offset: 0x000117AC
		private void TryRefreshBoss(SpecPActiveConfig activeConfig)
		{
			List<SpecPActivityConfig> myActConfigList = null;
			if (this.SpecPActivityDict.TryGetValue(activeConfig.TeQuanID, out myActConfigList))
			{
				foreach (SpecPActivityConfig item in myActConfigList)
				{
					if (item.ActType == SpecPActivityType.SPAT_Boss)
					{
						int MapCodeID = item.Param1;
						int ExtensionID = item.Param2;
						List<MonsterZone> monsterZoneList = GameManager.MonsterZoneMgr.GetMonsterZoneByMapCodeAndMonsterID(MapCodeID, ExtensionID);
						if (monsterZoneList != null && monsterZoneList.Count > 0)
						{
							MonsterZone zoneNode = monsterZoneList[Global.GetRandomNumber(0, monsterZoneList.Count)];
							SceneUIClasses sceneType = Global.GetMapSceneType(MapCodeID);
							GameManager.MonsterZoneMgr.AddDynamicMonsters(MapCodeID, ExtensionID, -1, 1, zoneNode.ToX, zoneNode.ToY, 0, zoneNode.PursuitRadius, sceneType, null, null);
						}
					}
				}
			}
		}

		// Token: 0x06000127 RID: 295 RVA: 0x000136B4 File Offset: 0x000118B4
		private bool ConditionNumCountAlreadyUser(string userid, int roleid, SpecPConditionType type, SpecPConditionConfig condition)
		{
			string beginStr = condition.FromDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
			string endStr = condition.ToDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
			string keyStr = string.Format("{0}_{1}_{2}", beginStr, endStr, (int)type);
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				roleid,
				keyStr,
				this.ActivityType,
				"0"
			});
			string[] dbResult = Global.ExecuteDBCmd(10221, strcmd, 0);
			bool result;
			if (dbResult == null || dbResult.Length == 0)
			{
				result = true;
			}
			else
			{
				int hasGetTimes = Global.SafeConvertToInt32(dbResult[3]);
				result = (hasGetTimes > 0);
			}
			return result;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00013798 File Offset: 0x00011998
		private void CoditionNumCountUser(string userid, int roleid, SpecPConditionType type, SpecPConditionConfig condition)
		{
			string beginStr = condition.FromDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
			string endStr = condition.ToDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
			string keyStr = string.Format("{0}_{1}_{2}", beginStr, endStr, (int)type);
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				roleid,
				keyStr,
				this.ActivityType,
				"1",
				TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH$mm$ss")
			});
			Global.ExecuteDBCmd(10222, strcmd, 0);
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00013850 File Offset: 0x00011A50
		public List<int> GetSpecPRoleInfo(GameClient client)
		{
			List<int> countList = Global.GetRoleParamsIntListFromDB(client, "153");
			if (countList.Count != 3)
			{
				for (int i = countList.Count; i < 3; i++)
				{
					countList.Add(0);
				}
			}
			int dayid = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
			if (countList[0] != dayid)
			{
				countList[0] = dayid;
				countList[1] = 0;
				countList[2] = 0;
			}
			return countList;
		}

		// Token: 0x0600012A RID: 298 RVA: 0x000138D7 File Offset: 0x00011AD7
		public void SaveSpecPRoleInfo(GameClient client, List<int> countList)
		{
			Global.SaveRoleParamsIntListToDB(client, countList, "153", true);
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00013910 File Offset: 0x00011B10
		private void ConditionNumTrigger(string userid, int roleid, SpecPConditionType type, int param, GameClient client = null)
		{
			DateTime nowDateTm = TimeUtil.NowDateTime();
			List<SpecPConditionConfig> specpConditionList = this.CalSpecPConditionListByNow(nowDateTm);
			SpecPConditionConfig condition = specpConditionList.Find((SpecPConditionConfig x) => x.ConditionType == type);
			if (null != condition)
			{
				lock (SpecPriorityActivity.Mutex)
				{
					this.OnConditionNumChangeBefore();
					switch (type)
					{
					case SpecPConditionType.SPCT_SingleCharge:
						if (param >= condition.ConditionList[0] && !this.ConditionNumCountAlreadyUser(userid, roleid, type, condition))
						{
							this.ModifySpecialPriorityActConitionInfo(condition.GroupID, 1);
							this.CoditionNumCountUser(userid, roleid, type, condition);
						}
						break;
					case SpecPConditionType.SPCT_SingleChargeKF:
						if (param >= condition.ConditionList[0] && !this.ConditionNumCountAlreadyUser(userid, roleid, type, condition))
						{
							KFCopyRpcClient.getInstance().SpecPriority_ModifyActivityConditionNum(condition.GroupID, 1);
							this.ModifySpecialPriorityActConitionInfo(condition.GroupID, 1);
							this.CoditionNumCountUser(userid, roleid, type, condition);
						}
						break;
					case SpecPConditionType.SPCT_ZhiGou:
						if (condition.ConditionList.Contains(param))
						{
							this.ModifySpecialPriorityActConitionInfo(condition.GroupID, 1);
						}
						break;
					case SpecPConditionType.SPCT_ZhiGouKF:
						if (condition.ConditionList.Contains(param))
						{
							KFCopyRpcClient.getInstance().SpecPriority_ModifyActivityConditionNum(condition.GroupID, 1);
							this.ModifySpecialPriorityActConitionInfo(condition.GroupID, 1);
						}
						break;
					case SpecPConditionType.SPCT_Donate:
						this.ModifySpecialPriorityActConitionInfo(condition.GroupID, 1);
						break;
					case SpecPConditionType.SPCT_DonateKF:
						KFCopyRpcClient.getInstance().SpecPriority_ModifyActivityConditionNum(condition.GroupID, 1);
						this.ModifySpecialPriorityActConitionInfo(condition.GroupID, 1);
						break;
					case SpecPConditionType.SPCT_Charge:
					{
						int inputYuanBao = GameManager.ClientMgr.QueryTotalChongZhiMoneyPeriod(roleid, condition.FromDate, condition.ToDate);
						if (inputYuanBao >= condition.ConditionList[0] && !this.ConditionNumCountAlreadyUser(userid, roleid, type, condition))
						{
							this.ModifySpecialPriorityActConitionInfo(condition.GroupID, 1);
							this.CoditionNumCountUser(userid, roleid, type, condition);
						}
						break;
					}
					case SpecPConditionType.SPCT_ChargeKF:
					{
						int inputYuanBao = GameManager.ClientMgr.QueryTotalChongZhiMoneyPeriod(roleid, condition.FromDate, condition.ToDate);
						if (inputYuanBao >= condition.ConditionList[0] && !this.ConditionNumCountAlreadyUser(userid, roleid, type, condition))
						{
							KFCopyRpcClient.getInstance().SpecPriority_ModifyActivityConditionNum(condition.GroupID, 1);
							this.ModifySpecialPriorityActConitionInfo(condition.GroupID, 1);
							this.CoditionNumCountUser(userid, roleid, type, condition);
						}
						break;
					}
					case SpecPConditionType.SPCT_Consume:
					{
						KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(0, -2);
						SpecPriorityActInfoDB specActData = null;
						if (client != null && client.ClientData.SpecPriorityActInfoDict.TryGetValue(kvpKey, out specActData))
						{
							if (specActData.PurNum != condition.GroupID)
							{
								specActData.PurNum = condition.GroupID;
								specActData.CountNum = 0;
							}
							int OldCountNum = specActData.CountNum;
							specActData.CountNum += param;
							if (OldCountNum < condition.ConditionList[0] && condition.ConditionList[0] <= specActData.CountNum)
							{
								this.ModifySpecialPriorityActConitionInfo(condition.GroupID, 1);
							}
							this.UpdateClientSpecPriorityActData(client, specActData);
						}
						break;
					}
					case SpecPConditionType.SPCT_ConsumeKF:
					{
						KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(0, -1);
						SpecPriorityActInfoDB specActData = null;
						if (client != null && client.ClientData.SpecPriorityActInfoDict.TryGetValue(kvpKey, out specActData))
						{
							if (specActData.PurNum != condition.GroupID)
							{
								specActData.PurNum = condition.GroupID;
								specActData.CountNum = 0;
							}
							int OldCountNum = specActData.CountNum;
							specActData.CountNum += param;
							if (OldCountNum < condition.ConditionList[0] && condition.ConditionList[0] <= specActData.CountNum)
							{
								KFCopyRpcClient.getInstance().SpecPriority_ModifyActivityConditionNum(condition.GroupID, 1);
								this.ModifySpecialPriorityActConitionInfo(condition.GroupID, 1);
							}
							this.UpdateClientSpecPriorityActData(client, specActData);
						}
						break;
					}
					}
					this.OnConditionNumChangeAfter();
				}
			}
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00013DC8 File Offset: 0x00011FC8
		private void DeleteClientSpecPriorityActData(GameClient client, int TeQuanID = 0)
		{
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, TeQuanID);
			Global.ExecuteDBCmd(13176, strcmd, client.ServerId);
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00013E0C File Offset: 0x0001200C
		private void UpdateClientSpecPriorityActData(GameClient client, SpecPriorityActInfoDB SpecPActData)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				SpecPActData.TeQuanID,
				SpecPActData.ActID,
				SpecPActData.PurNum,
				SpecPActData.CountNum
			});
			Global.ExecuteDBCmd(13175, strcmd, client.ServerId);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00013EBC File Offset: 0x000120BC
		private void InitPriorityActConditionInfo(DateTime now, bool launch = false)
		{
			lock (SpecPriorityActivity.Mutex)
			{
				List<SpecPConditionConfig> specpList = this.CalSpecPConditionListByNow(now);
				List<int> removeList = new List<int>();
				using (Dictionary<int, int>.Enumerator enumerator = this.ActConditionInfoDict.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, int> kvp = enumerator.Current;
						if (!specpList.Exists(delegate(SpecPConditionConfig x)
						{
							int groupID = x.GroupID;
							KeyValuePair<int, int> kvp2 = kvp;
							return groupID == kvp2.Key;
						}))
						{
							List<int> list = removeList;
							KeyValuePair<int, int> kvp3 = kvp;
							list.Add(kvp3.Key);
						}
					}
				}
				foreach (int key in removeList)
				{
					int key;
					this.ActConditionInfoDict.Remove(key);
				}
				foreach (SpecPConditionConfig item in specpList)
				{
					if (!this.ActConditionInfoDict.ContainsKey(item.GroupID))
					{
						this.ActConditionInfoDict[item.GroupID] = 0;
					}
				}
				if (launch)
				{
					string sSpecialPriorityAct = GameManager.GameConfigMgr.GetGameConfigItemStr("specpact", "");
					if (!string.IsNullOrEmpty(sSpecialPriorityAct))
					{
						string[] actFields = sSpecialPriorityAct.Split(new char[]
						{
							'|'
						});
						foreach (string item2 in actFields)
						{
							string[] groupFields = item2.Split(new char[]
							{
								','
							});
							if (groupFields.Length == 2)
							{
								int key = Global.SafeConvertToInt32(groupFields[0]);
								int value = Global.SafeConvertToInt32(groupFields[1]);
								if (this.ActConditionInfoDict.ContainsKey(key))
								{
									this.ActConditionInfoDict[key] = value;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00014168 File Offset: 0x00012368
		private void SaveSpecialPriorityActConitionInfo()
		{
			lock (SpecPriorityActivity.Mutex)
			{
				string strResult = "";
				foreach (KeyValuePair<int, int> item in this.ActConditionInfoDict)
				{
					strResult += string.Format("{0},{1}|", item.Key, item.Value);
				}
				if (!string.IsNullOrEmpty(strResult) && strResult.Substring(strResult.Length - 1) == "|")
				{
					strResult = strResult.Substring(0, strResult.Length - 1);
				}
				GameManager.GameConfigMgr.SetGameConfigItem("specpact", strResult);
				Global.UpdateDBGameConfigg("specpact", strResult);
			}
		}

		// Token: 0x06000130 RID: 304 RVA: 0x000142B0 File Offset: 0x000124B0
		public void TimerProc()
		{
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				lock (SpecPriorityActivity.Mutex)
				{
					SpecPrioritySyncData SyncData = KFCopyRpcClient.getInstance().SpecPriority_GetActivityConditionInfo();
					if (null != SyncData)
					{
						int CurrentDayID = TimeUtil.GetOffsetDay(now);
						if (CurrentDayID != this.LastUpdateDayID)
						{
							this.LastUpdateDayID = CurrentDayID;
							this.InitPriorityActConditionInfo(now, false);
						}
						this.OnConditionNumChangeBefore();
						bool saveCondition = false;
						using (Dictionary<int, int>.Enumerator enumerator = SyncData.ActConditionInfoDict.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								KeyValuePair<int, int> item = enumerator.Current;
								SpecPConditionConfig conditionConfig = this.SpecPConditionList.Find(delegate(SpecPConditionConfig x)
								{
									int groupID = x.GroupID;
									KeyValuePair<int, int> item2 = item;
									return groupID == item2.Key;
								});
								if (null != conditionConfig)
								{
									if (this.IfKFConditonType(conditionConfig.ConditionType))
									{
										int conditionNumOld = 0;
										Dictionary<int, int> actConditionInfoDict = this.ActConditionInfoDict;
										KeyValuePair<int, int> item3 = item;
										actConditionInfoDict.TryGetValue(item3.Key, out conditionNumOld);
										int num = conditionNumOld;
										item3 = item;
										if (num != item3.Value)
										{
											saveCondition = true;
											Dictionary<int, int> actConditionInfoDict2 = this.ActConditionInfoDict;
											item3 = item;
											int key = item3.Key;
											item3 = item;
											actConditionInfoDict2[key] = item3.Value;
										}
									}
								}
							}
						}
						if (saveCondition)
						{
							this.SaveSpecialPriorityActConitionInfo();
						}
						this.OnConditionNumChangeAfter();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
		}

		// Token: 0x06000131 RID: 305 RVA: 0x000144C8 File Offset: 0x000126C8
		public bool IfKFConditonType(SpecPConditionType ConditionType)
		{
			return SpecPConditionType.SPCT_SingleChargeKF == ConditionType || SpecPConditionType.SPCT_ZhiGouKF == ConditionType || SpecPConditionType.SPCT_DonateKF == ConditionType || SpecPConditionType.SPCT_ChargeKF == ConditionType || SpecPConditionType.SPCT_ConsumeKF == ConditionType;
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00014500 File Offset: 0x00012700
		public bool Init()
		{
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				if (!this.LoadSpecPriorityConditionData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityActiveData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityBossData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityAwardData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityZhiGouData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityMallData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityBuffData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityHongBaoData())
				{
					return false;
				}
				if (!this.LoadSpecPriorityChouJiangData())
				{
					return false;
				}
				this.RedPacketsTeQuanMessage = GameManager.systemParamsList.GetParamValueByName("RedPacketsTeQuanMessage");
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				this.ActivityType = 49;
				base.PredealDateTime();
				GlobalEventSource.getInstance().registerListener(36, this);
				this.InitPriorityActConditionInfo(now, true);
				this.LastUpdateDayID = TimeUtil.GetOffsetDay(now);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0001466C File Offset: 0x0001286C
		public bool LoadSpecPriorityConditionData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanTiaoJian.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanTiaoJian.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						SpecPConditionConfig myData = new SpecPConditionConfig();
						myData.GroupID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						string FromDate = Global.GetSafeAttributeStr(xmlItem, "KaiQiShiJian");
						if (!string.IsNullOrEmpty(FromDate))
						{
							myData.FromDate = DateTime.Parse(FromDate);
						}
						else
						{
							myData.FromDate = DateTime.Parse("2008-08-08 08:08:08");
						}
						string ToDate = Global.GetSafeAttributeStr(xmlItem, "JieShuShiJian");
						if (!string.IsNullOrEmpty(ToDate))
						{
							myData.ToDate = DateTime.Parse(ToDate);
						}
						else
						{
							myData.ToDate = DateTime.Parse("2028-08-08 08:08:08");
						}
						myData.ConditionType = (SpecPConditionType)Global.GetSafeAttributeLong(xmlItem, "TiaoJianLeiXing");
						if (myData.ConditionType == SpecPConditionType.SPCT_Donate || myData.ConditionType == SpecPConditionType.SPCT_DonateKF)
						{
							string[] strArray = Global.GetSafeAttributeStr(xmlItem, "GuDingLeiXing").Split(new char[]
							{
								'|'
							});
							string[] GoodsLimit = strArray[0].Split(new char[]
							{
								','
							});
							myData.ConditionList = new int[]
							{
								Global.SafeConvertToInt32(GoodsLimit[0]),
								Global.SafeConvertToInt32(GoodsLimit[1]),
								Global.SafeConvertToInt32(strArray[1])
							};
						}
						else
						{
							myData.ConditionList = Global.GetSafeAttributeIntArray(xmlItem, "GuDingLeiXing", -1, ',');
						}
						myData.ActiveSpecPList = Global.GetSafeAttributeIntArray(xmlItem, "JiHuoID", -1, '|');
						int basePercent = 0;
						FallGoodsItem fallGoodsItem = null;
						string AwardGoodsList = Global.GetDefAttributeStr(xmlItem, "JiangLiFanKui", "");
						if (!string.IsNullOrEmpty(AwardGoodsList))
						{
							List<FallGoodsItem> fallGoodsItemList = new List<FallGoodsItem>();
							string[] goodsFields = AwardGoodsList.Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < goodsFields.Length; i++)
							{
								string item = goodsFields[i].Trim();
								if (!(item == ""))
								{
									string[] itemFields = item.Split(new char[]
									{
										','
									});
									if (itemFields.Length == 7)
									{
										fallGoodsItem = null;
										try
										{
											fallGoodsItem = new FallGoodsItem
											{
												GoodsID = Convert.ToInt32(itemFields[0]),
												BasePercent = basePercent,
												SelfPercent = (int)(Convert.ToDouble(itemFields[1]) * 100000.0),
												Binding = Convert.ToInt32(itemFields[2]),
												LuckyRate = (int)Convert.ToDouble(itemFields[3]),
												FallLevelID = Convert.ToInt32(itemFields[4]),
												ZhuiJiaID = Convert.ToInt32(itemFields[5]),
												ExcellencePropertyID = Convert.ToInt32(itemFields[6])
											};
											basePercent += fallGoodsItem.SelfPercent;
										}
										catch (Exception)
										{
											fallGoodsItem = null;
										}
										if (null == fallGoodsItem)
										{
											LogManager.WriteLog(LogTypes.Error, string.Format("解析特权活动JiangLiFanKui项时发生错误, ID={0}, GoodsID={1}", myData.GroupID, item), null, true);
										}
										else
										{
											fallGoodsItemList.Add(fallGoodsItem);
										}
									}
								}
							}
							myData.fallGoodsItemList = fallGoodsItemList;
						}
						myData.EveryDayFinishNum = (int)Global.GetSafeAttributeLong(xmlItem, "MeiRiShangXian");
						this.SpecPConditionList.Add(myData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/TeQuanTiaoJian.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00014AB8 File Offset: 0x00012CB8
		public bool LoadSpecPriorityActiveData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanJiHuo.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanJiHuo.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						SpecPActiveConfig myData = new SpecPActiveConfig();
						myData.TeQuanID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myData.ConditonNum = (int)Global.GetSafeAttributeLong(xmlItem, "CanShu");
						myData.ActType = (SpecPActivityType)Global.GetSafeAttributeLong(xmlItem, "tips");
						this.SpecPActiveDict[myData.TeQuanID] = myData;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/TeQuanJiHuo.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00014BEC File Offset: 0x00012DEC
		public bool LoadSpecPriorityBossData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanBoss.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanBoss.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						SpecPActivityConfig myData = new SpecPActivityConfig();
						myData.TeQuanID = (int)Global.GetSafeAttributeLong(xmlItem, "TeQuanID");
						myData.ActID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myData.Param1 = (int)Global.GetSafeAttributeLong(xmlItem, "DiTuID");
						myData.Param2 = (int)Global.GetSafeAttributeLong(xmlItem, "GuaiWuID");
						List<SpecPActivityConfig> actList = null;
						if (!this.SpecPActivityDict.TryGetValue(myData.TeQuanID, out actList))
						{
							actList = new List<SpecPActivityConfig>();
							this.SpecPActivityDict[myData.TeQuanID] = actList;
						}
						myData.ActType = SpecPActivityType.SPAT_Boss;
						actList.Add(myData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/TeQuanBoss.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00014D88 File Offset: 0x00012F88
		public bool LoadSpecPriorityAwardData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanJiangLi.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanJiangLi.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						SpecPActivityConfig myData = new SpecPActivityConfig();
						myData.TeQuanID = (int)Global.GetSafeAttributeLong(xmlItem, "TeQuanID");
						myData.ActID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myData.Param1 = (int)Global.GetSafeAttributeLong(xmlItem, "LingQuTiaoJian");
						myData.PurchaseNum = 1;
						string goodsIDsOne = Global.GetSafeAttributeStr(xmlItem, "WuPinID");
						string[] fields = goodsIDsOne.Split(new char[]
						{
							'|'
						});
						if (fields.Length <= 0)
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型特权活动Award配置文件中的物品配置项失败", new object[0]), null, true);
						}
						else
						{
							myData.GoodsDataListOne = HuodongCachingMgr.ParseGoodsDataList(fields, "特权活动Award配置");
						}
						List<SpecPActivityConfig> actList = null;
						if (!this.SpecPActivityDict.TryGetValue(myData.TeQuanID, out actList))
						{
							actList = new List<SpecPActivityConfig>();
							this.SpecPActivityDict[myData.TeQuanID] = actList;
						}
						myData.ActType = SpecPActivityType.SPAT_Award;
						actList.Add(myData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/TeQuanJiangLi.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00014F7C File Offset: 0x0001317C
		public bool LoadSpecPriorityZhiGouData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanZhiGou.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanZhiGou.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						SpecPActivityConfig myData = new SpecPActivityConfig();
						myData.TeQuanID = (int)Global.GetSafeAttributeLong(xmlItem, "TeQuanID");
						myData.ActID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myData.PurchaseNum = (int)Global.GetSafeAttributeLong(xmlItem, "GouMaiCiShu");
						string Price = Global.GetSafeAttributeStr(xmlItem, "ZhiGouJiaGe");
						string[] PriceFiled = Price.Split(new char[]
						{
							'|'
						});
						if (PriceFiled.Length == 3)
						{
							myData.Param1 = Global.SafeConvertToInt32(PriceFiled[0]);
							myData.Param2 = Global.SafeConvertToInt32(PriceFiled[1]);
							myData.ZhiGouID = Global.SafeConvertToInt32(PriceFiled[2]);
						}
						this.ActZhiGouIDSet.Add(myData.ZhiGouID);
						List<SpecPActivityConfig> actList = null;
						if (!this.SpecPActivityDict.TryGetValue(myData.TeQuanID, out actList))
						{
							actList = new List<SpecPActivityConfig>();
							this.SpecPActivityDict[myData.TeQuanID] = actList;
						}
						myData.ActType = SpecPActivityType.SPAT_ZhiGou;
						string goodsIDsOne = Global.GetSafeAttributeStr(xmlItem, "DaoJuJiangLi");
						string goodsIDsTwo = Global.GetSafeAttributeStr(xmlItem, "FenZhiYeJiangLi");
						UserMoneyMgr.getInstance().CheckChargeItemConfigLogic(myData.ZhiGouID, myData.PurchaseNum, goodsIDsOne, goodsIDsTwo, string.Format("特权活动直购 ID={0}", myData.ActID));
						actList.Add(myData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/TeQuanZhiGou.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x000151C8 File Offset: 0x000133C8
		public bool LoadSpecPriorityMallData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanShangCheng.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanShangCheng.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						SpecPActivityConfig myData = new SpecPActivityConfig();
						myData.TeQuanID = (int)Global.GetSafeAttributeLong(xmlItem, "TeQuanID");
						myData.ActID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myData.Param1 = (int)Global.GetSafeAttributeLong(xmlItem, "JiaGe");
						myData.PurchaseNum = (int)Global.GetSafeAttributeLong(xmlItem, "GouMaiCiShu");
						string goodsIDsOne = Global.GetSafeAttributeStr(xmlItem, "WuPinID");
						string[] fields = goodsIDsOne.Split(new char[]
						{
							'|'
						});
						if (fields.Length <= 0)
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型特权活动Mall配置文件中的物品配置项失败", new object[0]), null, true);
						}
						else
						{
							myData.GoodsDataListOne = HuodongCachingMgr.ParseGoodsDataList(fields, "特权活动Mall配置");
						}
						List<SpecPActivityConfig> actList = null;
						if (!this.SpecPActivityDict.TryGetValue(myData.TeQuanID, out actList))
						{
							actList = new List<SpecPActivityConfig>();
							this.SpecPActivityDict[myData.TeQuanID] = actList;
						}
						myData.ActType = SpecPActivityType.SPAT_Mall;
						actList.Add(myData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/TeQuanShangCheng.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x000153C8 File Offset: 0x000135C8
		public bool LoadSpecPriorityBuffData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanBuff.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanBuff.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						SpecPActivityConfig myData = new SpecPActivityConfig();
						myData.TeQuanID = (int)Global.GetSafeAttributeLong(xmlItem, "TeQuanID");
						myData.ActID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myData.Param1 = (int)Global.GetSafeAttributeLong(xmlItem, "HuoDongLeiXing");
						myData.MultiNum = Global.GetSafeAttributeDouble(xmlItem, "KaiQiBeiShu");
						List<SpecPActivityConfig> actList = null;
						if (!this.SpecPActivityDict.TryGetValue(myData.TeQuanID, out actList))
						{
							actList = new List<SpecPActivityConfig>();
							this.SpecPActivityDict[myData.TeQuanID] = actList;
						}
						myData.ActType = SpecPActivityType.SPAT_Buff;
						actList.Add(myData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/TeQuanBuff.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00015564 File Offset: 0x00013764
		public bool LoadSpecPriorityHongBaoData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanHongBao.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanHongBao.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						SpecPActivityConfig myData = new SpecPActivityConfig();
						myData.TeQuanID = (int)Global.GetSafeAttributeLong(xmlItem, "TeQuanID");
						myData.ActID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myData.Param1 = (int)Global.GetSafeAttributeLong(xmlItem, "BangZuanShuLiang");
						myData.HongBaoRange = Global.GetSafeAttributeIntArray(xmlItem, "HongBaoQuYu", -1, ',');
						List<SpecPActivityConfig> actList = null;
						if (!this.SpecPActivityDict.TryGetValue(myData.TeQuanID, out actList))
						{
							actList = new List<SpecPActivityConfig>();
							this.SpecPActivityDict[myData.TeQuanID] = actList;
						}
						myData.ActType = SpecPActivityType.SPAT_HongBao;
						actList.Add(myData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/TeQuanHongBao.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00015704 File Offset: 0x00013904
		public bool LoadSpecPriorityChouJiangData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/TeQuanChouJiang.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/TeQuanChouJiang.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						SpecPActivityConfig myData = new SpecPActivityConfig();
						myData.TeQuanID = (int)Global.GetSafeAttributeLong(xmlItem, "TeQuanID");
						myData.ActID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						string ChouJiangType = Global.GetSafeAttributeStr(xmlItem, "ChouJiangLeiXing");
						if (!string.IsNullOrEmpty(ChouJiangType))
						{
							string[] StrArrayParam = ChouJiangType.Split(new char[]
							{
								','
							});
							foreach (string item in StrArrayParam)
							{
								if (string.Compare(item, "TeQuanQiFu", true) == 0)
								{
									myData.ChouJiangTypeSet.Add(1);
								}
								if (string.Compare(item, "TeQuanShouLie", true) == 0)
								{
									myData.ChouJiangTypeSet.Add(2);
								}
								if (string.Compare(item, "TeQuanBuHuo", true) == 0)
								{
									myData.ChouJiangTypeSet.Add(3);
								}
							}
						}
						List<SpecPActivityConfig> actList = null;
						if (!this.SpecPActivityDict.TryGetValue(myData.TeQuanID, out actList))
						{
							actList = new List<SpecPActivityConfig>();
							this.SpecPActivityDict[myData.TeQuanID] = actList;
						}
						myData.ActType = SpecPActivityType.SPAT_ChouJiang;
						actList.Add(myData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/TeQuanChouJiang.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x040001CC RID: 460
		public const string SpecPCondition_fileName = "Config/TeQuanTiaoJian.xml";

		// Token: 0x040001CD RID: 461
		public const string SpecPActive_fileName = "Config/TeQuanJiHuo.xml";

		// Token: 0x040001CE RID: 462
		public const string SpecPBoss_fileName = "Config/TeQuanBoss.xml";

		// Token: 0x040001CF RID: 463
		public const string SpecPAward_fileName = "Config/TeQuanJiangLi.xml";

		// Token: 0x040001D0 RID: 464
		public const string SpecPZhiGou_fileName = "Config/TeQuanZhiGou.xml";

		// Token: 0x040001D1 RID: 465
		public const string SpecPMall_fileName = "Config/TeQuanShangCheng.xml";

		// Token: 0x040001D2 RID: 466
		public const string SpecPBuff_fileName = "Config/TeQuanBuff.xml";

		// Token: 0x040001D3 RID: 467
		public const string SpecPHongBao_fileName = "Config/TeQuanHongBao.xml";

		// Token: 0x040001D4 RID: 468
		public const string SpecPChouJiang_fileName = "Config/TeQuanChouJiang.xml";

		// Token: 0x040001D5 RID: 469
		public static object Mutex = new object();

		// Token: 0x040001D6 RID: 470
		public List<SpecPConditionConfig> SpecPConditionList = new List<SpecPConditionConfig>();

		// Token: 0x040001D7 RID: 471
		public Dictionary<int, SpecPActiveConfig> SpecPActiveDict = new Dictionary<int, SpecPActiveConfig>();

		// Token: 0x040001D8 RID: 472
		public Dictionary<int, List<SpecPActivityConfig>> SpecPActivityDict = new Dictionary<int, List<SpecPActivityConfig>>();

		// Token: 0x040001D9 RID: 473
		public Dictionary<int, int> ActConditionInfoDict = new Dictionary<int, int>();

		// Token: 0x040001DA RID: 474
		public Dictionary<int, int> ActConditionInfoDictOld = new Dictionary<int, int>();

		// Token: 0x040001DB RID: 475
		public HashSet<int> ActZhiGouIDSet = new HashSet<int>();

		// Token: 0x040001DC RID: 476
		private HashSet<int> HongBaoIdSended = new HashSet<int>();

		// Token: 0x040001DD RID: 477
		private string RedPacketsTeQuanMessage;

		// Token: 0x040001DE RID: 478
		private int LastUpdateDayID;
	}
}
