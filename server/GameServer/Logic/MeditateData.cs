using System;

namespace GameServer.Logic
{
	// Token: 0x02000601 RID: 1537
	public class MeditateData
	{
		// Token: 0x17000189 RID: 393
		// (get) Token: 0x06001E10 RID: 7696 RVA: 0x001ACA98 File Offset: 0x001AAC98
		// (set) Token: 0x06001E11 RID: 7697 RVA: 0x001ACAAF File Offset: 0x001AACAF
		public int MeditateID { get; set; }

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06001E12 RID: 7698 RVA: 0x001ACAB8 File Offset: 0x001AACB8
		// (set) Token: 0x06001E13 RID: 7699 RVA: 0x001ACACF File Offset: 0x001AACCF
		public int MinZhuanSheng { get; set; }

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x06001E14 RID: 7700 RVA: 0x001ACAD8 File Offset: 0x001AACD8
		// (set) Token: 0x06001E15 RID: 7701 RVA: 0x001ACAEF File Offset: 0x001AACEF
		public int MinLevel { get; set; }

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06001E16 RID: 7702 RVA: 0x001ACAF8 File Offset: 0x001AACF8
		// (set) Token: 0x06001E17 RID: 7703 RVA: 0x001ACB0F File Offset: 0x001AAD0F
		public int MaxZhuanSheng { get; set; }

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06001E18 RID: 7704 RVA: 0x001ACB18 File Offset: 0x001AAD18
		// (set) Token: 0x06001E19 RID: 7705 RVA: 0x001ACB2F File Offset: 0x001AAD2F
		public int MaxLevel { get; set; }

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06001E1A RID: 7706 RVA: 0x001ACB38 File Offset: 0x001AAD38
		// (set) Token: 0x06001E1B RID: 7707 RVA: 0x001ACB4F File Offset: 0x001AAD4F
		public int Experience { get; set; }

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06001E1C RID: 7708 RVA: 0x001ACB58 File Offset: 0x001AAD58
		// (set) Token: 0x06001E1D RID: 7709 RVA: 0x001ACB6F File Offset: 0x001AAD6F
		public int StarSoul { get; set; }

		// Token: 0x04002B5D RID: 11101
		public int[] MediateRewardTuple;

		// Token: 0x04002B5E RID: 11102
		public int GetRewardTime;
	}
}
