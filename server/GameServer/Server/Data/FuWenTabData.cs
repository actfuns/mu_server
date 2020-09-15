using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000415 RID: 1045
	[ProtoContract]
	public class FuWenTabData
	{
		// Token: 0x04001BDF RID: 7135
		[ProtoMember(1)]
		public int TabID;

		// Token: 0x04001BE0 RID: 7136
		[ProtoMember(2)]
		public string Name;

		// Token: 0x04001BE1 RID: 7137
		[ProtoMember(3)]
		public List<int> FuWenEquipList;

		// Token: 0x04001BE2 RID: 7138
		[ProtoMember(4)]
		public List<int> ShenShiActiveList;

		// Token: 0x04001BE3 RID: 7139
		[ProtoMember(5)]
		public int SkillEquip;

		// Token: 0x04001BE4 RID: 7140
		[ProtoMember(6)]
		public int OwnerID;
	}
}
