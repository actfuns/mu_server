using System;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic.Olympics
{
	
	[ProtoContract]
	public class OlympicsShopData
	{
		
		public OlympicsShopData Clone(OlympicsShopData data)
		{
			this.ID = data.ID;
			this.DayID = data.DayID;
			this.Goods = data.Goods;
			this.Price = data.Price;
			this.NumSingle = data.NumSingle;
			this.NumFull = data.NumFull;
			this.NumSingleBuy = data.NumSingleBuy;
			this.NumFullBuy = data.NumFullBuy;
			return this;
		}

		
		[ProtoMember(1)]
		public int ID = 0;

		
		[ProtoMember(2)]
		public int DayID = 0;

		
		[ProtoMember(3)]
		public GoodsData Goods = null;

		
		[ProtoMember(4)]
		public int Price = 0;

		
		[ProtoMember(5)]
		public int NumSingle = 0;

		
		[ProtoMember(6)]
		public int NumFull = 0;

		
		[ProtoMember(7)]
		public int NumSingleBuy = 0;

		
		[ProtoMember(8)]
		public int NumFullBuy = -1;
	}
}
