using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SoulStoneQueryGetData
	{
		
		[ProtoMember(1)]
		public int CurrRandId;

		
		[ProtoMember(2)]
		public List<SoulStoneExtFuncItem> ExtFuncList;
	}
}
