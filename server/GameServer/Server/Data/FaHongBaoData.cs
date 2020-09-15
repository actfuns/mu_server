using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002E4 RID: 740
	[ProtoContract]
	public class FaHongBaoData
	{
		// Token: 0x0400130C RID: 4876
		[ProtoMember(1)]
		public int roleID;

		// Token: 0x0400130D RID: 4877
		[ProtoMember(2)]
		public int type;

		// Token: 0x0400130E RID: 4878
		[ProtoMember(3)]
		public int count;

		// Token: 0x0400130F RID: 4879
		[ProtoMember(4)]
		public int diamondNum;

		// Token: 0x04001310 RID: 4880
		[ProtoMember(5)]
		public string message;
	}
}
