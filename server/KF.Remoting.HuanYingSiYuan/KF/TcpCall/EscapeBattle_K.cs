using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AutoCSer.Metadata;
using AutoCSer.Net.TcpInternalServer;
using AutoCSer.Net.TcpServer;
using AutoCSer.Net.TcpStaticServer;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting;
using KF.Remoting.Data;
using Remoting;
using Server.Data;
using Server.Tools;

namespace KF.TcpCall
{
	
	[AutoCSer.Net.TcpStaticServer.Server(Name = "KfCall", IsServer = true, IsAttribute = true, IsClientAwaiter = false, MemberFilters = MemberFilters.Static, IsSegmentation = true, ClientSegmentationCopyPath = "GameServer\\Remoting\\")]
	public static class EscapeBattle_K
	{
		
		public static bool InitConfig()
		{
			lock (EscapeBattle_K.Mutex)
			{
				EscapeBattle_K.Initialize = false;
				bool bOk = EscapeBattle_K._Config.Load(KuaFuServerManager.GetResourcePath("Config\\EscapeActivityRules.xml", KuaFuServerManager.ResourcePathTypes.GameRes));
				EscapeBattleMatchConfig config = EscapeBattle_K._Config.MatchConfigList[0];
				EscapeBattleConsts.MinZhanDuiNumPerGame = config.MatchTeamNum;
				EscapeBattleConsts.MinRoleNumPerGame = config.EnterBattleNum;
				EscapeBattleConsts.BattleSignSecs = config.BattleSignSecs;
				DateTime.TryParse(KuaFuServerManager.systemParamsList.GetParamValueByName("EscapeStartTime"), out EscapeBattleConsts.EscapeStartTime);
				if (!bOk)
				{
					LogManager.WriteLog(LogTypes.Error, "EscapeBattle_K.InitConfig failed!", null, true);
				}
				EscapeBattle_K.Initialize = bOk;
			}
			return true;
		}

		
		public static void Update()
		{
			if (EscapeBattle_K.Initialize)
			{
				DateTime now = TimeUtil.NowDateTime();
				TimeSpan timeOfWeek = TimeUtil.TimeOfWeek(now);
				bool open = false;
				lock (EscapeBattle_K.Mutex)
				{
					List<TimeSpan> list = EscapeBattle_K._Config.MatchConfigList[0].TimePoints;
					for (int i = 0; i < list.Count - 1; i += 2)
					{
						if (list[i] <= timeOfWeek && timeOfWeek < list[i + 1])
						{
							open = true;
							break;
						}
					}
					if (EscapeBattle_K.SyncData.State != open)
					{
						EscapeBattle_K.SyncData.State = open;
					}
					int nowSecs = (int)now.TimeOfDay.TotalSeconds;
					if (nowSecs / EscapeBattleConsts.BattleSignSecs != EscapeBattle_K.LastMatchMinute / EscapeBattleConsts.BattleSignSecs)
					{
						EscapeBattle_K.LastMatchMinute = nowSecs;
						EscapeBattle_K.PrepareMatchList();
					}
					EscapeBattle_K.PrepareGameFuBen(now);
					if (open)
					{
						if (!EscapeBattle_K.NeedUpdateRank)
						{
							EscapeBattle_K.NeedUpdateRank = true;
						}
					}
					else if ((EscapeBattle_K.NeedUpdateRank && EscapeBattle_K.ThisLoopPkLogs.Count == 0) || EscapeBattle_K.lastUpdateTime.Day != now.Day)
					{
						EscapeBattle_K.NeedUpdateRank = false;
						EscapeBattle_K.LoadSyncData(now, true);
					}
					EscapeBattle_K.ClearTimeOverGameFuBen(now);
				}
				KFCallMsg[] asyncEvArray = null;
				lock (EscapeBattle_K.Mutex)
				{
					asyncEvArray = EscapeBattle_K.AsyncEvQ.ToArray();
					EscapeBattle_K.AsyncEvQ.Clear();
				}
				foreach (KFCallMsg msg in asyncEvArray)
				{
					ClientAgentManager.Instance().BroadCastMsg(msg, 0);
				}
				EscapeBattle_K.lastUpdateTime = now;
			}
		}

		
		private static bool PrepareMatchList()
		{
			EscapeBattle_K.JoinList.Clear();
			foreach (EscapeBattle_K.JoinPkData item in EscapeBattle_K.JoinDict.Values.ToList<EscapeBattle_K.JoinPkData>())
			{
                if (item.ReadyState && item.ReadyNum >= EscapeBattleConsts.MinRoleNumPerGame && !item.InGame)
				{
					EscapeBattle_K.JoinList.Add(new EscapeBattle_K.JoinPkData
					{
						ZhanDuiID = item.ZhanDuiID,
						ZoneId = item.ZoneId,
						ZhanDuiName = item.ZhanDuiName,
						ReadyNum = item.ReadyNum,
						DuanWeiJiFen = item.DuanWeiJiFen
					});
				}
			}
			if (EscapeBattle_K.JoinList.Count >= EscapeBattleConsts.MinZhanDuiNumPerGame)
			{
				EscapeBattleMatchConfig config = EscapeBattle_K._Config.MatchConfigList[0];
				EscapeBattle_K.JoinList.Sort((EscapeBattle_K.JoinPkData x, EscapeBattle_K.JoinPkData y) => x.DuanWeiJiFen - y.DuanWeiJiFen);
				List<int> notMatchList = new List<int>();
				int needLeftNum = EscapeBattle_K.JoinList.Count % config.MatchTeamNum;
				if (needLeftNum != 0)
				{
					int leftNum = 0;
					int rnd = Global.GetRandomNumber(0, EscapeBattle_K.JoinList.Count);
					for (int i = rnd; i < EscapeBattle_K.JoinList.Count + rnd; i++)
					{
						EscapeBattle_K.JoinPkData item = EscapeBattle_K.JoinList[i % EscapeBattle_K.JoinList.Count];
						if (EscapeBattle_K.LastMatchList.Contains(item.ZhanDuiID) && !notMatchList.Contains(item.ZhanDuiID))
						{
							notMatchList.Add(item.ZhanDuiID);
							leftNum++;
							if (leftNum >= needLeftNum)
							{
								break;
							}
						}
					}
					if (leftNum < needLeftNum)
					{
						for (int i = rnd; i < EscapeBattle_K.JoinList.Count + rnd; i++)
						{
							EscapeBattle_K.JoinPkData item = EscapeBattle_K.JoinList[i % EscapeBattle_K.JoinList.Count];
							if (!EscapeBattle_K.NotMatchList.Contains(item.ZhanDuiID) && !notMatchList.Contains(item.ZhanDuiID))
							{
								notMatchList.Add(item.ZhanDuiID);
								leftNum++;
								if (leftNum >= needLeftNum)
								{
									break;
								}
							}
						}
					}
					if (leftNum < needLeftNum)
					{
						for (int i = rnd; i < EscapeBattle_K.JoinList.Count + rnd; i++)
						{
							EscapeBattle_K.JoinPkData item = EscapeBattle_K.JoinList[i % EscapeBattle_K.JoinList.Count];
							if (!notMatchList.Contains(item.ZhanDuiID))
							{
								notMatchList.Add(item.ZhanDuiID);
								leftNum++;
								if (leftNum >= needLeftNum)
								{
									break;
								}
							}
						}
					}
					EscapeBattle_K.JoinList.RemoveAll((EscapeBattle_K.JoinPkData x) => notMatchList.Contains(x.ZhanDuiID));
				}
				EscapeBattle_K.NotMatchList = notMatchList;
				EscapeBattle_K.LastMatchList = EscapeBattle_K.JoinList.ConvertAll<int>((EscapeBattle_K.JoinPkData x) => x.ZhanDuiID);
				EscapeBattle_K.JoinList = EscapeBattle_K.JoinList.RandomSortList<EscapeBattle_K.JoinPkData>();
			}
			return true;
		}

		
		private static void PrepareGameFuBen(DateTime now)
		{
			for (int i = 0; i < EscapeBattle_K.JoinList.Count - (EscapeBattleConsts.MinZhanDuiNumPerGame - 1); i += EscapeBattleConsts.MinZhanDuiNumPerGame)
			{
				if (EscapeBattleConsts.MinZhanDuiNumPerGame == 3)
				{
					EscapeBattle_K.CreateGameFuBen(now, new EscapeBattle_K.JoinPkData[]
					{
						EscapeBattle_K.JoinList[i],
						EscapeBattle_K.JoinList[i + 1],
						EscapeBattle_K.JoinList[i + 2]
					});
				}
				else if (EscapeBattleConsts.MinZhanDuiNumPerGame == 2)
				{
					EscapeBattle_K.CreateGameFuBen(now, new EscapeBattle_K.JoinPkData[]
					{
						EscapeBattle_K.JoinList[i],
						EscapeBattle_K.JoinList[i + 1]
					});
				}
				else if (EscapeBattleConsts.MinZhanDuiNumPerGame == 1)
				{
					EscapeBattle_K.CreateGameFuBen(now, new EscapeBattle_K.JoinPkData[]
					{
						EscapeBattle_K.JoinList[i]
					});
				}
			}
		}

		
		private static bool CreateGameFuBen(DateTime now, params EscapeBattle_K.JoinPkData[] joinArr)
		{
			for (int side = 1; side <= joinArr.Length; side++)
			{
				joinArr[side - 1].Side = side;
				if (joinArr[side - 1].InGame)
				{
					return true;
				}
			}
			int toServerId = 0;
			int gameId = TianTiPersistence.Instance.GetNextGameId();
			if (ClientAgentManager.Instance().AssginKfFuben(EscapeBattle_K.GameType, (long)gameId, 10, out toServerId))
			{
				EscapeBattleFuBenData copyData = new EscapeBattleFuBenData();
				copyData.GameID = (long)gameId;
				copyData.ServerID = toServerId;
				EscapeBattleNtfEnterData data = new EscapeBattleNtfEnterData();
				data.ToServerId = toServerId;
				data.GameId = gameId;
				EscapeBattlePkLogData log = new EscapeBattlePkLogData();
				log.Season = EscapeBattle_K.SyncData.Season;
				log.StartTime = now;
				log.GameID = gameId;
				log.ToServerID = toServerId;
				EscapeBattleMatchConfig config = EscapeBattle_K._Config.MatchConfigList[0];
				log.EndTime = now.AddSeconds((double)config.TotalSecs);
				foreach (EscapeBattle_K.JoinPkData joinRole in joinArr)
				{
					copyData.SideDict[(long)joinRole.ZhanDuiID] = joinRole.Side;
					copyData.RoleDict.AddRange(TianTi5v5Service.GetZhanDuiMemberIDs(joinRole.ZhanDuiID));
					joinRole.ToServerID = toServerId;
					joinRole.CurrGameID = gameId;
					joinRole.CopyData = copyData;
					joinRole.InGame = true;
					data.ZhanDuiIDList.Add(joinRole.ZhanDuiID);
					log.ZhanDuiIDs.Add(joinRole.ZhanDuiID);
					log.ZoneIDs.Add(joinRole.ZoneId);
					log.ZhanDuiNames.Add(joinRole.ZhanDuiName);
					EscapeBattle_K.JoinPkData zhandui;
					if (EscapeBattle_K.JoinDict.TryGetValue(joinRole.ZhanDuiID, out zhandui))
					{
						zhandui.InGame = true;
						zhandui.CurrGameID = gameId;
						zhandui.Side = joinRole.Side;
						zhandui.ToServerID = toServerId;
						zhandui.CopyData = copyData;
						zhandui.State = 3;
						zhandui.ReadyState = false;
						zhandui.ReadyNum = 0;
					}
				}
				EscapeBattle_K.AsyncEvQ.Enqueue(KFCallMsg.New<EscapeBattleNtfEnterData>(KuaFuEventTypes.EscapeBattle_NotifyEnter, data));
				EscapeBattle_K.ThisLoopPkLogs[gameId] = log;
				if (!EscapeBattle_K.NeedUpdateRank)
				{
					EscapeBattle_K.NeedUpdateRank = true;
				}
				LogManager.WriteLog(LogTypes.Trace, string.Format("大逃杀第{0}赛季战队成员通知入场,GameID={1},zhanduiIDs={2}", EscapeBattle_K.SyncData.Season, data.GameId, string.Join<int>(",", log.ZhanDuiIDs)), null, true);
				return true;
			}
			LogManager.WriteLog(LogTypes.Warning, string.Format("大逃杀第{0}赛季分配游戏服务器失败", EscapeBattle_K.SyncData.Season), null, true);
			return false;
		}

		
		private static void ClearTimeOverGameFuBen(DateTime now)
		{
			List<int> PkLogsRemoveList = new List<int>();
			foreach (int gameID in EscapeBattle_K.ThisLoopPkLogs.Keys.ToList<int>())
			{
				EscapeBattlePkLogData log;
				if (EscapeBattle_K.ThisLoopPkLogs.TryGetValue(gameID, out log))
				{
					if (log.EndTime < now || log.State >= 7)
					{
						List<int> scoreList = new List<int>();
						foreach (int zhanDuiID in log.ZhanDuiIDs)
						{
							scoreList.Add(zhanDuiID);
							scoreList.Add(int.MinValue);
						}
						try
						{
							EscapeBattle_K.GameResult(gameID, scoreList);
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
							PkLogsRemoveList.Add(log.GameID);
						}
					}
				}
			}
			foreach (int gameid in PkLogsRemoveList)
			{
				EscapeBattlePkLogData log;
				if (EscapeBattle_K.ThisLoopPkLogs.TryGetValue(gameid, out log))
				{
					EscapeBattle_K.ThisLoopPkLogs.Remove(gameid);
					ClientAgentManager.Instance().RemoveKfFuben(EscapeBattle_K.GameType, log.ToServerID, (long)log.GameID);
				}
			}
		}

		
		public static void LoadSyncData(DateTime now, bool rebuild = false)
		{
			lock (EscapeBattle_K.Mutex)
			{
				List<KFEscapeRankInfo> rankList = new List<KFEscapeRankInfo>();
				List<KFEscapeRankInfo> SeasonRankList = new List<KFEscapeRankInfo>();
				if (KuaFuServerManager.IsGongNengOpened(115) && EscapeBattleConsts.CheckOpenState(now))
				{
					int weekStartDay = TimeUtil.GetWeekStartDayIdNow();
					int offsetDay = TimeUtil.GetOffsetDayNow();
					int startDay = EscapeBattle_K.Persistence.GetAsyncInt(14, weekStartDay);
					startDay = MathEx.Pack(offsetDay, startDay, EscapeBattleConsts.DaysPerSeason);
					int lastStartDay = startDay - EscapeBattleConsts.DaysPerSeason;
					DateTime minFightTime = TimeUtil.GetRealDate(startDay);
					if (EscapeBattle_K.Persistence.BuildEscapeRankList(startDay, minFightTime))
					{
					}
					EscapeBattle_K.SyncData.Season = startDay;
					if (offsetDay == startDay)
					{
						EscapeBattle_K.Persistence.SetAsyncInt(14, EscapeBattle_K.SyncData.Season);
					}
					rankList = EscapeBattle_K.Persistence.LoadEscapeRankData(startDay);
					SeasonRankList = EscapeBattle_K.Persistence.LoadEscapeRankData(lastStartDay);
				}
				EscapeBattle_K.SyncData.RankList = rankList;
				EscapeBattle_K.SyncData.SeasonRankList = SeasonRankList;
				EscapeBattle_K.SyncData.RankModTime = now;
			}
		}

		
		private static void ZhanDuiChangeState(EscapeBattle_K.JoinPkData pkData, int state)
		{
			int zhanDuiState = 0;
			if (state >= 5)
			{
				pkData.InGame = false;
				zhanDuiState = 0;
			}
			pkData.State = zhanDuiState;
			int sid = KuaFuServerManager.GetServerIDFromZoneID(pkData.ZoneId);
			ClientAgentManager.Instance().SendMsg(sid, KFCallMsg.New<int[]>(KuaFuEventTypes.EscapeBattle_GameState, new int[]
			{
				pkData.ZhanDuiID,
				pkData.CurrGameID,
				zhanDuiState
			}));
		}

		
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static EscapeBattleSyncData SyncZhengBaData(EscapeBattleSyncData lastSyncData)
		{
			EscapeBattleSyncData result = new EscapeBattleSyncData();
			lock (EscapeBattle_K.Mutex)
			{
				result.Season = EscapeBattle_K.SyncData.Season;
				result.State = EscapeBattle_K.SyncData.State;
				result.CenterTime = TimeUtil.NowDateTime();
				result.RankModTime = lastSyncData.RankModTime;
				if (EscapeBattle_K.SyncData.RankModTime != result.RankModTime && EscapeBattle_K.SyncData.RankList != null)
				{
					result.RankModTime = EscapeBattle_K.SyncData.RankModTime;
					result.RankList = new List<KFEscapeRankInfo>(EscapeBattle_K.SyncData.RankList);
					result.SeasonRankList = new List<KFEscapeRankInfo>(EscapeBattle_K.SyncData.SeasonRankList);
				}
			}
			return result;
		}

		
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static int GetZhanDuiState(int zhanDuiID)
		{
			int state = 0;
			lock (EscapeBattle_K.Mutex)
			{
				EscapeBattle_K.JoinPkData joinData;
				if (EscapeBattle_K.JoinDict.TryGetValue(zhanDuiID, out joinData))
				{
					if (joinData.State == 3 && !joinData.InGame)
					{
						return 0;
					}
					return joinData.State;
				}
			}
			return state;
		}

		
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static int ZhanDuiJoin(int zhanDuiID, int jiFen, int readyNum)
		{
			DateTime now = TimeUtil.NowDateTime();
			int result;
			if (!EscapeBattleConsts.CheckOpenState(now))
			{
				result = -11004;
			}
			else
			{
				TianTi5v5ZhanDuiData zhanDuiData = TianTi5v5Service.GetZhanDuiData(zhanDuiID);
				if (null == zhanDuiData)
				{
					result = -4031;
				}
				else
				{
					lock (EscapeBattle_K.Mutex)
					{
						EscapeBattle_K.JoinPkData joinData;
						if (!EscapeBattle_K.JoinDict.TryGetValue(zhanDuiID, out joinData))
						{
							joinData = new EscapeBattle_K.JoinPkData
							{
								ZhanDuiID = zhanDuiID
							};
							EscapeBattle_K.JoinDict[zhanDuiID] = joinData;
						}
						joinData.DuanWeiJiFen = zhanDuiData.EscapeJiFen;
						joinData.ZhanDuiName = zhanDuiData.ZhanDuiName;
						joinData.ZoneId = zhanDuiData.ZoneID;
						joinData.ReadyNum = readyNum;
						if (readyNum > 0)
						{
							joinData.ReadyState = true;
							joinData.State = 2;
						}
						else if (readyNum == 0)
						{
							joinData.ReadyState = false;
						}
					}
					result = 0;
				}
			}
			return result;
		}

		
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static int ZhengBaKuaFuLogin(AutoCSer.Net.TcpInternalServer.ServerSocketSender socket, int zhanDuiID, int gameId, int srcServerID, out EscapeBattleFuBenData copyData)
		{
			copyData = null;
			lock (EscapeBattle_K.Mutex)
			{
				EscapeBattle_K.JoinPkData roleData;
				if (!EscapeBattle_K.JoinDict.TryGetValue(zhanDuiID, out roleData) || roleData.CurrGameID == 0)
				{
					return -4006;
				}
				EscapeBattlePkLogData logData = null;
				if (!EscapeBattle_K.ThisLoopPkLogs.TryGetValue(roleData.CurrGameID, out logData))
				{
					return -4006;
				}
				copyData = roleData.CopyData;
			}
			KuaFuServerInfo serverInfo = KuaFuServerManager.GetKuaFuServerInfo(srcServerID);
			int result;
			if (null != serverInfo)
			{
				copyData.IPs = new string[]
				{
					serverInfo.DbIp,
					serverInfo.DbIp
				};
				copyData.Ports = new int[]
				{
					serverInfo.DbPort,
					serverInfo.LogDbPort
				};
				result = 0;
			}
			else
			{
				result = -11000;
			}
			return result;
		}

		
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static int ZhengBaRequestEnter(int zhanDuiID, out int gameId, out int kuaFuServerID, out string[] ips, out int[] ports)
		{
			gameId = 0;
			kuaFuServerID = 0;
			ips = null;
			ports = null;
			lock (EscapeBattle_K.Mutex)
			{
				EscapeBattle_K.JoinPkData roleData;
				if (!EscapeBattle_K.JoinDict.TryGetValue(zhanDuiID, out roleData) || roleData.CurrGameID == 0)
				{
					return -4006;
				}
				EscapeBattlePkLogData logData = null;
				if (!EscapeBattle_K.ThisLoopPkLogs.TryGetValue(roleData.CurrGameID, out logData))
				{
					return -4006;
				}
				if (logData.State >= 3)
				{
					return -2008;
				}
				gameId = roleData.CurrGameID;
				kuaFuServerID = roleData.ToServerID;
			}
			KuaFuServerInfo serverInfo = KuaFuServerManager.GetKuaFuServerInfo(kuaFuServerID);
			int result;
			if (null != serverInfo)
			{
				ips = new string[]
				{
					serverInfo.Ip
				};
				ports = new int[]
				{
					serverInfo.Port
				};
				result = 0;
			}
			else
			{
				result = -11001;
			}
			return result;
		}

		
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static int GameState(int gameId, int state)
		{
			int result;
			lock (EscapeBattle_K.Mutex)
			{
				EscapeBattlePkLogData log = null;
				if (!EscapeBattle_K.ThisLoopPkLogs.TryGetValue(gameId, out log))
				{
					result = -11003;
				}
				else
				{
					log.State = state;
					foreach (int zhanDuiID in log.ZhanDuiIDs)
					{
						EscapeBattle_K.JoinPkData pkData;
						if (EscapeBattle_K.JoinDict.TryGetValue(zhanDuiID, out pkData))
						{
							EscapeBattle_K.ZhanDuiChangeState(pkData, state);
						}
					}
					result = 0;
				}
			}
			return result;
		}

		
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static int GameResult(int gameId, List<int> zhanDuiScoreList)
		{
			int result;
			lock (EscapeBattle_K.Mutex)
			{
				EscapeBattlePkLogData log = null;
				LogManager.WriteLog(LogTypes.Trace, string.Format("EscapeBattle::GameResult,gameid={0},scoreList={1}", gameId, string.Join<int>("_", zhanDuiScoreList)), null, true);
				if (!EscapeBattle_K.ThisLoopPkLogs.TryGetValue(gameId, out log))
				{
					result = 3;
				}
				else
				{
					DateTime now = TimeUtil.NowDateTime();
					for (int i = 0; i < zhanDuiScoreList.Count - 1; i += 2)
					{
						int zhanDuiID = zhanDuiScoreList[i];
						int score = zhanDuiScoreList[i + 1];
						if (log.ZhanDuiIDs.Contains(zhanDuiID))
						{
							EscapeBattle_K.JoinPkData pkData;
							if (EscapeBattle_K.JoinDict.TryGetValue(zhanDuiID, out pkData))
							{
								if (pkData.InGame)
								{
									pkData.InGame = false;
									pkData.CurrGameID = 0;
									pkData.DuanWeiJiFen = TianTi5v5Service.UpdateEscapeZhanDui(pkData.ZhanDuiID, score, now);
								}
							}
							LogManager.WriteLog(LogTypes.Trace, string.Format("EscapeBattle::GameResult,gameid={0},zhanduiid={1},score={2}", gameId, zhanDuiID, score), null, true);
							EscapeBattle_K.ZhanDuiChangeState(pkData, 5);
						}
					}
					bool canRemove = true;
					foreach (int zhanDuiID in log.ZhanDuiIDs)
					{
                        EscapeBattle_K.JoinPkData pkData;
						if (EscapeBattle_K.JoinDict.TryGetValue(zhanDuiID, out pkData) && pkData.InGame)
						{
							canRemove = false;
						}
					}
					if (canRemove)
					{
						EscapeBattle_K.ThisLoopPkLogs.Remove(gameId);
						ClientAgentManager.Instance().RemoveKfFuben(EscapeBattle_K.GameType, log.ToServerID, (long)log.GameID);
					}
					result = 0;
				}
			}
			return result;
		}

		
		private static object Mutex = new object();

		
		private static bool Initialize = false;

		
		private static GameTypes GameType = GameTypes.EscapeBattle;

		
		private static EscapeBattleSyncData SyncData = new EscapeBattleSyncData();

		
		private static DateTime lastUpdateTime = TimeUtil.NowDateTime();

		
		private static Queue<KFCallMsg> AsyncEvQ = new Queue<KFCallMsg>();

		
		private static EscapeBattleConfig _Config = new EscapeBattleConfig();

		
		private static Dictionary<int, EscapeBattleZhanDuiData> ZhanDuiDataDict = new Dictionary<int, EscapeBattleZhanDuiData>();

		
		private static Dictionary<int, EscapeBattlePkLogData> ThisLoopPkLogs = new Dictionary<int, EscapeBattlePkLogData>();

		
		private static Dictionary<int, EscapeBattle_K.JoinPkData> JoinDict = new Dictionary<int, EscapeBattle_K.JoinPkData>();

		
		private static List<EscapeBattle_K.JoinPkData> JoinList = new List<EscapeBattle_K.JoinPkData>();

		
		private static List<int> LastMatchList = new List<int>();

		
		private static List<int> NotMatchList = new List<int>();

		
		private static int LastMatchMinute;

		
		private static bool NeedUpdateRank = false;

		
		private static TianTiPersistence Persistence = TianTiPersistence.Instance;

		
		private class JoinPkData
		{
			
