using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class CompBattleScoreData
	{
		
		[ProtoMember(1)]
		public long Score1;

		
		[ProtoMember(2)]
		public long Score2;

		
		[ProtoMember(3)]
		public long Score3;

		
		[ProtoMember(4)]
		public long BossMaxLifeV;
	}
}
