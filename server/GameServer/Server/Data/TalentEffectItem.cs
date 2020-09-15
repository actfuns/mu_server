using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200018D RID: 397
	[ProtoContract]
	public class TalentEffectItem
	{
		// Token: 0x040008C9 RID: 2249
		[ProtoMember(1, IsRequired = true)]
		public int ID = 0;

		// Token: 0x040008CA RID: 2250
		[ProtoMember(2, IsRequired = true)]
		public int Level = 0;

		// Token: 0x040008CB RID: 2251
		[ProtoMember(3, IsRequired = true)]
		public int TalentType = 1;

		// Token: 0x040008CC RID: 2252
		[ProtoMember(4, IsRequired = true)]
		public List<TalentEffectInfo> ItemEffectList = null;
	}
}
