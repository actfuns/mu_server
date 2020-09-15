using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000173 RID: 371
	[ProtoContract]
	public class ShenJiFuWenData
	{
		// Token: 0x040008A0 RID: 2208
		[ProtoMember(1)]
		public int ShenJiID = 0;

		// Token: 0x040008A1 RID: 2209
		[ProtoMember(2)]
		public int Level = 0;
	}
}
