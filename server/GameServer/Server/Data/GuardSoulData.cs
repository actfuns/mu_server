using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000145 RID: 325
	[ProtoContract]
	public class GuardSoulData
	{
		// Token: 0x0400074E RID: 1870
		[ProtoMember(1, IsRequired = true)]
		public int Type = 0;

		// Token: 0x0400074F RID: 1871
		[ProtoMember(2, IsRequired = true)]
		public int EquipSlot = -1;
	}
}
