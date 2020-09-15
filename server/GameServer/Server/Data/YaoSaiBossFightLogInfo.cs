using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020007F7 RID: 2039
	[ProtoContract]
	public class YaoSaiBossFightLogInfo
	{
		// Token: 0x04004397 RID: 17303
		[ProtoMember(1)]
		public int OtherRid;

		// Token: 0x04004398 RID: 17304
		[ProtoMember(2)]
		public List<YaoSaiBossFightLog> BossFightLogList;
	}
}
