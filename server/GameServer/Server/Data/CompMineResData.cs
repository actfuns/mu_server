using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class CompMineResData
	{
		
		[ProtoMember(1)]
		public int CompType;

		
		[ProtoMember(2)]
		public int MineRes;

		
		public int Rank;
	}
}
