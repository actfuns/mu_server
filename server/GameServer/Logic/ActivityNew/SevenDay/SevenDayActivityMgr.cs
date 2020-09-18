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
	
	public class SevenDayActivityMgr : SingletonTemplate<SevenDayActivityMgr>, IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		
		private SevenDayActivityMgr()
		{
		}

		
		public bool initialize()
		{
			this.LoadConfig();
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1310, 2, 2, SingletonTemplate<SevenDayActivityMgr>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1311, 3, 3, SingletonTemplate<SevenDayActivityMgr>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1312, 3, 3, SingletonTemplate<SevenDayActivityMgr>.Instance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(32, SingletonTemplate<SevenDayActivityMgr>.Instance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(32, SingletonTemplate<SevenDayActivityMgr>.Instance());
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
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

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return true;
		}

		
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

		
		public void LoadConfig()
		{
			this.LoginAct.LoadConfig();
			this.ChargeAct.LoadConfig();
			this.BuyAct.LoadConfig();
			this.GoalAct.LoadConfig();
		}

		
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

		
		public bool IsInActivityTime(GameClient client)
		{
			int currDay;
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8) && this.IsInActivityTime(client, out currDay);
		}

		
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

		
		private SevenDayLoginAct LoginAct = new SevenDayLoginAct();

		
		private SevenDayChargeAct ChargeAct = new SevenDayChargeAct();

		
		private SevenDayBuyAct BuyAct = new SevenDayBuyAct();

		
		private SevenDayGoalAct GoalAct = new SevenDayGoalAct();
	}
}
