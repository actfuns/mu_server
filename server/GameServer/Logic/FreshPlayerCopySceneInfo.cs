using System;

namespace GameServer.Logic
{
	// Token: 0x020005F8 RID: 1528
	public class FreshPlayerCopySceneInfo
	{
		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06001D6D RID: 7533 RVA: 0x001AC0B0 File Offset: 0x001AA2B0
		// (set) Token: 0x06001D6E RID: 7534 RVA: 0x001AC0C7 File Offset: 0x001AA2C7
		public int MapCode { get; set; }

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06001D6F RID: 7535 RVA: 0x001AC0D0 File Offset: 0x001AA2D0
		// (set) Token: 0x06001D70 RID: 7536 RVA: 0x001AC0E7 File Offset: 0x001AA2E7
		public int NeedKillMonster1Level { get; set; }

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06001D71 RID: 7537 RVA: 0x001AC0F0 File Offset: 0x001AA2F0
		// (set) Token: 0x06001D72 RID: 7538 RVA: 0x001AC107 File Offset: 0x001AA307
		public int NeedKillMonster1Num { get; set; }

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06001D73 RID: 7539 RVA: 0x001AC110 File Offset: 0x001AA310
		// (set) Token: 0x06001D74 RID: 7540 RVA: 0x001AC127 File Offset: 0x001AA327
		public int NeedKillMonster2ID { get; set; }

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06001D75 RID: 7541 RVA: 0x001AC130 File Offset: 0x001AA330
		// (set) Token: 0x06001D76 RID: 7542 RVA: 0x001AC147 File Offset: 0x001AA347
		public int NeedKillMonster2Num { get; set; }

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06001D77 RID: 7543 RVA: 0x001AC150 File Offset: 0x001AA350
		// (set) Token: 0x06001D78 RID: 7544 RVA: 0x001AC167 File Offset: 0x001AA367
		public int NeedCreateMonster2Num { get; set; }

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06001D79 RID: 7545 RVA: 0x001AC170 File Offset: 0x001AA370
		// (set) Token: 0x06001D7A RID: 7546 RVA: 0x001AC187 File Offset: 0x001AA387
		public string NeedCreateMonster2Pos { get; set; }

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06001D7B RID: 7547 RVA: 0x001AC190 File Offset: 0x001AA390
		// (set) Token: 0x06001D7C RID: 7548 RVA: 0x001AC1A7 File Offset: 0x001AA3A7
		public int NeedCreateMonster2Radius { get; set; }

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06001D7D RID: 7549 RVA: 0x001AC1B0 File Offset: 0x001AA3B0
		// (set) Token: 0x06001D7E RID: 7550 RVA: 0x001AC1C7 File Offset: 0x001AA3C7
		public int NeedCreateMonster2PursuitRadius { get; set; }

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06001D7F RID: 7551 RVA: 0x001AC1D0 File Offset: 0x001AA3D0
		// (set) Token: 0x06001D80 RID: 7552 RVA: 0x001AC1E7 File Offset: 0x001AA3E7
		public int GateID { get; set; }

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x06001D81 RID: 7553 RVA: 0x001AC1F0 File Offset: 0x001AA3F0
		// (set) Token: 0x06001D82 RID: 7554 RVA: 0x001AC207 File Offset: 0x001AA407
		public string GatePos { get; set; }

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06001D83 RID: 7555 RVA: 0x001AC210 File Offset: 0x001AA410
		// (set) Token: 0x06001D84 RID: 7556 RVA: 0x001AC227 File Offset: 0x001AA427
		public int CrystalID { get; set; }

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x06001D85 RID: 7557 RVA: 0x001AC230 File Offset: 0x001AA430
		// (set) Token: 0x06001D86 RID: 7558 RVA: 0x001AC247 File Offset: 0x001AA447
		public string CrystalPos { get; set; }

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06001D87 RID: 7559 RVA: 0x001AC250 File Offset: 0x001AA450
		// (set) Token: 0x06001D88 RID: 7560 RVA: 0x001AC267 File Offset: 0x001AA467
		public int DiaoXiangID { get; set; }

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x06001D89 RID: 7561 RVA: 0x001AC270 File Offset: 0x001AA470
		// (set) Token: 0x06001D8A RID: 7562 RVA: 0x001AC287 File Offset: 0x001AA487
		public string DiaoXiangPos { get; set; }
	}
}
