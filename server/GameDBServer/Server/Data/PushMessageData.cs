using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200009E RID: 158
	[ProtoContract]
	public class PushMessageData
	{
		// Token: 0x04000383 RID: 899
		[ProtoMember(1)]
		public string UserID = "";

		// Token: 0x04000384 RID: 900
		[ProtoMember(2)]
		public string PushID = "";

		// Token: 0x04000385 RID: 901
		[ProtoMember(3)]
		public string LastLoginTime = "";
	}
}
