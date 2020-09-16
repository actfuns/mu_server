using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class KingOfBattleStoreData
	{
		
		[ProtoMember(1)]
		public DateTime LastRefTime;

		
		[ProtoMember(2)]
		public List<KingOfBattleStoreSaleData> SaleList;
	}
}
