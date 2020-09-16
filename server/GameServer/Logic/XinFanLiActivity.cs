using System;

namespace GameServer.Logic
{
	
	public class XinFanLiActivity : KingActivity
	{
		
		public override bool GiveAward(GameClient client, int _params)
		{
			return base.GiveAward(client, new AwardItem
			{
				AwardYuanBao = _params
			});
		}
	}
}
