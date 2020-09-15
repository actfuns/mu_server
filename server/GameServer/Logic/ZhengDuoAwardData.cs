using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000428 RID: 1064
	[ProtoContract]
	public class ZhengDuoAwardData
	{
		// Token: 0x04001C8B RID: 7307
		[ProtoMember(1)]
		public int State;

		// Token: 0x04001C8C RID: 7308
		[ProtoMember(2)]
		public int Second;

		// Token: 0x04001C8D RID: 7309
		[ProtoMember(3)]
		public long Exp;

		// Token: 0x04001C8E RID: 7310
		[ProtoMember(4)]
		public int Money;

		// Token: 0x04001C8F RID: 7311
		[ProtoMember(5)]
		public List<GoodsData> GoodsList;
	}
}
