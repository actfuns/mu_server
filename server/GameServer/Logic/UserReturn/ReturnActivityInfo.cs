using System;

namespace GameServer.Logic.UserReturn
{
	
	public class ReturnActivityInfo
	{
		
		public int ID = 0;

		
		public int ActivityID = 0;

		
		public DateTime TimeBegin = DateTime.MinValue;

		
		public DateTime TimeEnd = DateTime.MinValue;

		
		public DateTime TimeBeginNoLogin = DateTime.MinValue;

		
		public DateTime TimeEndNoLogin = DateTime.MinValue;

		
		public int Level = 0;

		
		public int Vip = 4;

		
		public bool IsOpen = false;

		
		public string ActivityDay = "";

		
		public string TimeBeginStr = "";

		
		public string TimeEndStr = "";
	}
}
