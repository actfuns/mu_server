using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class EverydayActivityData
	{
		
		[ProtoMember(1)]
		public List<EverydayActInfo> EveryActInfoList;
	}
}
