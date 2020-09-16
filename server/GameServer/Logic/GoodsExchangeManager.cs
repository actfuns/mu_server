using System;
using System.Collections.Generic;
using System.Threading;
using Server.Data;

namespace GameServer.Logic
{
	
	public class GoodsExchangeManager
	{
		
		public int GetNextAutoID()
		{
			return (int)(Interlocked.Increment(ref this.BaseAutoID) & 2147483647L);
		}

		
		public void AddData(int exchangeID, ExchangeData ed)
		{
			lock (this._GoodsExchangeDict)
			{
				this._GoodsExchangeDict[exchangeID] = ed;
			}
		}

		
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

		
		public ExchangeData FindData(int exchangeID)
		{
			ExchangeData ed = null;
			lock (this._GoodsExchangeDict)
			{
				this._GoodsExchangeDict.TryGetValue(exchangeID, out ed);
			}
			return ed;
		}

		
		private long BaseAutoID = 0L;

		
		private Dictionary<int, ExchangeData> _GoodsExchangeDict = new Dictionary<int, ExchangeData>();
	}
}
