using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PrisonFuLuData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public string Name;

		
		[ProtoMember(3)]
		public int Level = 0;

		
		[ProtoMember(4)]
		public int ChangeLevel = 0;

		
		[ProtoMember(5)]
		public int ZoneID = 0;

		
		[ProtoMember(6)]
		public int LaoDongState = 0;

		
		[ProtoMember(7)]
		public long LaoDongTime = 0L;

		
		[ProtoMember(8)]
		public int Occupation = 0;

		
		[ProtoMember(9)]
		public int RoleSex = 0;
	}
}
