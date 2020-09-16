using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SingleChargeData
	{
		
		[ProtoMember(1)]
		public Dictionary<int, int> singleData = new Dictionary<int, int>();

		
		[ProtoMember(2)]
		public int YueKaMoney = 0;

		
		[ProtoMember(3)]
		public int ChargePlatType = 0;

		
		[ProtoMember(4)]
		public string SuperInputFanLiKey = "";

		
		[ProtoMember(5)]
		public Dictionary<int, JieriSuperInputData> SuperInputFanLiDict = new Dictionary<int, JieriSuperInputData>();

		
		[ProtoMember(6)]
		public Dictionary<int, int> MoneyVsChargeIDDict = new Dictionary<int, int>();

		
		[ProtoMember(7)]
		public int YueKaBangZuan;

		
		public Dictionary<int, int> ChargeIDVsMoneyDict = new Dictionary<int, int>();
	}
}
