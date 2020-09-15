using System;

namespace GameServer.Logic
{
	// Token: 0x0200053B RID: 1339
	public class ChangeLifePropertyInfo
	{
		// Token: 0x06001984 RID: 6532 RVA: 0x0018D5BC File Offset: 0x0018B7BC
		public void AddFrom(ChangeLifePropertyInfo info)
		{
			this.AddPhyAttackMinValue += info.AddPhyAttackMinValue;
			this.AddPhyAttackMaxValue += info.AddPhyAttackMaxValue;
			this.AddMagAttackMinValue += info.AddMagAttackMinValue;
			this.AddMagAttackMaxValue += info.AddMagAttackMaxValue;
			this.AddPhyDefenseMinValue += info.AddPhyDefenseMinValue;
			this.AddPhyDefenseMaxValue += info.AddPhyDefenseMaxValue;
			this.AddMagDefenseMinValue += info.AddMagDefenseMinValue;
			this.AddMagDefenseMaxValue += info.AddMagDefenseMaxValue;
			this.AddHitPropValue += info.AddHitPropValue;
			this.AddDodgePropValue += info.AddDodgePropValue;
			this.AddMaxLifePropValue += info.AddMaxLifePropValue;
		}

		// Token: 0x040023A2 RID: 9122
		public int PhyAttackMin = 0;

		// Token: 0x040023A3 RID: 9123
		public int PhyAttackMax = 0;

		// Token: 0x040023A4 RID: 9124
		public int MagAttackMin = 0;

		// Token: 0x040023A5 RID: 9125
		public int MagAttackMax = 0;

		// Token: 0x040023A6 RID: 9126
		public int PhyDefenseMin = 0;

		// Token: 0x040023A7 RID: 9127
		public int PhyDefenseMax = 0;

		// Token: 0x040023A8 RID: 9128
		public int MagDefenseMin = 0;

		// Token: 0x040023A9 RID: 9129
		public int MagDefenseMax = 0;

		// Token: 0x040023AA RID: 9130
		public int HitProp = 0;

		// Token: 0x040023AB RID: 9131
		public int DodgeProp = 0;

		// Token: 0x040023AC RID: 9132
		public int MaxLifeProp = 0;

		// Token: 0x040023AD RID: 9133
		public int AddPhyAttackMinValue = 0;

		// Token: 0x040023AE RID: 9134
		public int AddPhyAttackMaxValue = 0;

		// Token: 0x040023AF RID: 9135
		public int AddMagAttackMinValue = 0;

		// Token: 0x040023B0 RID: 9136
		public int AddMagAttackMaxValue = 0;

		// Token: 0x040023B1 RID: 9137
		public int AddPhyDefenseMinValue = 0;

		// Token: 0x040023B2 RID: 9138
		public int AddPhyDefenseMaxValue = 0;

		// Token: 0x040023B3 RID: 9139
		public int AddMagDefenseMinValue = 0;

		// Token: 0x040023B4 RID: 9140
		public int AddMagDefenseMaxValue = 0;

		// Token: 0x040023B5 RID: 9141
		public int AddHitPropValue = 0;

		// Token: 0x040023B6 RID: 9142
		public int AddDodgePropValue = 0;

		// Token: 0x040023B7 RID: 9143
		public int AddMaxLifePropValue = 0;
	}
}
