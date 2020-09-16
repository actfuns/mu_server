using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class KuaFuBossSceneStateData
	{
		
		[ProtoMember(1)]
		public int BossNum;

		
		[ProtoMember(2)]
		public int TotalBossNum;

		
		[ProtoMember(3)]
		public int MonsterNum;

		
		[ProtoMember(4)]
		public int TotalNormalNum;
	}
}
