using System;

namespace GameServer.Logic
{
	// Token: 0x0200070D RID: 1805
	public class XinFanLiActivity : KingActivity
	{
		// Token: 0x06002B4F RID: 11087 RVA: 0x00267D2C File Offset: 0x00265F2C
		public override bool GiveAward(GameClient client, int _params)
		{
			return base.GiveAward(client, new AwardItem
			{
				AwardYuanBao = _params
			});
		}
	}
}
