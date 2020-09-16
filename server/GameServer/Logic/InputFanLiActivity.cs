using System;

namespace GameServer.Logic
{
	
	public class InputFanLiActivity : Activity
	{
		
		public override bool GiveAward(GameClient client, int _params)
		{
			return base.GiveAward(client, new AwardItem
			{
				AwardYuanBao = (int)((double)_params * this.FanLiPersent)
			});
		}

		
		public double FanLiPersent = 0.0;
	}
}
