using System;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleArena
{
	// Token: 0x02000367 RID: 871
	[ProtoContract]
	public class CoupleArenaBuffHoldData
	{
		// Token: 0x04001708 RID: 5896
		[ProtoMember(1, IsRequired = true)]
		public bool IsZhenAiBuffValid;

		// Token: 0x04001709 RID: 5897
		[ProtoMember(2)]
		public int ZhenAiHolderZoneId;

		// Token: 0x0400170A RID: 5898
		[ProtoMember(3)]
		public string ZhenAiHolderRname;

		// Token: 0x0400170B RID: 5899
		[ProtoMember(4, IsRequired = true)]
		public bool IsYongQiBuffValid;

		// Token: 0x0400170C RID: 5900
		[ProtoMember(5)]
		public int YongQiHolderZoneId;

		// Token: 0x0400170D RID: 5901
		[ProtoMember(6)]
		public string YongQiHolderRname;
	}
}
