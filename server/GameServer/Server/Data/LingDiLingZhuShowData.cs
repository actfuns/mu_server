using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000310 RID: 784
	[ProtoContract]
	public class LingDiLingZhuShowData
	{
		// Token: 0x0400141A RID: 5146
		[ProtoMember(1)]
		public int AdmireCount;

		// Token: 0x0400141B RID: 5147
		[ProtoMember(2)]
		public RoleData4Selector RoleData4Selector;
	}
}
