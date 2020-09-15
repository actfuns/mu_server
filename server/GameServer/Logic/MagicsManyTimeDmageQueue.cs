using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x02000520 RID: 1312
	public class MagicsManyTimeDmageQueue
	{
		// Token: 0x060018E1 RID: 6369 RVA: 0x00184B7C File Offset: 0x00182D7C
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

		// Token: 0x060018E2 RID: 6370 RVA: 0x00184D78 File Offset: 0x00182F78
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

		// Token: 0x060018E3 RID: 6371 RVA: 0x00184EC8 File Offset: 0x001830C8
		public int GetManyTimeDmageQueueItemNumEx()
		{
			int count;
			lock (this.mutex)
			{
				count = this.execItemDict.Count;
			}
			return count;
		}

		// Token: 0x060018E4 RID: 6372 RVA: 0x00184F1C File Offset: 0x0018311C
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

		// Token: 0x060018E5 RID: 6373 RVA: 0x00185064 File Offset: 0x00183264
		public void AddManyTimeDmageQueueItem(ManyTimeDmageQueueItem manyTimeDmageQueueItem)
		{
			lock (this.ManyTimeDmageQueueItemList)
			{
				this.ManyTimeDmageQueueItemList.Add(manyTimeDmageQueueItem);
			}
		}

		// Token: 0x060018E6 RID: 6374 RVA: 0x001850B8 File Offset: 0x001832B8
		public int GetManyTimeDmageQueueItemNum()
		{
			int count;
			lock (this.ManyTimeDmageQueueItemList)
			{
				count = this.ManyTimeDmageQueueItemList.Count;
			}
			return count;
		}

		// Token: 0x060018E7 RID: 6375 RVA: 0x0018510C File Offset: 0x0018330C
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

		// Token: 0x060018E8 RID: 6376 RVA: 0x001851E4 File Offset: 0x001833E4
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

		// Token: 0x040022E6 RID: 8934
		private object mutex = new object();

		// Token: 0x040022E7 RID: 8935
		private static Dictionary<int, ManyTimeDmageMagicItem> manyTimeDmageQueueItemStaticDict = new Dictionary<int, ManyTimeDmageMagicItem>();

		// Token: 0x040022E8 RID: 8936
		private Dictionary<int, ManyTimeDmageMagicItem> manyTimeDmageQueueItemDict = new Dictionary<int, ManyTimeDmageMagicItem>();

		// Token: 0x040022E9 RID: 8937
		private HashSet<ManyTimeDmageMagicItem> execItemDict = new HashSet<ManyTimeDmageMagicItem>();

		// Token: 0x040022EA RID: 8938
		private List<ManyTimeDmageQueueItem> ManyTimeDmageQueueItemList = new List<ManyTimeDmageQueueItem>();
	}
}
