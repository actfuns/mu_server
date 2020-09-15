using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000126 RID: 294
	[ProtoContract]
	public class ChangeNameInfo
	{
		// Token: 0x0400065E RID: 1630
		[ProtoMember(1)]
		public int ZuanShi = 0;

		// Token: 0x0400065F RID: 1631
		[ProtoMember(2)]
		public List<EachRoleChangeName> RoleList = new List<EachRoleChangeName>();
	}
}
