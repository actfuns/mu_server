using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class NPCRole
	{
		
		[ProtoMember(1)]
		public int NpcID = 0;

		
		[ProtoMember(2)]
		public int PosX = 0;

		
		[ProtoMember(3)]
		public int PosY = 0;

		
		[ProtoMember(4)]
		public int MapCode = -1;

		
		[ProtoMember(5)]
		public string RoleString = "";

		
		[ProtoMember(6)]
		public int Dir = 0;
	}
}
