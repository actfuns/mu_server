using System;
using System.Collections.Generic;
using GameDBServer.Core;
using GameDBServer.Core.Executor;
using GameDBServer.DB.DBController;
using Server.Data;

namespace GameDBServer.Logic.BaiTan
{
	// Token: 0x02000115 RID: 277
	public class BaiTanManager : ScheduleTask, IManager
	{
		// Token: 0x06000496 RID: 1174 RVA: 0x00025688 File Offset: 0x00023888
		private BaiTanManager()
		{
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x000256BC File Offset: 0x000238BC
		public static BaiTanManager getInstance()
		{
			return BaiTanManager.instance;
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x000256D4 File Offset: 0x000238D4
		public bool initialize()
		{
			List<BaiTanLogItemData> dataList = BaiTanLogDBController.getInstance().getBaiTanLogItemDataList();
			bool result;
			if (null == dataList)
			{
				result = true;
			}
			else
			{
				foreach (BaiTanLogItemData data in dataList)
				{
					List<BaiTanLogItemData> _dataList = null;
					if (!this.dataCache.TryGetValue(data.rid, out _dataList))
					{
						_dataList = new List<BaiTanLogItemData>();
						this.dataCache.Add(data.rid, _dataList);
					}
					if (_dataList.Count < BaiTanManager.MaxCacheNum)
					{
						_dataList.Add(data);
						if (!this.dataCache.TryGetValue(data.OtherRoleID, out _dataList))
						{
							_dataList = new List<BaiTanLogItemData>();
							this.dataCache.Add(data.OtherRoleID, _dataList);
						}
						if (_dataList.Count < BaiTanManager.MaxCacheNum)
						{
							_dataList.Add(data);
						}
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x000257F4 File Offset: 0x000239F4
		public bool startup()
		{
			this.executor.start();
			this.executor.scheduleExecute(this, 2000L, 5000L);
			return true;
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x0002582C File Offset: 0x00023A2C
		public bool showdown()
		{
			this.executor.stop();
			this.deleteData();
			return true;
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x00025854 File Offset: 0x00023A54
		public bool destroy()
		{
			this.executor = null;
			bool result;
			if (this.dataCache == null)
			{
				result = true;
			}
			else
			{
				foreach (List<BaiTanLogItemData> list in this.dataCache.Values)
				{
					list.Clear();
				}
				this.dataCache.Clear();
				this.dataCache = null;
				result = true;
			}
			return result;
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x000258E8 File Offset: 0x00023AE8
		public void run()
		{
			if (TimeUtil.NOW() > this.executeSaveTime)
			{
				this.deleteData();
				this.executeSaveTime += BaiTanManager.deleteTime;
			}
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x00025928 File Offset: 0x00023B28
		public List<BaiTanLogItemData> getDetailByPageIndex(int bhId, int pageIndex)
		{
			int maxPageNum = BaiTanManager.MaxCacheNum / BaiTanManager.PageShowNum;
			List<BaiTanLogItemData> result;
			if (pageIndex >= maxPageNum)
			{
				result = null;
			}
			else
			{
				List<BaiTanLogItemData> baiTanLogItemDataList = null;
				if (!this.dataCache.TryGetValue(bhId, out baiTanLogItemDataList))
				{
					result = null;
				}
				else
				{
					int minIndex = pageIndex * BaiTanManager.PageShowNum;
					int getNum = BaiTanManager.PageShowNum;
					if (minIndex >= baiTanLogItemDataList.Count)
					{
						result = null;
					}
					else
					{
						if (minIndex + getNum >= baiTanLogItemDataList.Count)
						{
							getNum = baiTanLogItemDataList.Count - minIndex;
						}
						if (getNum == 0)
						{
							result = null;
						}
						else
						{
							result = baiTanLogItemDataList.GetRange(minIndex, getNum);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x000259D0 File Offset: 0x00023BD0
		private void deleteData()
		{
			foreach (int rid in this.dataCache.Keys)
			{
				List<BaiTanLogItemData> list = null;
				this.dataCache.TryGetValue(rid, out list);
				if (list != null && list.Count != 0)
				{
					string minTime = list[list.Count - 1].BuyTime;
					int deleteCount = BaiTanLogDBController.getInstance().delete(rid, minTime);
				}
			}
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x00025A7C File Offset: 0x00023C7C
		public void onAddBaiTanLog(BaiTanLogItemData data)
		{
			List<BaiTanLogItemData> _dataList = null;
			List<BaiTanLogItemData> _dataList2 = null;
			lock (this.dataCache)
			{
				if (!this.dataCache.TryGetValue(data.rid, out _dataList))
				{
					_dataList = new List<BaiTanLogItemData>();
					this.dataCache.Add(data.rid, _dataList);
				}
				if (!this.dataCache.TryGetValue(data.OtherRoleID, out _dataList2))
				{
					_dataList2 = new List<BaiTanLogItemData>();
					this.dataCache.Add(data.OtherRoleID, _dataList2);
				}
			}
			lock (_dataList)
			{
				_dataList.Insert(0, data);
				if (_dataList.Count > BaiTanManager.MaxCacheNum)
				{
					_dataList.RemoveAt(_dataList.Count - 1);
				}
			}
			lock (_dataList2)
			{
				_dataList2.Insert(0, data);
				if (_dataList2.Count > BaiTanManager.MaxCacheNum)
				{
					_dataList2.RemoveAt(_dataList2.Count - 1);
				}
			}
			BaiTanLogDBController.getInstance().insert(data);
		}

		// Token: 0x04000771 RID: 1905
		public static readonly int PageShowNum = 10;

		// Token: 0x04000772 RID: 1906
		public static readonly int MaxCacheNum = 100;

		// Token: 0x04000773 RID: 1907
		public static readonly long deleteTime = 3600000L;

		// Token: 0x04000774 RID: 1908
		private static BaiTanManager instance = new BaiTanManager();

		// Token: 0x04000775 RID: 1909
		private long executeSaveTime = TimeUtil.NOW() + BaiTanManager.deleteTime;

		// Token: 0x04000776 RID: 1910
		private ScheduleExecutor executor = new ScheduleExecutor(1);

		// Token: 0x04000777 RID: 1911
		private Dictionary<int, List<BaiTanLogItemData>> dataCache = new Dictionary<int, List<BaiTanLogItemData>>();
	}
}
