using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Data.Tarot
{
	// Token: 0x020000B6 RID: 182
	[ProtoContract]
	public class TarotSystemData
	{
		// Token: 0x06000198 RID: 408 RVA: 0x00008AE8 File Offset: 0x00006CE8
		public TarotSystemData()
		{
			this.KingData = new TarotKingData();
			this.TarotCardDatas = new List<TarotCardData>();
		}

		// Token: 0x040004D2 RID: 1234
		[ProtoMember(1)]
		public TarotKingData KingData;

		// Token: 0x040004D3 RID: 1235
		[ProtoMember(2)]
		public List<TarotCardData> TarotCardDatas;
	}
}
