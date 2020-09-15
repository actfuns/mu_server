using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.Spread
{
	// Token: 0x02000451 RID: 1105
	[ProtoContract]
	public class SpreadData
	{
		// Token: 0x04001DCF RID: 7631
		[ProtoMember(1)]
		public bool IsOpen = false;

		// Token: 0x04001DD0 RID: 7632
		[ProtoMember(2)]
		public string SpreadCode = "";

		// Token: 0x04001DD1 RID: 7633
		[ProtoMember(3)]
		public string VerifyCode = "";

		// Token: 0x04001DD2 RID: 7634
		[ProtoMember(4)]
		public int CountRole = 0;

		// Token: 0x04001DD3 RID: 7635
		[ProtoMember(5)]
		public int CountVip = 0;

		// Token: 0x04001DD4 RID: 7636
		[ProtoMember(6)]
		public int CountLevel = 0;

		// Token: 0x04001DD5 RID: 7637
		[ProtoMember(7)]
		public int State = 0;

		// Token: 0x04001DD6 RID: 7638
		[ProtoMember(8)]
		public Dictionary<int, string> AwardDic = new Dictionary<int, string>();

		// Token: 0x04001DD7 RID: 7639
		[ProtoMember(9)]
		public Dictionary<int, int> AwardCountDic = new Dictionary<int, int>();
	}
}
