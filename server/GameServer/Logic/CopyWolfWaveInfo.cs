using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class CopyWolfWaveInfo
	{
		
		public int WaveID = 0;

		
		public int NextTime = 0;

		
		public List<int[]> MonsterList = new List<int[]>();

		
		public List<CopyWolfSiteInfo> MonsterSiteDic = new List<CopyWolfSiteInfo>();
	}
}
