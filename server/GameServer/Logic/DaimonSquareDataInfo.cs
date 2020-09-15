using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020005FD RID: 1533
	public class DaimonSquareDataInfo
	{
		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06001DB2 RID: 7602 RVA: 0x001AC4D8 File Offset: 0x001AA6D8
		// (set) Token: 0x06001DB3 RID: 7603 RVA: 0x001AC4EF File Offset: 0x001AA6EF
		public int MapCode { get; set; }

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06001DB4 RID: 7604 RVA: 0x001AC4F8 File Offset: 0x001AA6F8
		// (set) Token: 0x06001DB5 RID: 7605 RVA: 0x001AC50F File Offset: 0x001AA70F
		public int MinChangeLifeNum { get; set; }

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06001DB6 RID: 7606 RVA: 0x001AC518 File Offset: 0x001AA718
		// (set) Token: 0x06001DB7 RID: 7607 RVA: 0x001AC52F File Offset: 0x001AA72F
		public int MaxChangeLifeNum { get; set; }

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06001DB8 RID: 7608 RVA: 0x001AC538 File Offset: 0x001AA738
		// (set) Token: 0x06001DB9 RID: 7609 RVA: 0x001AC54F File Offset: 0x001AA74F
		public int MinLevel { get; set; }

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06001DBA RID: 7610 RVA: 0x001AC558 File Offset: 0x001AA758
		// (set) Token: 0x06001DBB RID: 7611 RVA: 0x001AC56F File Offset: 0x001AA76F
		public int MaxLevel { get; set; }

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06001DBC RID: 7612 RVA: 0x001AC578 File Offset: 0x001AA778
		// (set) Token: 0x06001DBD RID: 7613 RVA: 0x001AC58F File Offset: 0x001AA78F
		public int MaxEnterNum { get; set; }

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06001DBE RID: 7614 RVA: 0x001AC598 File Offset: 0x001AA798
		// (set) Token: 0x06001DBF RID: 7615 RVA: 0x001AC5AF File Offset: 0x001AA7AF
		public int NeedGoodsID { get; set; }

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06001DC0 RID: 7616 RVA: 0x001AC5B8 File Offset: 0x001AA7B8
		// (set) Token: 0x06001DC1 RID: 7617 RVA: 0x001AC5CF File Offset: 0x001AA7CF
		public int NeedGoodsNum { get; set; }

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06001DC2 RID: 7618 RVA: 0x001AC5D8 File Offset: 0x001AA7D8
		// (set) Token: 0x06001DC3 RID: 7619 RVA: 0x001AC5EF File Offset: 0x001AA7EF
		public int MaxPlayerNum { get; set; }

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06001DC4 RID: 7620 RVA: 0x001AC5F8 File Offset: 0x001AA7F8
		// (set) Token: 0x06001DC5 RID: 7621 RVA: 0x001AC60F File Offset: 0x001AA80F
		public string[] MonsterID { get; set; }

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06001DC6 RID: 7622 RVA: 0x001AC618 File Offset: 0x001AA818
		// (set) Token: 0x06001DC7 RID: 7623 RVA: 0x001AC62F File Offset: 0x001AA82F
		public string[] MonsterNum { get; set; }

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06001DC8 RID: 7624 RVA: 0x001AC638 File Offset: 0x001AA838
		// (set) Token: 0x06001DC9 RID: 7625 RVA: 0x001AC64F File Offset: 0x001AA84F
		public int posX { get; set; }

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06001DCA RID: 7626 RVA: 0x001AC658 File Offset: 0x001AA858
		// (set) Token: 0x06001DCB RID: 7627 RVA: 0x001AC66F File Offset: 0x001AA86F
		public int posZ { get; set; }

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06001DCC RID: 7628 RVA: 0x001AC678 File Offset: 0x001AA878
		// (set) Token: 0x06001DCD RID: 7629 RVA: 0x001AC68F File Offset: 0x001AA88F
		public int Radius { get; set; }

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06001DCE RID: 7630 RVA: 0x001AC698 File Offset: 0x001AA898
		// (set) Token: 0x06001DCF RID: 7631 RVA: 0x001AC6AF File Offset: 0x001AA8AF
		public int MonsterSum { get; set; }

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06001DD0 RID: 7632 RVA: 0x001AC6B8 File Offset: 0x001AA8B8
		// (set) Token: 0x06001DD1 RID: 7633 RVA: 0x001AC6CF File Offset: 0x001AA8CF
		public string[] CreateNextWaveMonsterCondition { get; set; }

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06001DD2 RID: 7634 RVA: 0x001AC6D8 File Offset: 0x001AA8D8
		// (set) Token: 0x06001DD3 RID: 7635 RVA: 0x001AC6EF File Offset: 0x001AA8EF
		public int TimeModulus { get; set; }

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06001DD4 RID: 7636 RVA: 0x001AC6F8 File Offset: 0x001AA8F8
		// (set) Token: 0x06001DD5 RID: 7637 RVA: 0x001AC70F File Offset: 0x001AA90F
		public int ExpModulus { get; set; }

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06001DD6 RID: 7638 RVA: 0x001AC718 File Offset: 0x001AA918
		// (set) Token: 0x06001DD7 RID: 7639 RVA: 0x001AC72F File Offset: 0x001AA92F
		public int MoneyModulus { get; set; }

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x06001DD8 RID: 7640 RVA: 0x001AC738 File Offset: 0x001AA938
		// (set) Token: 0x06001DD9 RID: 7641 RVA: 0x001AC74F File Offset: 0x001AA94F
		public string[] AwardItem { get; set; }

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06001DDA RID: 7642 RVA: 0x001AC758 File Offset: 0x001AA958
		// (set) Token: 0x06001DDB RID: 7643 RVA: 0x001AC76F File Offset: 0x001AA96F
		public List<string> BeginTime { get; set; }

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06001DDC RID: 7644 RVA: 0x001AC778 File Offset: 0x001AA978
		// (set) Token: 0x06001DDD RID: 7645 RVA: 0x001AC78F File Offset: 0x001AA98F
		public int PrepareTime { get; set; }

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06001DDE RID: 7646 RVA: 0x001AC798 File Offset: 0x001AA998
		// (set) Token: 0x06001DDF RID: 7647 RVA: 0x001AC7AF File Offset: 0x001AA9AF
		public int DurationTime { get; set; }

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06001DE0 RID: 7648 RVA: 0x001AC7B8 File Offset: 0x001AA9B8
		// (set) Token: 0x06001DE1 RID: 7649 RVA: 0x001AC7CF File Offset: 0x001AA9CF
		public int LeaveTime { get; set; }
	}
}
