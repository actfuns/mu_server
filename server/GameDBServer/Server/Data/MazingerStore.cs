using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class MazingerStore
	{
		
		[ProtoMember(1)]
		public int result = 0;

		
		[ProtoMember(2)]
		private MazingerStoreData data;
	}
}
