using System;
using ProtoBuf;

namespace GameServer.Logic.BocaiSys
{
	// Token: 0x0200007D RID: 125
	[ProtoContract]
	public class SelfBuyInfo
	{
		// Token: 0x040002D8 RID: 728
		[ProtoMember(1)]
		public int ID;

		// Token: 0x040002D9 RID: 729
		[ProtoMember(2)]
		public string WuPinID;

		// Token: 0x040002DA RID: 730
		[ProtoMember(3)]
		public int BuyNum;
	}
}
