using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.ActivityNew.SevenDay
{
	// Token: 0x020001B2 RID: 434
	public class SevenDayActivityMgr : SingletonTemplate<SevenDayActivityMgr>, IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		// Token: 0x06000539 RID: 1337 RVA: 0x000499BF File Offset: 0x00047BBF
		private SevenDayActivityMgr()
		{
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x000499F8 File Offset: 0x00047BF8
		public bool initialize()
		{
			this.LoadConfig();
			return true;
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x00049A14 File Offset: 0x00047C14
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1310, 2, 2, SingletonTemplate<SevenDayActivityMgr>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1311, 3, 3, SingletonTemplate<SevenDayActivityMgr>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1312, 3, 3, SingletonTemplate<SevenDayActivityMgr>.Instance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(32, SingletonTemplate<SevenDayActivityMgr>.Instance());
			return true;
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x00049A84 File Offset: 0x00047C84
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(32, SingletonTemplate<SevenDayActivityMgr>.Instance());
			return true;
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x00049AAC File Offset: 0x00047CAC
		public bool destroy()
		{
			return true;
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x00049AC0 File Offset: 0x00047CC0
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1310:
				result = this.HandleClientQuery(client, nID, bytes, cmdParams);
				break;
			case 1311:
				result = this.HandleGetAward(client, nID, bytes, cmdParams);
				break;
			case 1312:
				result = this.HandleClientBuy(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x00049B1C File Offset: 0x00047D1C
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return true;
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x00049B30 File Offset: 0x00047D30
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 32)
			{
				SevenDayGoalEventObject evObj = eventObject as SevenDayGoalEventObject;
				try
				{
					if (evObj != null && this.IsInActivityTime(evObj.Client))
					{
						this.GoalAct.HandleEvent(evObj);
						if (evObj.Client.ClientSocket.session.SocketTime[4] > 0L)
						{
							this.CheckSendIconState(evObj.Client);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, "SevenDayActivityMgr.processEvent [SevenDayGoal]", ex, true);
				}
				finally
				{
					SevenDayGoalEvPool.Free(evObj);
				}
			}
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x00049BF8 File Offset: 0x00047DF8
		public void LoadConfig()
		{
			this.LoginAct.LoadConfig();
			this.ChargeAct.LoadConfig();
			this.BuyAct.LoadConfig();
			this.GoalAct.LoadConfig();
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x00049C2C File Offset: 0x00047E2C
		public void OnLogin(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
			{
				if (this.IsInActivityTime(client))
				{
					this.LoginAct.Update(client);
					this.ChargeAct.Update(client);
					this.GoalAct.Update(client);
					this.CheckSendIconState(client);
				}
				else if (TimeUtil.NowDateTime() > Global.GetRegTime(client.ClientData).AddDays(14.0))
				{
					lock (client.ClientData.SevenDayActDict)
					{
						if (client.ClientData.SevenDayActDict.Count > 0)
						{
							if (!Global.sendToDB<bool, int>(13221, client.ClientData.RoleID, client.ServerId))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("玩家超过七日活动结束后7天了，删除数据失败,roleid={0}", client.ClientData.RoleID), null, true);
							}
							else
							{
								client.ClientData.SevenDayActDict.Clear();
							}
						}
					}
				}
			}
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x00049D74 File Offset: 0x00047F74
		public void OnNewDay(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
			{
				if (this.IsInActivityTime(client))
				{
					this.LoginAct.Update(client);
					this.ChargeAct.Update(client);
					this.CheckSendIconState(client);
				}
			}
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x00049DC4 File Offset: 0x00047FC4
		public void OnCharge(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
			{
				if (this.IsInActivityTime(client))
				{
					this.ChargeAct.Update(client);
					this.CheckSendIconState(client);
				}
			}
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x00049E08 File Offset: 0x00048008
		private void CheckSendIconState(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
			{
				if (client != null)
				{
					bool bAnyChildActived = false;
					bool bChanged = false;
					bool bTmpFlag = this.LoginAct.HasAnyAwardCanGet(client);
					bAnyChildActived = (bAnyChildActived || bTmpFlag);
					bChanged |= client._IconStateMgr.AddFlushIconState(17001, bTmpFlag);
					bTmpFlag = this.ChargeAct.HasAnyAwardCanGet(client);
					bAnyChildActived = (bAnyChildActived || bTmpFlag);
					bChanged |= client._IconStateMgr.AddFlushIconState(17002, bTmpFlag);
					bTmpFlag = this.BuyAct.HasAnyCanBuy(client);
					bAnyChildActived = (bAnyChildActived || bTmpFlag);
					bChanged |= client._IconStateMgr.AddFlushIconState(17004, bTmpFlag);
					bool[] bGoalDay = null;
					bTmpFlag = this.GoalAct.HasAnyAwardCanGet(client, out bGoalDay);
					bChanged |= client._IconStateMgr.AddFlushIconState(17005, bGoalDay[0]);
					bChanged |= client._IconStateMgr.AddFlushIconState(17006, bGoalDay[1]);
					bChanged |= client._IconStateMgr.AddFlushIconState(17007, bGoalDay[2]);
					bChanged |= client._IconStateMgr.AddFlushIconState(17008, bGoalDay[3]);
					bChanged |= client._IconStateMgr.AddFlushIconState(17009, bGoalDay[4]);
					bChanged |= client._IconStateMgr.AddFlushIconState(17010, bGoalDay[5]);
					bChanged |= client._IconStateMgr.AddFlushIconState(17011, bGoalDay[6]);
					bAnyChildActived = (bAnyChildActived || bTmpFlag);
					bChanged |= client._IconStateMgr.AddFlushIconState(17003, bTmpFlag);
					bChanged |= client._IconStateMgr.AddFlushIconState(17000, bAnyChildActived);
					if (bChanged)
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
			}
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x00049FA4 File Offset: 0x000481A4
		private bool HandleClientQuery(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
			{
				result = false;
			}
			else
			{
				int act = Convert.ToInt32(cmdParams[1]);
				SevenDayActQueryData resultData = new SevenDayActQueryData();
				resultData.ActivityType = act;
				resultData.ItemDict = null;
				TCPOutPacket packet = null;
				Dictionary<int, SevenDayItemData> itemData = this.GetActivityData(client, (ESevenDayActType)act);
				if (itemData == null)
				{
					packet = DataHelper.ObjectToTCPOutPacket<SevenDayActQueryData>(resultData, TCPOutPacketPool.getInstance(), nID);
				}
				else
				{
					lock (itemData)
					{
						resultData.ItemDict = itemData;
						if (act == 2)
						{
							resultData.ItemDict = new Dictionary<int, SevenDayItemData>();
							foreach (KeyValuePair<int, SevenDayItemData> kv in itemData)
							{
								resultData.ItemDict.Add(kv.Key, new SevenDayItemData
								{
									AwardFlag = kv.Value.AwardFlag,
									Params1 = Global.TransMoneyToYuanBao(kv.Value.Params1),
									Params2 = kv.Value.Params2
								});
							}
						}
						packet = DataHelper.ObjectToTCPOutPacket<SevenDayActQueryData>(resultData, TCPOutPacketPool.getInstance(), nID);
					}
				}
				if (packet != null)
				{
					client.sendCmd(packet, true);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x0004A134 File Offset: 0x00048334
		private bool HandleGetAward(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
			{
				result = false;
			}
			else
			{
				int act = Convert.ToInt32(cmdParams[1]);
				int arg = Convert.ToInt32(cmdParams[2]);
				ESevenDayActErrorCode ec = ESevenDayActErrorCode.NotInActivityTime;
				if (!this.IsInActivityTime(client))
				{
					ec = ESevenDayActErrorCode.NotInActivityTime;
				}
				else if (act == 1)
				{
					ec = this.LoginAct.HandleGetAward(client, arg);
				}
				else if (act == 2)
				{
					ec = this.ChargeAct.HandleGetAward(client, arg);
				}
				else if (act == 3)
				{
					ec = this.GoalAct.HandleGetAward(client, arg);
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", (int)ec, act, arg), false);
				if (ec == ESevenDayActErrorCode.Success)
				{
					this.CheckSendIconState(client);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x0004A214 File Offset: 0x00048414
		private bool HandleClientBuy(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
			{
				result = false;
			}
			else
			{
				int id = Convert.ToInt32(cmdParams[1]);
				int cnt = Convert.ToInt32(cmdParams[2]);
				ESevenDayActErrorCode ec;
				if (!this.IsInActivityTime(client))
				{
					ec = ESevenDayActErrorCode.NotInActivityTime;
				}
				else
				{
					ec = this.BuyAct.HandleClientBuy(client, id, cnt);
				}
				if (ec == ESevenDayActErrorCode.Success)
				{
					this.CheckSendIconState(client);
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", (int)ec, id, cnt), false);
				result = true;
			}
			return result;
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x0004A2AC File Offset: 0x000484AC
		public bool IsInActivityTime(GameClient client)
		{
			int currDay;
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8) && this.IsInActivityTime(client, out currDay);
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x0004A2D8 File Offset: 0x000484D8
		public bool IsInActivityTime(GameClient client, out int currDay)
		{
			currDay = 0;
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
			{
				result = false;
			}
			else if (client == null)
			{
				result = false;
			}
			else
			{
				DateTime startDate = Global.GetRegTime(client.ClientData);
				startDate -= startDate.TimeOfDay;
				DateTime todayDate = TimeUtil.NowDateTime();
				todayDate -= todayDate.TimeOfDay;
				currDay = (todayDate - startDate).Days + 1;
				result = (currDay >= 1 && currDay <= 7);
			}
			return result;
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x0004A36C File Offset: 0x0004856C
		public Dictionary<int, SevenDayItemData> GetActivityData(GameClient client, ESevenDayActType actType)
		{
			Dictionary<int, SevenDayItemData> result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
			{
				result = null;
			}
			else if (client == null)
			{
				result = null;
			}
			else
			{
				Dictionary<int, SevenDayItemData> resultDict = null;
				lock (client.ClientData.SevenDayActDict)
				{
					if (!client.ClientData.SevenDayActDict.TryGetValue((int)actType, out resultDict))
					{
						resultDict = new Dictionary<int, SevenDayItemData>();
						client.ClientData.SevenDayActDict[(int)actType] = resultDict;
					}
				}
				result = resultDict;
			}
			return result;
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x0004A414 File Offset: 0x00048614
		public bool UpdateDb(int roleid, ESevenDayActType actType, int id, SevenDayItemData itemData, int serverId)
		{
			bool result;
			if (!Global.sendToDB<bool, SevenDayUpdateDbData>(13220, new SevenDayUpdateDbData
			{
				RoleId = roleid,
				ActivityType = (int)actType,
				Id = id,
				Data = itemData
			}, serverId))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("七日活动更新玩家数据失败, roleid={0}, act={1}, id={2}", roleid, actType, id), null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x0004A488 File Offset: 0x00048688
		public bool GiveAward(GameClient client, AwardItem item, ESevenDayActType type)
		{
			bool result;
			if (client == null || null == item)
			{
				result = false;
			}
			else
			{
				if (item.GoodsDataList != null)
				{
					for (int i = 0; i < item.GoodsDataList.Count; i++)
					{
						int nGoodsID = item.GoodsDataList[i].GoodsID;
						if (Global.IsCanGiveRewardByOccupation(client, nGoodsID))
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsDataList[i].GoodsID, item.GoodsDataList[i].GCount, item.GoodsDataList[i].Quality, "", item.GoodsDataList[i].Forge_level, item.GoodsDataList[i].Binding, 0, "", true, 1, this.GetActivityChineseName(type), "1900-01-01 12:00:00", item.GoodsDataList[i].AddPropIndex, item.GoodsDataList[i].BornIndex, item.GoodsDataList[i].Lucky, item.GoodsDataList[i].Strong, item.GoodsDataList[i].ExcellenceInfo, item.GoodsDataList[i].AppendPropLev, item.GoodsDataList[i].ChangeLifeLevForEquip, null, null, 0, true);
						}
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0004A608 File Offset: 0x00048808
		public string GetActivityChineseName(ESevenDayActType type)
		{
			string name = type.ToString();
			if (type == ESevenDayActType.Login)
			{
				name = "七日登录";
			}
			else if (type == ESevenDayActType.Charge)
			{
				name = "七日充值";
			}
			else if (type == ESevenDayActType.Goal)
			{
				name = "七日目标";
			}
			else if (type == ESevenDayActType.Buy)
			{
				name = "七日抢购";
			}
			return name;
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x0004A674 File Offset: 0x00048874
		public bool GiveEffectiveTimeAward(GameClient client, AwardItem item, ESevenDayActType type)
		{
			bool result;
			if (client == null || null == item)
			{
				result = false;
			}
			else
			{
				if (item.GoodsDataList != null)
				{
					for (int i = 0; i < item.GoodsDataList.Count; i++)
					{
						int nGoodsID = item.GoodsDataList[i].GoodsID;
						if (Global.IsCanGiveRewardByOccupation(client, nGoodsID))
						{
							Global.AddEffectiveTimeGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsDataList[i].GoodsID, item.GoodsDataList[i].GCount, item.GoodsDataList[i].Quality, "", item.GoodsDataList[i].Forge_level, item.GoodsDataList[i].Binding, 0, "", false, 1, this.GetActivityChineseName(type), item.GoodsDataList[i].Starttime, item.GoodsDataList[i].Endtime, item.GoodsDataList[i].AddPropIndex, item.GoodsDataList[i].BornIndex, item.GoodsDataList[i].Lucky, item.GoodsDataList[i].Strong, item.GoodsDataList[i].ExcellenceInfo, item.GoodsDataList[i].AppendPropLev, item.GoodsDataList[i].ChangeLifeLevForEquip, null, null);
						}
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x0004A810 File Offset: 0x00048A10
		public void On_GM(GameClient client, string[] cmdFields)
		{
			if (cmdFields != null && cmdFields.Length >= 2)
			{
				if (cmdFields[1] == "reload")
				{
					SingletonTemplate<SevenDayActivityMgr>.Instance().LoadConfig();
				}
				else if (cmdFields[1] == "get" && client != null)
				{
					if (cmdFields.Length >= 4)
					{
						this.HandleGetAward(client, 1311, null, new string[]
						{
							client.ClientData.RoleID.ToString(),
							cmdFields[2],
							cmdFields[3]
						});
					}
				}
				else if (cmdFields[1] == "buy" && client != null)
				{
					if (cmdFields.Length >= 4)
					{
						this.HandleClientBuy(client, 1312, null, new string[]
						{
							client.ClientData.RoleID.ToString(),
							cmdFields[2],
							cmdFields[3]
						});
					}
				}
			}
		}

		// Token: 0x040009B8 RID: 2488
		private SevenDayLoginAct LoginAct = new SevenDayLoginAct();

		// Token: 0x040009B9 RID: 2489
		private SevenDayChargeAct ChargeAct = new SevenDayChargeAct();

		// Token: 0x040009BA RID: 2490
		private SevenDayBuyAct BuyAct = new SevenDayBuyAct();

		// Token: 0x040009BB RID: 2491
		private SevenDayGoalAct GoalAct = new SevenDayGoalAct();
	}
}
