using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200027A RID: 634
	[ProtoContract]
	public class CompMineSideScore
	{
		// Token: 0x04000FD7 RID: 4055
		[ProtoMember(1)]
		public int MineTruckGo;

		// Token: 0x04000FD8 RID: 4056
		[ProtoMember(2)]
		public int SafeArrived;

		// Token: 0x04000FD9 RID: 4057
		[ProtoMember(3)]
		public int MineTruckProcess;

		// Token: 0x04000FDA RID: 4058
		[ProtoMember(4)]
		public List<CompMineResData> ResJiFenList = new List<CompMineResData>();

		// Token: 0x04000FDB RID: 4059
		public int SuppliesNum;

		// Token: 0x04000FDC RID: 4060
		public int SuppliesStep;
	}
}
