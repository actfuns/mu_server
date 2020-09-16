using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class KaiFuActivityData
	{
		
		[ProtoMember(1)]
		public int[] LevelUpAwardRemainQuota;

		
		[ProtoMember(2)]
		public int LevelUpGetAwardState;

		
		[ProtoMember(3)]
		public int KillBossNum;
	}
}
