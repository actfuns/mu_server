using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class EscapeBattleAreaInfo
	{
		
		[ProtoMember(1)]
		public int AreaID;

		
		[ProtoMember(2)]
		public int PosX;

		
		[ProtoMember(3)]
		public int PosY;
	}
}
