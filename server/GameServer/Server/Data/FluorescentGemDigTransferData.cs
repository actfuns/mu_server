using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FluorescentGemDigTransferData
	{
		
		[ProtoMember(1)]
		public int _Result;

		
		[ProtoMember(2)]
		public List<int> _GemList;

		
		[ProtoMember(3)]
		public Dictionary<int, int> _GemNumDict;
	}
}
