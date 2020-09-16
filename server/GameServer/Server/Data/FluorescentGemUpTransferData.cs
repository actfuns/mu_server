using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FluorescentGemUpTransferData
	{
		
		[ProtoMember(1)]
		public int _RoleID;

		
		[ProtoMember(2)]
		public int _UpType;

		
		[ProtoMember(3)]
		public int _BagIndex;

		
		[ProtoMember(4)]
		public int _Position;

		
		[ProtoMember(5)]
		public int _GemType;

		
		[ProtoMember(6)]
		public Dictionary<int, int> _DecGoodsDict;
	}
}
