using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic
{
	
	public class GVoiceRuntimeData
	{
		
		public object Mutex = new object();

		
		public int[] VoiceMessage;

		
		public int[] VoicePowerNum;

		
		public int VoiceMessageCD = 3000;

		
		[ProtoMember(2)]
		public string SDKGameID;

		
		[ProtoMember(3)]
		public string SDKKey;

		
		public Dictionary<int, string> ZhanMengGVoiceDict = new Dictionary<int, string>();

		
		public Dictionary<int, string> JunTuanGVoiceDict = new Dictionary<int, string>();

		
		public Dictionary<string, GVoiceSceneData> FuBenSeqID2RoomName = new Dictionary<string, GVoiceSceneData>();

		
		public Dictionary<int, int> MapCode2GVoiceTypeDict = new Dictionary<int, int>();

		
		public Dictionary<int, int> MapCode2GVoiceGroupDict = new Dictionary<int, int>();

		
		public HashSet<int> DestoryBhIds = new HashSet<int>();

		
		public long NextCheckExpireTicks;

		
		public long NextTicks1;

		
		public long NextTicks3;
	}
}
