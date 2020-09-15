using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000022 RID: 34
	[ProtoContract]
	public class RoleArmorData
	{
		// Token: 0x040000CE RID: 206
		[ProtoMember(1)]
		public int Armor;

		// Token: 0x040000CF RID: 207
		[ProtoMember(2)]
		public int Exp;
	}
}
