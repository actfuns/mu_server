using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ListRolesData
	{
		
		[ProtoMember(1)]
		public int StartIndex = 0;

		
		[ProtoMember(2)]
		public int TotalRolesCount = 0;

		
		[ProtoMember(3)]
		public int PageRolesCount = 0;

		
		[ProtoMember(4)]
		public List<SearchRoleData> SearchRoleDataList = null;
	}
}
