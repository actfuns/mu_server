using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000328 RID: 808
	[ProtoContract]
	public class KarenNotifyMsg
	{
		// Token: 0x040014DF RID: 5343
		[ProtoMember(1)]
		public int index;

		// Token: 0x040014E0 RID: 5344
		[ProtoMember(2)]
		public int LegionID;

		// Token: 0x040014E1 RID: 5345
		[ProtoMember(3)]
		public string param1 = "";

		// Token: 0x040014E2 RID: 5346
		[ProtoMember(4)]
		public string param2 = "";
	}
}
