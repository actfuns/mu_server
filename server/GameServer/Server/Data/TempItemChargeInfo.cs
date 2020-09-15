using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200012C RID: 300
	[ProtoContract]
	public class TempItemChargeInfo
	{
		// Token: 0x04000678 RID: 1656
		[ProtoMember(1)]
		public int ID = 0;

		// Token: 0x04000679 RID: 1657
		[ProtoMember(2)]
		public string userID = "";

		// Token: 0x0400067A RID: 1658
		[ProtoMember(3)]
		public int chargeRoleID = 0;

		// Token: 0x0400067B RID: 1659
		[ProtoMember(4)]
		public int addUserMoney = 0;

		// Token: 0x0400067C RID: 1660
		[ProtoMember(5)]
		public int zhigouID = 0;

		// Token: 0x0400067D RID: 1661
		[ProtoMember(6)]
		public string chargeTime = "";

		// Token: 0x0400067E RID: 1662
		[ProtoMember(7)]
		public byte isDel = 0;
	}
}
