using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ChangeNameInfo
	{
		
		[ProtoMember(1)]
		public int ZuanShi = 0;

		
		[ProtoMember(2)]
		public List<EachRoleChangeName> RoleList = new List<EachRoleChangeName>();
	}
}
