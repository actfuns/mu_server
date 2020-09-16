using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ZhanDuiZhengBaAwardsData
	{
		
		[ProtoMember(1)]
		public int Success;

		
		[ProtoMember(2)]
		public int NewGrade;

		
		[ProtoMember(3)]
		public string Awards;
	}
}
