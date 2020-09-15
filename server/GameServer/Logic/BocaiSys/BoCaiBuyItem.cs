using System;
using ProtoBuf;

namespace GameServer.Logic.BocaiSys
{
	// Token: 0x0200006F RID: 111
	[ProtoContract]
	public class BoCaiBuyItem
	{
		// Token: 0x0400028C RID: 652
		[ProtoMember(1)]
		public int BuyNum;

		// Token: 0x0400028D RID: 653
		[ProtoMember(2)]
		public string strBuyValue;

		// Token: 0x0400028E RID: 654
		[ProtoMember(3)]
		public long DataPeriods;
	}
}
