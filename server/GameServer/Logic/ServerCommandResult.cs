using System;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class ServerCommandResult
	{
		
		[ProtoMember(1)]
		public string Cmd;

		
		[ProtoMember(2)]
		public string ResultString;
	}
}
