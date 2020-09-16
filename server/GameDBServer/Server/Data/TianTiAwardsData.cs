using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class TianTiAwardsData
	{
		
		[ProtoMember(1)]
		public int Success;

		
		[ProtoMember(2)]
		public int DuanWeiJiFen;

		
		[ProtoMember(3)]
		public int RongYao;
	}
}
