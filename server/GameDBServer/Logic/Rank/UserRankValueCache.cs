using System;
using System.Collections.Generic;
using GameDBServer.DB;

namespace GameDBServer.Logic.Rank
{
	
	public class UserRankValueCache
	{
		
		public void Init(int rid)
		{
			this.roleID = rid;
		}

		
		public void Clear()
		{
			lock (UserRankValueCache.UserRankValueDictLock)
			{
				this.DictUserRankValue.Clear();
			}
			lock (this.UserChargeMoneyCountDicLock)
			{
				this.ChargeMoneyCountDic.Clear();
			}
		}

		
		public UserRankValue GetRankValueFromCache(RankDataKey key)
		{
			UserRankValue tmpRankData = null;
			lock (UserRankValueCache.UserRankValueDictLock)
			{
				if (this.DictUserRankValue.ContainsKey(key.GetKey()))
				{
					tmpRankData = this.DictUserRankValue[key.GetKey()];
				}
			}
			return tmpRankData;
		}

		
		public int AddUserRankValue(RankType ActType, int addValue)
		{
			double currSecond = Global.GetOffsetSecond(DateTime.Now);
			lock (UserRankValueCache.UserRankValueDictLock)
			{
				foreach (KeyValuePair<string, UserRankValue> item in this.DictUserRankValue)
				{
					RankDataKey key = RankDataKey.GetKeyFromStr(item.Key.ToString());
					if (null != key)
					{
						double startTime = Global.GetOffsetSecond(DateTime.Parse(key.StartDate));
						double endTime = Global.GetOffsetSecond(DateTime.Parse(key.EndDate));
						if (ActType == key.rankType && currSecond >= startTime && currSecond <= endTime)
						{
							item.Value.RankValue += addValue;
						}
					}
				}
			}
			lock (this.UserChargeMoneyCountDicLock)
			{
				foreach (KeyValuePair<string, Dictionary<int, int>> item2 in this.ChargeMoneyCountDic)
				{
					RankDataKey key = RankDataKey.GetKeyFromStr(item2.Key.ToString());
					if (null != key)
					{
						double startTime = Global.GetOffsetSecond(DateTime.Parse(key.StartDate));
						double endTime = Global.GetOffsetSecond(DateTime.Parse(key.EndDate));
						if (ActType == key.rankType && currSecond >= startTime && currSecond <= endTime)
						{
							if (item2.Value.ContainsKey(addValue))
							{
								Dictionary<int, int> value;
								(value = item2.Value)[addValue] = value[addValue] + 1;
							}
							else
							{
								item2.Value[addValue] = 1;
							}
						}
					}
				}
			}
			return 0;
		}

		
		private UserRankValue GetRankValueStruct(RankDataKey key)
		{
			double currSecond = Global.GetOffsetSecond(DateTime.Now);
			UserRankValue tmpRankData = this.GetRankValueFromCache(key);
			if (null != tmpRankData)
			{
				if (tmpRankData.EndTime >= currSecond)
				{
					return tmpRankData;
				}
				if (tmpRankData.QueryFromDBTime > tmpRankData.EndTime)
				{
					return tmpRankData;
				}
			}
			UserRankValue result;
			lock (UserRankValueCache.UserRankValueDictLock)
			{
				tmpRankData = this.InitRankValue(key);
				this.DictUserRankValue[key.GetKey()] = tmpRankData;
				result = tmpRankData;
			}
			return result;
		}

		
		public int GetRankValue(RankDataKey key)
		{
			UserRankValue tmpRankData = this.GetRankValueStruct(key);
			return (tmpRankData == null) ? 0 : tmpRankData.RankValue;
		}

		
		public UserRankValue InitRankValue(RankDataKey key)
		{
			DBManager dbMgr = DBManager.getInstance();
			UserRankValue result;
			if (null == dbMgr)
			{
				result = null;
			}
			else
			{
				UserRankValue DBRankValue = null;
				if (RankType.Charge == key.rankType)
				{
					DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref this.roleID);
					if (null != roleInfo)
					{
						DBRankValue = this.GetUserInputRankVaule(dbMgr, roleInfo.UserID, roleInfo.ZoneID, key.StartDate, key.EndDate);
					}
				}
				else if (RankType.Consume == key.rankType)
				{
					DBRankValue = this.GetUserConsumeRankValue(dbMgr, key.StartDate, key.EndDate);
				}
				result = DBRankValue;
			}
			return result;
		}

		
		public UserRankValue GetUserInputRankVaule(DBManager dbMgr, string userid, int zoneid, string fromDate, string toDate)
		{
			double currTime = Global.GetOffsetSecond(DateTime.Now);
			int input = DBQuery.GetUserInputMoney(dbMgr, userid, zoneid, fromDate, toDate);
			input = Global.TransMoneyToYuanBao(input);
			return new UserRankValue
			{
				QueryFromDBTime = currTime,
				BeginTime = Global.GetOffsetSecond(DateTime.Parse(fromDate)),
				EndTime = Global.GetOffsetSecond(DateTime.Parse(toDate)),
				RankValue = input
			};
		}

		
		public Dictionary<int, int> GetUserInputMoneyCount(DBManager dbMgr, string userid, int zoneid, string fromDate, string toDate)
		{
			RankDataKey rankDataKey = new RankDataKey
			{
				rankType = RankType.Charge,
				StartDate = fromDate,
				EndDate = toDate
			};
			Dictionary<int, int> retDic = null;
			lock (this.UserChargeMoneyCountDicLock)
			{
				if (this.ChargeMoneyCountDic.TryGetValue(rankDataKey.GetKey(), out retDic))
				{
					return new Dictionary<int, int>(retDic);
				}
				retDic = DBQuery.GetUserDanBiInputMoneyCount(dbMgr, userid, zoneid, fromDate, toDate);
				this.ChargeMoneyCountDic[rankDataKey.GetKey()] = retDic;
			}
			return new Dictionary<int, int>(retDic);
		}

		
		public UserRankValue GetUserConsumeRankValue(DBManager dbMgr, string fromDate, string toDate)
		{
			double currTime = Global.GetOffsetSecond(DateTime.Now);
			int consume = DBQuery.GetUserUsedMoney(dbMgr, this.roleID, fromDate, toDate);
			return new UserRankValue
			{
				QueryFromDBTime = currTime,
				BeginTime = Global.GetOffsetSecond(DateTime.Parse(fromDate)),
				EndTime = Global.GetOffsetSecond(DateTime.Parse(toDate)),
				RankValue = consume
			};
		}

		
		private int roleID = 0;

		
		private static object UserRankValueDictLock = new object();

		
		private object UserChargeMoneyCountDicLock = new object();

		
		private Dictionary<string, UserRankValue> DictUserRankValue = new Dictionary<string, UserRankValue>();

		
		public Dictionary<string, Dictionary<int, int>> ChargeMoneyCountDic = new Dictionary<string, Dictionary<int, int>>();
	}
}
