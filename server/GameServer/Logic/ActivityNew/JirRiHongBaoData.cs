using System;
using ProtoBuf;

namespace GameServer.Logic.ActivityNew
{
	
	[ProtoContract]
	public class JirRiHongBaoData
	{
		
		[ProtoMember(1)]
		public int ID;

		
		[ProtoMember(2)]
		public int State;
	}
}
