using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace Server.Data
{
	
	[ProtoContract]
	public class CompMineSelfScore
	{
		
		[ProtoMember(1)]
		public int RankNum;

		
		[ProtoMember(2)]
		public int AwardID;

		
		[ProtoMember(3)]
		public List<KFCompRankInfo> rankInfo2Client = new List<KFCompRankInfo>();
	}
}
