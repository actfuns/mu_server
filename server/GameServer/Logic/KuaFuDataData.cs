using System;
using System.Collections.Generic;
using System.Threading;
using KF.Contract.Data;

namespace GameServer.Logic
{
	// Token: 0x0200050D RID: 1293
	public class KuaFuDataData
	{
		// Token: 0x0400224A RID: 8778
		public object Mutex = new object();

		// Token: 0x0400224B RID: 8779
		public Thread BackGroundThread = null;

		// Token: 0x0400224C RID: 8780
		public string HuanYingSiYuanUri = null;

		// Token: 0x0400224D RID: 8781
		public Dictionary<string, long> AllowLoginUserDict = new Dictionary<string, long>();

		// Token: 0x0400224E RID: 8782
		public int ServerInfoAsyncAge;

		// Token: 0x0400224F RID: 8783
		public Dictionary<int, KuaFuServerInfo> ServerIdServerInfoDict = new Dictionary<int, KuaFuServerInfo>();
	}
}
