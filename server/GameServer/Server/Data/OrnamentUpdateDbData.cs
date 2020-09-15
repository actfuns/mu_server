using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003A9 RID: 937
	[ProtoContract]
	public class OrnamentUpdateDbData
	{
		// Token: 0x040018A6 RID: 6310
		[ProtoMember(1)]
		public int RoleId;

		// Token: 0x040018A7 RID: 6311
		[ProtoMember(2)]
		public OrnamentData Data;
	}
}
