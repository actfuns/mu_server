using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000115 RID: 277
	[ProtoContract]
	public class AddGoodsData
	{
		// Token: 0x040005D8 RID: 1496
		[ProtoMember(1)]
		public int roleID = 0;

		// Token: 0x040005D9 RID: 1497
		[ProtoMember(2)]
		public int id = 0;

		// Token: 0x040005DA RID: 1498
		[ProtoMember(3)]
		public int goodsID = 0;

		// Token: 0x040005DB RID: 1499
		[ProtoMember(4)]
		public int forgeLevel = 0;

		// Token: 0x040005DC RID: 1500
		[ProtoMember(5)]
		public int quality = 0;

		// Token: 0x040005DD RID: 1501
		[ProtoMember(6)]
		public int goodsNum = 0;

		// Token: 0x040005DE RID: 1502
		[ProtoMember(7)]
		public int binding = 0;

		// Token: 0x040005DF RID: 1503
		[ProtoMember(8)]
		public int site = 0;

		// Token: 0x040005E0 RID: 1504
		[ProtoMember(9)]
		public string jewellist = "";

		// Token: 0x040005E1 RID: 1505
		[ProtoMember(10)]
		public int newHint = 0;

		// Token: 0x040005E2 RID: 1506
		[ProtoMember(11)]
		public string newEndTime = "";

		// Token: 0x040005E3 RID: 1507
		[ProtoMember(12)]
		public int addPropIndex = 0;

		// Token: 0x040005E4 RID: 1508
		[ProtoMember(13)]
		public int bornIndex = 0;

		// Token: 0x040005E5 RID: 1509
		[ProtoMember(14)]
		public int lucky = 0;

		// Token: 0x040005E6 RID: 1510
		[ProtoMember(15)]
		public int strong = 0;

		// Token: 0x040005E7 RID: 1511
		[ProtoMember(16)]
		public int ExcellenceProperty = 0;

		// Token: 0x040005E8 RID: 1512
		[ProtoMember(17)]
		public int nAppendPropLev = 0;

		// Token: 0x040005E9 RID: 1513
		[ProtoMember(18)]
		public int ChangeLifeLevForEquip = 0;

		// Token: 0x040005EA RID: 1514
		[ProtoMember(19)]
		public int bagIndex = 0;

		// Token: 0x040005EB RID: 1515
		[ProtoMember(20)]
		public List<int> washProps = null;

		// Token: 0x040005EC RID: 1516
		[ProtoMember(21)]
		public List<int> ElementhrtsProps;

		// Token: 0x040005ED RID: 1517
		[ProtoMember(22)]
		public int juHunLevel;

		// Token: 0x040005EE RID: 1518
		[ProtoMember(23)]
		public int InsureCount;

		// Token: 0x040005EF RID: 1519
		[ProtoMember(24)]
		public int PackUp;

		// Token: 0x040005F0 RID: 1520
		[ProtoMember(25)]
		public string prop;
	}
}
