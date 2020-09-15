using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x0200034D RID: 845
	public class KuaFuMapData
	{
		// Token: 0x0400164D RID: 5709
		public object Mutex = new object();

		// Token: 0x0400164E RID: 5710
		public ConcurrentDictionary<IntPairKey, KuaFuLineData> LineMap2KuaFuLineDataDict = new ConcurrentDictionary<IntPairKey, KuaFuLineData>();

		// Token: 0x0400164F RID: 5711
		public ConcurrentDictionary<IntPairKey, KuaFuLineData> ServerMap2KuaFuLineDataDict = new ConcurrentDictionary<IntPairKey, KuaFuLineData>();

		// Token: 0x04001650 RID: 5712
		public ConcurrentDictionary<int, List<KuaFuLineData>> KuaFuMapServerIdDict = new ConcurrentDictionary<int, List<KuaFuLineData>>();

		// Token: 0x04001651 RID: 5713
		public ConcurrentDictionary<int, List<KuaFuLineData>> MapCode2KuaFuLineDataDict = new ConcurrentDictionary<int, List<KuaFuLineData>>();
	}
}