			public int ZhanDuiID;

			
			public int ZoneId;

			
			public string ZhanDuiName;

			
			public int ReadyNum;

			
			public int DuanWeiJiFen;

			
			public int ToServerID;

			
			public int CurrGameID;

			
			public EscapeBattleFuBenData CopyData;

			
			public int Side;

			
			public bool InGame;

			
			public bool ReadyState;

			
			public int State;
		}

		
		internal static class TcpStaticServer
		{
			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M3(int gameId, List<int> zhanDuiScoreList)
			{
				return EscapeBattle_K.GameResult(gameId, zhanDuiScoreList);
			}

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M21(int gameId, int state)
			{
				return EscapeBattle_K.GameState(gameId, state);
			}

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M22(int zhanDuiID)
			{
				return EscapeBattle_K.GetZhanDuiState(zhanDuiID);
			}

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static EscapeBattleSyncData _M1(EscapeBattleSyncData lastSyncData)
			{
				return EscapeBattle_K.SyncZhengBaData(lastSyncData);
			}

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M20(int zhanDuiID, int jiFen, int readyNum)
			{
				return EscapeBattle_K.ZhanDuiJoin(zhanDuiID, jiFen, readyNum);
			}

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M2(AutoCSer.Net.TcpInternalServer.ServerSocketSender _sender_, int zhanDuiID, int gameId, int srcServerID, out EscapeBattleFuBenData copyData)
			{
				return EscapeBattle_K.ZhengBaKuaFuLogin(_sender_, zhanDuiID, gameId, srcServerID, out copyData);
			}

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M4(int zhanDuiID, out int gameId, out int kuaFuServerID, out string[] ips, out int[] ports)
			{
				return EscapeBattle_K.ZhengBaRequestEnter(zhanDuiID, out gameId, out kuaFuServerID, out ips, out ports);
			}
		}
	}
}
