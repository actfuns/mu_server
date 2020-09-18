using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	
	public class BangHuiListMgr
	{
		
		public void RefreshBangHuiListData(DBManager DBMgr)
		{
			lock (this.BangHuiListMutex)
			{
				this.BangHuiListCacheData = DBQuery.GetBangHuiItemDataList(DBMgr, -1, 0, 10000);
			}
		}

		
		public BangHuiListData GetBangHuiListData(DBManager DBMgr, int isVerify, int startIndex, int endIndex)
		{
			BangHuiListData BHListData = null;
			lock (this.BangHuiListMutex)
			{
				if (isVerify >= 0)
				{
					BHListData = new BangHuiListData();
					BHListData.BangHuiItemDataList = new List<BangHuiItemData>();
					foreach (BangHuiItemData data in this.BangHuiListCacheData.BangHuiItemDataList)
					{
						if (isVerify == data.IsVerfiy)
						{
							BHListData.BangHuiItemDataList.Add(data);
							BHListData.TotalBangHuiItemNum++;
						}
					}
				}
				else
				{
					BHListData = this.BangHuiListCacheData;
				}
			}
			return BHListData;
		}

		
		public BangHuiCacheData GetBangHuiCacheData(int bhid)
		{
			BangHuiCacheData result;
			lock (this.BangHuiListMutex)
			{
				BangHuiCacheData data;
				if (!this.BangHuiCacheDataDict.TryGetValue(bhid, out data))
				{
					data = new BangHuiCacheData();
					if (!data.Query(bhid))
					{
						return null;
					}
					this.BangHuiCacheDataDict[bhid] = data;
				}
				result = data;
			}
			return result;
		}

		
		private object BangHuiListMutex = new object();

		
		private BangHuiListData BangHuiListCacheData = null;

		
		private Dictionary<int, BangHuiCacheData> BangHuiCacheDataDict = new Dictionary<int, BangHuiCacheData>();
	}
}
