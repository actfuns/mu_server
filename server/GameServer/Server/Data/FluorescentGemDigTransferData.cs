using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000137 RID: 311
	[ProtoContract]
	public class FluorescentGemDigTransferData
	{
		// Token: 0x040006CB RID: 1739
		[ProtoMember(1)]
		public int _Result;

		// Token: 0x040006CC RID: 1740
		[ProtoMember(2)]
		public List<int> _GemList;

		// Token: 0x040006CD RID: 1741
		[ProtoMember(3)]
		public Dictionary<int, int> _GemNumDict;
	}
}
