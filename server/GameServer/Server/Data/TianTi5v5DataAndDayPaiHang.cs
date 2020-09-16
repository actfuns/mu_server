using System;
using System.Collections.Generic;
using KF.Contract.Data;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class TianTi5v5DataAndDayPaiHang
	{
		
		[ProtoMember(1)]
		public TianTi5v5ZhanDuiData TianTi5v5Data;

		
		[ProtoMember(2)]
		public List<TianTi5v5ZhanDuiData> PaiHangRoleDataList;

		
		[ProtoMember(3)]
		public int HaveMonthPaiHangAwards;

		
		[ProtoMember(4)]
		public int TodayFightCount;
	}
}
