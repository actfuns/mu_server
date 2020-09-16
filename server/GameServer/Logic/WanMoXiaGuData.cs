using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class WanMoXiaGuData
	{
		
		
		public int TotalSecs
		{
			get
			{
				return this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs;
			}
		}

		
		public object Mutex = new object();

		
		public int MapID = 70300;

		
		public int CopyID = 70300;

		
		public List<List<int>> AwardList;

		
		public int BossMonsterID;

		
		public double WanMoXiaGuCall;

		
		public int GoodsBinding = 1;

		
		public int[] FuBenIds = new int[]
		{
			70300
		};

		
		public Dictionary<int, WanMoXiaGuMonsterConfigInfo> MonsterOrderConfigList = new Dictionary<int, WanMoXiaGuMonsterConfigInfo>();

		
		public int BeginNum;

		
		public int EndNum;

		
		public int PrepareSecs = 1;

		
		public int FightingSecs = 900;

		
		public int ClearRolesSecs = 15;
	}
}
