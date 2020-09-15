using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x020007D5 RID: 2005
	public class MonsterBuffer
	{
		// Token: 0x0600388B RID: 14475 RVA: 0x00303768 File Offset: 0x00301968
		public void AddTempBaseProp(int index, double value, long toTicks)
		{
			lock (this._TempBasePropsDict)
			{
				this._TempBasePropsDict[index] = new TempPropItem
				{
					PropValue = value,
					ToTicks = toTicks
				};
			}
		}

		// Token: 0x0600388C RID: 14476 RVA: 0x003037D0 File Offset: 0x003019D0
		public void AddTempExtProp(int index, double value, long toTicks)
		{
			lock (this._TempExtPropsDict)
			{
				this._TempExtPropsDict[index] = new TempPropItem
				{
					PropValue = value,
					ToTicks = toTicks
				};
			}
		}

		// Token: 0x0600388D RID: 14477 RVA: 0x00303838 File Offset: 0x00301A38
		public void ClearTempBaseProp()
		{
			lock (this._TempBasePropsDict)
			{
				this._TempBasePropsDict.Clear();
			}
		}

		// Token: 0x0600388E RID: 14478 RVA: 0x0030388C File Offset: 0x00301A8C
		public void ClearTempExtProp()
		{
			lock (this._TempExtPropsDict)
			{
				this._TempExtPropsDict.Clear();
			}
		}

		// Token: 0x0600388F RID: 14479 RVA: 0x003038E0 File Offset: 0x00301AE0
		public void Init()
		{
			this.ClearTempBaseProp();
			this.ClearTempExtProp();
		}

		// Token: 0x06003890 RID: 14480 RVA: 0x003038F4 File Offset: 0x00301AF4
		public double GetBaseProp(int index)
		{
			long nowTicks = TimeUtil.NOW() * 10000L;
			double tempProp = 0.0;
			TempPropItem tempPropItem = null;
			lock (this._TempBasePropsDict)
			{
				if (!this._TempBasePropsDict.TryGetValue(index, out tempPropItem))
				{
					return tempProp;
				}
				if (nowTicks - tempPropItem.ToTicks < 0L)
				{
					tempProp = tempPropItem.PropValue;
				}
				else
				{
					this._TempBasePropsDict.Remove(index);
				}
			}
			return tempProp;
		}

		// Token: 0x06003891 RID: 14481 RVA: 0x003039A8 File Offset: 0x00301BA8
		public double GetExtProp(int index)
		{
			long nowTicks = TimeUtil.NOW() * 10000L;
			double tempProp = 0.0;
			TempPropItem tempPropItem = null;
			lock (this._TempExtPropsDict)
			{
				if (!this._TempExtPropsDict.TryGetValue(index, out tempPropItem))
				{
					return tempProp;
				}
				if (nowTicks - tempPropItem.ToTicks < 0L)
				{
					tempProp = tempPropItem.PropValue;
				}
				else
				{
					this._TempExtPropsDict.Remove(index);
				}
			}
			return tempProp;
		}

		// Token: 0x04004162 RID: 16738
		private Dictionary<int, TempPropItem> _TempBasePropsDict = new Dictionary<int, TempPropItem>();

		// Token: 0x04004163 RID: 16739
		private Dictionary<int, TempPropItem> _TempExtPropsDict = new Dictionary<int, TempPropItem>();
	}
}
