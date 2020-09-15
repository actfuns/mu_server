using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x020004E8 RID: 1256
	public class ReplaceExtArg
	{
		// Token: 0x0600176B RID: 5995 RVA: 0x0016F6B3 File Offset: 0x0016D8B3
		public void Reset()
		{
			this.CurrEquipZhuiJiaLevel = -1;
			this.CurrEquipQiangHuaLevel = -1;
			this.CurrEquipSuit = -1;
		}

		// Token: 0x04002144 RID: 8516
		public int CurrEquipZhuiJiaLevel = -1;

		// Token: 0x04002145 RID: 8517
		public int CurrEquipQiangHuaLevel = -1;

		// Token: 0x04002146 RID: 8518
		public int CurrEquipSuit = -1;

		// Token: 0x04002147 RID: 8519
		public int CurrEquipJuHun = -1;
	}
}
