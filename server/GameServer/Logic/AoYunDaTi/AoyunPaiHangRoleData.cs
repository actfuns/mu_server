using System;
using ProtoBuf;

namespace GameServer.Logic.AoYunDaTi
{
	
	[ProtoContract]
	public class AoyunPaiHangRoleData
	{
		
		[ProtoMember(1)]
		public int ZoneId = 0;

		
		[ProtoMember(2)]
		public int RoleId = 0;

		
		[ProtoMember(3)]
		public string RoleName = null;

		
		[ProtoMember(4)]
		public int RolePoint = 0;

		
		[ProtoMember(5)]
		public int RoleCurrentPoint;

		
		[ProtoMember(6)]
		public int RoleRank = 0;
	}
}
