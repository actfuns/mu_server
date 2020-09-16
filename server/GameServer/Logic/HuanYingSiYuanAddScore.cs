using System;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class HuanYingSiYuanAddScore
	{
		
		[ProtoMember(1)]
		public int Score;

		
		[ProtoMember(2)]
		public int ZoneID;

		
		[ProtoMember(3)]
		public string Name = "";

		
		[ProtoMember(4)]
		public int Side;

		
		[ProtoMember(5)]
		public int RoleId;

		
		[ProtoMember(6)]
		public int ByLianShaNum;

		
		[ProtoMember(7)]
		public int Occupation;
	}
}
