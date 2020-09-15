using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000557 RID: 1367
	[ProtoContract]
	public class GoodsData
	{
		// Token: 0x060019D3 RID: 6611 RVA: 0x00191284 File Offset: 0x0018F484
		public GoodsData(GoodsData item)
		{
			if (null != item)
			{
				this.Id = -1;
				this.GoodsID = item.GoodsID;
				this.Using = 0;
				this.Forge_level = item.Forge_level;
				this.Starttime = "1900-01-01 12:00:00";
				this.Endtime = "1900-01-01 12:00:00";
				this.Site = 0;
				this.Quality = item.Quality;
				this.Props = item.Props;
				this.GCount = item.GCount;
				this.Binding = item.Binding;
				this.Jewellist = item.Jewellist;
				this.BagIndex = 0;
				this.AddPropIndex = item.AddPropIndex;
				this.BornIndex = item.BornIndex;
				this.Lucky = item.Lucky;
				this.Strong = item.Strong;
				this.ExcellenceInfo = item.ExcellenceInfo;
				this.AppendPropLev = item.AppendPropLev;
				this.ChangeLifeLevForEquip = item.ChangeLifeLevForEquip;
				this.WashProps = item.WashProps;
				this.ElementhrtsProps = item.ElementhrtsProps;
				this.SaleMoney1 = item.SaleMoney1;
				this.SaleYuanBao = item.SaleYuanBao;
				this.SaleYinPiao = item.SaleYinPiao;
				this.JuHunID = item.JuHunID;
			}
		}

		// Token: 0x060019D4 RID: 6612 RVA: 0x001913C9 File Offset: 0x0018F5C9
		public GoodsData()
		{
		}

		// Token: 0x040024BE RID: 9406
		[ProtoMember(1)]
		public int Id;

		// Token: 0x040024BF RID: 9407
		[ProtoMember(2)]
		public int GoodsID;

		// Token: 0x040024C0 RID: 9408
		[ProtoMember(3)]
		public int Using;

		// Token: 0x040024C1 RID: 9409
		[ProtoMember(4)]
		public int Forge_level;

		// Token: 0x040024C2 RID: 9410
		[ProtoMember(5)]
		public string Starttime;

		// Token: 0x040024C3 RID: 9411
		[ProtoMember(6)]
		public string Endtime;

		// Token: 0x040024C4 RID: 9412
		[ProtoMember(7)]
		public int Site;

		// Token: 0x040024C5 RID: 9413
		[ProtoMember(8)]
		public int Quality;

		// Token: 0x040024C6 RID: 9414
		[ProtoMember(9)]
		public string Props;

		// Token: 0x040024C7 RID: 9415
		[ProtoMember(10)]
		public int GCount;

		// Token: 0x040024C8 RID: 9416
		[ProtoMember(11)]
		public int Binding;

		// Token: 0x040024C9 RID: 9417
		[ProtoMember(12)]
		public string Jewellist;

		// Token: 0x040024CA RID: 9418
		[ProtoMember(13)]
		public int BagIndex;

		// Token: 0x040024CB RID: 9419
		[ProtoMember(14)]
		public int SaleMoney1;

		// Token: 0x040024CC RID: 9420
		[ProtoMember(15)]
		public int SaleYuanBao;

		// Token: 0x040024CD RID: 9421
		[ProtoMember(16)]
		public int SaleYinPiao;

		// Token: 0x040024CE RID: 9422
		[ProtoMember(17)]
		public int AddPropIndex;

		// Token: 0x040024CF RID: 9423
		[ProtoMember(18)]
		public int BornIndex;

		// Token: 0x040024D0 RID: 9424
		[ProtoMember(19)]
		public int Lucky;

		// Token: 0x040024D1 RID: 9425
		[ProtoMember(20)]
		public int Strong;

		// Token: 0x040024D2 RID: 9426
		[ProtoMember(21)]
		public int ExcellenceInfo;

		// Token: 0x040024D3 RID: 9427
		[ProtoMember(22)]
		public int AppendPropLev;

		// Token: 0x040024D4 RID: 9428
		[ProtoMember(23)]
		public int ChangeLifeLevForEquip;

		// Token: 0x040024D5 RID: 9429
		[ProtoMember(24)]
		public List<int> WashProps;

		// Token: 0x040024D6 RID: 9430
		[ProtoMember(25)]
		public List<int> ElementhrtsProps;

		// Token: 0x040024D7 RID: 9431
		[ProtoMember(26)]
		public int JuHunID;
	}
}
