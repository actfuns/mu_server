using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	// Token: 0x02000046 RID: 70
	public class RankService
	{
		// Token: 0x060002FB RID: 763 RVA: 0x0002A0B0 File Offset: 0x000282B0
		private RankService()
		{
		}

		// Token: 0x060002FC RID: 764 RVA: 0x0002A164 File Offset: 0x00028364
		public static RankService getInstance()
		{
			return RankService._Instance;
		}

		// Token: 0x060002FD RID: 765 RVA: 0x0002A17C File Offset: 0x0002837C
		public long RankVersion(int serverID)
		{
			long result;
			if (!this.IsAgent(serverID))
			{
				result = 0L;
			}
			else
			{
				result = this._persistence.DataVersion;
			}
			return result;
		}

		// Token: 0x060002FE RID: 766 RVA: 0x0002A1AC File Offset: 0x000283AC
		public bool IsAgent(int serverID)
		{
			bool isAgent = ClientAgentManager.Instance().ExistAgent(serverID);
			if (!isAgent)
			{
			}
			return isAgent;
		}

		// Token: 0x060002FF RID: 767 RVA: 0x0002A1D4 File Offset: 0x000283D4
		public int RankGradeUpdate(int serverID, KFRankData newData)
		{
			int result;
			lock (this._lockRank)
			{
				if (newData.RankType == 1)
				{
					this.JudgeClearOlympicsActivityData();
				}
				RankInfo info;
				if (!this._rankInfoDic.TryGetValue(newData.RankType, out info))
				{
					result = 0;
				}
				else
				{
					if (!this._newRankDic.ContainsKey(newData.RankType))
					{
						this._newRankDic.TryAdd(newData.RankType, new List<KFRankData>());
					}
					if (!this._newRankIndexDic.ContainsKey(newData.RankType))
					{
						this._newRankIndexDic.TryAdd(newData.RankType, new Dictionary<int, int>());
					}
					List<KFRankData> rankDataList = this._newRankDic[newData.RankType];
					Dictionary<int, int> indexDic = this._newRankIndexDic[newData.RankType];
					List<KFRankData> topList = this._newRankTopDic[newData.RankType];
					KFRankData oldData;
					if (!indexDic.ContainsKey(newData.RoleID))
					{
						oldData = newData;
						oldData.Rank = rankDataList.Count;
						rankDataList.Add(oldData);
						indexDic.Add(oldData.RoleID, oldData.Rank);
						if (info.RankTopCount > topList.Count)
						{
							topList.Add(oldData);
						}
					}
					else
					{
						int index = indexDic[newData.RoleID];
						oldData = rankDataList[index];
						oldData.GradeOld = oldData.Grade;
						oldData.Grade = newData.Grade;
						oldData.ServerID = newData.ServerID;
						oldData.RoleData = newData.RoleData;
					}
					this._persistence.DBRankDataUpdate(oldData);
					if (this._rankIsSortDic.ContainsKey(info.RankType))
					{
						this._rankIsSortDic[info.RankType] = true;
					}
					else
					{
						this._rankIsSortDic.TryAdd(info.RankType, true);
					}
					if (info.RankRefreshSpanType != 1)
					{
						ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(KuaFuEventTypes.RankPerson, new object[]
						{
							oldData
						}));
					}
					result = 1;
				}
			}
			return result;
		}

		// Token: 0x06000300 RID: 768 RVA: 0x0002A43C File Offset: 0x0002863C
		public List<KFRankData> FilterRankTopList(List<KFRankData> _RankTopList, RankInfo info)
		{
			List<KFRankData> result;
			if (_RankTopList.Count <= info.RankRoleCount)
			{
				result = _RankTopList;
			}
			else
			{
				List<KFRankData> RankTopListFilter = new List<KFRankData>();
				for (int index = 0; index < _RankTopList.Count; index++)
				{
					if (index < info.RankRoleCount)
					{
						RankTopListFilter.Add(_RankTopList[index]);
					}
					else
					{
						KFRankData rankinfo = new KFRankData();
						rankinfo.Clone(_RankTopList[index]);
						rankinfo.RoleData = null;
						RankTopListFilter.Add(rankinfo);
					}
				}
				result = RankTopListFilter;
			}
			return result;
		}

		// Token: 0x06000301 RID: 769 RVA: 0x0002A4CC File Offset: 0x000286CC
		public List<KFRankData> RankTopList(int serverID, int rankType)
		{
			List<KFRankData> result;
			lock (this._lockRank)
			{
				if (rankType == 1)
				{
					this.JudgeClearOlympicsActivityData();
				}
				RankInfo info;
				if (!this._rankInfoDic.TryGetValue(rankType, out info))
				{
					result = null;
				}
				else
				{
					List<KFRankData> _RankTopList;
					if (info.RankListType == 1)
					{
						if (!this._oldRankTopDic.ContainsKey(rankType))
						{
							return null;
						}
						_RankTopList = this._oldRankTopDic[rankType];
					}
					else
					{
						if (!this._newRankTopDic.ContainsKey(rankType))
						{
							return null;
						}
						_RankTopList = this._newRankTopDic[rankType];
					}
					result = this.FilterRankTopList(_RankTopList, info);
				}
			}
			return result;
		}

		// Token: 0x06000302 RID: 770 RVA: 0x0002A5AC File Offset: 0x000287AC
		public KFRankData RankRole(int serverID, int rankType, int roleID)
		{
			KFRankData result;
			lock (this._lockRank)
			{
				RankInfo info;
				if (!this._rankInfoDic.TryGetValue(rankType, out info))
				{
					result = null;
				}
				else
				{
					ConcurrentDictionary<int, List<KFRankData>> rankDataDic;
					ConcurrentDictionary<int, Dictionary<int, int>> rankIndexDic;
					if (info.RankListType == 1)
					{
						rankDataDic = this._oldRankDic;
						rankIndexDic = this._oldRankIndexDic;
					}
					else
					{
						rankDataDic = this._newRankDic;
						rankIndexDic = this._newRankIndexDic;
					}
					if (rankDataDic == null || !rankDataDic.ContainsKey(rankType))
					{
						result = null;
					}
					else if (rankIndexDic == null || !rankIndexDic.ContainsKey(rankType))
					{
						result = null;
					}
					else
					{
						List<KFRankData> dataList = rankDataDic[rankType];
						Dictionary<int, int> indexDic = rankIndexDic[rankType];
						if (!indexDic.ContainsKey(roleID))
						{
							result = null;
						}
						else
						{
							int index = indexDic[roleID];
							if (index >= dataList.Count)
							{
								result = null;
							}
							else
							{
								result = dataList[index];
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0002A6CC File Offset: 0x000288CC
		public void StartUp()
		{
			try
			{
				this.InitRankConfig();
				this.InitRankDB();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "RankService.StartUp failed!", ex, true);
			}
		}

		// Token: 0x06000304 RID: 772 RVA: 0x0002A710 File Offset: 0x00028910
		private void InitRankConfig()
		{
			this._rankInfoDic.Clear();
			this._rankInfoDic.TryAdd(1, new RankInfo
			{
				RankType = 1,
				RankMax = 50000,
				RankTopCount = 100,
				RankRoleCount = 3,
				RankListType = 1,
				RankRefreshSpanType = 1,
				RankRefreshTime = DateTime.Parse(string.Format("{0}-{1}-{2} {3}", new object[]
				{
					TimeUtil.NowDateTime().Year,
					TimeUtil.NowDateTime().Month,
					TimeUtil.NowDateTime().Day,
					"23:59:59"
				})),
				RankRefreshSecondTick = 0
			});
			string str = KuaFuServerManager.systemParamsList.GetParamValueByName("AoYunTime");
			if (!string.IsNullOrEmpty(str))
			{
				string[] arr = str.Split(new char[]
				{
					','
				});
				if (arr != null && arr.Length == 2)
				{
					DateTime.TryParse(arr[0], out this.timeOlympicsBegin);
					DateTime.TryParse(arr[1], out this.timeOlympicsEnd);
				}
			}
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0002A848 File Offset: 0x00028A48
		private void JudgeClearOlympicsActivityData()
		{
			DateTime timeNow = TimeUtil.NowDateTime();
			if (!(DateTime.MinValue == this.timeOlympicsBegin) && !(timeNow < this.timeOlympicsBegin.AddSeconds(-300.0)))
			{
				int KeyDate = this.timeOlympicsBegin.Year * 10000 + this.timeOlympicsBegin.Month * 100 + this.timeOlympicsBegin.Day;
				object oldDate = DbHelperMySQL.GetSingle("select value from t_async where id = " + 33);
				if (null == oldDate)
				{
					DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 33, KeyDate));
				}
				else if ((int)oldDate != KeyDate)
				{
					int RankType = 1;
					this._persistence.DBRankDelByType(RankType);
					if (this._newRankDic.ContainsKey(RankType))
					{
						this._newRankDic[RankType].Clear();
					}
					else
					{
						this._newRankDic.TryAdd(RankType, new List<KFRankData>());
					}
					if (this._newRankIndexDic.ContainsKey(RankType))
					{
						this._newRankIndexDic[RankType].Clear();
					}
					else
					{
						this._newRankIndexDic.TryAdd(RankType, new Dictionary<int, int>());
					}
					if (this._newRankTopDic.ContainsKey(RankType))
					{
						this._newRankTopDic[RankType].Clear();
					}
					else
					{
						this._newRankTopDic.TryAdd(RankType, new List<KFRankData>());
					}
					this.RankSort(RankType);
					DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 33, KeyDate));
				}
			}
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0002AA0C File Offset: 0x00028C0C
		private void InitRankDB()
		{
			this._newRankDic.Clear();
			this._oldRankDic.Clear();
			this._newRankIndexDic.Clear();
			this._oldRankIndexDic.Clear();
			this._newRankTopDic.Clear();
			this._oldRankTopDic.Clear();
			foreach (RankInfo info in this._rankInfoDic.Values)
			{
				if (info.RankType == 1)
				{
					this.JudgeClearOlympicsActivityData();
				}
				List<KFRankData> list = this._persistence.DBRankLoad(info.RankType, info.RankMax);
				this._newRankDic.TryAdd(info.RankType, list);
				this._rankIsSortDic.TryAdd(info.RankType, true);
				this.RankSort(info.RankType);
			}
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0002AB10 File Offset: 0x00028D10
		public void Update()
		{
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				if (!this._isUpdate && (now - this._lastUpdateTime).TotalMilliseconds >= 1000.0)
				{
					this._isUpdate = true;
					foreach (RankInfo info in this._rankInfoDic.Values)
					{
						if (!(now < info.RankRefreshTime))
						{
							this.RankSort(info.RankType);
							if (info.RankRefreshSpanType == 1)
							{
								info.RankRefreshTime = info.RankRefreshTime.AddDays(1.0);
							}
							else if (info.RankRefreshSpanType == 2)
							{
								info.RankRefreshTime = info.RankRefreshTime.AddSeconds((double)info.RankRefreshSecondTick);
							}
						}
					}
					this._isUpdate = false;
					this._lastUpdateTime = now;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "RankService.Update failed!", ex, true);
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x0002AC84 File Offset: 0x00028E84
		private void RankSort(int rankType)
		{
			lock (this._lockRank)
			{
				bool isSort = false;
				if (this._rankIsSortDic.TryGetValue(rankType, out isSort))
				{
					if (isSort)
					{
						this._rankIsSortDic[rankType] = false;
						RankInfo rankInfo;
						if (this._rankInfoDic.TryGetValue(rankType, out rankInfo))
						{
							List<KFRankData> newList = null;
							if (this._newRankDic.TryGetValue(rankType, out newList))
							{
								if (newList != null)
								{
									newList.Sort();
									List<KFRankData> oldList = null;
									Dictionary<int, int> oldIndexDic = null;
									List<KFRankData> oldTopList = null;
									if (this._newRankIndexDic.ContainsKey(rankType))
									{
										this._newRankIndexDic[rankType].Clear();
									}
									else
									{
										this._newRankIndexDic.TryAdd(rankType, new Dictionary<int, int>());
									}
									if (this._newRankTopDic.ContainsKey(rankType))
									{
										this._newRankTopDic[rankType].Clear();
									}
									else
									{
										this._newRankTopDic.TryAdd(rankType, new List<KFRankData>());
									}
									if (rankInfo.RankListType == 1)
									{
										if (this._oldRankDic.ContainsKey(rankType))
										{
											this._oldRankDic[rankType].Clear();
										}
										else
										{
											this._oldRankDic.TryAdd(rankType, new List<KFRankData>());
										}
										if (this._oldRankIndexDic.ContainsKey(rankType))
										{
											this._oldRankIndexDic[rankType].Clear();
										}
										else
										{
											this._oldRankIndexDic.TryAdd(rankType, new Dictionary<int, int>());
										}
										if (this._oldRankTopDic.ContainsKey(rankType))
										{
											this._oldRankTopDic[rankType].Clear();
										}
										else
										{
											this._oldRankTopDic.TryAdd(rankType, new List<KFRankData>());
										}
										oldList = this._oldRankDic[rankType];
										oldIndexDic = this._oldRankIndexDic[rankType];
										oldTopList = this._oldRankTopDic[rankType];
									}
									int index = 0;
									Dictionary<int, int> newIndexDic = this._newRankIndexDic[rankType];
									List<KFRankData> newTopList = this._newRankTopDic[rankType];
									bool isList = false;
									List<KFRankData> delList = new List<KFRankData>();
									foreach (KFRankData data in newList)
									{
										data.RankOld = data.Rank;
										index = (data.Rank = index + 1);
										if (data.RankOld != data.Rank || data.GradeOld != data.Grade)
										{
											if (this.IsAgent(data.ServerID) && rankInfo.RankRefreshSpanType != 1)
											{
												ClientAgentManager.Instance().PostAsyncEvent(data.ServerID, this._gameType, new AsyncDataItem(KuaFuEventTypes.RankPerson, new object[]
												{
													data
												}));
											}
											if (index <= rankInfo.RankTopCount)
											{
												isList = true;
											}
										}
										if ((double)index > (double)rankInfo.RankMax * 1.2)
										{
											delList.Add(data);
										}
										else
										{
											newIndexDic.Add(data.RoleID, index - 1);
											if (index <= rankInfo.RankTopCount)
											{
												newTopList.Add(data);
											}
											if (rankInfo.RankListType == 1)
											{
												KFRankData oldData = new KFRankData();
												oldData.Clone(data);
												oldList.Add(oldData);
												oldIndexDic.Add(oldData.RoleID, index - 1);
												if (index <= rankInfo.RankTopCount)
												{
													oldTopList.Add(oldData);
												}
											}
										}
									}
									if (isList)
									{
										List<KFRankData> filterRankTopList = this.FilterRankTopList(newTopList, rankInfo);
										ClientAgentManager.Instance().BroadCastAsyncEvent(this._gameType, new AsyncDataItem(KuaFuEventTypes.RankTop, new object[]
										{
											rankType,
											rankInfo.RankRefreshSpanType,
											filterRankTopList
										}), 0);
									}
									this._persistence.DBRankUpdate(newList);
									if (delList.Count > 0)
									{
										foreach (KFRankData d in delList)
										{
											newList.Remove(d);
										}
										this._persistence.DBRankDelMore(rankInfo.RankType, (int)((double)rankInfo.RankMax * 1.2));
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x040001B6 RID: 438
		private static readonly RankService _Instance = new RankService();

		// Token: 0x040001B7 RID: 439
		private object _lockRank = new object();

		// Token: 0x040001B8 RID: 440
		public readonly GameTypes _gameType = GameTypes.Ally;

		// Token: 0x040001B9 RID: 441
		private DateTime timeOlympicsBegin = DateTime.MinValue;

		// Token: 0x040001BA RID: 442
		private DateTime timeOlympicsEnd = DateTime.MinValue;

		// Token: 0x040001BB RID: 443
		private RankPersistence _persistence = RankPersistence.Instance;

		// Token: 0x040001BC RID: 444
		private ConcurrentDictionary<int, List<KFRankData>> _oldRankDic = new ConcurrentDictionary<int, List<KFRankData>>();

		// Token: 0x040001BD RID: 445
		private ConcurrentDictionary<int, List<KFRankData>> _newRankDic = new ConcurrentDictionary<int, List<KFRankData>>();

		// Token: 0x040001BE RID: 446
		private ConcurrentDictionary<int, Dictionary<int, int>> _oldRankIndexDic = new ConcurrentDictionary<int, Dictionary<int, int>>();

		// Token: 0x040001BF RID: 447
		private ConcurrentDictionary<int, Dictionary<int, int>> _newRankIndexDic = new ConcurrentDictionary<int, Dictionary<int, int>>();

		// Token: 0x040001C0 RID: 448
		private ConcurrentDictionary<int, List<KFRankData>> _oldRankTopDic = new ConcurrentDictionary<int, List<KFRankData>>();

		// Token: 0x040001C1 RID: 449
		private ConcurrentDictionary<int, List<KFRankData>> _newRankTopDic = new ConcurrentDictionary<int, List<KFRankData>>();

		// Token: 0x040001C2 RID: 450
		private ConcurrentDictionary<int, bool> _rankIsSortDic = new ConcurrentDictionary<int, bool>();

		// Token: 0x040001C3 RID: 451
		private ConcurrentDictionary<int, RankInfo> _rankInfoDic = new ConcurrentDictionary<int, RankInfo>();

		// Token: 0x040001C4 RID: 452
		private DateTime _lastUpdateTime = DateTime.MinValue;

		// Token: 0x040001C5 RID: 453
		private bool _isUpdate = false;
	}
}
