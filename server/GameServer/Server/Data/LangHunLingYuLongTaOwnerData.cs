using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LangHunLingYuLongTaOwnerData
	{
		
		[ProtoMember(1)]
		public string OwnerBHName = "";

		
		[ProtoMember(2)]
		public int OwnerBHid = 0;

		
		[ProtoMember(3)]
		public int OwnerBHZoneId = 0;

		
		[ProtoMember(4)]
		public int OwnerBHServerId = 0;
	}
}
