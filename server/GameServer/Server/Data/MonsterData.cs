using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000571 RID: 1393
	[ProtoContract]
	public class MonsterData
	{
		// Token: 0x0400258E RID: 9614
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x0400258F RID: 9615
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x04002590 RID: 9616
		[ProtoMember(3)]
		public int RoleSex = 0;

		// Token: 0x04002591 RID: 9617
		[ProtoMember(4)]
		public int Level = 1;

		// Token: 0x04002592 RID: 9618
		[ProtoMember(5)]
		public int Experience = 0;

		// Token: 0x04002593 RID: 9619
		[ProtoMember(6)]
		public int PosX = 0;

		// Token: 0x04002594 RID: 9620
		[ProtoMember(7)]
		public int PosY = 0;

		// Token: 0x04002595 RID: 9621
		[ProtoMember(8)]
		public int RoleDirection = 0;

		// Token: 0x04002596 RID: 9622
		[ProtoMember(9)]
		public double LifeV = 0.0;

		// Token: 0x04002597 RID: 9623
		[ProtoMember(10)]
		public double MaxLifeV = 0.0;

		// Token: 0x04002598 RID: 9624
		[ProtoMember(11)]
		public double MagicV = 0.0;

		// Token: 0x04002599 RID: 9625
		[ProtoMember(12)]
		public double MaxMagicV = 0.0;

		// Token: 0x0400259A RID: 9626
		[ProtoMember(13)]
		public int EquipmentBody = 0;

		// Token: 0x0400259B RID: 9627
		[ProtoMember(14)]
		public int ExtensionID = 0;

		// Token: 0x0400259C RID: 9628
		[ProtoMember(15)]
		public int MonsterType = 0;

		// Token: 0x0400259D RID: 9629
		[ProtoMember(16)]
		public int MasterRoleID = 0;

		// Token: 0x0400259E RID: 9630
		[ProtoMember(17)]
		public ushort AiControlType = 1;

		// Token: 0x0400259F RID: 9631
		[ProtoMember(18)]
		public string AnimalSound = "";

		// Token: 0x040025A0 RID: 9632
		[ProtoMember(19)]
		public int MonsterLevel = 0;

		// Token: 0x040025A1 RID: 9633
		[ProtoMember(20)]
		public long ZhongDuStart = 0L;

		// Token: 0x040025A2 RID: 9634
		[ProtoMember(21)]
		public int ZhongDuSeconds = 0;

		// Token: 0x040025A3 RID: 9635
		[ProtoMember(22)]
		public long FaintStart = 0L;

		// Token: 0x040025A4 RID: 9636
		[ProtoMember(23)]
		public int FaintSeconds = 0;

		// Token: 0x040025A5 RID: 9637
		[ProtoMember(24)]
		public int BattleWitchSide;

		// Token: 0x040025A6 RID: 9638
		[ProtoMember(25)]
		public long BirthTicks;
	}
}
