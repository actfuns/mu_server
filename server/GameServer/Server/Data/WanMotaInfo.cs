using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class WanMotaInfo
	{
		
		[ProtoMember(1)]
		public int nRoleID;

		
		[ProtoMember(2)]
		public string strRoleName;

		
		[ProtoMember(3)]
		public long lFlushTime;

		
		[ProtoMember(4)]
		public int nPassLayerCount;

		
		[ProtoMember(5)]
		public int nSweepLayer;

		
		[ProtoMember(6)]
		public string strSweepReward = "";

		
		[ProtoMember(7)]
		public long lSweepBeginTime;

		
		[ProtoMember(8)]
		public int nTopPassLayerCount;
	}
}
