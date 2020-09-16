using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SearchTeamData
	{
		
		[ProtoMember(1)]
		public int StartIndex = 0;

		
		[ProtoMember(2)]
		public int TotalTeamsCount = 0;

		
		[ProtoMember(3)]
		public int PageTeamsCount = 0;

		
		[ProtoMember(4)]
		public List<TeamData> TeamDataList = null;
	}
}
