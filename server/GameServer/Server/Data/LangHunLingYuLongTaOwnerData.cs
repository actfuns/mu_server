using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003C2 RID: 962
	[ProtoContract]
	public class LangHunLingYuLongTaOwnerData
	{
		// Token: 0x04001922 RID: 6434
		[ProtoMember(1)]
		public string OwnerBHName = "";

		// Token: 0x04001923 RID: 6435
		[ProtoMember(2)]
		public int OwnerBHid = 0;

		// Token: 0x04001924 RID: 6436
		[ProtoMember(3)]
		public int OwnerBHZoneId = 0;

		// Token: 0x04001925 RID: 6437
		[ProtoMember(4)]
		public int OwnerBHServerId = 0;
	}
}
