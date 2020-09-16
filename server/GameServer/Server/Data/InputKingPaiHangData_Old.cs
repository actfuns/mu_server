using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class InputKingPaiHangData_Old
	{
		
		[ProtoMember(1)]
		public string UserID;

		
		[ProtoMember(2)]
		public int PaiHang;

		
		[ProtoMember(3)]
		public string PaiHangTime = "";

		
		[ProtoMember(4)]
		public int PaiHangValue;

		
		[ProtoMember(5)]
		public string MaxLevelRoleName = "";

		
		[ProtoMember(6)]
		public int MaxLevelRoleZoneID = 1;
	}
}
