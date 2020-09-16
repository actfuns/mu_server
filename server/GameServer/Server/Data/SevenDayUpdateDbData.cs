using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SevenDayUpdateDbData
	{
		
		[ProtoMember(1)]
		public int RoleId;

		
		[ProtoMember(2)]
		public int ActivityType;

		
		[ProtoMember(3)]
		public int Id;

		
		[ProtoMember(4)]
		public SevenDayItemData Data;
	}
}
