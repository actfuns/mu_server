using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Data
{
	
	[ProtoContract]
	public class RebornStampData
	{
		
		[ProtoMember(1)]
		public int RoleID;

		
		[ProtoMember(2)]
		public int ResetNum;

		
		[ProtoMember(3)]
		public int UsePoint;

		
		[ProtoMember(4)]
		public List<int> StampInfo;
	}
}
