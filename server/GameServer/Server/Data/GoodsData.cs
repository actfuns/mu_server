using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class GoodsData
	{
		
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

		
		public GoodsData()
		{
		}

		
		[ProtoMember(1)]
		public int Id;

		
		[ProtoMember(2)]
		public int GoodsID;

		
		[ProtoMember(3)]
		public int Using;

		
		[ProtoMember(4)]
		public int Forge_level;

		
		[ProtoMember(5)]
		public string Starttime;

		
		[ProtoMember(6)]
		public string Endtime;

		
		[ProtoMember(7)]
		public int Site;

		
		[ProtoMember(8)]
		public int Quality;

		
		[ProtoMember(9)]
		public string Props;

		
		[ProtoMember(10)]
		public int GCount;

		
		[ProtoMember(11)]
		public int Binding;

		
		[ProtoMember(12)]
		public string Jewellist;

		
		[ProtoMember(13)]
		public int BagIndex;

		
		[ProtoMember(14)]
		public int SaleMoney1;

		
		[ProtoMember(15)]
		public int SaleYuanBao;

		
		[ProtoMember(16)]
		public int SaleYinPiao;

		
		[ProtoMember(17)]
		public int AddPropIndex;

		
		[ProtoMember(18)]
		public int BornIndex;

		
		[ProtoMember(19)]
		public int Lucky;

		
		[ProtoMember(20)]
		public int Strong;

		
		[ProtoMember(21)]
		public int ExcellenceInfo;

		
		[ProtoMember(22)]
		public int AppendPropLev;

		
		[ProtoMember(23)]
		public int ChangeLifeLevForEquip;

		
		[ProtoMember(24)]
		public List<int> WashProps;

		
		[ProtoMember(25)]
		public List<int> ElementhrtsProps;

		
		[ProtoMember(26)]
		public int JuHunID;
	}
}
