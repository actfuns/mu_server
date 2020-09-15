using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000089 RID: 137
	[ProtoContract]
	internal class NewZoneUpLevelItemData
	{
		// Token: 0x040002E2 RID: 738
		[ProtoMember(1)]
		public int LeftNum;

		// Token: 0x040002E3 RID: 739
		[ProtoMember(2)]
		public bool GetAward;
	}
}
