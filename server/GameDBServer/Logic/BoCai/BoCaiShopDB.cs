using System;
using ProtoBuf;

namespace GameDBServer.Logic.BoCai
{
	// Token: 0x0200011A RID: 282
	[ProtoContract]
	public class BoCaiShopDB
	{
		// Token: 0x04000781 RID: 1921
		[ProtoMember(1)]
		public int ID;

		// Token: 0x04000782 RID: 1922
		[ProtoMember(2)]
		public string WuPinID;

		// Token: 0x04000783 RID: 1923
		[ProtoMember(3)]
		public int BuyNum;

		// Token: 0x04000784 RID: 1924
		[ProtoMember(4)]
		public int RoleID;

		// Token: 0x04000785 RID: 1925
		[ProtoMember(5)]
		public int Periods;
	}
}
