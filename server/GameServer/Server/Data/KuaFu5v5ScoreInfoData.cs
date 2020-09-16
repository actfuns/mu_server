using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class KuaFu5v5ScoreInfoData
	{
		
		[ProtoMember(3)]
		public long Count1;

		
		[ProtoMember(4)]
		public int Count2;
	}
}
