using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class TianTi5v5AwardsData
	{
		
		[ProtoMember(1)]
		public int Success;

		
		[ProtoMember(2)]
		public int DuanWeiJiFen;

		
		[ProtoMember(3)]
		public int RongYao;

		
		[ProtoMember(4)]
		public int LianShengJiFen;

		
		[ProtoMember(5)]
		public int DuanWeiId;
	}
}
