using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000D0 RID: 208
	[ProtoContract]
	public class ShengXiaoGuessHistory
	{
		// Token: 0x040005A8 RID: 1448
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040005A9 RID: 1449
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x040005AA RID: 1450
		[ProtoMember(3)]
		public int GuessKey = 0;

		// Token: 0x040005AB RID: 1451
		[ProtoMember(4)]
		public int Mortgage = 0;

		// Token: 0x040005AC RID: 1452
		[ProtoMember(5)]
		public int ResultKey = 0;

		// Token: 0x040005AD RID: 1453
		[ProtoMember(6)]
		public int GainNum = 0;

		// Token: 0x040005AE RID: 1454
		[ProtoMember(7)]
		public int LeftMortgage = 0;

		// Token: 0x040005AF RID: 1455
		[ProtoMember(8)]
		public string GuessTime = "";
	}
}
