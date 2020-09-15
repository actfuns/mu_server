using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000166 RID: 358
	[ProtoContract]
	public class RoleAttributeValueData
	{
		// Token: 0x040007DE RID: 2014
		[ProtoMember(1)]
		public int RoleAttribyteType;

		// Token: 0x040007DF RID: 2015
		[ProtoMember(2)]
		public int AddVAlue;

		// Token: 0x040007E0 RID: 2016
		[ProtoMember(3)]
		public int Targetvalue;
	}
}
