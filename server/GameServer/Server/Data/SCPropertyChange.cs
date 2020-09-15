using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000019 RID: 25
	[ProtoContract]
	public class SCPropertyChange
	{
		// Token: 0x04000096 RID: 150
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x04000097 RID: 151
		[ProtoMember(2)]
		public int MoneyType;

		// Token: 0x04000098 RID: 152
		[ProtoMember(3)]
		public long Value;
	}
}
