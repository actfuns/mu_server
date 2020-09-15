using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200003D RID: 61
	[ProtoContract]
	public class ChangeNameResult
	{
		// Token: 0x0400013C RID: 316
		[ProtoMember(1)]
		public int ErrCode;

		// Token: 0x0400013D RID: 317
		[ProtoMember(2)]
		public int ZoneId;

		// Token: 0x0400013E RID: 318
		[ProtoMember(3)]
		public string NewName;

		// Token: 0x0400013F RID: 319
		[ProtoMember(4)]
		public ChangeNameInfo NameInfo = new ChangeNameInfo();
	}
}
