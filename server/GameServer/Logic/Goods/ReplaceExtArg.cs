using System;

namespace GameServer.Logic.Goods
{
	
	public class ReplaceExtArg
	{
		
		public void Reset()
		{
			this.CurrEquipZhuiJiaLevel = -1;
			this.CurrEquipQiangHuaLevel = -1;
			this.CurrEquipSuit = -1;
		}

		
		public int CurrEquipZhuiJiaLevel = -1;

		
		public int CurrEquipQiangHuaLevel = -1;

		
		public int CurrEquipSuit = -1;

		
		public int CurrEquipJuHun = -1;
	}
}
