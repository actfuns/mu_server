using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FluorescentGemSaveDBData
	{
		
		[ProtoMember(1)]
		public int _RoleID;

		
		[ProtoMember(2)]
		public int _GoodsID;

		
		[ProtoMember(3)]
		public int _Position;

		
		[ProtoMember(4)]
		public int _GemType;

		
		[ProtoMember(5)]
		public int _Bind;
	}
}
