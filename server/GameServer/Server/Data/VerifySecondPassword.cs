using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200017C RID: 380
	[ProtoContract]
	public class VerifySecondPassword
	{
		// Token: 0x04000879 RID: 2169
		[ProtoMember(1)]
		public string UserID;

		// Token: 0x0400087A RID: 2170
		[ProtoMember(2)]
		public string SecPwd;
	}
}
