using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200006A RID: 106
	[ProtoContract]
	public class RoleArmorData
	{
		// Token: 0x04000249 RID: 585
		[ProtoMember(1)]
		public int Armor;

		// Token: 0x0400024A RID: 586
		[ProtoMember(2)]
		public int Exp;
	}
}
