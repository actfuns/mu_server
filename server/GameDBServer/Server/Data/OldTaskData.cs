using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000092 RID: 146
	[ProtoContract]
	public class OldTaskData
	{
		// Token: 0x04000356 RID: 854
		[ProtoMember(1)]
		public int TaskID;

		// Token: 0x04000357 RID: 855
		[ProtoMember(2)]
		public int DoCount;
	}
}
