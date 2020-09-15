using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200081C RID: 2076
	[ProtoContract]
	public class YongZheZhanChangSelfScore
	{
		// Token: 0x040044BA RID: 17594
		[ProtoMember(1)]
		public int AddScore;

		// Token: 0x040044BB RID: 17595
		[ProtoMember(2)]
		public int ZoneID;

		// Token: 0x040044BC RID: 17596
		[ProtoMember(3)]
		public string Name = "";

		// Token: 0x040044BD RID: 17597
		[ProtoMember(4)]
		public int Side;

		// Token: 0x040044BE RID: 17598
		[ProtoMember(5)]
		public int RoleId;

		// Token: 0x040044BF RID: 17599
		[ProtoMember(6)]
		public int ByLianShaNum;

		// Token: 0x040044C0 RID: 17600
		[ProtoMember(7)]
		public int Occupation;

		// Token: 0x040044C1 RID: 17601
		[ProtoMember(8)]
		public int TotalScore;
	}
}
