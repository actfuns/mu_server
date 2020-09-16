using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class HeFuRechargeActivity : Activity
	{
		
		public HeFuRechargeData getDataByDay(int rank)
		{
			HeFuRechargeData data = null;
			if (this.ConfigDict.ContainsKey(rank))
			{
				data = this.ConfigDict[rank];
			}
			return data;
		}

		
		public Dictionary<int, HeFuRechargeData> ConfigDict = new Dictionary<int, HeFuRechargeData>();

		
		public string strcoe;
	}
}
