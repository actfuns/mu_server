using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class KaiFuOnlineInfoData
	{
		
		[ProtoMember(1)]
		public int SelfDayBit = 0;

		
		[ProtoMember(2)]
		public List<int> SelfDayOnlineSecsList;

		
		[ProtoMember(3)]
		public List<KaiFuOnlineAwardData> KaiFuOnlineAwardDataList;
	}
}
