using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000176 RID: 374
	[ProtoContract]
	public class SkillEquipData
	{
		// Token: 0x040008A8 RID: 2216
		[ProtoMember(1)]
		public int SkillEquip;

		// Token: 0x040008A9 RID: 2217
		[ProtoMember(2)]
		public List<int> ShenShiActiveList;
	}
}
