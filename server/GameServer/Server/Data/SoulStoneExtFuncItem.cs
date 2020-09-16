using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SoulStoneExtFuncItem
	{
		
		[ProtoMember(1)]
		public int FuncType;

		
		[ProtoMember(2)]
		public int CostType;
	}
}
