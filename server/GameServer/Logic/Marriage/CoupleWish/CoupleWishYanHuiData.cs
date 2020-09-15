using System;
using KF.Contract.Data;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleWish
{
	// Token: 0x02000370 RID: 880
	[ProtoContract]
	public class CoupleWishYanHuiData
	{
		// Token: 0x0400173D RID: 5949
		[ProtoMember(1)]
		public KuaFuRoleMiniData Man;

		// Token: 0x0400173E RID: 5950
		[ProtoMember(2)]
		public KuaFuRoleMiniData Wife;

		// Token: 0x0400173F RID: 5951
		[ProtoMember(3, IsRequired = true)]
		public int TotalJoinNum;

		// Token: 0x04001740 RID: 5952
		[ProtoMember(4, IsRequired = true)]
		public int MyJoinNum;

		// Token: 0x04001741 RID: 5953
		[ProtoMember(5, IsRequired = true)]
		public int DbCoupleId;
	}
}
