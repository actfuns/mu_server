using System;

namespace GameDBServer.Logic
{
	
	public class HuangDiTeQuanItem
	{
		
		public int ID = 0;

		
		public int ToLaoFangDayID = 0;

		
		public int ToLaoFangNum = 0;

		
		public int OffLaoFangDayID = 0;

		
		public int OffLaoFangNum = 0;

		
		public int BanCatDayID = 0;

		
		public int BanCatNum = 0;

		
		public long LastBanChatTicks = 0L;

		
		public long LastSendToLaoFangTicks = 0L;

		
		public long LastTakeOffLaoFangTicks = 0L;
	}
}
