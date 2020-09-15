using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000575 RID: 1397
	[ProtoContract]
	public class NPCTaskState
	{
		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060019F2 RID: 6642 RVA: 0x0019196C File Offset: 0x0018FB6C
		// (set) Token: 0x060019F3 RID: 6643 RVA: 0x00191983 File Offset: 0x0018FB83
		[ProtoMember(1)]
		public int NPCID { get; set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060019F4 RID: 6644 RVA: 0x0019198C File Offset: 0x0018FB8C
		// (set) Token: 0x060019F5 RID: 6645 RVA: 0x001919A3 File Offset: 0x0018FBA3
		[ProtoMember(2)]
		public int TaskState { get; set; }
	}
}
