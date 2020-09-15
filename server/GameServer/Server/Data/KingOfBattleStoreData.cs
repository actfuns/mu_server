using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200033B RID: 827
	[ProtoContract]
	public class KingOfBattleStoreData
	{
		// Token: 0x040015A8 RID: 5544
		[ProtoMember(1)]
		public DateTime LastRefTime;

		// Token: 0x040015A9 RID: 5545
		[ProtoMember(2)]
		public List<KingOfBattleStoreSaleData> SaleList;
	}
}
