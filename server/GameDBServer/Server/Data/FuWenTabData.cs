using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000175 RID: 373
	[ProtoContract]
	public class FuWenTabData
	{
		// Token: 0x040008A2 RID: 2210
		[ProtoMember(1)]
		public int TabID;

		// Token: 0x040008A3 RID: 2211
		[ProtoMember(2)]
		public string Name;

		// Token: 0x040008A4 RID: 2212
		[ProtoMember(3)]
		public List<int> FuWenEquipList;

		// Token: 0x040008A5 RID: 2213
		[ProtoMember(4)]
		public List<int> ShenShiActiveList;

		// Token: 0x040008A6 RID: 2214
		[ProtoMember(5)]
		public int SkillEquip;

		// Token: 0x040008A7 RID: 2215
		[ProtoMember(6)]
		public int OwnerID;
	}
}
