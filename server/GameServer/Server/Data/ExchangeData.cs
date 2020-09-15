using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000551 RID: 1361
	[ProtoContract]
	public class ExchangeData
	{
		// Token: 0x04002484 RID: 9348
		[ProtoMember(1)]
		public int ExchangeID;

		// Token: 0x04002485 RID: 9349
		[ProtoMember(2)]
		public int RequestRoleID;

		// Token: 0x04002486 RID: 9350
		[ProtoMember(3)]
		public int AgreeRoleID;

		// Token: 0x04002487 RID: 9351
		[ProtoMember(4)]
		public Dictionary<int, List<GoodsData>> GoodsDict;

		// Token: 0x04002488 RID: 9352
		[ProtoMember(5)]
		public Dictionary<int, int> MoneyDict;

		// Token: 0x04002489 RID: 9353
		[ProtoMember(6)]
		public Dictionary<int, int> LockDict;

		// Token: 0x0400248A RID: 9354
		[ProtoMember(7)]
		public Dictionary<int, int> DoneDict;

		// Token: 0x0400248B RID: 9355
		[ProtoMember(8)]
		public long AddDateTime;

		// Token: 0x0400248C RID: 9356
		[ProtoMember(9)]
		public int Done;

		// Token: 0x0400248D RID: 9357
		[ProtoMember(10)]
		public Dictionary<int, int> YuanBaoDict;
	}
}
