using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000087 RID: 135
	[ProtoContract]
	public class NewZoneActiveData
	{
		// Token: 0x040002DD RID: 733
		[ProtoMember(1)]
		public int YuanBao;

		// Token: 0x040002DE RID: 734
		[ProtoMember(2)]
		public int ActiveId;

		// Token: 0x040002DF RID: 735
		[ProtoMember(3)]
		public int GetState;

		// Token: 0x040002E0 RID: 736
		[ProtoMember(4)]
		public List<PaiHangItemData> Ranklist;
	}
}
