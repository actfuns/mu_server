using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200015C RID: 348
	[ProtoContract]
	public class LuoLanChengZhanLongTaOwnerData
	{
		// Token: 0x040007AE RID: 1966
		[ProtoMember(1)]
		public string OwnerBHName = "";

		// Token: 0x040007AF RID: 1967
		[ProtoMember(2)]
		public int OwnerBHid = -1;
	}
}
