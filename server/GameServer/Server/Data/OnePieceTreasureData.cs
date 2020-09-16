using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class OnePieceTreasureData
	{
		
		[ProtoMember(1)]
		public int PosID = 0;

		
		[ProtoMember(2)]
		public int EventID = 0;

		
		[ProtoMember(3)]
		public int RollNumNormal = 0;

		
		[ProtoMember(4)]
		public int RollNumMiracle = 0;

		
		[ProtoMember(5)]
		public long ResetPosTicks = 0L;
	}
}
