using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000236 RID: 566
	public class CaiJiLogic
	{
		// Token: 0x060007C7 RID: 1991 RVA: 0x00075FD0 File Offset: 0x000741D0
		public static bool LoadConfig()
		{
			CaiJiLogic.DailyNum = (int)GameManager.systemParamsList.GetParamValueIntByName("MuKuangNum", -1);
			CaiJiLogic.DeadReliveTime = (int)GameManager.systemParamsList.GetParamValueIntByName("CrystalDeadTime", -1);
			List<string> doubleAwardParams = GameManager.systemParamsList.GetParamValueStringListByName("MuKuangDoubleAward", '|');
			bool result;
			if (doubleAwardParams == null || doubleAwardParams.Count == 0)
			{
				result = false;
			}
			else
			{
				CaiJiLogic.dateTimeRangeArray = new CaiJiDateTimeRange[doubleAwardParams.Count];
				for (int loop = 0; loop < doubleAwardParams.Count; loop++)
				{
					string[] doubleAwardRange = doubleAwardParams[loop].Split(new char[]
					{
						','
					});
					if (doubleAwardRange.Length != 3)
					{
						return false;
					}
					CaiJiDateTimeRange DoubleAwardTimeRange = new CaiJiDateTimeRange();
					string startTime = doubleAwardRange[0];
					string[] temp = startTime.Split(new char[]
					{
						':'
					});
					DoubleAwardTimeRange.FromHour = int.Parse(temp[0]);
					DoubleAwardTimeRange.FromMinute = int.Parse(temp[1]);
					string endTime = doubleAwardRange[1];
					temp = endTime.Split(new char[]
					{
						':'
					});
					DoubleAwardTimeRange.EndHour = int.Parse(temp[0]);
					DoubleAwardTimeRange.EndMinute = int.Parse(temp[1]);
					DoubleAwardTimeRange.DoubleAwardRate = float.Parse(doubleAwardRange[2]);
					CaiJiLogic.dateTimeRangeArray[loop] = DoubleAwardTimeRange;
				}
				CaiJiLogic.GatherTimePer = (int)GameManager.systemParamsList.GetParamValueIntByName("GatherTimePer", 90);
				result = true;
			}
			return result;
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x00076150 File Offset: 0x00074350
		public static int JugeDateTimeInTimeRange(DateTime dateTime, DateTimeRange[] dateTimeRangeArray, bool equalEndTime = true)
		{
			int result;
			if (null == dateTimeRangeArray)
			{
				result = -1;
			}
			else
			{
				int hour = dateTime.Hour;
				int minute = dateTime.Minute;
				for (int i = 0; i < dateTimeRangeArray.Length; i++)
				{
					if (null != dateTimeRangeArray[i])
					{
						int time = dateTimeRangeArray[i].FromHour * 60 + dateTimeRangeArray[i].FromMinute;
						int time2 = dateTimeRangeArray[i].EndHour * 60 + dateTimeRangeArray[i].EndMinute;
						int time3 = hour * 60 + minute;
						if (!equalEndTime)
						{
							time2--;
						}
						if (time3 >= time && time3 <= time2)
						{
							return i;
						}
					}
				}
				result = -1;
			}
			return result;
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x00076214 File Offset: 0x00074414
		public static int ReqStartCaiJi(GameClient client, int monsterId, out int GatherTime)
		{
			GatherTime = 0;
			CaiJiLogic.CancelCaiJiState(client);
			int result;
			if (TimeUtil.NOW() < client.ClientData.CurrentMagicActionEndTicks)
			{
				result = -43;
			}
			else if (client.ClientData.CurrentLifeV <= 0)
			{
				CaiJiLogic.CancelCaiJiState(client);
				result = -3;
			}
			else
			{
				Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, monsterId);
				if (null == monster)
				{
					result = -1;
				}
				else if (monster.MonsterType != 1601)
				{
					result = -4;
				}
				else if (monster.IsCollected)
				{
					result = -4;
				}
				else
				{
					SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
					GetCaiJiTimeEventObject eventObj = new GetCaiJiTimeEventObject(client, monster);
					bool handled = GlobalEventSource4Scene.getInstance().fireEvent(eventObj, (int)sceneType);
					if (handled)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 600.0)
						{
							return -301;
						}
						GatherTime = eventObj.GatherTime;
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else if (sceneType == SceneUIClasses.HuanYingSiYuan)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 600.0)
						{
							return -301;
						}
						GatherTime = HuanYingSiYuanManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else if (sceneType == SceneUIClasses.YongZheZhanChang)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 600.0)
						{
							return -301;
						}
						GatherTime = YongZheZhanChangManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else if (sceneType == SceneUIClasses.KingOfBattle)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 600.0)
						{
							return -301;
						}
						GatherTime = KingOfBattleManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else if (sceneType == SceneUIClasses.Comp)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 600.0)
						{
							return -301;
						}
						GatherTime = CompManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else if (sceneType == SceneUIClasses.KarenEast)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 600.0)
						{
							return -301;
						}
						GatherTime = KarenBattleManager_MapEast.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else if (sceneType == SceneUIClasses.LingDiCaiJi)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 400.0)
						{
							return -301;
						}
						GatherTime = LingDiCaiJiManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else if (sceneType == SceneUIClasses.EscapeBattle)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 400.0)
						{
							return -301;
						}
						GatherTime = EscapeBattleManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 400.0)
						{
							return -301;
						}
						SystemXmlItem CaiJiMonsterXmlItem = null;
						if (!GameManager.systemCaiJiMonsterMgr.SystemXmlItemDict.TryGetValue(monster.MonsterInfo.ExtensionID, out CaiJiMonsterXmlItem) || null == CaiJiMonsterXmlItem)
						{
							return -4;
						}
						GatherTime = CaiJiMonsterXmlItem.GetIntValue("GatherTime", -1);
						if (client.ClientData.DailyCrystalCollectNum >= CaiJiLogic.DailyNum)
						{
							return -5;
						}
					}
					Global.EndMeditate(client);
					CaiJiLogic.SetCaiJiState(client, monsterId, 0L, monster.UniqueID);
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x00076718 File Offset: 0x00074918
		public static int ReqFinishCaiJi(GameClient client, int monsterId)
		{
			int result;
			if (monsterId != client.ClientData.CaijTargetId || client.ClientData.CaiJiStartTick == 0U || client.ClientData.CaijTargetId == 0)
			{
				CaiJiLogic.CancelCaiJiState(client);
				result = -3;
			}
			else if (client.ClientData.CurrentLifeV <= 0)
			{
				CaiJiLogic.CancelCaiJiState(client);
				result = -3;
			}
			else
			{
				Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, monsterId);
				if (null == monster)
				{
					CaiJiLogic.CancelCaiJiState(client);
					result = -1;
				}
				else if (monster.UniqueID != client.ClientData.CaiJiTargetUniqueID)
				{
					CaiJiLogic.CancelCaiJiState(client);
					result = -1;
				}
				else if (monster.MonsterType != 1601)
				{
					CaiJiLogic.CancelCaiJiState(client);
					result = -4;
				}
				else
				{
					SystemXmlItem CaiJiMonsterXmlItem = null;
					SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
					GetCaiJiTimeEventObject eventObj = new GetCaiJiTimeEventObject(client, monster);
					bool handled = GlobalEventSource4Scene.getInstance().fireEvent(eventObj, (int)sceneType);
					int GatherTime;
					if (handled)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 600.0)
						{
							return -301;
						}
						GatherTime = eventObj.GatherTime;
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else if (sceneType == SceneUIClasses.HuanYingSiYuan)
					{
						GatherTime = HuanYingSiYuanManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return -4;
						}
					}
					else if (sceneType == SceneUIClasses.YongZheZhanChang)
					{
						GatherTime = YongZheZhanChangManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return -4;
						}
					}
					else if (sceneType == SceneUIClasses.KingOfBattle)
					{
						GatherTime = KingOfBattleManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return -4;
						}
					}
					else if (sceneType == SceneUIClasses.Comp)
					{
						GatherTime = CompManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return -4;
						}
					}
					else if (sceneType == SceneUIClasses.KarenEast)
					{
						GatherTime = KarenBattleManager_MapEast.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return -4;
						}
					}
					else if (sceneType == SceneUIClasses.LingDiCaiJi)
					{
						GatherTime = LingDiCaiJiManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							CaiJiLogic.CancelCaiJiState(client);
							return GatherTime;
						}
					}
					else if (sceneType == SceneUIClasses.EscapeBattle)
					{
						GatherTime = EscapeBattleManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return -4;
						}
					}
					else
					{
						if (sceneType == SceneUIClasses.ShuiJingHuanJing)
						{
							if (client.ClientData.DailyCrystalCollectNum >= CaiJiLogic.DailyNum)
							{
								CaiJiLogic.CancelCaiJiState(client);
								return -6;
							}
						}
						if (!GameManager.systemCaiJiMonsterMgr.SystemXmlItemDict.TryGetValue(monster.MonsterInfo.ExtensionID, out CaiJiMonsterXmlItem) || null == CaiJiMonsterXmlItem)
						{
							CaiJiLogic.CancelCaiJiState(client);
							return -4;
						}
						GatherTime = CaiJiMonsterXmlItem.GetIntValue("GatherTime", -1);
					}
					GatherTime = GatherTime * CaiJiLogic.GatherTimePer / 100;
					uint intervalmsec = TimeUtil.timeGetTime() - client.ClientData.CaiJiStartTick;
					if ((ulong)intervalmsec < (ulong)((long)(GatherTime * 1000)))
					{
						CaiJiLogic.CancelCaiJiState(client);
						LogManager.WriteLog(LogTypes.Error, string.Format("采集读条时间不足intervalmsec={0}", intervalmsec), null, true);
						result = -5;
					}
					else
					{
						CaiJiLogic.CancelCaiJiState(client);
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 400.0)
						{
							result = -2;
						}
						else
						{
							lock (monster.CaiJiStateLock)
							{
								if (monster.IsCollected)
								{
									return -4;
								}
								monster.IsCollected = true;
							}
							if (!GlobalEventSource4Scene.getInstance().fireEvent(new CaiJiEventObject(client, monster), (int)sceneType))
							{
								if (sceneType == SceneUIClasses.HuanYingSiYuan)
								{
									HuanYingSiYuanManager.getInstance().OnCaiJiFinish(client, monster);
								}
								else if (sceneType == SceneUIClasses.LingDiCaiJi)
								{
									LingDiCaiJiManager.getInstance().OnCaiJiFinish(client, monster);
								}
								else
								{
									CaiJiLogic.UpdateCaiJiData(client);
									CaiJiLogic.NotifyCollectLastNum(client, 0, CaiJiLogic.DailyNum - client.ClientData.DailyCrystalCollectNum);
									float AwardRate = 1f;
									int rangeIndex = CaiJiLogic.JugeDateTimeInTimeRange(TimeUtil.NowDateTime(), CaiJiLogic.dateTimeRangeArray, true);
									if (rangeIndex >= 0)
									{
										AwardRate = CaiJiLogic.dateTimeRangeArray[rangeIndex].DoubleAwardRate;
									}
									int ExpAward = (int)(AwardRate * (float)CaiJiMonsterXmlItem.GetIntValue("ExpAward", -1));
									int XingHunAward = (int)(AwardRate * (float)CaiJiMonsterXmlItem.GetIntValue("XingHunAward", -1));
									int BindZuanShiAward = (int)(AwardRate * (float)CaiJiMonsterXmlItem.GetIntValue("BindZuanShiAward", -1));
									int BindJinBiAward = (int)(AwardRate * (float)CaiJiMonsterXmlItem.GetIntValue("BindJinBiAward", -1));
									int MoJingAward = (int)(AwardRate * (float)CaiJiMonsterXmlItem.GetIntValue("MoJingAward", -1));
									if (ExpAward > 0)
									{
										GameManager.ClientMgr.ProcessRoleExperience(client, (long)ExpAward, true, true, false, "none");
									}
									if (XingHunAward > 0)
									{
										GameManager.ClientMgr.ModifyStarSoulValue(client, XingHunAward, "采集获得星魂", true, true);
									}
									if (BindZuanShiAward > 0)
									{
										GameManager.ClientMgr.AddUserGold(client, BindZuanShiAward, "采集获得绑钻");
									}
									if (BindJinBiAward > 0)
									{
										GameManager.ClientMgr.AddMoney1(client, BindJinBiAward, "采集获得绑金", true);
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(28, new object[0]), new object[]
										{
											BindJinBiAward
										}), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlyErr, 0);
									}
									if (MoJingAward > 0)
									{
										GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, MoJingAward, "采集获得魔晶", true, true, false);
									}
									ProcessTask.ProcessAddTaskVal(client, TaskTypes.CaiJi_ShuiJingHuanJing, -1, 1, new object[0]);
								}
							}
							GameManager.MonsterMgr.DeadMonsterImmediately(monster);
							ProcessTask.Process(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, monster.RoleID, monster.MonsterInfo.ExtensionID, -1, TaskTypes.CaiJiGoods, null, 0, -1L, null);
							result = 0;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x00076E30 File Offset: 0x00075030
		public static int CancelCaiJiState(GameClient client)
		{
			if (null != client)
			{
				client.ClientData.CaiJiStartTick = 0U;
				client.ClientData.CaijTargetId = 0;
				client.ClientData.CaijGoodsDBId = 0L;
				client.ClientData.CaiJiTargetUniqueID = 0L;
				if (client.ClientData.gatherNpcID > 0)
				{
					client.ClientData.gatherNpcID = 0;
				}
			}
			return 0;
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x00076EA4 File Offset: 0x000750A4
		public static int SetCaiJiState(GameClient client, int monsterId, long goodsID = 0L, long uniqueId = 0L)
		{
			if (null != client)
			{
				client.ClientData.CaiJiStartTick = TimeUtil.timeGetTime();
				client.ClientData.CaijTargetId = monsterId;
				client.ClientData.CaijGoodsDBId = goodsID;
				client.ClientData.CaiJiTargetUniqueID = uniqueId;
			}
			return 0;
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x00076EF8 File Offset: 0x000750F8
		public static bool IsCaiJiState(GameClient client)
		{
			return null != client && ((client.ClientData.CaiJiStartTick > 0U && (client.ClientData.CaijTargetId > 0 || client.ClientData.CaijGoodsDBId > 0L)) || client.ClientData.gatherNpcID > 0);
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x00076F74 File Offset: 0x00075174
		public static int NotifyCollectLastNum(GameClient client, int HuodongType, int lastnum)
		{
			string strcmd = string.Format("{0}:{1}:{2}", 0, HuodongType, lastnum);
			client.sendCmd(682, strcmd, false);
			return 0;
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x00076FB4 File Offset: 0x000751B4
		public static int ReqCaiJiLastNum(GameClient client, int huodongType, out int lastnum)
		{
			lastnum = 0;
			int result;
			if (0 == huodongType)
			{
				lastnum = CaiJiLogic.DailyNum - client.ClientData.DailyCrystalCollectNum;
				result = 0;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x060007D0 RID: 2000 RVA: 0x00076FF0 File Offset: 0x000751F0
		public static void UpdateCaiJiData(GameClient client)
		{
			client.ClientData.DailyCrystalCollectNum++;
			client._IconStateMgr.CheckCaiJiState(client);
			Global.SaveRoleParamsInt32ValueToDB(client, "CaiJiCrystalNum", client.ClientData.DailyCrystalCollectNum, true);
			if (0 == client.ClientData.CrystalCollectDayID)
			{
				client.ClientData.CrystalCollectDayID = TimeUtil.NowDateTime().DayOfYear;
				Global.SaveRoleParamsInt32ValueToDB(client, "CaiJiCrystalDayID", client.ClientData.CrystalCollectDayID, true);
			}
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x00077080 File Offset: 0x00075280
		public static void InitRoleDailyCaiJiData(GameClient client, bool isLogin, bool isNewday)
		{
			if (GlobalNew.IsGongNengOpened(client, GongNengIDs.CrystalCollect, false))
			{
				if (isLogin)
				{
					client.ClientData.DailyCrystalCollectNum = Global.GetRoleParamsInt32FromDB(client, "CaiJiCrystalNum");
					client.ClientData.CrystalCollectDayID = Global.GetRoleParamsInt32FromDB(client, "CaiJiCrystalDayID");
				}
				bool bClear = false;
				if (isNewday)
				{
					if (client.ClientData.DailyCrystalCollectNum >= 0 && client.ClientData.CrystalCollectDayID > 0)
					{
						client.ClientData.OldCrystalCollectData = new OldCaiJiData();
						client.ClientData.OldCrystalCollectData.OldDay = client.ClientData.CrystalCollectDayID;
						client.ClientData.OldCrystalCollectData.OldNum = client.ClientData.DailyCrystalCollectNum;
					}
					bClear = true;
				}
				else if (0 == client.ClientData.CrystalCollectDayID)
				{
					bClear = true;
				}
				if (bClear)
				{
					client.ClientData.DailyCrystalCollectNum = 0;
					client.ClientData.CrystalCollectDayID = TimeUtil.NowDateTime().DayOfYear;
					Global.SaveRoleParamsInt32ValueToDB(client, "CaiJiCrystalNum", 0, true);
					Global.SaveRoleParamsInt32ValueToDB(client, "CaiJiCrystalDayID", client.ClientData.CrystalCollectDayID, true);
					if (Global.GetMapSceneType(client.ClientData.MapCode) == SceneUIClasses.ShuiJingHuanJing)
					{
						CaiJiLogic.NotifyCollectLastNum(client, 0, CaiJiLogic.DailyNum);
					}
				}
				client._IconStateMgr.CheckCaiJiState(client);
			}
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x000771FC File Offset: 0x000753FC
		public static bool HasLeftnum(GameClient client)
		{
			return GlobalNew.IsGongNengOpened(client, GongNengIDs.CrystalCollect, false) && client.ClientData.DailyCrystalCollectNum < CaiJiLogic.DailyNum;
		}

		// Token: 0x04000D4A RID: 3402
		public static CaiJiDateTimeRange[] dateTimeRangeArray = null;

		// Token: 0x04000D4B RID: 3403
		public static int DailyNum = 0;

		// Token: 0x04000D4C RID: 3404
		public static int DeadReliveTime = 0;

		// Token: 0x04000D4D RID: 3405
		public static int GatherTimePer = 100;
	}
}
