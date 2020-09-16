using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PaiHangData
	{
		
		[ProtoMember(1)]
		public int PaiHangType;

		
		[ProtoMember(2)]
		public List<PaiHangItemData> PaiHangList = null;
	}
}
