using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020001C5 RID: 453
	[ProtoContract]
	public class SevenDayItemData
	{
		// Token: 0x04000A09 RID: 2569
		[ProtoMember(1)]
		public int AwardFlag;

		// Token: 0x04000A0A RID: 2570
		[ProtoMember(2)]
		public int Params1;

		// Token: 0x04000A0B RID: 2571
		[ProtoMember(3)]
		public int Params2;
	}
}
