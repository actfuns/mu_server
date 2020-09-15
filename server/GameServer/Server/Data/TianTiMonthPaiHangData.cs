using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200048F RID: 1167
	[ProtoContract]
	public class TianTiMonthPaiHangData
	{
		// Token: 0x04001EEA RID: 7914
		[ProtoMember(1)]
		public TianTiPaiHangRoleData SelfPaiHangRoleData;

		// Token: 0x04001EEB RID: 7915
		[ProtoMember(2)]
		public List<TianTiPaiHangRoleData> PaiHangRoleDataList;
	}
}
