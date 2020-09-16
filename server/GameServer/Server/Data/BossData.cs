using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BossData
	{
		
		[ProtoMember(1)]
		public int MonsterID = 0;

		
		[ProtoMember(2)]
		public int ExtensionID = 0;

		
		[ProtoMember(3)]
		public string KillMonsterName = "";

		
		[ProtoMember(4)]
		public int KillerOnline = 0;

		
		[ProtoMember(5)]
		public string NextTime = "";
	}
}
