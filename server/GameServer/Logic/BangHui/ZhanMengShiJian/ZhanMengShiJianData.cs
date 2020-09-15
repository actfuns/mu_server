using System;
using ProtoBuf;

namespace GameServer.Logic.BangHui.ZhanMengShiJian
{
	// Token: 0x020005BD RID: 1469
	[ProtoContract]
	public class ZhanMengShiJianData
	{
		// Token: 0x04002960 RID: 10592
		public int PKId = 0;

		// Token: 0x04002961 RID: 10593
		[ProtoMember(1)]
		public int BHID = 0;

		// Token: 0x04002962 RID: 10594
		[ProtoMember(2)]
		public int ShiJianType = 0;

		// Token: 0x04002963 RID: 10595
		[ProtoMember(3)]
		public string RoleName = "";

		// Token: 0x04002964 RID: 10596
		[ProtoMember(4)]
		public string CreateTime = "";

		// Token: 0x04002965 RID: 10597
		[ProtoMember(5)]
		public int SubValue1 = -1;

		// Token: 0x04002966 RID: 10598
		[ProtoMember(6)]
		public int SubValue2 = -1;

		// Token: 0x04002967 RID: 10599
		[ProtoMember(7)]
		public int SubValue3 = -1;

		// Token: 0x04002968 RID: 10600
		[ProtoMember(8)]
		public string SubSzValue1 = "";
	}
}
