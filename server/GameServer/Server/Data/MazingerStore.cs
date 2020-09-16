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
		public MazingerStoreData data;

		
		[ProtoMember(3)]
		public int IsBoom = 0;
	}
}
