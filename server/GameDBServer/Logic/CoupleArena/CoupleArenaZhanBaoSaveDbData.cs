using System;
using ProtoBuf;

namespace GameDBServer.Logic.CoupleArena
{
	// Token: 0x0200012A RID: 298
	[ProtoContract]
	public class CoupleArenaZhanBaoSaveDbData
	{
		// Token: 0x040007BE RID: 1982
		[ProtoMember(1)]
		public CoupleArenaZhanBaoItemData ZhanBao;

		// Token: 0x040007BF RID: 1983
		[ProtoMember(2)]
		public int FirstWeekday;

		// Token: 0x040007C0 RID: 1984
		[ProtoMember(3)]
		public int FromMan;

		// Token: 0x040007C1 RID: 1985
		[ProtoMember(4)]
		public int FromWife;

		// Token: 0x040007C2 RID: 1986
		[ProtoMember(5)]
		public int ToMan;

		// Token: 0x040007C3 RID: 1987
		[ProtoMember(6)]
		public int ToWife;
	}
}
