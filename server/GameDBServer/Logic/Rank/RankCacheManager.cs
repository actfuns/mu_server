using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.Rank
{
	
	public class RankCacheManager
	{
		
		public void PrintfRankData()
		{
			LogManager.WriteLog(LogTypes.Error, "RankDataDict开始输出", null, true);
			lock (this.RankDataDictLock)
			{
				foreach (KeyValuePair<string, RankData> item in this.RankDataDict)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("RankDataKey = {0}", item.Key), null, true);
					foreach (InputKingPaiHangData rankData in item.Value.RankDataList)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("rankData 名次={0}, UserID={1}, 数值={2}, 更新时间={3}", new object[]
						{
							rankData.PaiHang,
							rankData.UserID,
							rankData.PaiHangValue,
							rankData.PaiHangTime
						}), null, true);
					}
				}
			}
			LogManager.WriteLog(LogTypes.Error, "RankDataDict结束输出", null, true);
		}

		
		public void OnUserDoSomething(int roleID, RankType rankType, int value)
		{
			DBManager dbMgr = DBManager.getInstance();
			if (null != dbMgr)
			{
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null != roleInfo)
				{
					double currSecond = Global.GetOffsetSecond(DateTime.Now);
					roleInfo.RankValue.AddUserRankValue(rankType, value);
					lock (this.RankDataDictLock)
					{
						foreach (KeyValuePair<string, RankData> item in this.RankDataDict)
						{
							RankDataKey rankDataKey = RankDataKey.GetKeyFromStr(item.Key);
							if (null != rankDataKey)
							{
								if (rankType == rankDataKey.rankType)
								{
									double startTime = Global.GetOffsetSecond(DateTime.Parse(rankDataKey.StartDate));
									double endTime = Global.GetOffsetSecond(DateTime.Parse(rankDataKey.EndDate));
									if (currSecond >= startTime && currSecond <= endTime)
									{
										bool bExist = false;
										foreach (InputKingPaiHangData rankData in item.Value.RankDataList)
										{
											if ((RankType.Charge == rankType && rankData.UserID == roleInfo.UserID) || (RankType.Consume == rankType && rankData.UserID == roleInfo.RoleID.ToString()))
											{
												rankData.PaiHangTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
												rankData.PaiHangValue += value;
												bExist = true;
												break;
											}
										}
										if (!bExist)
										{
											int userRankValue = roleInfo.RankValue.GetRankValue(rankDataKey);
											InputKingPaiHangData phData = new InputKingPaiHangData
											{
												UserID = ((RankType.Charge == rankType) ? roleInfo.UserID : roleInfo.RoleID.ToString()),
												PaiHang = 0,
												PaiHangTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
												PaiHangValue = userRankValue
											};
											item.Value.RankDataList.Add(phData);
										}
										this.BuildRank(item.Value);
									}
								}
							}
						}
					}
				}
			}
		}

		
		public List<InputKingPaiHangData> GetRankDataList(RankData rankData)
		{
			List<InputKingPaiHangData> result;
			lock (this.RankDataDictLock)
			{
				byte[] retBytes = DataHelper.ObjectToBytes<List<InputKingPaiHangData>>(rankData.RankDataList);
				result = DataHelper.BytesToObject<List<InputKingPaiHangData>>(retBytes, 0, retBytes.Length);
			}
			return result;
		}

		
		public RankData GetRankDataFromCache(RankDataKey key)
		{
			RankData tmpRankData = null;
			lock (this.RankDataDictLock)
			{
				if (this.RankDataDict.ContainsKey(key.GetKey()))
				{
					tmpRankData = this.RankDataDict[key.GetKey()];
				}
			}
			return tmpRankData;
		}

		
		public RankData GetRankData(RankDataKey key, List<int> minGateValueList, int maxPaiHang)
		{
			DBManager dbMgr = DBManager.getInstance();
			RankData result;
			if (null == dbMgr)
			{
				result = null;
			}
			else
			{
				double currSecond = Global.GetOffsetSecond(DateTime.Now);
				RankData tmpRankData = this.GetRankDataFromCache(key);
				if (null != tmpRankData)
				{
					double endTime = Global.GetOffsetSecond(DateTime.Parse(key.EndDate));
					if (endTime >= currSecond)
					{
						return tmpRankData;
					}
					if (tmpRankData.QueryFromDBTime > endTime)
					{
						return tmpRankData;
					}
				}
				lock (this.RankDataDictLock)
				{
					tmpRankData = this.InitRankData(key, minGateValueList, maxPaiHang);
					this.RankDataDict[key.GetKey()] = tmpRankData;
					result = tmpRankData;
				}
			}
			return result;
		}

		
		public RankData InitRankData(RankDataKey key, List<int> minGateValueList, int maxPaiHang)
		{
			DBManager dbMgr = DBManager.getInstance();
			RankData result;
			if (null == dbMgr)
			{
				result = null;
			}
			else
			{
				RankData DBRankData = null;
				if (RankType.Charge == key.rankType)
				{
					DBRankData = this.GetUserInputRank(dbMgr, key.StartDate, key.EndDate, minGateValueList, maxPaiHang);
				}
				else if (RankType.Consume == key.rankType)
				{
					DBRankData = this.GetUserConsumeRank(dbMgr, key.StartDate, key.EndDate, minGateValueList, maxPaiHang);
				}
				result = DBRankData;
			}
			return result;
		}

		
		private void BuildRank(RankData rankData)
		{
			if (null != rankData)
			{
				rankData.RankDataList.Sort(delegate(InputKingPaiHangData x, InputKingPaiHangData y)
				{
					int result;
					if (y.PaiHangValue == x.PaiHangValue)
					{
						double xTime = Global.GetOffsetSecond(DateTime.Parse(x.PaiHangTime));
						double yTime = Global.GetOffsetSecond(DateTime.Parse(y.PaiHangTime));
						result = (int)(xTime - yTime);
					}
					else
					{
						result = y.PaiHangValue - x.PaiHangValue;
					}
					return result;
				});
				List<InputKingPaiHangData> listPaiHang = new List<InputKingPaiHangData>();
				if (null != rankData.minGateValueList)
				{
					int preUserPaiHang = 0;
					for (int i = 0; i < rankData.RankDataList.Count; i++)
					{
						InputKingPaiHangData phData = rankData.RankDataList[i];
						phData.PaiHang = -1;
						for (int j = preUserPaiHang; j < rankData.minGateValueList.Count; j++)
						{
							if (phData.PaiHangValue >= rankData.minGateValueList[j])
							{
								phData.PaiHang = j + 1;
								listPaiHang.Add(phData);
								preUserPaiHang = phData.PaiHang;
								break;
							}
						}
						if (phData.PaiHang < 0 || phData.PaiHang >= rankData.minGateValueList.Count)
						{
							break;
						}
					}
					rankData.RankDataList = listPaiHang;
				}
			}
		}

		
		public RankData GetUserInputRank(DBManager dbMgr, string fromDate, string toDate, List<int> minGateValueList, int maxPaiHang)
		{
			double currTime = Global.GetOffsetSecond(DateTime.Now);
			List<InputKingPaiHangData> listPaiHangReal = DBQuery.GetUserInputPaiHang(dbMgr, fromDate, toDate, maxPaiHang);
			RankData tmpRankData = new RankData();
			tmpRankData.QueryFromDBTime = currTime;
			tmpRankData.MaxRankCount = (double)maxPaiHang;
			tmpRankData.minGateValueList = minGateValueList;
			tmpRankData.RankDataList = listPaiHangReal;
			this.BuildRank(tmpRankData);
			return tmpRankData;
		}

		
		public RankData GetUserConsumeRank(DBManager dbMgr, string fromDate, string toDate, List<int> minGateValueList, int maxPaiHang)
		{
			double currTime = Global.GetOffsetSecond(DateTime.Now);
			List<InputKingPaiHangData> listPaiHangReal = DBQuery.GetUserUsedMoneyPaiHang(dbMgr, fromDate, toDate, maxPaiHang);
			RankData tmpRankData = new RankData();
			tmpRankData.QueryFromDBTime = currTime;
			tmpRankData.MaxRankCount = (double)maxPaiHang;
			tmpRankData.minGateValueList = minGateValueList;
			tmpRankData.RankDataList = listPaiHangReal;
			this.BuildRank(tmpRankData);
			return tmpRankData;
		}

		
		private object RankDataDictLock = new object();

		
		private Dictionary<string, RankData> RankDataDict = new Dictionary<string, RankData>();
	}
}
