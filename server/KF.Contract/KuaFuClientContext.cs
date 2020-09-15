using System;
using System.Runtime.Remoting.Messaging;

namespace KF.Contract
{
	// Token: 0x02000002 RID: 2
	[Serializable]
	public class KuaFuClientContext : ILogicalThreadAffinative
	{
		// Token: 0x04000001 RID: 1
		public int ServerId;

		// Token: 0x04000002 RID: 2
		public int ClientId;

		// Token: 0x04000003 RID: 3
		public int GameType;

		// Token: 0x04000004 RID: 4
		public string Token;
	}
}
