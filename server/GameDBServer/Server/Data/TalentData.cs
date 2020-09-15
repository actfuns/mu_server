using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000B3 RID: 179
	[ProtoContract]
	public class TalentData
	{
		// Token: 0x040004C3 RID: 1219
		[ProtoMember(1, IsRequired = true)]
		public bool IsOpen = false;

		// Token: 0x040004C4 RID: 1220
		[ProtoMember(2, IsRequired = true)]
		public int TotalCount = 0;

		// Token: 0x040004C5 RID: 1221
		[ProtoMember(3, IsRequired = true)]
		public long Exp = 0L;

		// Token: 0x040004C6 RID: 1222
		[ProtoMember(4, IsRequired = true)]
		public Dictionary<int, int> CountList = new Dictionary<int, int>();

		// Token: 0x040004C7 RID: 1223
		[ProtoMember(5, IsRequired = true)]
		public List<TalentEffectItem> EffectList = new List<TalentEffectItem>();

		// Token: 0x040004C8 RID: 1224
		[ProtoMember(6, IsRequired = true)]
		public Dictionary<int, int> SkillOneValue = new Dictionary<int, int>();

		// Token: 0x040004C9 RID: 1225
		[ProtoMember(7, IsRequired = true)]
		public int SkillAllValue = 0;

		// Token: 0x040004CA RID: 1226
		[ProtoMember(8, IsRequired = true)]
		public int State = 0;
	}
}
