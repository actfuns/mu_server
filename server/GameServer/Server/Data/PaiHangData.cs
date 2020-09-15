using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000170 RID: 368
	[ProtoContract]
	public class PaiHangData
	{
		// Token: 0x04000838 RID: 2104
		[ProtoMember(1)]
		public int PaiHangType;

		// Token: 0x04000839 RID: 2105
		[ProtoMember(2)]
		public List<PaiHangItemData> PaiHangList = null;
	}
}
