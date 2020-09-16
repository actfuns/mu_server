using System;

namespace GameServer.Logic
{
	
	public enum GoodsExchangeCmds
	{
		
		None,
		
		Request,
		
		Refuse,
		
		Agree,
		
		Cancel,
		
		AddGoods,
		
		RemoveGoods,
		
		UpdateMoney,
		
		UpdateYuanBao,
		
		Lock,
		
		Unlock,
		
		Done
	}
}
