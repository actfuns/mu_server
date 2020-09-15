using System;
using System.Collections.Generic;
using GameDBServer.Core;
using GameDBServer.Core.Executor;
using GameDBServer.Data;
using GameDBServer.DB.DBController;
using GameDBServer.Server;
using GameDBServer.Server.CmdProcessor;

namespace GameDBServer.Logic
{
	// Token: 0x02000197 RID: 407
	public class ZhanMengShiJianManager : ZhanMengShiJianConstants, ScheduleTask, IManager
	{
		// Token: 0x0600073C RID: 1852 RVA: 0x00042EB4 File Offset: 0x000410B4
		private ZhanMengShiJianManager()
		{
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x00042EE8 File Offset: 0x000410E8
		public static ZhanMengShiJianManager getInstance()
		{
			return ZhanMengShiJianManager.instance;
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x00042F00 File Offset: 0x00041100
		public bool initialize()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(10138, ZhanMengShiJianCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10139, ZhanMengShiJianDetailCmdProcessor.getInstance());
			List<ZhanMengShiJianData> dataList = ZhanMengShiJianDBController.getInstance().getZhanMengShiJianDataList();
			bool result;
			if (null == dataList)
			{
				result = true;
			}
			else
			{
				foreach (ZhanMengShiJianData data in dataList)
				{
					List<ZhanMengShiJianData> _dataList = null;
					if (!this.dataCache.TryGetValue(data.BHID, out _dataList))
					{
						_dataList = new List<ZhanMengShiJianData>();
						this.dataCache.Add(data.BHID, _dataList);
					}
					if (_dataList.Count < ZhanMengShiJianConstants.MaxCacheNum)
					{
						_dataList.Add(data);
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x00042FF4 File Offset: 0x000411F4
		public bool startup()
		{
			this.executor.start();
			this.executor.scheduleExecute(this, 2000L, 5000L);
			return true;
		}

		// Token: 0x06000740 RID: 1856 RVA: 0x0004302C File Offset: 0x0004122C
		public bool showdown()
		{
			this.executor.stop();
			this.deleteData();
			return true;
		}

		// Token: 0x06000741 RID: 1857 RVA: 0x00043054 File Offset: 0x00041254
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
				foreach (List<ZhanMengShiJianData> list in this.dataCache.Values)
				{
					list.Clear();
				}
				this.dataCache.Clear();
				this.dataCache = null;
				result = true;
			}
			return result;
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x000430E8 File Offset: 0x000412E8
		public void run()
		{
			if (TimeUtil.NOW() > this.executeSaveTime)
			{
				this.deleteData();
				this.executeSaveTime += ZhanMengShiJianConstants.deleteTime;
			}
		}

		// Token: 0x06000743 RID: 1859 RVA: 0x00043128 File Offset: 0x00041328
		public List<ZhanMengShiJianData> getDetailByPageIndex(int bhId, int pageIndex)
		{
			int maxPageNum = ZhanMengShiJianConstants.MaxCacheNum / ZhanMengShiJianConstants.PageShowNum;
			List<ZhanMengShiJianData> result;
			if (pageIndex >= maxPageNum)
			{
				result = null;
			}
			else
			{
				List<ZhanMengShiJianData> zhanmengDataList = null;
				if (!this.dataCache.TryGetValue(bhId, out zhanmengDataList))
				{
					result = null;
				}
				else
				{
					int minIndex = pageIndex * ZhanMengShiJianConstants.PageShowNum;
					int getNum = ZhanMengShiJianConstants.PageShowNum;
					if (minIndex >= zhanmengDataList.Count)
					{
						result = null;
					}
					else
					{
						if (minIndex + getNum >= zhanmengDataList.Count)
						{
							getNum = zhanmengDataList.Count - minIndex;
						}
						if (getNum == 0)
						{
							result = null;
						}
						else
						{
							result = zhanmengDataList.GetRange(minIndex, getNum);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x000431D0 File Offset: 0x000413D0
		private void deleteData()
		{
			foreach (int bhId in this.dataCache.Keys)
			{
				List<ZhanMengShiJianData> list = null;
				this.dataCache.TryGetValue(bhId, out list);
				if (list != null && list.Count != 0)
				{
					string minTime = list[list.Count - 1].CreateTime;
					int deleteCount = ZhanMengShiJianDBController.getInstance().delete(bhId, minTime);
				}
			}
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x0004327C File Offset: 0x0004147C
		public void onZhanMengJieSan(int bhId)
		{
			if (this.dataCache.Remove(bhId))
			{
				ZhanMengShiJianDBController.getInstance().delete(bhId);
			}
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x000432AC File Offset: 0x000414AC
		public void onAddZhanMengShiJian(ZhanMengShiJianData data)
		{
			List<ZhanMengShiJianData> _dataList = null;
			lock (this.dataCache)
			{
				if (!this.dataCache.TryGetValue(data.BHID, out _dataList))
				{
					_dataList = new List<ZhanMengShiJianData>();
					this.dataCache.Add(data.BHID, _dataList);
				}
			}
			lock (_dataList)
			{
				_dataList.Insert(0, data);
				if (_dataList.Count > ZhanMengShiJianConstants.MaxCacheNum)
				{
					_dataList.RemoveAt(_dataList.Count - 1);
				}
			}
			ZhanMengShiJianDBController.getInstance().insert(data);
		}

		// Token: 0x04000947 RID: 2375
		private static ZhanMengShiJianManager instance = new ZhanMengShiJianManager();

		// Token: 0x04000948 RID: 2376
		private long executeSaveTime = TimeUtil.NOW() + ZhanMengShiJianConstants.deleteTime;

		// Token: 0x04000949 RID: 2377
		private ScheduleExecutor executor = new ScheduleExecutor(1);

		// Token: 0x0400094A RID: 2378
		private Dictionary<int, List<ZhanMengShiJianData>> dataCache = new Dictionary<int, List<ZhanMengShiJianData>>();
	}
}
