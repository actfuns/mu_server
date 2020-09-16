using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	internal class TodayCandoData
	{
		
		[ProtoMember(1)]
		public int ID;

		
		[ProtoMember(2)]
		public int LeftCount = 0;
	}
}
