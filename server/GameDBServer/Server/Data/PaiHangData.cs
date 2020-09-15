using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000093 RID: 147
	[ProtoContract]
	public class PaiHangData
	{
		// Token: 0x04000358 RID: 856
		[ProtoMember(1)]
		public int PaiHangType;

		// Token: 0x04000359 RID: 857
		[ProtoMember(2)]
		public List<PaiHangItemData> PaiHangList = null;
	}
}
