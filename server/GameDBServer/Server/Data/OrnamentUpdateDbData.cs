using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200015B RID: 347
	[ProtoContract]
	public class OrnamentUpdateDbData
	{
		// Token: 0x04000860 RID: 2144
		[ProtoMember(1)]
		public int RoleId;

		// Token: 0x04000861 RID: 2145
		[ProtoMember(2)]
		public OrnamentData Data;
	}
}
