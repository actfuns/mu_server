using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LangHunLingYuQiZhiBuffOwnerData
	{
		
		[ProtoMember(1)]
		public int NPCID;

		
		[ProtoMember(2)]
		public int OwnerBHID;

		
		[ProtoMember(3)]
		public string OwnerBHName;

		
		[ProtoMember(4)]
		public int OwnerBHZoneId = 0;
	}
}
