using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class GVoicePriorityData
	{
		
		[ProtoMember(5)]
		public string RoleIdList;

		
		[ProtoMember(6)]
		public int Type;

		
		[ProtoMember(7)]
		public int ID;
	}
}
