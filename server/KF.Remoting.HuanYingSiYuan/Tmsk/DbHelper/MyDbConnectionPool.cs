using System;
using System.Collections.Generic;
using System.Threading;

namespace Tmsk.DbHelper
{
	
	public class MyDbConnectionPool
	{
		
		public int ConnCount;

		
		public string DatabaseKey;

		
		public string ConnectionString;

		
		public Semaphore SemaphoreClients = new Semaphore(0, 100);

		
		public Queue<MyDbConnection2> DBConns = new Queue<MyDbConnection2>();
	}
}
