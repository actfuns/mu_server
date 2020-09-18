using System;
using System.Collections.Generic;
using GameDBServer.Core;
using GameDBServer.Core.Executor;
using GameDBServer.DB.DBController;
using Server.Data;

namespace GameDBServer.Logic.BaiTan
{
	
	public class BaiTanManager : ScheduleTask, IManager
	{
		
		private BaiTanManager()
		{
		}

		
		public static BaiTanManager getInstance()
		{
			return BaiTanManager.instance;
		}

		
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

		
		public bool startup()
		{
			this.executor.start();
			this.executor.scheduleExecute(this, 2000L, 5000L);
			return true;
		}

		
		public bool showdown()
		{
			this.executor.stop();
			this.deleteData();
			return true;
		}

		
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

		
		public void run()
		{
			if (TimeUtil.NOW() > this.executeSaveTime)
			{
				this.deleteData();
				this.executeSaveTime += BaiTanManager.deleteTime;
			}
		}

		
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

		
		public static readonly int PageShowNum = 10;

		
		public static readonly int MaxCacheNum = 100;

		
		public static readonly long deleteTime = 3600000L;

		
		private static BaiTanManager instance = new BaiTanManager();

		
		private long executeSaveTime = TimeUtil.NOW() + BaiTanManager.deleteTime;

		
		private ScheduleExecutor executor = new ScheduleExecutor(1);

		
		private Dictionary<int, List<BaiTanLogItemData>> dataCache = new Dictionary<int, List<BaiTanLogItemData>>();
	}
}
