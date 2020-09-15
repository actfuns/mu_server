using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200005C RID: 92
	[ProtoContract]
	public class GuardSoulData
	{
		// Token: 0x04000203 RID: 515
		[ProtoMember(1, IsRequired = true)]
		public int Type = 0;

		// Token: 0x04000204 RID: 516
		[ProtoMember(2, IsRequired = true)]
		public int EquipSlot = -1;
	}
}
