using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020003FA RID: 1018
	[ProtoContract]
	public class RoleCustomDataItem
	{
		// Token: 0x04001B2B RID: 6955
		[ProtoMember(1, IsRequired = true)]
		public int Occupation;

		// Token: 0x04001B2C RID: 6956
		[ProtoMember(2, IsRequired = true)]
		public string Main_quick_keys;

		// Token: 0x04001B2D RID: 6957
		[ProtoMember(3, IsRequired = true)]
		public List<int> RolePointList = new List<int>();

		// Token: 0x04001B2E RID: 6958
		[ProtoMember(4, IsRequired = true)]
		public List<TalentEffectItem> EffectList = new List<TalentEffectItem>();

		// Token: 0x04001B2F RID: 6959
		[ProtoMember(5, IsRequired = true)]
		public int DefaultSkillLev;

		// Token: 0x04001B30 RID: 6960
		[ProtoMember(6, IsRequired = true)]
		public int DefaultSkillUseNum;

		// Token: 0x04001B31 RID: 6961
		[ProtoMember(7, IsRequired = false)]
		public List<SkillEquipData> ShenShiEuipSkill;
	}
}
