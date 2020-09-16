using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JingLingYuanSuJueXingData
	{
		
		[ProtoMember(1)]
		public int ActiveType;

		
		[ProtoMember(2)]
		public int[] ActiveIDs;
	}
}
