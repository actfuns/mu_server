using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000083 RID: 131
	[ProtoContract]
	public class RoleAttributeValueData
	{
		// Token: 0x040002C6 RID: 710
		[ProtoMember(1)]
		public int RoleAttribyteType;

		// Token: 0x040002C7 RID: 711
		[ProtoMember(2)]
		public int AddVAlue;

		// Token: 0x040002C8 RID: 712
		[ProtoMember(3)]
		public int Targetvalue;
	}
}
