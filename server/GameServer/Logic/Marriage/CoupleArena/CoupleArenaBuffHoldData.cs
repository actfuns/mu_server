using System;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleArena
{
	
	[ProtoContract]
	public class CoupleArenaBuffHoldData
	{
		
		[ProtoMember(1, IsRequired = true)]
		public bool IsZhenAiBuffValid;

		
		[ProtoMember(2)]
		public int ZhenAiHolderZoneId;

		
		[ProtoMember(3)]
		public string ZhenAiHolderRname;

		
		[ProtoMember(4, IsRequired = true)]
		public bool IsYongQiBuffValid;

		
		[ProtoMember(5)]
		public int YongQiHolderZoneId;

		
		[ProtoMember(6)]
		public string YongQiHolderRname;
	}
}
