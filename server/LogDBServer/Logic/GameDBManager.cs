using System;

namespace LogDBServer.Logic
{
	// Token: 0x02000019 RID: 25
	public class GameDBManager
	{
		// Token: 0x04000037 RID: 55
		public const int DBAutoIncreaseStepValue = 1000000;

		// Token: 0x04000038 RID: 56
		public static int ZoneID = 1;

		// Token: 0x04000039 RID: 57
		public static ServerEvents SystemServerSQLEvents = new ServerEvents
		{
			EventRootPath = "SQLs",
			EventPreFileName = "sql"
		};
	}
}
