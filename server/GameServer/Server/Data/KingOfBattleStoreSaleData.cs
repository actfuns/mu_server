using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class KingOfBattleStoreSaleData
	{
		
		[ProtoMember(1)]
		public int ID;

		
		[ProtoMember(2)]
		public int Purchase;
	}
}
