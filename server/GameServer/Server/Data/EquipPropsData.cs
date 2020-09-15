using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200012E RID: 302
	[ProtoContract]
	public class EquipPropsData
	{
		// Token: 0x0400068A RID: 1674
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x0400068B RID: 1675
		[ProtoMember(2)]
		public double Strength = 0.0;

		// Token: 0x0400068C RID: 1676
		[ProtoMember(3)]
		public double Intelligence = 0.0;

		// Token: 0x0400068D RID: 1677
		[ProtoMember(4)]
		public double Dexterity = 0.0;

		// Token: 0x0400068E RID: 1678
		[ProtoMember(5)]
		public double Constitution = 0.0;

		// Token: 0x0400068F RID: 1679
		[ProtoMember(6)]
		public double MinAttack = 0.0;

		// Token: 0x04000690 RID: 1680
		[ProtoMember(7)]
		public double MaxAttack = 0.0;

		// Token: 0x04000691 RID: 1681
		[ProtoMember(8)]
		public double MinDefense = 0.0;

		// Token: 0x04000692 RID: 1682
		[ProtoMember(9)]
		public double MaxDefense = 0.0;

		// Token: 0x04000693 RID: 1683
		[ProtoMember(10)]
		public double MagicSkillIncrease = 0.0;

		// Token: 0x04000694 RID: 1684
		[ProtoMember(11)]
		public double MinMAttack = 0.0;

		// Token: 0x04000695 RID: 1685
		[ProtoMember(12)]
		public double MaxMAttack = 0.0;

		// Token: 0x04000696 RID: 1686
		[ProtoMember(13)]
		public double MinMDefense = 0.0;

		// Token: 0x04000697 RID: 1687
		[ProtoMember(14)]
		public double MaxMDefense = 0.0;

		// Token: 0x04000698 RID: 1688
		[ProtoMember(15)]
		public double PhySkillIncrease = 0.0;

		// Token: 0x04000699 RID: 1689
		[ProtoMember(16)]
		public double MaxHP = 0.0;

		// Token: 0x0400069A RID: 1690
		[ProtoMember(17)]
		public double MaxMP = 0.0;

		// Token: 0x0400069B RID: 1691
		[ProtoMember(18)]
		public double AttackSpeed = 0.0;

		// Token: 0x0400069C RID: 1692
		[ProtoMember(19)]
		public double Hit = 0.0;

		// Token: 0x0400069D RID: 1693
		[ProtoMember(20)]
		public double Dodge = 0.0;

		// Token: 0x0400069E RID: 1694
		[ProtoMember(21)]
		public int TotalPropPoint = 0;

		// Token: 0x0400069F RID: 1695
		[ProtoMember(22)]
		public int ChangeLifeCount = 0;

		// Token: 0x040006A0 RID: 1696
		[ProtoMember(23)]
		public int CombatForce = 0;

		// Token: 0x040006A1 RID: 1697
		[ProtoMember(24)]
		public int TEMPStrength = 0;

		// Token: 0x040006A2 RID: 1698
		[ProtoMember(25)]
		public int TEMPIntelligsence = 0;

		// Token: 0x040006A3 RID: 1699
		[ProtoMember(26)]
		public int TEMPDexterity = 0;

		// Token: 0x040006A4 RID: 1700
		[ProtoMember(27)]
		public int TEMPConstitution = 0;

		// Token: 0x040006A5 RID: 1701
		[ProtoMember(28)]
		public double Toughness = 0.0;

		// Token: 0x040006A6 RID: 1702
		[ProtoMember(29)]
		public int ArmorMax;

		// Token: 0x040006A7 RID: 1703
		[ProtoMember(30)]
		public int RebornCombatForce = 0;
	}
}
