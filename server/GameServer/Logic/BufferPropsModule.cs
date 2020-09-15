using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020001D7 RID: 471
	public class BufferPropsModule
	{
		// Token: 0x060005F7 RID: 1527 RVA: 0x0005460C File Offset: 0x0005280C
		public void Init(PropsCacheManager _propCacheManager)
		{
			this.propCacheManager = _propCacheManager;
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x00054618 File Offset: 0x00052818
		private void UpdateTimedProps(TimedPropsData data, bool enable)
		{
			double propsValue = 0.0;
			if (enable)
			{
				propsValue = data.propsValue;
			}
			if (data.propsType == 1)
			{
				this.propCacheManager.SetExtPropsSingle(new object[]
				{
					PropsSystemTypes.BufferPropsManager,
					data.skillId,
					data.propsType,
					data.propsIndex,
					propsValue
				});
			}
			else if (data.propsType == 0)
			{
				this.propCacheManager.SetBasePropsSingle(new object[]
				{
					PropsSystemTypes.BufferPropsManager,
					data.skillId,
					data.propsType,
					data.propsIndex,
					propsValue
				});
			}
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x0005470C File Offset: 0x0005290C
		public void UpdateTimedPropsData(long nowTicks, long startTicks, int bufferTicks, int propsType, int propsIndex, double propsValue, int skillId, int tag)
		{
			long key = ((long)skillId << 32) + (long)((long)propsType << 24) + (long)propsIndex;
			lock (this.mutex)
			{
				TimedPropsData data;
				if (!this.bufferDataDict.TryGetValue(key, out data))
				{
					data = new TimedPropsData(startTicks, bufferTicks, propsType, propsIndex, propsValue, tag, skillId);
					this.bufferDataDict[key] = data;
				}
				else
				{
					data.startTicks = startTicks;
					data.bufferTicks = bufferTicks;
					data.propsType = propsType;
					data.propsIndex = propsIndex;
					data.propsValue = propsValue;
					data.tag = tag;
					data.skillId = skillId;
					data.endTicks = startTicks + (long)bufferTicks;
				}
				this.UpdateTimedProps(data, true);
				this.TimerUpdateProps(nowTicks, true);
			}
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x000547F4 File Offset: 0x000529F4
		public bool TimerUpdateProps(long nowTicks, bool force = false)
		{
			bool ret = false;
			if (null != this.propCacheManager)
			{
				lock (this.mutex)
				{
					if (!force && nowTicks < this.MinExpireTicks)
					{
						return false;
					}
					this.MinExpireTicks = nowTicks + 10000L;
					List<long> list = new List<long>();
					foreach (KeyValuePair<long, TimedPropsData> kv in this.bufferDataDict)
					{
						long endTicks = kv.Value.endTicks;
						if (endTicks < nowTicks)
						{
							list.Add(kv.Key);
							this.UpdateTimedProps(kv.Value, false);
							if (!ret)
							{
								ret = RoleAlgorithm.NeedNotifyClient((ExtPropIndexes)kv.Value.propsIndex);
							}
						}
						else if (endTicks < this.MinExpireTicks)
						{
							this.MinExpireTicks = endTicks;
						}
					}
					foreach (long key in list)
					{
						this.bufferDataDict.Remove(key);
					}
				}
			}
			return ret;
		}

		// Token: 0x04000A6B RID: 2667
		private const long MinCheckIntervalTicks = 10000L;

		// Token: 0x04000A6C RID: 2668
		private object mutex = new object();

		// Token: 0x04000A6D RID: 2669
		private long MinExpireTicks = 0L;

		// Token: 0x04000A6E RID: 2670
		public Dictionary<long, TimedPropsData> bufferDataDict = new Dictionary<long, TimedPropsData>();

		// Token: 0x04000A6F RID: 2671
		public PropsCacheManager propCacheManager = null;
	}
}
