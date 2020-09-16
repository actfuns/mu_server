using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SystemOpenData : ICloneable
	{
		
		public object Clone()
		{
			return base.MemberwiseClone();
		}

		
		[ProtoMember(1)]
		public int paimaihangjinbi;

		
		[ProtoMember(2)]
		public int paimaihangzuanshi;

		
		[ProtoMember(3)]
		public int paimaihangmobi;

		
		[ProtoMember(4)]
		public int bangzuan;

		
		[ProtoMember(5)]
		public int zuanshi;

		
		[ProtoMember(6)]
		public int mobi;

		
		[ProtoMember(7)]
		public int paimaijiemianmobi;

		
		[ProtoMember(8)]
		public int paimaihangyinpiao;
	}
}
