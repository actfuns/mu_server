using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200081B RID: 2075
	[ProtoContract]
	public class YongZheZhanChangScoreData
	{
		// Token: 0x040044B8 RID: 17592
		[ProtoMember(1)]
		public int Score1;

		// Token: 0x040044B9 RID: 17593
		[ProtoMember(2)]
		public int Score2;
	}
}
