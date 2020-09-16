using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LingDiCaiJiMainData
	{
		
		[ProtoMember(1)]
		public List<LingDiCaiJiData> LingDiCaiJiDataList;

		
		[ProtoMember(2)]
		public int LingDiCaiJiLeftCount;
	}
}
