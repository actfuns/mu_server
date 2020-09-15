using System;
using System.Collections.Generic;

namespace GameServer.Logic.GoldAuction
{
	// Token: 0x020000A3 RID: 163
	public class AuctionAwardConfig
	{
		// Token: 0x040003D0 RID: 976
		public int ID;

		// Token: 0x040003D1 RID: 977
		public string Name;

		// Token: 0x040003D2 RID: 978
		public List<string> strGoodsList = new List<string>();

		// Token: 0x040003D3 RID: 979
		public int StartValues;

		// Token: 0x040003D4 RID: 980
		public int EndValues;
	}
}
