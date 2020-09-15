using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200057B RID: 1403
	[ProtoContract]
	public class PlayerJingJiEquipData
	{
		// Token: 0x040025F2 RID: 9714
		[ProtoMember(1)]
		public int EquipId;

		// Token: 0x040025F3 RID: 9715
		[ProtoMember(2)]
		public int Forge_level;

		// Token: 0x040025F4 RID: 9716
		[ProtoMember(3)]
		public int ExcellenceInfo;

		// Token: 0x040025F5 RID: 9717
		[ProtoMember(4)]
		public int BagIndex;
	}
}
