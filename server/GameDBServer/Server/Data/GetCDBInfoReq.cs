using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000046 RID: 70
	[ProtoContract]
	public class GetCDBInfoReq
	{
		// Token: 0x04000166 RID: 358
		[ProtoMember(2)]
		public string PTID;

		// Token: 0x04000167 RID: 359
		[ProtoMember(3)]
		public string ServerID = "";

		// Token: 0x04000168 RID: 360
		[ProtoMember(1)]
		public string Gamecode = "";
	}
}
