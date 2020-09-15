using System;
using ProtoBuf;

namespace GameDBServer.Logic.BoCai
{
	// Token: 0x0200011C RID: 284
	[ProtoContract]
	public class OpenLottery
	{
		// Token: 0x04000788 RID: 1928
		[ProtoMember(1)]
		public long DataPeriods;

		// Token: 0x04000789 RID: 1929
		[ProtoMember(2)]
		public string strWinNum;

		// Token: 0x0400078A RID: 1930
		[ProtoMember(3)]
		public int BocaiType;

		// Token: 0x0400078B RID: 1931
		[ProtoMember(4)]
		public long SurplusBalance;

		// Token: 0x0400078C RID: 1932
		[ProtoMember(5)]
		public long AllBalance;

		// Token: 0x0400078D RID: 1933
		[ProtoMember(6)]
		public int XiaoHaoDaiBi;

		// Token: 0x0400078E RID: 1934
		[ProtoMember(7)]
		public string WinInfo;

		// Token: 0x0400078F RID: 1935
		[ProtoMember(8)]
		public bool IsAward;
	}
}
