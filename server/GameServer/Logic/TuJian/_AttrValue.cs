using System;

namespace GameServer.Logic.TuJian
{
	// Token: 0x02000494 RID: 1172
	internal class _AttrValue
	{
		// Token: 0x0600156B RID: 5483 RVA: 0x00150AC8 File Offset: 0x0014ECC8
		public _AttrValue Add(_AttrValue other)
		{
			if (other != null)
			{
				this.MinDefense += other.MinDefense;
				this.MaxDefense += other.MaxDefense;
				this.MinMDefense += other.MinMDefense;
				this.MaxMDefense += other.MaxMDefense;
				this.MinAttack += other.MinAttack;
				this.MaxAttack += other.MaxAttack;
				this.MinMAttack += other.MinMAttack;
				this.MaxMAttack += other.MaxMAttack;
				this.HitV += other.HitV;
				this.Dodge += other.Dodge;
				this.MaxLifeV += other.MaxLifeV;
			}
			return this;
		}

		// Token: 0x04001F0A RID: 7946
		public int MinDefense = 0;

		// Token: 0x04001F0B RID: 7947
		public int MaxDefense = 0;

		// Token: 0x04001F0C RID: 7948
		public int MinMDefense = 0;

		// Token: 0x04001F0D RID: 7949
		public int MaxMDefense = 0;

		// Token: 0x04001F0E RID: 7950
		public int MinAttack = 0;

		// Token: 0x04001F0F RID: 7951
		public int MaxAttack = 0;

		// Token: 0x04001F10 RID: 7952
		public int MinMAttack = 0;

		// Token: 0x04001F11 RID: 7953
		public int MaxMAttack = 0;

		// Token: 0x04001F12 RID: 7954
		public int HitV = 0;

		// Token: 0x04001F13 RID: 7955
		public int Dodge = 0;

		// Token: 0x04001F14 RID: 7956
		public int MaxLifeV = 0;
	}
}
