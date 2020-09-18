using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.ActivityNew.SevenDay
{
	
	internal class SevenDayGoalAct
	{
		
		public SevenDayGoalAct()
		{
			this.evHandlerDict = new Dictionary<ESevenDayGoalFuncType, Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>>();
			this.evHandlerDict[ESevenDayGoalFuncType.RoleLevelUp] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_RoleLevelUp);
			this.evHandlerDict[ESevenDayGoalFuncType.SkillLevelUp] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_SkillLevelUp);
			this.evHandlerDict[ESevenDayGoalFuncType.MoJingCntInBag] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_MoJingCntInBag);
			this.evHandlerDict[ESevenDayGoalFuncType.RecoverMoJing] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_RecoverMoJing);
			this.evHandlerDict[ESevenDayGoalFuncType.ExchangeJinHuaJingShiByMoJing] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_ExchangeJinHuaJingShiByMoJing);
			this.evHandlerDict[ESevenDayGoalFuncType.JoinJingJiChangTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_JoinJingJiChangTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.WinJingJiChangTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_WinJingJiChangTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.JingJiChangRank] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_JingJiChangRank);
			this.evHandlerDict[ESevenDayGoalFuncType.PeiDaiBlueUp] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_PeiDaiBlueUp);
			this.evHandlerDict[ESevenDayGoalFuncType.PeiDaiPurpleUp] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_PeiDaiPurpleUp);
			this.evHandlerDict[ESevenDayGoalFuncType.RecoverEquipBlueUp] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_RecoverEquipBlueUp);
			this.evHandlerDict[ESevenDayGoalFuncType.MallInSaleCount] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_MallInSaleCount);
			this.evHandlerDict[ESevenDayGoalFuncType.GetEquipCountByQiFu] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_GetEquipCountByQiFu);
			this.evHandlerDict[ESevenDayGoalFuncType.PickUpEquipCount] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_PickUpEquipCount);
			this.evHandlerDict[ESevenDayGoalFuncType.EquipChuanChengTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_EquipChuanChengTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.EnterFuBenTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_EnterFuBenTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.KillMonsterInMap] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_KillMonsterInMap);
			this.evHandlerDict[ESevenDayGoalFuncType.JoinActivityTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_JoinActivityTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.HeChengTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_HeChengTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.UseGoodsCount] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_UseGoodsCount);
			this.evHandlerDict[ESevenDayGoalFuncType.JinBiZhuanHuanTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_JinBiZhuanHuanTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.BangZuanZhuanHuanTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_BangZuanZhuanHuanTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.ZuanShiZhuanHuanTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_ZuanShiZhuanHuanTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.ExchangeJinHuaJingShiByQiFuScore] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_ExchangeJinHuaJingShiByQiFuScore);
			this.evHandlerDict[ESevenDayGoalFuncType.CombatChange] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_CombatChange);
			this.evHandlerDict[ESevenDayGoalFuncType.PeiDaiForgeEquip] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_PeiDaiForgeEquip);
			this.evHandlerDict[ESevenDayGoalFuncType.ForgeEquipLevel] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_ForgeEquipLevel);
			this.evHandlerDict[ESevenDayGoalFuncType.ForgeEquipTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_ForgeEquipTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.CompleteChengJiu] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_CompleteChengJiu);
			this.evHandlerDict[ESevenDayGoalFuncType.ChengJiuLevel] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_ChengJiuLevel);
			this.evHandlerDict[ESevenDayGoalFuncType.JunXianLevel] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_JunXianLevel);
			this.evHandlerDict[ESevenDayGoalFuncType.PeiDaiAppendEquip] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_PeiDaiAppendEquip);
			this.evHandlerDict[ESevenDayGoalFuncType.AppendEquipLevel] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_AppendEquipLevel);
			this.evHandlerDict[ESevenDayGoalFuncType.AppendEquipTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_AppendEquipTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.ActiveXingZuo] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_ActiveXingZuo);
			this.evHandlerDict[ESevenDayGoalFuncType.GetSpriteCountBuleUp] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_GetSpriteCountBuleUp);
			this.evHandlerDict[ESevenDayGoalFuncType.GetSpriteCountPurpleUp] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_GetSpriteCountPurpleUp);
			this.evHandlerDict[ESevenDayGoalFuncType.WingLevel] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_WingLevel);
			this.evHandlerDict[ESevenDayGoalFuncType.WingSuitStarTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_WingSuitStarTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.CompleteTuJian] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_CompleteTuJian);
			this.evHandlerDict[ESevenDayGoalFuncType.PeiDaiSuitEquipCount] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_PeiDaiSuitEquipCount);
			this.evHandlerDict[ESevenDayGoalFuncType.PeiDaiSuitEquipLevel] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_PeiDaiSuitEquipLevel);
			this.evHandlerDict[ESevenDayGoalFuncType.EquipSuitUpTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_EquipSuitUpTimes);
		}

		
		public void LoadConfig()
		{
			Dictionary<ESevenDayGoalFuncType, List<int>> tmpFunc2GoalId = new Dictionary<ESevenDayGoalFuncType, List<int>>();
			Dictionary<int, SevenDayGoalAct._GoalItemConfig> tmpItemConfigDict = new Dictionary<int, SevenDayGoalAct._GoalItemConfig>();
			try
			{
				XElement xml = XElement.Load(Global.GameResPath("Config/SevenDay/SevenDayGoal.xml"));
				foreach (XElement xmlItem in xml.Elements())
				{
					SevenDayGoalAct._GoalItemConfig itemConfig = new SevenDayGoalAct._GoalItemConfig();
					itemConfig.Id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					itemConfig.Day = (int)Global.GetSafeAttributeLong(xmlItem, "Day");
					itemConfig.FuncType = (int)Global.GetSafeAttributeLong(xmlItem, "FunctionType");
					string[] szCond = Global.GetSafeAttributeStr(xmlItem, "TypeGoal").Split(new char[]
					{
						','
					});
					itemConfig.ExtCond1 = ((szCond.Length >= 1) ? Convert.ToInt32(szCond[0]) : 0);
					itemConfig.ExtCond2 = ((szCond.Length >= 2) ? Convert.ToInt32(szCond[1]) : 0);
					itemConfig.ExtCond3 = ((szCond.Length >= 3) ? Convert.ToInt32(szCond[2]) : 0);
					string[] szGoods = Global.GetSafeAttributeStr(xmlItem, "Award").Split(new char[]
					{
						'|'
					});
					itemConfig.GoodsList = HuodongCachingMgr.ParseGoodsDataList(szGoods, "七日目标");
					if (!tmpFunc2GoalId.ContainsKey((ESevenDayGoalFuncType)itemConfig.FuncType))
					{
						tmpFunc2GoalId[(ESevenDayGoalFuncType)itemConfig.FuncType] = new List<int>();
					}
					tmpFunc2GoalId[(ESevenDayGoalFuncType)itemConfig.FuncType].Add(itemConfig.Id);
					tmpItemConfigDict.Add(itemConfig.Id, itemConfig);
				}
				lock (this.ConfigMutex)
				{
					this.Func2GoalId = tmpFunc2GoalId;
					this.ItemConfigDict = tmpItemConfigDict;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("七日登录活动加载配置失败{0}", "Config/SevenDay/SevenDayGoal.xml"), ex, true);
			}
		}

		
		private int GetColor__(int ExcellencePropNum)
		{
			int color;
			if (ExcellencePropNum == 0)
			{
				color = 0;
			}
			else if (ExcellencePropNum >= 1 && ExcellencePropNum <= 2)
			{
				color = 1;
			}
			else if (ExcellencePropNum >= 3 && ExcellencePropNum <= 4)
			{
				color = 2;
			}
			else if (ExcellencePropNum >= 5 && ExcellencePropNum <= 6)
			{
				color = 3;
			}
			else
			{
				color = 4;
			}
			return color;
		}

		
		public bool HasAnyAwardCanGet(GameClient client, out bool[] bGoalDay)
		{
			bGoalDay = new bool[7];
			for (int i = 0; i < bGoalDay.Length; i++)
			{
				bGoalDay[i] = false;
			}
			bool result;
			if (client == null)
			{
				result = false;
			}
			else
			{
				bool bResult = false;
				int currDay;
				if (!SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client, out currDay))
				{
					result = false;
				}
				else
				{
					Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Goal);
					if (itemDict == null || itemDict.Count <= 0)
					{
						result = false;
					}
					else
					{
						Dictionary<ESevenDayGoalFuncType, List<int>> tmpFunc2GoalId = null;
						Dictionary<int, SevenDayGoalAct._GoalItemConfig> tmpItemConfigDict = null;
						lock (this.ConfigMutex)
						{
							tmpFunc2GoalId = this.Func2GoalId;
							tmpItemConfigDict = this.ItemConfigDict;
						}
						if (tmpFunc2GoalId == null || tmpItemConfigDict == null)
						{
							result = false;
						}
						else
						{
							lock (itemDict)
							{
								foreach (KeyValuePair<int, SevenDayItemData> kvp in itemDict)
								{
									SevenDayGoalAct._GoalItemConfig itemConfig = null;
									if (tmpItemConfigDict.TryGetValue(kvp.Key, out itemConfig))
									{
										if (itemConfig.Day <= currDay)
										{
											if (itemConfig.Day > 0 || itemConfig.Day <= bGoalDay.Length)
											{
												if (!bGoalDay[itemConfig.Day - 1])
												{
													if (this.CheckCanGetAward(client, kvp.Value, itemConfig))
													{
														bGoalDay[itemConfig.Day - 1] = true;
														bResult = true;
													}
												}
											}
										}
									}
								}
							}
							result = bResult;
						}
					}
				}
			}
			return result;
		}

		
		public ESevenDayActErrorCode HandleGetAward(GameClient client, int id)
		{
			int dayIdx;
			ESevenDayActErrorCode result;
			if (!SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client, out dayIdx))
			{
				result = ESevenDayActErrorCode.NotInActivityTime;
			}
			else
			{
				Dictionary<ESevenDayGoalFuncType, List<int>> tmpFunc2GoalId = null;
				Dictionary<int, SevenDayGoalAct._GoalItemConfig> tmpItemConfigDict = null;
				lock (this.ConfigMutex)
				{
					tmpFunc2GoalId = this.Func2GoalId;
					tmpItemConfigDict = this.ItemConfigDict;
				}
				if (tmpFunc2GoalId == null || tmpItemConfigDict == null)
				{
					result = ESevenDayActErrorCode.ServerConfigError;
				}
				else
				{
					SevenDayGoalAct._GoalItemConfig itemConfig = null;
					if (!tmpItemConfigDict.TryGetValue(id, out itemConfig) || itemConfig.GoodsList == null)
					{
						result = ESevenDayActErrorCode.ServerConfigError;
					}
					else if (itemConfig.Day > dayIdx)
					{
						result = ESevenDayActErrorCode.NotReachCondition;
					}
					else
					{
						Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Goal);
						if (itemDict == null)
						{
							result = ESevenDayActErrorCode.NotReachCondition;
						}
						else
						{
							lock (itemDict)
							{
								SevenDayItemData itemData = null;
								if (!itemDict.TryGetValue(id, out itemData))
								{
									return ESevenDayActErrorCode.NotReachCondition;
								}
								if (!this.CheckCanGetAward(client, itemData, itemConfig))
								{
									return ESevenDayActErrorCode.NotReachCondition;
								}
								if (!Global.CanAddGoodsNum(client, itemConfig.GoodsList.Count))
								{
									return ESevenDayActErrorCode.NoBagSpace;
								}
								itemData.AwardFlag = 1;
								if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(client.ClientData.RoleID, ESevenDayActType.Goal, id, itemData, client.ServerId))
								{
									itemData.AwardFlag = 0;
									return ESevenDayActErrorCode.DBFailed;
								}
								if (!SingletonTemplate<SevenDayActivityMgr>.Instance().GiveAward(client, new AwardItem
								{
									GoodsDataList = itemConfig.GoodsList
								}, ESevenDayActType.Goal))
								{
								}
							}
							result = ESevenDayActErrorCode.Success;
						}
					}
				}
			}
			return result;
		}

		
		private bool CheckCanGetAward(GameClient client, SevenDayItemData data, SevenDayGoalAct._GoalItemConfig itemConfig)
		{
			bool result;
			if (data == null || itemConfig == null || client == null)
			{
				result = false;
			}
			else if (data.AwardFlag != 0)
			{
				result = false;
			}
			else
			{
				switch (itemConfig.FuncType)
				{
				case 1:
				case 39:
					return data.Params1 > itemConfig.ExtCond1 || (data.Params1 == itemConfig.ExtCond1 && data.Params2 >= itemConfig.ExtCond2);
				case 2:
				case 3:
				case 9:
				case 10:
				case 26:
				case 31:
				case 32:
				case 37:
				case 38:
				case 43:
					return data.Params1 >= itemConfig.ExtCond1;
				case 4:
				case 6:
				case 7:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 22:
				case 23:
				case 24:
				case 28:
				case 29:
				case 34:
				case 35:
				case 40:
				case 44:
					return data.Params1 >= itemConfig.ExtCond1;
				case 5:
				case 17:
				case 19:
				case 20:
				case 21:
				case 25:
					return data.Params1 >= itemConfig.ExtCond2;
				case 8:
					return data.Params1 >= 1 && data.Params1 <= itemConfig.ExtCond1;
				case 18:
					return data.Params1 >= itemConfig.ExtCond3;
				case 27:
				case 33:
				case 36:
				case 42:
					return data.Params1 >= itemConfig.ExtCond2;
				case 30:
				case 41:
					return data.Params1 == 1;
				}
				result = false;
			}
			return result;
		}

		
		public void Update(GameClient client)
		{
			if (SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client))
			{
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.RoleLevelUp));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.SkillLevelUp));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.CombatChange));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.CompleteChengJiu));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.ChengJiuLevel));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.JunXianLevel));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.CompleteTuJian));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.WingLevel));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.ActiveXingZuo));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.GetSpriteCountBuleUp));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.GetSpriteCountPurpleUp));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PeiDaiAppendEquip));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PeiDaiForgeEquip));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PeiDaiBlueUp));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PeiDaiPurpleUp));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PeiDaiSuitEquipLevel));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PeiDaiSuitEquipCount));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.MallInSaleCount));
			}
		}

		
		public void HandleEvent(SevenDayGoalEventObject evObj)
		{
			if (evObj != null)
			{
				Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>> handler = null;
				if (this.evHandlerDict.TryGetValue(evObj.FuncType, out handler))
				{
					List<int> tmpGoalIdList = null;
					Dictionary<int, SevenDayGoalAct._GoalItemConfig> tmpGoalConfigDict = null;
					lock (this.ConfigMutex)
					{
						if (!this.Func2GoalId.TryGetValue(evObj.FuncType, out tmpGoalIdList) || tmpGoalIdList.Count <= 0)
						{
							return;
						}
						if ((tmpGoalConfigDict = this.ItemConfigDict) == null || tmpGoalConfigDict.Count <= 0)
						{
							return;
						}
					}
					handler(evObj, tmpGoalIdList, tmpGoalConfigDict);
				}
			}
		}

		
		private void _Handle_RoleLevelUp(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						itemData.Params1 = evObj.Client.ClientData.ChangeLifeCount;
						itemData.Params2 = evObj.Client.ClientData.Level;
					}
				}
			}
		}

		
		private void _Handle_SkillLevelUp(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Client.ClientData.SkillDataList != null)
				{
					Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (itemDict)
					{
						foreach (int goalId in goalIdList)
						{
							SevenDayItemData itemData = null;
							if (!itemDict.TryGetValue(goalId, out itemData))
							{
								itemData = new SevenDayItemData();
								itemData.AwardFlag = 0;
								itemDict[goalId] = itemData;
							}
							itemData.Params1 = 0;
						}
						bool bHadAddDefault = false;
						for (int i = 0; i < evObj.Client.ClientData.SkillDataList.Count; i++)
						{
							if (Global.GetPrevSkilID(evObj.Client.ClientData.SkillDataList[i].SkillID) <= 0)
							{
								if (evObj.Client.ClientData.SkillDataList[i].DbID == -1)
								{
									if (bHadAddDefault)
									{
										goto IL_1F7;
									}
									bHadAddDefault = true;
								}
								int skillLevel = evObj.Client.ClientData.SkillDataList[i].SkillLevel;
								foreach (int goalId in goalIdList)
								{
									SevenDayGoalAct._GoalItemConfig itemConfig = null;
									if (goalConfigDict.TryGetValue(goalId, out itemConfig))
									{
										if (skillLevel >= itemConfig.ExtCond2)
										{
											SevenDayItemData itemData = null;
											if (!itemDict.TryGetValue(goalId, out itemData))
											{
												itemData = new SevenDayItemData();
												itemData.Params1 = 0;
												itemData.AwardFlag = 0;
												itemDict[goalId] = itemData;
											}
											itemData.Params1++;
										}
									}
								}
							}
							IL_1F7:;
						}
					}
				}
			}
		}

		
		private void _Handle_MoJingCntInBag(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				int value = GameManager.ClientMgr.GetTianDiJingYuanValue(evObj.Client);
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							if (value > itemData.Params1)
							{
								int oldValue = itemData.Params1;
								itemData.Params1 = value;
								if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
								{
									itemData.Params1 = oldValue;
								}
							}
						}
					}
				}
			}
		}

		
		private void _Handle_RecoverMoJing(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							itemData.Params1 += evObj.Arg1;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
							{
								itemData.Params1 -= evObj.Arg1;
							}
						}
					}
				}
			}
		}

		
		private void _Handle_ExchangeJinHuaJingShiByMoJing(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							SevenDayGoalAct._GoalItemConfig itemConfig = null;
							if (goalConfigDict.TryGetValue(goalId, out itemConfig))
							{
								if (itemConfig.ExtCond1 == evObj.Arg1)
								{
									itemData.Params1++;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
									{
										itemData.Params1--;
									}
								}
							}
						}
					}
				}
			}
		}

		
		private void _Handle_JoinJingJiChangTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							itemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
							{
								itemData.Params1--;
							}
						}
					}
				}
			}
		}

		
		private void _Handle_WinJingJiChangTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							itemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
							{
								itemData.Params1--;
							}
						}
					}
				}
			}
		}

		
		private void _Handle_JingJiChangRank(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Arg1 >= 1)
				{
					Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (itemDict)
					{
						foreach (int goalId in goalIdList)
						{
							SevenDayItemData itemData = null;
							if (!itemDict.TryGetValue(goalId, out itemData))
							{
								itemData = new SevenDayItemData();
								itemData.Params1 = -1;
								itemData.AwardFlag = 0;
								itemDict[goalId] = itemData;
							}
							if (itemData.AwardFlag != 1)
							{
								if (itemData.Params1 < 1 || itemData.Params1 > evObj.Arg1)
								{
									int oldRank = itemData.Params1;
									itemData.Params1 = evObj.Arg1;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
									{
										itemData.Params1 = oldRank;
									}
								}
							}
						}
					}
				}
			}
		}

		
		private void _Handle_PeiDaiBlueUp(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				List<int> exceList = evObj.Client.UsingEquipMgr.GetUsingEquipExcellencePropNum();
				int equipCnt = (exceList != null) ? exceList.Count((int _e) => this.GetColor__(_e) >= 2) : 0;
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						SevenDayGoalAct._GoalItemConfig itemConfig = null;
						if (goalConfigDict.TryGetValue(goalId, out itemConfig))
						{
							itemData.Params1 = equipCnt;
						}
					}
				}
			}
		}

		
		private void _Handle_PeiDaiPurpleUp(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				List<int> exceList = evObj.Client.UsingEquipMgr.GetUsingEquipExcellencePropNum();
				int equipCnt = (exceList != null) ? exceList.Count((int _e) => this.GetColor__(_e) >= 3) : 0;
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						SevenDayGoalAct._GoalItemConfig itemConfig = null;
						if (goalConfigDict.TryGetValue(goalId, out itemConfig))
						{
							itemData.Params1 = equipCnt;
						}
					}
				}
			}
		}

		
		private void _Handle_RecoverEquipBlueUp(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				int categoriy = Global.GetGoodsCatetoriy(evObj.Arg1);
				bool isEquip = false;
				if (categoriy >= 0 && categoriy <= 6)
				{
					isEquip = true;
				}
				else if (categoriy >= 11 && categoriy < 49)
				{
					isEquip = true;
				}
				if (isEquip)
				{
					if (this.GetColor__(evObj.Arg3) >= 2)
					{
						Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
						lock (itemDict)
						{
							foreach (int goalId in goalIdList)
							{
								SevenDayItemData itemData = null;
								if (!itemDict.TryGetValue(goalId, out itemData))
								{
									itemData = new SevenDayItemData();
									itemData.Params1 = 0;
									itemData.AwardFlag = 0;
									itemDict[goalId] = itemData;
								}
								if (itemData.AwardFlag != 1)
								{
									itemData.Params1 += evObj.Arg2;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
									{
										itemData.Params1 -= evObj.Arg2;
									}
								}
							}
						}
					}
				}
			}
		}

		
		private void _Handle_MallInSaleCount(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Client.ClientData.SaleGoodsDataList != null)
				{
					int equipCount = 0;
					lock (evObj.Client.ClientData.SaleGoodsDataList)
					{
						foreach (GoodsData gd in evObj.Client.ClientData.SaleGoodsDataList)
						{
							if (gd != null)
							{
								int categoriy = Global.GetGoodsCatetoriy(gd.GoodsID);
								if (categoriy >= 0 && categoriy <= 6)
								{
									equipCount++;
								}
								else if (categoriy >= 11 && categoriy < 49)
								{
									equipCount++;
								}
							}
						}
					}
					Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (itemDict)
					{
						foreach (int goalId in goalIdList)
						{
							SevenDayItemData itemData = null;
							if (!itemDict.TryGetValue(goalId, out itemData))
							{
								itemData = new SevenDayItemData();
								itemData.Params1 = 0;
								itemData.AwardFlag = 0;
								itemDict[goalId] = itemData;
							}
							itemData.Params1 = equipCount;
						}
					}
				}
			}
		}

		
		private void _Handle_GetEquipCountByQiFu(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				int categoriy = Global.GetGoodsCatetoriy(evObj.Arg1);
				if (categoriy < 0 || categoriy > 6)
				{
					if (categoriy < 11 || categoriy >= 49)
					{
						return;
					}
				}
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							itemData.Params1 += evObj.Arg2;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
							{
								itemData.Params1 -= evObj.Arg2;
							}
						}
					}
				}
			}
		}

		
		private void _Handle_PickUpEquipCount(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				int categoriy = Global.GetGoodsCatetoriy(evObj.Arg1);
				if (categoriy < 0 || categoriy > 6)
				{
					if (categoriy < 11 || categoriy >= 49)
					{
						return;
					}
				}
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							itemData.Params1 += evObj.Arg2;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
							{
								itemData.Params1 -= evObj.Arg2;
							}
						}
					}
				}
			}
		}

		
		private void _Handle_EquipChuanChengTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							itemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
							{
								itemData.Params1--;
							}
						}
					}
				}
			}
		}

		
		private void _Handle_EnterFuBenTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Arg2 > 0)
				{
					Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (itemDict)
					{
						foreach (int goalId in goalIdList)
						{
							SevenDayItemData itemData = null;
							if (!itemDict.TryGetValue(goalId, out itemData))
							{
								itemData = new SevenDayItemData();
								itemData.Params1 = 0;
								itemData.AwardFlag = 0;
								itemDict[goalId] = itemData;
							}
							if (itemData.AwardFlag != 1)
							{
								SevenDayGoalAct._GoalItemConfig itemConfig = null;
								if (goalConfigDict.TryGetValue(goalId, out itemConfig))
								{
									if (itemConfig.ExtCond1 == evObj.Arg1)
									{
										itemData.Params1 += evObj.Arg2;
										if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
										{
											itemData.Params1 -= evObj.Arg2;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		
		private void _Handle_KillMonsterInMap(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							SevenDayGoalAct._GoalItemConfig itemConfig = null;
							if (goalConfigDict.TryGetValue(goalId, out itemConfig))
							{
								if (itemConfig.ExtCond1 == evObj.Arg1 && itemConfig.ExtCond2 == evObj.Arg2)
								{
									itemData.Params1++;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
									{
										itemData.Params1--;
									}
								}
							}
						}
					}
				}
			}
		}

		
		private void _Handle_JoinActivityTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							SevenDayGoalAct._GoalItemConfig itemConfig = null;
							if (goalConfigDict.TryGetValue(goalId, out itemConfig))
							{
								if (itemConfig.ExtCond1 == evObj.Arg1)
								{
									itemData.Params1++;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
									{
										itemData.Params1--;
									}
								}
							}
						}
					}
				}
			}
		}

		
		private void _Handle_HeChengTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							SevenDayGoalAct._GoalItemConfig itemConfig = null;
							if (goalConfigDict.TryGetValue(goalId, out itemConfig))
							{
								if (itemConfig.ExtCond1 == evObj.Arg1)
								{
									itemData.Params1++;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
									{
										itemData.Params1--;
									}
								}
							}
						}
					}
				}
			}
		}

		
		private void _Handle_UseGoodsCount(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Arg2 > 0)
				{
					Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (itemDict)
					{
						foreach (int goalId in goalIdList)
						{
							SevenDayItemData itemData = null;
							if (!itemDict.TryGetValue(goalId, out itemData))
							{
								itemData = new SevenDayItemData();
								itemData.Params1 = 0;
								itemData.AwardFlag = 0;
								itemDict[goalId] = itemData;
							}
							if (itemData.AwardFlag != 1)
							{
								SevenDayGoalAct._GoalItemConfig itemConfig = null;
								if (goalConfigDict.TryGetValue(goalId, out itemConfig))
								{
									if (itemConfig.ExtCond1 == evObj.Arg1)
									{
										itemData.Params1 += evObj.Arg2;
										if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
										{
											itemData.Params1 -= evObj.Arg2;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		
		private void _Handle_JinBiZhuanHuanTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							itemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
							{
								itemData.Params1--;
							}
						}
					}
				}
			}
		}

		
		private void _Handle_BangZuanZhuanHuanTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							itemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
							{
								itemData.Params1--;
							}
						}
					}
				}
			}
		}

		
		private void _Handle_ZuanShiZhuanHuanTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							itemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
							{
								itemData.Params1--;
							}
						}
					}
				}
			}
		}

		
		private void _Handle_ExchangeJinHuaJingShiByQiFuScore(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							SevenDayGoalAct._GoalItemConfig itemConfig = null;
							if (goalConfigDict.TryGetValue(goalId, out itemConfig))
							{
								if (itemConfig.ExtCond1 == evObj.Arg1)
								{
									itemData.Params1++;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
									{
										itemData.Params1--;
									}
								}
							}
						}
					}
				}
			}
		}

		
		private void _Handle_CombatChange(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						itemData.Params1 = evObj.Client.ClientData.CombatForce;
					}
				}
			}
		}

		
		private void _Handle_PeiDaiForgeEquip(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				List<int> forgeList = evObj.Client.UsingEquipMgr.GetUsingEquipForge();
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						SevenDayGoalAct._GoalItemConfig itemConfig = null;
						if (goalConfigDict.TryGetValue(goalId, out itemConfig))
						{
							itemData.Params1 = ((forgeList != null) ? forgeList.Count((int _forge) => _forge >= itemConfig.ExtCond1) : 0);
						}
					}
				}
			}
		}

		
		private void _Handle_ForgeEquipLevel(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							if (evObj.Arg1 > itemData.Params1)
							{
								int oldValue = itemData.Params1;
								itemData.Params1 = evObj.Arg1;
								if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
								{
									itemData.Params1 = oldValue;
								}
							}
						}
					}
				}
			}
		}

		
		private void _Handle_ForgeEquipTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							itemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
							{
								itemData.Params1--;
							}
						}
					}
				}
			}
		}

		
		private void _Handle_CompleteChengJiu(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						SevenDayGoalAct._GoalItemConfig itemConfig = null;
						if (goalConfigDict.TryGetValue(goalId, out itemConfig))
						{
							if (ChengJiuManager.IsChengJiuCompleted(evObj.Client, itemConfig.ExtCond1))
							{
								itemData.Params1 = 1;
							}
						}
					}
				}
			}
		}

		
		private void _Handle_ChengJiuLevel(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						itemData.Params1 = ChengJiuManager.GetChengJiuLevel(evObj.Client);
					}
				}
			}
		}

		
		private void _Handle_JunXianLevel(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						itemData.Params1 = GameManager.ClientMgr.GetShengWangLevelValue(evObj.Client);
					}
				}
			}
		}

		
		private void _Handle_PeiDaiAppendEquip(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				List<int> appendList = evObj.Client.UsingEquipMgr.GetUsingEquipAppend();
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						SevenDayGoalAct._GoalItemConfig itemConfig = null;
						if (goalConfigDict.TryGetValue(goalId, out itemConfig))
						{
							itemData.Params1 = ((appendList != null) ? appendList.Count((int _append) => _append >= itemConfig.ExtCond1) : 0);
						}
					}
				}
			}
		}

		
		private void _Handle_AppendEquipLevel(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Arg1 > 0)
				{
					Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (itemDict)
					{
						foreach (int goalId in goalIdList)
						{
							SevenDayItemData itemData = null;
							if (!itemDict.TryGetValue(goalId, out itemData))
							{
								itemData = new SevenDayItemData();
								itemData.Params1 = 0;
								itemData.AwardFlag = 0;
								itemDict[goalId] = itemData;
							}
							if (itemData.AwardFlag != 1)
							{
								if (evObj.Arg1 > itemData.Params1)
								{
									int oldLvl = itemData.Params1;
									itemData.Params1 = evObj.Arg1;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
									{
										itemData.Params1 = oldLvl;
									}
								}
							}
						}
					}
				}
			}
		}

		
		private void _Handle_AppendEquipTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							itemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
							{
								itemData.Params1--;
							}
						}
					}
				}
			}
		}

		
		private void _Handle_ActiveXingZuo(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Client.ClientData.RoleStarConstellationInfo != null)
				{
					Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (itemDict)
					{
						foreach (int goalId in goalIdList)
						{
							SevenDayItemData itemData = null;
							if (!itemDict.TryGetValue(goalId, out itemData))
							{
								itemData = new SevenDayItemData();
								itemData.Params1 = 0;
								itemData.AwardFlag = 0;
								itemDict[goalId] = itemData;
							}
							SevenDayGoalAct._GoalItemConfig itemConfig = null;
							if (goalConfigDict.TryGetValue(goalId, out itemConfig))
							{
								int star = 0;
								if (evObj.Client.ClientData.RoleStarConstellationInfo.TryGetValue(itemConfig.ExtCond1, out star))
								{
									itemData.Params1 = star;
								}
							}
						}
					}
				}
			}
		}

		
		private void _Handle_GetSpriteCountBuleUp(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				int spriteCount = 0;
				if (evObj.Client.ClientData.DamonGoodsDataList != null)
				{
					evObj.Client.ClientData.DamonGoodsDataList.ForEach(delegate(GoodsData _sprite)
					{
						if (_sprite.Site == 5000 && Global.GetGoodsColorEx(_sprite) >= 2)
						{
							spriteCount++;
						}
					});
				}
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							itemData.Params1 = spriteCount;
						}
					}
				}
			}
		}

		
		private void _Handle_GetSpriteCountPurpleUp(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				int spriteCount = 0;
				if (evObj.Client.ClientData.DamonGoodsDataList != null)
				{
					evObj.Client.ClientData.DamonGoodsDataList.ForEach(delegate(GoodsData _sprite)
					{
						if (_sprite.Site == 5000 && Global.GetGoodsColorEx(_sprite) >= 3)
						{
							spriteCount++;
						}
					});
				}
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							itemData.Params1 = spriteCount;
						}
					}
				}
			}
		}

		
		private void _Handle_WingLevel(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Client.ClientData.MyWingData != null)
				{
					Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (itemDict)
					{
						foreach (int goalId in goalIdList)
						{
							SevenDayItemData itemData = null;
							if (!itemDict.TryGetValue(goalId, out itemData))
							{
								itemData = new SevenDayItemData();
								itemData.AwardFlag = 0;
								itemDict[goalId] = itemData;
							}
							itemData.Params1 = evObj.Client.ClientData.MyWingData.WingID;
							itemData.Params2 = evObj.Client.ClientData.MyWingData.ForgeLevel;
						}
					}
				}
			}
		}

		
		private void _Handle_WingSuitStarTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Client.ClientData.MyWingData != null)
				{
					Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (itemDict)
					{
						foreach (int goalId in goalIdList)
						{
							SevenDayItemData itemData = null;
							if (!itemDict.TryGetValue(goalId, out itemData))
							{
								itemData = new SevenDayItemData();
								itemData.Params1 = 0;
								itemData.AwardFlag = 0;
								itemDict[goalId] = itemData;
							}
							else if (itemData.AwardFlag == 1)
							{
							}
							itemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
							{
								itemData.Params1--;
							}
						}
					}
				}
			}
		}

		
		private void _Handle_CompleteTuJian(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						SevenDayGoalAct._GoalItemConfig itemConfig = null;
						if (goalConfigDict.TryGetValue(goalId, out itemConfig))
						{
							if (evObj.Client.ClientData.ActivedTuJianItem != null && evObj.Client.ClientData.ActivedTuJianItem.Contains(itemConfig.ExtCond1))
							{
								itemData.Params1 = 1;
							}
							else
							{
								itemData.Params1 = 0;
							}
						}
					}
				}
			}
		}

		
		private void _Handle_PeiDaiSuitEquipCount(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				List<int> suitList = evObj.Client.UsingEquipMgr.GetUsingEquipSuit();
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						SevenDayGoalAct._GoalItemConfig itemConfig = null;
						if (goalConfigDict.TryGetValue(goalId, out itemConfig))
						{
							itemData.Params1 = ((suitList != null) ? suitList.Count((int _suit) => _suit >= itemConfig.ExtCond1) : 0);
						}
					}
				}
			}
		}

		
		private void _Handle_PeiDaiSuitEquipLevel(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				List<int> suitList = evObj.Client.UsingEquipMgr.GetUsingEquipSuit();
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						SevenDayGoalAct._GoalItemConfig itemConfig = null;
						if (goalConfigDict.TryGetValue(goalId, out itemConfig))
						{
							itemData.Params1 = ((suitList != null && suitList.Count > 0) ? suitList.Max() : 0);
						}
					}
				}
			}
		}

		
		private void _Handle_EquipSuitUpTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				List<int> suitList = evObj.Client.UsingEquipMgr.GetUsingEquipSuit();
				Dictionary<int, SevenDayItemData> itemDict = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (itemDict)
				{
					foreach (int goalId in goalIdList)
					{
						SevenDayItemData itemData = null;
						if (!itemDict.TryGetValue(goalId, out itemData))
						{
							itemData = new SevenDayItemData();
							itemData.Params1 = 0;
							itemData.AwardFlag = 0;
							itemDict[goalId] = itemData;
						}
						if (itemData.AwardFlag != 1)
						{
							itemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, goalId, itemData, evObj.Client.ServerId))
							{
								itemData.Params1--;
							}
						}
					}
				}
			}
		}

		
		private Dictionary<ESevenDayGoalFuncType, Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>> evHandlerDict = null;

		
		private object ConfigMutex = new object();

		
		private Dictionary<ESevenDayGoalFuncType, List<int>> Func2GoalId = null;

		
		private Dictionary<int, SevenDayGoalAct._GoalItemConfig> ItemConfigDict = null;

		
		private class _GoalItemConfig
		{
			
			public int Id;

			
			public int Day;

			
			public int FuncType;

			
			public List<GoodsData> GoodsList;

			
			public int ExtCond1;

			
			public int ExtCond2;

			
			public int ExtCond3;
		}
	}
}
