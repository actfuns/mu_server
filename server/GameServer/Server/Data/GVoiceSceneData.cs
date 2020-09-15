using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020007A7 RID: 1959
	[ProtoContract]
	public class GVoiceSceneData
	{
		// Token: 0x04003F1B RID: 16155
		[ProtoMember(2)]
		public string SDKGameID;

		// Token: 0x04003F1C RID: 16156
		[ProtoMember(3)]
		public string SDKKey;

		// Token: 0x04003F1D RID: 16157
		[ProtoMember(4)]
		public string RoomName;
	}
}
