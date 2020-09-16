using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class YongZheZhanChangScoreData
	{
		
		[ProtoMember(1)]
		public int Score1;

		
		[ProtoMember(2)]
		public int Score2;
	}
}
