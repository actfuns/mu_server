using System;
using System.Collections.Generic;
using GameServer.Logic;

namespace Server.Data
{
	
	public class MoYuRunData
	{
		
		public object Mutex = new object();

		
		public Dictionary<int, MoYuMonsterInfo> MonsterXmlDict = new Dictionary<int, MoYuMonsterInfo>();

		
		public Dictionary<int, BossAttackLog> BossAttackLogDict = new Dictionary<int, BossAttackLog>();

		
		public List<int> BroadGoodsIDList = new List<int>();

		
		public List<int> MapCodeList = new List<int>();

		
		public Activity ThemeMoYuActivity = new Activity();

		
		public DateTime LastBirthTimePoint = DateTime.MinValue;
	}
}
