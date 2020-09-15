using System;
using ProtoBuf;

namespace GameServer.Logic.UserReturn
{
	// Token: 0x020004B3 RID: 1203
	[ProtoContract]
	public class ReturnActivity
	{
		// Token: 0x04001FEF RID: 8175
		[ProtoMember(1)]
		public bool IsOpen;

		// Token: 0x04001FF0 RID: 8176
		[ProtoMember(2)]
		public DateTime NotLoggedInBegin;

		// Token: 0x04001FF1 RID: 8177
		[ProtoMember(3)]
		public DateTime NotLoggedInFinish;

		// Token: 0x04001FF2 RID: 8178
		[ProtoMember(4)]
		public int Level;

		// Token: 0x04001FF3 RID: 8179
		[ProtoMember(5)]
		public int VIPNeedExp;

		// Token: 0x04001FF4 RID: 8180
		[ProtoMember(6)]
		public int ActivityID;

		// Token: 0x04001FF5 RID: 8181
		[ProtoMember(7)]
		public string ActivityDay;
	}
}
