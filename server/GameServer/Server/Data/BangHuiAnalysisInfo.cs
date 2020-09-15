using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000215 RID: 533
	[ProtoContract]
	public class BangHuiAnalysisInfo
	{
		// Token: 0x04000C22 RID: 3106
		[ProtoMember(1)]
		public List<int> listAnalysisData = new List<int>();
	}
}
