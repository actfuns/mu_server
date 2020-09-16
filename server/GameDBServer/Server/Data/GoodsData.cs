using System;
using System.Collections.Generic;
using GameDBServer.DB;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class GoodsData
	{
		
		[ProtoMember(1)]
		[DBMapping(ColumnName = "Id")]
		public int Id;

		
		[ProtoMember(2)]
		[DBMapping(ColumnName = "goodsid")]
		public int GoodsID;

		
		[ProtoMember(3)]
		[DBMapping(ColumnName = "isusing")]
		public int Using;

		
		[DBMapping(ColumnName = "forge_level")]
		[ProtoMember(4)]
		public int Forge_level;

		
		[ProtoMember(5)]
		[DBMapping(ColumnName = "starttime")]
		public string Starttime;

		
		[ProtoMember(6)]
		[DBMapping(ColumnName = "endtime")]
		public string Endtime;

		
		[DBMapping(ColumnName = "site")]
		[ProtoMember(7)]
		public int Site;

		
		[DBMapping(ColumnName = "quality")]
		[ProtoMember(8)]
		public int Quality;

		
		[DBMapping(ColumnName = "Props")]
		[ProtoMember(9)]
		public string Props;

		
		[DBMapping(ColumnName = "gcount")]
		[ProtoMember(10)]
		public int GCount;

		
		[ProtoMember(11)]
		[DBMapping(ColumnName = "binding")]
		public int Binding;

		
		[DBMapping(ColumnName = "jewellist")]
		[ProtoMember(12)]
		public string Jewellist;

		
		[ProtoMember(13)]
		[DBMapping(ColumnName = "bagindex")]
		public int BagIndex;

		
		[DBMapping(ColumnName = "salemoney1")]
		[ProtoMember(14)]
		public int SaleMoney1;

		
		[ProtoMember(15)]
		[DBMapping(ColumnName = "saleyuanbao")]
		public int SaleYuanBao;

		
		[DBMapping(ColumnName = "saleyinpiao")]
		[ProtoMember(16)]
		public int SaleYinPiao;

		
		[DBMapping(ColumnName = "addpropindex")]
		[ProtoMember(17)]
		public int AddPropIndex;

		
		[ProtoMember(18)]
		[DBMapping(ColumnName = "bornindex")]
		public int BornIndex;

		
		[ProtoMember(19)]
		[DBMapping(ColumnName = "lucky")]
		public int Lucky;

		
		[ProtoMember(20)]
		[DBMapping(ColumnName = "strong")]
		public int Strong;

		
		[DBMapping(ColumnName = "excellenceinfo")]
		[ProtoMember(21)]
		public int ExcellenceInfo;

		
		[DBMapping(ColumnName = "appendproplev")]
		[ProtoMember(22)]
		public int AppendPropLev;

		
		[ProtoMember(23)]
		[DBMapping(ColumnName = "equipchangelife")]
		public int ChangeLifeLevForEquip;

		
		[ProtoMember(24)]
		public List<int> WashProps;

		
		[ProtoMember(25)]
		public List<int> ElementhrtsProps;

		
		[ProtoMember(26)]
		[DBMapping(ColumnName = "juhun")]
		public int JuHunID;
	}
}
