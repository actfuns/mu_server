using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000559 RID: 1369
	[ProtoContract]
	public class GoodsLimitData
	{
		// Token: 0x040024D8 RID: 9432
		[ProtoMember(1)]
		public int GoodsID;

		// Token: 0x040024D9 RID: 9433
		[ProtoMember(2)]
		public int DayID;

		// Token: 0x040024DA RID: 9434
		[ProtoMember(3)]
		public int UsedNum;
	}
}
