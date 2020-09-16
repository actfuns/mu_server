using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LingZhuShouWeiData
	{
		
		[ProtoMember(1)]
		public int Result;

		
		[ProtoMember(2)]
		public List<LingDiShouWeiData> ShouWeiList;
	}
}
