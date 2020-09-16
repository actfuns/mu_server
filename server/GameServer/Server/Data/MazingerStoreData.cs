using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class MazingerStoreData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public int Type = 0;

		
		[ProtoMember(3)]
		public int Stage = 0;

		
		[ProtoMember(4)]
		public int StarLevel = 0;

		
		[ProtoMember(5)]
		public int Exp = 0;
	}
}
