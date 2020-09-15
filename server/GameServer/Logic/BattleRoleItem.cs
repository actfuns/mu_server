using System;

namespace GameServer.Logic
{
	// Token: 0x020006E7 RID: 1767
	public class BattleRoleItem
	{
		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06002A56 RID: 10838 RVA: 0x0025ADE4 File Offset: 0x00258FE4
		// (set) Token: 0x06002A57 RID: 10839 RVA: 0x0025ADFB File Offset: 0x00258FFB
		public GameClient Client { get; set; }

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x06002A58 RID: 10840 RVA: 0x0025AE04 File Offset: 0x00259004
		// (set) Token: 0x06002A59 RID: 10841 RVA: 0x0025AE1B File Offset: 0x0025901B
		public double Percent { get; set; }
	}
}
