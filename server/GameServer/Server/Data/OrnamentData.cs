using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003A8 RID: 936
	[ProtoContract]
	public class OrnamentData
	{
		// Token: 0x040018A3 RID: 6307
		[ProtoMember(1)]
		public int ID = 0;

		// Token: 0x040018A4 RID: 6308
		[ProtoMember(2)]
		public int Param1 = 0;

		// Token: 0x040018A5 RID: 6309
		[ProtoMember(3)]
		public int Param2 = 0;
	}
}
