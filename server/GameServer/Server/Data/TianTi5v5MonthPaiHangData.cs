using System;
using System.Collections.Generic;
using KF.Contract.Data;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class TianTi5v5MonthPaiHangData
	{
		
		[ProtoMember(1)]
		public TianTi5v5ZhanDuiData SelfPaiHangRoleData;

		
		[ProtoMember(2)]
		public List<TianTi5v5ZhanDuiData> PaiHangRoleDataList;
	}
}
