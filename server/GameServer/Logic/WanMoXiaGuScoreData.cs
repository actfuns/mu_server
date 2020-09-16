using System;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class WanMoXiaGuScoreData
	{
		
		[ProtoMember(1)]
		public double BossLifePercent;

		
		[ProtoMember(2)]
		public int MonsterID;

		
		[ProtoMember(3)]
		public int MonsterCount;

		
		[ProtoMember(4)]
		public int Decorations;

		
		[ProtoMember(5)]
		public string Intro;
	}
}
