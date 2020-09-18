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
	
	public class ZhanMengShiJianManager : ZhanMengShiJianConstants, ScheduleTask, IManager
	{
		
		private ZhanMengShiJianManager()
		{
		}

		
		public static ZhanMengShiJianManager getInstance()
		{
			return ZhanMengShiJianManager.instance;
		}

		
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

		
		public void run()
		{
			if (TimeUtil.NOW() > this.executeSaveTime)
			{
				this.deleteData();
				this.executeSaveTime += ZhanMengShiJianConstants.deleteTime;
			}
		}

		
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

		
		public void onZhanMengJieSan(int bhId)
		{
			if (this.dataCache.Remove(bhId))
			{
				ZhanMengShiJianDBController.getInstance().delete(bhId);
			}
		}

		
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

		
		private static ZhanMengShiJianManager instance = new ZhanMengShiJianManager();

		
		private long executeSaveTime = TimeUtil.NOW() + ZhanMengShiJianConstants.deleteTime;

		
		private ScheduleExecutor executor = new ScheduleExecutor(1);

		
		private Dictionary<int, List<ZhanMengShiJianData>> dataCache = new Dictionary<int, List<ZhanMengShiJianData>>();
	}
}
