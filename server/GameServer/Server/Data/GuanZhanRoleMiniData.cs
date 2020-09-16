using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class GuanZhanRoleMiniData
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
		public int Occupation = 0;

		
		[ProtoMember(6)]
		public int RoleSex = 0;

		
		[ProtoMember(7)]
		public int BHZhiWu = 0;

		
		[ProtoMember(8)]
		public int Param1 = 0;

		
		[ProtoMember(9)]
		public int Param2 = 0;
	}
}
