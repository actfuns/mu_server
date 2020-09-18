using System;

namespace GameServer.Logic
{
	
	public class BroadcastInfoItem
	{
		
		public int ID = 0;

		
		public int InfoClass = 0;

		
		public int HintErrID = -1;

		
		public int TimeType = 0;

		
		public int KaiFuStartDay = -1;

		
		public int KaiFuShowType = -1;

		
		public string WeekDays = "";

		
		public BroadcastTimeItem[] Times = null;

		
		public DateTimeRange[] OnlineNoticeTimeRanges = null;

		
		public string Text = "";

		
		public int MinZhuanSheng = 0;

		
		public int MinLevel = 0;
	}
}
