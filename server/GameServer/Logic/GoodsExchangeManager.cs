using System;
using System.Collections.Generic;
using System.Threading;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020006DE RID: 1758
	public class GoodsExchangeManager
	{
		// Token: 0x060029ED RID: 10733 RVA: 0x0025A524 File Offset: 0x00258724
		public int GetNextAutoID()
		{
			return (int)(Interlocked.Increment(ref this.BaseAutoID) & 2147483647L);
		}

		// Token: 0x060029EE RID: 10734 RVA: 0x0025A54C File Offset: 0x0025874C
		public void AddData(int exchangeID, ExchangeData ed)
		{
			lock (this._GoodsExchangeDict)
			{
				this._GoodsExchangeDict[exchangeID] = ed;
			}
		}

		// Token: 0x060029EF RID: 10735 RVA: 0x0025A5A0 File Offset: 0x002587A0
		public void RemoveData(int exchangeID)
		{
			lock (this._GoodsExchangeDict)
			{
				if (this._GoodsExchangeDict.ContainsKey(exchangeID))
				{
					this._GoodsExchangeDict.Remove(exchangeID);
				}
			}
		}

		// Token: 0x060029F0 RID: 10736 RVA: 0x0025A608 File Offset: 0x00258808
		public ExchangeData FindData(int exchangeID)
		{
			ExchangeData ed = null;
			lock (this._GoodsExchangeDict)
			{
				this._GoodsExchangeDict.TryGetValue(exchangeID, out ed);
			}
			return ed;
		}

		// Token: 0x0400397C RID: 14716
		private long BaseAutoID = 0L;

		// Token: 0x0400397D RID: 14717
		private Dictionary<int, ExchangeData> _GoodsExchangeDict = new Dictionary<int, ExchangeData>();
	}
}
