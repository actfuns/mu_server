using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class KuaFuWorldRoleData
	{
		
		[ProtoMember(1)]
		public int LocalRoleID;

		
		[ProtoMember(2)]
		public int TempRoleID;

		
		[ProtoMember(3)]
		public string WorldRoleID;
	}
}
