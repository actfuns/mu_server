using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000B0 RID: 176
	[ProtoContract]
	public class SpecActInfoDB
	{
		// Token: 0x04000498 RID: 1176
		[ProtoMember(1)]
		public int GroupID = 0;

		// Token: 0x04000499 RID: 1177
		[ProtoMember(2)]
		public int ActID = 0;

		// Token: 0x0400049A RID: 1178
		[ProtoMember(3)]
		public int PurNum = 0;

		// Token: 0x0400049B RID: 1179
		[ProtoMember(4)]
		public int CountNum = 0;

		// Token: 0x0400049C RID: 1180
		[ProtoMember(5)]
		public short Active = 0;
	}
}
