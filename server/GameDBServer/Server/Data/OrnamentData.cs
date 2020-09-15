using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200015A RID: 346
	[ProtoContract]
	public class OrnamentData
	{
		// Token: 0x0400085D RID: 2141
		[ProtoMember(1)]
		public int ID = 0;

		// Token: 0x0400085E RID: 2142
		[ProtoMember(2)]
		public int Param1 = 0;

		// Token: 0x0400085F RID: 2143
		[ProtoMember(3)]
		public int Param2 = 0;
	}
}
