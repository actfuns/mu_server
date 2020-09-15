using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000127 RID: 295
	[ProtoContract]
	public class ChangeNameResult
	{
		// Token: 0x04000660 RID: 1632
		[ProtoMember(1)]
		public int ErrCode;

		// Token: 0x04000661 RID: 1633
		[ProtoMember(2)]
		public int ZoneId;

		// Token: 0x04000662 RID: 1634
		[ProtoMember(3)]
		public string NewName;

		// Token: 0x04000663 RID: 1635
		[ProtoMember(4)]
		public ChangeNameInfo NameInfo = new ChangeNameInfo();
	}
}
