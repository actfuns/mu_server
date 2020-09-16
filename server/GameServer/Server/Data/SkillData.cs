using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SkillData
	{
		
		[ProtoMember(1)]
		public int DbID;

		
		[ProtoMember(2)]
		public int SkillID;

		
		[ProtoMember(3)]
		public int SkillLevel;

		
		[ProtoMember(4)]
		public int UsedNum;
	}
}
