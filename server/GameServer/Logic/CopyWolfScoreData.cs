using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class CopyWolfScoreData
	{
		
		[ProtoMember(1)]
		public int Wave = 0;

		
		[ProtoMember(2)]
		public long EndTime = 0L;

		
		[ProtoMember(3)]
		public int FortLifeNow = 0;

		
		[ProtoMember(4)]
		public int FortLifeMax = 0;

		
		[ProtoMember(5)]
		public Dictionary<int, int> RoleMonsterScore = new Dictionary<int, int>();

		
		[ProtoMember(6)]
		public int MonsterCount = 0;
	}
}
