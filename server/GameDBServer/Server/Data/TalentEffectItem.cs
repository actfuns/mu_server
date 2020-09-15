using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000B4 RID: 180
	[ProtoContract]
	public class TalentEffectItem
	{
		// Token: 0x040004CB RID: 1227
		[ProtoMember(1, IsRequired = true)]
		public int ID = 0;

		// Token: 0x040004CC RID: 1228
		[ProtoMember(2, IsRequired = true)]
		public int Level = 0;

		// Token: 0x040004CD RID: 1229
		[ProtoMember(3, IsRequired = true)]
		public int TalentType = 1;

		// Token: 0x040004CE RID: 1230
		[ProtoMember(4, IsRequired = true)]
		public List<TalentEffectInfo> ItemEffectList = null;
	}
}
