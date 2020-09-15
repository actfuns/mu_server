using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000563 RID: 1379
	[ProtoContract]
	public class JingJiSaveData
	{
		// Token: 0x0400251E RID: 9502
		[ProtoMember(1)]
		public bool isWin;

		// Token: 0x0400251F RID: 9503
		[ProtoMember(2)]
		public int roleId;

		// Token: 0x04002520 RID: 9504
		[ProtoMember(3)]
		public int level;

		// Token: 0x04002521 RID: 9505
		[ProtoMember(4)]
		public int changeLiveCount;

		// Token: 0x04002522 RID: 9506
		[ProtoMember(5)]
		public long nextChallengeTime;

		// Token: 0x04002523 RID: 9507
		[ProtoMember(6)]
		public double[] baseProps;

		// Token: 0x04002524 RID: 9508
		[ProtoMember(7)]
		public double[] extProps;

		// Token: 0x04002525 RID: 9509
		[ProtoMember(8)]
		public List<PlayerJingJiEquipData> equipDatas;

		// Token: 0x04002526 RID: 9510
		[ProtoMember(9)]
		public List<PlayerJingJiSkillData> skillDatas;

		// Token: 0x04002527 RID: 9511
		[ProtoMember(10)]
		public int combatForce = 0;

		// Token: 0x04002528 RID: 9512
		[ProtoMember(11)]
		public int robotId;

		// Token: 0x04002529 RID: 9513
		[ProtoMember(12)]
		public WingData wingData;

		// Token: 0x0400252A RID: 9514
		[ProtoMember(13)]
		public long settingFlags;

		// Token: 0x0400252B RID: 9515
		[ProtoMember(14)]
		public int Occupation;

		// Token: 0x0400252C RID: 9516
		[ProtoMember(15)]
		public RoleHuiJiData HuiJiData;

		// Token: 0x0400252D RID: 9517
		[ProtoMember(16)]
		public SkillEquipData ShenShiEuipSkill;

		// Token: 0x0400252E RID: 9518
		[ProtoMember(17)]
		public List<int> PassiveEffectList;

		// Token: 0x0400252F RID: 9519
		[ProtoMember(18)]
		public int SubOccupation;
	}
}
