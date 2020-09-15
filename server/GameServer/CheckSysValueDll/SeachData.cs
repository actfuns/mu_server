using System;
using ProtoBuf;

namespace CheckSysValueDll
{
	// Token: 0x020008EF RID: 2287
	[ProtoContract]
	public class SeachData
	{
		// Token: 0x04005006 RID: 20486
		[ProtoMember(1)]
		public string AttName;

		// Token: 0x04005007 RID: 20487
		[ProtoMember(2)]
		public string SeachVal;
	}
}
