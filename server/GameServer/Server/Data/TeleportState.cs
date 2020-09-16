using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class TeleportState
	{
		
		[ProtoMember(1)]
		public int ToMapCode;

		
		[ProtoMember(2)]
		public int State;
	}
}
