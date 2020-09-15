using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000AE RID: 174
	[ProtoContract]
	public class ServerListData
	{
		// Token: 0x04000491 RID: 1169
		[ProtoMember(1)]
		public int RetCode = 0;

		// Token: 0x04000492 RID: 1170
		[ProtoMember(2)]
		public int RolesCount = 0;

		// Token: 0x04000493 RID: 1171
		[ProtoMember(3)]
		public List<LineData> LineDataList = null;
	}
}
