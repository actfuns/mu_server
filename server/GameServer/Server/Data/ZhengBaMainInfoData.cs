using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ZhengBaMainInfoData
	{
		
		[ProtoMember(1)]
		public int RealActDay;

		
		[ProtoMember(2)]
		public List<TianTiPaiHangRoleData> Top16List;

		
		[ProtoMember(3)]
		public int MaxSupportGroup;

		
		[ProtoMember(4)]
		public int MaxOpposeGroup;

		
		[ProtoMember(5, IsRequired = true)]
		public int CanGetAwardId;

		
		[ProtoMember(6, IsRequired = true)]
		public int RankResultOfDay;
	}
}
