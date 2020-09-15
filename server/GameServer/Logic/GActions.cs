using System;

namespace GameServer.Logic
{
	// Token: 0x02000629 RID: 1577
	public enum GActions
	{
		// Token: 0x04002CE1 RID: 11489
		Stand,
		// Token: 0x04002CE2 RID: 11490
		Walk,
		// Token: 0x04002CE3 RID: 11491
		Run,
		// Token: 0x04002CE4 RID: 11492
		Attack,
		// Token: 0x04002CE5 RID: 11493
		Injured,
		// Token: 0x04002CE6 RID: 11494
		Magic = 6,
		// Token: 0x04002CE7 RID: 11495
		Bow = 9,
		// Token: 0x04002CE8 RID: 11496
		Death = 12,
		// Token: 0x04002CE9 RID: 11497
		HorseStand = 14,
		// Token: 0x04002CEA RID: 11498
		HorseRun = 16,
		// Token: 0x04002CEB RID: 11499
		HorseDead = 20,
		// Token: 0x04002CEC RID: 11500
		Sit = 23,
		// Token: 0x04002CED RID: 11501
		PreAttack,
		// Token: 0x04002CEE RID: 11502
		IdleStand,
		// Token: 0x04002CEF RID: 11503
		Italic,
		// Token: 0x04002CF0 RID: 11504
		Collect,
		// Token: 0x04002CF1 RID: 11505
		Wenhao,
		// Token: 0x04002CF2 RID: 11506
		Genwolai,
		// Token: 0x04002CF3 RID: 11507
		Guzhang,
		// Token: 0x04002CF4 RID: 11508
		Huanhu,
		// Token: 0x04002CF5 RID: 11509
		Jushang,
		// Token: 0x04002CF6 RID: 11510
		Xingli,
		// Token: 0x04002CF7 RID: 11511
		Chongfeng,
		// Token: 0x04002CF8 RID: 11512
		Mobai,
		// Token: 0x04002CF9 RID: 11513
		Tiaoxin,
		// Token: 0x04002CFA RID: 11514
		Zuoxia,
		// Token: 0x04002CFB RID: 11515
		Shuijiao,
		// Token: 0x04002CFC RID: 11516
		MaxAction
	}
}
