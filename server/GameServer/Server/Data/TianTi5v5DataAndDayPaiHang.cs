using System;
using System.Collections.Generic;
using KF.Contract.Data;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000BD RID: 189
	[ProtoContract]
	public class TianTi5v5DataAndDayPaiHang
	{
		// Token: 0x04000477 RID: 1143
		[ProtoMember(1)]
		public TianTi5v5ZhanDuiData TianTi5v5Data;

		// Token: 0x04000478 RID: 1144
		[ProtoMember(2)]
		public List<TianTi5v5ZhanDuiData> PaiHangRoleDataList;

		// Token: 0x04000479 RID: 1145
		[ProtoMember(3)]
		public int HaveMonthPaiHangAwards;

		// Token: 0x0400047A RID: 1146
		[ProtoMember(4)]
		public int TodayFightCount;
	}
}
