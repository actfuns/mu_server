using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RoleCustomData
	{
		
		[ProtoMember(1, IsRequired = true)]
		public int roleId;

		
		[ProtoMember(2, IsRequired = true)]
		public RoleData4Selector roleData4Selector;

		
		[ProtoMember(3, IsRequired = true)]
		public List<RoleCustomDataItem> customDataList;
	}
}
