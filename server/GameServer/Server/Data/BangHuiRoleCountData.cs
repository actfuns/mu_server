using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangHuiRoleCountData
	{
		
		[ProtoMember(1)]
		public int BHID;

		
		[ProtoMember(2)]
		public int RoleCount;

		
		[ProtoMember(3)]
		public int ServerID;
	}
}
