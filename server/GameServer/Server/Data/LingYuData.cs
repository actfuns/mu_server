using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LingYuData
	{
		
		public LingYuData()
		{
			this.Type = -1;
			this.Level = 0;
			this.Suit = 0;
		}

		
		[ProtoMember(1)]
		public int Type;

		
		[ProtoMember(2)]
		public int Level;

		
		[ProtoMember(3)]
		public int Suit;
	}
}
