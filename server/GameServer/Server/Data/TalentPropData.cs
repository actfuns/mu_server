using System;
using System.Collections.Generic;
using GameServer.Logic;

namespace Server.Data
{
	// Token: 0x02000192 RID: 402
	public class TalentPropData
	{
		// Token: 0x060004D5 RID: 1237 RVA: 0x00042767 File Offset: 0x00040967
		public TalentPropData()
		{
			this.ResetProps();
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x00042796 File Offset: 0x00040996
		public void ResetProps()
		{
			this.PropItem.ResetProps();
			this.SkillOneValue = new Dictionary<int, int>();
			this.SkillAllValue = 0;
		}

		// Token: 0x040008E0 RID: 2272
		public Dictionary<int, int> SkillOneValue = new Dictionary<int, int>();

		// Token: 0x040008E1 RID: 2273
		public int SkillAllValue = 2;

		// Token: 0x040008E2 RID: 2274
		public EquipPropItem PropItem = new EquipPropItem();
	}
}
