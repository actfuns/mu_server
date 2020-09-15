using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000277 RID: 631
	[ProtoContract]
	public class CompMineBaseData
	{
		// Token: 0x04000FCF RID: 4047
		[ProtoMember(1)]
		public int MineTruckGo;

		// Token: 0x04000FD0 RID: 4048
		[ProtoMember(2)]
		public int SafeArrived;
	}
}
