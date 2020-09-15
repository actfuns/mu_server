using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000285 RID: 645
	[ProtoContract]
	public class CompDaLingZhuShowData
	{
		// Token: 0x04001001 RID: 4097
		[ProtoMember(1)]
		public int AdmireCount;

		// Token: 0x04001002 RID: 4098
		[ProtoMember(2)]
		public RoleData4Selector RoleData4Selector;
	}
}
