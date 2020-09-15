using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020002EF RID: 751
	public class ZhanMengHongBaoData
	{
		// Token: 0x04001346 RID: 4934
		public long[] LastUpdateTicks = new long[3];

		// Token: 0x04001347 RID: 4935
		public List<HongBaoSendData> HongBaoList = new List<HongBaoSendData>();

		// Token: 0x04001348 RID: 4936
		public List<HongBaoRankItemData> RecvRankList = new List<HongBaoRankItemData>();

		// Token: 0x04001349 RID: 4937
		public List<HongBaoRankItemData> SendRankList = new List<HongBaoRankItemData>();
	}
}
