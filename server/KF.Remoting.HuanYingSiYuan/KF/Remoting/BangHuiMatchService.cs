using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract.Data;
using KF.Remoting.Data;
using Server.Tools;

namespace KF.Remoting
{
	
	public class BangHuiMatchService
	{
		
		public static BangHuiMatchService Instance()
		{
			return BangHuiMatchService._instance;
		}

		
		
		
		private KuaFuData<Dictionary<int, List<BangHuiMatchRankInfo>>> BHMatchRankInfoDict
		{
			get
			{
				return this.Persistence.BHMatchRankInfoDict;
			}
			set
			{
				this.Persistence.BHMatchRankInfoDict = value;
			}
		}

		
		
		
		private KuaFuData<List<BangHuiMatchPKInfo>> BHMatchPKInfoList_Gold
		{
			get
			{
				return this.Persistence.BHMatchPKInfoList_Gold;
			}
			set
			{
				this.Persistence.BHMatchPKInfoList_Gold = value;
			}
		}

		
		
		
		private Dictionary<int, KuaFuData<BHMatchBHData>> BHMatchBHDataDict_Gold
		{
			get
			{
				return this.Persistence.BHMatchBHDataDict_Gold;
			}
			set
			{
				this.Persistence.BHMatchBHDataDict_Gold = value;
			}
		}

		
		
		
		private Dictionary<int, KuaFuData<BHMatchBHData>> BHMatchBHDataDict_Rookie
		{
			get
			{
				return this.Persistence.BHMatchBHDataDict_Rookie;
			}
			set
			{
				this.Persistence.BHMatchBHDataDict_Rookie = value;
			}
		}

		
		
		
		private List<BHMatchBHData> BHMatchBHDataList_GoldJoin
		{
			get
			{
				return this.Persistence.BHMatchBHDataList_GoldJoin;
			}
			set
			{
				this.Persistence.BHMatchBHDataList_GoldJoin = value;
			}
		}

		
		
		
		private List<BHMatchBHData> BHMatchBHDataList_RookieJoin
		{
			get
			{
				return this.Persistence.BHMatchBHDataList_RookieJoin;
			}
			set
			{
				this.Persistence.BHMatchBHDataList_RookieJoin = value;
			}
		}

		
		
		
		private List<BHMatchBHData> BHMatchBHDataList_RookieJoinLast
		{
			get
			{
				return this.Persistence.BHMatchBHDataList_RookieJoinLast;
			}
			set
			{
				this.Persistence.BHMatchBHDataList_RookieJoinLast = value;
			}
		}

		
		public void InitConfig()
		{
			string strLeagueNewNum = KuaFuServerManager.systemParamsList.GetParamValueByName("LeagueNewNum");
			string[] LeagueNewNumFields = strLeagueNewNum.Split(new char[]
			{
				','
			});
			if (LeagueNewNumFields.Length == 2)
			{
				this.RuntimeData.RookieWinScoreFactor = Global.SafeConvertToInt32(LeagueNewNumFields[0]);
				this.RuntimeData.RookieLoseScoreFactor = Global.SafeConvertToInt32(LeagueNewNumFields[1]);
			}
			this.RuntimeData.RookiePromotionNum = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("LeagueUp", -1);
			if (!this.RuntimeData.Load(KuaFuServerManager.GetResourcePath("Config\\LeagueMatch.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\LeagueOpen.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\LeagueSuperList.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\LeagueNewRandom.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.platformType.ToString()))
			{
				LogManager.WriteLog(LogTypes.Error, "BangHuiMatchService.InitConfig failed!", null, true);
			}
		}

		
		private int ComputeCurrentSeasonID(DateTime now)
		{
			int GoldSeasonID = this.Persistence.GetGoldSeasonID();
			int CurSeasonID_Gold = this.GetCurrentSeasonIDByTime(now, true);
			int CurSeasonID_Rookie = this.GetCurrentSeasonIDByTime(now, false);
			return (GoldSeasonID == 0 || GoldSeasonID != CurSeasonID_Gold) ? CurSeasonID_Rookie : CurSeasonID_Gold;
		}

		
		private int GetCurrentSeasonIDByTime(DateTime now, bool calGold = true)
		{
			int result;
			lock (this.RuntimeData.MutexConfig)
			{
				if (!this.RuntimeData.CheckOpenState(now))
				{
					result = 0;
				}
				else
				{
					DateTime OpenTime = this.RuntimeData.GetActivityOpenTm();
					TimeSpan end = TimeSpan.MinValue;
					foreach (BHMatchConfig item in this.RuntimeData.BHMatchConfigDict.Values)
					{
						if (item.ID != 1 || calGold)
						{
							int lastWeakMatch = item.TimePoints.Count / 2 - item.RoundNum % (item.TimePoints.Count / 2);
							for (int i = 0; i < item.TimePoints.Count - 1; i += 2)
							{
								TimeSpan myTmp = item.TimePoints[i + 1];
								if (myTmp.Days == 0)
								{
									myTmp += new TimeSpan(7, 0, 0, 0);
								}
								if (myTmp > end && i / 2 < lastWeakMatch)
								{
									end = myTmp;
								}
							}
						}
					}
					end -= new TimeSpan(1, 0, 0, 0);
					TimeSpan endSunday = new TimeSpan(7, 0, 0, 0) - end;
					TimeSpan span = now + endSunday - OpenTime;
					int spanDay = span.Days - span.Days % (7 * this.RuntimeData.GetSeasonWeaks());
					OpenTime = OpenTime.AddDays((double)spanDay);
					result = BangHuiMatchUtils.MakeSeason(OpenTime);
				}
			}
			return result;
		}

		
		private int FixRound(BangHuiMatchType type, int round)
		{
			BHMatchConfig matchConfig = this.RuntimeData.GetBHMatchConfig((int)type);
			return (round > matchConfig.RoundNum) ? 1 : round;
		}

		
		private int GetCurrentRoundByTime(BangHuiMatchType type, DateTime now)
		{
			int result;
			lock (this.RuntimeData.MutexConfig)
			{
				if (!this.RuntimeData.CheckOpenState(now))
				{
					result = 0;
				}
				else
				{
					if (type == BangHuiMatchType.BHMT_Begin)
					{
						DateTime curSeasonTm = BangHuiMatchUtils.GetSeasonDateTm(this.CurrentSeasonID_Gold);
						if (now < curSeasonTm)
						{
							return 1;
						}
					}
					else if (type == BangHuiMatchType.Rookie)
					{
						DateTime curSeasonTm = BangHuiMatchUtils.GetSeasonDateTm(this.CurrentSeasonID_Rookie);
						if (now < curSeasonTm)
						{
							return 1;
						}
					}
					DateTime OpenTime = this.RuntimeData.GetActivityOpenTm();
					BHMatchConfig matchConfig = null;
					if (!this.RuntimeData.BHMatchConfigDict.TryGetValue((int)type, out matchConfig))
					{
						result = 0;
					}
					else
					{
						TimeSpan fromMonday = new TimeSpan((int)now.DayOfWeek, now.Hour, now.Minute, now.Second);
						if (fromMonday.Days == 0)
						{
							fromMonday += new TimeSpan(7, 0, 0, 0);
						}
						int weekRound = 0;
						for (int i = 0; i < matchConfig.TimePoints.Count - 1; i += 2)
						{
							TimeSpan myTmp = matchConfig.TimePoints[i + 1];
							if (myTmp.Days == 0)
							{
								myTmp += new TimeSpan(7, 0, 0, 0);
							}
							if (fromMonday > myTmp)
							{
								weekRound++;
							}
						}
						int week = (now - OpenTime).Days % (7 * this.RuntimeData.GetSeasonWeaks()) / 7;
						int round = week * matchConfig.TimePoints.Count / 2 + weekRound + 1;
						result = Math.Min(round, matchConfig.RoundNum + 1);
					}
				}
			}
			return result;
		}

		
		private int ComputeLastSeasonID(int CurSeasonID)
		{
			int result;
			if (0 == CurSeasonID)
			{
				result = 0;
			}
			else
			{
				result = BangHuiMatchUtils.MakeSeason(BangHuiMatchUtils.GetSeasonDateTm(CurSeasonID).AddDays((double)(-7 * this.RuntimeData.GetSeasonWeaks())));
			}
			return result;
		}

		
		public void LoadDatabase(DateTime now)
		{
			try
			{
				lock (this.Mutex)
				{
					this.CurrentSeasonID_Gold = this.ComputeCurrentSeasonID(now);
					this.LastSeasonID_Gold = this.Persistence.LoadLastSeasonIDGold();
					this.CurrentSeasonID_Rookie = this.GetCurrentSeasonIDByTime(now, false);
					this.LastSeasonID_Rookie = this.ComputeLastSeasonID(this.CurrentSeasonID_Rookie);
					this.Persistence.LoadDatabase(this.CurrentSeasonID_Gold, this.LastSeasonID_Gold, this.CurrentSeasonID_Rookie, this.LastSeasonID_Rookie);
					this.FixCurGoldRankInfo();
					this.InitFuBenManagerData(now);
					this.ReloadChampionRoleData_Gold();
					this.HandleBHMatchGoldAccident(now);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "BangHuiMatchService.LoadDatabase failed!", ex, true);
			}
		}

		
		private void InitFuBenManagerData(DateTime now)
		{
			this.LastUpdateTime = now;
			for (int typeloop = 1; typeloop <= 2; typeloop++)
			{
				this.FuBenMgrDict[typeloop] = new BHMatchFuBenMgrData();
				this.FuBenMgrDict[typeloop].Round = (byte)this.GetCurrentRoundByTime((BangHuiMatchType)typeloop, now);
				this.StateMachineDict[typeloop] = new BHMatchStateMachine();
				this.StateMachineDict[typeloop].Install(new BHMatchStateMachine.StateHandler(BHMatchStateMachine.StateType.Init, null, new Action<DateTime, int>(this.MS_Init_Update), null));
				this.StateMachineDict[typeloop].Install(new BHMatchStateMachine.StateHandler(BHMatchStateMachine.StateType.SignUp, null, new Action<DateTime, int>(this.MS_SignUp_Update), null));
				this.StateMachineDict[typeloop].Install(new BHMatchStateMachine.StateHandler(BHMatchStateMachine.StateType.PrepareGame, null, new Action<DateTime, int>(this.MS_PrepareGame_Update), null));
				this.StateMachineDict[typeloop].Install(new BHMatchStateMachine.StateHandler(BHMatchStateMachine.StateType.NotifyEnter, null, new Action<DateTime, int>(this.MS_NotifyEnter_Update), null));
				this.StateMachineDict[typeloop].Install(new BHMatchStateMachine.StateHandler(BHMatchStateMachine.StateType.GameStart, null, new Action<DateTime, int>(this.MS_GameStart_Update), null));
				this.StateMachineDict[typeloop].Install(new BHMatchStateMachine.StateHandler(BHMatchStateMachine.StateType.RankAnalyse, new Action<DateTime, int>(this.MS_RankAnalyse_Enter), null, null));
				this.StateMachineDict[typeloop].SetCurrState(BHMatchStateMachine.StateType.Init, TimeUtil.NowDateTime(), typeloop);
				this.StateMachineDict[typeloop].Tick(now, typeloop);
			}
		}

		
		private void HandleBHMatchGoldAccident(DateTime now)
		{
			if (this.CurrentSeasonID_Gold != 0 && 0 != this.BHMatchBHDataList_GoldJoin.Count)
			{
				int curRound = (int)this.FuBenMgrDict[1].Round;
				BHMatchConfig matchConfig = this.RuntimeData.GetBHMatchConfig(1);
				if (curRound <= matchConfig.RoundNum)
				{
					BangHuiMatchPKInfo pkInfo = this.BHMatchPKInfoList_Gold.V.Find((BangHuiMatchPKInfo x) => x.season == this.CurrentSeasonID_Gold && (int)x.round == curRound);
					if (null == pkInfo)
					{
						this.GenerateNextRoundPKInfo_Gold(now, curRound);
						LogManager.WriteLog(LogTypes.Analysis, string.Format("BHMatch::HandleBHMatchGoldAccident SeasonID_Gold:{0} SeasonID_Rookie:{1} Round:{2}", this.CurrentSeasonID_Gold, this.CurrentSeasonID_Rookie, curRound), null, true);
					}
				}
			}
		}

		
		public void SwitchLastGoldBH_GM()
		{
			List<BangHuiMatchRankInfo> goldRankList = null;
			if (this.BHMatchRankInfoDict.V.TryGetValue(0, out goldRankList))
			{
				goldRankList.Reverse(0, goldRankList.Count);
				TimeUtil.AgeByNow(ref this.BHMatchRankInfoDict.Age);
				this.ReloadChampionRoleData_Gold();
			}
		}

		
		private void ReloadChampionRoleData_Gold()
		{
			List<BangHuiMatchRankInfo> goldRankList = null;
			if (!this.BHMatchRankInfoDict.V.TryGetValue(0, out goldRankList))
			{
				this.BHMatchChampionRoleData_Gold.Bytes0 = null;
				TimeUtil.AgeByNow(ref this.BHMatchChampionRoleData_Gold.Age);
			}
			else
			{
				KuaFuData<BHMatchBHData> bhData = null;
				if (goldRankList.Count == 0 || !this.BHMatchBHDataDict_Gold.TryGetValue(goldRankList[0].Key, out bhData))
				{
					this.BHMatchChampionRoleData_Gold.Bytes0 = null;
					TimeUtil.AgeByNow(ref this.BHMatchChampionRoleData_Gold.Age);
				}
				else
				{
					this.BHMatchChampionRoleData_Gold.Bytes0 = this.Persistence.LoadBHMatchRoleData(bhData.V.type, bhData.V.rid);
					TimeUtil.AgeByNow(ref this.BHMatchChampionRoleData_Gold.Age);
				}
			}
		}

		
		private void FixCurGoldRankInfo()
		{
			List<BangHuiMatchRankInfo> goldRankList = null;
			if (this.BHMatchRankInfoDict.V.TryGetValue(8, out goldRankList) && goldRankList.Count != 0)
			{
				Dictionary<int, List<BangHuiMatchRankInfo>> WinVsRankDict = new Dictionary<int, List<BangHuiMatchRankInfo>>();
				foreach (BangHuiMatchRankInfo item in goldRankList)
				{
					List<BangHuiMatchRankInfo> partRankList = null;
					if (!WinVsRankDict.TryGetValue(item.Value, out partRankList))
					{
						partRankList = new List<BangHuiMatchRankInfo>();
						WinVsRankDict[item.Value] = partRankList;
					}
					partRankList.Add(item);
				}
				List<BangHuiMatchPKInfo> pkInfoList = this.BHMatchPKInfoList_Gold.V.FindAll((BangHuiMatchPKInfo x) => x.season == this.CurrentSeasonID_Gold);
				foreach (KeyValuePair<int, List<BangHuiMatchRankInfo>> kvp in WinVsRankDict)
				{
					List<BangHuiMatchRankInfo> partRankList = kvp.Value;
					foreach (BangHuiMatchRankInfo item in partRankList)
					{
						item.RankValue = 0;
					}
					if (partRankList.Count > 1)
					{
						for (int i = 0; i < partRankList.Count; i++)
						{
							BangHuiMatchRankInfo left = partRankList[i];
							for (int j = i + 1; j < partRankList.Count; j++)
							{
								BangHuiMatchRankInfo right = partRankList[j];
								BangHuiMatchPKInfo pkInfo = pkInfoList.Find((BangHuiMatchPKInfo x) => (x.bhid1 == left.Key && x.bhid2 == right.Key) || (x.bhid2 == left.Key && x.bhid1 == right.Key));
								if (null != pkInfo)
								{
									if (pkInfo.bhid1 == left.Key)
									{
										if (pkInfo.result == 1)
										{
											left.RankValue++;
										}
										else if (pkInfo.result == 2)
										{
											right.RankValue++;
										}
									}
									else if (pkInfo.result == 1)
									{
										right.RankValue++;
									}
									else if (pkInfo.result == 2)
									{
										left.RankValue++;
									}
								}
							}
						}
					}
				}
				List<BangHuiMatchRankInfo> newRankList = new List<BangHuiMatchRankInfo>(goldRankList);
				foreach (BangHuiMatchRankInfo item in newRankList)
				{
					LogManager.WriteLog(LogTypes.Analysis, string.Format("BHMatch::FixCurGoldRankInfo Before Key:{0} Param1:{1} Param2:{2} Value:{3} RankValue:{4}", new object[]
					{
						item.Key,
						item.Param1,
						item.Param2,
						item.Value,
						item.RankValue
					}), null, true);
				}
				newRankList.Sort(delegate(BangHuiMatchRankInfo left, BangHuiMatchRankInfo right)
				{
					int result;
					if (left.Value > right.Value)
					{
						result = -1;
					}
					else if (left.Value < right.Value)
					{
						result = 1;
					}
					else if (left.RankValue > right.RankValue)
					{
						result = -1;
					}
					else if (left.RankValue < right.RankValue)
					{
						result = 1;
					}
					else
					{
						int leftBefore = goldRankList.FindIndex((BangHuiMatchRankInfo x) => x.Key == left.Key);
						int rightBefore = goldRankList.FindIndex((BangHuiMatchRankInfo x) => x.Key == right.Key);
						if (leftBefore < rightBefore)
						{
							result = -1;
						}
						else if (leftBefore > rightBefore)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
					}
					return result;
				});
				foreach (BangHuiMatchRankInfo item in newRankList)
				{
					LogManager.WriteLog(LogTypes.Analysis, string.Format("BHMatch::FixCurGoldRankInfo After Key:{0} Param1:{1} Param2:{2} Value:{3} RankValue:{4}", new object[]
					{
						item.Key,
						item.Param1,
						item.Param2,
						item.Value,
						item.RankValue
					}), null, true);
				}
				this.BHMatchRankInfoDict.V[8] = newRankList;
				TimeUtil.AgeByNow(ref this.BHMatchRankInfoDict.Age);
			}
		}

		
		private BangHuiMatchType GetMatchTypeByBhid(int bhid)
		{
			BangHuiMatchType result;
			lock (this.Mutex)
			{
				if (this.BHMatchBHDataList_GoldJoin.Exists((BHMatchBHData x) => x.bhid == bhid))
				{
					result = BangHuiMatchType.BHMT_Begin;
				}
				else
				{
					result = BangHuiMatchType.Rookie;
				}
			}
			return result;
		}

		
		public void Update(DateTime now)
		{
			try
			{
				if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot6))
				{
					if ((now - this.LastUpdateTime).TotalMilliseconds >= 1000.0)
					{
						this.UpdateFrameCount += 1U;
						foreach (KeyValuePair<int, BHMatchStateMachine> kvp in this.StateMachineDict)
						{
							lock (this.Mutex)
							{
								kvp.Value.Tick(now, kvp.Key);
							}
						}
						this.Persistence.DelayWriteDataProc();
						this.LastUpdateTime = now;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "BangHuiMatchService.Update failed!", ex, true);
			}
		}

		
		private void MS_Init_Update(DateTime now, int param)
		{
			if (this.RuntimeData.CheckOpenState(now))
			{
				BHMatchConfig matchConfig = this.RuntimeData.GetBHMatchConfig(param);
				BHMatchStateMachine.StateType GameState = BHMatchStateMachine.StateType.SignUp;
				for (int i = 0; i < matchConfig.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)matchConfig.TimePoints[i].Days)
					{
						if (now.TimeOfDay.TotalSeconds >= matchConfig.SecondsOfDay[i] - (double)matchConfig.ApplyOverTime && now.TimeOfDay.TotalSeconds < matchConfig.SecondsOfDay[i])
						{
							GameState = BHMatchStateMachine.StateType.PrepareGame;
						}
						else if (now.TimeOfDay.TotalSeconds >= matchConfig.SecondsOfDay[i] && now.TimeOfDay.TotalSeconds < matchConfig.SecondsOfDay[i + 1])
						{
							GameState = BHMatchStateMachine.StateType.GameStart;
						}
						else if (now.TimeOfDay.TotalSeconds >= matchConfig.SecondsOfDay[i + 1] && now.TimeOfDay.TotalSeconds <= matchConfig.SecondsOfDay[i + 1] + (double)matchConfig.ApplyStartTime)
						{
							GameState = BHMatchStateMachine.StateType.RankAnalyse;
						}
					}
				}
				this.CurrentSeasonID_Gold = this.ComputeCurrentSeasonID(now);
				this.CurrentSeasonID_Rookie = this.GetCurrentSeasonIDByTime(now, false);
				this.StateMachineDict[param].SetCurrState(GameState, now, param);
				LogManager.WriteLog(LogTypes.Analysis, string.Format("BHMatch::MS_Init_Update MatchType:{0} To:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
				{
					(BangHuiMatchType)param,
					GameState,
					this.CurrentSeasonID_Gold,
					this.CurrentSeasonID_Rookie,
					this.FuBenMgrDict[param].Round
				}), null, true);
			}
		}

		
		private void MS_SignUp_Update(DateTime now, int param)
		{
			if (this.RuntimeData.CheckOpenState(now))
			{
				BHMatchConfig matchConfig = this.RuntimeData.GetBHMatchConfig(param);
				BHMatchStateMachine.StateType GameState = BHMatchStateMachine.StateType.None;
				for (int i = 0; i < matchConfig.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)matchConfig.TimePoints[i].Days)
					{
						double ApplyOverTime = matchConfig.SecondsOfDay[i] - (double)matchConfig.ApplyOverTime;
						if (this.LastUpdateTime.TimeOfDay.TotalSeconds < ApplyOverTime && now.TimeOfDay.TotalSeconds >= ApplyOverTime)
						{
							GameState = BHMatchStateMachine.StateType.PrepareGame;
						}
					}
				}
				if (GameState == BHMatchStateMachine.StateType.PrepareGame)
				{
					this.StateMachineDict[param].SetCurrState(GameState, now, param);
					LogManager.WriteLog(LogTypes.Analysis, string.Format("BHMatch::MS_SignUp_Update MatchType:{0} To:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
					{
						(BangHuiMatchType)param,
						GameState,
						this.CurrentSeasonID_Gold,
						this.CurrentSeasonID_Rookie,
						this.FuBenMgrDict[param].Round
					}), null, true);
				}
			}
		}

		
		private void MS_PrepareGame_Update(DateTime now, int param)
		{
			if (ClientAgentManager.Instance().IsAnyKfAgentAlive())
			{
				BHMatchConfig matchConfig = this.RuntimeData.GetBHMatchConfig(param);
				int round = this.GetCurrentRoundByTime((BangHuiMatchType)param, now);
				this.FuBenMgrDict[param].Round = (byte)round;
				if (param == 1)
				{
					if (this.BHMatchBHDataList_GoldJoin.Count < 8 || round > matchConfig.RoundNum)
					{
						this.StateMachineDict[param].SetCurrState(BHMatchStateMachine.StateType.NotifyEnter, now, param);
						LogManager.WriteLog(LogTypes.Analysis, string.Format("BHMatch::MS_PrepareGame_Update MatchType:{0} SkipTo:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
						{
							(BangHuiMatchType)param,
							BHMatchStateMachine.StateType.NotifyEnter,
							this.CurrentSeasonID_Gold,
							this.CurrentSeasonID_Rookie,
							this.FuBenMgrDict[param].Round
						}), null, true);
						return;
					}
					DateTime curSeasonTm = BangHuiMatchUtils.GetSeasonDateTm(this.CurrentSeasonID_Gold);
					if (now < curSeasonTm)
					{
						this.StateMachineDict[param].SetCurrState(BHMatchStateMachine.StateType.SignUp, now, param);
						LogManager.WriteLog(LogTypes.Analysis, string.Format("BHMatch::MS_PrepareGame_Update MatchType:{0} SkipTo:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
						{
							(BangHuiMatchType)param,
							BHMatchStateMachine.StateType.SignUp,
							this.CurrentSeasonID_Gold,
							this.CurrentSeasonID_Rookie,
							this.FuBenMgrDict[param].Round
						}), null, true);
						return;
					}
					BHMatchGoldGroupConfig groupConfig = this.RuntimeData.GetBHMatchGoldGroupConfig(round);
					for (int i = 0; i < groupConfig.GroupUnion.Length; i++)
					{
						int group;
						int group2;
						BangHuiMatchUtils.SplitUnionGroup(groupConfig.GroupUnion[i], out group, out group2);
						BHMatchFuBenData fubenData = new BHMatchFuBenData();
						fubenData.Type = (byte)param;
						fubenData.bhid1 = this.BHMatchBHDataList_GoldJoin[group - 1].bhid;
						fubenData.bhid2 = this.BHMatchBHDataList_GoldJoin[group2 - 1].bhid;
						int toServerId = 0;
						int gameId = TianTiPersistence.Instance.GetNextGameId();
						if (ClientAgentManager.Instance().AssginKfFuben(this.GameType, (long)gameId, matchConfig.MaxEnterNum, out toServerId))
						{
							fubenData.ServerId = toServerId;
							fubenData.GameId = (long)gameId;
							this.FuBenMgrDict[param].FuBenDataDict[gameId] = fubenData;
							this.FuBenMgrDict[param].BHidVsGameId[fubenData.bhid1] = gameId;
							this.FuBenMgrDict[param].BHidVsGameId[fubenData.bhid2] = gameId;
							LogManager.WriteLog(LogTypes.Analysis, string.Format("黄金赛分组 gameId:{0} bhid1:{1} bhname1:{2} bhid2:{3} bhname2:{4}", new object[]
							{
								gameId,
								fubenData.bhid1,
								this.BHMatchBHDataList_GoldJoin[group - 1].bhname,
								fubenData.bhid2,
								this.BHMatchBHDataList_GoldJoin[group2 - 1].bhname
							}), null, true);
						}
						else
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("黄金赛{0}赛季第{1}轮分配游戏服务器失败,bhid1={2},bhid2={3}", new object[]
							{
								this.CurrentSeasonID_Gold,
								round,
								fubenData.bhid1,
								fubenData.bhid2
							}), null, true);
						}
					}
				}
				else
				{
					if (this.BHMatchBHDataList_RookieJoin.Count == 0 || round > matchConfig.RoundNum)
					{
						this.StateMachineDict[param].SetCurrState(BHMatchStateMachine.StateType.NotifyEnter, now, param);
						LogManager.WriteLog(LogTypes.Analysis, string.Format("BHMatch::MS_PrepareGame_Update MatchType:{0} SkipTo:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
						{
							(BangHuiMatchType)param,
							BHMatchStateMachine.StateType.NotifyEnter,
							this.CurrentSeasonID_Gold,
							this.CurrentSeasonID_Rookie,
							this.FuBenMgrDict[param].Round
						}), null, true);
						return;
					}
					this.BHMatchBHDataList_RookieJoin.Sort(delegate(BHMatchBHData left, BHMatchBHData right)
					{
						int result;
						if (left.cur_score > right.cur_score)
						{
							result = -1;
						}
						else if (left.cur_score < right.cur_score)
						{
							result = 1;
						}
						else if (left.hist_score > right.hist_score)
						{
							result = -1;
						}
						else if (left.hist_score < right.hist_score)
						{
							result = 1;
						}
						else if (left.bhid > right.bhid)
						{
							result = -1;
						}
						else if (left.bhid < right.bhid)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					List<BHMatchRookieRandomConfig> rookieRandomList = null;
					lock (this.RuntimeData.MutexConfig)
					{
						rookieRandomList = this.RuntimeData.BHMatchRookieRandomConfigList;
					}
					int iloop = 0;
					while (iloop < rookieRandomList.Count && this.BHMatchBHDataList_RookieJoin.Count > 0)
					{
						BHMatchRookieRandomConfig randomConfig = rookieRandomList[iloop];
						if (randomConfig.BeginNum <= this.BHMatchBHDataList_RookieJoin.Count)
						{
							int endNum = Math.Min(this.BHMatchBHDataList_RookieJoin.Count, randomConfig.EndNum);
							int rangeNum = endNum - randomConfig.BeginNum + 1;
							List<BHMatchBHData> bhRangeList = this.BHMatchBHDataList_RookieJoin.GetRange(randomConfig.BeginNum - 1, rangeNum);
							Random r = new Random((int)now.Ticks);
							int i = 0;
							while (bhRangeList.Count > 0 && i < bhRangeList.Count * 2)
							{
								int idx = r.Next(0, bhRangeList.Count);
								int idx2 = r.Next(0, bhRangeList.Count);
								BHMatchBHData tmp = bhRangeList[idx];
								bhRangeList[idx] = bhRangeList[idx2];
								bhRangeList[idx2] = tmp;
								i++;
							}
							int currIdx = 0;
							for (i = 0; i < bhRangeList.Count / 2; i++)
							{
								BHMatchBHData joinBH = bhRangeList[currIdx++];
								BHMatchBHData joinBH2 = bhRangeList[currIdx++];
								BHMatchFuBenData fubenData = new BHMatchFuBenData();
								fubenData.Type = (byte)param;
								fubenData.bhid1 = joinBH.bhid;
								fubenData.bhid2 = joinBH2.bhid;
								int toServerId = 0;
								int gameId = TianTiPersistence.Instance.GetNextGameId();
								if (ClientAgentManager.Instance().AssginKfFuben(this.GameType, (long)gameId, matchConfig.MaxEnterNum, out toServerId))
								{
									fubenData.ServerId = toServerId;
									fubenData.GameId = (long)gameId;
									this.FuBenMgrDict[param].FuBenDataDict[gameId] = fubenData;
									this.FuBenMgrDict[param].BHidVsGameId[joinBH.bhid] = gameId;
									this.FuBenMgrDict[param].BHidVsGameId[joinBH2.bhid] = gameId;
									LogManager.WriteLog(LogTypes.Analysis, string.Format("新星赛分组 gameId:{0} bhid1:{1} bhname1:{2} bhid2:{3} bhname2:{4}", new object[]
									{
										gameId,
										fubenData.bhid1,
										joinBH.bhname,
										fubenData.bhid2,
										joinBH2.bhname
									}), null, true);
								}
								else
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("新星赛{0}赛季第{1}轮分配游戏服务器失败,bhid1={2},bhid2={3}", new object[]
									{
										this.CurrentSeasonID_Rookie,
										round,
										joinBH.bhid,
										joinBH2.bhid
									}), null, true);
								}
							}
							while (currIdx < bhRangeList.Count)
							{
								BHMatchBHData joinBH3 = bhRangeList[currIdx++];
								joinBH3.hist_score += this.RuntimeData.RookieWinScoreFactor;
								joinBH3.cur_score += this.RuntimeData.RookieWinScoreFactor;
								this.Persistence.SaveBHMatchBHSeasonData(this.CurrentSeasonID_Rookie, joinBH3, false, true);
							}
						}
						iloop++;
					}
				}
				this.StateMachineDict[param].SetCurrState(BHMatchStateMachine.StateType.NotifyEnter, now, param);
				LogManager.WriteLog(LogTypes.Analysis, string.Format("BHMatch::MS_PrepareGame_Update MatchType:{0} To:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
				{
					(BangHuiMatchType)param,
					BHMatchStateMachine.StateType.NotifyEnter,
					this.CurrentSeasonID_Gold,
					this.CurrentSeasonID_Rookie,
					this.FuBenMgrDict[param].Round
				}), null, true);
			}
		}

		
		private void MS_NotifyEnter_Update(DateTime now, int param)
		{
			BHMatchConfig matchConfig = this.RuntimeData.GetBHMatchConfig(param);
			BHMatchStateMachine.StateType GameState = BHMatchStateMachine.StateType.None;
			for (int i = 0; i < matchConfig.TimePoints.Count - 1; i += 2)
			{
				if (now.DayOfWeek == (DayOfWeek)matchConfig.TimePoints[i].Days)
				{
					if (now.TimeOfDay.TotalSeconds >= matchConfig.SecondsOfDay[i])
					{
						GameState = BHMatchStateMachine.StateType.GameStart;
					}
				}
			}
			if (GameState == BHMatchStateMachine.StateType.GameStart)
			{
				BHMatchFuBenMgrData fubenMgr = this.FuBenMgrDict[param];
				foreach (BHMatchFuBenData item in fubenMgr.FuBenDataDict.Values)
				{
					BHMatchNtfEnterData SyncData = new BHMatchNtfEnterData();
					SyncData.bhid1 = item.bhid1;
					SyncData.bhid2 = item.bhid2;
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.EvItemGameType, new AsyncDataItem(KuaFuEventTypes.BHMatchNtfEnter, new object[]
					{
						SyncData
					}), 0);
				}
				this.StateMachineDict[param].SetCurrState(GameState, now, param);
				LogManager.WriteLog(LogTypes.Analysis, string.Format("BHMatch::MS_PrepareGame_Update MatchType:{0} To:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
				{
					(BangHuiMatchType)param,
					GameState,
					this.CurrentSeasonID_Gold,
					this.CurrentSeasonID_Rookie,
					this.FuBenMgrDict[param].Round
				}), null, true);
			}
		}

		
		private void MS_GameStart_Update(DateTime now, int param)
		{
			BHMatchConfig matchConfig = this.RuntimeData.GetBHMatchConfig(param);
			BHMatchStateMachine.StateType GameState = BHMatchStateMachine.StateType.None;
			for (int i = 0; i < matchConfig.TimePoints.Count - 1; i += 2)
			{
				if (now.DayOfWeek == (DayOfWeek)matchConfig.TimePoints[i].Days)
				{
					if (now.TimeOfDay.TotalSeconds >= matchConfig.SecondsOfDay[i + 1] + (double)(matchConfig.ApplyStartTime / 2))
					{
						GameState = BHMatchStateMachine.StateType.RankAnalyse;
					}
				}
			}
			if (GameState == BHMatchStateMachine.StateType.RankAnalyse)
			{
				this.StateMachineDict[param].SetCurrState(GameState, now, param);
				LogManager.WriteLog(LogTypes.Analysis, string.Format("BHMatch::MS_GameStart_Update MatchType:{0} To:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
				{
					(BangHuiMatchType)param,
					GameState,
					this.CurrentSeasonID_Gold,
					this.CurrentSeasonID_Rookie,
					this.FuBenMgrDict[param].Round
				}), null, true);
			}
		}

		
		private void HandleUnCompleteFuBenData()
		{
			foreach (KeyValuePair<int, BHMatchFuBenMgrData> fubenMgrItem in this.FuBenMgrDict)
			{
				int matchType = fubenMgrItem.Key;
				foreach (KeyValuePair<int, BHMatchFuBenData> fubenItem in fubenMgrItem.Value.FuBenDataDict)
				{
					BHMatchFuBenData fubenData = fubenItem.Value;
					KuaFuData<BHMatchBHData> bh1Data = null;
					KuaFuData<BHMatchBHData> bh2Data = null;
					if (matchType == 1)
					{
						this.BHMatchBHDataDict_Gold.TryGetValue(fubenData.bhid1, out bh1Data);
						this.BHMatchBHDataDict_Gold.TryGetValue(fubenData.bhid2, out bh2Data);
						if (bh1Data != null && null != bh2Data)
						{
							bh1Data.V.hist_play++;
							bh2Data.V.hist_play++;
							TimeUtil.AgeByNow(ref bh1Data.Age);
							TimeUtil.AgeByNow(ref bh2Data.Age);
							this.Persistence.SaveBHMatchBHData(bh1Data.V, false, false);
							this.Persistence.SaveBHMatchBHData(bh2Data.V, false, false);
						}
					}
					else
					{
						this.BHMatchBHDataDict_Rookie.TryGetValue(fubenData.bhid1, out bh1Data);
						this.BHMatchBHDataDict_Rookie.TryGetValue(fubenData.bhid2, out bh2Data);
						if (bh1Data != null && null != bh2Data)
						{
							bh1Data.V.hist_score += this.RuntimeData.RookieLoseScoreFactor;
							bh2Data.V.hist_score += this.RuntimeData.RookieLoseScoreFactor;
							bh1Data.V.cur_score += this.RuntimeData.RookieLoseScoreFactor;
							bh2Data.V.cur_score += this.RuntimeData.RookieLoseScoreFactor;
							bh1Data.V.hist_play++;
							bh2Data.V.hist_play++;
							TimeUtil.AgeByNow(ref bh1Data.Age);
							TimeUtil.AgeByNow(ref bh2Data.Age);
							this.Persistence.SaveBHMatchBHData(bh1Data.V, false, false);
							this.Persistence.SaveBHMatchBHData(bh2Data.V, false, false);
							this.Persistence.SaveBHMatchBHSeasonData(this.CurrentSeasonID_Rookie, bh1Data.V, false, true);
							this.Persistence.SaveBHMatchBHSeasonData(this.CurrentSeasonID_Rookie, bh2Data.V, false, true);
						}
					}
					if (bh1Data != null && null != bh2Data)
					{
						this.GeneratePKInfo((byte)matchType, this.CurrentSeasonID_Rookie, (int)this.FuBenMgrDict[matchType].Round, bh1Data.V, bh2Data.V, 0);
					}
					ClientAgentManager.Instance().RemoveKfFuben(this.GameType, fubenData.ServerId, fubenData.GameId);
				}
				fubenMgrItem.Value.Clear();
			}
		}

		
		private void MS_RankAnalyse_Enter(DateTime now, int param)
		{
			this.HandleUnCompleteFuBenData();
			int curRound = this.GetCurrentRoundByTime((BangHuiMatchType)param, now);
			if (this.CurrentSeasonID_Gold != this.ComputeCurrentSeasonID(now))
			{
				this.HandlePromotion();
				this.Persistence.SaveLastSeasonIDGold(this.CurrentSeasonID_Gold);
				this.LastSeasonID_Gold = this.CurrentSeasonID_Gold;
				this.CurrentSeasonID_Gold = this.ComputeCurrentSeasonID(now);
				for (int goldLoop = 0; goldLoop < this.BHMatchBHDataList_GoldJoin.Count; goldLoop++)
				{
					BHMatchBHData bhData = this.BHMatchBHDataList_GoldJoin[goldLoop];
					this.Persistence.SaveBHMatchBHSeasonData(this.CurrentSeasonID_Gold, bhData, true, true);
				}
				this.Persistence.ReloadDatabasePerRound(1, this.CurrentSeasonID_Gold, this.LastSeasonID_Gold, true);
				this.ReloadChampionRoleData_Gold();
				this.GenerateNextRoundPKInfo_Gold(now, curRound);
				this.FuBenMgrDict[1].Round = (byte)this.GetCurrentRoundByTime(BangHuiMatchType.BHMT_Begin, now);
			}
			else if (param == 1)
			{
				this.Persistence.ReloadDatabasePerRound(param, this.CurrentSeasonID_Gold, this.LastSeasonID_Gold, false);
				this.FixCurGoldRankInfo();
				BHMatchConfig matchConfig = this.RuntimeData.GetBHMatchConfig(param);
				if (curRound <= matchConfig.RoundNum)
				{
					this.GenerateNextRoundPKInfo_Gold(now, curRound);
				}
			}
			if (this.CurrentSeasonID_Rookie != this.GetCurrentSeasonIDByTime(now, false))
			{
				this.LastSeasonID_Rookie = this.CurrentSeasonID_Rookie;
				this.CurrentSeasonID_Rookie = this.GetCurrentSeasonIDByTime(now, false);
				this.Persistence.ReloadDatabasePerRound(2, this.CurrentSeasonID_Rookie, this.LastSeasonID_Rookie, true);
			}
			else if (param == 2)
			{
				this.Persistence.ReloadDatabasePerRound(param, this.CurrentSeasonID_Rookie, this.LastSeasonID_Rookie, false);
			}
			this.FuBenMgrDict[param].Round = (byte)this.GetCurrentRoundByTime((BangHuiMatchType)param, now);
			this.StateMachineDict[param].SetCurrState(BHMatchStateMachine.StateType.SignUp, now, param);
			LogManager.WriteLog(LogTypes.Analysis, string.Format("BHMatch::MS_RankAnalyse_Enter MatchType:{0} To:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
			{
				(BangHuiMatchType)param,
				BHMatchStateMachine.StateType.SignUp,
				this.CurrentSeasonID_Gold,
				this.CurrentSeasonID_Rookie,
				this.FuBenMgrDict[param].Round
			}), null, true);
		}

		
		private void GenerateNextRoundPKInfo_Gold(DateTime now, int round)
		{
			if (this.BHMatchBHDataList_GoldJoin.Count == 8)
			{
				round = this.FixRound(BangHuiMatchType.BHMT_Begin, round);
				BHMatchGoldGroupConfig groupConfig = this.RuntimeData.GetBHMatchGoldGroupConfig(round);
				for (int i = 0; i < groupConfig.GroupUnion.Length; i++)
				{
					int group;
					int group2;
					BangHuiMatchUtils.SplitUnionGroup(groupConfig.GroupUnion[i], out group, out group2);
					this.GeneratePKInfo(1, this.CurrentSeasonID_Gold, round, this.BHMatchBHDataList_GoldJoin[group - 1], this.BHMatchBHDataList_GoldJoin[group2 - 1], 0);
				}
			}
		}

		
		private void GeneratePKInfo(byte type, int season, int round, BHMatchBHData bh1Data, BHMatchBHData bh2Data, byte result)
		{
			BangHuiMatchPKInfo pkInfo = null;
			if (type == 1)
			{
				pkInfo = this.BHMatchPKInfoList_Gold.V.Find((BangHuiMatchPKInfo x) => x.season == season && (int)x.round == round && x.bhid1 == bh1Data.bhid);
			}
			if (null == pkInfo)
			{
				pkInfo = new BangHuiMatchPKInfo();
				pkInfo.type = type;
				pkInfo.season = season;
				pkInfo.round = (byte)round;
				pkInfo.bhid1 = bh1Data.bhid;
				pkInfo.bhname1 = KuaFuServerManager.FormatName(bh1Data.bhname, bh1Data.zoneid_bh);
				pkInfo.zoneid1 = bh1Data.zoneid_bh;
				pkInfo.bhid2 = bh2Data.bhid;
				pkInfo.bhname2 = KuaFuServerManager.FormatName(bh2Data.bhname, bh2Data.zoneid_bh);
				pkInfo.zoneid2 = bh2Data.zoneid_bh;
				if (type == 1)
				{
					this.BHMatchPKInfoList_Gold.V.Add(pkInfo);
				}
			}
			pkInfo.result = result;
			this.Persistence.SaveBHMatchPKInfo(pkInfo);
			if (type == 1)
			{
				TimeUtil.AgeByNow(ref this.BHMatchPKInfoList_Gold.Age);
			}
		}

		
		private void HandlePromotion()
		{
			List<BangHuiMatchRankInfo> RookieRankList = null;
			if (this.CurrentSeasonID_Rookie == this.CurrentSeasonID_Gold)
			{
				this.Persistence.ReloadRankInfo(9, this.CurrentSeasonID_Rookie, this.LastSeasonID_Rookie);
				if (!this.BHMatchRankInfoDict.V.TryGetValue(9, out RookieRankList))
				{
					RookieRankList = new List<BangHuiMatchRankInfo>();
				}
			}
			else
			{
				this.Persistence.ReloadRankInfo(1, this.CurrentSeasonID_Rookie, this.LastSeasonID_Rookie);
				if (!this.BHMatchRankInfoDict.V.TryGetValue(1, out RookieRankList))
				{
					RookieRankList = new List<BangHuiMatchRankInfo>();
				}
			}
			if (RookieRankList.Count != 0)
			{
				KuaFuData<BHMatchBHData> rookieChampion = null;
				this.BHMatchBHDataDict_Rookie.TryGetValue(RookieRankList[0].Key, out rookieChampion);
				rookieChampion.V.hist_champion++;
				TimeUtil.AgeByNow(ref rookieChampion.Age);
				this.Persistence.SaveBHMatchBHData(rookieChampion.V, true, false);
			}
			for (int rankloop = 0; rankloop < RookieRankList.Count; rankloop++)
			{
				KuaFuData<BHMatchBHData> bhData = null;
				if (this.BHMatchBHDataDict_Rookie.TryGetValue(RookieRankList[rankloop].Key, out bhData))
				{
					if (bhData.V.best_record == 0 || bhData.V.best_record >= rankloop + 1)
					{
						bhData.V.best_record = rankloop + 1;
						TimeUtil.AgeByNow(ref bhData.Age);
						this.Persistence.SaveBHMatchBHData(bhData.V, false, false);
					}
				}
			}
			this.Persistence.ReloadRankInfo(8, this.CurrentSeasonID_Gold, this.LastSeasonID_Gold);
			this.FixCurGoldRankInfo();
			List<BangHuiMatchRankInfo> CurGoldRankList = null;
			if (!this.BHMatchRankInfoDict.V.TryGetValue(8, out CurGoldRankList))
			{
				CurGoldRankList = new List<BangHuiMatchRankInfo>();
			}
			for (int rankloop = 0; rankloop < CurGoldRankList.Count; rankloop++)
			{
				KuaFuData<BHMatchBHData> bhData = null;
				if (this.BHMatchBHDataDict_Gold.TryGetValue(CurGoldRankList[rankloop].Key, out bhData))
				{
					if (bhData.V.best_record == 0 || bhData.V.best_record >= rankloop + 1)
					{
						bhData.V.best_record = rankloop + 1;
						TimeUtil.AgeByNow(ref bhData.Age);
						this.Persistence.SaveBHMatchBHData(bhData.V, false, false);
					}
				}
			}
			this.BHMatchBHDataList_GoldJoin.Sort(delegate(BHMatchBHData left, BHMatchBHData right)
			{
				int rankLeft = CurGoldRankList.FindIndex((BangHuiMatchRankInfo x) => x.Key == left.bhid);
				int rankRight = CurGoldRankList.FindIndex((BangHuiMatchRankInfo x) => x.Key == right.bhid);
				int result;
				if (rankLeft > rankRight)
				{
					result = 1;
				}
				else if (rankLeft < rankRight)
				{
					result = -1;
				}
				else
				{
					result = 0;
				}
				return result;
			});
			for (int goldLoop = 0; goldLoop < this.BHMatchBHDataList_GoldJoin.Count; goldLoop++)
			{
				this.BHMatchBHDataList_GoldJoin[goldLoop].group = goldLoop + 1;
				this.Persistence.SaveBHMatchBHSeasonData(this.CurrentSeasonID_Gold, this.BHMatchBHDataList_GoldJoin[goldLoop], false, false);
			}
			foreach (BHMatchBHData item in this.BHMatchBHDataList_RookieJoin)
			{
				item.ChangeSeason();
			}
			foreach (BHMatchBHData item in this.BHMatchBHDataList_GoldJoin)
			{
				item.ChangeSeason();
			}
			int promotionNum = 0;
			if (this.BHMatchBHDataList_GoldJoin.Count == 0)
			{
				if (RookieRankList.Count >= 8)
				{
					promotionNum = 8;
				}
			}
			else
			{
				this.BHMatchBHDataList_GoldJoin[0].hist_champion++;
				this.Persistence.SaveBHMatchBHData(this.BHMatchBHDataList_GoldJoin[0], true, false);
				promotionNum = Math.Min(this.RuntimeData.RookiePromotionNum, RookieRankList.Count);
				if (promotionNum > 0 && this.BHMatchBHDataList_GoldJoin.Count > promotionNum)
				{
					int index = this.BHMatchBHDataList_GoldJoin.Count - promotionNum;
					for (int i = index; i < this.BHMatchBHDataList_GoldJoin.Count; i++)
					{
						LogManager.WriteLog(LogTypes.Analysis, string.Format("黄金赛淘汰,bhid={0} bhname={1}", this.BHMatchBHDataList_GoldJoin[i].bhid, this.BHMatchBHDataList_GoldJoin[i].bhname), null, true);
					}
					this.BHMatchBHDataList_GoldJoin.RemoveRange(index, promotionNum);
				}
			}
			KuaFuData<BHMatchBHData> bhDataGold = null;
			for (int rookieLoop = 0; rookieLoop < promotionNum; rookieLoop++)
			{
				KuaFuData<BHMatchBHData> bhDataRookie = null;
				this.BHMatchBHDataDict_Rookie.TryGetValue(RookieRankList[rookieLoop].Key, out bhDataRookie);
				if (!this.BHMatchBHDataDict_Gold.TryGetValue(bhDataRookie.V.bhid, out bhDataGold))
				{
					bhDataGold = new KuaFuData<BHMatchBHData>();
					bhDataGold.V.type = 1;
					bhDataGold.V.bhid = bhDataRookie.V.bhid;
					bhDataGold.V.bhname = bhDataRookie.V.bhname;
					bhDataGold.V.zoneid_bh = bhDataRookie.V.zoneid_bh;
					bhDataGold.V.rid = bhDataRookie.V.rid;
					bhDataGold.V.rname = bhDataRookie.V.rname;
					bhDataGold.V.zoneid_r = bhDataRookie.V.zoneid_r;
					this.BHMatchBHDataDict_Gold[bhDataGold.V.bhid] = bhDataGold;
					this.Persistence.SaveBHMatchBHData(bhDataGold.V, true, true);
				}
				bhDataGold.V.group = this.BHMatchBHDataList_GoldJoin.Count + 1;
				this.BHMatchBHDataList_GoldJoin.Add(bhDataGold.V);
				LogManager.WriteLog(LogTypes.Analysis, string.Format("新星赛晋级,bhid={0} bhname={1} group={2}", bhDataGold.V.bhid, bhDataGold.V.bhname, bhDataGold.V.group), null, true);
			}
			this.BHMatchBHDataList_RookieJoinLast = new List<BHMatchBHData>(this.BHMatchBHDataList_RookieJoin);
			this.BHMatchBHDataList_RookieJoin.Clear();
		}

		
		public BHMatchSyncData SyncData_BHMatch(long ageRank, long agePKInfo, long ageChampion)
		{
			BHMatchSyncData SyncData = new BHMatchSyncData();
			try
			{
				lock (this.Mutex)
				{
					BHMatchFuBenMgrData fubenMgrGold = null;
					BHMatchFuBenMgrData fubenMgrRookie = null;
					this.FuBenMgrDict.TryGetValue(1, out fubenMgrGold);
					this.FuBenMgrDict.TryGetValue(2, out fubenMgrRookie);
					SyncData.LastSeasonID_Gold = this.LastSeasonID_Gold;
					SyncData.CurSeasonID_Gold = this.CurrentSeasonID_Gold;
					SyncData.LastSeasonID_Rookie = this.LastSeasonID_Rookie;
					SyncData.CurSeasonID_Rookie = this.CurrentSeasonID_Rookie;
					SyncData.RoundGoldReal = (int)fubenMgrGold.Round;
					SyncData.RoundRookieReal = (int)fubenMgrRookie.Round;
					SyncData.BHMatchRankInfoDict.Age = this.BHMatchRankInfoDict.Age;
					SyncData.BHMatchPKInfoList_Gold.Age = this.BHMatchPKInfoList_Gold.Age;
					SyncData.BHMatchChampionRoleData_Gold.Age = this.BHMatchChampionRoleData_Gold.Age;
					if (ageRank != this.BHMatchRankInfoDict.Age)
					{
						SyncData.BHMatchRankInfoDict = this.BHMatchRankInfoDict;
					}
					if (agePKInfo != this.BHMatchPKInfoList_Gold.Age)
					{
						SyncData.BHMatchPKInfoList_Gold = this.BHMatchPKInfoList_Gold;
					}
					if (ageChampion != this.BHMatchChampionRoleData_Gold.Age)
					{
						SyncData.BHMatchChampionRoleData_Gold = this.BHMatchChampionRoleData_Gold;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "BangHuiMatchService.SyncData_BHMatch failed!", ex, true);
			}
			return SyncData;
		}

		
		public string GetKuaFuGameState_BHMatch(int bhid)
		{
			string result = "";
			try
			{
				lock (this.Mutex)
				{
					BangHuiMatchType matchType = this.GetMatchTypeByBhid(bhid);
					int signState = -4005;
					if (matchType == BangHuiMatchType.Rookie)
					{
						if (!this.BHMatchBHDataList_RookieJoin.Exists((BHMatchBHData x) => x.bhid == bhid))
						{
							signState = -4008;
						}
						else if (this.StateMachineDict[2].GetCurrState() == BHMatchStateMachine.StateType.NotifyEnter || this.StateMachineDict[2].GetCurrState() == BHMatchStateMachine.StateType.GameStart)
						{
							if (!this.FuBenMgrDict[2].BHidVsGameId.ContainsKey(bhid))
							{
								signState = -4009;
							}
						}
					}
					result = string.Format("{0}:{1}", (int)matchType, signState);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "BangHuiMatchService.GetKuaFuGameState_BHMatch failed!", ex, true);
			}
			return result;
		}

		
		public bool CheckRookieJoinLast_BHMatch(int bhid)
		{
			try
			{
				lock (this.Mutex)
				{
					if (!this.BHMatchBHDataList_RookieJoinLast.Exists((BHMatchBHData x) => x.bhid == bhid))
					{
						return false;
					}
					return true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "BangHuiMatchService.GetKuaFuGameState_BHMatch failed!", ex, true);
			}
			return false;
		}

		
		public int RookieSignUp_BHMatch(int bhid, int zoneid_bh, string bhname, int rid, string rname, int zoneid_r)
		{
			int result = 0;
			try
			{
				lock (this.Mutex)
				{
					DateTime now = TimeUtil.NowDateTime();
					if (!this.RuntimeData.CheckOpenState(now))
					{
						result = -11004;
						return result;
					}
					BHMatchConfig matchConfig = this.RuntimeData.GetBHMatchConfig(2);
					if (null == matchConfig)
					{
						result = -3;
						return result;
					}
					DateTime closeTm = this.RuntimeData.GetActivityCloseTm();
					int toCloseDays = (int)(closeTm - now).TotalDays;
					if (toCloseDays < 7)
					{
						int weekRound = 0;
						for (int dayLoop = 0; dayLoop < toCloseDays; dayLoop++)
						{
							bool haveMatch = false;
							DateTime addNowTm = now.AddDays((double)dayLoop);
							for (int i = 0; i < matchConfig.TimePoints.Count - 1; i += 2)
							{
								TimeSpan myTmp = matchConfig.TimePoints[i];
								if (i == 0 && addNowTm.DayOfWeek == (DayOfWeek)myTmp.Days && addNowTm.TimeOfDay < myTmp.Subtract(new TimeSpan(0, 0, matchConfig.ApplyOverTime)))
								{
									haveMatch = true;
									break;
								}
								if (addNowTm.DayOfWeek == (DayOfWeek)myTmp.Days)
								{
									haveMatch = true;
									break;
								}
							}
							if (haveMatch)
							{
								weekRound++;
							}
						}
						if (weekRound == 0)
						{
							result = -2001;
							return result;
						}
					}
					DateTime curSeasonTm = BangHuiMatchUtils.GetSeasonDateTm(this.CurrentSeasonID_Rookie);
					if (now < curSeasonTm || this.StateMachineDict[2].GetCurrState() != BHMatchStateMachine.StateType.SignUp)
					{
						result = -2001;
						return result;
					}
					BangHuiMatchType matchType = this.GetMatchTypeByBhid(bhid);
					if (matchType == BangHuiMatchType.BHMT_Begin)
					{
						result = -4005;
						return result;
					}
					if (this.BHMatchBHDataList_RookieJoin.Exists((BHMatchBHData x) => x.bhid == bhid))
					{
						result = -4005;
						return result;
					}
					KuaFuData<BHMatchBHData> bhData = null;
					if (!this.BHMatchBHDataDict_Rookie.TryGetValue(bhid, out bhData))
					{
						bhData = new KuaFuData<BHMatchBHData>();
						bhData.V.type = (int)matchType;
						bhData.V.bhid = bhid;
						bhData.V.bhname = bhname;
						bhData.V.zoneid_bh = zoneid_bh;
						bhData.V.rid = rid;
						bhData.V.rname = rname;
						bhData.V.zoneid_r = zoneid_r;
						TimeUtil.AgeByNow(ref bhData.Age);
						this.BHMatchBHDataDict_Rookie[bhid] = bhData;
						this.Persistence.SaveBHMatchBHData(bhData.V, true, true);
					}
					this.BHMatchBHDataList_RookieJoin.Add(bhData.V);
					this.Persistence.SaveBHMatchBHSeasonData(this.CurrentSeasonID_Rookie, bhData.V, true, true);
					return result;
				}
			}
			catch (Exception ex)
			{
				result = -11;
				LogManager.WriteLog(LogTypes.Error, "BangHuiMatchService.RookieSignUp_BHMatch failed!", ex, true);
			}
			return result;
		}

		
		public BHMatchFuBenData GetFuBenDataByBhid_BHMatch(int bhid)
		{
			BHMatchFuBenData fuBenData = null;
			try
			{
				lock (this.Mutex)
				{
					BangHuiMatchType type = this.GetMatchTypeByBhid(bhid);
					int gameid = 0;
					this.FuBenMgrDict[(int)type].BHidVsGameId.TryGetValue(bhid, out gameid);
					this.FuBenMgrDict[(int)type].FuBenDataDict.TryGetValue(gameid, out fuBenData);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "BangHuiMatchService.GetFuBenDataByBhid_BHMatch failed!", ex, true);
			}
			return fuBenData;
		}

		
		public BHMatchFuBenData GetFuBenDataByGameId_BHMatch(int gameid)
		{
			BHMatchFuBenData fuBenData = null;
			try
			{
				lock (this.Mutex)
				{
					for (int typeloop = 1; typeloop <= 2; typeloop++)
					{
						this.FuBenMgrDict[typeloop].FuBenDataDict.TryGetValue(gameid, out fuBenData);
						if (null != fuBenData)
						{
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "BangHuiMatchService.GetFuBenDataByGameId_BHMatch failed!", ex, true);
			}
			return fuBenData;
		}

		
		public KuaFuCmdData GetBHDataByBhid_BHMatch(int type, int bhid, long age)
		{
			try
			{
				lock (this.Mutex)
				{
					KuaFuData<BHMatchBHData> bhData = null;
					if (type == 1)
					{
						this.BHMatchBHDataDict_Gold.TryGetValue(bhid, out bhData);
					}
					if (type == 2)
					{
						this.BHMatchBHDataDict_Rookie.TryGetValue(bhid, out bhData);
					}
					if (bhData == null)
					{
						return null;
					}
					if (age != bhData.Age)
					{
						return new KuaFuCmdData
						{
							Age = bhData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<BHMatchBHData>(bhData.V)
						};
					}
					return new KuaFuCmdData
					{
						Age = bhData.Age
					};
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "BangHuiMatchService.GetBHDataByBhid_BHMatch failed!", ex, true);
			}
			return null;
		}

		
		public int GameFuBenComplete_BHMatch(BangHuiMatchStatisticalData data)
		{
			int result = 0;
			try
			{
				lock (this.Mutex)
				{
					KuaFuData<BHMatchBHData> bh1Data = null;
					KuaFuData<BHMatchBHData> bh2Data = null;
					BangHuiMatchType matchType = this.GetMatchTypeByBhid(data.bhid1);
					if (matchType == BangHuiMatchType.BHMT_Begin)
					{
						this.BHMatchBHDataDict_Gold.TryGetValue(data.bhid1, out bh1Data);
						this.BHMatchBHDataDict_Gold.TryGetValue(data.bhid2, out bh2Data);
					}
					else
					{
						this.BHMatchBHDataDict_Rookie.TryGetValue(data.bhid1, out bh1Data);
						this.BHMatchBHDataDict_Rookie.TryGetValue(data.bhid2, out bh2Data);
					}
					if (bh1Data == null || null == bh2Data)
					{
						result = -2;
						return result;
					}
					BHMatchFuBenData fubenData = null;
					this.FuBenMgrDict[(int)matchType].FuBenDataDict.TryGetValue(data.GameId, out fubenData);
					if (null == fubenData)
					{
						result = -4000;
						return result;
					}
					ClientAgentManager.Instance().RemoveKfFuben(this.GameType, fubenData.ServerId, (long)data.GameId);
					this.FuBenMgrDict[(int)matchType].FuBenDataDict.Remove(data.GameId);
					bh1Data.V.hist_play++;
					bh2Data.V.hist_play++;
					bh1Data.V.hist_kill += data.kill1;
					bh2Data.V.hist_kill += data.kill2;
					bh1Data.V.hist_score += data.score1;
					bh2Data.V.hist_score += data.score2;
					bh1Data.V.cur_score += data.score1;
					bh2Data.V.cur_score += data.score2;
					if (!string.IsNullOrEmpty(data.bhname1))
					{
						bh1Data.V.bhname = data.bhname1;
					}
					if (!string.IsNullOrEmpty(data.bhname2))
					{
						bh2Data.V.bhname = data.bhname2;
					}
					if (!string.IsNullOrEmpty(data.rname1))
					{
						bh1Data.V.rid = data.rid1;
						bh1Data.V.rname = data.rname1;
						bh1Data.V.zoneid_r = data.zoneid_r1;
					}
					if (!string.IsNullOrEmpty(data.rname2))
					{
						bh2Data.V.rid = data.rid2;
						bh2Data.V.rname = data.rname2;
						bh2Data.V.zoneid_r = data.zoneid_r2;
					}
					if (data.result == 1)
					{
						bh1Data.V.cur_win++;
						bh1Data.V.hist_win++;
						if (data.bullshit)
						{
							bh1Data.V.hist_bullshit++;
						}
					}
					else if (data.result == 2)
					{
						bh2Data.V.cur_win++;
						bh2Data.V.hist_win++;
						if (data.bullshit)
						{
							bh2Data.V.hist_bullshit++;
						}
					}
					if (null != data.bzroledata1)
					{
						this.Persistence.SaveBHMatchRolesData((int)matchType, data.rid1, data.rname1, data.zoneid_r1, data.bhid1, data.bzroledata1);
					}
					if (null != data.bzroledata2)
					{
						this.Persistence.SaveBHMatchRolesData((int)matchType, data.rid2, data.rname2, data.zoneid_r2, data.bhid2, data.bzroledata2);
					}
					TimeUtil.AgeByNow(ref bh1Data.Age);
					TimeUtil.AgeByNow(ref bh2Data.Age);
					this.Persistence.SaveBHMatchBHData(bh1Data.V, false, data.kill1 != 0);
					this.Persistence.SaveBHMatchBHData(bh2Data.V, false, data.kill2 != 0);
					int CurrentSeasonID = (matchType == BangHuiMatchType.BHMT_Begin) ? this.CurrentSeasonID_Gold : this.CurrentSeasonID_Rookie;
					this.Persistence.SaveBHMatchBHSeasonData(CurrentSeasonID, bh1Data.V, data.result == 1, data.score1 != 0);
					this.Persistence.SaveBHMatchBHSeasonData(CurrentSeasonID, bh2Data.V, data.result == 2, data.score2 != 0);
					foreach (BHMatchRoleData rData in data.roleStatisticalData.Values)
					{
						if (rData.rid != 0 && !string.IsNullOrEmpty(rData.rname))
						{
							rData.type = (int)matchType;
							this.Persistence.SaveBHMatchRolesData((int)matchType, rData.rid, rData.rname, rData.zoneid, rData.bhid, null);
							this.Persistence.SaveBHMatchRolesSeasonData(CurrentSeasonID, rData, 0 != rData.mvp, 0 != rData.kill);
						}
					}
					this.GeneratePKInfo((byte)matchType, CurrentSeasonID, (int)this.FuBenMgrDict[(int)matchType].Round, bh1Data.V, bh2Data.V, data.result);
					LogManager.WriteLog(LogTypes.Analysis, string.Format("BHMatch::GameFuBenComplete_BHMatch GameID:{14} Type:{15} SeasonID:{0} Round:{1}\r\n                            bhid1:{2} bhname1:S{3}·{4} rcount1:{5} score1:{6} | bhid2:{7} bhname2:S{8}·{9} rcount2:{10} score2:{11} | result:{12} | templechg:{13}", new object[]
					{
						CurrentSeasonID,
						this.FuBenMgrDict[(int)matchType].Round,
						data.bhid1,
						bh1Data.V.zoneid_bh,
						bh1Data.V.bhname,
						data.rolecount1,
						data.score1,
						data.bhid2,
						bh2Data.V.zoneid_bh,
						bh2Data.V.bhname,
						data.rolecount2,
						data.score2,
						data.result,
						data.templechg,
						data.GameId,
						matchType
					}), null, true);
					return result;
				}
			}
			catch (Exception ex)
			{
				result = -11;
				LogManager.WriteLog(LogTypes.Error, "BangHuiMatchService.GameFuBenComplete_BHMatch failed!", ex, true);
			}
			return result;
		}

		
		public int RemoveBangHui_BHMatch(int bhid)
		{
			return 0;
		}

		
		private static BangHuiMatchService _instance = new BangHuiMatchService();

		
		public readonly GameTypes GameType = GameTypes.BangHuiMatch;

		
		public readonly GameTypes EvItemGameType = GameTypes.TianTi;

		
		private object Mutex = new object();

		
		private BangHuiMatchCommonData RuntimeData = new BangHuiMatchCommonData();

		
		public BangHuiMatchPersistence Persistence = BangHuiMatchPersistence.Instance;

		
		public KuaFuCmdData BHMatchChampionRoleData_Gold = new KuaFuCmdData();

		
		private int CurrentSeasonID_Gold = 0;

		
		private int LastSeasonID_Gold = 0;

		
		private int CurrentSeasonID_Rookie = 0;

		
		private int LastSeasonID_Rookie = 0;

		
		private Dictionary<int, BHMatchFuBenMgrData> FuBenMgrDict = new Dictionary<int, BHMatchFuBenMgrData>();

		
		private Dictionary<int, BHMatchStateMachine> StateMachineDict = new Dictionary<int, BHMatchStateMachine>();

		
		private uint UpdateFrameCount = 0U;

		
		private DateTime LastUpdateTime = DateTime.MinValue;
	}
}
