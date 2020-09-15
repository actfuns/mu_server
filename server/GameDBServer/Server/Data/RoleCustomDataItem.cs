using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200016A RID: 362
	[ProtoContract]
	public class RoleCustomDataItem
	{
		// Token: 0x04000887 RID: 2183
		[ProtoMember(1, IsRequired = true)]
		public int Occupation;

		// Token: 0x04000888 RID: 2184
		[ProtoMember(2, IsRequired = true)]
		public string Main_quick_keys;

		// Token: 0x04000889 RID: 2185
		[ProtoMember(3, IsRequired = true)]
		public List<int> RolePointList = new List<int>();

		// Token: 0x0400088A RID: 2186
		[ProtoMember(4, IsRequired = true)]
		public List<TalentEffectItem> EffectList = new List<TalentEffectItem>();

		// Token: 0x0400088B RID: 2187
		[ProtoMember(5, IsRequired = true)]
		public int DefaultSkillLev;

		// Token: 0x0400088C RID: 2188
		[ProtoMember(6, IsRequired = true)]
		public int DefaultSkillUseNum;

		// Token: 0x0400088D RID: 2189
		[ProtoMember(7, IsRequired = false)]
		public List<SkillEquipData> ShenShiEuipSkill;
	}
}
