using System;
using System.Collections.Generic;
using KF.Contract.Data;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000BC RID: 188
	[ProtoContract]
	public class TianTi5v5MonthPaiHangData
	{
		// Token: 0x04000475 RID: 1141
		[ProtoMember(1)]
		public TianTi5v5ZhanDuiData SelfPaiHangRoleData;

		// Token: 0x04000476 RID: 1142
		[ProtoMember(2)]
		public List<TianTi5v5ZhanDuiData> PaiHangRoleDataList;
	}
}
