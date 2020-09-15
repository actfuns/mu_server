using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200018A RID: 394
	[ProtoContract]
	public class SpecActInfoDB
	{
		// Token: 0x040008B8 RID: 2232
		[ProtoMember(1)]
		public int GroupID = 0;

		// Token: 0x040008B9 RID: 2233
		[ProtoMember(2)]
		public int ActID = 0;

		// Token: 0x040008BA RID: 2234
		[ProtoMember(3)]
		public int PurNum = 0;

		// Token: 0x040008BB RID: 2235
		[ProtoMember(4)]
		public int CountNum = 0;

		// Token: 0x040008BC RID: 2236
		[ProtoMember(5)]
		public short Active = 0;
	}
}
