using System;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic.Marriage.CoupleWish
{
	// Token: 0x0200036F RID: 879
	[ProtoContract]
	public class CoupleWishTop1AdmireData
	{
		// Token: 0x04001738 RID: 5944
		[ProtoMember(1, IsRequired = true)]
		public int DbCoupleId;

		// Token: 0x04001739 RID: 5945
		[ProtoMember(2)]
		public RoleData4Selector ManSelector;

		// Token: 0x0400173A RID: 5946
		[ProtoMember(3)]
		public RoleData4Selector WifeSelector;

		// Token: 0x0400173B RID: 5947
		[ProtoMember(4)]
		public int BeAdmireCount;

		// Token: 0x0400173C RID: 5948
		[ProtoMember(5)]
		public int MyAdmireCount;
	}
}
