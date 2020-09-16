using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FluorescentGemEquipChangesTransferData
	{
		
		[ProtoMember(1)]
		public int _Position;

		
		[ProtoMember(2)]
		public int _GemType;

		
		[ProtoMember(3)]
		public GoodsData _GoodsData = null;
	}
}
