using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class GVoiceSceneData
	{
		
		[ProtoMember(2)]
		public string SDKGameID;

		
		[ProtoMember(3)]
		public string SDKKey;

		
		[ProtoMember(4)]
		public string RoomName;
	}
}
