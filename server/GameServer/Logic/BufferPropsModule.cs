using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class BufferPropsModule
	{
		
		public void Init(PropsCacheManager _propCacheManager)
		{
			this.propCacheManager = _propCacheManager;
		}

		
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

		
		private const long MinCheckIntervalTicks = 10000L;

		
		private object mutex = new object();

		
		private long MinExpireTicks = 0L;

		
		public Dictionary<long, TimedPropsData> bufferDataDict = new Dictionary<long, TimedPropsData>();

		
		public PropsCacheManager propCacheManager = null;
	}
}
