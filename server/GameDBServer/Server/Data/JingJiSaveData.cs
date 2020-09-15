using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000075 RID: 117
	[ProtoContract]
	public class JingJiSaveData
	{
		// Token: 0x0400027D RID: 637
		[ProtoMember(1)]
		public bool isWin;

		// Token: 0x0400027E RID: 638
		[ProtoMember(2)]
		public int roleId;

		// Token: 0x0400027F RID: 639
		[ProtoMember(3)]
		public int level;

		// Token: 0x04000280 RID: 640
		[ProtoMember(4)]
		public int changeLiveCount;

		// Token: 0x04000281 RID: 641
		[ProtoMember(5)]
		public long nextChallengeTime;

		// Token: 0x04000282 RID: 642
		[ProtoMember(6)]
		public double[] baseProps;

		// Token: 0x04000283 RID: 643
		[ProtoMember(7)]
		public double[] extProps;

		// Token: 0x04000284 RID: 644
		[ProtoMember(8)]
		public List<PlayerJingJiEquipData> equipDatas;

		// Token: 0x04000285 RID: 645
		[ProtoMember(9)]
		public List<PlayerJingJiSkillData> skillDatas;

		// Token: 0x04000286 RID: 646
		[ProtoMember(10)]
		public int combatForce = 0;

		// Token: 0x04000287 RID: 647
		[ProtoMember(11)]
		public int robotId;

		// Token: 0x04000288 RID: 648
		[ProtoMember(12)]
		public WingData wingData;

		// Token: 0x04000289 RID: 649
		[ProtoMember(13)]
		public long settingFlags;

		// Token: 0x0400028A RID: 650
		[ProtoMember(14)]
		public int Occupation;

		// Token: 0x0400028B RID: 651
		[ProtoMember(16)]
		public SkillEquipData ShenShiEuipSkill;

		// Token: 0x0400028C RID: 652
		[ProtoMember(17)]
		public List<int> PassiveEffectList;

		// Token: 0x0400028D RID: 653
		[ProtoMember(18)]
		public int SubOccupation;
	}
}
