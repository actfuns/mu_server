using System;
using System.Collections.Generic;
using KF.Contract.Data;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000216 RID: 534
	[ProtoContract]
	public class BangHuiMatchMainInfo
	{
		// Token: 0x04000C23 RID: 3107
		[ProtoMember(1)]
		public int round = 0;

		// Token: 0x04000C24 RID: 3108
		[ProtoMember(2)]
		public List<BangHuiMatchPKInfo> LastRoundPKInfo = new List<BangHuiMatchPKInfo>();

		// Token: 0x04000C25 RID: 3109
		[ProtoMember(3)]
		public List<BangHuiMatchPKInfo> CurRoundPKInfo = new List<BangHuiMatchPKInfo>();

		// Token: 0x04000C26 RID: 3110
		[ProtoMember(4)]
		public int timestate;

		// Token: 0x04000C27 RID: 3111
		[ProtoMember(5)]
		public int seasonid;
	}
}
