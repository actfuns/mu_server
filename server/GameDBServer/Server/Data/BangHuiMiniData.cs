using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000D3 RID: 211
	[ProtoContract]
	public class BangHuiMiniData
	{
		// Token: 0x040005C5 RID: 1477
		[ProtoMember(1)]
		public int BHID = 0;

		// Token: 0x040005C6 RID: 1478
		[ProtoMember(2)]
		public string BHName = "";

		// Token: 0x040005C7 RID: 1479
		[ProtoMember(3)]
		public int ZoneID = 0;
	}
}
