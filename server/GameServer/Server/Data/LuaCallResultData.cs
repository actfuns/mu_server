using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005AD RID: 1453
	[ProtoContract]
	public class LuaCallResultData
	{
		// Token: 0x040028EF RID: 10479
		[ProtoMember(1)]
		public int MapCode = 0;

		// Token: 0x040028F0 RID: 10480
		[ProtoMember(2)]
		public int RoleID = 0;

		// Token: 0x040028F1 RID: 10481
		[ProtoMember(3)]
		public int NPCID = 0;

		// Token: 0x040028F2 RID: 10482
		[ProtoMember(4)]
		public int IsSuccess;

		// Token: 0x040028F3 RID: 10483
		[ProtoMember(5)]
		public string Result;

		// Token: 0x040028F4 RID: 10484
		[ProtoMember(6)]
		public int Tag;

		// Token: 0x040028F5 RID: 10485
		[ProtoMember(7)]
		public int ExtensionID;

		// Token: 0x040028F6 RID: 10486
		[ProtoMember(8)]
		public string LuaFunction;

		// Token: 0x040028F7 RID: 10487
		[ProtoMember(9)]
		public int ForceRefresh = 0;
	}
}
