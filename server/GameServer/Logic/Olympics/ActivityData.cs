using System;
using ProtoBuf;

namespace GameServer.Logic.Olympics
{
	// Token: 0x02000388 RID: 904
	[ProtoContract]
	public class ActivityData
	{
		// Token: 0x040017DA RID: 6106
		[ProtoMember(1)]
		public int ActivityType = 0;

		// Token: 0x040017DB RID: 6107
		[ProtoMember(2)]
		public bool ActivityIsOpen = false;

		// Token: 0x040017DC RID: 6108
		[ProtoMember(3)]
		public DateTime TimeBegin = DateTime.MinValue;

		// Token: 0x040017DD RID: 6109
		[ProtoMember(4)]
		public DateTime TimeEnd = DateTime.MinValue;

		// Token: 0x040017DE RID: 6110
		[ProtoMember(5)]
		public DateTime TimeAwardBegin = DateTime.MinValue;

		// Token: 0x040017DF RID: 6111
		[ProtoMember(6)]
		public DateTime TimeAwardEnd = DateTime.MinValue;
	}
}
