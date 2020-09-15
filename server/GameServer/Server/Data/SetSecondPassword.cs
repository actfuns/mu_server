using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200017D RID: 381
	[ProtoContract]
	public class SetSecondPassword
	{
		// Token: 0x0400087B RID: 2171
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x0400087C RID: 2172
		[ProtoMember(2)]
		public string OldSecPwd;

		// Token: 0x0400087D RID: 2173
		[ProtoMember(3)]
		public string NewSecPwd;
	}
}
