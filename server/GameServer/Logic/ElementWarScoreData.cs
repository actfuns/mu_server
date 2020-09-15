using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x020002B4 RID: 692
	[ProtoContract]
	public class ElementWarScoreData
	{
		// Token: 0x040011AB RID: 4523
		[ProtoMember(1)]
		public int Wave = 0;

		// Token: 0x040011AC RID: 4524
		[ProtoMember(2)]
		public long EndTime = 0L;

		// Token: 0x040011AD RID: 4525
		[ProtoMember(3)]
		public long MonsterCount = 0L;
	}
}
