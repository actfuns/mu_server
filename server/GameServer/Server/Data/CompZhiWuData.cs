using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace Server.Data
{
	// Token: 0x02000282 RID: 642
	[ProtoContract]
	public class CompZhiWuData
	{
		// Token: 0x04000FF9 RID: 4089
		[ProtoMember(1)]
		public List<KFCompRoleData> CompRoleData = new List<KFCompRoleData>();
	}
}
