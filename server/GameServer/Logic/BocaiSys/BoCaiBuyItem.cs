using System;
using ProtoBuf;

namespace GameServer.Logic.BocaiSys
{
	
	[ProtoContract]
	public class BoCaiBuyItem
	{
		
		[ProtoMember(1)]
		public int BuyNum;

		
		[ProtoMember(2)]
		public string strBuyValue;

		
		[ProtoMember(3)]
		public long DataPeriods;
	}
}
