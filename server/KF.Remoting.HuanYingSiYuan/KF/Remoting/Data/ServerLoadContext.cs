using System;
using System.Collections.Generic;
using KF.Contract.Data;

namespace KF.Remoting.Data
{
	// Token: 0x02000015 RID: 21
	public class ServerLoadContext
	{
		// Token: 0x060000AF RID: 175 RVA: 0x0000A31C File Offset: 0x0000851C
		public void CalcServerLoadAvg()
		{
			if (this.AlivedServerCount > 0 && this.AlivedGameFuBenCount > 0)
			{
				this.RealServerLoadAvg = this.AlivedGameFuBenCount / this.AlivedServerCount;
			}
			else
			{
				this.RealServerLoadAvg = 0;
			}
			this.ServerLoadAvg = this.RealServerLoadAvg + 5;
		}

		// Token: 0x0400006E RID: 110
		public int AlivedServerCount;

		// Token: 0x0400006F RID: 111
		public int AlivedGameFuBenCount;

		// Token: 0x04000070 RID: 112
		public int ServerLoadAvg;

		// Token: 0x04000071 RID: 113
		public int RealServerLoadAvg;

		// Token: 0x04000072 RID: 114
		public int SignUpRoleCount;

		// Token: 0x04000073 RID: 115
		public int StartGameRoleCount;

		// Token: 0x04000074 RID: 116
		public bool AssginGameFuBenComplete;

		// Token: 0x04000075 RID: 117
		public LinkedList<KuaFuServerGameConfig> IdelActiveServerQueue = new LinkedList<KuaFuServerGameConfig>();
	}
}
