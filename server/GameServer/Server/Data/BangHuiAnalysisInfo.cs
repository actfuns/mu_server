using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangHuiAnalysisInfo
	{
		
		[ProtoMember(1)]
		public List<int> listAnalysisData = new List<int>();
	}
}
