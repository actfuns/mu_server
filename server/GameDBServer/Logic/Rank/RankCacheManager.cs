using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.Rank
{
	// Token: 0x02000162 RID: 354
	public class RankCacheManager
	{
		// Token: 0x06000604 RID: 1540 RVA: 0x00035BE4 File Offset: 0x00033DE4
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

		// Token: 0x06000605 RID: 1541 RVA: 0x00035D48 File Offset: 0x00033F48
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

		// Token: 0x06000606 RID: 1542 RVA: 0x0003602C File Offset: 0x0003422C
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

		// Token: 0x06000607 RID: 1543 RVA: 0x0003608C File Offset: 0x0003428C
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

		// Token: 0x06000608 RID: 1544 RVA: 0x00036108 File Offset: 0x00034308
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

		// Token: 0x06000609 RID: 1545 RVA: 0x000361EC File Offset: 0x000343EC
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

		// Token: 0x0600060A RID: 1546 RVA: 0x000362CC File Offset: 0x000344CC
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

		// Token: 0x0600060B RID: 1547 RVA: 0x000363FC File Offset: 0x000345FC
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

		// Token: 0x0600060C RID: 1548 RVA: 0x00036454 File Offset: 0x00034654
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

		// Token: 0x04000873 RID: 2163
		private object RankDataDictLock = new object();

		// Token: 0x04000874 RID: 2164
		private Dictionary<string, RankData> RankDataDict = new Dictionary<string, RankData>();
	}
}
