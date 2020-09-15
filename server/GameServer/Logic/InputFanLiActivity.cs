using System;

namespace GameServer.Logic
{
	// Token: 0x020006FE RID: 1790
	public class InputFanLiActivity : Activity
	{
		// Token: 0x06002B11 RID: 11025 RVA: 0x002669D0 File Offset: 0x00264BD0
		public override bool GiveAward(GameClient client, int _params)
		{
			return base.GiveAward(client, new AwardItem
			{
				AwardYuanBao = (int)((double)_params * this.FanLiPersent)
			});
		}

		// Token: 0x04003A1D RID: 14877
		public double FanLiPersent = 0.0;
	}
}
