using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200003C RID: 60
	[ProtoContract]
	public class ChangeNameInfo
	{
		// Token: 0x0400013A RID: 314
		[ProtoMember(1)]
		public int ZuanShi = 0;

		// Token: 0x0400013B RID: 315
		[ProtoMember(2)]
		public List<EachRoleChangeName> RoleList = new List<EachRoleChangeName>();
	}
}
