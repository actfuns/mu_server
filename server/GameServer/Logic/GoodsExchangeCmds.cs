using System;

namespace GameServer.Logic
{
	// Token: 0x0200063B RID: 1595
	public enum GoodsExchangeCmds
	{
		// Token: 0x04002EB2 RID: 11954
		None,
		// Token: 0x04002EB3 RID: 11955
		Request,
		// Token: 0x04002EB4 RID: 11956
		Refuse,
		// Token: 0x04002EB5 RID: 11957
		Agree,
		// Token: 0x04002EB6 RID: 11958
		Cancel,
		// Token: 0x04002EB7 RID: 11959
		AddGoods,
		// Token: 0x04002EB8 RID: 11960
		RemoveGoods,
		// Token: 0x04002EB9 RID: 11961
		UpdateMoney,
		// Token: 0x04002EBA RID: 11962
		UpdateYuanBao,
		// Token: 0x04002EBB RID: 11963
		Lock,
		// Token: 0x04002EBC RID: 11964
		Unlock,
		// Token: 0x04002EBD RID: 11965
		Done
	}
}
