using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class SingleLuoLanFaZhenFubenData
	{
		
		public int FubenID = 0;

		
		public int FubenSeqID = 0;

		
		public int FinalBossLeftNum = 1;

		
		public int SpecailBossLeftNum = 1;

		
		public Dictionary<int, FazhenMapData> MapDatas = new Dictionary<int, FazhenMapData>();
	}
}
