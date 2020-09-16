using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	
	public class zhengDuoService
	{
		
		public static zhengDuoService Instance()
		{
			return zhengDuoService._instance;
		}

		
		public void InitConfig()
		{
			if (!this._config.Load(KuaFuServerManager.GetResourcePath("Config\\PlunderLands.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\PlunderLandsMonster.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\PlunderLandsRebirth.xml", KuaFuServerManager.ResourcePathTypes.GameRes)))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("加载[{0}]时出错!!!", "争夺之地"), null, true);
			}
			else
			{
				this.LoadDBConfig();
			}
		}

		
		private void LoadDBConfig()
		{
			lock (this.Mutex)
			{
				int stepProcessEnd;
				for (;;)
				{
					stepProcessEnd = this._persistence.DBWeekAndStepGet(32);
					if (stepProcessEnd >= 0)
					{
						break;
					}
					Thread.Sleep(2000);
				}
				this.StepProcessEnd = stepProcessEnd;
				DateTime weekStartTime = TimeUtil.GetWeekStartTimeNow();
				DateTime now = TimeUtil.NowDateTime();
				TimeSpan ts = now - weekStartTime;
				if (ts >= this._config.FirstStartTime)
				{
					this.RankTime = weekStartTime;
				}
				else
				{
					this.RankTime = weekStartTime.AddDays(-7.0);
				}
				int week = TimeUtil.GetOffsetDay(this.RankTime);
				this.ReloadRankDatas(week);
				this.SyncData.Age = TimeUtil.AgeByNow(this.SyncData.Age);
			}
		}

		
		private void ReloadRankDatas(int weekDay)
		{
			Dictionary<int, ZhengDuoRankData> dict;
			for (;;)
			{
				dict = this._persistence.DBRankList(weekDay);
				if (dict != null)
				{
					break;
				}
				Thread.Sleep(2000);
			}
			this.RankDict = dict;
			Array.Clear(this.SyncData.RankDatas, 0, this.SyncData.RankDatas.Length);
			for (int i = 0; i < this.SyncData.RankDatas.Length; i++)
			{
				ZhengDuoRankData data;
				if (this.RankDict.TryGetValue(i, out data) && data.Bhid > 0)
				{
					this.SyncData.RankDatas[i] = data;
				}
			}
		}

		
		private ZhengDuoSceneInfo GetCurrentZhengDuoSceneInfo(TimeSpan timeOfWeek)
		{
			foreach (ZhengDuoSceneInfo info in this._config.SceneDataDict.Values)
			{
				if (timeOfWeek >= info.TimeBegin && timeOfWeek < info.NextTime)
				{
					return info;
				}
			}
			return null;
		}

		
		public int GetSuccessRank(EZhengDuoStep step)
		{
			int result;
			switch (step)
			{
			case EZhengDuoStep.Rank16To8:
				result = 8;
				break;
			case EZhengDuoStep.Rank8To4:
				result = 4;
				break;
			case EZhengDuoStep.Rank4To2:
				result = 2;
				break;
			case EZhengDuoStep.Rank2To1:
				result = 1;
				break;
			default:
				result = 16;
				break;
			}
			return result;
		}

		
		public ZhengDuoSyncData ZhengDuoSync(int serverID, long version)
		{
			ZhengDuoSyncData result;
			if (version < this.SyncData.Age)
			{
				result = this.SyncData;
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public int ZhengDuoSign(int serverID, int bhid, int usedTime, int zoneId, string bhName, int bhLevel, long bhZhanLi)
		{
			int week = TimeUtil.GetWeekStartDayIdNow();
			lock (this.Mutex)
			{
				if (this.SyncData.WeekDay == week && this.SyncData.ZhengDuoStep == 1)
				{
					List<ZhengDuoRankData> rankDataList = new List<ZhengDuoRankData>();
					int index = 0;
					for (int i = 0; i < this.SyncData.RankDatas.Length; i++)
					{
						ZhengDuoRankData rankData = this.SyncData.RankDatas[i];
						if (rankData != null && rankData.Bhid != bhid)
						{
							rankDataList.Add(rankData);
							if (rankData.UsedMillisecond < usedTime)
							{
								index = rankDataList.Count;
							}
						}
					}
					if (index < 16)
					{
						ZhengDuoRankData data = new ZhengDuoRankData
						{
							Bhid = bhid,
							UsedMillisecond = usedTime,
							ServerID = serverID,
							ZoneId = zoneId,
							BhName = bhName,
							BhLevel = bhLevel,
							ZhanLi = bhZhanLi,
							Week = week,
							State = 1
						};
						data.Rank1 = index;
						data.Rank2 = 16;
						rankDataList.Insert(index, data);
						LogManager.WriteLog(LogTypes.Info, string.Format("争夺之地提交海选结果#bhid={0},usedTime={1},week={2}", bhid, usedTime, week), null, true);
						for (int i = 0; i < this.SyncData.RankDatas.Length; i++)
						{
							if (i < rankDataList.Count)
							{
								if (this.SyncData.RankDatas[i] != rankDataList[i])
								{
									rankDataList[i].Rank1 = i;
									this._persistence.DBRankUpdata(rankDataList[i]);
								}
								this.SyncData.RankDatas[i] = rankDataList[i];
							}
							else
							{
								this.SyncData.RankDatas[i] = null;
							}
						}
						this.SyncData.Age = TimeUtil.AgeByNow(this.SyncData.Age);
					}
				}
				else
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("争夺之地提交海选结果失败，非海选时间拒绝提交", new object[0]), null, true);
				}
			}
			return 0;
		}

		
		private List<ZhengDuoRankData> GetListByGroup(int rank1, int step)
		{
			List<ZhengDuoRankData> rankDataList = new List<ZhengDuoRankData>();
			List<ZhengDuoRankData> result;
			if (step < 1 || step > 5)
			{
				result = rankDataList;
			}
			else
			{
				bool find = false;
				int rank2 = this.GetSuccessRank((EZhengDuoStep)step);
				int countPerGroup = 16 / rank2;
				for (int i = 0; i < rank2; i++)
				{
					rankDataList.Clear();
					for (int j = 0; j < countPerGroup; j++)
					{
						int r = KFZhengDuoConfig.GroupInfo[i * countPerGroup + j];
						if (r == rank1)
						{
							find = true;
						}
						ZhengDuoRankData rankData = this.SyncData.RankDatas[r];
						if (null != rankData)
						{
							rankDataList.Add(rankData);
						}
					}
					if (find)
					{
						break;
					}
				}
				result = rankDataList;
			}
			return result;
		}

		
		private ZhengDuoRankData GetEnemy(ZhengDuoRankData rankData0, int step)
		{
			int bits = step - 2;
			ZhengDuoRankData result;
			if (bits < 0)
			{
				result = null;
			}
			else
			{
				int rank = this.GetSuccessRank((EZhengDuoStep)step);
				int oldRank = this.GetSuccessRank((EZhengDuoStep)(step - 1));
				lock (this.Mutex)
				{
					if (rankData0 == null || rankData0.Rank2 != oldRank)
					{
						return null;
					}
					int flag0 = rankData0.Rank1 >> bits;
					for (int i = 0; i < this.SyncData.RankDatas.Length; i++)
					{
						ZhengDuoRankData rankData = this.SyncData.RankDatas[i];
						if (rankData != null && rankData.Rank2 == oldRank)
						{
							int flag = rankData.Rank1 >> bits;
							if (flag0 + flag == oldRank)
							{
								return rankData;
							}
						}
					}
				}
				result = null;
			}
			return result;
		}

		
		public int ZhengDuoResult(int bhidSuccess, int[] bhids)
		{
			int result;
			if (bhids == null || bhids.Length < 2)
			{
				result = -18;
			}
			else
			{
				int week = TimeUtil.GetWeekStartDayIdNow();
				lock (this.Mutex)
				{
					if (this.SyncData.WeekDay == week && this.SyncData.ZhengDuoStep > 1)
					{
						int successRank = int.MaxValue;
						int rank = this.GetSuccessRank((EZhengDuoStep)this.SyncData.ZhengDuoStep);
						List<ZhengDuoRankData> rankDataList = new List<ZhengDuoRankData>();
						for (int i = 0; i < this.SyncData.RankDatas.Length; i++)
						{
							ZhengDuoRankData rankData = this.SyncData.RankDatas[i];
							if (rankData != null && bhids.Contains(rankData.Bhid))
							{
								rankDataList.Add(rankData);
								if (bhidSuccess > 0)
								{
									if (bhidSuccess == rankData.Bhid)
									{
										successRank = rankData.Rank1;
									}
								}
								else if (successRank > rankData.Rank1)
								{
									successRank = rankData.Rank1;
								}
							}
						}
						if (rankDataList.Any((ZhengDuoRankData x) => x.Rank2 == rank))
						{
							LogManager.WriteLog(LogTypes.Info, string.Format("争夺之地提交淘汰赛结果失败，已过期#successbhid={0},bhid0={1},bhid1={2},week={3}", new object[]
							{
								bhidSuccess,
								bhids[0],
								bhids[1],
								week
							}), null, true);
							return 0;
						}
						if (successRank >= 16)
						{
							return -18;
						}
						ZhengDuoFuBenData fuBenData;
						if (rankDataList.Count > 0 && this.Bhid2FuBenDict.TryGetValue(rankDataList[0].Bhid, out fuBenData))
						{
							fuBenData.State = GameFuBenState.End;
						}
						LogManager.WriteLog(LogTypes.Info, string.Format("争夺之地提交淘汰赛结果#successbhid={0},bhid0={1},bhid1={2},week={3}", new object[]
						{
							bhidSuccess,
							bhids[0],
							bhids[1],
							week
						}), null, true);
						foreach (ZhengDuoRankData data in rankDataList)
						{
							data.State = 0;
							if (data.Rank1 == successRank)
							{
								data.Rank2 = this.GetSuccessRank((EZhengDuoStep)this.SyncData.ZhengDuoStep);
								data.Enemy = 0;
							}
							else
							{
								data.Lose = 1;
							}
							this._persistence.DBRankUpdata(data);
						}
						this.SyncData.Age = TimeUtil.AgeByNow(this.SyncData.Age);
					}
					else
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("争夺之地提交海选结果失败，非海选时间拒绝提交", new object[0]), null, true);
					}
				}
				result = 0;
			}
			return result;
		}

		
		private void ProcessZhengDuoRank(int step, bool notify = true)
		{
			if (step >= 1)
			{
				int rank = this.GetSuccessRank((EZhengDuoStep)step);
				int oldRank = this.GetSuccessRank((EZhengDuoStep)(step - 1));
				int nextRank = this.GetSuccessRank(step + EZhengDuoStep.Preliminarises);
				lock (this.Mutex)
				{
					bool update = false;
					int i;
					if (step >= 2)
					{
						for (i = 0; i < rank; i++)
						{
							bool complete = false;
							int successRank = int.MaxValue;
							List<ZhengDuoRankData> rankDataList = this.GetListByGroup(i, step);
							foreach (ZhengDuoRankData data in rankDataList)
							{
								if (data.Rank2 == rank)
								{
									complete = true;
								}
								else if (data.Rank2 == oldRank && data.Rank1 < successRank)
								{
									successRank = data.Rank1;
								}
							}
							if (!complete && successRank < 16)
							{
								int[] bhids = new int[2];
								foreach (ZhengDuoRankData data in rankDataList)
								{
									data.State = 0;
									if (data.Rank1 == successRank)
									{
										bhids[0] = data.Bhid;
										data.Rank2 = rank;
										data.Enemy = 0;
									}
									else
									{
										bhids[1] = data.Bhid;
										data.Lose = 1;
									}
									this._persistence.DBRankUpdata(data);
								}
								update = true;
								LogManager.WriteLog(LogTypes.Info, string.Format("争夺之地提交淘汰赛结果，无战斗结果，自动判定#successbhid={0},otherBhid={1},week={2}", bhids[0], bhids[1], this.SyncData.WeekDay), null, true);
							}
						}
					}
					i = 0;
					while (i < nextRank)
					{
						bool complete = false;
						List<int> list = new List<int>();
						List<ZhengDuoRankData> rankDataList = this.GetListByGroup(i, step + 1);
						foreach (ZhengDuoRankData data in rankDataList)
						{
							if (data.Rank2 == rank)
							{
								list.Add(data.Rank1);
							}
							else
							{
								data.Lose = 1;
								this._persistence.DBRankUpdata(data);
							}
						}
						if (list.Count == 2)
						{
							this.SyncData.RankDatas[list[0]].Enemy = this.SyncData.RankDatas[list[1]].Bhid;
							this.SyncData.RankDatas[list[1]].Enemy = this.SyncData.RankDatas[list[0]].Bhid;
							LogManager.WriteLog(LogTypes.Info, string.Format("争夺之地分配对手#{0}<==>{1}", this.SyncData.RankDatas[list[0]].Bhid, this.SyncData.RankDatas[list[1]].Bhid), null, true);
							goto IL_3B9;
						}
						if (list.Count == 1)
						{
							this.SyncData.RankDatas[list[0]].Enemy = 0;
							LogManager.WriteLog(LogTypes.Info, string.Format("争夺之地分配对手#{0}无对手,直接晋级", this.SyncData.RankDatas[list[0]].Bhid), null, true);
							goto IL_3B9;
						}
						IL_410:
						i++;
						continue;
						IL_3B9:
						foreach (int r in list)
						{
							ZhengDuoRankData rankData = this.SyncData.RankDatas[r];
							update = true;
							this._persistence.DBRankUpdata(rankData);
						}
						goto IL_410;
					}
					if (notify && update)
					{
						this.SyncData.Age = TimeUtil.AgeByNow(this.SyncData.Age);
					}
				}
			}
		}

		
		public int GmCommand(string[] args, byte[] data)
		{
			if (args.Length > 0)
			{
				if (args[0] == "-zhengduo")
				{
					int step;
					int state;
					if (args.Length >= 3 && int.TryParse(args[1], out step) && int.TryParse(args[2], out state))
					{
						if (step == 10)
						{
							lock (this.Mutex)
							{
								this.SyncData.Age = TimeUtil.AgeByNow(this.SyncData.Age);
							}
						}
						else
						{
							lock (this.Mutex)
							{
								this.SyncData.ZhengDuoStep = step;
								this.SyncData.State = state;
								if (state > 0)
								{
									this.ProcessZhengDuoRank(this.SyncData.ZhengDuoStep - 1, true);
								}
								else
								{
									this.ProcessZhengDuoRank(this.SyncData.ZhengDuoStep, true);
								}
								this.SyncData.Age = TimeUtil.AgeByNow(this.SyncData.Age);
							}
						}
					}
				}
			}
			return 0;
		}

		
		private void ClearZhengDuoFuBenData()
		{
			lock (this.Mutex)
			{
				if (this.SyncData.State == 0)
				{
					foreach (ZhengDuoFuBenData data in this.FuBenDict.Values)
					{
						try
						{
							ClientAgentManager.Instance().RemoveKfFuben(GameTypes.ZhengDuo, data.ServerId, data.GameId);
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
						}
					}
				}
			}
		}

		
		public ZhengDuoFuBenData GetZhengDuoFuBenDataByBhid(int bhid)
		{
			ZhengDuoFuBenData fuBenData = null;
			lock (this.Mutex)
			{
				if (this.SyncData.ZhengDuoStep < 2 || this.SyncData.State == 0)
				{
					return null;
				}
				if (this.Bhid2FuBenDict.TryGetValue(bhid, out fuBenData) && fuBenData.WeekDay == this.SyncData.WeekDay && fuBenData.GroupIndex == this.SyncData.ZhengDuoStep)
				{
					return fuBenData;
				}
				fuBenData = null;
				int step = this.SyncData.ZhengDuoStep;
				int rank = this.GetSuccessRank((EZhengDuoStep)(step - 1));
				List<ZhengDuoRankData> list = new List<ZhengDuoRankData>();
				for (int i = 0; i < rank; i++)
				{
					List<ZhengDuoRankData> rankDataList = this.GetListByGroup(i, step);
					foreach (ZhengDuoRankData data in rankDataList)
					{
						if (data.Rank2 == rank)
						{
							if (data.Bhid == bhid || data.Enemy == bhid)
							{
								list.Add(data);
							}
						}
					}
					if (list.Count >= 1)
					{
						int serverId = 0;
						long gameId = this._persistence.CreateZhengDuoFuBen(17, serverId);
						if (gameId > 0L && ClientAgentManager.Instance().AssginKfFuben(GameTypes.ZhengDuo, gameId, 60, out serverId))
						{
							fuBenData = new ZhengDuoFuBenData
							{
								GameId = gameId,
								ServerId = serverId,
								GroupIndex = this.SyncData.ZhengDuoStep,
								State = GameFuBenState.Start,
								WeekDay = this.SyncData.WeekDay,
								PlayerDict = new Dictionary<int, int>()
							};
							this.FuBenDict[fuBenData.GameId] = fuBenData;
							fuBenData.PlayerDict[list[0].Bhid] = 1;
							this.Bhid2FuBenDict[list[0].Bhid] = fuBenData;
							if (list.Count >= 2)
							{
								fuBenData.PlayerDict[list[1].Bhid] = 2;
								this.Bhid2FuBenDict[list[1].Bhid] = fuBenData;
							}
							LogManager.WriteLog(LogTypes.Info, string.Format("争夺之地分配副本#gameId={0},serverId={1},{2}<==>{3}", new object[]
							{
								gameId,
								serverId,
								list[0].Bhid,
								(list.Count >= 2) ? list[1].Bhid : 0
							}), null, true);
							break;
						}
					}
				}
			}
			return fuBenData;
		}

		
		public ZhengDuoFuBenData GetZhengDuoFuBenData(long gameId)
		{
			lock (this.Mutex)
			{
				ZhengDuoFuBenData fuBenData;
				if (this.FuBenDict.TryGetValue(gameId, out fuBenData))
				{
					return fuBenData;
				}
			}
			return null;
		}

		
		public void Update(DateTime now)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot2))
			{
				bool updateData = false;
				int weekDay = TimeUtil.GetWeekStartDayIdNow();
				TimeSpan timeOfWeek = TimeUtil.GetTimeOfWeekNow();
				lock (this.Mutex)
				{
					if (this.SyncData.WeekDay != weekDay)
					{
						this.SyncData.WeekDay = weekDay;
						updateData = true;
					}
					if (timeOfWeek < this._config.FirstStartTime)
					{
						this.CurrentSceneInfo = null;
						if (this.SyncData.ZhengDuoStep != 0 || this.SyncData.State > 0)
						{
							this.SyncData.ZhengDuoStep = 0;
							this.SyncData.State = 0;
							updateData = true;
						}
					}
					else if (this.CurrentSceneInfo == null)
					{
						ZhengDuoSceneInfo sceneInfo = this.GetCurrentZhengDuoSceneInfo(timeOfWeek);
						if (null == sceneInfo)
						{
							return;
						}
						int stepOld = this._persistence.DBWeekAndStepGet(31);
						if (sceneInfo.Id == 1)
						{
							this.SyncData.ZhengDuoStep = 1;
							this.StepProcessEnd = 0;
							this.ReloadRankDatas(weekDay);
							LogManager.WriteLog(LogTypes.Info, string.Format("争夺之地,进入海选阶段", new object[0]), null, true);
						}
						else if (stepOld == sceneInfo.Id - 1 || stepOld == sceneInfo.Id)
						{
							LogManager.WriteLog(LogTypes.Info, string.Format("争夺之地,进入{0}阶段", (EZhengDuoStep)sceneInfo.Id), null, true);
							this.SyncData.ZhengDuoStep = sceneInfo.Id;
						}
						else
						{
							this.SyncData.ZhengDuoStep = 0;
							this.SyncData.State = 0;
							LogManager.WriteLog(LogTypes.Info, string.Format("争夺之地,因为前阶段的海选或淘汰赛未开启，本期活动不开启#current={0},last={1}", (EZhengDuoStep)sceneInfo.Id, (EZhengDuoStep)stepOld), null, true);
						}
						if (this.SyncData.ZhengDuoStep > 0)
						{
							while (this.StepProcessEnd < this.SyncData.ZhengDuoStep - 1)
							{
								this.StepProcessEnd++;
								this.ProcessZhengDuoRank(this.StepProcessEnd, false);
								this._persistence.DBWeekAndStepSet(32, this.StepProcessEnd);
							}
							this._persistence.DBWeekAndStepSet(31, this.SyncData.ZhengDuoStep);
							if (timeOfWeek >= sceneInfo.TimeBegin && timeOfWeek < sceneInfo.TimeEnd)
							{
								this.SyncData.State = 1;
								int rank = this.GetSuccessRank((EZhengDuoStep)(this.SyncData.ZhengDuoStep - 1));
								foreach (ZhengDuoRankData data in this.SyncData.RankDatas)
								{
									if (data != null && data.Rank2 == rank && data.Lose == 0)
									{
										data.State = 1;
									}
								}
							}
							else
							{
								this.SyncData.State = 0;
							}
						}
						updateData = true;
						this.CurrentSceneInfo = sceneInfo;
					}
					else
					{
						if (this.SyncData.ZhengDuoStep != this.CurrentSceneInfo.Id)
						{
							return;
						}
						if (timeOfWeek < this.CurrentSceneInfo.TimeEnd)
						{
							if (this.SyncData.State == 0)
							{
								this.SyncData.State = 1;
								updateData = true;
							}
						}
						else if (timeOfWeek < this.CurrentSceneInfo.TimeProcessEnd)
						{
							if (this.SyncData.State == 1)
							{
								LogManager.WriteLog(LogTypes.Info, string.Format("争夺之地,结束战斗状态#step={0}", (EZhengDuoStep)this.CurrentSceneInfo.Id), null, true);
								this.SyncData.State = 0;
								updateData = true;
							}
							if (this.StepProcessEnd < this.CurrentSceneInfo.Id)
							{
								bool end = true;
								foreach (ZhengDuoRankData item in this.SyncData.RankDatas)
								{
									if (item != null && item.State > 0)
									{
										end = false;
										break;
									}
								}
								if (end)
								{
									this.ClearZhengDuoFuBenData();
									this.StepProcessEnd++;
									this.ProcessZhengDuoRank(this.StepProcessEnd, false);
									this._persistence.DBWeekAndStepSet(32, this.StepProcessEnd);
									updateData = true;
								}
							}
						}
						else if (timeOfWeek < this.CurrentSceneInfo.NextTime)
						{
							if (this.SyncData.State == 1)
							{
								this.SyncData.State = 0;
								updateData = true;
							}
							if (this.StepProcessEnd < this.CurrentSceneInfo.Id)
							{
								this.ClearZhengDuoFuBenData();
								this.StepProcessEnd++;
								this.ProcessZhengDuoRank(this.StepProcessEnd, false);
								this._persistence.DBWeekAndStepSet(32, this.StepProcessEnd);
								updateData = true;
							}
							long age = TimeUtil.NOW();
							if (age - this.SyncData.Age > 75000L && timeOfWeek < this.CurrentSceneInfo.TimeProcessEnd)
							{
								updateData = true;
							}
						}
						else
						{
							this.CurrentSceneInfo = null;
						}
					}
					if (updateData)
					{
						this.SyncData.Age = TimeUtil.AgeByNow(this.SyncData.Age);
						ClientAgentManager.Instance().BroadCastAsyncEvent(this.EvItemGameType, new AsyncDataItem(KuaFuEventTypes.UpdateZhengDuoSyncData, new object[]
						{
							this.SyncData
						}), 0);
					}
				}
			}
		}

		
		private const GameTypes GameType = GameTypes.ZhengDuo;

		
		private static zhengDuoService _instance = new zhengDuoService();

		
		private ZhengDuoPersistence _persistence = ZhengDuoPersistence.Instance;

		
		public readonly GameTypes EvItemGameType = GameTypes.TianTi;

		
		public Dictionary<int, ZhengDuoRankData> RankDict = new Dictionary<int, ZhengDuoRankData>();

		
		public Dictionary<long, ZhengDuoFuBenData> FuBenDict = new Dictionary<long, ZhengDuoFuBenData>();

		
		public Dictionary<int, ZhengDuoFuBenData> Bhid2FuBenDict = new Dictionary<int, ZhengDuoFuBenData>();

		
		public ZhengDuoSceneInfo CurrentSceneInfo;

		
		public ZhengDuoSyncData SyncData = new ZhengDuoSyncData();

		
		public DateTime RankTime;

		
		private int StepProcessEnd;

		
		public object Mutex = new object();

		
		private KFZhengDuoConfig _config = new KFZhengDuoConfig();
	}
}
