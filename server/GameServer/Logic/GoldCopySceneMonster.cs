using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020006D9 RID: 1753
	public class GoldCopySceneMonster
	{
		// Token: 0x1700026F RID: 623
		// (get) Token: 0x060029CD RID: 10701 RVA: 0x00258EBC File Offset: 0x002570BC
		// (set) Token: 0x060029CE RID: 10702 RVA: 0x00258ED3 File Offset: 0x002570D3
		public int m_Wave { get; set; }

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x060029CF RID: 10703 RVA: 0x00258EDC File Offset: 0x002570DC
		// (set) Token: 0x060029D0 RID: 10704 RVA: 0x00258EF3 File Offset: 0x002570F3
		public int m_Num { get; set; }

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x060029D1 RID: 10705 RVA: 0x00258EFC File Offset: 0x002570FC
		// (set) Token: 0x060029D2 RID: 10706 RVA: 0x00258F13 File Offset: 0x00257113
		public bool m_bIsFirstWave { get; set; }

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x060029D3 RID: 10707 RVA: 0x00258F1C File Offset: 0x0025711C
		// (set) Token: 0x060029D4 RID: 10708 RVA: 0x00258F33 File Offset: 0x00257133
		public List<int> m_MonsterID { get; set; }

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x060029D5 RID: 10709 RVA: 0x00258F3C File Offset: 0x0025713C
		// (set) Token: 0x060029D6 RID: 10710 RVA: 0x00258F53 File Offset: 0x00257153
		public int m_Delay1 { get; set; }

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x060029D7 RID: 10711 RVA: 0x00258F5C File Offset: 0x0025715C
		// (set) Token: 0x060029D8 RID: 10712 RVA: 0x00258F73 File Offset: 0x00257173
		public int m_Delay2 { get; set; }
	}
}
