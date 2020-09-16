using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RebornBossData
	{
		
		[ProtoMember(1)]
		public int ExtensionID = 0;

		
		[ProtoMember(2)]
		public string NextTime = "";

		
		[ProtoMember(3)]
		public int AwardExtensionID = 0;

		
		[ProtoMember(4)]
		public int RankNum = 0;

		
		[ProtoMember(5)]
		public int BossKill = 0;
	}
}
