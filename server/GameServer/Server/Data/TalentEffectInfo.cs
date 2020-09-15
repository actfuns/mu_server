using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200018E RID: 398
	[ProtoContract]
	public class TalentEffectInfo
	{
		// Token: 0x040008CD RID: 2253
		[ProtoMember(1, IsRequired = true)]
		public int EffectType = 0;

		// Token: 0x040008CE RID: 2254
		[ProtoMember(2, IsRequired = true)]
		public int EffectID = 0;

		// Token: 0x040008CF RID: 2255
		[ProtoMember(3, IsRequired = true)]
		public double EffectValue = 0.0;
	}
}
