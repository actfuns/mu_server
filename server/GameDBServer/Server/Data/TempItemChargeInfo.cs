using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000042 RID: 66
	[ProtoContract]
	public class TempItemChargeInfo
	{
		// Token: 0x04000153 RID: 339
		[ProtoMember(1)]
		public int ID = 0;

		// Token: 0x04000154 RID: 340
		[ProtoMember(2)]
		public string userID = "";

		// Token: 0x04000155 RID: 341
		[ProtoMember(3)]
		public int chargeRoleID = 0;

		// Token: 0x04000156 RID: 342
		[ProtoMember(4)]
		public int addUserMoney = 0;

		// Token: 0x04000157 RID: 343
		[ProtoMember(5)]
		public int zhigouID = 0;

		// Token: 0x04000158 RID: 344
		[ProtoMember(6)]
		public string chargeTime = "";

		// Token: 0x04000159 RID: 345
		[ProtoMember(7)]
		public byte isDel = 0;
	}
}
