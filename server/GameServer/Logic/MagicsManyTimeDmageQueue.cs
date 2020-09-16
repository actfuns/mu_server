using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	public class MagicsManyTimeDmageQueue
	{
		
		public bool AddManyTimeDmageQueueItemEx(int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode)
		{
			Lazy<long> startTicks = new Lazy<long>(() => TimeUtil.NOW());
			Lazy<List<ManyTimeDmageItem>> lazyList = new Lazy<List<ManyTimeDmageItem>>(() => MagicsManyTimeDmageCachingMgr.GetManyTimeDmageItems(magicCode));
			bool result;
			lock (this.mutex)
			{
				ManyTimeDmageMagicItem item;
				if (this.manyTimeDmageQueueItemDict.TryGetValue(magicCode, out item))
				{
					if (item.itemList == null)
					{
						result = false;
					}
					else
					{
						if (item.Start(startTicks.Value, magicCode, enemy, enemyX, enemyY, realEnemyX, realEnemyY))
						{
							this.execItemDict.Add(item);
						}
						result = true;
					}
				}
				else
				{
					ManyTimeDmageMagicItem itemStatic;
					if (!MagicsManyTimeDmageQueue.manyTimeDmageQueueItemStaticDict.TryGetValue(magicCode, out itemStatic))
					{
						itemStatic = new ManyTimeDmageMagicItem();
						itemStatic.itemList = lazyList.Value;
						MagicsManyTimeDmageQueue.manyTimeDmageQueueItemStaticDict[magicCode] = itemStatic;
					}
					if (!this.manyTimeDmageQueueItemDict.TryGetValue(magicCode, out item))
					{
						item = new ManyTimeDmageMagicItem();
						item.itemList = itemStatic.itemList;
						this.manyTimeDmageQueueItemDict[magicCode] = item;
					}
					if (item.itemList == null)
					{
						result = false;
					}
					else
					{
						if (item.Start(startTicks.Value, magicCode, enemy, enemyX, enemyY, realEnemyX, realEnemyY))
						{
							this.execItemDict.Add(item);
						}
						result = true;
					}
				}
			}
			return result;
		}

		
		public bool AddDelayMagicItemEx(int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode)
		{
			Lazy<long> startTicks = new Lazy<long>(() => TimeUtil.NOW());
			bool result;
			lock (this.mutex)
			{
				ManyTimeDmageMagicItem item;
				if (this.manyTimeDmageQueueItemDict.TryGetValue(magicCode, out item))
				{
					if (item.Start(startTicks.Value, magicCode, enemy, enemyX, enemyY, realEnemyX, realEnemyY))
					{
						this.execItemDict.Add(item);
					}
					result = true;
				}
				else
				{
					ManyTimeDmageMagicItem itemStatic;
					if (!MagicsManyTimeDmageQueue.manyTimeDmageQueueItemStaticDict.TryGetValue(magicCode, out itemStatic))
					{
						itemStatic = new ManyTimeDmageMagicItem();
						MagicsManyTimeDmageQueue.manyTimeDmageQueueItemStaticDict[magicCode] = itemStatic;
					}
					if (!this.manyTimeDmageQueueItemDict.TryGetValue(magicCode, out item))
					{
						item = new ManyTimeDmageMagicItem();
						item.itemList = itemStatic.itemList;
						this.manyTimeDmageQueueItemDict[magicCode] = item;
					}
					if (item.Start(startTicks.Value, magicCode, enemy, enemyX, enemyY, realEnemyX, realEnemyY))
					{
						this.execItemDict.Add(item);
					}
					result = true;
				}
			}
			return result;
		}

		
		public int GetManyTimeDmageQueueItemNumEx()
		{
			int count;
			lock (this.mutex)
			{
				count = this.execItemDict.Count;
			}
			return count;
		}

		
		public ManyTimeDmageMagicItem GetCanExecItemsEx(out ManyTimeDmageItem subItem)
		{
			ManyTimeDmageMagicItem magicItem = null;
			subItem = null;
			long ticks = TimeUtil.NowEx();
			lock (this.mutex)
			{
				List<ManyTimeDmageMagicItem> removeList = null;
				foreach (ManyTimeDmageMagicItem item in this.execItemDict)
				{
					if (ticks > item.execTicks)
					{
						magicItem = item;
						subItem = magicItem.Get();
						if (!magicItem.Next())
						{
							if (null == removeList)
							{
								removeList = new List<ManyTimeDmageMagicItem>();
							}
							removeList.Add(item);
						}
						break;
					}
				}
				if (null != removeList)
				{
					foreach (ManyTimeDmageMagicItem item in removeList)
					{
						this.execItemDict.Remove(item);
					}
				}
			}
			return magicItem;
		}

		
		public void AddManyTimeDmageQueueItem(ManyTimeDmageQueueItem manyTimeDmageQueueItem)
		{
			lock (this.ManyTimeDmageQueueItemList)
			{
				this.ManyTimeDmageQueueItemList.Add(manyTimeDmageQueueItem);
			}
		}

		
		public int GetManyTimeDmageQueueItemNum()
		{
			int count;
			lock (this.ManyTimeDmageQueueItemList)
			{
				count = this.ManyTimeDmageQueueItemList.Count;
			}
			return count;
		}

		
		public List<ManyTimeDmageQueueItem> GetCanExecItems()
		{
			long ticks = TimeUtil.NOW();
			List<ManyTimeDmageQueueItem> canExecItemList = new List<ManyTimeDmageQueueItem>();
			lock (this.ManyTimeDmageQueueItemList)
			{
				for (int i = 0; i < this.ManyTimeDmageQueueItemList.Count; i++)
				{
					if (ticks >= this.ManyTimeDmageQueueItemList[i].ToExecTicks)
					{
						canExecItemList.Add(this.ManyTimeDmageQueueItemList[i]);
					}
				}
				for (int i = 0; i < canExecItemList.Count; i++)
				{
					this.ManyTimeDmageQueueItemList.Remove(canExecItemList[i]);
				}
			}
			return canExecItemList;
		}

		
		public void Clear()
		{
			lock (this.ManyTimeDmageQueueItemList)
			{
				this.ManyTimeDmageQueueItemList.Clear();
			}
			lock (this.mutex)
			{
				this.manyTimeDmageQueueItemDict.Clear();
				this.execItemDict.Clear();
			}
		}

		
		private object mutex = new object();

		
		private static Dictionary<int, ManyTimeDmageMagicItem> manyTimeDmageQueueItemStaticDict = new Dictionary<int, ManyTimeDmageMagicItem>();

		
		private Dictionary<int, ManyTimeDmageMagicItem> manyTimeDmageQueueItemDict = new Dictionary<int, ManyTimeDmageMagicItem>();

		
		private HashSet<ManyTimeDmageMagicItem> execItemDict = new HashSet<ManyTimeDmageMagicItem>();

		
		private List<ManyTimeDmageQueueItem> ManyTimeDmageQueueItemList = new List<ManyTimeDmageQueueItem>();
	}
}
