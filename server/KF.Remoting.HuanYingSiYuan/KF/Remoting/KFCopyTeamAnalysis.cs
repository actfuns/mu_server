using System;
using System.Collections.Generic;

namespace KF.Remoting
{
	
	public class KFCopyTeamAnalysis
	{
		
		public Dictionary<int, KFCopyTeamAnalysis.Item> AnalysisDict = new Dictionary<int, KFCopyTeamAnalysis.Item>();

		
		public class Item
		{
			
			public int TotalCopyCount;

			
			public int StartCopyCount;

			
			public int UnStartCopyCount;

			
			public int TotalRoleCount;

			
			public int StartRoleCount;

			
			public int UnStartRoleCount;
		}
	}
}
