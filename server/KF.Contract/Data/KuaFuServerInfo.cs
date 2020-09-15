using System;

namespace KF.Contract.Data
{
	// Token: 0x02000012 RID: 18
	[Serializable]
	public class KuaFuServerInfo
	{
		// Token: 0x0400004E RID: 78
		public int ServerId;

		// Token: 0x0400004F RID: 79
		public string Ip;

		// Token: 0x04000050 RID: 80
		public int Port;

		// Token: 0x04000051 RID: 81
		public string DbIp;

		// Token: 0x04000052 RID: 82
		public int DbPort;

		// Token: 0x04000053 RID: 83
		public string LogDbIp;

		// Token: 0x04000054 RID: 84
		public int LogDbPort;

		// Token: 0x04000055 RID: 85
		public int State;

		// Token: 0x04000056 RID: 86
		public int Flags;

		// Token: 0x04000057 RID: 87
		public int Age;

		// Token: 0x04000058 RID: 88
		public int Load;
	}
}
