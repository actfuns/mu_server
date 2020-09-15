using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200032A RID: 810
	[ProtoContract]
	public class KarenBattleScoreData
	{
		// Token: 0x040014E7 RID: 5351
		[ProtoMember(1)]
		public int LegionID;

		// Token: 0x040014E8 RID: 5352
		[ProtoMember(2)]
		public string Name;

		// Token: 0x040014E9 RID: 5353
		[ProtoMember(3)]
		public int Score;

		// Token: 0x040014EA RID: 5354
		[ProtoMember(4)]
		public List<int> ResourceList = new List<int>();

		// Token: 0x040014EB RID: 5355
		[ProtoMember(5)]
		public long ticks;
	}
}
