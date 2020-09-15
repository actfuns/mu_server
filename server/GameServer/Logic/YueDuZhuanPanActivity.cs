using System;

namespace GameServer.Logic
{
	// Token: 0x0200071D RID: 1821
	public class YueDuZhuanPanActivity : Activity
	{
		// Token: 0x06002B7F RID: 11135 RVA: 0x00268A00 File Offset: 0x00266C00
		public override AwardItem GetAward(GameClient client)
		{
			return null;
		}

		// Token: 0x06002B80 RID: 11136 RVA: 0x00268A14 File Offset: 0x00266C14
		public override bool GiveAward(GameClient client, int _params)
		{
			return true;
		}

		// Token: 0x06002B81 RID: 11137 RVA: 0x00268A28 File Offset: 0x00266C28
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			return true;
		}
	}
}
