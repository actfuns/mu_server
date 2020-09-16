using System;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class CopyWolfAwardsData
	{
		
		[ProtoMember(1)]
		public long Exp;

		
		[ProtoMember(2)]
		public int Money;

		
		[ProtoMember(3)]
		public int WolfMoney;

		
		[ProtoMember(4)]
		public int Wave;

		
		[ProtoMember(5)]
		public int RoleScore = 0;
	}
}
