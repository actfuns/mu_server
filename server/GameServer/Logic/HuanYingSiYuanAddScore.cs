using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x02000501 RID: 1281
	[ProtoContract]
	public class HuanYingSiYuanAddScore
	{
		// Token: 0x040021DD RID: 8669
		[ProtoMember(1)]
		public int Score;

		// Token: 0x040021DE RID: 8670
		[ProtoMember(2)]
		public int ZoneID;

		// Token: 0x040021DF RID: 8671
		[ProtoMember(3)]
		public string Name = "";

		// Token: 0x040021E0 RID: 8672
		[ProtoMember(4)]
		public int Side;

		// Token: 0x040021E1 RID: 8673
		[ProtoMember(5)]
		public int RoleId;

		// Token: 0x040021E2 RID: 8674
		[ProtoMember(6)]
		public int ByLianShaNum;

		// Token: 0x040021E3 RID: 8675
		[ProtoMember(7)]
		public int Occupation;
	}
}
