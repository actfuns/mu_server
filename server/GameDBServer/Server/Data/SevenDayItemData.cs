using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200010A RID: 266
	[ProtoContract]
	public class SevenDayItemData
	{
		// Token: 0x0400073C RID: 1852
		[ProtoMember(1)]
		public int AwardFlag;

		// Token: 0x0400073D RID: 1853
		[ProtoMember(2)]
		public int Params1;

		// Token: 0x0400073E RID: 1854
		[ProtoMember(3)]
		public int Params2;
	}
}
