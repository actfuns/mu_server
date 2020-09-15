using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000C2 RID: 194
	[ProtoContract]
	public class TianTiAwardsData
	{
		// Token: 0x0400053E RID: 1342
		[ProtoMember(1)]
		public int Success;

		// Token: 0x0400053F RID: 1343
		[ProtoMember(2)]
		public int DuanWeiJiFen;

		// Token: 0x04000540 RID: 1344
		[ProtoMember(3)]
		public int RongYao;
	}
}
