using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000B5 RID: 181
	[ProtoContract]
	public class TalentEffectInfo
	{
		// Token: 0x040004CF RID: 1231
		[ProtoMember(1, IsRequired = true)]
		public int EffectType = 0;

		// Token: 0x040004D0 RID: 1232
		[ProtoMember(2, IsRequired = true)]
		public int EffectID = 0;

		// Token: 0x040004D1 RID: 1233
		[ProtoMember(3, IsRequired = true)]
		public double EffectValue = 0.0;
	}
}
