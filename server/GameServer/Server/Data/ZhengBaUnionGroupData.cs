using System;
using System.Collections.Generic;
using KF.Contract.Data;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000421 RID: 1057
	[ProtoContract]
	public class ZhengBaUnionGroupData
	{
		// Token: 0x04001C6E RID: 7278
		[ProtoMember(1)]
		public int UnionGroup;

		// Token: 0x04001C6F RID: 7279
		[ProtoMember(2)]
		public List<ZhengBaSupportAnalysisData> SupportDatas;

		// Token: 0x04001C70 RID: 7280
		[ProtoMember(3)]
		public List<ZhengBaSupportLogData> SupportLogs;

		// Token: 0x04001C71 RID: 7281
		[ProtoMember(4)]
		public List<ZhengBaSupportFlagData> SupportFlags;

		// Token: 0x04001C72 RID: 7282
		[ProtoMember(5)]
		public int WinZhengBaPoint;
	}
}
