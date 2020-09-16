using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RebornBossScoreData
	{
		
		[ProtoMember(1)]
		public int LeftLifePct;

		
		[ProtoMember(2)]
		public List<RebornBossAttackLog> rankList = new List<RebornBossAttackLog>();

		
		[ProtoMember(3)]
		public int SelfRankNum;

		
		[ProtoMember(4)]
		public int SelfDamagePct;

		
		[ProtoMember(5)]
		public string NextTime = "";

		
		public int BossExtensionID;

		
		public DateTime BossRefreshTime = DateTime.MinValue;

		
		public DateTime BossBeAttackTm;

		
		public double VLife;

		
		public double VLifeMax;

		
		public int MonsterID;
	}
}
