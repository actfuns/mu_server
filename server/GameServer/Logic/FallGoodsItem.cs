using System;

namespace GameServer.Logic
{
	// Token: 0x020006DF RID: 1759
	public class FallGoodsItem
	{
		// Token: 0x17000275 RID: 629
		// (get) Token: 0x060029F2 RID: 10738 RVA: 0x0025A684 File Offset: 0x00258884
		// (set) Token: 0x060029F3 RID: 10739 RVA: 0x0025A69B File Offset: 0x0025889B
		public int GoodsID { get; set; }

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x060029F4 RID: 10740 RVA: 0x0025A6A4 File Offset: 0x002588A4
		// (set) Token: 0x060029F5 RID: 10741 RVA: 0x0025A6BB File Offset: 0x002588BB
		public int BasePercent { get; set; }

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x060029F6 RID: 10742 RVA: 0x0025A6C4 File Offset: 0x002588C4
		// (set) Token: 0x060029F7 RID: 10743 RVA: 0x0025A6DB File Offset: 0x002588DB
		public int SelfPercent { get; set; }

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x060029F8 RID: 10744 RVA: 0x0025A6E4 File Offset: 0x002588E4
		// (set) Token: 0x060029F9 RID: 10745 RVA: 0x0025A6FB File Offset: 0x002588FB
		public int Binding { get; set; }

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x060029FA RID: 10746 RVA: 0x0025A704 File Offset: 0x00258904
		// (set) Token: 0x060029FB RID: 10747 RVA: 0x0025A71B File Offset: 0x0025891B
		public int FallLevelID { get; set; }

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x060029FC RID: 10748 RVA: 0x0025A724 File Offset: 0x00258924
		// (set) Token: 0x060029FD RID: 10749 RVA: 0x0025A73B File Offset: 0x0025893B
		public bool IsGood { get; set; }

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x060029FE RID: 10750 RVA: 0x0025A744 File Offset: 0x00258944
		// (set) Token: 0x060029FF RID: 10751 RVA: 0x0025A75B File Offset: 0x0025895B
		public int LuckyRate { get; set; }

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06002A00 RID: 10752 RVA: 0x0025A764 File Offset: 0x00258964
		// (set) Token: 0x06002A01 RID: 10753 RVA: 0x0025A77B File Offset: 0x0025897B
		public int ZhuiJiaID { get; set; }

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06002A02 RID: 10754 RVA: 0x0025A784 File Offset: 0x00258984
		// (set) Token: 0x06002A03 RID: 10755 RVA: 0x0025A79B File Offset: 0x0025899B
		public int ExcellencePropertyID { get; set; }
	}
}
