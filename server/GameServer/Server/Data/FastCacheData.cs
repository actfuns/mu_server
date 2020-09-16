using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FastCacheData
	{
		
		[ProtoMember(1)]
		public int ID;

		
		[ProtoMember(2)]
		public bool Flag_BaseInfo;

		
		[ProtoMember(3)]
		public string Position;

		
		[ProtoMember(4)]
		public long ZhanLi;
	}
}
