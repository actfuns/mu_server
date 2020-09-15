using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200040B RID: 1035
	[ProtoContract]
	public class ShenQiData
	{
		// Token: 0x04001B8B RID: 7051
		[ProtoMember(1)]
		public int ShenQiID;

		// Token: 0x04001B8C RID: 7052
		[ProtoMember(2)]
		public int LifeAdd;

		// Token: 0x04001B8D RID: 7053
		[ProtoMember(3)]
		public int AttackAdd;

		// Token: 0x04001B8E RID: 7054
		[ProtoMember(4)]
		public int DefenseAdd;

		// Token: 0x04001B8F RID: 7055
		[ProtoMember(5)]
		public int ToughnessAdd;

		// Token: 0x04001B90 RID: 7056
		[ProtoMember(6)]
		public int BurstType;

		// Token: 0x04001B91 RID: 7057
		[ProtoMember(7)]
		public int UpResultType;

		// Token: 0x04001B92 RID: 7058
		[ProtoMember(8)]
		public int ShenLiJingHuaLeft;
	}
}
