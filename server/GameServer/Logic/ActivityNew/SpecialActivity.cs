using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.UserMoneyCharge;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	
	public class SpecialActivity : Activity, IEventListener
	{
		
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 36)
			{
				int ZhiGouActID = -1;
				ChargeItemBaseEventObject obj = eventObject as ChargeItemBaseEventObject;
				foreach (KeyValuePair<int, SpecActInfoDB> kvp in obj.Player.ClientData.SpecActInfoDict)
				{
					SpecActInfoDB myData = kvp.Value;
					if (myData.Active != 0)
					{
						SpecialActivityConfig actProto = null;
						if (this.SpecialActDict.TryGetValue(myData.ActID, out actProto))
						{
							if (actProto.Type == 14 && actProto.Price.ZhiGouID == obj.ChargeItemConfig.ChargeItemID)
							{
								ZhiGouActID = myData.ActID;
								break;
							}
						}
					}
				}
				if (ZhiGouActID != -1)
				{
					string cmd = this.BuildFetchSpecActAwardCmd(obj.Player, 0, ZhiGouActID);
					obj.Player.sendCmd<string>(1512, cmd, false);
				}
			}
		}

		
		public void OnMoneyChargeEventOnLine(GameClient client, int addMoney)
		{
			int GroupID = this.GenerateSpecActGroupID();
			if (-1 != GroupID)
			{
				SpecialActivityTimeConfig timeConfig = null;
				if (this.SpecialActTimeDict.TryGetValue(GroupID, out timeConfig))
				{
					string FromActDate = timeConfig.FromDate.ToString("yyyy-MM-dd HH:mm:ss");
					string ToActDate = timeConfig.ToDate.ToString("yyyy-MM-dd HH:mm:ss");
					if (!string.IsNullOrEmpty(FromActDate) && !string.IsNullOrEmpty(ToActDate))
					{
						this.OnMoneyChargeEvent(client.strUserID, client.ClientData.RoleID, addMoney, FromActDate, ToActDate);
					}
				}
			}
		}

		
		public void OnMoneyChargeEventOffLine(string userid, int roleid, int addMoney)
		{
			int GroupID = this.GenerateSpecActGroupID();
			SpecialActivityTimeConfig timeConfig = null;
			if (this.SpecialActTimeDict.TryGetValue(GroupID, out timeConfig))
			{
				string FromActDate = timeConfig.FromDate.ToString("yyyy-MM-dd HH:mm:ss");
				string ToActDate = timeConfig.ToDate.ToString("yyyy-MM-dd HH:mm:ss");
				this.OnMoneyChargeEvent(userid, roleid, addMoney, FromActDate, ToActDate);
			}
		}

		
		protected void OnMoneyChargeEvent(string userid, int roleid, int addMoney, string FromActDate, string ToActDate)
		{
			string strYuanbaoToJiFen = GameManager.systemParamsList.GetParamValueByName("SpecialChongZhiDuiHuan");
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
						string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							roleid,
							JiFenAdd,
							FromActDate.Replace(':', '$'),
							ToActDate.Replace(':', '$')
						});
						Global.ExecuteDBCmd(13163, strcmd, 0);
					}
				}
			}
		}

		
		public void MoneyConst(GameClient client, int moneyCost)
		{
			if (client.ClientData.SpecActInfoDict != null && client.ClientData.SpecActInfoDict.Count != 0)
			{
				foreach (KeyValuePair<int, SpecActInfoDB> kvp in client.ClientData.SpecActInfoDict)
				{
					SpecActInfoDB myData = kvp.Value;
					if (myData.Active != 0)
					{
						SpecialActivityConfig actProto = null;
						if (this.SpecialActDict.TryGetValue(myData.ActID, out actProto))
						{
							if (actProto.Type == 3)
							{
								myData.CountNum += moneyCost;
								this.UpdateClientSpecActData(client, myData);
							}
						}
					}
				}
				if (client._IconStateMgr.CheckSpecialActivity(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		
		public bool CheckIconState(GameClient client)
		{
			bool bFlush = false;
			bool result;
			if (client.ClientData.SpecActInfoDict == null || client.ClientData.SpecActInfoDict.Count == 0)
			{
				result = bFlush;
			}
			else
			{
				foreach (KeyValuePair<int, SpecActInfoDB> kvp in client.ClientData.SpecActInfoDict)
				{
					SpecActInfoDB myActInfoDB = kvp.Value;
					int ErrCode = this.SpecActCheckCondition(client, kvp.Key, false);
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

		
		public void OnRoleLogin(GameClient client, bool isLogin)
		{
			this.GenerateSpecActGroup(client);
			this.NotifyActivityState(client);
		}

		
		public SpecialActivityData GetSpecialActivityDataForClient(GameClient client)
		{
			SpecialActivityData mySpecActData = new SpecialActivityData();
			mySpecActData.GroupID = this.GetClientSpecActGroupID(client);
			mySpecActData.SpecActInfoList = new List<SpecActInfo>();
			foreach (KeyValuePair<int, SpecActInfoDB> kvp in client.ClientData.SpecActInfoDict)
			{
				SpecActInfoDB mySaveData = kvp.Value;
				if (mySaveData.Active != 0)
				{
					SpecialActivityConfig myActConfig = null;
					if (this.SpecialActDict.TryGetValue(mySaveData.ActID, out myActConfig))
					{
						SpecActInfo ActInfo = new SpecActInfo();
						ActInfo.ActID = mySaveData.ActID;
						SpecActGoalData CurGoalNum = this.GetCurrentGoalNum(client, mySaveData, myActConfig);
						ActInfo.ShowNum = CurGoalNum.NumOne;
						ActInfo.ShowNum2 = CurGoalNum.NumTwo;
						int PurNum = mySaveData.PurNum;
						if (myActConfig.Type == 14)
						{
							PurNum = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, myActConfig.Price.ZhiGouID);
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
						mySpecActData.SpecActInfoList.Add(ActInfo);
					}
				}
			}
			return mySpecActData;
		}

		
		public int SpecActCheckCondition(GameClient client, int ActID, bool CheckCost = true)
		{
			SpecActInfoDB mySaveData = null;
			int result;
			if (!client.ClientData.SpecActInfoDict.TryGetValue(ActID, out mySaveData))
			{
				result = -2;
			}
			else if (mySaveData.Active == 0)
			{
				result = -2;
			}
			else
			{
				SpecialActivityConfig myActConfig = null;
				if (!this.SpecialActDict.TryGetValue(mySaveData.ActID, out myActConfig))
				{
					result = -2;
				}
				else if (myActConfig.Type == 14)
				{
					result = -12;
				}
				else
				{
					DateTime nowDateTm = TimeUtil.NowDateTime();
					if (nowDateTm < myActConfig.FromDay || nowDateTm > myActConfig.ToDay)
					{
						result = -2001;
					}
					else
					{
						SpecActGoalData CurGoalNum = this.GetCurrentGoalNum(client, mySaveData, myActConfig);
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
						if (myActConfig.Type == 2)
						{
							if (this.GetCurrentSpecActJiFen(client, myActConfig) < myActConfig.Price.NumOne)
							{
								return -24;
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
			}
			return result;
		}

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int ActID)
		{
			SpecActInfoDB mySaveData = null;
			bool result;
			if (!client.ClientData.SpecActInfoDict.TryGetValue(ActID, out mySaveData))
			{
				result = false;
			}
			else if (mySaveData.Active == 0)
			{
				result = false;
			}
			else
			{
				SpecialActivityConfig myActConfig = null;
				if (!this.SpecialActDict.TryGetValue(mySaveData.ActID, out myActConfig))
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

		
		public int SpecActGiveAward(GameClient client, int ActID)
		{
			SpecActInfoDB mySaveData = null;
			int result;
			if (!client.ClientData.SpecActInfoDict.TryGetValue(ActID, out mySaveData))
			{
				result = -2;
			}
			else if (mySaveData.Active == 0)
			{
				result = -2;
			}
			else
			{
				SpecialActivityConfig myActConfig = null;
				if (!this.SpecialActDict.TryGetValue(mySaveData.ActID, out myActConfig))
				{
					result = -2;
				}
				else
				{
					string castResList = "";
					if (myActConfig.Type == 2)
					{
						if (!this.SubSpecActJiFen(client, myActConfig))
						{
							return -24;
						}
						int SpecJiFen = this.GetCurrentSpecActJiFen(client, myActConfig);
						castResList = EventLogManager.NewResPropString(ResLogType.SpecJiFen, new object[]
						{
							-myActConfig.Price.NumOne,
							SpecJiFen + myActConfig.Price.NumOne,
							SpecJiFen
						});
					}
					if (myActConfig.Type == 1 && myActConfig.Price.NumOne > 0)
					{
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myActConfig.Price.NumOne, "专属活动抢购", true, true, false, DaiBiSySType.None))
						{
							return -10;
						}
						castResList = EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
						{
							-myActConfig.Price.NumOne,
							client.ClientData.UserMoney + myActConfig.Price.NumOne,
							client.ClientData.UserMoney
						});
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
					this.UpdateClientSpecActData(client, mySaveData);
					if (client._IconStateMgr.CheckSpecialActivity(client))
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
					string strResList = EventLogManager.MakeGoodsDataPropString(myAwardItem.GoodsDataList);
					EventLogManager.AddPurchaseEvent(client, 7, mySaveData.ActID, castResList, strResList);
					result = 0;
				}
			}
			return result;
		}

		
		public string BuildFetchSpecActAwardCmd(GameClient client, int ErrCode, int actID)
		{
			int roleID = client.ClientData.RoleID;
			SpecActInfoDB mySaveData = null;
			string result;
			if (!client.ClientData.SpecActInfoDict.TryGetValue(actID, out mySaveData))
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
				SpecialActivityConfig myActConfig = null;
				if (!this.SpecialActDict.TryGetValue(mySaveData.ActID, out myActConfig))
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
						mySaveData.PurNum = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, myActConfig.Price.ZhiGouID);
					}
					int LeftPurNum = myActConfig.PurchaseNum - mySaveData.PurNum;
					SpecActGoalData CurGoalNum = this.GetCurrentGoalNum(client, mySaveData, myActConfig);
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

		
		private int GenerateSpecActGroupID()
		{
			DateTime kaifuTm = Global.GetKaiFuTime();
			DateTime nowDateTm = TimeUtil.NowDateTime();
			foreach (KeyValuePair<int, SpecialActivityTimeConfig> kvp in this.SpecialActTimeDict)
			{
				SpecialActivityTimeConfig data = kvp.Value;
				if (!(kaifuTm < data.ServerOpenFromDate) && !(kaifuTm > data.ServerOpenToDate))
				{
					if (!(nowDateTm < data.FromDate) && !(nowDateTm > data.ToDate))
					{
						return data.GroupID;
					}
				}
			}
			return -1;
		}

		
		private int GetCurrentSpecActJiFen(GameClient client, SpecialActivityConfig myActConfig)
		{
			SpecialActivityTimeConfig timeConfig = null;
			int result;
			if (!this.SpecialActTimeDict.TryGetValue(myActConfig.GroupID, out timeConfig))
			{
				result = 0;
			}
			else
			{
				string FromActDate = timeConfig.FromDate.ToString("yyyy-MM-dd HH:mm:ss");
				string ToActDate = timeConfig.ToDate.ToString("yyyy-MM-dd HH:mm:ss");
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, FromActDate.Replace(':', '$'), ToActDate.Replace(':', '$'));
				string[] fields = Global.ExecuteDBCmd(13162, strcmd, client.ServerId);
				if (fields == null || fields.Length < 2)
				{
					result = 0;
				}
				else
				{
					result = Global.SafeConvertToInt32(fields[1]);
				}
			}
			return result;
		}

		
		private bool SubSpecActJiFen(GameClient client, SpecialActivityConfig myActConfig)
		{
			SpecialActivityTimeConfig timeConfig = null;
			bool result;
			if (!this.SpecialActTimeDict.TryGetValue(myActConfig.GroupID, out timeConfig))
			{
				result = false;
			}
			else
			{
				string FromActDate = timeConfig.FromDate.ToString("yyyy-MM-dd HH:mm:ss");
				string ToActDate = timeConfig.ToDate.ToString("yyyy-MM-dd HH:mm:ss");
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					client.ClientData.RoleID,
					-myActConfig.Price.NumOne,
					FromActDate.Replace(':', '$'),
					ToActDate.Replace(':', '$')
				});
				string[] fields = Global.ExecuteDBCmd(13163, strcmd, client.ServerId);
				if (fields == null || fields.Length < 2)
				{
					result = false;
				}
				else
				{
					int retJiFen = Convert.ToInt32(fields[1]);
					result = (retJiFen >= 0);
				}
			}
			return result;
		}

		
		private HashSet<int> GetClientSpecActGroupIDSet(GameClient client)
		{
			HashSet<int> groupIDSet = new HashSet<int>();
			HashSet<int> result;
			if (client.ClientData.SpecActInfoDict == null || client.ClientData.SpecActInfoDict.Count == 0)
			{
				result = groupIDSet;
			}
			else
			{
				foreach (KeyValuePair<int, SpecActInfoDB> kvp in client.ClientData.SpecActInfoDict)
				{
					groupIDSet.Add(kvp.Value.GroupID);
				}
				result = groupIDSet;
			}
			return result;
		}

		
		private int GetClientSpecActGroupID(GameClient client)
		{
			int GroupID = -1;
			int result;
			if (client.ClientData.SpecActInfoDict == null || client.ClientData.SpecActInfoDict.Count == 0)
			{
				result = GroupID;
			}
			else
			{
				using (Dictionary<int, SpecActInfoDB>.Enumerator enumerator = client.ClientData.SpecActInfoDict.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<int, SpecActInfoDB> kvp = enumerator.Current;
						GroupID = kvp.Value.GroupID;
					}
				}
				result = GroupID;
			}
			return result;
		}

		
		private SpecActInfoDB CreatNewSpecAct(GameClient client, SpecialActivityConfig myActConfig)
		{
			SpecActInfoDB SpecActData = new SpecActInfoDB
			{
				GroupID = myActConfig.GroupID,
				ActID = myActConfig.ID
			};
			if (this.CheckNeedCondition(client, myActConfig))
			{
				SpecActData.Active = 1;
			}
			else
			{
				SpecActData.Active = 0;
			}
			return SpecActData;
		}

		
		private void GenerateSpecActGroup(GameClient client)
		{
			int GroupID = this.GenerateSpecActGroupID();
			DateTime nowDateTm = TimeUtil.NowDateTime();
			lock (client.ClientData)
			{
				if (null == client.ClientData.SpecActInfoDict)
				{
					client.ClientData.SpecActInfoDict = new Dictionary<int, SpecActInfoDB>();
				}
				HashSet<int> CurGroupIDSet = this.GetClientSpecActGroupIDSet(client);
				foreach (int id in CurGroupIDSet)
				{
					if (id != GroupID)
					{
						this.DeleteClientSpecActData(client, id);
					}
				}
				Dictionary<int, SpecActInfoDB> SpecActInfoForUpdate = new Dictionary<int, SpecActInfoDB>(client.ClientData.SpecActInfoDict);
				foreach (KeyValuePair<int, SpecActInfoDB> kvp in client.ClientData.SpecActInfoDict)
				{
					if (kvp.Value.GroupID != GroupID)
					{
						SpecActInfoForUpdate.Remove(kvp.Key);
					}
				}
				foreach (KeyValuePair<int, SpecialActivityConfig> kvp2 in this.SpecialActDict)
				{
					SpecialActivityConfig myActConfig = kvp2.Value;
					if (myActConfig.GroupID == GroupID)
					{
						if (myActConfig.Type != 14 || UserMoneyMgr.getInstance().PlatformOpenStateVavle != 0)
						{
							SpecActInfoDB SpecActData = null;
							if (SpecActInfoForUpdate.TryGetValue(myActConfig.ID, out SpecActData))
							{
								if (nowDateTm < myActConfig.FromDay || nowDateTm > myActConfig.ToDay)
								{
									SpecActData.Active = 0;
									this.UpdateClientSpecActData(client, SpecActData);
								}
							}
							else if (!(nowDateTm < myActConfig.FromDay) && !(nowDateTm > myActConfig.ToDay))
							{
								SpecActData = this.CreatNewSpecAct(client, myActConfig);
								SpecActInfoForUpdate[SpecActData.ActID] = SpecActData;
								this.UpdateClientSpecActData(client, SpecActData);
							}
						}
					}
				}
				client.ClientData.SpecActInfoDict = SpecActInfoForUpdate;
			}
		}

		
		private bool CheckFirstSecondCondition(int FirstValue, int SecondValue, SpecActLimitData Limit)
		{
			return FirstValue >= Limit.MinFirst && (FirstValue != Limit.MinFirst || SecondValue >= Limit.MinSecond) && FirstValue <= Limit.MaxFirst && (FirstValue != Limit.MaxFirst || SecondValue <= Limit.MaxSecond);
		}

		
		private bool CheckNeedCondition(GameClient client, SpecialActivityConfig myActConfig)
		{
			if (myActConfig.LevLimit.IfValid())
			{
				if (!this.CheckFirstSecondCondition(client.ClientData.ChangeLifeCount, client.ClientData.Level, myActConfig.LevLimit))
				{
					return false;
				}
			}
			if (myActConfig.VipLimit.IfValid())
			{
				if (client.ClientData.VipLevel < myActConfig.VipLimit.MinFirst || client.ClientData.VipLevel > myActConfig.VipLimit.MaxFirst)
				{
					return false;
				}
			}
			if (myActConfig.ChongZhiLimit.IfValid())
			{
				int totalChongZhiMoney = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
				int totalChongZhiYuanBao = Global.TransMoneyToYuanBao(totalChongZhiMoney);
				if (totalChongZhiYuanBao < myActConfig.ChongZhiLimit.MinFirst || totalChongZhiYuanBao > myActConfig.ChongZhiLimit.MaxFirst)
				{
					return false;
				}
			}
			if (myActConfig.WingLimit.IfValid())
			{
				if (client.ClientData.MyWingData == null)
				{
					return false;
				}
				if (!this.CheckFirstSecondCondition(client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.ForgeLevel, myActConfig.WingLimit))
				{
					return false;
				}
			}
			if (myActConfig.ChengJiuLimit.IfValid())
			{
				int ChengJiuLev = ChengJiuManager.GetChengJiuLevel(client);
				if (ChengJiuLev < myActConfig.ChengJiuLimit.MinFirst || ChengJiuLev > myActConfig.ChengJiuLimit.MaxFirst)
				{
					return false;
				}
			}
			if (myActConfig.JunXianLimit.IfValid())
			{
				int junxian = GameManager.ClientMgr.GetShengWangLevelValue(client);
				if (junxian < myActConfig.JunXianLimit.MinFirst || junxian > myActConfig.JunXianLimit.MaxFirst)
				{
					return false;
				}
			}
			if (myActConfig.MerlinLimit.IfValid())
			{
				if (client.ClientData.MerlinData == null)
				{
					return false;
				}
				if (!this.CheckFirstSecondCondition(client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, myActConfig.MerlinLimit))
				{
					return false;
				}
			}
			if (myActConfig.ShengWuLimit.IfValid())
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
				if (TotalLev < myActConfig.ShengWuLimit.MinFirst || TotalLev > myActConfig.ShengWuLimit.MaxFirst)
				{
					return false;
				}
			}
			if (myActConfig.RingLimit.IfValid())
			{
				if (client.ClientData.MyMarriageData == null)
				{
					return false;
				}
				if (!this.CheckFirstSecondCondition((int)client.ClientData.MyMarriageData.byGoodwilllevel, (int)client.ClientData.MyMarriageData.byGoodwillstar, myActConfig.RingLimit))
				{
					return false;
				}
			}
			if (myActConfig.ShouHuShenLimit.IfValid())
			{
				if (client.ClientData.MyGuardStatueDetail == null)
				{
					return false;
				}
				if (!this.CheckFirstSecondCondition(client.ClientData.MyGuardStatueDetail.GuardStatue.Suit, this.CalMyGuardStatueLevel(client), myActConfig.ShouHuShenLimit))
				{
					return false;
				}
			}
			return true;
		}

		
		private SpecActGoalData GetCurrentGoalNum(GameClient client, SpecActInfoDB mySaveData, SpecialActivityConfig myActConfig)
		{
			SpecActGoalData GoalNum = new SpecActGoalData();
			switch (myActConfig.Type)
			{
			case 1:
				GoalNum.NumOne = client.ClientData.UserMoney;
				break;
			case 2:
				GoalNum.NumOne = this.GetCurrentSpecActJiFen(client, myActConfig);
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

		
		private void DeleteClientSpecActData(GameClient client, int GroupID = 0)
		{
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, GroupID);
			Global.ExecuteDBCmd(13161, strcmd, client.ServerId);
		}

		
		private void UpdateClientSpecActData(GameClient client, SpecActInfoDB SpecActData)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				client.ClientData.RoleID,
				SpecActData.GroupID,
				SpecActData.ActID,
				SpecActData.PurNum,
				SpecActData.CountNum,
				SpecActData.Active
			});
			Global.ExecuteDBCmd(13160, strcmd, client.ServerId);
		}

		
		public void NotifyActivityState(GameClient client)
		{
			bool bNotifyOpen = false;
			if (client.ClientData.SpecActInfoDict != null && client.ClientData.SpecActInfoDict.Count != 0)
			{
				foreach (KeyValuePair<int, SpecActInfoDB> kvp in client.ClientData.SpecActInfoDict)
				{
					if (kvp.Value.Active == 1)
					{
						bNotifyOpen = true;
						break;
					}
				}
			}
			if (bNotifyOpen)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					4,
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
					4,
					0,
					"",
					0,
					0
				});
				client.sendCmd(770, strcmd, false);
			}
		}

		
		public void Dispose()
		{
			GlobalEventSource.getInstance().removeListener(36, this);
		}

		
		public bool Init()
		{
			try
			{
				if (!this.LoadSpecialActivityTimeData())
				{
					return false;
				}
				if (!this.LoadSpecialActivityData())
				{
					return false;
				}
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				this.ActivityType = 44;
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

		
		private bool ParseSpecActLimitData(SpecActLimitData LevLimit, string Value)
		{
			bool result;
			if (string.Compare(Value, "-1") == 0 || string.IsNullOrEmpty(Value))
			{
				result = true;
			}
			else
			{
				string[] Filed = Value.Split(new char[]
				{
					'|'
				});
				if (Filed.Length != 2)
				{
					result = false;
				}
				else
				{
					string[] LimitFirst = Filed[0].Split(new char[]
					{
						','
					});
					string[] LimitSecond = Filed[1].Split(new char[]
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

		
		private bool ParseSpecActDay(int groupID, string Day, SpecialActivityConfig myData)
		{
			SpecialActivityTimeConfig timeConfig = null;
			bool result;
			if (!this.SpecialActTimeDict.TryGetValue(groupID, out timeConfig))
			{
				result = false;
			}
			else if (string.Compare(Day, "-1") == 0 || string.IsNullOrEmpty(Day))
			{
				myData.FromDay = timeConfig.FromDate;
				myData.ToDay = timeConfig.ToDate;
				result = true;
			}
			else
			{
				string[] DayFiled = Day.Split(new char[]
				{
					','
				});
				if (DayFiled.Length == 2)
				{
					int SpanFromDay = Global.SafeConvertToInt32(DayFiled[0]) - 1;
					int SpanToDay = Global.SafeConvertToInt32(DayFiled[1]);
					myData.FromDay = Global.GetAddDaysDataTime(timeConfig.FromDate, SpanFromDay, true);
					myData.ToDay = Global.GetAddDaysDataTime(timeConfig.FromDate, SpanToDay, true);
				}
				else
				{
					int SpanFromDay = Global.SafeConvertToInt32(DayFiled[0]) - 1;
					myData.FromDay = Global.GetAddDaysDataTime(timeConfig.FromDate, SpanFromDay, true);
					myData.ToDay = new DateTime(myData.FromDay.Year, myData.FromDay.Month, myData.FromDay.Day, 23, 59, 59);
				}
				result = true;
			}
			return result;
		}

		
		public bool LoadSpecialActivityData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/SpecialActivity/SpecialActivity.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/SpecialActivity/SpecialActivity.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						SpecialActivityConfig myData = new SpecialActivityConfig();
						myData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myData.GroupID = (int)Global.GetSafeAttributeLong(xmlItem, "GroupID");
						string DayString = Global.GetSafeAttributeStr(xmlItem, "Day");
						if (!this.ParseSpecActDay(myData.GroupID, DayString, myData))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析专享活动文件Day失败 ID:{0},GroupID:{1}", myData.ID, myData.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(myData.LevLimit, Global.GetSafeAttributeStr(xmlItem, "NeedLevel")))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析专享活动文件NeedLevel失败 ID:{0},GroupID:{1}", myData.ID, myData.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(myData.VipLimit, Global.GetSafeAttributeStr(xmlItem, "NeedVIP")))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析专享活动文件NeedVIP失败 ID:{0},GroupID:{1}", myData.ID, myData.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(myData.ChongZhiLimit, Global.GetSafeAttributeStr(xmlItem, "NeedChongZhi")))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析专享活动文件NeedChongZhi失败 ID:{0},GroupID:{1}", myData.ID, myData.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(myData.WingLimit, Global.GetSafeAttributeStr(xmlItem, "NeedWing")))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析专享活动文件NeedWing失败 ID:{0},GroupID:{1}", myData.ID, myData.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(myData.ChengJiuLimit, Global.GetSafeAttributeStr(xmlItem, "NeedChengJiu")))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析专享活动文件NeedChengJiu失败 ID:{0},GroupID:{1}", myData.ID, myData.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(myData.JunXianLimit, Global.GetSafeAttributeStr(xmlItem, "NeedJunXian")))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析专享活动文件NeedJunXian失败 ID:{0},GroupID:{1}", myData.ID, myData.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(myData.MerlinLimit, Global.GetSafeAttributeStr(xmlItem, "NeedMerlin")))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析专享活动文件NeedMerlin失败 ID:{0},GroupID:{1}", myData.ID, myData.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(myData.ShengWuLimit, Global.GetSafeAttributeStr(xmlItem, "NeedShengWu")))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析专享活动文件NeedShengWu失败 ID:{0},GroupID:{1}", myData.ID, myData.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(myData.RingLimit, Global.GetSafeAttributeStr(xmlItem, "NeedRing")))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析专享活动文件NeedRing失败 ID:{0},GroupID:{1}", myData.ID, myData.GroupID), null, true);
						}
						else if (!this.ParseSpecActLimitData(myData.ShouHuShenLimit, Global.GetSafeAttributeStr(xmlItem, "NeedShouHuShen")))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析专享活动文件NeedShouHuShen失败 ID:{0},GroupID:{1}", myData.ID, myData.GroupID), null, true);
						}
						else
						{
							myData.Type = (int)Global.GetSafeAttributeLong(xmlItem, "Type");
							string GoalString = Global.GetSafeAttributeStr(xmlItem, "Goal");
							string[] GoalFiled = GoalString.Split(new char[]
							{
								','
							});
							if (GoalFiled.Length == 2)
							{
								myData.GoalData.NumOne = Global.SafeConvertToInt32(GoalFiled[0]);
								myData.GoalData.NumTwo = Global.SafeConvertToInt32(GoalFiled[1]);
							}
							else
							{
								myData.GoalData.NumOne = Global.SafeConvertToInt32(GoalFiled[0]);
							}
							string goodsIDsOne = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							string[] fields = goodsIDsOne.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型专享活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								myData.GoodsDataListOne = HuodongCachingMgr.ParseGoodsDataList(fields, "专享活动配置1");
							}
							string goodsIDsTwo = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (!string.IsNullOrEmpty(goodsIDsTwo))
							{
								fields = goodsIDsTwo.Split(new char[]
								{
									'|'
								});
								myData.GoodsDataListTwo = HuodongCachingMgr.ParseGoodsDataList(fields, "专享活动配置2");
							}
							string goodsIDsThr = Global.GetSafeAttributeStr(xmlItem, "GoodsThr");
							myData.GoodsDataListThr.Init(goodsIDsThr, Global.GetSafeAttributeStr(xmlItem, "EffectiveTime"), "专享活动配置3");
							string Price = Global.GetSafeAttributeStr(xmlItem, "Price");
							string[] PriceFiled = Price.Split(new char[]
							{
								'|'
							});
							if (PriceFiled.Length == 2)
							{
								myData.Price.NumOne = Global.SafeConvertToInt32(PriceFiled[0]);
								myData.Price.NumTwo = Global.SafeConvertToInt32(PriceFiled[1]);
							}
							else if (PriceFiled.Length == 3)
							{
								myData.Price.NumOne = Global.SafeConvertToInt32(PriceFiled[0]);
								myData.Price.NumTwo = Global.SafeConvertToInt32(PriceFiled[1]);
								myData.Price.ZhiGouID = Global.SafeConvertToInt32(PriceFiled[2]);
							}
							else
							{
								myData.Price.NumOne = Global.SafeConvertToInt32(PriceFiled[0]);
							}
							myData.PurchaseNum = (int)Global.GetSafeAttributeLong(xmlItem, "PurchaseNum");
							if (myData.Type == 14)
							{
								UserMoneyMgr.getInstance().CheckChargeItemConfigLogic(myData.Price.ZhiGouID, myData.PurchaseNum, goodsIDsOne, goodsIDsTwo, string.Format("专享活动 ID={0}", myData.ID));
							}
							this.SpecialActDict[myData.ID] = myData;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/SpecialActivity/SpecialActivity.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public bool LoadSpecialActivityTimeData()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/SpecialActivity/SpecialActivityTime.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/SpecialActivity/SpecialActivityTime.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						SpecialActivityTimeConfig myData = new SpecialActivityTimeConfig();
						myData.GroupID = (int)Global.GetSafeAttributeLong(xmlItem, "GroupID");
						string ServerOpenFromDate = Global.GetSafeAttributeStr(xmlItem, "ServerOpenFromDate");
						if (string.Compare(ServerOpenFromDate, "-1") != 0)
						{
							myData.ServerOpenFromDate = DateTime.Parse(ServerOpenFromDate);
						}
						else
						{
							myData.ServerOpenFromDate = Global.GetKaiFuTime();
						}
						string ServerOpenToDate = Global.GetSafeAttributeStr(xmlItem, "ServerOpenToDate");
						if (string.Compare(ServerOpenToDate, "-1") != 0)
						{
							myData.ServerOpenToDate = DateTime.Parse(ServerOpenToDate);
						}
						else
						{
							myData.ServerOpenToDate = DateTime.Parse("2028-08-08 08:08:08");
						}
						string FromDate = Global.GetSafeAttributeStr(xmlItem, "FromDate");
						if (!string.IsNullOrEmpty(FromDate))
						{
							myData.FromDate = DateTime.Parse(FromDate);
						}
						else
						{
							myData.FromDate = DateTime.Parse("2008-08-08 08:08:08");
						}
						string ToDate = Global.GetSafeAttributeStr(xmlItem, "ToDate");
						if (!string.IsNullOrEmpty(ToDate))
						{
							myData.ToDate = DateTime.Parse(ToDate);
						}
						else
						{
							myData.ToDate = DateTime.Parse("2028-08-08 08:08:08");
						}
						this.SpecialActTimeDict[myData.GroupID] = myData;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/SpecialActivity/SpecialActivityTime.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		protected const string SpecialChongZhiDuiHuan = "SpecialChongZhiDuiHuan";

		
		public const string SpecialActivityData_fileName = "Config/SpecialActivity/SpecialActivity.xml";

		
		public const string SpecialActivityTimeData_fileName = "Config/SpecialActivity/SpecialActivityTime.xml";

		
		protected Dictionary<int, SpecialActivityTimeConfig> SpecialActTimeDict = new Dictionary<int, SpecialActivityTimeConfig>();

		
		protected Dictionary<int, SpecialActivityConfig> SpecialActDict = new Dictionary<int, SpecialActivityConfig>();
	}
}
