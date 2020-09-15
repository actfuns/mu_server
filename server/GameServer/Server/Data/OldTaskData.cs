using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000577 RID: 1399
	[ProtoContract]
	public class OldTaskData
	{
		// Token: 0x040025C2 RID: 9666
		[ProtoMember(1)]
		public int TaskID;

		// Token: 0x040025C3 RID: 9667
		[ProtoMember(2)]
		public int DoCount;
	}
}
