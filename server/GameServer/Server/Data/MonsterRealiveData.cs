using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	internal class MonsterRealiveData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public int PosX = 0;

		
		[ProtoMember(3)]
		public int PosY = 0;

		
		[ProtoMember(4)]
		public int Direction = 0;
	}
}
