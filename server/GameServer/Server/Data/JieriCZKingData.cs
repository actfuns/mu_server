using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.Data;

namespace Server.Data
{
	// Token: 0x0200014B RID: 331
	[ProtoContract]
	public class JieriCZKingData
	{
		// Token: 0x04000766 RID: 1894
		[ProtoMember(1)]
		public int YuanBao;

		// Token: 0x04000767 RID: 1895
		[ProtoMember(2)]
		public List<InputKingPaiHangData> ListPaiHang;

		// Token: 0x04000768 RID: 1896
		[ProtoMember(3)]
		public int State;

		// Token: 0x04000769 RID: 1897
		[ProtoMember(4)]
		public List<InputKingPaiHangData> ListPaiHangYestoday;
	}
}
