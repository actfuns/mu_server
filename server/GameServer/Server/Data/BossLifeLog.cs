using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BossLifeLog
	{
		
		[ProtoMember(1)]
		public long InjureSum;

		
		[ProtoMember(2)]
		public List<BHAttackLog> BHAttackRank;

		
		[ProtoMember(3)]
		public BHAttackLog SelfBHAttack;
	}
}
