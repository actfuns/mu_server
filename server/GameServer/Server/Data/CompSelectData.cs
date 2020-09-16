using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class CompSelectData
	{
		
		[ProtoMember(1)]
		public List<int> RecommendCompList = new List<int>();

		
		[ProtoMember(2)]
		public List<string> DaLingZhuNameList = new List<string>();
	}
}
