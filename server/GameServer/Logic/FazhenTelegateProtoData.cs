using System;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class FazhenTelegateProtoData
	{
		
		[ProtoMember(1)]
		public int gateId = 0;

		
		[ProtoMember(2)]
		public int DestMapCode = 0;
	}
}
