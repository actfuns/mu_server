using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000DF RID: 223
	[ProtoContract]
	[Serializable]
	public class AgeDataT
	{
		// Token: 0x04000618 RID: 1560
		[ProtoMember(1)]
		public long Age;

		// Token: 0x04000619 RID: 1561
		[ProtoMember(2)]
		public byte[] V;
	}
}
