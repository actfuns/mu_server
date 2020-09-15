using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000117 RID: 279
	[ProtoContract]
	public class ArtifactResultData
	{
		// Token: 0x040005FA RID: 1530
		[ProtoMember(1)]
		public int State = 0;

		// Token: 0x040005FB RID: 1531
		[ProtoMember(2)]
		public int EquipDbID = 0;

		// Token: 0x040005FC RID: 1532
		[ProtoMember(3)]
		public int Bind = 0;
	}
}
