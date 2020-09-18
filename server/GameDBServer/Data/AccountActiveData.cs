using System;
using GameDBServer.DB;

namespace GameDBServer.Data
{
	
	internal class AccountActiveData
	{
		
		[DBMapping(ColumnName = "Account")]
		public string strAccount;

		
		[DBMapping(ColumnName = "createTime")]
		public string strCreateTime;

		
		[DBMapping(ColumnName = "seriesLoginCount")]
		public int nSeriesLoginCount;

		
		[DBMapping(ColumnName = "lastSeriesLoginTime")]
		public string strLastSeriesLoginTime;
	}
}
