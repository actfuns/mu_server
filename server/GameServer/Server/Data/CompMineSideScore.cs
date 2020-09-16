using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class CompMineSideScore
	{
		
		[ProtoMember(1)]
		public int MineTruckGo;

		
		[ProtoMember(2)]
		public int SafeArrived;

		
		[ProtoMember(3)]
		public int MineTruckProcess;

		
		[ProtoMember(4)]
		public List<CompMineResData> ResJiFenList = new List<CompMineResData>();

		
		public int SuppliesNum;

		
		public int SuppliesStep;
	}
}
