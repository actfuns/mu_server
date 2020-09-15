using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000023 RID: 35
	[ProtoContract]
	public class ServerDayData
	{
		// Token: 0x040000D0 RID: 208
		[ProtoMember(1)]
		public int Dayid = 0;

		// Token: 0x040000D1 RID: 209
		[ProtoMember(2)]
		public string CDate;

		// Token: 0x040000D2 RID: 210
		[ProtoMember(3)]
		public int WorldLevel = 0;
	}
}
