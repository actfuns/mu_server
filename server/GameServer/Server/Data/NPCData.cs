using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000574 RID: 1396
	[ProtoContract]
	public class NPCData
	{
		// Token: 0x040025AA RID: 9642
		[ProtoMember(1)]
		public int MapCode = 0;

		// Token: 0x040025AB RID: 9643
		[ProtoMember(2)]
		public int RoleID = 0;

		// Token: 0x040025AC RID: 9644
		[ProtoMember(3)]
		public int NPCID = 0;

		// Token: 0x040025AD RID: 9645
		[ProtoMember(4)]
		public List<int> NewTaskIDs = null;

		// Token: 0x040025AE RID: 9646
		[ProtoMember(5)]
		public List<int> OperationIDs = null;

		// Token: 0x040025AF RID: 9647
		[ProtoMember(6)]
		public List<int> ScriptIDs = null;

		// Token: 0x040025B0 RID: 9648
		[ProtoMember(7)]
		public int ExtensionID = 0;

		// Token: 0x040025B1 RID: 9649
		[ProtoMember(8)]
		public List<int> NewTaskIDsDoneCount = null;

		// Token: 0x040025B2 RID: 9650
		[ProtoMember(9)]
		public int GatherTime = 0;
	}
}
