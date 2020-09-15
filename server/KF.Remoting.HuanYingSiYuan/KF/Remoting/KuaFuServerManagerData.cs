using System;
using System.Collections.Concurrent;
using KF.Contract.Data;

namespace KF.Remoting
{
	// Token: 0x0200003E RID: 62
	public class KuaFuServerManagerData
	{
		// Token: 0x04000170 RID: 368
		public ConcurrentDictionary<int, KuaFuServerInfo> ServerIdServerInfoDict = new ConcurrentDictionary<int, KuaFuServerInfo>();

		// Token: 0x04000171 RID: 369
		public ConcurrentDictionary<int, KuaFuServerGameConfig> ServerIdGameConfigDict = new ConcurrentDictionary<int, KuaFuServerGameConfig>();

		// Token: 0x04000172 RID: 370
		public ConcurrentDictionary<int, KuaFuServerGameConfig> KuaFuServerIdGameConfigDict = new ConcurrentDictionary<int, KuaFuServerGameConfig>();
	}
}
