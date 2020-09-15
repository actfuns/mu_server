using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200012A RID: 298
	[ProtoContract]
	public class SingleChargeData
	{
		// Token: 0x0400066F RID: 1647
		[ProtoMember(1)]
		public Dictionary<int, int> singleData = new Dictionary<int, int>();

		// Token: 0x04000670 RID: 1648
		[ProtoMember(2)]
		public int YueKaMoney = 0;

		// Token: 0x04000671 RID: 1649
		[ProtoMember(3)]
		public int ChargePlatType = 0;

		// Token: 0x04000672 RID: 1650
		[ProtoMember(4)]
		public string SuperInputFanLiKey = "";

		// Token: 0x04000673 RID: 1651
		[ProtoMember(5)]
		public Dictionary<int, JieriSuperInputData> SuperInputFanLiDict = new Dictionary<int, JieriSuperInputData>();

		// Token: 0x04000674 RID: 1652
		[ProtoMember(6)]
		public Dictionary<int, int> MoneyVsChargeIDDict = new Dictionary<int, int>();

		// Token: 0x04000675 RID: 1653
		[ProtoMember(7)]
		public int YueKaBangZuan;

		// Token: 0x04000676 RID: 1654
		public Dictionary<int, int> ChargeIDVsMoneyDict = new Dictionary<int, int>();
	}
}
