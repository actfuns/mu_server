using System;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020001E5 RID: 485
	public class PassiveSkillData
	{
		// Token: 0x06000615 RID: 1557 RVA: 0x00055AF7 File Offset: 0x00053CF7
		public PassiveSkillData()
		{
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x00055B10 File Offset: 0x00053D10
		public PassiveSkillData(int _skillId, int _skillLevel, int _triggerType, int _triggerRate, int _coolDown, int _spanTime)
		{
			this.skillId = _skillId;
			this.skillLevel = _skillLevel;
			this.triggerType = _triggerType;
			this.triggerRate = _triggerRate;
			this.coolDown = _coolDown;
			this.triggerCD = _spanTime;
			this.skillData.SkillID = this.skillId;
			this.skillData.SkillLevel = this.skillLevel;
		}

		// Token: 0x04000A99 RID: 2713
		public int skillId;

		// Token: 0x04000A9A RID: 2714
		public int skillLevel;

		// Token: 0x04000A9B RID: 2715
		public int triggerRate;

		// Token: 0x04000A9C RID: 2716
		public int triggerType;

		// Token: 0x04000A9D RID: 2717
		public int coolDown;

		// Token: 0x04000A9E RID: 2718
		public int triggerCD;

		// Token: 0x04000A9F RID: 2719
		public SkillData skillData = new SkillData();
	}
}
