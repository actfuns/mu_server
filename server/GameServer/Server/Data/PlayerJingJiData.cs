using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200057A RID: 1402
	[ProtoContract]
	public class PlayerJingJiData
	{
		// Token: 0x040025D2 RID: 9682
		[ProtoMember(1)]
		public int roleId;

		// Token: 0x040025D3 RID: 9683
		[ProtoMember(2)]
		public string roleName;

		// Token: 0x040025D4 RID: 9684
		[ProtoMember(3)]
		public int level;

		// Token: 0x040025D5 RID: 9685
		[ProtoMember(4)]
		public int changeLiveCount;

		// Token: 0x040025D6 RID: 9686
		[ProtoMember(5)]
		public int occupationId;

		// Token: 0x040025D7 RID: 9687
		[ProtoMember(6)]
		public int winCount = 0;

		// Token: 0x040025D8 RID: 9688
		[ProtoMember(7)]
		public int ranking = -1;

		// Token: 0x040025D9 RID: 9689
		[ProtoMember(8)]
		public long nextRewardTime;

		// Token: 0x040025DA RID: 9690
		[ProtoMember(9)]
		public long nextChallengeTime;

		// Token: 0x040025DB RID: 9691
		[ProtoMember(10)]
		public double[] baseProps;

		// Token: 0x040025DC RID: 9692
		[ProtoMember(11)]
		public double[] extProps;

		// Token: 0x040025DD RID: 9693
		[ProtoMember(12)]
		public List<PlayerJingJiEquipData> equipDatas;

		// Token: 0x040025DE RID: 9694
		[ProtoMember(13)]
		public List<PlayerJingJiSkillData> skillDatas;

		// Token: 0x040025DF RID: 9695
		[ProtoMember(14)]
		public int combatForce = 0;

		// Token: 0x040025E0 RID: 9696
		[ProtoMember(15)]
		public int sex;

		// Token: 0x040025E1 RID: 9697
		[ProtoMember(16)]
		public string name;

		// Token: 0x040025E2 RID: 9698
		[ProtoMember(17)]
		public int zoneId;

		// Token: 0x040025E3 RID: 9699
		[ProtoMember(18)]
		public int MaxWinCnt;

		// Token: 0x040025E4 RID: 9700
		[ProtoMember(19)]
		public WingData wingData;

		// Token: 0x040025E5 RID: 9701
		[ProtoMember(20)]
		public long settingFlags;

		// Token: 0x040025E6 RID: 9702
		[ProtoMember(21)]
		public int AdmiredCount;

		// Token: 0x040025E7 RID: 9703
		[ProtoMember(22)]
		public List<int> OccupationList;

		// Token: 0x040025E8 RID: 9704
		[ProtoMember(23)]
		public int JunTuanId;

		// Token: 0x040025E9 RID: 9705
		[ProtoMember(24)]
		public string JunTuanName;

		// Token: 0x040025EA RID: 9706
		[ProtoMember(25)]
		public int JunTuanZhiWu;

		// Token: 0x040025EB RID: 9707
		[ProtoMember(26)]
		public int LingDi;

		// Token: 0x040025EC RID: 9708
		[ProtoMember(27)]
		public RoleHuiJiData HuiJiData;

		// Token: 0x040025ED RID: 9709
		[ProtoMember(28)]
		public SkillEquipData ShenShiEquipData;

		// Token: 0x040025EE RID: 9710
		[ProtoMember(29)]
		public List<int> PassiveEffectList;

		// Token: 0x040025EF RID: 9711
		[ProtoMember(30)]
		public int CompType;

		// Token: 0x040025F0 RID: 9712
		[ProtoMember(31)]
		public byte CompZhiWu;

		// Token: 0x040025F1 RID: 9713
		[ProtoMember(32)]
		public int SubOccupation;
	}
}
