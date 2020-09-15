using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200058F RID: 1423
	[ProtoContract]
	public class SearchTeamData
	{
		// Token: 0x04002816 RID: 10262
		[ProtoMember(1)]
		public int StartIndex = 0;

		// Token: 0x04002817 RID: 10263
		[ProtoMember(2)]
		public int TotalTeamsCount = 0;

		// Token: 0x04002818 RID: 10264
		[ProtoMember(3)]
		public int PageTeamsCount = 0;

		// Token: 0x04002819 RID: 10265
		[ProtoMember(4)]
		public List<TeamData> TeamDataList = null;
	}
}
