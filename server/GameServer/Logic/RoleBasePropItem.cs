using System;

namespace GameServer.Logic
{
	// Token: 0x020005E7 RID: 1511
	public class RoleBasePropItem
	{
		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06001C87 RID: 7303 RVA: 0x001AB2C8 File Offset: 0x001A94C8
		// (set) Token: 0x06001C88 RID: 7304 RVA: 0x001AB2DF File Offset: 0x001A94DF
		public double LifeV { get; set; }

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06001C89 RID: 7305 RVA: 0x001AB2E8 File Offset: 0x001A94E8
		// (set) Token: 0x06001C8A RID: 7306 RVA: 0x001AB2FF File Offset: 0x001A94FF
		public double MagicV { get; set; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06001C8B RID: 7307 RVA: 0x001AB308 File Offset: 0x001A9508
		// (set) Token: 0x06001C8C RID: 7308 RVA: 0x001AB31F File Offset: 0x001A951F
		public double MinDefenseV { get; set; }

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06001C8D RID: 7309 RVA: 0x001AB328 File Offset: 0x001A9528
		// (set) Token: 0x06001C8E RID: 7310 RVA: 0x001AB33F File Offset: 0x001A953F
		public double MaxDefenseV { get; set; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06001C8F RID: 7311 RVA: 0x001AB348 File Offset: 0x001A9548
		// (set) Token: 0x06001C90 RID: 7312 RVA: 0x001AB35F File Offset: 0x001A955F
		public double MinMDefenseV { get; set; }

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06001C91 RID: 7313 RVA: 0x001AB368 File Offset: 0x001A9568
		// (set) Token: 0x06001C92 RID: 7314 RVA: 0x001AB37F File Offset: 0x001A957F
		public double MaxMDefenseV { get; set; }

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06001C93 RID: 7315 RVA: 0x001AB388 File Offset: 0x001A9588
		// (set) Token: 0x06001C94 RID: 7316 RVA: 0x001AB39F File Offset: 0x001A959F
		public double MinAttackV { get; set; }

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06001C95 RID: 7317 RVA: 0x001AB3A8 File Offset: 0x001A95A8
		// (set) Token: 0x06001C96 RID: 7318 RVA: 0x001AB3BF File Offset: 0x001A95BF
		public double MaxAttackV { get; set; }

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06001C97 RID: 7319 RVA: 0x001AB3C8 File Offset: 0x001A95C8
		// (set) Token: 0x06001C98 RID: 7320 RVA: 0x001AB3DF File Offset: 0x001A95DF
		public double MinMAttackV { get; set; }

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06001C99 RID: 7321 RVA: 0x001AB3E8 File Offset: 0x001A95E8
		// (set) Token: 0x06001C9A RID: 7322 RVA: 0x001AB3FF File Offset: 0x001A95FF
		public double MaxMAttackV { get; set; }

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06001C9B RID: 7323 RVA: 0x001AB408 File Offset: 0x001A9608
		// (set) Token: 0x06001C9C RID: 7324 RVA: 0x001AB41F File Offset: 0x001A961F
		public double RecoverLifeV { get; set; }

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06001C9D RID: 7325 RVA: 0x001AB428 File Offset: 0x001A9628
		// (set) Token: 0x06001C9E RID: 7326 RVA: 0x001AB43F File Offset: 0x001A963F
		public double RecoverMagicV { get; set; }

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06001C9F RID: 7327 RVA: 0x001AB448 File Offset: 0x001A9648
		// (set) Token: 0x06001CA0 RID: 7328 RVA: 0x001AB45F File Offset: 0x001A965F
		public double Dodge { get; set; }

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06001CA1 RID: 7329 RVA: 0x001AB468 File Offset: 0x001A9668
		// (set) Token: 0x06001CA2 RID: 7330 RVA: 0x001AB47F File Offset: 0x001A967F
		public double HitV { get; set; }

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06001CA3 RID: 7331 RVA: 0x001AB488 File Offset: 0x001A9688
		// (set) Token: 0x06001CA4 RID: 7332 RVA: 0x001AB49F File Offset: 0x001A969F
		public double PhySkillIncreasePercent { get; set; }

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06001CA5 RID: 7333 RVA: 0x001AB4A8 File Offset: 0x001A96A8
		// (set) Token: 0x06001CA6 RID: 7334 RVA: 0x001AB4BF File Offset: 0x001A96BF
		public double MagicSkillIncreasePercent { get; set; }

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06001CA7 RID: 7335 RVA: 0x001AB4C8 File Offset: 0x001A96C8
		// (set) Token: 0x06001CA8 RID: 7336 RVA: 0x001AB4DF File Offset: 0x001A96DF
		public double AttackSpeed { get; set; }

		// Token: 0x04002A65 RID: 10853
		public double[] arrRoleExtProp = null;
	}
}
