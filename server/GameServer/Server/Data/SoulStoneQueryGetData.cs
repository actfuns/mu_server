using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002D3 RID: 723
	[ProtoContract]
	public class SoulStoneQueryGetData
	{
		// Token: 0x040012A3 RID: 4771
		[ProtoMember(1)]
		public int CurrRandId;

		// Token: 0x040012A4 RID: 4772
		[ProtoMember(2)]
		public List<SoulStoneExtFuncItem> ExtFuncList;
	}
}
