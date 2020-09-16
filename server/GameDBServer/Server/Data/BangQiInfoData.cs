using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangQiInfoData
	{
		
		[ProtoMember(1)]
		public string BangQiName = "";

		
		[ProtoMember(2)]
		public int BangQiLevel = 0;

		
		[ProtoMember(3)]
		public Dictionary<int, BHLingDiOwnData> BHLingDiOwnDict = null;
	}
}
