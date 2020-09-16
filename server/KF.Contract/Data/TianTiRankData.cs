using System;
using System.Collections.Generic;

namespace KF.Contract.Data
{
	
	[Serializable]
	public class TianTiRankData
	{
		
		public DateTime ModifyTime;

		
		public int MaxPaiMingRank;

		
		public List<TianTiRoleInfoData> TianTiRoleInfoDataList;

		
		public List<TianTiRoleInfoData> TianTiMonthRoleInfoDataList;
	}
}
