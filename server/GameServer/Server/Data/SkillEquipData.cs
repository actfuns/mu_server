using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000417 RID: 1047
	[ProtoContract]
	public class SkillEquipData
	{
		// Token: 0x04001BE9 RID: 7145
		[ProtoMember(1)]
		public int SkillEquip;

		// Token: 0x04001BEA RID: 7146
		[ProtoMember(2)]
		public List<int> ShenShiActiveList;
	}
}
