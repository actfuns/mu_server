using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200006E RID: 110
	[ProtoContract]
	public class JieriCZKingData
	{
		// Token: 0x04000264 RID: 612
		[ProtoMember(1)]
		public int YuanBao;

		// Token: 0x04000265 RID: 613
		[ProtoMember(2)]
		public List<InputKingPaiHangData> ListPaiHang;

		// Token: 0x04000266 RID: 614
		[ProtoMember(3)]
		public int State;

		// Token: 0x04000267 RID: 615
		[ProtoMember(4)]
		public List<InputKingPaiHangData> ListPaiHangYestoday;
	}
}
