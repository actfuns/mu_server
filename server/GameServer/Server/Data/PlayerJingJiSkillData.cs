using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PlayerJingJiSkillData
	{
		
		[ProtoMember(1)]
		public int skillID;

		
		[ProtoMember(2)]
		public int skillLevel;
	}
}
