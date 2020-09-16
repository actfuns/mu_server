using System;
using System.Collections.Concurrent;
using KF.Contract.Data;

namespace KF.Remoting
{
	
	public class KuaFuServerManagerData
	{
		
		public ConcurrentDictionary<int, KuaFuServerInfo> ServerIdServerInfoDict = new ConcurrentDictionary<int, KuaFuServerInfo>();

		
		public ConcurrentDictionary<int, KuaFuServerGameConfig> ServerIdGameConfigDict = new ConcurrentDictionary<int, KuaFuServerGameConfig>();

		
		public ConcurrentDictionary<int, KuaFuServerGameConfig> KuaFuServerIdGameConfigDict = new ConcurrentDictionary<int, KuaFuServerGameConfig>();
	}
}
