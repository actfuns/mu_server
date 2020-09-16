using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace Remoting
{
	
	public class Zork5v5Service
	{
		
		public static Zork5v5Service Instance()
		{
			return Zork5v5Service._instance;
		}

		
		public void InitConfig()
		{
			try
			{
				lock (this.MutexConfig)
				{
					DateTime.TryParse(KuaFuServerManager.systemParamsList.GetParamValueByName("ZorkStartTime"), out this.ZorkStartTime);
					this.SceneDataDict.Clear();
					string fileName = "Config/ZorkActivityRules.xml";
					string fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					XElement xml = ConfigHelper.Load(fullPathFileName);
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement node in xmlItems)
					{
						ZorkBattleSceneInfo sceneItem = new ZorkBattleSceneInfo();
						int id = (int)ConfigHelper.GetElementAttributeValueLong(node, "ID", 0L);
						int mapCode = (int)ConfigHelper.GetElementAttributeValueLong(node, "MapCode", 0L);
						sceneItem.Id = id;
						sceneItem.MapCode = mapCode;
						sceneItem.MaxEnterNum = (int)ConfigHelper.GetElementAttributeValueLong(node, "MaxEnterNum", 0L);
						sceneItem.PrepareSecs = (int)ConfigHelper.GetElementAttributeValueLong(node, "PrepareSecs", 0L);
						sceneItem.FightingSecs = (int)ConfigHelper.GetElementAttributeValueLong(node, "FightingSecs", 0L);
						sceneItem.ClearRolesSecs = (int)ConfigHelper.GetElementAttributeValueLong(node, "ClearRolesSecs", 0L);
						sceneItem.BattleSignSecs = (int)ConfigHelper.GetElementAttributeValueLong(node, "BattleSignSecs", 0L);
						sceneItem.SeasonFightRound = (int)ConfigHelper.GetElementAttributeValueLong(node, "SeasonFightDay", 0L);
						string[] fields = node.Attribute("TimePoints").Value.Split(new char[]
						{
							',',
							'-',
							'|'
						});
						for (int i = 0; i < fields.Length; i += 3)
						{
							TimeSpan dayPart = new TimeSpan(Convert.ToInt32(fields[i]), 0, 0, 0);
							TimeSpan time = DateTime.Parse(fields[i + 1]).TimeOfDay.Add(dayPart);
							TimeSpan time2 = DateTime.Parse(fields[i + 2]).TimeOfDay.Add(dayPart);
							sceneItem.TimePoints.Add(time);
							sceneItem.TimePoints.Add(time2);
						}
						for (int i = 0; i < sceneItem.TimePoints.Count; i++)
						{
							TimeSpan ts = new TimeSpan(sceneItem.TimePoints[i].Hours, sceneItem.TimePoints[i].Minutes, sceneItem.TimePoints[i].Seconds);
							sceneItem.SecondsOfDay.Add(ts.TotalSeconds);
						}
						for (int i = 0; i < sceneItem.TimePoints.Count; i++)
						{
							TimeSpan ts = new TimeSpan(sceneItem.TimePoints[i].Hours, sceneItem.TimePoints[i].Minutes, sceneItem.TimePoints[i].Seconds);
							sceneItem.SecondsOfDay.Add(ts.TotalSeconds);
						}
						this.SceneDataDict[id] = sceneItem;
						this.SeasonWeeks = Math.Max(this.SeasonWeeks, (int)Math.Ceiling((double)sceneItem.SeasonFightRound / (double)(sceneItem.TimePoints.Count / 2)));
					}
					this.ZorkLevelRangeList.Clear();
					fileName = "Config/ZorkDanAward.xml";
					fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					xml = ConfigHelper.Load(fullPathFileName);
					xmlItems = xml.Elements();
					foreach (XElement node in xmlItems)
					{
						ZorkBattleAwardConfig item = new ZorkBattleAwardConfig();
						item.ID = (int)ConfigHelper.GetElementAttributeValueLong(node, "ID", 0L);
						item.RankValue = (int)ConfigHelper.GetElementAttributeValueLong(node, "RankValue", 0L);
						item.WinRankValue = (int)ConfigHelper.GetElementAttributeValueLong(node, "WinRankValue", 0L);
						item.LoseRankValue = (int)ConfigHelper.GetElementAttributeValueLong(node, "LoseRankValue", 0L);
						this.ZorkLevelRangeList.Add(item);
						this.ZorkLevelRangeList.Sort(delegate(ZorkBattleAwardConfig left, ZorkBattleAwardConfig righit)
						{
							int result;
							if (left.ID > righit.ID)
							{
								result = -1;
							}
							else if (left.ID > righit.ID)
							{
								result = 1;
							}
							else
							{
								result = 0;
							}
							return result;
						});
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		
		public void LoadDatabase(DateTime now)
		{
			try
			{
				lock (this.Mutex)
				{
					this.CurrentSeasonID = this.Persistence.LoadZorkSeasonID();
					bool InitFirstSeason = this.CurrentSeasonID == 0;
					this.CurrentSeasonID = this.ComputeCurrentSeasonID(now, this.CurrentSeasonID);
					if (InitFirstSeason)
					{
						this.Persistence.SaveZorkSeasonID(this.CurrentSeasonID);
					}
					this.CurrentRound = this.GetCurrentRoundByTime(now, this.CurrentSeasonID);
					this.TopZhanDui = this.Persistence.LoadZorkTopZhanDui();
					this.UpdateTopZhanDuiInfo();
					this.TopKiller = this.Persistence.LoadZorkTopKiller();
					this.ReloadRankInfo(0, this.ZorkBattleRankInfoDict);
					this.ReloadRankInfo(1, this.ZorkBattleRankInfoDict);
					this.InitFuBenManagerData(now);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "Zork5v5Service.LoadDatabase failed!", ex, true);
			}
		}

		
		public bool CheckOpenState(DateTime now)
		{
			return !(now < this.ZorkStartTime);
		}

		
		private void UpdateTopZhanDuiInfo()
		{
			this.TopZhanDuiName = "";
			if (this.TopZhanDui > 0)
			{
				TianTi5v5ZhanDuiData zhanduiData = TianTi5v5Service.GetZhanDuiData(this.TopZhanDui);
				if (null != zhanduiData)
				{
					this.TopZhanDuiName = KuaFuServerManager.FormatName(zhanduiData.ZoneID, zhanduiData.ZhanDuiName);
				}
			}
		}

		
		private int GetCurrentRoundByTime(DateTime now, int CurrentSeasonID)
		{
			int result;
			if (!this.CheckOpenState(now))
			{
				result = 0;
			}
			else if (!KuaFuServerManager.IsGongNengOpened(114))
			{
				result = 0;
			}
			else
			{
				lock (this.MutexConfig)
				{
					ZorkBattleSceneInfo sceneConfig = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
					if (null == sceneConfig)
					{
						result = 0;
					}
					else
					{
						DateTime curSeasonTime = ZorkBattleUtils.GetSeasonDateTm(CurrentSeasonID);
						if (now < curSeasonTime)
						{
							result = 1;
						}
						else
						{
							TimeSpan fromMonday = new TimeSpan((int)now.DayOfWeek, now.Hour, now.Minute, now.Second);
							if (fromMonday.Days == 0)
							{
								fromMonday += new TimeSpan(7, 0, 0, 0);
							}
							int weekRound = 0;
							for (int i = 0; i < sceneConfig.TimePoints.Count - 1; i += 2)
							{
								TimeSpan myTmp = sceneConfig.TimePoints[i + 1];
								if (myTmp.Days == 0)
								{
									myTmp += new TimeSpan(7, 0, 0, 0);
								}
								if (fromMonday > myTmp)
								{
									weekRound++;
								}
							}
							int week = (now - curSeasonTime).Days % (7 * this.SeasonWeeks) / 7;
							int round = week * sceneConfig.TimePoints.Count / 2 + weekRound + 1;
							result = Math.Min(round, sceneConfig.SeasonFightRound + 1);
						}
					}
				}
			}
			return result;
		}

		
		private int ComputeCurrentSeasonID(DateTime now, int CurrentSeasonID)
		{
			int result;
			if (!this.CheckOpenState(now))
			{
				result = 0;
			}
			else if (!KuaFuServerManager.IsGongNengOpened(114))
			{
				result = 0;
			}
			else
			{
				lock (this.MutexConfig)
				{
					DateTime OpenTime = ZorkBattleUtils.GetSeasonDateTm(CurrentSeasonID);
					if (OpenTime == DateTime.MinValue)
					{
						TimeSpan start = TimeSpan.MaxValue;
						foreach (ZorkBattleSceneInfo item in this.SceneDataDict.Values)
						{
							for (int i = 0; i < item.TimePoints.Count - 1; i += 2)
							{
								TimeSpan startTm = item.TimePoints[i];
								if (startTm.Days == 0)
								{
									startTm += new TimeSpan(7, 0, 0, 0);
								}
								if (startTm < start)
								{
									start = startTm;
								}
							}
						}
						start -= new TimeSpan(1, 0, 0, 0);
						int spanday = TimeUtil.NowDateTime().DayOfWeek - DayOfWeek.Monday;
						spanday = ((spanday >= 0) ? (-spanday) : (-(7 + spanday)));
						TimeSpan nowfromMonday = new TimeSpan(Math.Abs(spanday), now.Hour, now.Minute, now.Second);
						if (nowfromMonday < start)
						{
							OpenTime = TimeUtil.NowDateTime().AddDays((double)spanday);
						}
						else
						{
							OpenTime = TimeUtil.NowDateTime().AddDays((double)(spanday + 7));
						}
					}
					else if ((now - OpenTime).Days >= this.SeasonWeeks * 7)
					{
						int spanday = TimeUtil.NowDateTime().DayOfWeek - DayOfWeek.Monday;
						spanday = ((spanday >= 0) ? (-spanday) : (-(7 + spanday)));
						OpenTime = TimeUtil.NowDateTime().AddDays((double)spanday);
					}
					result = ZorkBattleUtils.MakeSeason(OpenTime);
				}
			}
			return result;
		}

		
		public bool ReloadRankInfo(int rankType, KuaFuData<Dictionary<int, List<KFZorkRankInfo>>> ZorkBattleRankInfoDict)
		{
			bool ret = true;
			List<KFZorkRankInfo> rankList = new List<KFZorkRankInfo>();
			if (!ZorkBattleRankInfoDict.V.TryGetValue(rankType, out rankList))
			{
				rankList = (ZorkBattleRankInfoDict.V[rankType] = new List<KFZorkRankInfo>());
			}
			else
			{
				rankList.Clear();
			}
			try
			{
				if (rankType == 0)
				{
					TianTi5v5Service.CalZorkBattleRankTeamJiFen(rankList);
				}
				else
				{
					ret = this.Persistence.LoadZorkBattleRankInfo(rankType, rankList);
				}
				TimeUtil.AgeByNow(ref ZorkBattleRankInfoDict.Age);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				return false;
			}
			return ret;
		}

		
		private void InitFuBenManagerData(DateTime now)
		{
			this.LastUpdateTime = now;
			this.FuBenDataDict.Clear();
			this.ZhanDuiIDVsGameIDDict.Clear();
			this.BybZhanDuiIDSet.Clear();
			this.StateMachine = new Zork5v5StateMachine();
			this.StateMachine.Install(new Zork5v5StateMachine.StateHandler(Zork5v5StateMachine.StateType.Init, null, new Action<DateTime, int>(this.MS_Init_Update), null));
			this.StateMachine.Install(new Zork5v5StateMachine.StateHandler(Zork5v5StateMachine.StateType.SignUp, null, new Action<DateTime, int>(this.MS_SignUp_Update), null));
			this.StateMachine.Install(new Zork5v5StateMachine.StateHandler(Zork5v5StateMachine.StateType.PrepareGame, null, new Action<DateTime, int>(this.MS_PrepareGame_Update), null));
			this.StateMachine.Install(new Zork5v5StateMachine.StateHandler(Zork5v5StateMachine.StateType.NotifyEnter, null, new Action<DateTime, int>(this.MS_NotifyEnter_Update), null));
			this.StateMachine.Install(new Zork5v5StateMachine.StateHandler(Zork5v5StateMachine.StateType.GameStart, null, new Action<DateTime, int>(this.MS_GameStart_Update), null));
			this.StateMachine.Install(new Zork5v5StateMachine.StateHandler(Zork5v5StateMachine.StateType.RankAnalyse, new Action<DateTime, int>(this.MS_RankAnalyse_Enter), new Action<DateTime, int>(this.MS_RankAnalyse_Update), null));
			this.StateMachine.SetCurrState(Zork5v5StateMachine.StateType.Init, TimeUtil.NowDateTime(), 0);
			this.StateMachine.Tick(now, 0);
		}

		
		private void MS_Init_Update(DateTime now, int param)
		{
			if (this.CheckOpenState(now))
			{
				if (KuaFuServerManager.IsGongNengOpened(114))
				{
					ZorkBattleSceneInfo matchConfig = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
					Zork5v5StateMachine.StateType GameState = Zork5v5StateMachine.StateType.Init;
					for (int i = 0; i < matchConfig.TimePoints.Count - 1; i += 2)
					{
						if (now.DayOfWeek == (DayOfWeek)matchConfig.TimePoints[i].Days)
						{
							int RoundSeconds = matchConfig.BattleSignSecs + matchConfig.PrepareSecs + matchConfig.FightingSecs + matchConfig.ClearRolesSecs;
							int MatchPerRound = (int)(matchConfig.SecondsOfDay[i + 1] - matchConfig.SecondsOfDay[i]) / RoundSeconds;
							for (int matchloop = 0; matchloop < MatchPerRound; matchloop++)
							{
								int signSeconds = (int)matchConfig.SecondsOfDay[i] + RoundSeconds * matchloop;
								int startSeconds = signSeconds + matchConfig.BattleSignSecs;
								int endSeconds = startSeconds + RoundSeconds - matchConfig.BattleSignSecs;
								if (now.TimeOfDay.TotalSeconds >= (double)signSeconds && now.TimeOfDay.TotalSeconds < (double)startSeconds)
								{
									GameState = Zork5v5StateMachine.StateType.SignUp;
								}
								else if (now.TimeOfDay.TotalSeconds >= (double)startSeconds && now.TimeOfDay.TotalSeconds < (double)endSeconds)
								{
									GameState = Zork5v5StateMachine.StateType.GameStart;
								}
							}
						}
					}
					if (this.CurrentSeasonID > 0)
					{
						if (this.CurrentSeasonID != this.ComputeCurrentSeasonID(now, this.CurrentSeasonID))
						{
							GameState = Zork5v5StateMachine.StateType.RankAnalyse;
						}
					}
					else
					{
						this.CurrentSeasonID = this.ComputeCurrentSeasonID(now, this.CurrentSeasonID);
						this.Persistence.SaveZorkSeasonID(this.CurrentSeasonID);
						this.CurrentRound = this.GetCurrentRoundByTime(now, this.CurrentSeasonID);
					}
					if (GameState != Zork5v5StateMachine.StateType.Init)
					{
						this.StateMachine.SetCurrState(GameState, now, param);
						LogManager.WriteLog(LogTypes.Analysis, string.Format("Zork::MS_Init_Update To:{0} SeasonID:{1} Round:{2}", GameState, this.CurrentSeasonID, this.CurrentRound), null, true);
					}
				}
			}
		}

		
		private void MS_SignUp_Update(DateTime now, int param)
		{
			ZorkBattleSceneInfo matchConfig = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
			Zork5v5StateMachine.StateType GameState = Zork5v5StateMachine.StateType.None;
			for (int i = 0; i < matchConfig.TimePoints.Count - 1; i += 2)
			{
				if (now.DayOfWeek == (DayOfWeek)matchConfig.TimePoints[i].Days)
				{
					int RoundSeconds = matchConfig.BattleSignSecs + matchConfig.PrepareSecs + matchConfig.FightingSecs + matchConfig.ClearRolesSecs;
					int MatchPerRound = (int)(matchConfig.SecondsOfDay[i + 1] - matchConfig.SecondsOfDay[i]) / RoundSeconds;
					for (int matchloop = 0; matchloop < MatchPerRound; matchloop++)
					{
						int signSeconds = (int)matchConfig.SecondsOfDay[i] + RoundSeconds * matchloop;
						int startSeconds = signSeconds + matchConfig.BattleSignSecs;
						int endSeconds = startSeconds + RoundSeconds - matchConfig.BattleSignSecs;
						if (this.LastUpdateTime.TimeOfDay.TotalSeconds < (double)startSeconds && now.TimeOfDay.TotalSeconds >= (double)startSeconds)
						{
							GameState = Zork5v5StateMachine.StateType.PrepareGame;
						}
					}
				}
			}
			if (GameState == Zork5v5StateMachine.StateType.PrepareGame)
			{
				this.StateMachine.SetCurrState(GameState, now, param);
				LogManager.WriteLog(LogTypes.Analysis, string.Format("Zork::MS_SignUp_Update To:{0} SeasonID:{1} Round:{2}", GameState, this.CurrentSeasonID, this.CurrentRound), null, true);
			}
		}

		
		private void MS_PrepareGame_Update(DateTime now, int param)
		{
			ZorkBattleSceneInfo matchConfig = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
			List<KF5v5PiPeiTeam> SignUpTeamList = this.PiPeiDict.Values.ToList<KF5v5PiPeiTeam>();
			if (SignUpTeamList.Count < matchConfig.MaxEnterNum - 1)
			{
				this.StateMachine.SetCurrState(Zork5v5StateMachine.StateType.NotifyEnter, now, param);
				LogManager.WriteLog(LogTypes.Analysis, string.Format("Zork::MS_PrepareGame_Update To:{0} SeasonID:{1} Round:{2}", Zork5v5StateMachine.StateType.NotifyEnter, this.CurrentSeasonID, this.CurrentRound), null, true);
			}
			else
			{
				List<KF5v5PiPeiTeam> TeamStandByList = new List<KF5v5PiPeiTeam>(SignUpTeamList);
				if (TeamStandByList.Count > 0)
				{
					Random r = new Random((int)now.Ticks);
					int i = 0;
					while (TeamStandByList.Count > 0 && i < TeamStandByList.Count * 2)
					{
						int idx = r.Next(0, TeamStandByList.Count);
						int idx2 = r.Next(0, TeamStandByList.Count);
						KF5v5PiPeiTeam tmp = TeamStandByList[idx];
						TeamStandByList[idx] = TeamStandByList[idx2];
						TeamStandByList[idx2] = tmp;
						i++;
					}
				}
				int currIdx = 0;
				for (int i = 0; i < TeamStandByList.Count / matchConfig.MaxEnterNum; i++)
				{
					int zhanduiCount = 0;
					int side = 0;
					KuaFu5v5FuBenData ZorkFuBenData = new KuaFu5v5FuBenData();
					for (int teamIdx = currIdx; teamIdx < currIdx + matchConfig.MaxEnterNum; teamIdx++)
					{
						KF5v5PiPeiTeam team = TeamStandByList[teamIdx];
						if (ZorkFuBenData.AddZhanDuiWithName(team.TeamID, team.ZhanDuiName, ref zhanduiCount, ref side))
						{
							TianTi5v5ZhanDuiData teamData = TianTi5v5Service.GetZhanDuiData(team.TeamID);
							if (null != teamData)
							{
								foreach (TianTi5v5ZhanDuiRoleData role in teamData.teamerList)
								{
									KuaFuFuBenRoleData kuaFuFuBenRoleData = new KuaFuFuBenRoleData
									{
										ServerId = team.ServerID,
										RoleId = role.RoleID,
										Side = side
									};
									ZorkFuBenData.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData, team.TeamID);
								}
							}
						}
					}
					currIdx += matchConfig.MaxEnterNum;
					this.AssginKfFuben(ZorkFuBenData);
				}
				if (currIdx > 0)
				{
					TeamStandByList.RemoveRange(0, currIdx);
				}
				if (TeamStandByList.Count >= matchConfig.MaxEnterNum - 1)
				{
					int zhanduiCount = 0;
					int side = 0;
					KuaFu5v5FuBenData ZorkFuBenData = new KuaFu5v5FuBenData();
					for (int i = 0; i < TeamStandByList.Count; i++)
					{
						KF5v5PiPeiTeam team = TeamStandByList[i];
						if (ZorkFuBenData.AddZhanDuiWithName(team.TeamID, team.ZhanDuiName, ref zhanduiCount, ref side))
						{
							TianTi5v5ZhanDuiData teamData = TianTi5v5Service.GetZhanDuiData(team.TeamID);
							if (null != teamData)
							{
								foreach (TianTi5v5ZhanDuiRoleData role in teamData.teamerList)
								{
									KuaFuFuBenRoleData kuaFuFuBenRoleData = new KuaFuFuBenRoleData
									{
										ServerId = team.ServerID,
										RoleId = role.RoleID,
										Side = side
									};
									ZorkFuBenData.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData, team.TeamID);
								}
							}
						}
					}
					this.AssginKfFuben(ZorkFuBenData);
				}
				else if (TeamStandByList.Count > 0)
				{
					foreach (KF5v5PiPeiTeam item in TeamStandByList)
					{
						this.BybZhanDuiIDSet.Add(item.TeamID);
					}
					string zhanduiIdArray = string.Join<int>("|", this.BybZhanDuiIDSet.ToArray<int>());
					LogManager.WriteLog(LogTypes.Analysis, string.Format("Zork::轮空 SeasonID:{0} Round:{1} zhanduiId:{2} ", this.CurrentSeasonID, this.CurrentRound, zhanduiIdArray), null, true);
				}
				this.StateMachine.SetCurrState(Zork5v5StateMachine.StateType.NotifyEnter, now, param);
				LogManager.WriteLog(LogTypes.Analysis, string.Format("Zork::MS_PrepareGame_Update To:{0} SeasonID:{1} Round:{2}", Zork5v5StateMachine.StateType.NotifyEnter, this.CurrentSeasonID, this.CurrentRound), null, true);
			}
		}

		
		private void AssginKfFuben(KuaFu5v5FuBenData fubenData)
		{
			int toServerId = 0;
			int gameId = 0;
			gameId = TianTiPersistence.Instance.GetNextGameId();
			int roleNum = fubenData.ZhanDuiDict.Count * 5;
			if (ClientAgentManager.Instance().AssginKfFuben(this.GameType, (long)gameId, roleNum, out toServerId))
			{
				fubenData.ServerId = toServerId;
				fubenData.GameId = gameId;
				fubenData.GameType = (int)this.GameType;
				fubenData.LoginInfo = KuaFuServerManager.GetKuaFuLoginInfo(0, toServerId);
				this.FuBenDataDict[gameId] = fubenData;
				foreach (int zhanDuiID in fubenData.ZhanDuiDict.Keys)
				{
					this.ZhanDuiIDVsGameIDDict[zhanDuiID] = gameId;
					KF5v5PiPeiTeam kuaFuRoleDataTemp;
					if (this.PiPeiDict.TryGetValue(zhanDuiID, out kuaFuRoleDataTemp))
					{
						kuaFuRoleDataTemp.GameId = fubenData.GameId;
					}
				}
				string zhanduiIdArray = string.Join<int>("|", fubenData.ZhanDuiDict.Keys.ToArray<int>());
				LogManager.WriteLog(LogTypes.Analysis, string.Format("Zork::分组 SeasonID:{0} gameId:{1} zhanduiId:{2} Round:{3}", new object[]
				{
					this.CurrentSeasonID,
					gameId,
					zhanduiIdArray,
					this.CurrentRound
				}), null, true);
			}
			else
			{
				string zhanduiIdArray = string.Join<int>("|", fubenData.ZhanDuiDict.Keys.ToArray<int>());
				LogManager.WriteLog(LogTypes.Analysis, string.Format("Zork::分配游戏服务器失败 SeasonID:{0} gameId:{1} zhanduiId:{2} Round:{3}", new object[]
				{
					this.CurrentSeasonID,
					gameId,
					zhanduiIdArray,
					this.CurrentRound
				}), null, true);
			}
		}

		
		private void MS_NotifyEnter_Update(DateTime now, int param)
		{
			ZorkBattleSceneInfo matchConfig = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
			Zork5v5StateMachine.StateType GameState = Zork5v5StateMachine.StateType.None;
			for (int i = 0; i < matchConfig.TimePoints.Count - 1; i += 2)
			{
				if (now.DayOfWeek == (DayOfWeek)matchConfig.TimePoints[i].Days)
				{
					int RoundSeconds = matchConfig.BattleSignSecs + matchConfig.PrepareSecs + matchConfig.FightingSecs + matchConfig.ClearRolesSecs;
					int MatchPerRound = (int)(matchConfig.SecondsOfDay[i + 1] - matchConfig.SecondsOfDay[i]) / RoundSeconds;
					for (int matchloop = 0; matchloop < MatchPerRound; matchloop++)
					{
						int signSeconds = (int)matchConfig.SecondsOfDay[i] + RoundSeconds * matchloop;
						int startSeconds = signSeconds + matchConfig.BattleSignSecs;
						int endSeconds = startSeconds + RoundSeconds - matchConfig.BattleSignSecs;
						if ((double)startSeconds <= now.TimeOfDay.TotalSeconds && now.TimeOfDay.TotalSeconds < (double)endSeconds)
						{
							GameState = Zork5v5StateMachine.StateType.GameStart;
						}
					}
				}
			}
			if (GameState == Zork5v5StateMachine.StateType.GameStart)
			{
				foreach (KuaFu5v5FuBenData item in this.FuBenDataDict.Values)
				{
					KuaFu5v5FuBenData SyncData = item;
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.EvItemGameType, new AsyncDataItem(KuaFuEventTypes.Zork5v5NtfEnter, new object[]
					{
						SyncData
					}), 0);
				}
				this.StateMachine.SetCurrState(GameState, now, param);
				LogManager.WriteLog(LogTypes.Analysis, string.Format("Zork::MS_NotifyEnter_Update To:{0} SeasonID:{1} Round:{2}", GameState, this.CurrentSeasonID, this.CurrentRound), null, true);
			}
		}

		
		private void MS_GameStart_Update(DateTime now, int param)
		{
			ZorkBattleSceneInfo matchConfig = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
			Zork5v5StateMachine.StateType GameState = Zork5v5StateMachine.StateType.None;
			for (int i = 0; i < matchConfig.TimePoints.Count - 1; i += 2)
			{
				if (now.DayOfWeek == (DayOfWeek)matchConfig.TimePoints[i].Days)
				{
					int RoundSeconds = matchConfig.BattleSignSecs + matchConfig.PrepareSecs + matchConfig.FightingSecs + matchConfig.ClearRolesSecs;
					int MatchPerRound = (int)(matchConfig.SecondsOfDay[i + 1] - matchConfig.SecondsOfDay[i]) / RoundSeconds;
					for (int matchloop = 0; matchloop < MatchPerRound; matchloop++)
					{
						int signSeconds = (int)matchConfig.SecondsOfDay[i] + RoundSeconds * matchloop;
						int startSeconds = signSeconds + matchConfig.BattleSignSecs;
						int endSeconds = startSeconds + RoundSeconds - matchConfig.BattleSignSecs;
						if (this.LastUpdateTime.TimeOfDay.TotalSeconds < (double)endSeconds && (double)endSeconds <= now.TimeOfDay.TotalSeconds)
						{
							GameState = Zork5v5StateMachine.StateType.RankAnalyse;
						}
					}
				}
			}
			if (GameState == Zork5v5StateMachine.StateType.RankAnalyse)
			{
				this.StateMachine.SetCurrState(GameState, now, param);
				LogManager.WriteLog(LogTypes.Analysis, string.Format("Zork::MS_GameStart_Update To:{0} SeasonID:{1} Round:{2}", GameState, this.CurrentSeasonID, this.CurrentRound), null, true);
			}
		}

		
		private void MS_RankAnalyse_Enter(DateTime now, int param)
		{
			this.PiPeiDict.Clear();
			this.BybZhanDuiIDSet.Clear();
		}

		
		private void MS_RankAnalyse_Update(DateTime now, int param)
		{
			ZorkBattleSceneInfo matchConfig = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
			Zork5v5StateMachine.StateType GameState = Zork5v5StateMachine.StateType.None;
			for (int i = 0; i < matchConfig.TimePoints.Count - 1; i += 2)
			{
				if (now.DayOfWeek == (DayOfWeek)matchConfig.TimePoints[i].Days)
				{
					int RoundSeconds = matchConfig.BattleSignSecs + matchConfig.PrepareSecs + matchConfig.FightingSecs + matchConfig.ClearRolesSecs;
					int MatchPerRound = (int)(matchConfig.SecondsOfDay[i + 1] - matchConfig.SecondsOfDay[i]) / RoundSeconds;
					for (int matchloop = 0; matchloop < MatchPerRound; matchloop++)
					{
						int signSeconds = (int)matchConfig.SecondsOfDay[i] + RoundSeconds * matchloop;
						int startSeconds = signSeconds + matchConfig.BattleSignSecs;
						int endSeconds = startSeconds + RoundSeconds - matchConfig.BattleSignSecs;
						int analyseSeconds = endSeconds + matchConfig.BattleSignSecs / 2;
						if (this.LastUpdateTime.TimeOfDay.TotalSeconds < (double)analyseSeconds && (double)analyseSeconds <= now.TimeOfDay.TotalSeconds)
						{
							if (now.TimeOfDay.TotalSeconds > matchConfig.SecondsOfDay[i + 1])
							{
								GameState = Zork5v5StateMachine.StateType.Init;
							}
							else
							{
								GameState = Zork5v5StateMachine.StateType.SignUp;
							}
						}
					}
				}
			}
			int CalSeasonID = this.ComputeCurrentSeasonID(now, this.CurrentSeasonID);
			if (this.CurrentSeasonID != CalSeasonID)
			{
				GameState = Zork5v5StateMachine.StateType.Init;
			}
			if (Zork5v5StateMachine.StateType.None != GameState)
			{
				this.HandleUnCompleteFuBenData();
				if (this.CurrentSeasonID != CalSeasonID)
				{
					this.Persistence.SaveZorkSeasonID(this.CurrentSeasonID);
					this.CurrentSeasonID = CalSeasonID;
					TianTi5v5Service.ClearAllZhanDuiZorkData();
					this.ZorkBattleRankInfoDict.V.Clear();
					TimeUtil.AgeByNow(ref this.ZorkBattleRankInfoDict.Age);
				}
				else
				{
					this.ReloadRankInfo(0, this.ZorkBattleRankInfoDict);
					this.ReloadRankInfo(1, this.ZorkBattleRankInfoDict);
				}
				this.CurrentRound = this.GetCurrentRoundByTime(now, this.CurrentSeasonID);
				ZorkBattleSceneInfo sceneConfig = this.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
				if (this.CurrentRound > sceneConfig.SeasonFightRound)
				{
					List<KFZorkRankInfo> rankList;
					if (this.ZorkBattleRankInfoDict.V.TryGetValue(0, out rankList) && rankList.Count != 0)
					{
						this.TopZhanDui = rankList[0].Key;
					}
					else
					{
						this.TopZhanDui = 0;
					}
					this.Persistence.SaveZorkTopZhanDui(this.TopZhanDui);
					this.UpdateTopZhanDuiInfo();
					if (this.ZorkBattleRankInfoDict.V.TryGetValue(1, out rankList) && rankList.Count != 0)
					{
						this.TopKiller = rankList[0].Key;
					}
					else
					{
						this.TopKiller = 0;
					}
					this.Persistence.SaveZorkTopKiller(this.TopKiller);
				}
				this.StateMachine.SetCurrState(GameState, now, param);
				LogManager.WriteLog(LogTypes.Analysis, string.Format("Zork::MS_RankAnalyse_Update To:{0} SeasonID:{1} Round:{2}", GameState, this.CurrentSeasonID, this.CurrentRound), null, true);
			}
		}

		
		private void HandleUnCompleteFuBenData()
		{
			foreach (KeyValuePair<int, KuaFu5v5FuBenData> fubenItem in this.FuBenDataDict)
			{
				KuaFu5v5FuBenData fubenData = fubenItem.Value;
				ClientAgentManager.Instance().RemoveKfFuben(this.GameType, fubenData.ServerId, (long)fubenData.GameId);
			}
			this.FuBenDataDict.Clear();
			this.ZhanDuiIDVsGameIDDict.Clear();
		}

		
		public KuaFu5v5FuBenData GetFuBenDataByGameId_ZorkBattle(int gameid)
		{
			KuaFu5v5FuBenData fuBenData = null;
			try
			{
				lock (this.Mutex)
				{
					this.FuBenDataDict.TryGetValue(gameid, out fuBenData);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "Zork5v5Service.GetFuBenDataByGameId_ZorkBattle failed!", ex, true);
			}
			return fuBenData;
		}

		
		public KuaFu5v5FuBenData GetFuBenDataByZhanDuiId_ZorkBattle(int ZhanDuiId)
		{
			KuaFu5v5FuBenData fuBenData = null;
			try
			{
				lock (this.Mutex)
				{
					int gameid = 0;
					this.ZhanDuiIDVsGameIDDict.TryGetValue(ZhanDuiId, out gameid);
					this.FuBenDataDict.TryGetValue(gameid, out fuBenData);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "Zork5v5Service.GetFuBenDataByZhanDuiId_ZorkBattle failed!", ex, true);
			}
			return fuBenData;
		}

		
		public ZorkBattleSyncData SyncData_ZorkBattle(long gsTicks, long ageRank)
		{
			ZorkBattleSyncData SyncData = new ZorkBattleSyncData();
			try
			{
				lock (this.Mutex)
				{
					SyncData.CurSeasonID = this.CurrentSeasonID;
					SyncData.CurRound = this.CurrentRound;
					SyncData.TopZhanDui = this.TopZhanDui;
					SyncData.TopZhanDuiName = this.TopZhanDuiName;
					SyncData.TopKiller = this.TopKiller;
					SyncData.DiffKFCenterSeconds = (int)(gsTicks - TimeUtil.NOW()) / 1000;
					SyncData.ZorkBattleRankInfoDict.Age = this.ZorkBattleRankInfoDict.Age;
					if (ageRank != this.ZorkBattleRankInfoDict.Age)
					{
						SyncData.ZorkBattleRankInfoDict = this.ZorkBattleRankInfoDict;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "Zork5v5Service.SyncData_ZorkBattle failed!", ex, true);
			}
			return SyncData;
		}

		
		public string GetKuaFuGameState_ZorkBattle(int zhanduiID)
		{
			string result = "";
			try
			{
				lock (this.Mutex)
				{
					int signState = -4034;
					if (!this.PiPeiDict.ContainsKey(zhanduiID))
					{
						signState = -4035;
					}
					else if (this.StateMachine.GetCurrState() == Zork5v5StateMachine.StateType.NotifyEnter || this.StateMachine.GetCurrState() == Zork5v5StateMachine.StateType.GameStart)
					{
						if (this.BybZhanDuiIDSet.Contains(zhanduiID))
						{
							signState = -4036;
						}
						else if (!this.ZhanDuiIDVsGameIDDict.ContainsKey(zhanduiID))
						{
							signState = -4006;
						}
					}
					result = string.Format("{0}", signState);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "Zork5v5Service.GetKuaFuGameState_ZorkBattle failed!", ex, true);
			}
			return result;
		}

		
		public int SignUp_ZorkBattle(int zhanduiID, int serverID)
		{
			int result = 0;
			try
			{
				lock (this.Mutex)
				{
					DateTime now = TimeUtil.NowDateTime();
					if (!this.CheckOpenState(now))
					{
						result = -11004;
						return result;
					}
					if (this.StateMachine.GetCurrState() != Zork5v5StateMachine.StateType.SignUp && this.StateMachine.GetCurrState() != Zork5v5StateMachine.StateType.RankAnalyse)
					{
						result = -2001;
						return result;
					}
					TianTi5v5ZhanDuiData zhanduiData = TianTi5v5Service.GetZhanDuiData(zhanduiID);
					if (null == zhanduiData)
					{
						result = -5;
						return result;
					}
					if (this.PiPeiDict.ContainsKey(zhanduiID))
					{
						result = -5;
						return result;
					}
					KF5v5PiPeiTeam pipeiTeam = new KF5v5PiPeiTeam
					{
						TeamID = zhanduiID,
						ServerID = serverID,
						GroupIndex = this.CalDuanWeiByJiFen(zhanduiData.ZorkJiFen),
						ZhanDouLi = zhanduiData.ZhanDouLi,
						ZorkJiFen = zhanduiData.ZorkJiFen,
						ZhanDuiName = KuaFuServerManager.FormatName(zhanduiData.ZoneID, zhanduiData.ZhanDuiName)
					};
					this.PiPeiDict[zhanduiID] = pipeiTeam;
					LogManager.WriteLog(LogTypes.Analysis, string.Format("Zork::比赛报名 SeasonID:{0} Round:{1} ZhanDuiID:{2}", this.CurrentSeasonID, this.CurrentRound, zhanduiID), null, true);
				}
				return result;
			}
			catch (Exception ex)
			{
				result = -11;
				LogManager.WriteLog(LogTypes.Error, "Zork5v5Service.SignUp_ZorkBattle failed!", ex, true);
			}
			return result;
		}

		
		public int GameFuBenComplete_ZorkBattle(ZorkBattleStatisticalData data)
		{
			int result = 0;
			try
			{
				lock (this.Mutex)
				{
					KuaFu5v5FuBenData fubenData;
					if (!this.FuBenDataDict.TryGetValue(data.GameId, out fubenData))
					{
						result = -4000;
						return result;
					}
					ClientAgentManager.Instance().RemoveKfFuben(this.GameType, fubenData.ServerId, (long)data.GameId);
					this.FuBenDataDict.Remove(data.GameId);
					foreach (KeyValuePair<int, int> item in fubenData.ZhanDuiDict)
					{
						this.ZhanDuiIDVsGameIDDict.Remove(item.Key);
					}
					foreach (KeyValuePair<int, TianTi5v5ZhanDuiData> item2 in data.ZhanDuiDict)
					{
						TianTi5v5ZhanDuiData zhanduiData = item2.Value;
						TianTi5v5Service.UpdateZorkZhanDuiData(zhanduiData);
					}
					foreach (ZorkBattleRoleInfo client in data.ClientContextDataList)
					{
						this.Persistence.UpdateZorkBattleRoleData(client, true);
					}
					string zhanduiIdArray = string.Join<int>("|", data.ZhanDuiDict.Keys.ToArray<int>());
					string strLog = string.Format("Zork::GameFuBenComplete_ZorkBattle SeasonID:{0} GameID:{1} ZhanDuiIDWin:{2} ZhanDuiID:{3} Round:{2} ZhanDuiInfo:", new object[]
					{
						this.CurrentSeasonID,
						data.GameId,
						data.ZhanDuiIDWin,
						zhanduiIdArray,
						this.CurrentRound
					});
					foreach (KeyValuePair<int, TianTi5v5ZhanDuiData> item2 in data.ZhanDuiDict)
					{
						TianTi5v5ZhanDuiData zhanduiData = item2.Value;
						strLog += string.Format(" [ZhanDuiID:{0} JiFen:{1}]", item2.Key, zhanduiData.ZorkJiFen);
					}
					LogManager.WriteLog(LogTypes.Analysis, strLog, null, true);
					return result;
				}
			}
			catch (Exception ex)
			{
				result = -11;
				LogManager.WriteLog(LogTypes.Error, "Zork5v5Service.GameFuBenComplete_ZorkBattle failed!", ex, true);
			}
			return result;
		}

		
		public int CalDuanWeiByJiFen(int jifen)
		{
			int DuanWei = 0;
			lock (this.MutexConfig)
			{
				foreach (ZorkBattleAwardConfig item in this.ZorkLevelRangeList)
				{
					if ((item.RankValue < 0 || jifen >= item.RankValue) && item.ID > DuanWei)
					{
						DuanWei = item.ID;
					}
				}
			}
			return DuanWei;
		}

		
		public void Update(DateTime now)
		{
			try
			{
				if ((now - this.LastUpdateTime).TotalMilliseconds >= 1000.0)
				{
					this.UpdateFrameCount += 1U;
					lock (this.Mutex)
					{
						this.StateMachine.Tick(now, 0);
					}
					this.LastUpdateTime = now;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "Zork5v5Service.Update failed!", ex, true);
			}
		}

		
		public const int RankShowNum = 30;

		
		private static Zork5v5Service _instance = new Zork5v5Service();

		
		public readonly GameTypes GameType = GameTypes.Zork5v5;

		
		public readonly GameTypes EvItemGameType = GameTypes.TianTi;

		
		private object Mutex = new object();

		
		private object MutexConfig = new object();

		
		public Dictionary<int, ZorkBattleSceneInfo> SceneDataDict = new Dictionary<int, ZorkBattleSceneInfo>();

		
		public List<ZorkBattleAwardConfig> ZorkLevelRangeList = new List<ZorkBattleAwardConfig>();

		
		public DateTime ZorkStartTime;

		
		public TianTiPersistence Persistence = TianTiPersistence.Instance;

		
		private Zork5v5StateMachine StateMachine = new Zork5v5StateMachine();

		
		public Dictionary<int, KF5v5PiPeiTeam> PiPeiDict = new Dictionary<int, KF5v5PiPeiTeam>();

		
		private Dictionary<int, KuaFu5v5FuBenData> FuBenDataDict = new Dictionary<int, KuaFu5v5FuBenData>();

		
		private Dictionary<int, int> ZhanDuiIDVsGameIDDict = new Dictionary<int, int>();

		
		private HashSet<int> BybZhanDuiIDSet = new HashSet<int>();

		
		private KuaFuData<Dictionary<int, List<KFZorkRankInfo>>> ZorkBattleRankInfoDict = new KuaFuData<Dictionary<int, List<KFZorkRankInfo>>>();

		
		private int CurrentSeasonID = 0;

		
		private int CurrentRound = 0;

		
		private int TopZhanDui = 0;

		
		private string TopZhanDuiName;

		
		private int TopKiller = 0;

		
		private int SeasonWeeks = 1;

		
		private uint UpdateFrameCount = 0U;

		
		private DateTime LastUpdateTime = DateTime.MinValue;
	}
}
