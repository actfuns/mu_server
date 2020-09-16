using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LingDiMapInfoData
	{
		
		[ProtoMember(1)]
		public long FightingEndTime = 0L;

		
		[ProtoMember(2)]
		public long FightingStartTime = 0L;

		
		[ProtoMember(3)]
		public Dictionary<int, string> BHNameDict = null;
	}
}
