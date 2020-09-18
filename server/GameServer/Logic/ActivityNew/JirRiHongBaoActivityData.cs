using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.ActivityNew
{
	
	[ProtoContract]
	public class JirRiHongBaoActivityData
	{
		
		[ProtoMember(1)]
		public List<JirRiHongBaoData> InfoList;

		
		[ProtoMember(2)]
		public long DataAge;
	}
}
