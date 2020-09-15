using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000040 RID: 64
	[ProtoContract]
	public class SingleChargeData
	{
		// Token: 0x0400014B RID: 331
		[ProtoMember(1)]
		public Dictionary<int, int> singleData = new Dictionary<int, int>();

		// Token: 0x0400014C RID: 332
		[ProtoMember(2)]
		public int YueKaMoney = 0;

		// Token: 0x0400014D RID: 333
		[ProtoMember(3)]
		public int ChargePlatType = 0;

		// Token: 0x0400014E RID: 334
		[ProtoMember(4)]
		public string SuperInputFanLiKey = "";

		// Token: 0x0400014F RID: 335
		[ProtoMember(5)]
		public Dictionary<int, JieriSuperInputData> SuperInputFanLiDict = new Dictionary<int, JieriSuperInputData>();

		// Token: 0x04000150 RID: 336
		[ProtoMember(6)]
		public Dictionary<int, int> MoneyVsChargeIDDict = new Dictionary<int, int>();

		// Token: 0x04000151 RID: 337
		[ProtoMember(7)]
		public int YueKaBangZuan;
	}
}
