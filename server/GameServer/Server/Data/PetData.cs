using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PetData
	{
		
		[ProtoMember(1)]
		public int DbID = 0;

		
		[ProtoMember(2)]
		public int PetID = 0;

		
		[ProtoMember(3)]
		public string PetName = "";

		
		[ProtoMember(4)]
		public int PetType = 0;

		
		[ProtoMember(5)]
		public int FeedNum = 0;

		
		[ProtoMember(6)]
		public int ReAliveNum = 0;

		
		[ProtoMember(7)]
		public long AddDateTime = 0L;

		
		[ProtoMember(8)]
		public string PetProps = "";

		
		[ProtoMember(9)]
		public int Level = 1;
	}
}
