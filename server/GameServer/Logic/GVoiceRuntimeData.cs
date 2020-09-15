using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020007AB RID: 1963
	public class GVoiceRuntimeData
	{
		// Token: 0x04003F2C RID: 16172
		public object Mutex = new object();

		// Token: 0x04003F2D RID: 16173
		public int[] VoiceMessage;

		// Token: 0x04003F2E RID: 16174
		public int[] VoicePowerNum;

		// Token: 0x04003F2F RID: 16175
		public int VoiceMessageCD = 3000;

		// Token: 0x04003F30 RID: 16176
		[ProtoMember(2)]
		public string SDKGameID;

		// Token: 0x04003F31 RID: 16177
		[ProtoMember(3)]
		public string SDKKey;

		// Token: 0x04003F32 RID: 16178
		public Dictionary<int, string> ZhanMengGVoiceDict = new Dictionary<int, string>();

		// Token: 0x04003F33 RID: 16179
		public Dictionary<int, string> JunTuanGVoiceDict = new Dictionary<int, string>();

		// Token: 0x04003F34 RID: 16180
		public Dictionary<string, GVoiceSceneData> FuBenSeqID2RoomName = new Dictionary<string, GVoiceSceneData>();

		// Token: 0x04003F35 RID: 16181
		public Dictionary<int, int> MapCode2GVoiceTypeDict = new Dictionary<int, int>();

		// Token: 0x04003F36 RID: 16182
		public Dictionary<int, int> MapCode2GVoiceGroupDict = new Dictionary<int, int>();

		// Token: 0x04003F37 RID: 16183
		public HashSet<int> DestoryBhIds = new HashSet<int>();

		// Token: 0x04003F38 RID: 16184
		public long NextCheckExpireTicks;

		// Token: 0x04003F39 RID: 16185
		public long NextTicks1;

		// Token: 0x04003F3A RID: 16186
		public long NextTicks3;
	}
}
