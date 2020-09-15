using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000602 RID: 1538
	public class ExperienceCopyMapDataInfo
	{
		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06001E1F RID: 7711 RVA: 0x001ACB80 File Offset: 0x001AAD80
		// (set) Token: 0x06001E20 RID: 7712 RVA: 0x001ACB97 File Offset: 0x001AAD97
		public int CopyMapID { get; set; }

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06001E21 RID: 7713 RVA: 0x001ACBA0 File Offset: 0x001AADA0
		// (set) Token: 0x06001E22 RID: 7714 RVA: 0x001ACBB7 File Offset: 0x001AADB7
		public int MapCodeID { get; set; }

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06001E23 RID: 7715 RVA: 0x001ACBC0 File Offset: 0x001AADC0
		// (set) Token: 0x06001E24 RID: 7716 RVA: 0x001ACBD7 File Offset: 0x001AADD7
		public Dictionary<int, List<int>> MonsterIDList { get; set; }

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06001E25 RID: 7717 RVA: 0x001ACBE0 File Offset: 0x001AADE0
		// (set) Token: 0x06001E26 RID: 7718 RVA: 0x001ACBF7 File Offset: 0x001AADF7
		public Dictionary<int, List<int>> MonsterNumList { get; set; }

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06001E27 RID: 7719 RVA: 0x001ACC00 File Offset: 0x001AAE00
		// (set) Token: 0x06001E28 RID: 7720 RVA: 0x001ACC17 File Offset: 0x001AAE17
		public int posX { get; set; }

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06001E29 RID: 7721 RVA: 0x001ACC20 File Offset: 0x001AAE20
		// (set) Token: 0x06001E2A RID: 7722 RVA: 0x001ACC37 File Offset: 0x001AAE37
		public int posZ { get; set; }

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06001E2B RID: 7723 RVA: 0x001ACC40 File Offset: 0x001AAE40
		// (set) Token: 0x06001E2C RID: 7724 RVA: 0x001ACC57 File Offset: 0x001AAE57
		public int Radius { get; set; }

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06001E2D RID: 7725 RVA: 0x001ACC60 File Offset: 0x001AAE60
		// (set) Token: 0x06001E2E RID: 7726 RVA: 0x001ACC77 File Offset: 0x001AAE77
		public int MonsterSum { get; set; }

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06001E2F RID: 7727 RVA: 0x001ACC80 File Offset: 0x001AAE80
		// (set) Token: 0x06001E30 RID: 7728 RVA: 0x001ACC97 File Offset: 0x001AAE97
		public int[] CreateNextWaveMonsterCondition { get; set; }
	}
}
