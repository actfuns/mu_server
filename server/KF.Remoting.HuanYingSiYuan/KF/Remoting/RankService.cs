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
	
	public class RankService
	{
		
		private RankService()
		{
		}

		
		public static RankService getInstance()
		{
			return RankService._Instance;
		}

		
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

		
		public bool IsAgent(int serverID)
		{
			bool isAgent = ClientAgentManager.Instance().ExistAgent(serverID);
			if (!isAgent)
			{
			}
			return isAgent;
		}

		
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

		
		private static readonly RankService _Instance = new RankService();

		
		private object _lockRank = new object();

		
		public readonly GameTypes _gameType = GameTypes.Ally;

		
		private DateTime timeOlympicsBegin = DateTime.MinValue;

		
		private DateTime timeOlympicsEnd = DateTime.MinValue;

		
		private RankPersistence _persistence = RankPersistence.Instance;

		
		private ConcurrentDictionary<int, List<KFRankData>> _oldRankDic = new ConcurrentDictionary<int, List<KFRankData>>();

		
		private ConcurrentDictionary<int, List<KFRankData>> _newRankDic = new ConcurrentDictionary<int, List<KFRankData>>();

		
		private ConcurrentDictionary<int, Dictionary<int, int>> _oldRankIndexDic = new ConcurrentDictionary<int, Dictionary<int, int>>();

		
		private ConcurrentDictionary<int, Dictionary<int, int>> _newRankIndexDic = new ConcurrentDictionary<int, Dictionary<int, int>>();

		
		private ConcurrentDictionary<int, List<KFRankData>> _oldRankTopDic = new ConcurrentDictionary<int, List<KFRankData>>();

		
		private ConcurrentDictionary<int, List<KFRankData>> _newRankTopDic = new ConcurrentDictionary<int, List<KFRankData>>();

		
		private ConcurrentDictionary<int, bool> _rankIsSortDic = new ConcurrentDictionary<int, bool>();

		
		private ConcurrentDictionary<int, RankInfo> _rankInfoDic = new ConcurrentDictionary<int, RankInfo>();

		
		private DateTime _lastUpdateTime = DateTime.MinValue;

		
		private bool _isUpdate = false;
	}
}
