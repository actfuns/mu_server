using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class TalentEffectItem
	{
		
		[ProtoMember(1, IsRequired = true)]
		public int ID = 0;

		
		[ProtoMember(2, IsRequired = true)]
		public int Level = 0;

		
		[ProtoMember(3, IsRequired = true)]
		public int TalentType = 1;

		
		[ProtoMember(4, IsRequired = true)]
		public List<TalentEffectInfo> ItemEffectList = null;
	}
}
