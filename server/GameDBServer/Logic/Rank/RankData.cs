using System;
using System.Collections.Generic;
using Server.Data;

namespace GameDBServer.Logic.Rank
{
	
	public class RankData
	{
		
		public double QueryFromDBTime;

		
		public double MaxRankCount;

		
		public List<int> minGateValueList = null;

		
		public List<InputKingPaiHangData> RankDataList = null;
	}
}
