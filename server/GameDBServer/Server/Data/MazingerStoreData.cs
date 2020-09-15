using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000067 RID: 103
	[ProtoContract]
	public class MazingerStoreData
	{
		// Token: 0x04000233 RID: 563
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000234 RID: 564
		[ProtoMember(2)]
		public int Type = 0;

		// Token: 0x04000235 RID: 565
		[ProtoMember(3)]
		public int Stage = 0;

		// Token: 0x04000236 RID: 566
		[ProtoMember(4)]
		public int StarLevel = 0;

		// Token: 0x04000237 RID: 567
		[ProtoMember(5)]
		public int Exp = 0;
	}
}
