using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class MapTeleportInfo
	{
		
		[ProtoMember(1)]
		public int MapCode = 0;

		
		[ProtoMember(2)]
		public List<TeleportState> TeleportStateList;
	}
}
