using System;
using ProtoBuf;

namespace GameDBServer.Logic.CoupleArena
{
	// Token: 0x02000129 RID: 297
	[ProtoContract]
	public class CoupleArenaZhanBaoItemData
	{
		// Token: 0x040007B8 RID: 1976
		[ProtoMember(1)]
		public int TargetManZoneId;

		// Token: 0x040007B9 RID: 1977
		[ProtoMember(2)]
		public string TargetManRoleName;

		// Token: 0x040007BA RID: 1978
		[ProtoMember(3)]
		public int TargetWifeZoneId;

		// Token: 0x040007BB RID: 1979
		[ProtoMember(4)]
		public string TargetWifeRoleName;

		// Token: 0x040007BC RID: 1980
		[ProtoMember(5)]
		public bool IsWin;

		// Token: 0x040007BD RID: 1981
		[ProtoMember(6)]
		public int GetJiFen;
	}
}
