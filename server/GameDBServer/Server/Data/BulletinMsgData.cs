using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200003A RID: 58
	[ProtoContract]
	public class BulletinMsgData
	{
		// Token: 0x0400012F RID: 303
		[ProtoMember(1)]
		public string MsgID = "";

		// Token: 0x04000130 RID: 304
		[ProtoMember(2)]
		public int PlayMinutes;

		// Token: 0x04000131 RID: 305
		[ProtoMember(3)]
		public int ToPlayNum = 0;

		// Token: 0x04000132 RID: 306
		[ProtoMember(4)]
		public string BulletinText = "";

		// Token: 0x04000133 RID: 307
		[ProtoMember(5)]
		public long BulletinTicks = 0L;

		// Token: 0x04000134 RID: 308
		[ProtoMember(6)]
		public int playingNum = 0;

		// Token: 0x04000135 RID: 309
		[ProtoMember(7)]
		public int MsgType = 0;

		// Token: 0x04000136 RID: 310
		[ProtoMember(8)]
		public int Interval = 0;
	}
}
