using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PaiHangItemData
	{
		
		[ProtoMember(1)]
		public int RoleID;

		
		[ProtoMember(2)]
		public string RoleName;

		
		[ProtoMember(3)]
		public int Val1;

		
		[ProtoMember(4)]
		public int Val2;

		
		[ProtoMember(5)]
		public int Val3;

		
		[ProtoMember(6)]
		public string uid;
	}
}
