using System;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic.Olympics
{
	// Token: 0x0200038E RID: 910
	[ProtoContract]
	public class OlympicsShopData
	{
		// Token: 0x06000F81 RID: 3969 RVA: 0x000F303C File Offset: 0x000F123C
		public OlympicsShopData Clone(OlympicsShopData data)
		{
			this.ID = data.ID;
			this.DayID = data.DayID;
			this.Goods = data.Goods;
			this.Price = data.Price;
			this.NumSingle = data.NumSingle;
			this.NumFull = data.NumFull;
			this.NumSingleBuy = data.NumSingleBuy;
			this.NumFullBuy = data.NumFullBuy;
			return this;
		}

		// Token: 0x040017FB RID: 6139
		[ProtoMember(1)]
		public int ID = 0;

		// Token: 0x040017FC RID: 6140
		[ProtoMember(2)]
		public int DayID = 0;

		// Token: 0x040017FD RID: 6141
		[ProtoMember(3)]
		public GoodsData Goods = null;

		// Token: 0x040017FE RID: 6142
		[ProtoMember(4)]
		public int Price = 0;

		// Token: 0x040017FF RID: 6143
		[ProtoMember(5)]
		public int NumSingle = 0;

		// Token: 0x04001800 RID: 6144
		[ProtoMember(6)]
		public int NumFull = 0;

		// Token: 0x04001801 RID: 6145
		[ProtoMember(7)]
		public int NumSingleBuy = 0;

		// Token: 0x04001802 RID: 6146
		[ProtoMember(8)]
		public int NumFullBuy = -1;
	}
}
