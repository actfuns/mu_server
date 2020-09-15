using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x02000502 RID: 1282
	[ProtoContract]
	public class HuanYingSiYuanLianSha
	{
		// Token: 0x040021E4 RID: 8676
		[ProtoMember(1)]
		public int ZoneID;

		// Token: 0x040021E5 RID: 8677
		[ProtoMember(2)]
		public string Name = "";

		// Token: 0x040021E6 RID: 8678
		[ProtoMember(3)]
		public int LianShaType;

		// Token: 0x040021E7 RID: 8679
		[ProtoMember(4)]
		public int Occupation;

		// Token: 0x040021E8 RID: 8680
		[ProtoMember(5)]
		public int ExtScore;

		// Token: 0x040021E9 RID: 8681
		[ProtoMember(6)]
		public int Side;
	}
}
