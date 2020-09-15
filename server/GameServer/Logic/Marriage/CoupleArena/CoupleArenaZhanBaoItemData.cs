using System;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleArena
{
	// Token: 0x02000363 RID: 867
	[ProtoContract]
	public class CoupleArenaZhanBaoItemData
	{
		// Token: 0x040016F6 RID: 5878
		[ProtoMember(1)]
		public int TargetManZoneId;

		// Token: 0x040016F7 RID: 5879
		[ProtoMember(2)]
		public string TargetManRoleName;

		// Token: 0x040016F8 RID: 5880
		[ProtoMember(3)]
		public int TargetWifeZoneId;

		// Token: 0x040016F9 RID: 5881
		[ProtoMember(4)]
		public string TargetWifeRoleName;

		// Token: 0x040016FA RID: 5882
		[ProtoMember(5)]
		public bool IsWin;

		// Token: 0x040016FB RID: 5883
		[ProtoMember(6)]
		public int GetJiFen;
	}
}
