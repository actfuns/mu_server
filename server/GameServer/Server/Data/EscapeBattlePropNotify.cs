using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class EscapeBattlePropNotify
	{
		
		[ProtoMember(1)]
		public int Type;

		
		[ProtoMember(2)]
		public Dictionary<int, double[]> MergeProp;
	}
}
