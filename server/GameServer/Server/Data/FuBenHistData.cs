using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FuBenHistData
	{
		
		[ProtoMember(1)]
		public int FuBenID = 0;

		
		[ProtoMember(2)]
		public int RoleID = 0;

		
		[ProtoMember(3)]
		public string RoleName = "";

		
		[ProtoMember(4)]
		public int UsedSecs = 0;
	}
}
