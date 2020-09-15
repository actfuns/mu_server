using System;
using ProtoBuf;

namespace GameServer.Logic.BocaiSys
{
	// Token: 0x0200006B RID: 107
	[ProtoContract]
	public class BuyBoCai2SDB
	{
		// Token: 0x04000279 RID: 633
		[ProtoMember(1)]
		public int m_RoleID;

		// Token: 0x0400027A RID: 634
		[ProtoMember(2)]
		public string m_RoleName;

		// Token: 0x0400027B RID: 635
		[ProtoMember(3)]
		public int ZoneID;

		// Token: 0x0400027C RID: 636
		[ProtoMember(4)]
		public string strUserID;

		// Token: 0x0400027D RID: 637
		[ProtoMember(5)]
		public int ServerId;

		// Token: 0x0400027E RID: 638
		[ProtoMember(6)]
		public int BuyNum;

		// Token: 0x0400027F RID: 639
		[ProtoMember(7)]
		public string strBuyValue;

		// Token: 0x04000280 RID: 640
		[ProtoMember(8)]
		public bool IsSend;

		// Token: 0x04000281 RID: 641
		[ProtoMember(9)]
		public bool IsWin;

		// Token: 0x04000282 RID: 642
		[ProtoMember(10)]
		public int BocaiType;

		// Token: 0x04000283 RID: 643
		[ProtoMember(11)]
		public long DataPeriods;
	}
}
