using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RoleCustomDataItem
	{
		
		[ProtoMember(1, IsRequired = true)]
		public int Occupation;

		
		[ProtoMember(2, IsRequired = true)]
		public string Main_quick_keys;

		
		[ProtoMember(3, IsRequired = true)]
		public List<int> RolePointList = new List<int>();

		
		[ProtoMember(4, IsRequired = true)]
		public List<TalentEffectItem> EffectList = new List<TalentEffectItem>();

		
		[ProtoMember(5, IsRequired = true)]
		public int DefaultSkillLev;

		
		[ProtoMember(6, IsRequired = true)]
		public int DefaultSkillUseNum;

		
		[ProtoMember(7, IsRequired = false)]
		public List<SkillEquipData> ShenShiEuipSkill;
	}
}
