using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ModifyWanMotaData
	{
		
		[ProtoMember(1)]
		public string strParams;

		
		[ProtoMember(2)]
		public string strSweepReward;
	}
}
