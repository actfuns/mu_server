using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020005F5 RID: 1525
	public class BloodCastleDataInfo
	{
		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06001D1C RID: 7452 RVA: 0x001ABBB8 File Offset: 0x001A9DB8
		// (set) Token: 0x06001D1D RID: 7453 RVA: 0x001ABBCF File Offset: 0x001A9DCF
		public int MapCode { get; set; }

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06001D1E RID: 7454 RVA: 0x001ABBD8 File Offset: 0x001A9DD8
		// (set) Token: 0x06001D1F RID: 7455 RVA: 0x001ABBEF File Offset: 0x001A9DEF
		public int MinChangeLifeNum { get; set; }

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06001D20 RID: 7456 RVA: 0x001ABBF8 File Offset: 0x001A9DF8
		// (set) Token: 0x06001D21 RID: 7457 RVA: 0x001ABC0F File Offset: 0x001A9E0F
		public int MaxChangeLifeNum { get; set; }

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06001D22 RID: 7458 RVA: 0x001ABC18 File Offset: 0x001A9E18
		// (set) Token: 0x06001D23 RID: 7459 RVA: 0x001ABC2F File Offset: 0x001A9E2F
		public int MinLevel { get; set; }

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06001D24 RID: 7460 RVA: 0x001ABC38 File Offset: 0x001A9E38
		// (set) Token: 0x06001D25 RID: 7461 RVA: 0x001ABC4F File Offset: 0x001A9E4F
		public int MaxLevel { get; set; }

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06001D26 RID: 7462 RVA: 0x001ABC58 File Offset: 0x001A9E58
		// (set) Token: 0x06001D27 RID: 7463 RVA: 0x001ABC6F File Offset: 0x001A9E6F
		public int MaxEnterNum { get; set; }

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06001D28 RID: 7464 RVA: 0x001ABC78 File Offset: 0x001A9E78
		// (set) Token: 0x06001D29 RID: 7465 RVA: 0x001ABC8F File Offset: 0x001A9E8F
		public int NeedGoodsID { get; set; }

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06001D2A RID: 7466 RVA: 0x001ABC98 File Offset: 0x001A9E98
		// (set) Token: 0x06001D2B RID: 7467 RVA: 0x001ABCAF File Offset: 0x001A9EAF
		public int NeedGoodsNum { get; set; }

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06001D2C RID: 7468 RVA: 0x001ABCB8 File Offset: 0x001A9EB8
		// (set) Token: 0x06001D2D RID: 7469 RVA: 0x001ABCCF File Offset: 0x001A9ECF
		public int MaxPlayerNum { get; set; }

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06001D2E RID: 7470 RVA: 0x001ABCD8 File Offset: 0x001A9ED8
		// (set) Token: 0x06001D2F RID: 7471 RVA: 0x001ABCEF File Offset: 0x001A9EEF
		public int NeedKillMonster1Level { get; set; }

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06001D30 RID: 7472 RVA: 0x001ABCF8 File Offset: 0x001A9EF8
		// (set) Token: 0x06001D31 RID: 7473 RVA: 0x001ABD0F File Offset: 0x001A9F0F
		public int NeedKillMonster1Num { get; set; }

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06001D32 RID: 7474 RVA: 0x001ABD18 File Offset: 0x001A9F18
		// (set) Token: 0x06001D33 RID: 7475 RVA: 0x001ABD2F File Offset: 0x001A9F2F
		public int NeedKillMonster2ID { get; set; }

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06001D34 RID: 7476 RVA: 0x001ABD38 File Offset: 0x001A9F38
		// (set) Token: 0x06001D35 RID: 7477 RVA: 0x001ABD4F File Offset: 0x001A9F4F
		public int NeedKillMonster2Num { get; set; }

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06001D36 RID: 7478 RVA: 0x001ABD58 File Offset: 0x001A9F58
		// (set) Token: 0x06001D37 RID: 7479 RVA: 0x001ABD6F File Offset: 0x001A9F6F
		public int NeedCreateMonster2Num { get; set; }

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06001D38 RID: 7480 RVA: 0x001ABD78 File Offset: 0x001A9F78
		// (set) Token: 0x06001D39 RID: 7481 RVA: 0x001ABD8F File Offset: 0x001A9F8F
		public string NeedCreateMonster2Pos { get; set; }

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06001D3A RID: 7482 RVA: 0x001ABD98 File Offset: 0x001A9F98
		// (set) Token: 0x06001D3B RID: 7483 RVA: 0x001ABDAF File Offset: 0x001A9FAF
		public int NeedCreateMonster2Radius { get; set; }

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06001D3C RID: 7484 RVA: 0x001ABDB8 File Offset: 0x001A9FB8
		// (set) Token: 0x06001D3D RID: 7485 RVA: 0x001ABDCF File Offset: 0x001A9FCF
		public int NeedCreateMonster2PursuitRadius { get; set; }

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06001D3E RID: 7486 RVA: 0x001ABDD8 File Offset: 0x001A9FD8
		// (set) Token: 0x06001D3F RID: 7487 RVA: 0x001ABDEF File Offset: 0x001A9FEF
		public int GateID { get; set; }

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06001D40 RID: 7488 RVA: 0x001ABDF8 File Offset: 0x001A9FF8
		// (set) Token: 0x06001D41 RID: 7489 RVA: 0x001ABE0F File Offset: 0x001AA00F
		public string GatePos { get; set; }

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06001D42 RID: 7490 RVA: 0x001ABE18 File Offset: 0x001AA018
		// (set) Token: 0x06001D43 RID: 7491 RVA: 0x001ABE2F File Offset: 0x001AA02F
		public int CrystalID { get; set; }

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06001D44 RID: 7492 RVA: 0x001ABE38 File Offset: 0x001AA038
		// (set) Token: 0x06001D45 RID: 7493 RVA: 0x001ABE4F File Offset: 0x001AA04F
		public string CrystalPos { get; set; }

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x06001D46 RID: 7494 RVA: 0x001ABE58 File Offset: 0x001AA058
		// (set) Token: 0x06001D47 RID: 7495 RVA: 0x001ABE6F File Offset: 0x001AA06F
		public int TimeModulus { get; set; }

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06001D48 RID: 7496 RVA: 0x001ABE78 File Offset: 0x001AA078
		// (set) Token: 0x06001D49 RID: 7497 RVA: 0x001ABE8F File Offset: 0x001AA08F
		public int ExpModulus { get; set; }

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x06001D4A RID: 7498 RVA: 0x001ABE98 File Offset: 0x001AA098
		// (set) Token: 0x06001D4B RID: 7499 RVA: 0x001ABEAF File Offset: 0x001AA0AF
		public int MoneyModulus { get; set; }

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06001D4C RID: 7500 RVA: 0x001ABEB8 File Offset: 0x001AA0B8
		// (set) Token: 0x06001D4D RID: 7501 RVA: 0x001ABECF File Offset: 0x001AA0CF
		public string[] AwardItem1 { get; set; }

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06001D4E RID: 7502 RVA: 0x001ABED8 File Offset: 0x001AA0D8
		// (set) Token: 0x06001D4F RID: 7503 RVA: 0x001ABEEF File Offset: 0x001AA0EF
		public string[] AwardItem2 { get; set; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06001D50 RID: 7504 RVA: 0x001ABEF8 File Offset: 0x001AA0F8
		// (set) Token: 0x06001D51 RID: 7505 RVA: 0x001ABF0F File Offset: 0x001AA10F
		public List<string> BeginTime { get; set; }

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06001D52 RID: 7506 RVA: 0x001ABF18 File Offset: 0x001AA118
		// (set) Token: 0x06001D53 RID: 7507 RVA: 0x001ABF2F File Offset: 0x001AA12F
		public int PrepareTime { get; set; }

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06001D54 RID: 7508 RVA: 0x001ABF38 File Offset: 0x001AA138
		// (set) Token: 0x06001D55 RID: 7509 RVA: 0x001ABF4F File Offset: 0x001AA14F
		public int DurationTime { get; set; }

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06001D56 RID: 7510 RVA: 0x001ABF58 File Offset: 0x001AA158
		// (set) Token: 0x06001D57 RID: 7511 RVA: 0x001ABF6F File Offset: 0x001AA16F
		public int LeaveTime { get; set; }

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x06001D58 RID: 7512 RVA: 0x001ABF78 File Offset: 0x001AA178
		// (set) Token: 0x06001D59 RID: 7513 RVA: 0x001ABF8F File Offset: 0x001AA18F
		public int DiaoXiangID { get; set; }

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06001D5A RID: 7514 RVA: 0x001ABF98 File Offset: 0x001AA198
		// (set) Token: 0x06001D5B RID: 7515 RVA: 0x001ABFAF File Offset: 0x001AA1AF
		public string DiaoXiangPos { get; set; }
	}
}
