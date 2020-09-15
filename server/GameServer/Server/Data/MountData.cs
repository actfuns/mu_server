using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200085A RID: 2138
	[ProtoContract]
	public class MountData
	{
		// Token: 0x040046B9 RID: 18105
		[ProtoMember(1)]
		public int GoodsID;

		// Token: 0x040046BA RID: 18106
		[ProtoMember(2)]
		public bool IsNew;

		// Token: 0x040046BB RID: 18107
		public int SkillID;
	}
}
