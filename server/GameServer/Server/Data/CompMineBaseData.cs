using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class CompMineBaseData
	{
		
		[ProtoMember(1)]
		public int MineTruckGo;

		
		[ProtoMember(2)]
		public int SafeArrived;
	}
}
