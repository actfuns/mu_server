using System;
using KF.Contract.Data;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic.Marriage.CoupleWish
{
	// Token: 0x0200036A RID: 874
	[ProtoContract]
	public class CoupleWishCoupleData
	{
		// Token: 0x0400171B RID: 5915
		[ProtoMember(1)]
		public int DbCoupleId;

		// Token: 0x0400171C RID: 5916
		[ProtoMember(2)]
		public KuaFuRoleMiniData Man;

		// Token: 0x0400171D RID: 5917
		[ProtoMember(3)]
		public RoleData4Selector ManSelector;

		// Token: 0x0400171E RID: 5918
		[ProtoMember(4)]
		public KuaFuRoleMiniData Wife;

		// Token: 0x0400171F RID: 5919
		[ProtoMember(5)]
		public RoleData4Selector WifeSelector;

		// Token: 0x04001720 RID: 5920
		[ProtoMember(6)]
		public int BeWishedNum;

		// Token: 0x04001721 RID: 5921
		[ProtoMember(7)]
		public int Rank;
	}
}
