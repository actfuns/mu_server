using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class TianTiMonthPaiHangData
	{
		
		[ProtoMember(1)]
		public TianTiPaiHangRoleData SelfPaiHangRoleData;

		
		[ProtoMember(2)]
		public List<TianTiPaiHangRoleData> PaiHangRoleDataList;
	}
}
