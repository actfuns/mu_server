using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class NameRegisterData
	{
		
		[ProtoMember(1)]
		public string Name;

		
		[ProtoMember(2)]
		public string PingTaiID;

		
		[ProtoMember(3)]
		public int ZoneID;

		
		[ProtoMember(4)]
		public int NameType;

		
		[ProtoMember(5)]
		public string UserID;

		
		[ProtoMember(6)]
		public string RegTime;
	}
}
