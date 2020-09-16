using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RebornRankAdmireData
	{
		
		[ProtoMember(1)]
		public int AdmireCount;

		
		[ProtoMember(2)]
		public RoleData4Selector RoleData4Selector;

		
		[ProtoMember(3)]
		public int Value;

		
		[ProtoMember(4)]
		public int PtID;

		
		[ProtoMember(5)]
		public string Param;
	}
}
