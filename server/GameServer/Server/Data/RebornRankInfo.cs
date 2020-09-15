using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003E1 RID: 993
	[ProtoContract]
	public class RebornRankInfo
	{
		// Token: 0x04001A6B RID: 6763
		[ProtoMember(1)]
		public int Key = 0;

		// Token: 0x04001A6C RID: 6764
		[ProtoMember(2)]
		public int Value = 0;

		// Token: 0x04001A6D RID: 6765
		[ProtoMember(3)]
		public string Param1 = "";

		// Token: 0x04001A6E RID: 6766
		[ProtoMember(4)]
		public string Param2 = "";

		// Token: 0x04001A6F RID: 6767
		[ProtoMember(5)]
		public int UserPtID = 0;
	}
}
