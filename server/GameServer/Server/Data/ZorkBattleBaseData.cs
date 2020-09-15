using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000846 RID: 2118
	[ProtoContract]
	public class ZorkBattleBaseData
	{
		// Token: 0x04004636 RID: 17974
		[ProtoMember(1)]
		public List<int> listAnalysisData = new List<int>();

		// Token: 0x04004637 RID: 17975
		[ProtoMember(2)]
		public Dictionary<int, int> ArchievementAwardDict = new Dictionary<int, int>();

		// Token: 0x04004638 RID: 17976
		[ProtoMember(3)]
		public int TeamDuanWei;
	}
}
