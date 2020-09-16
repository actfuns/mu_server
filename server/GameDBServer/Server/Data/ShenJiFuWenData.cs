using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ShenJiFuWenData
	{
		
		[ProtoMember(1)]
		public int ShenJiID = 0;

		
		[ProtoMember(2)]
		public int Level = 0;
	}
}
