using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200008F RID: 143
	[ProtoContract]
	public class LineData
	{
		// Token: 0x04000330 RID: 816
		[ProtoMember(1)]
		public int LineID = 0;

		// Token: 0x04000331 RID: 817
		[ProtoMember(2)]
		public string GameServerIP = "";

		// Token: 0x04000332 RID: 818
		[ProtoMember(3)]
		public int GameServerPort = 0;

		// Token: 0x04000333 RID: 819
		[ProtoMember(4)]
		public int OnlineCount = 0;
	}
}
