using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000123 RID: 291
	[ProtoContract]
	public class BulletinMsgData
	{
		// Token: 0x0400064F RID: 1615
		[ProtoMember(1)]
		public string MsgID = "";

		// Token: 0x04000650 RID: 1616
		[ProtoMember(2)]
		public int PlayMinutes;

		// Token: 0x04000651 RID: 1617
		[ProtoMember(3)]
		public int ToPlayNum = 0;

		// Token: 0x04000652 RID: 1618
		[ProtoMember(4)]
		public string BulletinText = "";

		// Token: 0x04000653 RID: 1619
		[ProtoMember(5)]
		public long BulletinTicks = 0L;

		// Token: 0x04000654 RID: 1620
		[ProtoMember(6)]
		public int playingNum = 0;

		// Token: 0x04000655 RID: 1621
		[ProtoMember(7)]
		public int MsgType = 0;

		// Token: 0x04000656 RID: 1622
		[ProtoMember(8)]
		public int Interval = 0;

		// Token: 0x04000657 RID: 1623
		public long LastBulletinTicks = 0L;
	}
}
