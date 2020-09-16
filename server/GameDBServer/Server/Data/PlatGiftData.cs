using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PlatGiftData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public string UserID = null;

		
		[ProtoMember(3)]
		public int Type = 0;

		
		[ProtoMember(4)]
		public int ID = 0;

		
		[ProtoMember(5)]
		public string ExtraData = null;

		
		[ProtoMember(6)]
		public string Message = null;

		
		[ProtoMember(7)]
		public long Time = 0L;

		
		[ProtoMember(8)]
		public string RoleName = null;

		
		[ProtoMember(9)]
		public string Sign = null;
	}
}
