using System;
using GameDBServer.DB;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000029 RID: 41
	[ProtoContract]
	public class BaiTanLogItemData
	{
		// Token: 0x0400009A RID: 154
		[DBMapping(ColumnName = "Id")]
		public int Id = 0;

		// Token: 0x0400009B RID: 155
		[DBMapping(ColumnName = "rid")]
		[ProtoMember(1)]
		public int rid = 0;

		// Token: 0x0400009C RID: 156
		[ProtoMember(2)]
		[DBMapping(ColumnName = "otherroleid")]
		public int OtherRoleID = 0;

		// Token: 0x0400009D RID: 157
		[ProtoMember(3)]
		[DBMapping(ColumnName = "otherrname")]
		public string OtherRName = "";

		// Token: 0x0400009E RID: 158
		[DBMapping(ColumnName = "goodsid")]
		[ProtoMember(4)]
		public int GoodsID = 0;

		// Token: 0x0400009F RID: 159
		[DBMapping(ColumnName = "goodsnum")]
		[ProtoMember(5)]
		public int GoodsNum = 0;

		// Token: 0x040000A0 RID: 160
		[ProtoMember(6)]
		[DBMapping(ColumnName = "forgelevel")]
		public int ForgeLevel = 0;

		// Token: 0x040000A1 RID: 161
		[DBMapping(ColumnName = "totalprice")]
		[ProtoMember(7)]
		public int TotalPrice = 0;

		// Token: 0x040000A2 RID: 162
		[ProtoMember(8)]
		[DBMapping(ColumnName = "leftyuanbao")]
		public int LeftYuanBao = 0;

		// Token: 0x040000A3 RID: 163
		[DBMapping(ColumnName = "buytime")]
		[ProtoMember(9)]
		public string BuyTime = "";

		// Token: 0x040000A4 RID: 164
		[ProtoMember(10)]
		[DBMapping(ColumnName = "yinliang")]
		public int YinLiang = 0;

		// Token: 0x040000A5 RID: 165
		[DBMapping(ColumnName = "left_yinliang")]
		[ProtoMember(11)]
		public int LeftYinLiang = 0;

		// Token: 0x040000A6 RID: 166
		[ProtoMember(12)]
		[DBMapping(ColumnName = "tax")]
		public int Tax = 0;

		// Token: 0x040000A7 RID: 167
		[DBMapping(ColumnName = "excellenceinfo")]
		[ProtoMember(13)]
		public int Excellenceinfo = 0;

		// Token: 0x040000A8 RID: 168
		[ProtoMember(14)]
		[DBMapping(ColumnName = "washprops")]
		public string Washprops = "";
	}
}
