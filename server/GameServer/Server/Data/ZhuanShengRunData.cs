using System;
using System.Collections.Generic;
using GameServer.Logic;

namespace Server.Data
{
	
	public class ZhuanShengRunData
	{
		
		public const int MaxAttackRankNum = 20;

		
		public const int MaxAttackRankNumClient = 5;

		
		public object Mutex = new object();

		
		public Dictionary<int, ZhuanShengMapInfo> ZhuanShengMapDict = new Dictionary<int, ZhuanShengMapInfo>();

		
		public Dictionary<int, List<ShiLianReward>> ShiLianRewardDict = new Dictionary<int, List<ShiLianReward>>();

		
		public List<int> BroadGoodsIDList = new List<int>();

		
		public Activity ThemeZSActivity = new Activity();
	}
}
