using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class YaoSaiBossData
	{
		
		[ProtoMember(1)]
		public int BossID;

		
		[ProtoMember(2)]
		public double LifeV;

		
		[ProtoMember(3)]
		public DateTime DeadTime;

		
		[ProtoMember(4)]
		public int OwnerID;
	}
}
