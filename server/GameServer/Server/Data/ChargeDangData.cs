using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ChargeDangData
	{
		
		[ProtoMember(1)]
		public List<SingleChargeData> chargeData = new List<SingleChargeData>();
	}
}
