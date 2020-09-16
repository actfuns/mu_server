using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.TarotData
{
	
	[ProtoContract]
	public class TarotSystemData
	{
		
		public TarotSystemData()
		{
			this.KingData = new TarotKingData();
			this.TarotCardDatas = new List<TarotCardData>();
		}

		
		[ProtoMember(1)]
		public TarotKingData KingData;

		
		[ProtoMember(2)]
		public List<TarotCardData> TarotCardDatas;
	}
}
