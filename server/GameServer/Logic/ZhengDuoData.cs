using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x02000424 RID: 1060
	[ProtoContract]
	public class ZhengDuoData
	{
		// Token: 0x04001C7E RID: 7294
		[ProtoMember(1)]
		public int Step;

		// Token: 0x04001C7F RID: 7295
		[ProtoMember(2)]
		public int State;

		// Token: 0x04001C80 RID: 7296
		[ProtoMember(3)]
		public int SignUp;

		// Token: 0x04001C81 RID: 7297
		[ProtoMember(4)]
		public string OtherName;

		// Token: 0x04001C82 RID: 7298
		[ProtoMember(5)]
		public int OtherZoneId;

		// Token: 0x04001C83 RID: 7299
		[ProtoMember(6)]
		public int Lose;
	}
}
