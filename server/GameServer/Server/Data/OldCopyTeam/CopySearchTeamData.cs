using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data.OldCopyTeam
{
	
	[ProtoContract]
	public class CopySearchTeamData
	{
		
		public CopySearchTeamData SimpleClone()
		{
			CopySearchTeamData simple = new CopySearchTeamData();
			simple.PageTeamsCount = this.PageTeamsCount;
			simple.StartIndex = this.StartIndex;
			simple.TotalTeamsCount = this.TotalTeamsCount;
			if (null != this.TeamDataList)
			{
				simple.TeamDataList = new List<CopyTeamData>();
				foreach (CopyTeamData item in this.TeamDataList)
				{
					simple.TeamDataList.Add(item.SimpleClone());
				}
			}
			return simple;
		}

		
		[ProtoMember(1)]
		public int StartIndex = 0;

		
		[ProtoMember(2)]
		public int TotalTeamsCount = 0;

		
		[ProtoMember(3)]
		public int PageTeamsCount = 0;

		
		[ProtoMember(4)]
		public List<CopyTeamData> TeamDataList = null;
	}
}
