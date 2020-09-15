using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data.OldCopyTeam
{
	// Token: 0x02000545 RID: 1349
	[ProtoContract]
	public class CopySearchTeamData
	{
		// Token: 0x060019BF RID: 6591 RVA: 0x00190D60 File Offset: 0x0018EF60
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

		// Token: 0x04002421 RID: 9249
		[ProtoMember(1)]
		public int StartIndex = 0;

		// Token: 0x04002422 RID: 9250
		[ProtoMember(2)]
		public int TotalTeamsCount = 0;

		// Token: 0x04002423 RID: 9251
		[ProtoMember(3)]
		public int PageTeamsCount = 0;

		// Token: 0x04002424 RID: 9252
		[ProtoMember(4)]
		public List<CopyTeamData> TeamDataList = null;
	}
}
