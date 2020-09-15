using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003BE RID: 958
	[ProtoContract]
	public class LangHunLingYuKingShowDataHist
	{
		// Token: 0x04001914 RID: 6420
		[ProtoMember(1)]
		public int AdmireCount;

		// Token: 0x04001915 RID: 6421
		[ProtoMember(2)]
		public DateTime CompleteTime;

		// Token: 0x04001916 RID: 6422
		[ProtoMember(3)]
		public string BHName;

		// Token: 0x04001917 RID: 6423
		[ProtoMember(4)]
		public RoleData4Selector RoleData4Selector;
	}
}
