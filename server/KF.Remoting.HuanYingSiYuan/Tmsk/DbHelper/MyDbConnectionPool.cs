using System;
using System.Collections.Generic;
using System.Threading;

namespace Tmsk.DbHelper
{
	// Token: 0x0200001C RID: 28
	public class MyDbConnectionPool
	{
		// Token: 0x040000AA RID: 170
		public int ConnCount;

		// Token: 0x040000AB RID: 171
		public string DatabaseKey;

		// Token: 0x040000AC RID: 172
		public string ConnectionString;

		// Token: 0x040000AD RID: 173
		public Semaphore SemaphoreClients = new Semaphore(0, 100);

		// Token: 0x040000AE RID: 174
		public Queue<MyDbConnection2> DBConns = new Queue<MyDbConnection2>();
	}
}
