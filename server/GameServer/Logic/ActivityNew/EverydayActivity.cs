using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.UserMoneyCharge;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	
	public class EverydayActivity : Activity, IEventListener
	{
		
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 36)
			{
				int ZhiGouActID = -1;
				ChargeItemBaseEventObject obj = eventObject as ChargeItemBaseEventObject;
				foreach (KeyValuePair<int, EverydayActInfoDB> kvp in obj.Player.ClientData.EverydayActInfoDict)
				{
					EverydayActInfoDB myData = kvp.Value;
					EverydayActivityConfig actProto = null;
					if (this.ActivityConfigDict.TryGetValue(myData.ActID, out actProto))
					{
						if (actProto.Type == 14 && actProto.Price.ZhiGouID == obj.ChargeItemConfig.ChargeItemID)
						{
							ZhiGouActID = myData.ActID;
							break;
						}
					}
				}
				if (ZhiGouActID != -1)
				{
					string cmd = this.BuildFetchEverydayActAwardCmd(obj.Player, 0, ZhiGouActID);
					obj.Player.sendCmd<string>(1507, cmd, false);
				}
			}
		}

		
		public void OnMoneyChargeEvent(string userid, int roleid, int addMoney)
		{
			string strYuanbaoToJiFen = GameManager.systemParamsList.GetParamValueByName("EveryDayChongZhiDuiHuan");
			if (!string.IsNullOrEmpty(strYuanbaoToJiFen))
			{
				string[] strFieldsMtoJiFen = strYuanbaoToJiFen.Split(new char[]
				{
					':'
				});
				if (strFieldsMtoJiFen.Length == 2)
				{
					int DivJiFen = Convert.ToInt32(strFieldsMtoJiFen[0]);
					if (DivJiFen != 0)
					{
						double YuanbaoToJiFenDiv = Convert.ToDouble(strFieldsMtoJiFen[1]) / (double)DivJiFen;
						int JiFenAdd = (int)(YuanbaoToJiFenDiv * (double)Global.TransMoneyToYuanBao(addMoney));
						DateTime nowDateTime = TimeUtil.NowDateTime();
						DateTime FromActDate = new DateTime(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day, 0, 0, 0);
						DateTime ToActDate = new DateTime(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day, 23, 59, 59);
						string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							roleid,
							JiFenAdd,
							FromActDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'),
							ToActDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$')
						});
						Global.ExecuteDBCmd(13173, strcmd, 0);
					}
				}
			}
		}

		
		public bool CheckValidChargeItem(int zhigouID)
		{
			List<EverydayActGroupInfoDB> ActGroupInfoList = null;
			lock (this.Mutex)
			{
				ActGroupInfoList = this.GetCurrentActGroupInfo();
			}
			this.CleanUpActGroupInfo(ActGroupInfoList, 1);
			foreach (EverydayActGroupInfoDB item in ActGroupInfoList)
			{
				EverydayActivityGroupConfig myInfo = null;
				if (this.ActivityGroupConfigDict.TryGetValue(item.GroupID, out myInfo))
				{
					foreach (int actId in myInfo.ActivityIDList)
					{
						EverydayActivityConfig myActConfig = null;
						if (this.ActivityConfigDict.TryGetValue(actId, out myActConfig))
						{
							if (myActConfig.Price.ZhiGouID == zhigouID)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		
		public void MoneyConst(GameClient client, int moneyCost)
		{
			if (client.ClientData.EverydayActInfoDict != null && client.ClientData.EverydayActInfoDict.Count != 0)
			{
				foreach (KeyValuePair<int, EverydayActInfoDB> kvp in client.ClientData.EverydayActInfoDict)
				{
					EverydayActInfoDB myData = kvp.Value;
					EverydayActivityConfig actProto = null;
					if (this.ActivityConfigDict.TryGetValue(myData.ActID, out actProto))
					{
						if (actProto.Type == 3)
						{
							myData.CountNum += moneyCost;
							this.UpdateClientEverydayActData(client, myData);
						}
					}
				}
				if (client._IconStateMgr.CheckEverydayActivity(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		
		public bool CheckIconState(GameClient client)
		{
			bool bFlush = false;
			bool result;
			if (client.ClientData.EverydayActInfoDict == null || client.ClientData.EverydayActInfoDict.Count == 0)
			{
				result = bFlush;
			}
			else
			{
				foreach (KeyValuePair<int, EverydayActInfoDB> kvp in client.ClientData.EverydayActInfoDict)
				{
					EverydayActInfoDB myActInfoDB = kvp.Value;
					int ErrCode = this.EverydayActCheckCondition(client, kvp.Key, false);
					if (ErrCode == 0)
					{
						bFlush = true;
						break;
					}
				}
				result = bFlush;
			}
			return result;
		}

		
		public void OnRoleLogin(GameClient client)
		{
			this.GenerateEverydayActivity(client);
			this.NotifyActivityState(client);
		}

		
		public EverydayActivityData GetEverydayActivityDataForClient(GameClient client)
		{
			EverydayActivityData myActData = new EverydayActivityData();
			myActData.EveryActInfoList = new List<EverydayActInfo>();
			EverydayActivityData result;
			if (null == client.ClientData.EverydayActInfoDict)
			{
				result = myActData;
			}
			else
			{
				foreach (KeyValuePair<int, EverydayActInfoDB> kvp in client.ClientData.EverydayActInfoDict)
				{
					EverydayActInfoDB mySaveData = kvp.Value;
					EverydayActivityConfig myActConfig = null;
					if (this.ActivityConfigDict.TryGetValue(mySaveData.ActID, out myActConfig))
					{
						EverydayActInfo ActInfo = new EverydayActInfo();
						ActInfo.ActID = mySaveData.ActID;
						EveryActGoalData CurGoalNum = this.GetCurrentGoalNum(client, mySaveData, myActConfig);
						ActInfo.ShowNum = CurGoalNum.NumOne;
						ActInfo.ShowNum2 = CurGoalNum.NumTwo;
						int PurNum = mySaveData.PurNum;
						if (myActConfig.Type == 14)
						{
							PurNum = UserMoneyMgr.getInstance().GetChargeItemDayPurchaseNum(client, myActConfig.Price.ZhiGouID);
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
						if (myActConfig.GoalData.IsValid())
						{
							if (CurGoalNum.NumOne < myActConfig.GoalData.NumOne || (CurGoalNum.NumOne == myActConfig.GoalData.NumOne && CurGoalNum.NumTwo < myActConfig.GoalData.NumTwo))
							{
								ActInfo.State = -1;
							}
						}
						myActData.EveryActInfoList.Add(ActInfo);
					}
				}
				result = myActData;
			}
			return result;
		}

		
		public int EverydayActCheckCondition(GameClient client, int ActID, bool CheckCost = true)
		{
			EverydayActInfoDB mySaveData = null;
			int result;
			if (!client.ClientData.EverydayActInfoDict.TryGetValue(ActID, out mySaveData))
			{
				result = -2;
			}
			else
			{
				EverydayActivityConfig myActConfig = null;
				if (!this.ActivityConfigDict.TryGetValue(mySaveData.ActID, out myActConfig))
				{
					result = -2;
				}
				else if (myActConfig.Type == 14)
				{
					result = -12;
				}
				else
				{
					EveryActGoalData CurGoalNum = this.GetCurrentGoalNum(client, mySaveData, myActConfig);
					if (myActConfig.GoalData.IsValid())
					{
						if (CurGoalNum.NumOne < myActConfig.GoalData.NumOne || (CurGoalNum.NumOne == myActConfig.GoalData.NumOne && CurGoalNum.NumTwo < myActConfig.GoalData.NumTwo))
						{
							return -12;
						}
					}
					if (myActConfig.PurchaseNum == -1)
					{
						if (mySaveData.PurNum == 1)
						{
							return -200;
						}
					}
					else if (myActConfig.PurchaseNum - mySaveData.PurNum <= 0)
					{
						return -200;
					}
					if (CheckCost && myActConfig.Type == 2)
					{
						if (this.GetCurrentEverydayActJiFen(client, myActConfig) < myActConfig.Price.NumOne)
						{
							return -39;
						}
					}
					if (CheckCost && myActConfig.Type == 1)
					{
						if (client.ClientData.UserMoney < myActConfig.Price.NumOne)
						{
							return -10;
						}
					}
					result = 0;
				}
			}
			return result;
		}

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int ActID)
		{
			EverydayActInfoDB mySaveData = null;
			bool result;
			if (!client.ClientData.EverydayActInfoDict.TryGetValue(ActID, out mySaveData))
			{
				result = false;
			}
			else
			{
				EverydayActivityConfig myActConfig = null;
				if (!this.ActivityConfigDict.TryGetValue(mySaveData.ActID, out myActConfig))
				{
					result = false;
				}
				else
				{
					int nOccu = Global.CalcOriginalOccupationID(client);
					List<GoodsData> lData = new List<GoodsData>();
					foreach (GoodsData item in myActConfig.GoodsDataListOne)
					{
						lData.Add(item);
					}
					int count = myActConfig.GoodsDataListTwo.Count;
					for (int i = 0; i < count; i++)
					{
						GoodsData data = myActConfig.GoodsDataListTwo[i];
						if (Global.IsRoleOccupationMatchGoods(nOccu, data.GoodsID))
						{
							lData.Add(data);
						}
					}
					AwardItem tmpAwardItem = myActConfig.GoodsDataListThr.ToAwardItem();
					foreach (GoodsData item in tmpAwardItem.GoodsDataList)
					{
						lData.Add(item);
					}
					result = Global.CanAddGoodsDataList(client, lData);
				}
			}
			return result;
		}

		
		public int EverydayActGiveAward(GameClient client, int ActID)
		{
			EverydayActInfoDB mySaveData = null;
			int result;
			if (!client.ClientData.EverydayActInfoDict.TryGetValue(ActID, out mySaveData))
			{
				result = -2;
			}
			else
			{
				EverydayActivityConfig myActConfig = null;
				if (!this.ActivityConfigDict.TryGetValue(mySaveData.ActID, out myActConfig))
				{
					result = -2;
				}
				else
				{
					if (myActConfig.Type == 2)
					{
						if (!this.SubEverydayActJiFen(client, myActConfig))
						{
							return -39;
						}
					}
					if (myActConfig.Type == 1 && myActConfig.Price.NumOne > 0)
					{
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myActConfig.Price.NumOne, "每日活动抢购", true, true, false, DaiBiSySType.None))
						{
							return -10;
						}
					}
					AwardItem myAwardItem = new AwardItem();
					myAwardItem.GoodsDataList = myActConfig.GoodsDataListOne;
					base.GiveAward(client, myAwardItem);
					myAwardItem.GoodsDataList = myActConfig.GoodsDataListTwo;
					base.GiveAward(client, myAwardItem);
					myAwardItem = myActConfig.GoodsDataListThr.ToAwardItem();
					base.GiveEffectiveTimeAward(client, myAwardItem);
					if (myActConfig.PurchaseNum == -1)
					{
						mySaveData.PurNum = 1;
					}
					else
					{
						mySaveData.PurNum++;
					}
					this.UpdateClientEverydayActData(client, mySaveData);
					if (client._IconStateMgr.CheckEverydayActivity(client))
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
					result = 0;
				}
			}
			return result;
		}

		
		public string BuildFetchEverydayActAwardCmd(GameClient client, int ErrCode, int actID)
		{
			int roleID = client.ClientData.RoleID;
			EverydayActInfoDB mySaveData = null;
			string result;
			if (!client.ClientData.EverydayActInfoDict.TryGetValue(actID, out mySaveData))
			{
				result = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					-2,
					roleID,
					actID,
					0,
					0
				});
			}
			else
			{
				EverydayActivityConfig myActConfig = null;
				if (!this.ActivityConfigDict.TryGetValue(mySaveData.ActID, out myActConfig))
				{
					result = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-2,
						roleID,
						actID,
						0,
						0
					});
				}
				else
				{
					if (myActConfig.Type == 14)
					{
						mySaveData.PurNum = UserMoneyMgr.getInstance().GetChargeItemDayPurchaseNum(client, myActConfig.Price.ZhiGouID);
					}
					int LeftPurNum = myActConfig.PurchaseNum - mySaveData.PurNum;
					EveryActGoalData CurGoalNum = this.GetCurrentGoalNum(client, mySaveData, myActConfig);
					int ShowNum = CurGoalNum.NumOne;
					result = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						ErrCode,
						roleID,
						actID,
						LeftPurNum,
						ShowNum
					});
				}
			}
			return result;
		}

		
		public void NotifyActivityState(GameClient client)
		{
			bool bNotifyOpen = false;
			if (client.ClientData.EverydayActInfoDict != null && client.ClientData.EverydayActInfoDict.Count != 0 && this.PlatformOpenStateVavle == 1 && !client.ClientSocket.IsKuaFuLogin)
			{
				bNotifyOpen = true;
			}
			if (bNotifyOpen)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					12,
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
					12,
					0,
					"",
					0,
					0
				});
				client.sendCmd(770, strcmd, false);
			}
		}

		
		public void ShowActiveConditionInfoGM(GameClient client)
		{
			for (int loop = 0; loop < 23; loop++)
			{
				EverydayActNeedType NeedType = this.ConvertPaiHangTypesToActNeedType((PaiHangTypes)loop);
				if (EverydayActNeedType.EANT_Null != NeedType)
				{
					string strcmd = StringUtil.substitute("{0}:{1}:{2}", new object[]
					{
						0,
						loop,
						100
					});
					PaiHangData paiHangData = Global.sendToDB<PaiHangData, string>(269, strcmd, 0);
					if (null == paiHangData)
					{
						this.ActiveConditionDict.Clear();
						return;
					}
					if (null != paiHangData.PaiHangList)
					{
						this.CacheNeedCondition(NeedType, paiHangData);
					}
				}
			}
			foreach (KeyValuePair<int, EveryActActiveData> item in this.ActiveConditionDict)
			{
				string strinfo = string.Format("NeedType={0} NumOne={1} NumTwo={2}", item.Key, item.Value.NumOne, item.Value.NumTwo);
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
			}
			if (null != client.ClientData.EverydayActInfoDict)
			{
				foreach (EverydayActInfoDB item2 in client.ClientData.EverydayActInfoDict.Values)
				{
					EverydayActivityGroupConfig myInfo = null;
					if (this.ActivityGroupConfigDict.TryGetValue(item2.GroupID, out myInfo))
					{
						string strinfo = string.Format("GroupID={0} ActID={1} PurNum={2} CountNum={3} TypeID={4}", new object[]
						{
							item2.GroupID,
							item2.ActID,
							item2.PurNum,
							item2.CountNum,
							myInfo.TypeID
						});
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					}
				}
			}
			List<EverydayActGroupInfoDB> ActGroupInfoList = this.GetCurrentActGroupInfo();
			foreach (EverydayActGroupInfoDB item3 in ActGroupInfoList)
			{
				EverydayActivityGroupConfig myInfo = null;
				if (this.ActivityGroupConfigDict.TryGetValue(item3.GroupID, out myInfo))
				{
					string strinfo = string.Format("ActiveDay={0} GroupID={1} TypeID={2}", item3.ActiveDay, item3.GroupID, myInfo.TypeID);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
				}
			}
		}

		
		public void CacheNeedCondition(EverydayActNeedType type, PaiHangData paiHangData)
		{
			EveryActActiveData myActiveData = new EveryActActiveData();
			List<PaiHangItemData> PaiHangList = paiHangData.PaiHangList;
			int DivCalNum = Math.Min(PaiHangList.Count, 100);
			for (int i = 0; i < DivCalNum; i++)
			{
				PaiHangItemData phData = PaiHangList[i];
				if (EverydayActNeedType.EANT_CombatForce == type || EverydayActNeedType.EANT_UserMoney == type || EverydayActNeedType.EANT_ChengJiu == type || EverydayActNeedType.EANT_ShengWang == type || EverydayActNeedType.EANT_HolyItem == type)
				{
					myActiveData.NumOne += (long)phData.Val1;
				}
				else if (EverydayActNeedType.EANT_Level == type || EverydayActNeedType.EANT_Wing == type || EverydayActNeedType.EANT_Ring == type || EverydayActNeedType.EANT_Merlin == type || EverydayActNeedType.EANT_GuardStatue == type)
				{
					myActiveData.NumOne += (long)phData.Val1;
					myActiveData.NumTwo += (long)phData.Val2;
				}
			}
			if (DivCalNum > 0)
			{
				if (EverydayActNeedType.EANT_CombatForce == type || EverydayActNeedType.EANT_UserMoney == type || EverydayActNeedType.EANT_ChengJiu == type || EverydayActNeedType.EANT_ShengWang == type || EverydayActNeedType.EANT_HolyItem == type)
				{
					myActiveData.NumOne /= (long)DivCalNum;
				}
				else if (EverydayActNeedType.EANT_Level == type)
				{
					int TransNum = Global.GetUnionLevel2((int)myActiveData.NumTwo, (int)myActiveData.NumOne) / DivCalNum;
					myActiveData.NumOne = (long)((TransNum - 1) / 100);
					myActiveData.NumTwo = (long)((TransNum - 1) % 100 + 1);
				}
				else if (EverydayActNeedType.EANT_Wing == type)
				{
					int TransNum = (int)myActiveData.NumOne * 10 + (int)myActiveData.NumTwo;
					TransNum /= DivCalNum;
					myActiveData.NumOne = (long)(TransNum / 10);
					myActiveData.NumTwo = (long)(TransNum % 10);
				}
				else if (EverydayActNeedType.EANT_Ring == type)
				{
					int TransNum = (int)myActiveData.NumOne * MarriageOtherLogic.getInstance().GetMaxGoodwillStar() + (int)myActiveData.NumTwo;
					TransNum /= DivCalNum;
					myActiveData.NumOne = (long)(TransNum / MarriageOtherLogic.getInstance().GetMaxGoodwillStar());
					myActiveData.NumTwo = (long)(TransNum % MarriageOtherLogic.getInstance().GetMaxGoodwillStar());
				}
				else if (EverydayActNeedType.EANT_Merlin == type)
				{
					int TransNum = (int)myActiveData.NumOne * MerlinSystemParamsConfigData._MaxStarNum + (int)myActiveData.NumTwo;
					TransNum /= DivCalNum;
					myActiveData.NumOne = (long)(TransNum / MerlinSystemParamsConfigData._MaxStarNum);
					myActiveData.NumTwo = (long)(TransNum % MerlinSystemParamsConfigData._MaxStarNum);
				}
				else if (EverydayActNeedType.EANT_GuardStatue == type)
				{
					int TransNum = (int)myActiveData.NumTwo;
					TransNum /= DivCalNum;
					myActiveData.NumOne = (long)(TransNum / 10 + 1);
					myActiveData.NumTwo = (long)(TransNum % 10);
				}
			}
			this.ActiveConditionDict[(int)type] = myActiveData;
		}

		
		private bool CheckNeedCondition(EverydayActivityGroupConfig myActGroupConfig)
		{
			EveryActActiveData myActiveData = null;
			if (!this.ActiveConditionDict.TryGetValue(myActGroupConfig.NeedType, out myActiveData))
			{
				myActiveData = new EveryActActiveData();
			}
			return this.CheckFirstSecondCondition((int)myActiveData.NumOne, (int)myActiveData.NumTwo, myActGroupConfig.NeedNum);
		}

		
		private void CleanUpActGroupInfo(List<EverydayActGroupInfoDB> ActGroupInfoList, int CleanDayNum)
		{
			int NowDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
			ActGroupInfoList.RemoveAll((EverydayActGroupInfoDB x) => NowDay - x.ActiveDay > CleanDayNum);
		}

		
		private List<EverydayActGroupInfoDB> GetCurrentActGroupInfo()
		{
			List<EverydayActGroupInfoDB> ActGroupInfoList = new List<EverydayActGroupInfoDB>();
			string sEverydayAct = GameManager.GameConfigMgr.GetGameConfigItemStr("everydayact", "");
			if (!string.IsNullOrEmpty(sEverydayAct))
			{
				string[] actFields = sEverydayAct.Split(new char[]
				{
					'|'
				});
				foreach (string item in actFields)
				{
					string[] groupFields = item.Split(new char[]
					{
						','
					});
					if (groupFields.Length == 2)
					{
						ActGroupInfoList.Add(new EverydayActGroupInfoDB
						{
							ActiveDay = Global.SafeConvertToInt32(groupFields[0]),
							GroupID = Global.SafeConvertToInt32(groupFields[1])
						});
					}
				}
			}
			this.CleanUpActGroupInfo(ActGroupInfoList, 2);
			return ActGroupInfoList;
		}

		
		private void SaveCurrentActGroupInfo(List<EverydayActGroupInfoDB> ActGroupInfoList)
		{
			string strResult = "";
			foreach (EverydayActGroupInfoDB item in ActGroupInfoList)
			{
				strResult += string.Format("{0},{1}|", item.ActiveDay, item.GroupID);
			}
			if (!string.IsNullOrEmpty(strResult) && strResult.Substring(strResult.Length - 1) == "|")
			{
				strResult = strResult.Substring(0, strResult.Length - 1);
			}
			GameManager.GameConfigMgr.SetGameConfigItem("everydayact", strResult);
			Global.UpdateDBGameConfigg("everydayact", strResult);
		}

		
		private List<int> FilterValidGroupIDList(List<int> GroupIDList, int TypeID)
		{
			List<int> copyGroupIDList = new List<int>(GroupIDList);
			copyGroupIDList.RemoveAll(delegate(int item)
			{
				EverydayActivityGroupConfig myValid = null;
				return !this.ActivityGroupConfigDict.TryGetValue(item, out myValid) || myValid.TypeID != TypeID;
			});
			return copyGroupIDList;
		}

		
		private HashSet<int> FilterValidTypeIDSet(HashSet<int> TypeIDSet, List<EverydayActGroupInfoDB> ActGroupInfoList)
		{
			HashSet<int> copyTypeIDSet = new HashSet<int>(TypeIDSet);
			copyTypeIDSet.RemoveWhere(delegate(int item)
			{
				bool TypeValid = true;
				foreach (EverydayActGroupInfoDB info in ActGroupInfoList)
				{
					EverydayActivityGroupConfig myInfo = null;
					if (this.ActivityGroupConfigDict.TryGetValue(info.GroupID, out myInfo))
					{
						if (item == myInfo.TypeID)
						{
							TypeValid = false;
							break;
						}
					}
				}
				return !TypeValid;
			});
			return copyTypeIDSet;
		}

		
		private void GenerateEverydayActGroupID(List<EverydayActGroupInfoDB> ActGroupInfoList)
		{
			int NowDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
			foreach (EverydayActGroupInfoDB item in ActGroupInfoList)
			{
				if (NowDay == item.ActiveDay)
				{
					return;
				}
			}
			for (int loop = 0; loop < 23; loop++)
			{
				EverydayActNeedType NeedType = this.ConvertPaiHangTypesToActNeedType((PaiHangTypes)loop);
				if (EverydayActNeedType.EANT_Null != NeedType)
				{
					string strcmd = StringUtil.substitute("{0}:{1}:{2}", new object[]
					{
						0,
						loop,
						100
					});
					PaiHangData paiHangData = Global.sendToDB<PaiHangData, string>(269, strcmd, 0);
					if (null == paiHangData)
					{
						this.ActiveConditionDict.Clear();
						return;
					}
					if (null != paiHangData.PaiHangList)
					{
						this.CacheNeedCondition(NeedType, paiHangData);
					}
				}
			}
			EveryActActiveData myActiveData = null;
			if (this.ActiveConditionDict.TryGetValue(2, out myActiveData))
			{
				HashSet<int> ValidTypeIDSet = new HashSet<int>();
				List<int> ValidGroupIDList = new List<int>();
				foreach (EverydayActivityGroupConfig item2 in this.ActivityGroupConfigDict.Values)
				{
					EverydayActivityGroupConfig myGroupConifg = item2;
					EverydayActivityTypeConfig myTypeConfig = null;
					if (this.ActivityTypeConfigDict.TryGetValue(myGroupConifg.TypeID, out myTypeConfig))
					{
						if (this.CheckFirstSecondCondition((int)myActiveData.NumOne, (int)myActiveData.NumTwo, myTypeConfig.LevLimit))
						{
							if (this.CheckNeedCondition(myGroupConifg))
							{
								ValidTypeIDSet.Add(myGroupConifg.TypeID);
								ValidGroupIDList.Add(myGroupConifg.GroupID);
							}
						}
					}
				}
				int GenerateGroupID = 0;
				List<EverydayActGroupInfoDB> copyActGroupInfoList = new List<EverydayActGroupInfoDB>(ActGroupInfoList);
				for (int dayLoop = 2; dayLoop >= 0; dayLoop--)
				{
					this.CleanUpActGroupInfo(copyActGroupInfoList, dayLoop);
					HashSet<int> filterVTypeIDSet = this.FilterValidTypeIDSet(ValidTypeIDSet, copyActGroupInfoList);
					if (filterVTypeIDSet.Count != 0)
					{
						int[] ValidTypeIDArray = filterVTypeIDSet.ToArray<int>();
						int RandValue = Global.GetRandomNumber(0, ValidTypeIDArray.Length);
						int RandomTypeID = ValidTypeIDArray[RandValue];
						List<int> filterVGIDList = this.FilterValidGroupIDList(ValidGroupIDList, RandomTypeID);
						if (filterVGIDList.Count != 0)
						{
							RandValue = Global.GetRandomNumber(0, filterVGIDList.Count);
							GenerateGroupID = filterVGIDList[RandValue];
							break;
						}
					}
				}
				EverydayActivityGroupConfig myGroupConfig = null;
				if (this.ActivityGroupConfigDict.TryGetValue(GenerateGroupID, out myGroupConfig))
				{
					ActGroupInfoList.Add(new EverydayActGroupInfoDB
					{
						GroupID = GenerateGroupID,
						ActiveDay = NowDay
					});
				}
				this.SaveCurrentActGroupInfo(ActGroupInfoList);
			}
		}

		
		private void GenerateEverydayActivity(GameClient client)
		{
			if (this.PlatformOpenStateVavle == 1 && !client.ClientSocket.IsKuaFuLogin)
			{
				List<EverydayActGroupInfoDB> copyActGroupInfoList = null;
				lock (this.Mutex)
				{
					List<EverydayActGroupInfoDB> ActGroupInfoList = this.GetCurrentActGroupInfo();
					this.GenerateEverydayActGroupID(ActGroupInfoList);
					copyActGroupInfoList = new List<EverydayActGroupInfoDB>(ActGroupInfoList);
					this.CleanUpActGroupInfo(copyActGroupInfoList, 0);
				}
				lock (client.ClientData)
				{
					if (null == client.ClientData.EverydayActInfoDict)
					{
						client.ClientData.EverydayActInfoDict = new Dictionary<int, EverydayActInfoDB>();
					}
					Dictionary<int, EverydayActInfoDB> EverydayActInfoDictUpdate = new Dictionary<int, EverydayActInfoDB>(client.ClientData.EverydayActInfoDict);
					using (Dictionary<int, EverydayActInfoDB>.ValueCollection.Enumerator enumerator = client.ClientData.EverydayActInfoDict.Values.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							EverydayActInfoDB info = enumerator.Current;
							if (!copyActGroupInfoList.Exists((EverydayActGroupInfoDB x) => x.GroupID == info.GroupID && x.ActiveDay == info.ActiveDay))
							{
								EverydayActInfoDictUpdate.Remove(info.ActID);
								this.DeleteClientEverydayActData(client, info.GroupID, 0);
							}
						}
					}
					foreach (EverydayActGroupInfoDB item in copyActGroupInfoList)
					{
						EverydayActivityGroupConfig myGroupConfig = null;
						if (this.ActivityGroupConfigDict.TryGetValue(item.GroupID, out myGroupConfig))
						{
							using (Dictionary<int, EverydayActInfoDB>.ValueCollection.Enumerator enumerator = client.ClientData.EverydayActInfoDict.Values.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									EverydayActInfoDB info = enumerator.Current;
									if (item.GroupID == info.GroupID)
									{
										if (!myGroupConfig.ActivityIDList.Exists((int x) => x == info.ActID))
										{
											EverydayActInfoDictUpdate.Remove(info.ActID);
											this.DeleteClientEverydayActData(client, info.GroupID, info.ActID);
										}
									}
								}
							}
							foreach (int actid in myGroupConfig.ActivityIDList)
							{
								EverydayActivityConfig myActConfig = null;
								if (this.ActivityConfigDict.TryGetValue(actid, out myActConfig))
								{
									if (myActConfig.Type != 14 || UserMoneyMgr.getInstance().PlatformOpenStateVavle != 0)
									{
										EverydayActInfoDB EverydayActData = null;
										if (!EverydayActInfoDictUpdate.TryGetValue(actid, out EverydayActData))
										{
											EverydayActData = new EverydayActInfoDB
											{
												GroupID = item.GroupID,
												ActID = actid,
												ActiveDay = item.ActiveDay
											};
											EverydayActInfoDictUpdate[EverydayActData.ActID] = EverydayActData;
											this.UpdateClientEverydayActData(client, EverydayActData);
										}
									}
								}
							}
						}
					}
					client.ClientData.EverydayActInfoDict = EverydayActInfoDictUpdate;
				}
			}
		}

		
		private EverydayActNeedType ConvertPaiHangTypesToActNeedType(PaiHangTypes type)
		{
			EverydayActNeedType NeedType = EverydayActNeedType.EANT_Null;
			switch (type)
			{
			case PaiHangTypes.RoleLevel:
				NeedType = EverydayActNeedType.EANT_Level;
				break;
			case PaiHangTypes.CombatForceList:
				NeedType = EverydayActNeedType.EANT_CombatForce;
				break;
			case PaiHangTypes.Wing:
				NeedType = EverydayActNeedType.EANT_Wing;
				break;
			case PaiHangTypes.Ring:
				NeedType = EverydayActNeedType.EANT_Ring;
				break;
			case PaiHangTypes.Merlin:
				NeedType = EverydayActNeedType.EANT_Merlin;
				break;
			case PaiHangTypes.UserMoney:
				NeedType = EverydayActNeedType.EANT_UserMoney;
				break;
			case PaiHangTypes.ChengJiu:
				NeedType = EverydayActNeedType.EANT_ChengJiu;
				break;
			case PaiHangTypes.ShengWang:
				NeedType = EverydayActNeedType.EANT_ShengWang;
				break;
			case PaiHangTypes.GuardStatue:
				NeedType = EverydayActNeedType.EANT_GuardStatue;
				break;
			case PaiHangTypes.HolyItem:
				NeedType = EverydayActNeedType.EANT_HolyItem;
				break;
			}
			return NeedType;
		}

		
		private void DeleteClientEverydayActData(GameClient client, int GroupID, int ActID = 0)
		{
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, GroupID, ActID);
			Global.ExecuteDBCmd(13171, strcmd, client.ServerId);
		}

		
		private void UpdateClientEverydayActData(GameClient client, EverydayActInfoDB EverydayActData)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				client.ClientData.RoleID,
				EverydayActData.GroupID,
				EverydayActData.ActID,
				EverydayActData.PurNum,
				EverydayActData.CountNum,
				EverydayActData.ActiveDay
			});
			Global.ExecuteDBCmd(13170, strcmd, client.ServerId);
		}

		
		private bool CheckFirstSecondCondition(int FirstValue, int SecondValue, EveryActLimitData Limit)
		{
			if (Limit.MinFirst != -1)
			{
				if (FirstValue < Limit.MinFirst || (Limit.MinSecond != -1 && FirstValue == Limit.MinFirst && SecondValue < Limit.MinSecond))
				{
					return false;
				}
			}
			if (Limit.MaxFirst != -1)
			{
				if (FirstValue > Limit.MaxFirst || (Limit.MaxSecond != -1 && FirstValue == Limit.MaxFirst && SecondValue > Limit.MaxSecond))
				{
					return false;
				}
			}
			return true;
		}

		
		private int CalMyGuardStatueLevel(GameClient client)
		{
			GuardStatueData data = client.ClientData.MyGuardStatueDetail.GuardStatue;
			int result;
			if (data.Level > 0 && data.Level % 10 == 0 && (data.Level + 10) / 10 != data.Suit)
			{
				result = 10;
			}
			else
			{
				result = data.Level % 10;
			}
			return result;
		}

		
		private int GetCurrentEverydayActJiFen(GameClient client, EverydayActivityConfig myActConfig)
		{
			DateTime nowDateTime = TimeUtil.NowDateTime();
			DateTime FromActDate = new DateTime(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day, 0, 0, 0);
			DateTime ToActDate = new DateTime(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day, 23, 59, 59);
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, FromActDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'), ToActDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'));
			string[] fields = Global.ExecuteDBCmd(13172, strcmd, client.ServerId);
			int result;
			if (fields == null || fields.Length < 2)
			{
				result = 0;
			}
			else
			{
				result = Global.SafeConvertToInt32(fields[1]);
			}
			return result;
		}

		
		private bool SubEverydayActJiFen(GameClient client, EverydayActivityConfig myActConfig)
		{
			DateTime nowDateTime = TimeUtil.NowDateTime();
			DateTime FromActDate = new DateTime(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day, 0, 0, 0);
			DateTime ToActDate = new DateTime(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day, 23, 59, 59);
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				client.ClientData.RoleID,
				-myActConfig.Price.NumOne,
				FromActDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'),
				ToActDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$')
			});
			string[] fields = Global.ExecuteDBCmd(13173, strcmd, client.ServerId);
			bool result;
			if (fields == null || fields.Length < 2)
			{
				result = false;
			}
			else
			{
				int retJiFen = Convert.ToInt32(fields[1]);
				result = (retJiFen >= 0);
			}
			return result;
		}

		
		private EveryActGoalData GetCurrentGoalNum(GameClient client, EverydayActInfoDB mySaveData, EverydayActivityConfig myActConfig)
		{
			EveryActGoalData GoalNum = new EveryActGoalData();
			switch (myActConfig.Type)
			{
			case 1:
				GoalNum.NumOne = client.ClientData.UserMoney;
				break;
			case 2:
				GoalNum.NumOne = this.GetCurrentEverydayActJiFen(client, myActConfig);
				break;
			case 3:
				GoalNum.NumOne = mySaveData.CountNum;
				break;
			case 5:
				GoalNum.NumOne = client.ClientData.ChangeLifeCount;
				GoalNum.NumTwo = client.ClientData.Level;
				break;
			case 6:
				if (client.ClientData.MyWingData != null)
				{
					GoalNum.NumOne = client.ClientData.MyWingData.WingID;
					GoalNum.NumTwo = client.ClientData.MyWingData.ForgeLevel;
				}
				break;
			case 7:
				GoalNum.NumOne = client.ClientData.VipLevel;
				break;
			case 8:
				GoalNum.NumOne = ChengJiuManager.GetChengJiuLevel(client);
				break;
			case 9:
				GoalNum.NumOne = GameManager.ClientMgr.GetShengWangLevelValue(client);
				break;
			case 10:
				if (client.ClientData.MerlinData != null)
				{
					GoalNum.NumOne = client.ClientData.MerlinData._Level;
					GoalNum.NumTwo = client.ClientData.MerlinData._StarNum;
				}
				break;
			case 11:
			{
				int TotalLev = 0;
				foreach (KeyValuePair<sbyte, HolyItemData> kvp in client.ClientData.MyHolyItemDataDic)
				{
					HolyItemData myHolyData = kvp.Value;
					foreach (KeyValuePair<sbyte, HolyItemPartData> item in myHolyData.m_PartArray)
					{
						HolyItemPartData myHolyPartData = item.Value;
						TotalLev += (int)myHolyPartData.m_sSuit;
					}
				}
				GoalNum.NumOne = TotalLev;
				break;
			}
			case 12:
				if (client.ClientData.MyMarriageData != null)
				{
					GoalNum.NumOne = (int)client.ClientData.MyMarriageData.byGoodwilllevel;
					GoalNum.NumTwo = (int)client.ClientData.MyMarriageData.byGoodwillstar;
				}
				break;
			case 13:
				if (client.ClientData.MyGuardStatueDetail != null)
				{
					GoalNum.NumOne = client.ClientData.MyGuardStatueDetail.GuardStatue.Suit;
					GoalNum.NumTwo = this.CalMyGuardStatueLevel(client);
				}
				break;
			}
			return GoalNum;
		}

		
		public void Dispose()
		{
			GlobalEventSource.getInstance().removeListener(36, this);
		}

		
		public bool Init()
		{
			try
			{
				Dictionary<int, int> OpenStateDict = new Dictionary<int, int>();
				string strPlatformOpen = GameManager.systemParamsList.GetParamValueByName("EveryDayActivityOpen");
				if (!string.IsNullOrEmpty(strPlatformOpen))
				{
					string[] Fields = strPlatformOpen.Split(new char[]
					{
						'|'
					});
					foreach (string dat in Fields)
					{
						string[] State = dat.Split(new char[]
						{
							','
						});
						if (State.Length == 2)
						{
							OpenStateDict[Global.SafeConvertToInt32(State[0])] = Global.SafeConvertToInt32(State[1]);
						}
					}
				}
				OpenStateDict.TryGetValue(UserMoneyMgr.getInstance().GetActivityPlatformType(), out this.PlatformOpenStateVavle);
				if (!this.LoadEverydayActivityTypeData())
				{
					return false;
				}
				if (!this.LoadEverydayActivityGroupData())
				{
					return false;
				}
				if (!this.LoadEverydayActivityData())
				{
					return false;
				}
				this.CheckEverydayConfigFileLogic();
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				this.ActivityType = 47;
				base.PredealDateTime();
				GlobalEventSource.getInstance().registerListener(36, this);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		private void CheckEverydayConfigFileLogic()
		{
			foreach (EverydayActivityGroupConfig groupItem in this.ActivityGroupConfigDict.Values)
			{
				EverydayActivityTypeConfig myTypeConfig = null;
				if (!this.ActivityTypeConfigDict.TryGetValue(groupItem.TypeID, out myTypeConfig))
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("警告：每日活动找不到对应TypeID GroupID={0} TypeID={1}", groupItem.GroupID, groupItem.TypeID), null, true);
				}
				foreach (int actItem in groupItem.ActivityIDList)
				{
					EverydayActivityConfig myActConfig = null;
					if (!this.ActivityConfigDict.TryGetValue(actItem, out myActConfig))
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("警告：每日活动找不到对应ActivityID GroupID={0} ActivityID={1}", groupItem.GroupID, actItem), null, true);
					}
				}
			}
		}

		
		private bool ParseEverydayActLimitData(EveryActLimitData LevLimit, string Value)
		{
			bool result;
			if (string.Compare(Value, "-1") == 0 || string.IsNullOrEmpty(Value))
			{
				result = true;
			}
			else
			{
				string[] Fields = Value.Split(new char[]
				{
					'|'
				});
				if (Fields.Length != 2)
				{
					result = false;
				}
				else
				{
					string[] LimitFirst = Fields[0].Split(new char[]
					{
						','
					});
					string[] LimitSecond = Fields[1].Split(new char[]
					{
						','
					});
					if (LimitFirst.Length == 2 && LimitSecond.Length == 2)
					{
						LevLimit.MinFirst = Global.SafeConvertToInt32(LimitFirst[0]);
						LevLimit.MinSecond = Global.SafeConvertToInt32(LimitFirst[1]);
						LevLimit.MaxFirst = Global.SafeConvertToInt32(LimitSecond[0]);
						LevLimit.MaxSecond = Global.SafeConvertToInt32(LimitSecond[1]);
					}
					else
					{
						if (LimitFirst.Length != 1 || LimitSecond.Length != 1)
						{
							return false;
						}
						LevLimit.MinFirst = Global.SafeConvertToInt32(LimitFirst[0]);
						LevLimit.MaxFirst = Global.SafeConvertToInt32(LimitSecond[0]);
					}
					result = true;
				}
			}
			return result;
		}

		
		public bool LoadEverydayActivityTypeData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EveryDayActivity/EveryDayActivityType.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EveryDayActivity/EveryDayActivityType.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						EverydayActivityTypeConfig myData = new EverydayActivityTypeConfig();
						myData.TypeID = (int)Global.GetSafeAttributeLong(xmlItem, "TypeID");
						string TempString = Global.GetSafeAttributeStr(xmlItem, "OpenLevel");
						string[] TempField = TempString.Split(new char[]
						{
							'|'
						});
						if (TempField.Length == 2)
						{
							myData.LevLimit.MinFirst = Global.SafeConvertToInt32(TempField[0]);
							myData.LevLimit.MinSecond = Global.SafeConvertToInt32(TempField[1]);
						}
						TempString = Global.GetSafeAttributeStr(xmlItem, "CloseLevel");
						TempField = TempString.Split(new char[]
						{
							'|'
						});
						if (TempField.Length == 2)
						{
							myData.LevLimit.MaxFirst = Global.SafeConvertToInt32(TempField[0]);
							myData.LevLimit.MaxSecond = Global.SafeConvertToInt32(TempField[1]);
						}
						this.ActivityTypeConfigDict[myData.TypeID] = myData;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/EveryDayActivity/EveryDayActivityType.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public bool LoadEverydayActivityGroupData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EveryDayActivity/EveryDayActivityGroup.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EveryDayActivity/EveryDayActivityGroup.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						EverydayActivityGroupConfig myData = new EverydayActivityGroupConfig();
						myData.GroupID = (int)Global.GetSafeAttributeLong(xmlItem, "GroupID");
						myData.TypeID = (int)Global.GetSafeAttributeLong(xmlItem, "TypeID");
						myData.NeedType = (int)Global.GetSafeAttributeLong(xmlItem, "NeedType");
						if (!this.ParseEverydayActLimitData(myData.NeedNum, Global.GetSafeAttributeStr(xmlItem, "NeedNum")))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析每日活动组文件NeedNum失败 GroupID:{0}", myData.GroupID), null, true);
						}
						else
						{
							string TempString = Global.GetSafeAttributeStr(xmlItem, "ActivityID");
							string[] TempField = TempString.Split(new char[]
							{
								'|'
							});
							foreach (string item in TempField)
							{
								myData.ActivityIDList.Add(Global.SafeConvertToInt32(item));
							}
							this.ActivityGroupConfigDict[myData.GroupID] = myData;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/EveryDayActivity/EveryDayActivityGroup.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public bool LoadEverydayActivityData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EveryDayActivity/EveryDayActivity.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EveryDayActivity/EveryDayActivity.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						EverydayActivityConfig myData = new EverydayActivityConfig();
						myData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ActivityID");
						myData.Type = (int)Global.GetSafeAttributeLong(xmlItem, "GoalType");
						string GoalString = Global.GetSafeAttributeStr(xmlItem, "GoalNum");
						string[] GoalFields = GoalString.Split(new char[]
						{
							','
						});
						if (GoalFields.Length == 2)
						{
							myData.GoalData.NumOne = Global.SafeConvertToInt32(GoalFields[0]);
							myData.GoalData.NumTwo = Global.SafeConvertToInt32(GoalFields[1]);
						}
						else
						{
							myData.GoalData.NumOne = Global.SafeConvertToInt32(GoalFields[0]);
						}
						string goodsIDsOne = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
						string[] fields = goodsIDsOne.Split(new char[]
						{
							'|'
						});
						if (fields.Length <= 0)
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型每日活动配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							myData.GoodsDataListOne = HuodongCachingMgr.ParseGoodsDataList(fields, "每日活动配置1");
						}
						string goodsIDsTwo = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
						if (!string.IsNullOrEmpty(goodsIDsTwo))
						{
							fields = goodsIDsTwo.Split(new char[]
							{
								'|'
							});
							myData.GoodsDataListTwo = HuodongCachingMgr.ParseGoodsDataList(fields, "每日活动配置2");
						}
						string goodsIDsThr = Global.GetSafeAttributeStr(xmlItem, "GoodsThr");
						myData.GoodsDataListThr.Init(goodsIDsThr, Global.GetSafeAttributeStr(xmlItem, "EffectiveTime"), "每日活动配置3");
						string Price = Global.GetSafeAttributeStr(xmlItem, "Price");
						string[] PriceFields = Price.Split(new char[]
						{
							'|'
						});
						if (PriceFields.Length == 2)
						{
							myData.Price.NumOne = Global.SafeConvertToInt32(PriceFields[0]);
							myData.Price.NumTwo = Global.SafeConvertToInt32(PriceFields[1]);
						}
						else if (PriceFields.Length == 3)
						{
							myData.Price.NumOne = Global.SafeConvertToInt32(PriceFields[0]);
							myData.Price.NumTwo = Global.SafeConvertToInt32(PriceFields[1]);
							myData.Price.ZhiGouID = Global.SafeConvertToInt32(PriceFields[2]);
						}
						else
						{
							myData.Price.NumOne = Global.SafeConvertToInt32(PriceFields[0]);
						}
						myData.PurchaseNum = (int)Global.GetSafeAttributeLong(xmlItem, "PurchaseNum");
						if (myData.Type == 14)
						{
							UserMoneyMgr.getInstance().CheckChargeItemConfigLogic(myData.Price.ZhiGouID, myData.PurchaseNum, goodsIDsOne, goodsIDsTwo, string.Format("每日活动 ID={0}", myData.ID));
						}
						this.ActivityConfigDict[myData.ID] = myData;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/EveryDayActivity/EveryDayActivity.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		protected const string EveryDayChongZhiDuiHuan = "EveryDayChongZhiDuiHuan";

		
		protected const string EveryDayActivityOpen = "EveryDayActivityOpen";

		
		public const string EverydayActivityTypeData_fileName = "Config/EveryDayActivity/EveryDayActivityType.xml";

		
		public const string EverydayActivityGroupData_fileName = "Config/EveryDayActivity/EveryDayActivityGroup.xml";

		
		public const string EverydayActivityData_fileName = "Config/EveryDayActivity/EveryDayActivity.xml";

		
		public const int MaxActiveConditionDataNum = 100;

		
		public object Mutex = new object();

		
		protected int PlatformOpenStateVavle = 0;

		
		protected Dictionary<int, EverydayActivityTypeConfig> ActivityTypeConfigDict = new Dictionary<int, EverydayActivityTypeConfig>();

		
		protected Dictionary<int, EverydayActivityGroupConfig> ActivityGroupConfigDict = new Dictionary<int, EverydayActivityGroupConfig>();

		
		protected Dictionary<int, EverydayActivityConfig> ActivityConfigDict = new Dictionary<int, EverydayActivityConfig>();

		
		protected Dictionary<int, EveryActActiveData> ActiveConditionDict = new Dictionary<int, EveryActActiveData>();
	}
}
