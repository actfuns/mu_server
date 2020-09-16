using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FakeRoleData
	{
		
		[ProtoMember(1)]
		public int FakeRoleID = 0;

		
		[ProtoMember(2)]
		public int FakeRoleType = 0;

		
		[ProtoMember(3)]
		public int ToExtensionID = 0;

		
		[ProtoMember(4)]
		public RoleDataMini MyRoleDataMini = null;
	}
}
