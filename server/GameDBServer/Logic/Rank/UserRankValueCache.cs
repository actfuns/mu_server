using System;
using System.Collections.Generic;
using GameDBServer.DB;

namespace GameDBServer.Logic.Rank
{
	// Token: 0x02000164 RID: 356
	public class UserRankValueCache
	{
		// Token: 0x06000610 RID: 1552 RVA: 0x00036506 File Offset: 0x00034706
		public void Init(int rid)
		{
			this.roleID = rid;
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x00036510 File Offset: 0x00034710
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

		// Token: 0x06000612 RID: 1554 RVA: 0x000365A0 File Offset: 0x000347A0
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

		// Token: 0x06000613 RID: 1555 RVA: 0x0003661C File Offset: 0x0003481C
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

		// Token: 0x06000614 RID: 1556 RVA: 0x000368A0 File Offset: 0x00034AA0
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

		// Token: 0x06000615 RID: 1557 RVA: 0x00036958 File Offset: 0x00034B58
		public int GetRankValue(RankDataKey key)
		{
			UserRankValue tmpRankData = this.GetRankValueStruct(key);
			return (tmpRankData == null) ? 0 : tmpRankData.RankValue;
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x00036984 File Offset: 0x00034B84
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

		// Token: 0x06000617 RID: 1559 RVA: 0x00036A28 File Offset: 0x00034C28
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

		// Token: 0x06000618 RID: 1560 RVA: 0x00036A94 File Offset: 0x00034C94
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

		// Token: 0x06000619 RID: 1561 RVA: 0x00036B54 File Offset: 0x00034D54
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

		// Token: 0x0400087A RID: 2170
		private int roleID = 0;

		// Token: 0x0400087B RID: 2171
		private static object UserRankValueDictLock = new object();

		// Token: 0x0400087C RID: 2172
		private object UserChargeMoneyCountDicLock = new object();

		// Token: 0x0400087D RID: 2173
		private Dictionary<string, UserRankValue> DictUserRankValue = new Dictionary<string, UserRankValue>();

		// Token: 0x0400087E RID: 2174
		public Dictionary<string, Dictionary<int, int>> ChargeMoneyCountDic = new Dictionary<string, Dictionary<int, int>>();
	}
}
