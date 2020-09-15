using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000581 RID: 1409
	[ProtoContract]
	public class PushMessageData
	{
		// Token: 0x04002606 RID: 9734
		[ProtoMember(1)]
		public string UserID = "";

		// Token: 0x04002607 RID: 9735
		[ProtoMember(2)]
		public string PushID = "";

		// Token: 0x04002608 RID: 9736
		[ProtoMember(3)]
		public string LastLoginTime = "";
	}
}
