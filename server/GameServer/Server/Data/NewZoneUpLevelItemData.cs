using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000573 RID: 1395
	[ProtoContract]
	internal class NewZoneUpLevelItemData
	{
		// Token: 0x040025A8 RID: 9640
		[ProtoMember(1)]
		public int LeftNum;

		// Token: 0x040025A9 RID: 9641
		[ProtoMember(2)]
		public bool GetAward;
	}
}
