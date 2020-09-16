using System;
using GameDBServer.DB;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BaiTanLogItemData
	{
		
		[DBMapping(ColumnName = "Id")]
		public int Id = 0;

		
		[DBMapping(ColumnName = "rid")]
		[ProtoMember(1)]
		public int rid = 0;

		
		[ProtoMember(2)]
		[DBMapping(ColumnName = "otherroleid")]
		public int OtherRoleID = 0;

		
		[ProtoMember(3)]
		[DBMapping(ColumnName = "otherrname")]
		public string OtherRName = "";

		
		[DBMapping(ColumnName = "goodsid")]
		[ProtoMember(4)]
		public int GoodsID = 0;

		
		[DBMapping(ColumnName = "goodsnum")]
		[ProtoMember(5)]
		public int GoodsNum = 0;

		
		[ProtoMember(6)]
		[DBMapping(ColumnName = "forgelevel")]
		public int ForgeLevel = 0;

		
		[DBMapping(ColumnName = "totalprice")]
		[ProtoMember(7)]
		public int TotalPrice = 0;

		
		[ProtoMember(8)]
		[DBMapping(ColumnName = "leftyuanbao")]
		public int LeftYuanBao = 0;

		
		[DBMapping(ColumnName = "buytime")]
		[ProtoMember(9)]
		public string BuyTime = "";

		
		[ProtoMember(10)]
		[DBMapping(ColumnName = "yinliang")]
		public int YinLiang = 0;

		
		[DBMapping(ColumnName = "left_yinliang")]
		[ProtoMember(11)]
		public int LeftYinLiang = 0;

		
		[ProtoMember(12)]
		[DBMapping(ColumnName = "tax")]
		public int Tax = 0;

		
		[DBMapping(ColumnName = "excellenceinfo")]
		[ProtoMember(13)]
		public int Excellenceinfo = 0;

		
		[ProtoMember(14)]
		[DBMapping(ColumnName = "washprops")]
		public string Washprops = "";
	}
}
