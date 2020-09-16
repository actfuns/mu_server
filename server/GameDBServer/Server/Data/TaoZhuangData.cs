using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class TaoZhuangData
	{
		
		[ProtoMember(1)]
		public int ID;

		
		[ProtoMember(2)]
		public List<int> ActiviteList;
	}
}
