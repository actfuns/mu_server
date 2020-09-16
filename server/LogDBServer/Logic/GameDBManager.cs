using System;

namespace LogDBServer.Logic
{
	
	public class GameDBManager
	{
		
		public const int DBAutoIncreaseStepValue = 1000000;

		
		public static int ZoneID = 1;

		
		public static ServerEvents SystemServerSQLEvents = new ServerEvents
		{
			EventRootPath = "SQLs",
			EventPreFileName = "sql"
		};
	}
}
