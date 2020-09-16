using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ServerListData
	{
		
		[ProtoMember(1)]
		public int RetCode = 0;

		
		[ProtoMember(2)]
		public int RolesCount = 0;

		
		[ProtoMember(3)]
		public List<LineData> LineDataList = null;
	}
}
