using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000595 RID: 1429
	[ProtoContract]
	public class SpriteExtensionPropsHitedData
	{
		// Token: 0x04002843 RID: 10307
		[ProtoMember(1)]
		public int roleId;

		// Token: 0x04002844 RID: 10308
		[ProtoMember(2)]
		public int enemy;

		// Token: 0x04002845 RID: 10309
		[ProtoMember(3)]
		public int enemyX;

		// Token: 0x04002846 RID: 10310
		[ProtoMember(4)]
		public int enemyY;

		// Token: 0x04002847 RID: 10311
		[ProtoMember(5)]
		public int ExtensionPropID;
	}
}
