using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SkillEquipData
	{
		
		[ProtoMember(1)]
		public int SkillEquip;

		
		[ProtoMember(2)]
		public List<int> ShenShiActiveList;
	}
}
