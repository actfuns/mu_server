using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000CA RID: 202
	[ProtoContract]
	public class WingData
	{
		// Token: 0x04000584 RID: 1412
		[ProtoMember(1)]
		public int DbID = 0;

		// Token: 0x04000585 RID: 1413
		[ProtoMember(2)]
		public int WingID = 0;

		// Token: 0x04000586 RID: 1414
		[ProtoMember(3)]
		public int ForgeLevel = 0;

		// Token: 0x04000587 RID: 1415
		[ProtoMember(4)]
		public long AddDateTime = 0L;

		// Token: 0x04000588 RID: 1416
		[ProtoMember(5)]
		public int JinJieFailedNum = 0;

		// Token: 0x04000589 RID: 1417
		[ProtoMember(6)]
		public int Using = 0;

		// Token: 0x0400058A RID: 1418
		[ProtoMember(7)]
		public int StarExp = 0;

		// Token: 0x0400058B RID: 1419
		[ProtoMember(8)]
		public int ZhuLingNum = 0;

		// Token: 0x0400058C RID: 1420
		[ProtoMember(9)]
		public int ZhuHunNum = 0;
	}
}
