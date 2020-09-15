using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.TarotData
{
	// Token: 0x02000197 RID: 407
	[ProtoContract]
	public class TarotSystemData
	{
		// Token: 0x060004DA RID: 1242 RVA: 0x00042849 File Offset: 0x00040A49
		public TarotSystemData()
		{
			this.KingData = new TarotKingData();
			this.TarotCardDatas = new List<TarotCardData>();
		}

		// Token: 0x040008FE RID: 2302
		[ProtoMember(1)]
		public TarotKingData KingData;

		// Token: 0x040008FF RID: 2303
		[ProtoMember(2)]
		public List<TarotCardData> TarotCardDatas;
	}
}
