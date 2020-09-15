using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000124 RID: 292
	[ProtoContract]
	public class ChangeEquipData
	{
		// Token: 0x04000658 RID: 1624
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000659 RID: 1625
		[ProtoMember(2)]
		public GoodsData EquipGoodsData = null;

		// Token: 0x0400065A RID: 1626
		[ProtoMember(3)]
		public WingData UsingWinData = null;
	}
}
