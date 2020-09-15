using System;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleArena
{
	// Token: 0x02000364 RID: 868
	[ProtoContract]
	public class CoupleArenaZhanBaoSaveDbData
	{
		// Token: 0x040016FC RID: 5884
		[ProtoMember(1)]
		public CoupleArenaZhanBaoItemData ZhanBao;

		// Token: 0x040016FD RID: 5885
		[ProtoMember(2)]
		public int FirstWeekday;

		// Token: 0x040016FE RID: 5886
		[ProtoMember(3)]
		public int FromMan;

		// Token: 0x040016FF RID: 5887
		[ProtoMember(4)]
		public int FromWife;

		// Token: 0x04001700 RID: 5888
		[ProtoMember(5)]
		public int ToMan;

		// Token: 0x04001701 RID: 5889
		[ProtoMember(6)]
		public int ToWife;
	}
}
