using System;
using System.Collections.Generic;

namespace KF.Contract.Data
{
	
	[Serializable]
	public class TianTi5v5RankData
	{
		
		public DateTime ModifyTime;

		
		public int MaxPaiMingRank;

		
		public List<TianTi5v5ZhanDuiData> TianTi5v5RoleInfoDataList;

		
		public List<TianTi5v5ZhanDuiData> TianTi5v5MonthRoleInfoDataList;
	}
}
