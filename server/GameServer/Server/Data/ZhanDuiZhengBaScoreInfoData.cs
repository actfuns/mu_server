using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ZhanDuiZhengBaScoreInfoData
	{
		
		[ProtoMember(3)]
		public long Count1;

		
		[ProtoMember(4)]
		public int Count2;
	}
}
