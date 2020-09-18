using System;
using ProtoBuf;

namespace GameDBServer.Logic.BoCai
{
	
	[ProtoContract]
	public class BoCaiShopDB
	{
		
		[ProtoMember(1)]
		public int ID;

		
		[ProtoMember(2)]
		public string WuPinID;

		
		[ProtoMember(3)]
		public int BuyNum;

		
		[ProtoMember(4)]
		public int RoleID;

		
		[ProtoMember(5)]
		public int Periods;
	}
}
