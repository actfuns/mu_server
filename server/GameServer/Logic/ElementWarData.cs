using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class ElementWarData
	{
		
		public ElementWarMonsterConfigInfo GetOrderConfig(int order)
		{
			ElementWarMonsterConfigInfo result;
			if (this.MonsterOrderConfigList.ContainsKey(order))
			{
				result = this.MonsterOrderConfigList[order];
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		
		public int TotalSecs
		{
			get
			{
				return this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs;
			}
		}

		
		public object Mutex = new object();

		
		public int MapID = 70100;

		
		public int CopyID = 70100;

		
		public int MinAwardWave = 0;

		
		public int[] AwardLight;

		
		public int[] YuanSuShiLianAward2;

		
		public int GoodsBinding = 1;

		
		public Dictionary<int, ElementWarMonsterConfigInfo> MonsterOrderConfigList = new Dictionary<int, ElementWarMonsterConfigInfo>();

		
		public int PrepareSecs = 1;

		
		public int FightingSecs = 900;

		
		public int ClearRolesSecs = 15;
	}
}
