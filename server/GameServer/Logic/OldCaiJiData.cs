using System;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class OldCaiJiData
	{
		
		[ProtoMember(1)]
		public int OldDay = -1;

		
		[ProtoMember(2)]
		public int OldNum = -1;
	}
}
