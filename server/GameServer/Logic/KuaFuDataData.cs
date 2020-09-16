using System;
using System.Collections.Generic;
using System.Threading;
using KF.Contract.Data;

namespace GameServer.Logic
{
	
	public class KuaFuDataData
	{
		
		public object Mutex = new object();

		
		public Thread BackGroundThread = null;

		
		public string HuanYingSiYuanUri = null;

		
		public Dictionary<string, long> AllowLoginUserDict = new Dictionary<string, long>();

		
		public int ServerInfoAsyncAge;

		
		public Dictionary<int, KuaFuServerInfo> ServerIdServerInfoDict = new Dictionary<int, KuaFuServerInfo>();
	}
}
