using System;
using System.Collections.Generic;
using GameDBServer.DB;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200005A RID: 90
	[ProtoContract]
	public class GoodsData
	{
		// Token: 0x040001E6 RID: 486
		[ProtoMember(1)]
		[DBMapping(ColumnName = "Id")]
		public int Id;

		// Token: 0x040001E7 RID: 487
		[ProtoMember(2)]
		[DBMapping(ColumnName = "goodsid")]
		public int GoodsID;

		// Token: 0x040001E8 RID: 488
		[ProtoMember(3)]
		[DBMapping(ColumnName = "isusing")]
		public int Using;

		// Token: 0x040001E9 RID: 489
		[DBMapping(ColumnName = "forge_level")]
		[ProtoMember(4)]
		public int Forge_level;

		// Token: 0x040001EA RID: 490
		[ProtoMember(5)]
		[DBMapping(ColumnName = "starttime")]
		public string Starttime;

		// Token: 0x040001EB RID: 491
		[ProtoMember(6)]
		[DBMapping(ColumnName = "endtime")]
		public string Endtime;

		// Token: 0x040001EC RID: 492
		[DBMapping(ColumnName = "site")]
		[ProtoMember(7)]
		public int Site;

		// Token: 0x040001ED RID: 493
		[DBMapping(ColumnName = "quality")]
		[ProtoMember(8)]
		public int Quality;

		// Token: 0x040001EE RID: 494
		[DBMapping(ColumnName = "Props")]
		[ProtoMember(9)]
		public string Props;

		// Token: 0x040001EF RID: 495
		[DBMapping(ColumnName = "gcount")]
		[ProtoMember(10)]
		public int GCount;

		// Token: 0x040001F0 RID: 496
		[ProtoMember(11)]
		[DBMapping(ColumnName = "binding")]
		public int Binding;

		// Token: 0x040001F1 RID: 497
		[DBMapping(ColumnName = "jewellist")]
		[ProtoMember(12)]
		public string Jewellist;

		// Token: 0x040001F2 RID: 498
		[ProtoMember(13)]
		[DBMapping(ColumnName = "bagindex")]
		public int BagIndex;

		// Token: 0x040001F3 RID: 499
		[DBMapping(ColumnName = "salemoney1")]
		[ProtoMember(14)]
		public int SaleMoney1;

		// Token: 0x040001F4 RID: 500
		[ProtoMember(15)]
		[DBMapping(ColumnName = "saleyuanbao")]
		public int SaleYuanBao;

		// Token: 0x040001F5 RID: 501
		[DBMapping(ColumnName = "saleyinpiao")]
		[ProtoMember(16)]
		public int SaleYinPiao;

		// Token: 0x040001F6 RID: 502
		[DBMapping(ColumnName = "addpropindex")]
		[ProtoMember(17)]
		public int AddPropIndex;

		// Token: 0x040001F7 RID: 503
		[ProtoMember(18)]
		[DBMapping(ColumnName = "bornindex")]
		public int BornIndex;

		// Token: 0x040001F8 RID: 504
		[ProtoMember(19)]
		[DBMapping(ColumnName = "lucky")]
		public int Lucky;

		// Token: 0x040001F9 RID: 505
		[ProtoMember(20)]
		[DBMapping(ColumnName = "strong")]
		public int Strong;

		// Token: 0x040001FA RID: 506
		[DBMapping(ColumnName = "excellenceinfo")]
		[ProtoMember(21)]
		public int ExcellenceInfo;

		// Token: 0x040001FB RID: 507
		[DBMapping(ColumnName = "appendproplev")]
		[ProtoMember(22)]
		public int AppendPropLev;

		// Token: 0x040001FC RID: 508
		[ProtoMember(23)]
		[DBMapping(ColumnName = "equipchangelife")]
		public int ChangeLifeLevForEquip;

		// Token: 0x040001FD RID: 509
		[ProtoMember(24)]
		public List<int> WashProps;

		// Token: 0x040001FE RID: 510
		[ProtoMember(25)]
		public List<int> ElementhrtsProps;

		// Token: 0x040001FF RID: 511
		[ProtoMember(26)]
		[DBMapping(ColumnName = "juhun")]
		public int JuHunID;
	}
}
