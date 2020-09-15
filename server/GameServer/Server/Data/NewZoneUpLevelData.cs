using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000572 RID: 1394
	[ProtoContract]
	internal class NewZoneUpLevelData
	{
		// Token: 0x040025A7 RID: 9639
		[ProtoMember(1)]
		public List<NewZoneUpLevelItemData> Items;
	}
}
