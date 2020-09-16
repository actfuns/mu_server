using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class DecorationData
	{
		
		[ProtoMember(1)]
		public int AutoID = 0;

		
		[ProtoMember(2)]
		public int DecoID = 0;

		
		[ProtoMember(3)]
		public int PosX = 0;

		
		[ProtoMember(4)]
		public int PosY = 0;

		
		[ProtoMember(5)]
		public int MapCode = -1;

		
		[ProtoMember(6)]
		public long StartTicks = 0L;

		
		[ProtoMember(7)]
		public int MaxLiveTicks = 0;

		
		[ProtoMember(8)]
		public int AlphaTicks = 0;
	}
}
