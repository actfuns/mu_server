using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000088 RID: 136
	[ProtoContract]
	internal class NewZoneUpLevelData
	{
		// Token: 0x040002E1 RID: 737
		[ProtoMember(1)]
		public List<NewZoneUpLevelItemData> Items;
	}
}
