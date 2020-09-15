using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	// Token: 0x02000117 RID: 279
	public class BangHuiListMgr
	{
		// Token: 0x060004A4 RID: 1188 RVA: 0x00025D6C File Offset: 0x00023F6C
		public void RefreshBangHuiListData(DBManager DBMgr)
		{
			lock (this.BangHuiListMutex)
			{
				this.BangHuiListCacheData = DBQuery.GetBangHuiItemDataList(DBMgr, -1, 0, 10000);
			}
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x00025DC4 File Offset: 0x00023FC4
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

		// Token: 0x060004A6 RID: 1190 RVA: 0x00025EB4 File Offset: 0x000240B4
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

		// Token: 0x0400077B RID: 1915
		private object BangHuiListMutex = new object();

		// Token: 0x0400077C RID: 1916
		private BangHuiListData BangHuiListCacheData = null;

		// Token: 0x0400077D RID: 1917
		private Dictionary<int, BangHuiCacheData> BangHuiCacheDataDict = new Dictionary<int, BangHuiCacheData>();
	}
}
