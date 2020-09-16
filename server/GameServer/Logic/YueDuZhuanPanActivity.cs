using System;

namespace GameServer.Logic
{
	
	public class YueDuZhuanPanActivity : Activity
	{
		
		public override AwardItem GetAward(GameClient client)
		{
			return null;
		}

		
		public override bool GiveAward(GameClient client, int _params)
		{
			return true;
		}

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			return true;
		}
	}
}
