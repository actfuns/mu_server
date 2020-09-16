using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class TianTiDataAndDayPaiHang
	{
		
		[ProtoMember(1)]
		public RoleTianTiData TianTiData;

		
		[ProtoMember(2)]
		public List<TianTiPaiHangRoleData> PaiHangRoleDataList;
	}
}
