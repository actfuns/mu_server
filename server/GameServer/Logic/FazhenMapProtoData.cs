using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class FazhenMapProtoData
	{
		
		[ProtoMember(1)]
		public int SrcMapCode = 0;

		
		[ProtoMember(2)]
		public List<FazhenTelegateProtoData> listTelegate = null;
	}
}
