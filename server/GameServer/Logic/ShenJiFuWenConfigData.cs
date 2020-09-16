using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class ShenJiFuWenConfigData
	{
		
		public ShenJiFuWenEffectData GetEffect(int lev)
		{
			ShenJiFuWenEffectData result;
			if (lev <= 0 || lev > this.ShenJiEffectList.Count)
			{
				result = null;
			}
			else
			{
				result = this.ShenJiEffectList[lev - 1];
			}
			return result;
		}

		
		public int ShenJiID;

		
		public int PreShenJiID;

		
		public int PreShenJiLev;

		
		public int MaxLevel;

		
		public int UpNeed;

		
		public List<ShenJiFuWenEffectData> ShenJiEffectList = new List<ShenJiFuWenEffectData>();
	}
}
