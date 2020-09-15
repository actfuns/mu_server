using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200018C RID: 396
	[ProtoContract]
	public class TalentData
	{
		// Token: 0x040008C0 RID: 2240
		[ProtoMember(1, IsRequired = true)]
		public bool IsOpen = false;

		// Token: 0x040008C1 RID: 2241
		[ProtoMember(2, IsRequired = true)]
		public int TotalCount = 0;

		// Token: 0x040008C2 RID: 2242
		[ProtoMember(3, IsRequired = true)]
		public long Exp = 0L;

		// Token: 0x040008C3 RID: 2243
		[ProtoMember(4, IsRequired = true)]
		public Dictionary<int, int> CountList = new Dictionary<int, int>();

		// Token: 0x040008C4 RID: 2244
		[ProtoMember(5, IsRequired = true)]
		public List<TalentEffectItem> EffectList = new List<TalentEffectItem>();

		// Token: 0x040008C5 RID: 2245
		[ProtoMember(6, IsRequired = true)]
		public Dictionary<int, int> SkillOneValue = new Dictionary<int, int>();

		// Token: 0x040008C6 RID: 2246
		[ProtoMember(7, IsRequired = true)]
		public int SkillAllValue = 0;

		// Token: 0x040008C7 RID: 2247
		[ProtoMember(8, IsRequired = true)]
		public int State = 0;

		// Token: 0x040008C8 RID: 2248
		[ProtoMember(9, IsRequired = true)]
		public int Occupation = 0;
	}
}
