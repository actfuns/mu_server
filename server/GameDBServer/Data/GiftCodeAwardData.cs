using System;
using ProtoBuf;

namespace GameDBServer.Data
{
	
	[ProtoContract]
	public class GiftCodeAwardData
	{
		
		[ProtoMember(1)]
		public int Dbid = 0;

		
		[ProtoMember(2)]
		public string UserId = "";

		
		[ProtoMember(3)]
		public int RoleID = 0;

		
		[ProtoMember(4)]
		public string GiftId = "";

		
		[ProtoMember(5)]
		public string CodeNo = "";
	}
}
