using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003FD RID: 1021
	[ProtoContract]
	public class ShenJiFuWenData
	{
		// Token: 0x04001B3A RID: 6970
		[ProtoMember(1)]
		public int ShenJiID = 0;

		// Token: 0x04001B3B RID: 6971
		[ProtoMember(2)]
		public int Level = 0;
	}
}
