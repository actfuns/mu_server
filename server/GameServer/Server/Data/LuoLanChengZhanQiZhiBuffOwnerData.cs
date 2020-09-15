using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200015B RID: 347
	[ProtoContract]
	public class LuoLanChengZhanQiZhiBuffOwnerData
	{
		// Token: 0x040007AB RID: 1963
		[ProtoMember(1)]
		public int NPCID;

		// Token: 0x040007AC RID: 1964
		[ProtoMember(2)]
		public int OwnerBHID;

		// Token: 0x040007AD RID: 1965
		[ProtoMember(3)]
		public string OwnerBHName;
	}
}
