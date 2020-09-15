using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000AD RID: 173
	[ProtoContract]
	public class ServerDayData
	{
		// Token: 0x0400048E RID: 1166
		[ProtoMember(1)]
		public int Dayid = 0;

		// Token: 0x0400048F RID: 1167
		[ProtoMember(2)]
		public string CDate;

		// Token: 0x04000490 RID: 1168
		[ProtoMember(3)]
		public int WorldLevel = 0;
	}
}
